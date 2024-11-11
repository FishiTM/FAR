using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAR
{
    internal class Smoothing
    {
        private static Random RND = new Random();
        public static int Calculate(int defaultValue, int smoothingValue)
        {
            // SET VALUES
            int minResult = defaultValue - smoothingValue;
            if (minResult < 0) { minResult = 0; }
            int maxResult = defaultValue + smoothingValue;
            // CALCULATE
            int returnValue = RND.Next(minResult, maxResult);
            // RETURN
            return returnValue;
        }
    }
}
