using Accord.Controls;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using ComunicadorSerial_Arduino;
using DataClasses;
using MyoSharp.Communication;
using MyoSharp.Device;
using PlotForm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private KernelSupportVectorMachine SVM;

        public bool[] ChannalsToTrain { get; set; }

        //Arduino
        private USBConnectInterface _connectClick;

        private int trials = 0;

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

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            // start listening for Myo _dataColected
            _channel.StartListening();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _channel.Dispose();
            _hub.Dispose();
        }

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
            DataTraining = ExtractFeatures(Pose1RawData, Pose2RawData);

            IEnumerable<int> outputs1 = Enumerable.Repeat(-1, Pose1RawData.Count);
            IEnumerable<int> outputs2 = Enumerable.Repeat(1, Pose2RawData.Count);

            int[] totalOutput = outputs1.Concat(outputs2).ToArray<int>();

            // Estimate the kernel from the data
            var gaussian = Gaussian.Estimate(DataTraining);

            // Create a Gaussian binary support machine with 2 inputs
            SVM = new KernelSupportVectorMachine(gaussian, ChannalsToTrain.Count(a => a));

            // Create a new Sequential Minimal Optimization (SMO) learning
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimization(SVM, DataTraining, totalOutput)
            {
                UseComplexityHeuristic = true
            };

            // Teach the vector machine
            double error = teacher.Run();

            // Classify the samples using the model
            int[] answers = DataTraining.Apply(SVM.Compute).Apply(System.Math.Sign);

            // Plot the results
            if (ChannalsToTrain.Count(a => a) != 2)
            {
                DisplayText.AppendText("\n Result can not be shown os a 2D Plot");
                return;
            }

            ScatterplotBox.Show("Expected results", DataTraining, totalOutput);
            ScatterplotBox.Show("GaussianSVM results", DataTraining, answers);
        }

        private void USB_Connect_Click(object sender, RoutedEventArgs e)
        {
            _connectClick = new USBConnectInterface();

            _connectClick.Show();
        }

        private void Live_Click(object sender, RoutedEventArgs e)
        {
            var data = ExtractFeatures(_dataColected);

            int[] answers = data.Apply(SVM.Compute).Apply(System.Math.Sign);

            _connectClick.SendDataToUsb(answers[0] == -1 ? "1" : "3");
        }

        #endregion ButtonClicks

        private double[][] ExtractFeatures(List<EmgTrainData> pose1RawData, List<EmgTrainData> pose2RawData)
        {
            var allRawData = new List<EmgTrainData>(pose1RawData.Count + pose2RawData.Count);

            allRawData.AddRange(pose1RawData);
            allRawData.AddRange(pose2RawData);

            double[][] model = new double[allRawData.Count][];

            int i = 0;

            foreach (var poseSet in allRawData)
            {
                model[i] = GetAverageEnergi(poseSet);
                i++;
            }

            return model;
        }

        private double[][] ExtractFeatures(List<int[]> pose1RawData)
        {
            double[][] model = new double[pose1RawData.Count][];

            int dataNumber = 0;

            foreach (var poseSet in pose1RawData)
            {
                double[] dataSet = new double[ChannalsToTrain.Count(a => a)];
                var c = 0;

                for (var i = 0; i < poseSet.Length; i++)
                {
                    if (!ChannalsToTrain[i]) continue;
                    dataSet[c] = dataSet[c] + Math.Pow((double)poseSet[i], 2);
                    c++;
                }

                for (var j = 0; j < dataSet.Length; j++)
                {
                    dataSet[j] = Math.Sqrt(dataSet[j] / pose1RawData.Count);
                }

                model[dataNumber] = dataSet;
            }

            return model;
        }

        private double[] GetAverageEnergi(EmgTrainData v)
        {
            double[] model = new double[ChannalsToTrain.Count(a => a)];

            foreach (var value in v.AquisitionData)
            {
                int c = 0;
                for (int i = 0; i < ChannalsToTrain.Length; i++)
                {
                    if (ChannalsToTrain[i])
                    {
                        model[c] = model[c] + Math.Pow(value[i], 2);
                        c++;
                    }
                }
            }

            for (int j = 0; j < model.Length; j++)
            {
                model[j] = Math.Sqrt(model[j] / v.AquisitionData.Count);
            }

            return model;
        }
              
        //TODO: Implement Multiple training methos.
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