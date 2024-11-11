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
        public static int Calculate(int def, int value)
        {
            // SET VALUES
            int min = def - value;
            if (min < 0)
                min = 0;
            int max = def + value;
            // CALCULATE
            int ret = RND.Next(min, max);
            // RETURN
            return ret;
        }
    }
}
