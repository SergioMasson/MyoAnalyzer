using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using MyoAnalyzer.Classification;
using MyoAnalyzer.Classification.Extraceter;

namespace MyoAnalyzer.Classification
{
    class KSVMMultipleTrainner : ITrainner
    {
        private MultilabelSupportVectorMachine SVM;

        private IExtracter FeatureExtracter;

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

            FeatureExtracter = new AverageEnergyExtracter(_channelsToTrain);

            var data = FeatureExtracter.ExtractFeaturesFromSingle(rawData);

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

            FeatureExtracter = new AverageEnergyExtracter(_channelsToTrain);

            double trainnerComplexity = 500;

            int classifierSize = channelsToTrain.Count(a => a);

            List<double[]> dataTraining = new List<double[]>();           

            dataTraining = poseRawData.Select(pose => FeatureExtracter.ExtractFeaturesFromMany(pose)).Aggregate(dataTraining, (current, singlePoseData) => current.Concat(singlePoseData).ToList());

            int[][] totalOutput = GenerateOutputs(poseRawData);
            
            IKernel kernel = new Linear();

            SVM = new MultilabelSupportVectorMachine(classifierSize, kernel, poseRawData.Count);

            var teacher = new MultilabelSupportVectorLearning(SVM, dataTraining.ToArray(), totalOutput);

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
