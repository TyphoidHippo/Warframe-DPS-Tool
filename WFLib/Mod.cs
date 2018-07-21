using System;
using System.Collections.Generic;
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
    }

    public static class MainMods
    {
        public static readonly Mod Serration = new Mod("Serration", 0, 0, 0, 0, 165, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Mod HeavyCaliber = new Mod("HeavyCaliber", 0, 0, 0, 0, 165, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public static readonly Mod SplitChamber = new Mod("SplitChamber", 0, 0, 0, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Mod VigilanteArmaments = new Mod("VigilanteArmaments", 0, 0, 0, 60, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public static readonly Mod PointStrike = new Mod("PointStrike", 150, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Mod VitalSense = new Mod("VitalSense", 0, 120, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public static readonly Mod PrimedCryoRounds = new Mod("PrimedCryoRounds", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 165, 0, 0, 0, 0);
        public static readonly Mod Hellfire = new Mod("Hellfire", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90, 0, 0, 0);
        public static readonly Mod Stormbringer = new Mod("Stormbringer", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90, 0, 0);
        public static readonly Mod InfectedClip = new Mod("InfectedClip", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90, 0);

        public static readonly Mod VileAcceleration = new Mod("VileAcceleration", 0, 0, 0, 0, -15, 0, 0, 0, 90, 0, 0, 0, 0, 0, 0);
        public static readonly Mod SpeedTrigger = new Mod("SpeedTrigger", 0, 0, 0, 0, 0, 0, 0, 0, 60, 0, 0, 0, 0, 0, 0);
        public static readonly Mod PrimedFastHands = new Mod("PrimedFastHands", 0, 0, 0, 0, 0, 0, 0, 0, 0, 55, 0, 0, 0, 0, 0);

        public static readonly Mod FangedFusillade = new Mod("FangedFusillade", 0, 0, 0, 0, 0, 0, 0, 120, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Mod CrashCourse = new Mod("CrashCourse", 0, 0, 0, 0, 0, 120, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Mod PiercingCaliber = new Mod("PiercingCaliber", 0, 0, 0, 0, 0, 0, 120, 0, 0, 0, 0, 0, 0, 0, 0);

        public static readonly Mod LastingPurity = new Mod("LastingPurity", 0, 0, 0, 0, 0, 0, 0, 120, 0, 0, 0, 0, 0, 0, 0);

        public static readonly Mod Riven = new Mod("Riven", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        private static readonly IReadOnlyCollection<Mod> _AllMods = null;
        private static readonly IReadOnlyCollection<Mod> _AllModsNoHeavyCaliber = null;
        public static IReadOnlyCollection<Mod> AllMods(bool pIncludeHeavyCaliber)
        {
            return pIncludeHeavyCaliber ? _AllMods : _AllModsNoHeavyCaliber;
        }
        //public static readonly IReadOnlyCollection<Mod> TestMods = new Mod[]
        //{
        //    Serration, HeavyCaliber, SplitChamber,
        //    VigilanteArmaments, PointStrike, VitalSense,
        //    VileAcceleration,
        //    PrimedFastHands,
        //    PrimedCryoRounds
        //};

        static MainMods()
        {
            var allMods = new List<Mod>();
            var allFields = typeof(MainMods).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var fi in allFields)
            {
                if (fi.FieldType == typeof(Mod))
                {
                    var instance = fi.GetValue(null);
                    allMods.Add((Mod)instance);
                }
            }

            _AllMods = allMods;
            var noHC = new List<Mod>(allMods);
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