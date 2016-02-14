using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyoAnalyzer
{
    public static class Common
    {
        public static void RemoveDuplicates(this List<string> totaldata, StreamWriter writer)
        {                
            bool getData = true;

            foreach (var data in totaldata)
            {
                if (getData)
                {
                    writer.WriteLine(data);                    
                }
                getData = !getData;
            }
        }       
    }
}