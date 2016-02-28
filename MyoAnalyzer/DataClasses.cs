using System.Collections.Generic;

namespace MyoAnalyzer
{
    public class EmgTrainData
    {
        public EmgTrainData()
        {
            AquisitionData = new List<double[]>();
        }

        public List<double[]> AquisitionData { get; set; }
    }
}