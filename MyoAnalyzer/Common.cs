using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyoAnalyzer
{
    public static class Common
    {
        public static void FormatData(this List<string> Totaldata, StreamWriter writer)
        {                
            bool getData = true;

            foreach (var data in Totaldata)
            {
                if (getData)
                {
                    writer.WriteLine(data);                    
                }

                getData = !getData;
            }
        }

        public static int CountChannels(this bool[] Channels)
        {
            return Channels.Count(channel => channel);
        }
    }
}