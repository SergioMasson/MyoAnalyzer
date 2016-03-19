using ComunicadorSerial_Arduino;
using MyoSharp.Communication;
using MyoSharp.Device;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DynamicPlotWPF;
using MyoAnalyzer.Classification;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;
using MyoAnalyzer.XAML_blocks;

namespace MyoAnalyzer
{
    /// <summary>
    /// Interaction logic for PlotDynamicWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int NUMBER_OF_SENSORS = 8;
        private readonly IChannel _channel;
        private readonly IHub _hub;
        private DateTime StartTime;

        private AppState State;
        private TrainMethods DefaultTrain;

        private List<string> EmgDataToSave;
        private List<int[]> _dataColected;

        // Machi learning variables
        private double[][] DataTraining;

        // Class to perform the trainning
        private ITrainner Trainner;

        // Arduino
        private USBConnectInterface _connectClick;       

        public bool[] ChannalsToTrain { get; set; }

        private bool IsStraeming;

        public MainWindow()
        {
            InitializeComponent();

            _connectClick = new USBConnectInterface();

            IsStraeming = false;

            GesturesComboBox.Items.Add("Open");
            GesturesComboBox.Items.Add("Close");
            GesturesComboBox.Items.Add("Rock`n Roll");
            GesturesComboBox.Items.Add("Like");
            GesturesComboBox.Items.Add("One");

            Channel1CheckBox.DataContext = this;
            Channel2CheckBox.DataContext = this;
            Channel3CheckBox.DataContext = this;
            Channel4CheckBox.DataContext = this;
            Channel5CheckBox.DataContext = this;
            Channel6CheckBox.DataContext = this;
            Channel7CheckBox.DataContext = this;
            Channel8CheckBox.DataContext = this;

            ChannalsToTrain = new bool[8];

            State = AppState.Started;
            EmgDataToSave = new List<string>();

            _dataColected = new List<int[]>();

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
            if (State == AppState.Acquiring)
            {
                string data = "";

                int[] datasColected = new int[9];               

                // pull _dataColected from each sensor
                for (var i = 0; i < NUMBER_OF_SENSORS; ++i)
                {
                    datasColected[i] = e.EmgData.GetDataForSensor(i);
                    data = data + e.EmgData.GetDataForSensor(i) + " \t ";
                }

                datasColected[8] = (int)(e.Timestamp - StartTime).TotalMilliseconds;

                //TODO: Fazer interpolação
                if (_dataColected.Count > 2)
                {
                    if (_dataColected?.Last()?[8] == datasColected[8])
                        return;
                }              

                _dataColected.Add(datasColected);
                EmgDataToSave.Add(data + (int)(e.Timestamp - StartTime).TotalMilliseconds);             
                   
            }                    
        }

        #endregion Myo EventHandlers

        #region ButtonClicks

        private void GetDataClick(object sender, RoutedEventArgs e)
        {
            if (State == AppState.Acquiring)
            {
                State = AppState.Waiting;

                StaticMultiLinesPlotWindow DataPlot = new StaticMultiLinesPlotWindow(_dataColected);

                DataPlot.Show();

                GetDataButton.Content = "Get Data";

                return;
            }

            State = AppState.Acquiring;
            StartTime = DateTime.UtcNow;
            _dataColected = new List<int[]>();
            DisplayText.AppendText("\n You are now recording Emg Data from Myo");
            DisplayText.ScrollToEnd();
            GetDataButton.Content = "Stop Acquiring";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.FileName = "dados.txt";
            save.Filter = "Text File | *.txt";

            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                EmgDataToSave.RemoveDuplicates(writer);
                writer.Dispose();
                writer.Close();
            }
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

            Trainner = new KSVMMultipleTrainner();

            var error = Trainner.Train(posesToTrain, ChannalsToTrain);

            DisplayText.AppendText("\nTrainning sucsseded! The total error for your classification was:" + error + " % ");
            DisplayText.ScrollToEnd();
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
                XAML_blocks.ErroMassege error = new XAML_blocks.ErroMassege("\n You need create a classificator first!");
                error.Show();
                return;
            }

            if (!Trainner.IsTrainned())
            {
                XAML_blocks.ErroMassege error = new XAML_blocks.ErroMassege("\n You need to train you classificator first in order to teste it!");
                error.Show();
                return;
            }

            ClassificationTestWindow testWindow = new ClassificationTestWindow(Trainner);

            testWindow.Show();

            //if (State != AppState.Acquiring)
            //{
            //    _dataColected = new List<int[]>();
            //    State = AppState.Acquiring;
            //    TestClassificationButton.Content = " Finish ";
            //    return;
            //}

            //State = AppState.Waiting;
            //TestClassificationButton.Content = "Start Test";

            //Gestures result = Trainner.Classify(_dataColected);

            //DisplayText.Clear();
            //DisplayText.AppendText("No USB device detected! But the result was: " + result);
            //DisplayText.ScrollToEnd();

            //DisplayText.AppendText("\n No USB device detected! But the result was: " + result);

            //_connectClick.SendDataToUsb(((int)result).ToString());


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
                _dataColected = new List<int[]>();
                State = AppState.Acquiring;
                TestClassificationButton.Content = " Finish ";
                return;
            }
        }

        private void RankAttributes_Click(object sender, RoutedEventArgs e)
        {
            List<Pose> posesToRankAttributes = (from XAML_blocks.GesturePanel pose in GesturesPanel.Children select pose.Pose).ToList();

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
            //TODO:Implement this
            IsStraeming = true;
        }

        #endregion ButtonClicks

        private void TrainMethodChanges(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedValue.ToString();

            switch (value)
            {
                case "Kernel":
                    DefaultTrain = TrainMethods.Kernel;
                    DisplayText.AppendText("Training method set to Kernel");
                    break;

                case "Linear SVM":
                    DefaultTrain = TrainMethods.LinearSVM;
                    break;

                case "Neural Networks":
                    DefaultTrain = TrainMethods.NeuralNetworks;
                    break;
            }
        }
      
    }
}