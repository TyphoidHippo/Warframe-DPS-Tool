using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFLib
{
    public class Sniper
    {
        public Sniper(
            string pName,
            double pImpact,
            double pPuncture,
            double pSlash,
            double pCold,
            double pElectric,
            double pHeat,
            double pToxin,
            double pMagnetic,
            double pBlast,
            double pViral,
            double pRadiation,
            double pCorrosive,
            double pGas,
            int pPelletsTillx15,
            double pFireRate,
            int pMagazine,
            double pReload,
            Percent pCritChance,
            double pCritMultiplier,
            Percent pZoomBonusCC,
            Percent pZoomBonusCD,
            bool pFireRateCappedAt10)
        {
            this.Name = pName;
            this.Impact = pImpact;
            this.Puncture = pPuncture;
            this.Slash = pSlash;

            this.Cold = pCold;
            this.Electric = pElectric;
            this.Heat = pHeat;
            this.Toxin = pToxin;

            this.Magnetic = pMagnetic;
            this.Blast = pBlast;
            this.Viral = pViral;
            this.Radiation = pRadiation;
            this.Corrosive = pCorrosive;
            this.Gas = pGas;

            this.PelletsTillx15 = pPelletsTillx15;
            this.FireRate = pFireRate;
            this.Magazine = pMagazine;
            this.Reload = pReload;
            this.CritChance = pCritChance;
            this.CritMultiplier = pCritMultiplier;
            this.ZoomBonusCC = pZoomBonusCC;
            this.ZoomBonusCD = pZoomBonusCD;
            this.FireRateCappedAt10 = pFireRateCappedAt10;

            if(this.PelletsTillx15 == 0) { this.PelletsTillx15 = int.MaxValue; }
        }

        public readonly string Name;

        public readonly double Impact;
        public readonly double Puncture;
        public readonly double Slash;

        public readonly double Cold;
        public readonly double Electric;
        public readonly double Heat;
        public readonly double Toxin;

        public readonly double Magnetic;
        public readonly double Blast;
        public readonly double Viral;
        public readonly double Radiation;
        public readonly double Corrosive;
        public readonly double Gas;

        public readonly int PelletsTillx15;
        public readonly double FireRate;
        public readonly int Magazine;
        public readonly double Reload;
        public readonly Percent CritChance;
        public readonly double CritMultiplier;
        public readonly Percent ZoomBonusCC;
        public readonly Percent ZoomBonusCD;
        public readonly bool FireRateCappedAt10;
        public double TotalBaseDamage
        {
            get
            {
                return this.Impact
                  + this.Puncture
                  + this.Slash
                  + this.Electric
                  + this.Cold
                  + this.Heat
                  + this.Toxin
                  + this.Magnetic
                  + this.Blast
                  + this.Viral
                  + this.Radiation
                  + this.Corrosive
                  + this.Gas;
            }
        }

        public static readonly IReadOnlyCollection<Sniper> AllSnipers = null;

        static Sniper()
        {
            var allSnipers = new List<Sniper>();

            var ass = System.Reflection.Assembly.GetEntryAssembly();
            var path = System.IO.Path.GetDirectoryName(ass.Location);

            foreach (var v in System.IO.File.ReadAllLines(path + "\\Weapons.ini"))
            {
                string line = v;
                if(line == null) { continue; }
                line = line.Trim();
                if (string.IsNullOrWhiteSpace(line)) { continue; }
                if (line.StartsWith("#")) { continue; }

                var parts = line.Split(',');
                string name = parts[0].Trim();

                if (parts.Length != 23) { continue; }

                try
                {
                    int ii = 0;
                    allSnipers.Add(new Sniper(
                        parts[ii++],
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        int.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        int.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        int.Parse(parts[ii++]),
                        double.Parse(parts[ii++]),
                        int.Parse(parts[ii++]),
                        int.Parse(parts[ii++]),
                        bool.Parse(parts[ii++])
                        ));
                }
                catch(Exception ex)
                {
                    var vv = ex;
                    if (Debugger.IsAttached) { Debugger.Break(); }
                }
            }

            AllSnipers = allSnipers;
        }
    }
}