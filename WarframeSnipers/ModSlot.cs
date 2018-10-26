using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFLib;

namespace WarframeDPSTool
{
    class ModSlot
    {
        private readonly ComboBox _Wrapped;
        private readonly Action _OnChange;

        public ModSlot(ComboBox pWrapped, Action pOnChange)
        {
            this._Wrapped = pWrapped;
            this.Value = null;
            this._OnChange = pOnChange;
            this._Wrapped.SelectedValueChanged += _Wrapped_SelectedValueChanged;
        }

        private void _Wrapped_SelectedValueChanged(object sender, EventArgs e)
        {
            this._OnChange();
        }

        private void AssertText()
        {
            string s = "";
            if (this.Value != null)
            {
                s = this.Value.Name;
            }
            if (this._Wrapped.Text != s)
            {
                throw new NotImplementedException();
            }
        }

        public Mod Value
        {
            get { return (Mod)this._Wrapped.SelectedItem; }
            set
            {
                int i = -1;
                if (value != null) { i = this._Wrapped.Items.IndexOf(value); }
                if (this._Wrapped.SelectedIndex != i)
                {
                    this._Wrapped.SelectedIndex = i;
                }
                this.AssertText();
            }
        }

        public string Text
        {
            get { return this._Wrapped.Text; }
            set
            {
                int ii = -1;
                for(int i=0;i<this._Wrapped.Items.Count;i++)
                {
                    if(((Mod)this._Wrapped.Items[i]).Name == value)
                    {
                        ii = i;
                        break;
                    }
                }
                if (this._Wrapped.SelectedIndex != ii)
                {
                    this._Wrapped.SelectedIndex = ii;
                }

                this.AssertText();
            }
        }

        public void ClearChoices()
        {
            this._Wrapped.Items.Clear();
            this.AssertText();
        }

        public void AddChoices(IReadOnlyCollection<Mod> p)
        {
            this._Wrapped.Items.AddRange(p.ToArray());
        }
    }
}

namespace WarframeDPSTool
{
    static class ModSlotExtensions
    {
        private static class Defaults
        {
            public static IReadOnlyCollection<string> FromClass(WeaponClass p, bool pAllowHeavyCaliber)
            {
                if (p.HasFlag(WeaponClass.Rifle))
                {
                    if (pAllowHeavyCaliber) { return RifleHC; }
                    return RifleNoHC;
                }
                if (p.HasFlag(WeaponClass.Shotgun)) { return Shotgun; }
                if (p.HasFlag(WeaponClass.Secondary)) { return Secondary; }
                if (p.HasFlag(WeaponClass.Melee)) { return Melee; }
                throw new NotImplementedException();
            }

            private static readonly IReadOnlyCollection<string> Rifle = new string[]
            {
                "Hellfire",
                "PointStrike",
                "PrimedCryoRounds",
                "Serration",
                "SplitChamber",
                "Stormbringer",
                "VitalSense",
            };

            private static readonly IReadOnlyCollection<string> Shotgun = new string[]
            {
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
            };
            private static readonly IReadOnlyCollection<string> Melee = new string[]
            {
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
            };
            private static readonly IReadOnlyCollection<string> Secondary = new string[]
            {
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
            };

            private static readonly IReadOnlyCollection<string> RifleHC;
            private static readonly IReadOnlyCollection<string> RifleNoHC;
            static Defaults()
            {
                {
                    var v = new List<string>(Rifle);
                    v.Add("HeavyCaliber");
                    v.Sort();
                    RifleHC = v;
                }
                {
                    var v = new List<string>(Rifle);
                    v.Add("VigilanteArmaments");
                    v.Sort();
                    RifleNoHC = v;
                }
            }
        }

        public static void SetToDefaults(this IReadOnlyCollection<ModSlot> pThis, WeaponClass p, bool pAllowHeavyCaliber)
        {
            pThis.SetSelectedMods(Defaults.FromClass(p, pAllowHeavyCaliber));
        }
        public static void SetSelectedMods(this IReadOnlyCollection<ModSlot> pThis, IReadOnlyCollection<string> pModNames)
        {
            for (int i = 0; i < pModNames.Count; i++)
            {
                pThis.ElementAt(i).Text = pModNames.ElementAt(i);
            }
        }
        public static void SetSelectedMods(this IReadOnlyCollection<ModSlot> pThis, IReadOnlyCollection<Mod> pMods)
        {
            for (int i = 0; i < pMods.Count; i++)
            {
                pThis.ElementAt(i).Value = pMods.ElementAt(i);
            }
        }
        public static void ClearSelectedMods(this IReadOnlyCollection<ModSlot> pThis)
        {
            foreach(var slot in pThis)
            {
                slot.Value = null;
            }
        }
        public static void SetModChoices(this IReadOnlyCollection<ModSlot> pThis, IReadOnlyCollection<Mod> p)
        {
            foreach(var slot in pThis)
            {
                slot.ClearChoices();
                slot.AddChoices(p);
            }
        }
    }
}
