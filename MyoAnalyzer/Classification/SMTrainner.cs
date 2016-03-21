using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Controls;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using MyoAnalyzer.Classification.Extraceter;
using MyoAnalyzer.Classification.Ranker;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;

namespace MyoAnalyzer.Classification
{
    class SMTrainner: ITrainner
    {     
        private bool[] _channelsToTrain;
        private  FeatureRanker Ranker;
        private bool _isTrainned;

        private string label1;
        private string label2;       


        public SMTrainner()
        {
            _isTrainned = false;
            Ranker = new FeatureRanker();
        }

        public Gestures Classify(List<int[]> rawData)
        {
            return Gestures.None;
        }

        public Gestures Classify(EmgTrainData rawData)
        {
            throw new NotImplementedException();
        }

        public void ResetTrain()
        {          
            _channelsToTrain = null;
            _isTrainned = false;

        }

        public double Train(List<Pose> poseRawData, bool[] channelsToTrain)
        {
            return 0.0;
        }

        private double[][] ExtractFeaturesFromSingleTry(List<int[]> pose1RawData)
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

        public bool IsTrainned()
        {
            return _isTrainned;
        }
    }
}
