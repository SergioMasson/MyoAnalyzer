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
        void Train(List<Pose> pose1RawData, bool[] channelsToTrain);

        string Classify(List<int[]> rawData);

        void ResetTrain();

        bool IsTrainned();

    }

}
