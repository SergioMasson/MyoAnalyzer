using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoAnalyzer.Classification.Preprocessing
{
    class Demodulation : IPreprocessor
    {
        public List<double[]> Apply(List<double[]> data)
        {           
            var model = data;

            foreach (var sensorData in data)
            {
                var modulatedData = new double[sensorData.Count()];

                for (var i = 0; i < sensorData.Count(); i++)
                {
                    modulatedData[i] = Math.Abs(sensorData[i]);
                }

                model.Add(modulatedData);
            }
            return model;
        }
    }
}
