using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoAnalyzer.Classification.Preprocessing
{
    class MovingAverage : IPreprocessor
    {
        private readonly int _halfWindowSize = 4;

        public List<double[]> Apply(List<double[]> data)
        {       
            var model = data;

            for (var j = 0; j < data.Count; j++)
            {
                for (var i = 0; i < data[j].Count(); i++)
                {
                    model[j][i] = data[j].AverageForPoint(_halfWindowSize, i);
                }
            }
            return model;
        }
    }
}
