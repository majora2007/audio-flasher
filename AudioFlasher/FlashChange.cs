using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioFlasher
{
    class FlashChange : IComparable<FlashChange>
    {
        public double FreqInSec { get; set; }
        public double StartTime { get; set; } // Smaller values are higher priority

        public FlashChange( double freq, double time )
        {
            FreqInSec = freq;
            StartTime = time;
        }

        public int CompareTo( FlashChange other )
        {
            if ( this.StartTime < other.StartTime ) return -1;
            else if ( this.StartTime > other.StartTime ) return 1;
            else return 0;
        }
    }
}
