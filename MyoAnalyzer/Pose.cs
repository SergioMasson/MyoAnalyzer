using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataClasses;

namespace MyoAnalyzer
{
    public class Pose
    {

        public Pose(string name)
        {
            TotalPoseData = new List<EmgTrainData>();
            GestureName = name;
            GestureCode = 0;
        }

        public List<EmgTrainData> TotalPoseData;
        public string GestureName { get; set; }
        public int GestureCode { get; set; }
    }
}
