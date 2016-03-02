using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyoAnalyzer.Enums;

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

        public static Dictionary<Gestures, string> PoseToString = new Dictionary<Gestures, string>()
        {
            {Gestures.Open, "Open"},
            {Gestures.Close, "Close"},
            {Gestures.Rock, "Rock`n Roll"},
            {Gestures.Like, "Like"},
            {Gestures.One, "One" }
        };
    }
}