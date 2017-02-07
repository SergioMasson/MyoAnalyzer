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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace DynamicPlotWPF
{
    /// <summary>
    /// Interaction logic for StaticMultiLinesPlotWindow.xaml
    /// </summary>
    public partial class StaticMultiLinesPlotWindow : Window
    {
        private List<double[]> Signal;

        private int[] TimeStamp;

        private readonly int NUMBER_OF_SENSORS = 8;

        private readonly SolidColorBrush[] ColourBrush = new SolidColorBrush[]
        {
            Brushes.OrangeRed,
            Brushes.LightSkyBlue,
            Brushes.Gold,
            Brushes.Fuchsia,
            Brushes.Pink,
            Brushes.GreenYellow,
            Brushes.White,
            Brushes.MediumPurple
        };

        public StaticMultiLinesPlotWindow(List<double[]> signal)
        {
            Signal = signal;
            TimeStamp = signal.Select( a => (int)a[NUMBER_OF_SENSORS]).ToArray();
            InitializeComponent();
        }

        private void StaticPlotWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlotDataFrom();
        }

        private void IndividualPlot_Click(object sender, RoutedEventArgs e)
        {
            int[] sensorNumber;

            int individualSensor;

            if (int.TryParse(NumberOfSensoresTextBox.Text, out individualSensor))
            {
                sensorNumber = new[] {individualSensor};
                PlotSingleItemData(sensorNumber);
                return;
            }               

            try
            {
                char[] separators = {' ', ';', ','};
                sensorNumber = NumberOfSensoresTextBox.Text.Split(separators).Select(n => Convert.ToInt32(n)).ToArray();
            }
            catch (Exception exeption)
            {
                return;
            }

            PlotSingleItemData(sensorNumber);
        }

        private void PlotAll_Click(object sender, RoutedEventArgs e)
        {
            PlotDataFrom();
        }

        private void PlotDataFrom()
        {
            SignalPlot.Children.RemoveAll((typeof(LineGraph)));

            var timeAxisVariable = new EnumerableDataSource<int>(TimeStamp);

            timeAxisVariable.SetXMapping(x => SignalAxis.ConvertToDouble(x));

            for (int i = 0; i < NUMBER_OF_SENSORS; i++)
            {
                int[] dataToPlot = Signal.Select(a => (int)a[i]).ToArray();

                var numberOpenDataSource = new EnumerableDataSource<int>(dataToPlot);
                numberOpenDataSource.SetYMapping(y => y);

                CompositeDataSource compositeDataSource1 = new CompositeDataSource(timeAxisVariable, numberOpenDataSource);

                SignalPlot.AddLineGraph(compositeDataSource1, new Pen(ColourBrush[i], 1), new PenDescription("Sensor " + i));

                SignalPlot.Legend.Background = Brushes.Black;

                SignalPlot.Legend.Foreground = Brushes.Black;

            }

            SignalPlot.Viewport.FitToView();
        }

        private void PlotSingleItemData(int[] Sensors)
        {

            SignalPlot.Children.RemoveAll((typeof(LineGraph)));

            var timeAxisVariable = new EnumerableDataSource<int>(TimeStamp);

            timeAxisVariable.SetXMapping(x => SignalAxis.ConvertToDouble(x));

            foreach (var fakeSensorNumber in Sensors)
            {
                if (fakeSensorNumber >= NUMBER_OF_SENSORS)
                    return;

                int sensorNumber = fakeSensorNumber - 1;
                                            
                int[] dataToPlot = Signal.Select(a => (int)a[sensorNumber]).ToArray();

                var numberOpenDataSource = new EnumerableDataSource<int>(dataToPlot);
                numberOpenDataSource.SetYMapping(y => y);

                CompositeDataSource compositeDataSource1 = new CompositeDataSource(timeAxisVariable, numberOpenDataSource);

                SignalPlot.AddLineGraph(compositeDataSource1, new Pen(ColourBrush[sensorNumber], 1), new PenDescription("Sensor " + fakeSensorNumber));               
            }

            SignalPlot.Legend.Background = Brushes.Black;

            SignalPlot.Legend.Foreground = Brushes.Black;

        }

        private void HideGrid_Click(object sender, RoutedEventArgs e)
        {

            if (SignalPlot.AxisGrid.IsVisible)
            {
                SignalPlot.AxisGrid.Visibility = Visibility.Hidden;
                IndividualPlotButton.Content = "Show Grids";
                return;
            }

            SignalPlot.AxisGrid.Visibility = Visibility.Visible;
            IndividualPlotButton.Content = "Hide Grids";

        }

        private void SensorNumber_Click(object sender, MouseButtonEventArgs e)
        {
            NumberOfSensoresTextBox.Text = "";
        }
    }
}
