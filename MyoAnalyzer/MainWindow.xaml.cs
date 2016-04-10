using ComunicadorSerial_Arduino;
using DynamicPlotWPF;
using MyoAnalyzer.Classification;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;
using MyoAnalyzer.XAML_blocks;
using MyoSharp.Communication;
using MyoSharp.Device;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyoAnalyzer
{
  

    public partial class MainWindow
    {
        private const int NUMBER_OF_SENSORS = 8;
        private readonly IChannel _channel;
        private readonly IHub _hub;
        private DateTime StartTime;

        private AppState State;
        private StreamStatus _streamStatus;

        private List<string> EmgDataToSave;
        private List<double[]> _dataColected;
      
        private double[][] DataTraining;
       
        private ITrainner Trainner;
     
        private USBConnectInterface _connectClick;

        public bool[] ChannalsToTrain { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _connectClick = new USBConnectInterface();           

            GesturesComboBox.Items.Add("Open");
            GesturesComboBox.Items.Add("Close");
            GesturesComboBox.Items.Add("Rock`n Roll");
            GesturesComboBox.Items.Add("Like");
            GesturesComboBox.Items.Add("One");
            GesturesComboBox.SelectedIndex = 0;

            Channel1CheckBox.DataContext = this;
            Channel2CheckBox.DataContext = this;
            Channel3CheckBox.DataContext = this;
            Channel4CheckBox.DataContext = this;
            Channel5CheckBox.DataContext = this;
            Channel6CheckBox.DataContext = this;
            Channel7CheckBox.DataContext = this;
            Channel8CheckBox.DataContext = this;

            ChannalsToTrain = new bool[8];

            for (int i = 0; i < ChannalsToTrain.Length; i++)
            {
                ChannalsToTrain[i] = true;
            }

            State = AppState.Started;
            _streamStatus = StreamStatus.None;

            EmgDataToSave = new List<string>();

            _dataColected = new List<double[]>();

            // get set up to listen for Myo events

            try
            {
                _channel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create()));
            }
            catch (Exception e)
            {
                ErroMassege noMyoMassege = new ErroMassege("Myo Connect not found");
                noMyoMassege.Show();
                return;
            }

            _hub = Hub.Create(_channel);
            _hub.MyoConnected += Hub_MyoConnected;
            _hub.MyoDisconnected += Hub_MyoDisconnected;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _channel.StartListening();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _channel.Dispose();
            _hub.Dispose();
        }

        #region Myo EventHandlers

        private void Hub_MyoDisconnected(object sender, MyoEventArgs e)
        {
            e.Myo.EmgDataAcquired -= Myo_EmgDataAcquired;
        }

        private void Hub_MyoConnected(object sender, MyoEventArgs e)
        {
            e.Myo.Vibrate(VibrationType.Long);
            e.Myo.EmgDataAcquired += Myo_EmgDataAcquired;
            e.Myo.SetEmgStreaming(true);
        }

        private void Myo_EmgDataAcquired(object sender, EmgDataEventArgs e)
        {
            if (State != AppState.Acquiring)
            {
                return;
            }

            string data = "";

            double[] datasColected = new double[9];

            // pull _dataColected from each sensor
            for (var i = 0; i < NUMBER_OF_SENSORS; ++i)
            {
                datasColected[i] = e.EmgData.GetDataForSensor(i);
                data = data + e.EmgData.GetDataForSensor(i) + " \t ";
            }

            datasColected[8] = (e.Timestamp - StartTime).TotalMilliseconds;

            //TODO: Fazer interpolação
            if (_dataColected.Count > 2)
            {
                if (_dataColected?.Last()?[8] == datasColected[8])
                    return;
            }

            _dataColected.Add(datasColected);
            EmgDataToSave.Add(data + (int)(e.Timestamp - StartTime).TotalMilliseconds);
        }

        #endregion Myo EventHandlers

        #region ButtonClicks

        private void GetDataClick(object sender, RoutedEventArgs e)
        {
            if (State == AppState.Acquiring)
            {
                State = AppState.Waiting;

                StaticMultiLinesPlotWindow dataPlot = new StaticMultiLinesPlotWindow(_dataColected);

                dataPlot.Show();

                GetDataButton.Content = "Get Data";

                return;
            }

            State = AppState.Acquiring;
            StartTime = DateTime.UtcNow;
            _dataColected = new List<double[]>();
            EmgDataToSave = new List<string>();
            DisplayText.AppendText("\n You are now recording Emg Data from Myo");
            DisplayText.ScrollToEnd();
            GetDataButton.Content = "Stop Acquiring";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var save = new System.Windows.Forms.SaveFileDialog
            {
                FileName = Properties.Resources.DefaulTextToSave,
                Filter = Properties.Resources.MainWindow_SaveButton_Click_Text_File
            };

            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                EmgDataToSave.RemoveDuplicates(writer);
                writer.Dispose();
                writer.Close();
            }

            EmgDataToSave = new List<string>();
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            EmgDataToSave = new List<string>();
            DisplayText.AppendText("\n Data contet has been removed!");
            DisplayText.ScrollToEnd();
        }

        private void ResetTraining_Click(object sender, RoutedEventArgs e)
        {
            EmgDataToSave = new List<string>();

            DisplayText.AppendText("\nAll training data removed!");
            DisplayText.ScrollToEnd();
        }

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            List<Pose> posesToTrain = (from XAML_blocks.GesturePanel pose in GesturesPanel.Children select pose.Pose).ToList();

            Trainner = new KVizinhosTrainner();

            var error = Trainner.Train(posesToTrain, ChannalsToTrain);

            DisplayText.AppendText("\nTrainning sucsseded! The total error for your classification was: " + error + " % ");
            DisplayText.ScrollToEnd();

            LiveButton.Visibility = Visibility.Visible;
        }

        private void USB_Connect_Click(object sender, RoutedEventArgs e)
        {
            _connectClick = new USBConnectInterface();

            _connectClick.Show();
        }

        private void TestClassification_Click(object sender, RoutedEventArgs e)
        {
            if (Trainner == null)
            {
                XAML_blocks.ErroMassege error = new XAML_blocks.ErroMassege("\n You need to create a classificator first in order to teste it!!");
                error.Show();
                return;
            }

            if (!Trainner.IsTrainned())
            {
                XAML_blocks.ErroMassege error = new XAML_blocks.ErroMassege("\n You need to train you classificator first in order to teste it!");
                error.Show();
                return;
            }

            List<Pose> posesToTrain = (from XAML_blocks.GesturePanel pose in GesturesPanel.Children select pose.Pose).ToList();

            ClassificationTestWindow testWindow = new ClassificationTestWindow(Trainner, posesToTrain);

            testWindow.Show();
        }

        private void PlotTest_Click(object sender, RoutedEventArgs e)
        {
        }

        private void GoLive_Click(object sender, RoutedEventArgs e)
        {
            if (_hub.Myos.Count == 0)
            {
                DisplayText.AppendText("\n No Myo device found! please connect one and try again.");
                return;
            }

            if (Trainner == null)
            {
                DisplayText.AppendText("\n You need to train you classificator first in order to do live!");
                return;
            }

            if (!Trainner.IsTrainned())
            {
                XAML_blocks.ErroMassege error = new XAML_blocks.ErroMassege("\n You need to train you classificator first in order to teste it!");
                error.Show();
                return;
            }

            if (State != AppState.Acquiring)
            {
                _dataColected = new List<double[]>();
                State = AppState.Acquiring;
                StartTime = DateTime.UtcNow;
                TryPoseButton.Content = "Finish";
                return;
            }

            var result = Trainner.Classify(_dataColected);

            DisplayText.AppendText("\n You just did a " + result);
            DisplayText.ScrollToEnd();

            if (_connectClick != null)
            {
                if (_connectClick.IsStreamReady())
                {
                    _connectClick.SendDataToUsb(((int)result).ToString());
                }
            }

            TryPoseButton.Content = "Go Live";

            State = AppState.Waiting;
        }

        private void RankAttributes_Click(object sender, RoutedEventArgs e)
        {            
            List<Pose> posesToRankAttributes = (from XAML_blocks.GesturePanel pose in GesturesPanel.Children select pose.Pose).ToList();

            if (!posesToRankAttributes.Any(a => a.TotalPoseData.Any()))
            {
                XAML_blocks.ErroMassege error = new XAML_blocks.ErroMassege("\n You must load at least one data sample in order to use the Rank Attribute menu!");
                error.Show();
                return;
            }

            AttributeRankWindow RankWindow = new AttributeRankWindow(posesToRankAttributes);
            RankWindow.Show();
        }

        private void AddGesture_Click(object sender, RoutedEventArgs e)
        {         
            XAML_blocks.GesturePanel newGesture = new XAML_blocks.GesturePanel(GesturesComboBox.Items[GesturesComboBox.SelectedIndex].ToString());

            GesturesPanel.Children.Add(newGesture);

            if (GesturesPanel.Children.Count > 1 && RankDataButton.Visibility == Visibility.Hidden)
            {
                RankDataButton.Visibility = Visibility.Visible;
            }
        }

        private void CleanGestures_Click(object sender, RoutedEventArgs e)
        {
            GesturesPanel.Children.Clear();
            RankDataButton.Visibility = Visibility.Hidden;
        }

        private void StarLiveDataStreaming_Click(object sender, RoutedEventArgs e)
        {          

        }

        private async void TrainAndTest_Click(object sender, RoutedEventArgs e)
        {
            TrainAndTestInputWindow trainAndTestWindow = new TrainAndTestInputWindow();

            trainAndTestWindow.Show();

            int[] data = await trainAndTestWindow.DataDone();

            double percentageToTrain = data[0] / 100.0;

            double numberOfRepetitions = data[1];

            List<Pose> totalPoseData = (from XAML_blocks.GesturePanel pose in GesturesPanel.Children select pose.Pose).ToList();

            ResultWindow resultWindow = new ResultWindow(totalPoseData, percentageToTrain, numberOfRepetitions, ChannalsToTrain);

            resultWindow.Show();

            DisplayText.AppendText("\nTrainning sucsseded!");
            DisplayText.ScrollToEnd();
        }

        private async void LiveStreaming_Click(object sender, RoutedEventArgs e)
        {
            if (_streamStatus != StreamStatus.Streaming)
            {                
                StopLive.Visibility = Visibility.Visible;
                LiveButton.Visibility = Visibility.Hidden;

                _streamStatus = StreamStatus.Streaming;

                DisplayText.Text = string.Empty;

                DisplayText.AppendText("Streaming Started! ");

                await AwaitForTreamingToStop();

                DisplayText.Text = string.Empty;

                DisplayText.AppendText("You just stoped Streaming");

            }          
        }

        private void StopLive_Click(object sender, RoutedEventArgs e)
        {
            if (_streamStatus == StreamStatus.Streaming)
            {
                _streamStatus = StreamStatus.Awayting;
                StopLive.Visibility = Visibility.Hidden;
                LiveButton.Visibility = Visibility.Visible;               
            }
        }

        #endregion ButtonClicks

        private Task<List<double[]>> GetDataToEvaluteLive()
        {
            return Task<List<double[]>>.Factory.StartNew(WaitingForEmgData);
        }

        private List<double[]> WaitingForEmgData()
        {
            int WindowSize = 100;

            _dataColected = new List<double[]>();
            StartTime = DateTime.UtcNow;

            State = AppState.Acquiring;

            while (_dataColected.Count <= WindowSize)
            {
                
            }

            return _dataColected;
        }       

        private async Task AwaitForTreamingToStop()
        {
            var previewsResult = Gestures.None;

            while (_streamStatus == StreamStatus.Streaming)
            {
                List<double[]> dataToEvaluate = await GetDataToEvaluteLive();

                Gestures result = Trainner.Classify(dataToEvaluate);

                DisplayText.AppendText("\n You just did a " + result);

                DisplayText.ScrollToEnd();

                if (result != previewsResult)
                {
                    previewsResult = result;

                    if (_connectClick != null)
                    {
                        if (_connectClick.IsStreamReady())
                        {
                            _connectClick.SendDataToUsb(((int)result).ToString());
                        }
                    }
                }
            }
            _connectClick?.Close();
        }      
    }
}