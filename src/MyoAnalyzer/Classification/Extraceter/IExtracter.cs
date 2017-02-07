using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyoAnalyzer.DataTypes;

namespace MyoAnalyzer.Classification.Extraceter
{
    interface IExtracter
    {
        double[][] ExtractFeaturesFromMany(Pose poseRawData);

        double[][] ExtractFeaturesFromSingle(List<double[]> poseRawData);
    }
}
