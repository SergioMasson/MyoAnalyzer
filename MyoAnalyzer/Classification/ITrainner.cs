using System.Collections.Generic;
using MyoAnalyzer.DataTypes;
using MyoAnalyzer.Enums;

namespace MyoAnalyzer.Classification
{
    interface ITrainner
    {
        double Train(List<Pose> pose1RawData, bool[] channelsToTrain);

        Gestures Classify(List<int[]> rawData);

        void ResetTrain();

        bool IsTrainned();
    }

}
