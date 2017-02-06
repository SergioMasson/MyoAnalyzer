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

        private int Percentage;

        private int Repetitions;

        public TrainAndTestInputWindow()
        {           
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Percentage = int.Parse(PercentageTextBox.Text);
                PercentageError.Visibility = Visibility.Hidden;
                try
                {
                    Repetitions = int.Parse(RepetitionTextBox.Text);
                    RepetitionError.Visibility = Visibility.Hidden;
                }
                catch (Exception exception)
                {                  
                    RepetitionError.Visibility = Visibility.Visible;
                    return;
                }                
            }
            catch (Exception exception)
            {                
                PercentageError.Visibility = Visibility.Visible;
                return;
            }          
           
            this.Close();
        }

        public Task<int[]>DataDone()
        {
            return Task<int[]>.Factory.StartNew(WaitingForData);
        }

        public int[] WaitingForData()
        {
            while (true)
            {
                if (Percentage != 0 && Repetitions != 0)
                {
                    int[] data = {Percentage, Repetitions};
                    return data;
                }           
            }
        }
    }
}
