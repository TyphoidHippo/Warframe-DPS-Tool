﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFLib;

namespace WarframeDPSTool
{
    class HealthBinding
    {
        public HealthBinding(
            TextBox pVsImpact,
            TextBox pVsPuncture,
            TextBox pVsSlash,
            TextBox pVsCold,
            TextBox pVsHeat,
            TextBox pVsElectric,
            TextBox pVsToxin,
            TextBox pVsViral,
            TextBox pVsRadiation,
            TextBox pVsGas,
            TextBox pVsMagnetic,
            TextBox pVsCorrosive,
            TextBox pVsBlast)
        {
            this.VsImpact = new PercentBinding(pVsImpact);
            this.VsPuncture = new PercentBinding(pVsPuncture);
            this.VsSlash = new PercentBinding(pVsSlash);
            this.VsCold = new PercentBinding(pVsCold);
            this.VsHeat = new PercentBinding(pVsHeat);
            this.VsElectric = new PercentBinding(pVsElectric);
            this.VsToxin = new PercentBinding(pVsToxin);
            this.VsViral = new PercentBinding(pVsViral);
            this.VsRadiation = new PercentBinding(pVsRadiation);
            this.VsGas = new PercentBinding(pVsGas);
            this.VsMagnetic = new PercentBinding(pVsMagnetic);
            this.VsCorrosive = new PercentBinding(pVsCorrosive);
            this.VsBlast = new PercentBinding(pVsBlast);
        }
        public readonly PercentBinding VsImpact;
        public readonly PercentBinding VsPuncture;
        public readonly PercentBinding VsSlash;
        public readonly PercentBinding VsCold;
        public readonly PercentBinding VsHeat;
        public readonly PercentBinding VsElectric;
        public readonly PercentBinding VsToxin;
        public readonly PercentBinding VsViral;
        public readonly PercentBinding VsRadiation;
        public readonly PercentBinding VsGas;
        public readonly PercentBinding VsMagnetic;
        public readonly PercentBinding VsCorrosive;
        public readonly PercentBinding VsBlast;
    }
}


namespace WarframeDPSTool
{
    static class HealthBindingExtensions
    {
        public static void SetToRobotic(this HealthBinding p)
        {
            p.VsImpact.Value = 0;
            p.VsPuncture.Value = 25;
            p.VsSlash.Value = -25;
            p.VsCold.Value = 0;
            p.VsHeat.Value = -50;
            p.VsElectric.Value = 50;
            p.VsToxin.Value = -25;
            p.VsViral.Value = 0;
            p.VsRadiation.Value = 25;
            p.VsGas.Value = 0;
            p.VsMagnetic.Value = 0;
            p.VsCorrosive.Value = 0;
            p.VsBlast.Value = 0;
        }
        public static void Clear(this HealthBinding pThis)
        {
            pThis.VsImpact.Clear();
            pThis.VsPuncture.Clear();
            pThis.VsSlash.Clear();
            pThis.VsCold.Clear();
            pThis.VsHeat.Clear();
            pThis.VsElectric.Clear();
            pThis.VsToxin.Clear();
            pThis.VsViral.Clear();
            pThis.VsRadiation.Clear();
            pThis.VsGas.Clear();
            pThis.VsMagnetic.Clear();
            pThis.VsCorrosive.Clear();
            pThis.VsBlast.Clear();
        }
        public static Health ToHealth(this HealthBinding pThis)
        {
            return new Health(
                pThis.VsImpact.Value,
                pThis.VsPuncture.Value,
                pThis.VsSlash.Value,
                pThis.VsCold.Value,
                pThis.VsHeat.Value,
                pThis.VsElectric.Value,
                pThis.VsToxin.Value,
                pThis.VsViral.Value,
                pThis.VsRadiation.Value,
                pThis.VsGas.Value,
                pThis.VsMagnetic.Value,
                pThis.VsCorrosive.Value,
                pThis.VsBlast.Value);
        }
    }
}