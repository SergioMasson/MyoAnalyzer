using System.Collections.Generic;

namespace MyoAnalyzer.Classification
{
    interface ITrainner
    {
        double Train(List<Pose> pose1RawData, bool[] channelsToTrain);

        string Classify(List<int[]> rawData);

        void ResetTrain();

        bool IsTrainned();
    }

}
