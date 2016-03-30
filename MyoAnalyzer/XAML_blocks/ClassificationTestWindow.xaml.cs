using Accord.Math;
using MyoAnalyzer.Classification;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace MyoAnalyzer.XAML_blocks
{
    /// <summary>
    /// Interaction logic for ClassificationTestWindow.xaml
    /// </summary>
    public partial class ClassificationTestWindow : Window
    {
        public ITrainner Trainner;

        private readonly List<Pose> Poses;

        private readonly List<string> Report;       

        private readonly Gestures[] GestureList;

        private double[][] ConfusionMatrixData;

        private int N;

        public ClassificationTestWindow(ITrainner trainner, List<Pose> gestures)
        {
            Trainner = trainner;

            N = gestures.Count;

            ConfusionMatrixData = new double[N][];
            GestureList = new Gestures[N];

            for (int i = 0; i < N; i++)
            {
                ConfusionMatrixData[i] = new double[N];
                GestureList[i] = gestures[i].GestureName;
            }

            Poses = gestures;

            Report = new List<string>();

            InitializeComponent();
        }

        private void ClassificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var gesture in Poses)
            {
                XAML_blocks.GesturePanel newGesture = new XAML_blocks.GesturePanel(gesture.GestureName);

                PosePanel.Children.Add(newGesture);
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            double rightGuess = 0;
            int totalGuess = 0;
            int rowNumber = 0;

            ConfusionMatrixData = new double[N][];

            for (int i = 0; i < N; i++)
            {
                ConfusionMatrixData[i] = new double[N];
            }

            Report.Add("--------- Test Report --------------");
            Report.Add(" \n Number \t Original Pose \t Output Pose");

            List<Pose> totalPoses = (from XAML_blocks.GesturePanel pose in PosePanel.Children select pose.Pose).ToList();

            foreach (var pose in totalPoses)
            {
                double totalRowGuest = 0;

                foreach (var emgData in pose.TotalPoseData)
                {
                    totalGuess++;
                    totalRowGuest++;

                    var outPutGesture = Trainner.Classify(emgData);

                    if (outPutGesture == pose.GestureName)
                    {
                        rightGuess++;
                    }

                    var index = GestureList.IndexOf(outPutGesture);

                    ConfusionMatrixData[rowNumber][index]++;

                    string reportLine = totalGuess + "\t" + pose.GestureName.ToString() + "\t" + outPutGesture;
                    Report.Add(reportLine);
                }

                NormalizeRow(ConfusionMatrixData[rowNumber], totalRowGuest);

                rowNumber++;
            }

            ResultsWindowTextBox.AppendText("\n Test sucessed! Your classifyer scored : " + rightGuess + " / " + totalGuess);

            SaveButton.Visibility = Visibility.Visible;
            ConfusionButton.Visibility = Visibility.Visible;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.FileName = "Report.txt";
            save.Filter = "Text File | *.txt";

            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                Report.RemoveDuplicates(writer);
                writer.Dispose();
                writer.Close();
            }
        }

        private void Confusion_Click(object sender, RoutedEventArgs e)
        {
            ConfusionGrid.Visibility = Visibility.Visible;

            int column = 0;
            int row = 0;

            DataSet data = new DataSet();
            DataTable dt = new DataTable();

            dt.Columns.Add("#");

            foreach (var pose in Poses)
            {
                dt.Columns.Add(pose.GestureName.ToString());
            }

            foreach (var pose in Poses)
            {
                row = Poses.IndexOf(pose);

                DataRow dr = dt.NewRow();

                dr[0] = pose.GestureName.ToString();

                foreach (var newpose in Poses)
                {
                    column = Poses.IndexOf(newpose);

                    dr[column + 1] = ConfusionMatrixData[row][column];
                }
                dt.Rows.Add(dr);
                dt.AcceptChanges();
            }

            data.Tables.Add(dt);

            ConfusionMatrix.ItemsSource = data.Tables[0].DefaultView;
        }

        private void NormalizeRow(double[] data, double total)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = data[i] / total;
            }
        }
    }
}