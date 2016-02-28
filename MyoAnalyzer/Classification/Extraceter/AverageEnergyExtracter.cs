using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoAnalyzer.Classification.Extraceter
{
    class AverageEnergyExtracter : IExtracter
    {

        private readonly bool[] _channelsToTrain;


        public AverageEnergyExtracter(bool[] channelsToTrain)
        {
            _channelsToTrain = channelsToTrain;
        }

        public double[][] ExtractFeatures(Pose poseRawData)
        {

            var allRawData = poseRawData.TotalPoseData;

            double[][] model = new double[allRawData.Count][];

            int i = 0;

            foreach (var poseSet in allRawData)
            {
                model[i] = GetAverageEnergi(poseSet);
                i++;
            }

            return model;
        }

        private double[] GetAverageEnergi(EmgTrainData poseSet)
        {
            double[] model = new double[_channelsToTrain.Count(a => a)];

            foreach (var value in poseSet.AquisitionData)
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
                model[j] = Math.Sqrt(model[j] / poseSet.AquisitionData.Count);
            }

            return model;
        }

    }
}
