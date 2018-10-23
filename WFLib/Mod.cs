using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFLib
{
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public class Mod
    {
        public Mod(
            string pName,
            Percent pCritChance,
            Percent pCritDamage,
            Percent pMagazineSize,
            Percent pMultishot,
            Percent pDamage,
            Percent pImpact,
            Percent pPuncture,
            Percent pSlash,
            Percent pFireRate,
            Percent pReload,
            Percent pCold,
            Percent pHeat,
            Percent pElectric,
            Percent pToxin,
            Percent pAugmentBonus)
        {
            this.Name = pName;
            this.CritChance = pCritChance;
            this.CritDamage = pCritDamage;
            this.MagazineSize = pMagazineSize;
            this.Multishot = pMultishot;
            this.Damage = pDamage;
            this.Impact = pImpact;
            this.Puncture = pPuncture;
            this.Slash = pSlash;
            this.FireRate = pFireRate;
            this.Reload = pReload;
            this.Cold = pCold;
            this.Heat = pHeat;
            this.Electric = pElectric;
            this.Toxin = pToxin;
            this.AugmentBonus = pAugmentBonus;
        }
        public readonly string Name;
        public readonly Percent CritChance;
        public readonly Percent CritDamage;
        public readonly Percent MagazineSize;
        public readonly Percent Multishot;
        public readonly Percent Damage;
        public readonly Percent Impact;
        public readonly Percent Puncture;
        public readonly Percent Slash;
        public readonly Percent FireRate;
        public readonly Percent Reload;
        public readonly Percent Cold;
        public readonly Percent Heat;
        public readonly Percent Electric;
        public readonly Percent Toxin;
        public readonly Percent AugmentBonus;

        public bool IsAugment { get { return this.AugmentBonus.AsDecimal0to1 != 0; } }
    }

    public static class MainMods
    {
        private static readonly Mod HeavyCaliber = new Mod("HeavyCaliber", 0, 0, 0, 0, 165, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Mod Riven = new Mod("Riven", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        private static readonly IReadOnlyCollection<Mod> _AllMods = null;
        private static readonly IReadOnlyCollection<Mod> _AllModsNoHeavyCaliber = null;
        public static IReadOnlyCollection<Mod> AllMods(bool pIncludeHeavyCaliber)
        {
            return pIncludeHeavyCaliber ? _AllMods : _AllModsNoHeavyCaliber;
        }

        static MainMods()
        {
            var allMods = new Dictionary<string, Mod>();
            allMods.Add(Riven.Name, Riven);
            allMods.Add(HeavyCaliber.Name, HeavyCaliber);

            var ass = System.Reflection.Assembly.GetEntryAssembly();
            var path = System.IO.Path.GetDirectoryName(ass.Location);

            foreach (var v in System.IO.File.ReadAllLines(path + "\\Mods.ini"))
            {
                string line = v;
                if (line == null) { continue; }
                line = line.Trim();
                if (string.IsNullOrWhiteSpace(line)) { continue; }
                if (line.StartsWith("#")) { continue; }

                var parts = line.Split(',').ToList();
                string name = parts[0].Trim();

                if (string.IsNullOrWhiteSpace(name)) { continue; }
                if (allMods.ContainsKey(name)) { continue; }

                if (parts.Count != 16) { continue; }

                try
                {
                    int ii = 0;
                    allMods.Add(name, new Mod(parts[ii++],
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++])),
                        Percent.FromPercent0to100(double.Parse(parts[ii++]))
                        ));
                }
                catch (Exception ex)
                {
                    var vv = ex;
                    if (Debugger.IsAttached) { Debugger.Break(); }
                }
            }


            _AllMods = allMods.Values;
            var noHC = new List<Mod>(_AllMods);
            noHC.Remove(HeavyCaliber);
            _AllModsNoHeavyCaliber = noHC;
        }
    }

    public static class Arcanes
    {
        public static readonly Mod Momentum = new Mod("Arcane Momentum", 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0);
    }

    public static class ModExtensions
    {
        public static Mod Consolidate(this IReadOnlyCollection<Mod> p, string pName)
        {
            Percent CritChance = Percent.Zero;
            Percent CritDamage = Percent.Zero;
            Percent MagazineSize = Percent.Zero;
            Percent Multishot = Percent.Zero;
            Percent Damage = Percent.Zero;
            Percent Impact = Percent.Zero;
            Percent Puncture = Percent.Zero;
            Percent Slash = Percent.Zero;
            Percent FireRate = Percent.Zero;
            Percent Reload = Percent.Zero;
            Percent Cold = Percent.Zero;
            Percent Heat = Percent.Zero;
            Percent Electric = Percent.Zero;
            Percent Toxin = Percent.Zero;
            Percent AugmentBonus = Percent.Zero;

            foreach (var mod in p)
            {
                CritChance += mod.CritChance;
                CritDamage += mod.CritDamage;
                MagazineSize += mod.MagazineSize;
                Multishot += mod.Multishot;
                Damage += mod.Damage;
                Impact += mod.Impact;
                Puncture += mod.Puncture;
                Slash += mod.Slash;
                FireRate += mod.FireRate;
                Reload += mod.Reload;
                Cold += mod.Cold;
                Heat += mod.Heat;
                Electric += mod.Electric;
                Toxin += mod.Toxin;
                AugmentBonus += mod.AugmentBonus;
            }

            return new Mod(
                pName,
                CritChance,
                CritDamage,
                MagazineSize,
                Multishot,
                Damage,
                Impact,
                Puncture,
                Slash,
                FireRate,
                Reload,
                Cold,
                Heat,
                Electric,
                Toxin,
                AugmentBonus);
        }
    }
}