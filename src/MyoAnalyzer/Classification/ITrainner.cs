using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;
using System.Collections.Generic;

namespace MyoAnalyzer.Classification
{
    public interface ITrainner
    {
        double Train(List<Pose> pose1RawData, bool[] channelsToTrain);

        Gestures Classify(List<double[]> rawData);

        Gestures Classify(EmgTrainData rawData);

        void ResetTrain();

        bool IsTrainned();
    }
}