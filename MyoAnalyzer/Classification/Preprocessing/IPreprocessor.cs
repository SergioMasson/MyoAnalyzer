using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace MyoAnalyzer.Classification.Preprocessing
{
    interface IPreprocessor
    {
        List<double[]> Apply(List<double[]> data);       
    }
}
