using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFLib
{
    public class Health
    {
        public Health(
            Percent pVsImpact,
            Percent pVsPuncture,
            Percent pVsSlash,
            Percent pVsCold,
            Percent pVsHeat,
            Percent pVsElectric,
            Percent pVsToxin,
            Percent pVsViral,
            Percent pVsRadiation,
            Percent pVsGas,
            Percent pVsMagnetic,
            Percent pVsCorrosive,
            Percent pVsBlast)
        {
            this.VsImpact = pVsImpact;
            this.VsPuncture = pVsPuncture;
            this.VsSlash = pVsSlash;
            this.VsCold = pVsCold;
            this.VsHeat = pVsHeat;
            this.VsElectric = pVsElectric;
            this.VsToxin = pVsToxin;
            this.VsViral = pVsViral;
            this.VsRadiation = pVsRadiation;
            this.VsGas = pVsGas;
            this.VsMagnetic = pVsMagnetic;
            this.VsCorrosive = pVsCorrosive;
            this.VsBlast = pVsBlast;
        }

        public readonly Percent VsImpact;
        public readonly Percent VsPuncture;
        public readonly Percent VsSlash;
        public readonly Percent VsCold;
        public readonly Percent VsHeat;
        public readonly Percent VsElectric;
        public readonly Percent VsToxin;
        public readonly Percent VsViral;
        public readonly Percent VsRadiation;
        public readonly Percent VsGas;
        public readonly Percent VsMagnetic;
        public readonly Percent VsCorrosive;
        public readonly Percent VsBlast;

        public static readonly Health Robotic = new Health(
            0,
            25,
            -25,
            0,
            -50,
            50,
            -25,
            0,
            25,
            0,
            0,
            0,
            0);
    }
}
