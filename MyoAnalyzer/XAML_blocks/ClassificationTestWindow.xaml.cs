using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using MyoAnalyzer.Classification;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;


namespace MyoAnalyzer.XAML_blocks
{
    /// <summary>
    /// Interaction logic for ClassificationTestWindow.xaml
    /// </summary>
    public partial class ClassificationTestWindow : Window
    {

        public ITrainner Trainner;

        private List<Pose> Poses;

        private List<string> Report; 

        public ClassificationTestWindow(ITrainner trainner, List<Pose> gestures)
        {
            Trainner = trainner;

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

            double TotalGuess = 0;


            Report.Add("--------- Test Report --------------");
            Report.Add(" \n Number \t Original Pose \t Output Pose");

            List<Pose> totalPoses = (from XAML_blocks.GesturePanel pose in PosePanel.Children select pose.Pose).ToList();

            foreach (var pose in totalPoses)
            {
                foreach (var emgData in pose.TotalPoseData)
                {
                    TotalGuess++;                   
                    var outPutGesture = Trainner.Classify(emgData);
                    if (outPutGesture == pose.GestureName)
                    {
                        rightGuess++;
                    }                     
                    string reportLine = TotalGuess + "\t" + pose.GestureName.ToString() + "\t" + outPutGesture;
                    Report.Add(reportLine);
                }             
            }

            ResultsWindowTextBox.AppendText("\n Test sucessed! Your classifyer scored : " + rightGuess + " / " + TotalGuess);

            SaveButton.Visibility = Visibility.Visible;
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
    }
}
