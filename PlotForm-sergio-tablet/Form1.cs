using DataClasses;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace PlotForm
{
    public partial class Plot : Form
    {
        #region Constants

        private const int NUMBER_OF_SENSORS = 8;

        private static readonly Color[] DATA_SERIES_COLORS = {
            Color.Red,
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Pink,
            Color.Orange,
            Color.Purple,
            Color.Black,
        };

        #endregion Constants

        #region Fields

        private readonly PointPairList[] _pointPairs;
        private readonly ZedGraphControl _graphControl;
        private readonly List<LineItem> _sortOrderZ;

        #endregion Fields

        #region Constructors

        public Plot()
        {
            InitializeComponent();

            // construct our graph
            _graphControl = new ZedGraphControl()
            {
                Dock = DockStyle.Fill,
            };
            _graphControl.GraphPane.Title.Text = "Myo EMG _dataColected vs Time";
            _graphControl.GraphPane.YAxis.Title.Text = "EMG Value";
            _graphControl.GraphPane.XAxis.Title.Text = "Time (milliseconds)";

            _pointPairs = new PointPairList[NUMBER_OF_SENSORS];
            _sortOrderZ = new List<LineItem>();
            for (var i = 0; i < _pointPairs.Length; ++i)
            {
                _pointPairs[i] = new PointPairList();

                var dataPointLine = _graphControl.GraphPane.AddCurve("Sensor " + i, _pointPairs[i], Color.Black);
                dataPointLine.Line.IsVisible = false;
                dataPointLine.Symbol.Fill = new Fill(DATA_SERIES_COLORS[i]);

                _sortOrderZ.Add(dataPointLine);
            }

            Controls.Add(_graphControl);
        }

        #endregion Constructors

        private void RefreshGraph()
        {
            // force a redraw for new _dataColected
            _graphControl.AxisChange();
            _graphControl.Invalidate();
        }

        #region Event Handlers

        public void SendDataToPlot(List<int[]> dataColected)
        {
            foreach (var points in dataColected)
            {
                for (int i = 0; i < 8; i++)
                {
                    _pointPairs[i].Add(points[8], points[i]);
                }
                RefreshGraph();
            }
        }

        #endregion Event Handlers
    }
}