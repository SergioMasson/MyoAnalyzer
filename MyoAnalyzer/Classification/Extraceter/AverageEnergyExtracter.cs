using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyoAnalyzer.DataTypes;

namespace MyoAnalyzer.Classification.Extraceter
{
    class AverageEnergyExtracter : IExtracter
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
                model[i] = GetAverageEnergi(poseSet);
                i++;
            }

            return model;
        }

        public double[][] ExtractFeaturesFromSingle(List<int[]> pose1RawData)
        {
            double[][] model = new double[1][];

            model[0] = new double[_channelsToTrain.Count(a => a)];

            int dataTrained = 0;

            for (int i = 0; i < _channelsToTrain.Length - 1; i++)
            {

                if (_channelsToTrain[i])
                {
                    foreach (var poseSet in pose1RawData)
                    {
                        model[0][dataTrained] += Math.Pow(poseSet[i], 2);
                    }
                    dataTrained++;
                }
            }

            for (var j = 0; j < model[0].Length; j++)
            {
                model[0][j] = Math.Sqrt(model[0][j] / pose1RawData.Count);
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
