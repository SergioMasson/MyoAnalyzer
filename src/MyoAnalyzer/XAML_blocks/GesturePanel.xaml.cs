using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;

namespace MyoAnalyzer.XAML_blocks
{
    /// <summary>
    /// Interaction logic for GesturePanel.xaml
    /// </summary>
    public partial class GesturePanel : UserControl
    {

        public Pose Pose { get; set; }     

        public GesturePanel(string poseName)
        {
            InitializeComponent();

            Pose = new Pose(poseName);

            this.PoseName.Text = poseName;           
        }

        public GesturePanel(Gestures poseName)
        {
            InitializeComponent();

            Pose = new Pose(poseName);

            this.PoseName.Text = poseName.ToString();
        }

        private void LoadPoseData_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();

            open.Multiselect = true;

            if (open.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            List<EmgTrainData> totalPoseData = new List<EmgTrainData>();

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

                totalPoseData.Add(finalData);
            }

            Pose.TotalPoseData.AddRange(totalPoseData);

            NumberPose1Samples.Text = Pose.TotalPoseData.Count.ToString();
        }

        private void CleanGestureData_Click(object sender, RoutedEventArgs e)
        {
            Pose.TotalPoseData = new List<EmgTrainData>();

            NumberPose1Samples.Text = "0";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {

        }

        public void CleanGestureData()
        {
            Pose.TotalPoseData = new List<EmgTrainData>();
        }
    }
}
