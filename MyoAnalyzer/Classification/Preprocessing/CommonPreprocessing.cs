using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyoAnalyzer.Classification.Preprocessing
{
    public static class CommonPreprocessing
    {
        
        public static double AverageForPoint(this double[] dataArray, int halfWindowSize, int position) 
        {
            if ((position - halfWindowSize) < 0 || dataArray.Count() <= (position + halfWindowSize))
                return AverageForPoint(dataArray, halfWindowSize - 1, position);

            return dataArray.Skip(position - halfWindowSize).Take((halfWindowSize * 2) + 1).Sum(x => x);
        }       
    }  
}
