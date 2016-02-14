using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using DataClasses;

namespace MyoAnalyzer
{
    class KSVMMultipleTrainner : ITrainner
    {
        private MultilabelSupportVectorMachine SVM;

        private bool[] _channelsToTrain;

        private bool _isTrainned;

        private string[] Labels;       

        public KSVMMultipleTrainner()
        {
            _isTrainned = false;
        }

        public string Classify(List<int[]> rawData)
        {
            if (_channelsToTrain == null)
                return "0";

            var data = ExtractFeaturesFromSingleTry(rawData);

            int[][] answers = data.Apply(SVM.Compute);

            string finalAnswer = GetLabelFromResult(answers[0]);
            //return GetLabels[answers];
            
            return finalAnswer;

        }

        private string GetLabelFromResult(int[] answers)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                if (answers[i] == 1)
                    return Labels[i];
            }

            return "Erro";

        }

        public void ResetTrain()
        {
            SVM = null;
            _channelsToTrain = null;
            _isTrainned = false;

        }

       
        public double Train(List<Pose> poseRawData, bool[] channelsToTrain)
        {                     
            Labels = GetLabels(poseRawData);

            _channelsToTrain = channelsToTrain;

            double trainnerComplexity = 500;

            int classifierSize = channelsToTrain.Count(a => a);

            double[][] dataTraining = ExtractFeatures(poseRawData);

            int[][] totalOutput = GenerateOutputs(poseRawData);
            
            IKernel kernel = new Linear();

            SVM = new MultilabelSupportVectorMachine(classifierSize, kernel, poseRawData.Count);

            var teacher = new MultilabelSupportVectorLearning(SVM, dataTraining, totalOutput);

            TryToOptimize(teacher, trainnerComplexity);           
           
            double error = teacher.Run()*100;

            _isTrainned = true;

            return error;

        }

        private int[][] GenerateOutputs(List<Pose> poseRawData)
        {
            int numberOfClasses = poseRawData.Count;

            List<int[]> model = new List<int[]>();

            int output = 0;

            foreach (Pose pose in poseRawData)
            {
                foreach (var poseData in pose.TotalPoseData)
                {
                    IEnumerable<int> emgData = Enumerable.Repeat(-1, poseRawData.Count);
                    int[] emgDataArray = emgData.ToArray<int>();
                    emgDataArray[output] = 1;
                    model.Add(emgDataArray);
                }             
                output++;
            }
            return model.ToArray();
        }

        private string[] GetLabels(List<Pose> poseRawData)
        {
            string[] model = new string[poseRawData.Count];

            for (int i = 0; i < poseRawData.Count; i++)
            {
                model[i] = poseRawData[i].GestureName;
            }
            return model;
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

        private double[][] ExtractFeatures(List<Pose> poseRawData)
        {

            var allRawData = new List<EmgTrainData>();

            foreach (Pose pose in poseRawData)
            {
                allRawData.AddRange(pose.TotalPoseData);
            }          

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

        private void TryToOptimize(MultilabelSupportVectorLearning teacher,double trainnerComplexity)
        {
            try
            {
                teacher.Algorithm = (svm, classInputs, classOutputs, i, j) => new SequentialMinimalOptimization(svm, classInputs, classOutputs)
                {
                    Complexity = trainnerComplexity
                };
            }
            catch (Exception)
            {
                TryToOptimize(teacher, trainnerComplexity*0.5);
            }
        }
    }
}
