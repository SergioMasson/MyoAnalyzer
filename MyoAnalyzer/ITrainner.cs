using System.Collections.Generic;

namespace MyoAnalyzer
{
    interface ITrainner
    {
        double Train(List<Pose> pose1RawData, bool[] channelsToTrain);

        string Classify(List<int[]> rawData);

        void ResetTrain();

        bool IsTrainned();

    }

}
