using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using DataClasses;

namespace MyoAnalyzer
{
    interface ITrainner
    {
        void Train(List<EmgTrainData> pose1RawData, List<EmgTrainData> pose2RawData, bool[] channelsToTrain);

        int Classify(List<int[]> rawData, int label1, int label2);

        void ResetTrain();

        bool IsTrainned();

    }

}
