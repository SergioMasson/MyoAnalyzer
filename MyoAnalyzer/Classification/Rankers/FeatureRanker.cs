using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Univariate;

namespace MyoAnalyzer.Classification.Ranker
{
    public class FeatureRanker
    {
        public double[][] RankFeatures(double[][] rawData1, double[][] rawData2, int channelNumber)
        {

            List<Attribute> attributes = new List<Attribute>();           

            if (rawData1 == null || rawData2.Length == 0)
                return null;

            for (var i = 0; i < 8; i++)
            {
                attributes.Add(GetAttributeQuality(rawData1, rawData2, i));
            }                                            

            var model = attributes.OrderByDescending(a => a.GetQuality()).Select(GetProperties).Take(channelNumber).ToArray();

            return model;
        }

        private double CompateDistance(double average, IEnumerable<double> otherAverages)
        {
            double isolationCoef = otherAverages.Sum(a => Math.Pow(a - average, 2))/ otherAverages.Count();
            return isolationCoef;
        }

        private Attribute GetAttributeQuality(double[][] rawData1, double[][] rawData2, int channel)
        {

            double[] data1 = rawData1.Select(data => data[channel]).ToArray();

            double[] data2 = rawData2.Select(data => data[channel]).ToArray();

            double average1 = GetAverage(data1);

            double average2 = GetAverage(data2);

            double deviation1 = GetDeviation(data1, average1);

            double deviation2 = GetDeviation(data2, average2);

            Attribute model = new Attribute(channel + 1)
            {
                Average1 = average1,
                Average2 = average2,
                StDeviation1 = deviation1,
                StDeviation2 = deviation2,
            };

            return model;
        }

        private double GetDeviation(double[] dataSet, double average)
        {
            double deviation = dataSet.Sum(data => Math.Pow((data - average), 2));

            return Math.Sqrt(deviation / dataSet.Length);
        }

        private double GetAverage(double[] dataSet)
        {
            double average = dataSet.Sum();

            return (average/dataSet.Length);
        }

        private double[] GetProperties(Attribute atribute)
        {
            double[] model = new double[4];

            model[0] = atribute.Number;

            model[1] = atribute.Average1;

            model[2] = atribute.Average2;

            model[3] = atribute.StDeviation1 + atribute.StDeviation2;

            return model;

        }
    }

    internal class Attribute
    {
        public double Number { get; set; }
        public double StDeviation1 { get; set;}
        public double Average1 { get; set;}
        public double StDeviation2 { get; set; }
        public double Average2 { get; set; }

        public Attribute(int number)
        {
            Number = number;
            StDeviation1 = 0;
            Average1 = 0;
            StDeviation2 = 0;
            Average2 = 0;
        }

        public double GetQuality()
        {
            return Math.Abs(Average1 - Average2)/(StDeviation1 + StDeviation2);
        } 

    }
    
 
}
