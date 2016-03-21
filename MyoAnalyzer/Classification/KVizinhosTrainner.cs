using MyoAnalyzer.Classification.Extraceter;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyoAnalyzer.Classification
{
    public class KVizinhosTrainner : ITrainner
    {
        private List<Point> PoitsSet;

        private bool isTrainned;

        private IExtracter FeatureExtracter;

        private bool[] _channelsToTrain;

        private const int NeighborsNumber = 5;

        public KVizinhosTrainner()
        {
            PoitsSet = new List<Point>();
        }

        public double Train(List<Pose> poseRawData, bool[] channelsToTrain)
        {
            _channelsToTrain = channelsToTrain;

            FeatureExtracter = new AverageEnergyExtracter(_channelsToTrain);

            foreach (var pose in poseRawData)
            {
                var attribute = FeatureExtracter.ExtractFeaturesFromMany(pose);

                foreach (var pointattribute in attribute)
                {
                    PoitsSet.Add(new Point(pointattribute)
                    {
                        Label = pose.GestureName
                    });
                }
            }

            isTrainned = true;

            return 0;
        }

        public Gestures Classify(List<int[]> rawData)
        {
            Point newData = new Point(FeatureExtracter.ExtractFeaturesFromSingle(rawData).First());

            PoitsSet.Sort((x, y) => newData.GetDistance(x).CompareTo(newData.GetDistance(y)));

            return
                PoitsSet.Take(NeighborsNumber)
                    .GroupBy(i => i.Label)
                    .OrderByDescending(grp => grp.Count())
                    .Select(grp => grp.Key)
                    .First();
        }

        public Gestures Classify(EmgTrainData rawData)
        {           
            Pose rawPose = new Pose(Gestures.None);

            rawPose.TotalPoseData.Add(rawData);

            Point newData = new Point(FeatureExtracter.ExtractFeaturesFromMany(rawPose).First());

            PoitsSet.Sort((x, y) => newData.GetDistance(x).CompareTo(newData.GetDistance(y)));

            return
                PoitsSet.Take(NeighborsNumber)
                    .GroupBy(i => i.Label)
                    .OrderByDescending(grp => grp.Count())
                    .Select(grp => grp.Key)
                    .First();
        }

        public void ResetTrain()
        {
            PoitsSet = new List<Point>();
        }

        public bool IsTrainned()
        {
            return isTrainned;
        }

        private string[] GetLabels(List<Pose> poseRawData)
        {
            string[] model = new string[poseRawData.Count];

            for (int i = 0; i < poseRawData.Count; i++)
            {
                model[i] = Common.PoseToString[poseRawData[i].GestureName];
            }
            return model;
        }

        internal class Point
        {
            private readonly double[] Coordinates;

            public Gestures Label { get; set; }

            public Point(double[] coordinates)
            {
                this.Coordinates = coordinates;
            }

            public double GetDistance(Point pointB)
            {
                if (pointB.Coordinates.Length != this.Coordinates.Length)
                {
                    throw new InvalidOperationException("Both vectors must be same size");
                }

                double distance = pointB.Coordinates.Select((t, i) => Math.Pow(t - this.Coordinates[i], 2)).Sum();

                return Math.Sqrt(distance);
            }
        }
    }
}