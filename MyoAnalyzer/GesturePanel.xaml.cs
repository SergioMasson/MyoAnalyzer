﻿using DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyoAnalyzer
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
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}