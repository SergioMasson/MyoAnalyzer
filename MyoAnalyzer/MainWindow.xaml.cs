using ComunicadorSerial_Arduino;
using DataClasses;
using MyoSharp.Communication;
using MyoSharp.Device;
using PlotForm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MyoAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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

        private List<EmgTrainData> Pose1RawData;
        private List<EmgTrainData> Pose2RawData;

        // Machi learning variables
        private double[][] DataTraining;

        // Class to perform the trainning
        private ITrainner Trainner;

        //Arduino
        private USBConnectInterface _connectClick;

        public bool[] ChannalsToTrain { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _connectClick = new USBConnectInterface();

            Channel1CheckBox.DataContext = this;
            Channel2CheckBox.DataContext = this;
            Channel3CheckBox.DataContext = this;
            Channel4CheckBox.DataContext = this;
            Channel5CheckBox.DataContext = this;
            Channel6CheckBox.DataContext = this;
            Channel7CheckBox.DataContext = this;
            Channel8CheckBox.DataContext = this;

            Pose1RawData = new List<EmgTrainData>();
            Pose2RawData = new List<EmgTrainData>();

            ChannalsToTrain = new bool[8];

            State = AppState.Started;
            EmgDataToSave = new List<string>();

            _dataColected = new List<int[]>();

            // get set up to listen for Myo events
            _channel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create()));

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

                Plot PlotForm = new Plot();

                PlotForm.Show();

                PlotForm.SendDataToPlot(_dataColected);

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
                EmgDataToSave.FormatData(writer);
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

        private void LoadData1Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();

            open.Multiselect = true;

            if (open.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            foreach (string fileName in open.FileNames)
            {
                EmgTrainData finalData = new EmgTrainData();

                List<double[]> model = new List<double[]>();

                string[] lines = System.IO.File.ReadAllLines(fileName);

                foreach (string line in lines)
                {
                    string[] datas = line.Split('\t');

                    double[] dData = new double[datas.Length];

                    for (int i = 0; i < datas.Length; i++)
                    {
                        dData[i] = Convert.ToDouble(datas[i]);
                    }

                    model.Add(dData);
                }

                finalData.AquisitionData = model;

                Pose1RawData.Add(finalData);
            }

            NumberPose1Samples.Text = Pose1RawData.Count.ToString();
        }

        private void LoadData2Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();

            open.Multiselect = true;

            if (open.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            foreach (string fileName in open.FileNames)
            {
                EmgTrainData finalData = new EmgTrainData();

                List<double[]> model = new List<double[]>();

                string[] lines = System.IO.File.ReadAllLines(fileName);

                foreach (string line in lines)
                {
                    string[] datas = line.Split('\t');

                    double[] dData = new double[datas.Length];

                    for (int i = 0; i < datas.Length; i++)
                    {
                        dData[i] = Convert.ToDouble(datas[i]);
                    }

                    model.Add(dData);
                }

                finalData.AquisitionData = model;

                Pose2RawData.Add(finalData);
            }

            NumberPose2Samples.Text = Pose2RawData.Count.ToString();
        }

        private void ResetTraining_Click(object sender, RoutedEventArgs e)
        {
            Pose1RawData = new List<EmgTrainData>();
            Pose2RawData = new List<EmgTrainData>();

            EmgDataToSave = new List<string>();

            DisplayText.AppendText("\nAll training data removed!");
            DisplayText.ScrollToEnd();

            NumberPose1Samples.Text = Pose1RawData.Count.ToString();
            NumberPose2Samples.Text = Pose2RawData.Count.ToString();
        }

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {

            Trainner = new KSVMTrainner();

            Trainner.Train(Pose1RawData, Pose2RawData, ChannalsToTrain);                       
        }

        private void USB_Connect_Click(object sender, RoutedEventArgs e)
        {
            _connectClick = new USBConnectInterface();

            _connectClick.Show();
        }

        private void TestClassification_Click(object sender, RoutedEventArgs e)
        {

            if (_hub.Myos.Count == 0)
            {
                ErroMassege error = new ErroMassege("\n No Myo device found! please connect one and try again.");
                error.Show();
                return;
            }

            if (Trainner == null)
            {
                ErroMassege error = new ErroMassege("\n You need create a classificator first!");
                error.Show();
                return;
            }

            if (!Trainner.IsTrainned())
            {
                ErroMassege error = new ErroMassege("\n You need to train you classificator first in order to teste it!");
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

            State = AppState.Waiting;
            TestClassificationButton.Content = "Start Test";

            int result = Trainner.Classify(_dataColected, 1, 3);

            DisplayText.AppendText("\n No USB device detected! But the result was: " + result.ToString());
            DisplayText.ScrollToEnd();

            try
            {
                _connectClick.SendDataToUsb(result.ToString());
            }
            catch (Exception)
            {
                DisplayText.AppendText("\n No USB device detected! But the result was: " + result.ToString()); ;
            }
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
                ErroMassege error = new ErroMassege("\n You need to train you classificator first in order to teste it!");
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