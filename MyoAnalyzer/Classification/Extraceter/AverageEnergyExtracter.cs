using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MyoAnalyzer.DataTypes;

namespace MyoAnalyzer.Classification.Extraceter
{
    internal class AverageEnergyExtracter : IExtracter
    {

        private readonly bool[] _channelsToTrain;     

        public AverageEnergyExtracter(bool[] channelsToTrain)
        {
            _channelsToTrain = channelsToTrain;
        }

        public double[][] ExtractFeaturesFromMany(Pose poseRawData)
        {

            var allRawData = poseRawData.TotalPoseData;

            double[][] model = new double[allRawData.Count][];

            int i = 0;

            foreach (var poseSet in allRawData)
            {
                model[i] = GetAverageEnergi(poseSet.AquisitionData);
                i++;
            }

            return model;
        }

        public double[][] ExtractFeaturesFromSingle(List<double[]> pose1RawData)
        {           
            double[][] model = new double[1][];

            model[0] = GetAverageEnergi(pose1RawData);           

            return model;
        }

        private double[] GetAverageEnergi(List<double[]> rawPoseSet)
        {
            double[] model = new double[_channelsToTrain.Count(a => a)];                      

            foreach (var value in rawPoseSet)
            {
                int c = 0;
                for (int i = 0; i < _channelsToTrain.Length; i++)
                {
                    if (_channelsToTrain[i])
                    {
                        model[c] = model[c] + Math.Pow(value[i], 2);
                        c++;
                    }
                }
            }
            for (int j = 0; j < model.Length; j++)
            {
                model[j] = Math.Sqrt(model[j]/ rawPoseSet.Count);
            }

            return model;
        }
    }
}
