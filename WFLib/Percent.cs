using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFLib
{
    [System.Diagnostics.DebuggerDisplay("{AsPercent0to100}%")]
    public class Percent
    {
        private Percent(double pDecimal0to1)
        {
            this.AsDecimal0to1 = pDecimal0to1;
        }
        public double AsPercent0to100 { get { return this.AsDecimal0to1 * 100.0; } }
        public double AsDecimal0to1;

        public static Percent FromPercent0to100(double p) { return new Percent(p / 100.0); }
        public static Percent FromDecimal0to1(double p) { return new Percent(p); }
        public static Percent Zero { get { return new Percent(0); } }

        public static implicit operator Percent(int d)
        {
            return Percent.FromPercent0to100(d);
        }

        public static Percent operator +(Percent a, Percent b)
        {
            return Percent.FromDecimal0to1(a.AsDecimal0to1 + b.AsDecimal0to1);
        }
    }

    public static class PercentExtensions
    {
        public static bool HasValue(this Percent p)
        {
            return p.AsDecimal0to1 != 0.0;
        }
    }
}
