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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace DynamicPlotWPF
{
    /// <summary>
    /// Interaction logic for PlotDynamicWindow.xaml
    /// </summary>
    public partial class PlotDynamicWindow : Window
    {
        private List<int[]> Signal;

        private int[] TimeStamp;

        private ObservableDataSource<Point> Source;

        public PlotDynamicWindow()
        {            
            InitializeComponent();         
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
                        
            Source = new ObservableDataSource<Point>();

            Source.SetXYMapping(p => p);           

            Signal1.AddLineGraph(Source,
                new Pen(Brushes.Blue, 2),
                new CirclePointMarker {Size = 2.0, Fill = Brushes.Black},
                new PenDescription("Myo Singal (Sensor 1)"));


            //plotter.AddLineGraph(compositeDataSource2,
            //    new Pen(Brushes.Green, 2),
            //    new TrianglePointMarker
            //    {
            //        Size = 10.0,
            //        Pen = new Pen(Brushes.Black, 2.0),
            //        Fill = Brushes.GreenYellow
            //    },
            //    new PenDescription("Number bugs closed"));

            Signal1.Viewport.FitToView();
        }

        public void UpdateGraph(int data, int timeStamp)
        {
            Point p1 = new Point(timeStamp, data);

            Source.AppendAsync(Dispatcher, p1);

            if(Source.Collection.Count < 2000)
                Source.SuspendUpdate();
        }
    }   
}
