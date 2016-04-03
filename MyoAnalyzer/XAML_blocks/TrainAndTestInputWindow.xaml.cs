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
using System.Windows.Shapes;

namespace MyoAnalyzer.XAML_blocks
{
    /// <summary>
    /// Interaction logic for TrainAndTestInputWindow.xaml
    /// </summary>
    public partial class TrainAndTestInputWindow : Window
    {

        private double Percentage;

        private double Repetitions;

        public TrainAndTestInputWindow()
        {           
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Percentage = Convert.ToDouble(PercentageTextBox.Text);
            Repetitions = Convert.ToDouble(RepetitionTextBox.Text);
            this.Close();
        }

        public Task<double[]>DataDone()
        {
            return Task<double[]>.Factory.StartNew(WaitingForData);
        }

        public double[] WaitingForData()
        {
            while (true)
            {
                if (Percentage != 0 && Repetitions != 0)
                {
                    double[] data = {Percentage, Repetitions};
                    return data;
                }           
            }
        }
    }
}
