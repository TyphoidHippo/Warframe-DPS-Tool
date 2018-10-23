using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFLib;

namespace WarframeDPSTool
{
    class ModBinding
    {
        public ModBinding(
            TextBox pCritChance,
            TextBox pCritDamage,
            TextBox pMultishot,
            TextBox pDamage,
            TextBox pImpact,
            TextBox pPuncture,
            TextBox pSlash,
            TextBox pFireRate,
            TextBox pReload,
            TextBox pMagazineSize,
            TextBox pCold,
            TextBox pHeat,
            TextBox pElectric,
            TextBox pToxin,
            TextBox pAugmentBonusDamage)
        {
            this.CritChance = new PercentBinding(pCritChance);
            this.CritDamage = new PercentBinding(pCritDamage);
            this.Multishot = new PercentBinding(pMultishot);
            this.Damage = new PercentBinding(pDamage);
            this.Impact = new PercentBinding(pImpact);
            this.Puncture = new PercentBinding(pPuncture);
            this.Slash = new PercentBinding(pSlash);
            this.FireRate = new PercentBinding(pFireRate);
            this.Reload = new PercentBinding(pReload);
            this.MagazineSize = new PercentBinding(pMagazineSize);
            this.Cold = new PercentBinding(pCold);
            this.Heat = new PercentBinding(pHeat);
            this.Electric = new PercentBinding(pElectric);
            this.Toxin = new PercentBinding(pToxin);
            if (pAugmentBonusDamage != null)
            {
                this.AugmentBonusDamage = new PercentBinding(pAugmentBonusDamage);
            }
        }

        public readonly PercentBinding CritChance;
        public readonly PercentBinding CritDamage;
        public readonly PercentBinding Multishot;
        public readonly PercentBinding Damage;
        public readonly PercentBinding Impact;
        public readonly PercentBinding Puncture;
        public readonly PercentBinding Slash;
        public readonly PercentBinding FireRate;
        public readonly PercentBinding Reload;
        public readonly PercentBinding MagazineSize;
        public readonly PercentBinding Cold;
        public readonly PercentBinding Heat;
        public readonly PercentBinding Electric;
        public readonly PercentBinding Toxin;
        public readonly PercentBinding AugmentBonusDamage;
    }

    static class ModBindingExtensions
    {
        public static void Clear(this ModBinding pThis)
        {
            pThis.CritChance.Clear();
            pThis.CritDamage.Clear();
            pThis.Multishot.Clear();
            pThis.Damage.Clear();
            pThis.Impact.Clear();
            pThis.Puncture.Clear();
            pThis.Slash.Clear();
            pThis.FireRate.Clear();
            pThis.Reload.Clear();
            pThis.MagazineSize.Clear();
            pThis.Cold.Clear();
            pThis.Heat.Clear();
            pThis.Electric.Clear();
            pThis.Toxin.Clear();
            if (pThis.AugmentBonusDamage != null) { pThis.AugmentBonusDamage.Clear(); }
        }
        public static void AddValues(this ModBinding pThis, Mod p)
        {
            pThis.AugmentBonusDamage.Value += p.AugmentBonus;
            pThis.Cold.Value += p.Cold;
            pThis.CritChance.Value += p.CritChance;
            pThis.CritDamage.Value += p.CritDamage;
            pThis.Damage.Value += p.Damage;
            pThis.Electric.Value += p.Electric;
            pThis.FireRate.Value += p.FireRate;
            pThis.Heat.Value += p.Heat;
            pThis.Impact.Value += p.Impact;
            pThis.MagazineSize.Value += p.MagazineSize;
            pThis.Multishot.Value += p.Multishot;
            pThis.Puncture.Value += p.Puncture;
            pThis.Reload.Value += p.Reload;
            pThis.Slash.Value += p.Slash;
            pThis.Toxin.Value += p.Toxin;
        }

        public static bool HasValue(this ModBinding pThis)
        {
            if (pThis.CritChance.Value.HasValue()) { return true; }
            if (pThis.CritDamage.Value.HasValue()) { return true; }
            if (pThis.Multishot.Value.HasValue()) { return true; }
            if (pThis.Damage.Value.HasValue()) { return true; }
            if (pThis.Impact.Value.HasValue()) { return true; }
            if (pThis.Puncture.Value.HasValue()) { return true; }
            if (pThis.Slash.Value.HasValue()) { return true; }
            if (pThis.FireRate.Value.HasValue()) { return true; }
            if (pThis.Reload.Value.HasValue()) { return true; }
            if (pThis.MagazineSize.Value.HasValue()) { return true; }
            if (pThis.Cold.Value.HasValue()) { return true; }
            if (pThis.Heat.Value.HasValue()) { return true; }
            if (pThis.Electric.Value.HasValue()) { return true; }
            if (pThis.Toxin.Value.HasValue()) { return true; }
            if (pThis.AugmentBonusDamage != null && pThis.AugmentBonusDamage.Value.HasValue()) { return true; }
            return false;
        }
        public static Mod ToMod(this ModBinding pThis, string pName)
        {
            return new Mod(pName,
                pThis.CritChance.Value,
                pThis.CritDamage.Value,
                pThis.MagazineSize.Value,
                pThis.Multishot.Value,
                pThis.Damage.Value,
                pThis.Impact.Value,
                pThis.Puncture.Value,
                pThis.Slash.Value,
                pThis.FireRate.Value,
                pThis.Reload.Value,
                pThis.Cold.Value,
                pThis.Heat.Value,
                pThis.Electric.Value,
                pThis.Toxin.Value,
                pThis.AugmentBonusDamage == null ? null : pThis.AugmentBonusDamage.Value);
        }
    }
}
