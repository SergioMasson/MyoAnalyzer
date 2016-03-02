using System.Collections.Generic;
using System.Linq;
using MyoAnalyzer.Enums;

namespace MyoAnalyzer.DataTypes
{
    public class Pose
    {
        public Pose(string name)
        {
            TotalPoseData = new List<EmgTrainData>();
            GestureName = Common.PoseToString.First(a => a.Value == name).Key;
            GestureCode = 0;
        }
        public List<EmgTrainData> TotalPoseData;
        public Gestures GestureName { get; set; }
        public int GestureCode { get; set; }
    }
}
