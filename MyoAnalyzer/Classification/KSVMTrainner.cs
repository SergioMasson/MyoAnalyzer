using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Controls;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;

namespace MyoAnalyzer.Classification
{
    class KSVMTrainner : ITrainner
    {
        private KernelSupportVectorMachine SVM;

        private bool[] _channelsToTrain;

        private bool _isTrainned;

        private string label1;
        private string label2;

        public KSVMTrainner()
        {
            _isTrainned = false;
        }

        public Gestures Classify(List<int[]> rawData)
        {
            if (_channelsToTrain == null)
                return Gestures.None;

            var data = ExtractFeaturesFromSingleTry(rawData);

            var answers = data.Apply(SVM.Compute).Apply(Math.Sign);

            return answers[0] == -1 ? Common.PoseToString.First(a => a.Value == label1).Key : Common.PoseToString.First(a => a.Value == label2).Key;
        }

        public void ResetTrain()
        {
            SVM = null;
            _channelsToTrain = null;
            _isTrainned = false;

        }

        public double Train(List<Pose> poseRawData, bool[] channelsToTrain)
        {
            List<EmgTrainData> pose1RawData = poseRawData.First().TotalPoseData;

            List<EmgTrainData> pose2RawData = poseRawData.Last().TotalPoseData;

            label1 = Common.PoseToString[poseRawData.First().GestureName];

            label2 = Common.PoseToString[poseRawData.Last().GestureName];

            _channelsToTrain = channelsToTrain;

            int classifierSize = channelsToTrain.Count(a => a);

            double[][] dataTraining = ExtractFeatures(pose1RawData, pose2RawData);

            IEnumerable<int> outputs1 = Enumerable.Repeat(-1, pose1RawData.Count);
            IEnumerable<int> outputs2 = Enumerable.Repeat(1, pose2RawData.Count);

            int[] totalOutput = outputs1.Concat(outputs2).ToArray<int>();

            // Estimate the kernel from the data
            var gaussian = Gaussian.Estimate(dataTraining);

            // Create a Gaussian binary support machine with 2 inputs
            SVM = new KernelSupportVectorMachine(gaussian, classifierSize);

            // Create a new Sequential Minimal Optimization (SMO) learning
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimization(SVM, dataTraining, totalOutput)
            {
                UseComplexityHeuristic = true
            };

            // Teach the vector machine
            double error = teacher.Run()*100;

            // Classify the samples using the model
            int[] answers = dataTraining.Apply(SVM.Compute).Apply(Math.Sign);

            _isTrainned = true;

            // Plot the results
            if (classifierSize != 2)
            {                
                return error;
            }

            ScatterplotBox.Show("Expected results", dataTraining, totalOutput);
            ScatterplotBox.Show("GaussianSVM results", dataTraining, answers);

            return error;

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

        private double[][] ExtractFeatures(List<EmgTrainData> pose1RawData, List<EmgTrainData> pose2RawData)
        {
            var allRawData = new List<EmgTrainData>(pose1RawData.Count + pose2RawData.Count);

            allRawData.AddRange(pose1RawData);
            allRawData.AddRange(pose2RawData);

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

        public bool IsTrainned()
        {
            return _isTrainned;
        }
    }
}
