using MyoAnalyzer.Classification;
using MyoAnalyzer.DataTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using Accord.Math;
using MyoAnalyzer.Enums;

namespace MyoAnalyzer.XAML_blocks
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {        
        private readonly List<Pose> _poses;
        private readonly bool[] _channalsToTrain;

        private readonly double _repetitions;
        private readonly double _percentage;

        private double[][] ConfusionMatrixData;

        public ResultWindow(List<Pose> poses, double percentage, double repetitions, bool[] channalsToTrain)
        {
            _poses = poses;
            _channalsToTrain = channalsToTrain;
            _repetitions = repetitions;
            _percentage = percentage;

            int n = poses.Count;

            ConfusionMatrixData = new double[n][];

            for (int i = 0; i < n; i++)
            {
                ConfusionMatrixData[i] = new double[n];               
            }

            InitializeComponent();
        }

        private void RandowSeparation(List<Pose> totalPoseData, double percentage, out List<Pose> posesToTrain, out List<Pose> posesToTest)
        {
            posesToTrain = new List<Pose>();
            posesToTest = new List<Pose>();

            Random rnd = new Random();

            foreach (var pose in totalPoseData)
            {
                Pose poseToTrain = new Pose(pose.GestureName);
                Pose poseToTest = new Pose(pose.GestureName);

                var randowData = pose.TotalPoseData.OrderBy(x => rnd.Next());

                var n = (int)(randowData.Count() * percentage);

                poseToTrain.TotalPoseData = randowData.Take(n).ToList();
                poseToTest.TotalPoseData = randowData.Reverse().Take(randowData.Count() - n).ToList();

                posesToTrain.Add(poseToTrain);
                posesToTest.Add(poseToTest);
            }
        }

        private void Train()
        {
            var errorOnTrain = 0.0;
            var errorOnTest = 0.0;

            int N = _poses.Count;

            Gestures[] GestureList = new Gestures[N];
        
            for (var i = 0; i < _repetitions; i++)
            {
                double[][] confusionMatrixData = new double[N][];

                List<Pose> posesToTrain;
                List<Pose> posesToTest;

                RandowSeparation(_poses, _percentage, out posesToTrain, out posesToTest);

                var trainner = new KVizinhosTrainner();
                errorOnTrain += trainner.Train(posesToTrain, _channalsToTrain);               

                double rightGuess = 0;
                int totalGuess = 0;
                int rowNumber = 0;               

                

                for (int k = 0; k < N; k++)
                {
                    confusionMatrixData[k] = new double[N];
                    GestureList[k] = posesToTest[k].GestureName;
                }

                for (int j = 0; j < N; j++)
                {
                    confusionMatrixData[j] = new double[N];
                }

                foreach (var pose in posesToTest)
                {
                    double totalRowGuest = 0;

                    foreach (var emgData in pose.TotalPoseData)
                    {
                        totalGuess++;
                        totalRowGuest++;

                        var outPutGesture = trainner.Classify(emgData);

                        if (outPutGesture == pose.GestureName)
                        {
                            rightGuess++;
                        }

                        var index = GestureList.IndexOf(outPutGesture);

                        confusionMatrixData[rowNumber][index]++;                       
                    }                   

                    NormalizeRow(confusionMatrixData[rowNumber], totalRowGuest);

                    rowNumber++;
                }

                errorOnTest += rightGuess / totalGuess;
                ConfusionMatrixData = SumMatrix(ConfusionMatrixData, confusionMatrixData);
            }

            TrainningErrorTextBox.Text = (errorOnTrain / _repetitions).ToString(CultureInfo.InvariantCulture);
            TestErrorTextBox.Text = (errorOnTest / _repetitions).ToString(CultureInfo.InvariantCulture);

            NormalizeMatrix(ConfusionMatrixData, _repetitions);

            SetMatrixToScreen();
        }

        private void Results_Loaded(object sender, RoutedEventArgs e)
        {
            Train();
        }

        private void NormalizeRow(double[] data, double total)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = data[i] / total;
            }
        }

        private void NormalizeMatrix(double[][] matrix, double total)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                NormalizeRow(matrix[i], total);
            }
        }

        private double[][] SumMatrix(double[][] matrix1, double[][]matrix2)
        {
            double[][] newMatrix = matrix1;

            foreach (var row in matrix1)
            {
                foreach (var element in row)
                {
                    newMatrix[row.IndexOf(element)][matrix1.IndexOf(row)] =
                        matrix2[row.IndexOf(element)][matrix1.IndexOf(row)] + element;
                }
            }
            return newMatrix;
        }

        private void SetMatrixToScreen()
        {
            DataSet data = new DataSet();
            DataTable dt = new DataTable();

            dt.Columns.Add("#");

            foreach (var pose in _poses)
            {
                dt.Columns.Add(pose.GestureName.ToString());
            }

            foreach (var pose in _poses)
            {
                var row = _poses.IndexOf(pose);

                DataRow dr = dt.NewRow();

                dr[0] = pose.GestureName.ToString();

                foreach (var newpose in _poses)
                {
                    var column = _poses.IndexOf(newpose);

                    dr[column + 1] = ConfusionMatrixData[row][column];
                }
                dt.Rows.Add(dr);
                dt.AcceptChanges();
            }

            data.Tables.Add(dt);

            ConfusionMatrix.ItemsSource = data.Tables[0].DefaultView;
        }
    }
}