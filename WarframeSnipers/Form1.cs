using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WFLib;

namespace WarframeDPSTool
{
    public partial class Form1 : Form
    {
        class FormState
        {
            public FormState(Form1 pParent)
            {
                Action onChange = () => { using (pParent.MassStateChange) { } };
                this.MainMods = new ModBinding(
                    pParent.modMainCritChance,
                    pParent.modMainCritDamage,
                    pParent.modMainMultishot,
                    pParent.modMainDamage,
                    pParent.modMainImpact,
                    pParent.modMainPuncture,
                    pParent.modMainSlash,
                    pParent.modMainFireRate,
                    pParent.modMainReload,
                    pParent.modMainMagazineSize,
                    pParent.modMainCold,
                    pParent.modMainHeat,
                    pParent.modMainElectric,
                    pParent.modMainToxin,
                    pParent.modMainAugmentBonusDamage,
                    onChange);

                this.Riven1 = new ModBinding(
                    pParent.riven1CritChance,
                    pParent.riven1CritDamage,
                    pParent.riven1Multishot,
                    pParent.riven1Damage,
                    pParent.riven1Impact,
                    pParent.riven1Puncture,
                    pParent.riven1Slash,
                    pParent.riven1FireRate,
                    pParent.riven1Reload,
                    pParent.riven1MagazineSize,
                    pParent.riven1Cold,
                    pParent.riven1Heat,
                    pParent.riven1Electric,
                    pParent.riven1Toxin,
                    null,
                    onChange);

                this.Riven2 = new ModBinding(
                    pParent.riven2CritChance,
                    pParent.riven2CritDamage,
                    pParent.riven2Multishot,
                    pParent.riven2Damage,
                    pParent.riven2Impact,
                    pParent.riven2Puncture,
                    pParent.riven2Slash,
                    pParent.riven2FireRate,
                    pParent.riven2Reload,
                    pParent.riven2MagazineSize,
                    pParent.riven2Cold,
                    pParent.riven2Heat,
                    pParent.riven2Electric,
                    pParent.riven2Toxin,
                    null,
                    onChange);

                this.Health = new HealthBinding(
                    pParent.healthVsImpact,
                    pParent.healthVsPuncture,
                    pParent.healthVsSlash,
                    pParent.healthVsCold,
                    pParent.healthVsHeat,
                    pParent.healthVsElectric,
                    pParent.healthVsToxin,
                    pParent.healthVsViral,
                    pParent.healthVsRadiation,
                    pParent.healthVsGas,
                    pParent.healthVsMagnetic,
                    pParent.healthVsCorrosive,
                    pParent.healthVsBlast,
                    onChange);
            }

            public readonly ModBinding MainMods;
            public readonly ModBinding Riven1;
            public readonly ModBinding Riven2;
            public readonly HealthBinding Health;
            public Weapon Weapon;
            public readonly List<Elements> Elements = new List<Elements>();
            public readonly List<Elements[]> ElementChoices = new List<Elements[]>();

            public readonly List<Elements> Primitives = new List<Elements>();
            public readonly List<Elements> LastPrimitives = new List<Elements>();

            public Eidolon Eidolon = Eidolon.Terry;
            public DPSCase DPSCase = DPSCase.Average;
        }


        private readonly FormState State;
        private readonly List<Label> DynamicLabels = new List<Label>();
        private readonly List<ModSlot> ModSlots = new List<ModSlot>();
        private readonly List<TextBox> Riven1Inputs = new List<TextBox>();
        private readonly List<TextBox> Riven2Inputs = new List<TextBox>();

        private int _MassStateChange = 0;
        private ActionLock MassStateChange
        {
            get
            {
                this._MassStateChange++;
                if (this._MassStateChange == 1)
                {
                    this.SetDynamicLabels("Calculating...");
                    this.SetAutoModButtonsEnabled(false);
                }

                return new ActionLock(() =>
                {
                    this._MassStateChange--;

                    if (this._MassStateChange == 0)
                    {
                        if (this.State.Weapon == null)
                        {
                            this.SetDynamicLabels("Choose Weapon");
                            this.SetAutoModButtonsEnabled(false);
                        }
                        else
                        {
                            this.CheckPrimitivesForChanges();
                            int damageAfterNumberOfShots = 0;
                            int.TryParse(this.damageAfterNumberOfShotsInput.Text, out damageAfterNumberOfShots);

                            var dps = DPSCalculation.MainCalculation(this.State.Eidolon, this.State.DPSCase, this.State.Weapon, this.State.Elements.ToArray(), this.State.Health.ToHealth(), damageAfterNumberOfShots, this.State.MainMods.ToMod("MainMods"), this.rivenChoice1.Checked ? this.State.Riven1.ToMod("Riven1") : this.State.Riven2.ToMod("Riven2"));

                            this.toLimbBreak_NumReloads.Text = dps.ToLimbBreak_NumReloads.ToString();
                            this.toLimbBreak_NumShots.Text = dps.ToLimbBreak_NumShots.ToString();
                            this.toLimbBreak_Time.Text = String.Format("{0:0.00}", dps.ToLimbBreak_TimeSeconds);
                            this.toLimbBreak_TotalDamage.Text = String.Format("{0:0.00}", dps.ToLimbBreak_TotalDamage);
                            this.toLimbBreak_WastedDamage.Text = String.Format("{0:0.00}", (dps.ToLimbBreak_TotalDamage - dps.EidolonLimbHealth));

                            this.damageAfterNumberOfShotsOutput.Text = string.Format("{0:0.00}", dps.DamageAfterVariableNumberOfShots);

                            this.reloadTimeValue.Text = String.Format("{0:0.00}", dps.ReloadTime.TotalSeconds);
                            this.wfBuilderSustainedRawValue.Text = String.Format("{0:0.00}", dps.WFBuilderSustainedRaw);
                            this.wfBuilderSustainedDetailsValue.Text = String.Format("{0:0.00}", dps.WFBuilderSustainedDetails);

                            this.eidolonLimbHealth.Text = String.Format("{0:0.00}", dps.EidolonLimbHealth);
                            this.SetAutoModButtonsEnabled(true);

                            bool isRifle = this.State.Weapon.WeaponClass.HasFlag(WeaponClass.Rifle);
                            bool isSniper = this.State.Weapon.WeaponClass.HasFlag(WeaponClass.Sniper);
                            this.chkOptimizeIncludesHeavyCaliber.Enabled = isRifle;
                            this.chkArcaneMomentum1.Enabled = isSniper;
                            this.chkArcaneMomentum2.Enabled = isSniper;
                            if (!isRifle)
                            {
                                this.chkOptimizeIncludesHeavyCaliber.Checked = false;
                            }
                            if (!isSniper)
                            {
                                this.chkArcaneMomentum1.Checked = false;
                                this.chkArcaneMomentum2.Checked = false;
                            }
                        }
                    }
                });
            }
        }

        public Form1()
        {
            InitializeComponent();

            this.chkArcaneMomentum1.CheckedChanged += (s, e) => this.SlottedModsChanged();
            this.chkArcaneMomentum2.CheckedChanged += (s, e) => this.SlottedModsChanged();

            this.Text += string.Format(" - v:{0}", Application.ProductVersion);

            this.modMainElectric.TextChanged += this.CheckPrimitivesForChanges;
            this.modMainHeat.TextChanged += this.CheckPrimitivesForChanges;
            this.modMainCold.TextChanged += this.CheckPrimitivesForChanges;
            this.modMainToxin.TextChanged += this.CheckPrimitivesForChanges;

            this.riven1Electric.TextChanged += this.CheckPrimitivesForChanges;
            this.riven1Heat.TextChanged += this.CheckPrimitivesForChanges;
            this.riven1Cold.TextChanged += this.CheckPrimitivesForChanges;
            this.riven1Toxin.TextChanged += this.CheckPrimitivesForChanges;

            this.riven2Electric.TextChanged += this.CheckPrimitivesForChanges;
            this.riven2Heat.TextChanged += this.CheckPrimitivesForChanges;
            this.riven2Cold.TextChanged += this.CheckPrimitivesForChanges;
            this.riven2Toxin.TextChanged += this.CheckPrimitivesForChanges;


            this.State = new FormState(this);

            this.DynamicLabels.Add(this.wfBuilderSustainedDetailsValue);
            this.DynamicLabels.Add(this.wfBuilderSustainedRawValue);
            this.DynamicLabels.Add(this.reloadTimeValue);
            this.DynamicLabels.Add(this.toLimbBreak_NumReloads);
            this.DynamicLabels.Add(this.toLimbBreak_NumShots);
            this.DynamicLabels.Add(this.toLimbBreak_Time);
            this.DynamicLabels.Add(this.toLimbBreak_TotalDamage);
            this.DynamicLabels.Add(this.toLimbBreak_WastedDamage);

            this.ModSlots.Add(new ModSlot(this.modSlot1, this.SlottedModsChanged));
            this.ModSlots.Add(new ModSlot(this.modSlot2, this.SlottedModsChanged));
            this.ModSlots.Add(new ModSlot(this.modSlot3, this.SlottedModsChanged));
            this.ModSlots.Add(new ModSlot(this.modSlot4, this.SlottedModsChanged));
            this.ModSlots.Add(new ModSlot(this.modSlot5, this.SlottedModsChanged));
            this.ModSlots.Add(new ModSlot(this.modSlot6, this.SlottedModsChanged));
            this.ModSlots.Add(new ModSlot(this.modSlot7, this.SlottedModsChanged));
            this.ModSlots.Add(new ModSlot(this.modSlot8, this.SlottedModsChanged));


            this.SetDynamicLabels("Choose Weapon");

            this.sniperSelectionBox.Items.AddRange(Weapon.All.ToArray());

            this.btnHealthRobotic_Click(null, null);
            //this.btnMainModsDefault_Click(null, null);

            this.Riven1Inputs.Add(this.riven1Cold);
            this.Riven1Inputs.Add(this.riven1CritChance);
            this.Riven1Inputs.Add(this.riven1CritDamage);
            this.Riven1Inputs.Add(this.riven1Damage);
            this.Riven1Inputs.Add(this.riven1Electric);
            this.Riven1Inputs.Add(this.riven1FireRate);
            this.Riven1Inputs.Add(this.riven1Heat);
            this.Riven1Inputs.Add(this.riven1Impact);
            this.Riven1Inputs.Add(this.riven1MagazineSize);
            this.Riven1Inputs.Add(this.riven1Multishot);
            this.Riven1Inputs.Add(this.riven1Puncture);
            this.Riven1Inputs.Add(this.riven1Reload);
            this.Riven1Inputs.Add(this.riven1Slash);
            this.Riven1Inputs.Add(this.riven1Toxin);

            this.Riven2Inputs.Add(this.riven2Cold);
            this.Riven2Inputs.Add(this.riven2CritChance);
            this.Riven2Inputs.Add(this.riven2CritDamage);
            this.Riven2Inputs.Add(this.riven2Damage);
            this.Riven2Inputs.Add(this.riven2Electric);
            this.Riven2Inputs.Add(this.riven2FireRate);
            this.Riven2Inputs.Add(this.riven2Heat);
            this.Riven2Inputs.Add(this.riven2Impact);
            this.Riven2Inputs.Add(this.riven2MagazineSize);
            this.Riven2Inputs.Add(this.riven2Multishot);
            this.Riven2Inputs.Add(this.riven2Puncture);
            this.Riven2Inputs.Add(this.riven2Reload);
            this.Riven2Inputs.Add(this.riven2Slash);
            this.Riven2Inputs.Add(this.riven2Toxin);

            this.EnableRivens(false);
        }

        private void EnableRivens(bool pEnabled)
        {
            this.rivenChoice1.Enabled = pEnabled;
            this.rivenChoice2.Enabled = pEnabled;
            foreach (var v in this.Riven1Inputs)
            {
                v.Enabled = pEnabled;
                if (!pEnabled) { v.Text = ""; }
            }
            foreach (var v in this.Riven2Inputs)
            {
                v.Enabled = pEnabled;
                if (!pEnabled) { v.Text = ""; }
            }
        }

        private void SetDynamicLabels(string p)
        {
            foreach(var v in this.DynamicLabels)
            {
                v.Text = p;
            }
        }

        private void SetAutoModButtonsEnabled(bool p)
        {
            this.btnMainModsDefault.Enabled = p;
            this.btnMainModsOptimize.Enabled = p;
            this.btnMainModsClear.Enabled = p;
        }

        private static bool PrimitivesChanged(List<Elements> a, List<Elements> b)
        {
            if(a.Count != b.Count) { return true; }
            foreach(var v in a)
            {
                if (!b.Contains(v)) { return true; }
            }
            return false;
        }

        private static bool AllNullOrWhitespaceOrZero(params TextBox[] p)
        {
            foreach(var v in p)
            {
                if (!string.IsNullOrWhiteSpace(v.Text))
                {
                    double d = 0.0;
                    double.TryParse(v.Text, out d);
                    if (d != 0.0) { return false; }
                }
            }
            return true;
        }

        private void ElementChoiceChanged()
        {
            this.State.Elements.Clear();
            Elements[] elems = null;
            int count = this.State.ElementChoices.Count;
            if(count <= 1)
            {
                this.elementChoice1.Checked = true;
            }
            else if(count <3 && this.elementChoice3.Checked)
            {
                this.elementChoice2.Checked = true;
            }

            if (this.elementChoice1.Checked) { elems = this.State.ElementChoices[0]; }
            if (this.elementChoice2.Checked) { elems = this.State.ElementChoices[1]; }
            if (this.elementChoice3.Checked) { elems = this.State.ElementChoices[2]; }

            this.State.Elements.AddRange(elems != null ? elems : new Elements[] { });
        }

        private void ElementChoicesChanged()
        {
            var elementChoices = ElementCombinations.PossibleCombinations(this.State.Primitives.ToArray());

            this.elementChoice3.Enabled = elementChoices.Length > 2;
            this.elementChoice2.Enabled = elementChoices.Length > 1;
            this.elementChoice1.Enabled = elementChoices.Length > 0;

            Action<Elements[], RadioButton> SetChoiceText = (e, b) =>
            {
                string s = e.Length>0 ? e[0].ToString() : "";
                for(int i=1; i<e.Length;i++)
                {
                    s += ", " + e[i];
                }
                b.Text = s;
            };

            var ec0 = elementChoices.Length > 0 ? elementChoices[0] : new Elements[] { };
            var ec1 = elementChoices.Length > 1 ? elementChoices[1] : new Elements[] { };
            var ec2 = elementChoices.Length > 2 ? elementChoices[2] : new Elements[] { };

            SetChoiceText(ec0, this.elementChoice1);
            SetChoiceText(ec1, this.elementChoice2);
            SetChoiceText(ec2, this.elementChoice3);

            this.State.ElementChoices.Clear();
            this.State.ElementChoices.AddRange(elementChoices);

            this.ElementChoiceChanged();
        }

        private void CheckPrimitivesForChanges(object sender, EventArgs e)
        { this.CheckPrimitivesForChanges(); }
        private void CheckPrimitivesForChanges()
        {
            this.State.Primitives.Clear();
            var E = this.riven1Electric;
            var H = this.riven1Heat;
            var C = this.riven1Cold;
            var T = this.riven1Toxin;

            if(this.rivenChoice2.Checked)
            {
                E = this.riven2Electric;
                H = this.riven2Heat;
                C = this.riven2Cold;
                T = this.riven2Toxin;
            }

            if (!AllNullOrWhitespaceOrZero(this.modMainElectric, E )) { this.State.Primitives.Add(Elements.Electric); }
            if (!AllNullOrWhitespaceOrZero(this.modMainHeat, H )) { this.State.Primitives.Add(Elements.Heat); }
            if (!AllNullOrWhitespaceOrZero(this.modMainCold, C )) { this.State.Primitives.Add(Elements.Cold); }
            if (!AllNullOrWhitespaceOrZero(this.modMainToxin, T )) { this.State.Primitives.Add(Elements.Toxin); }

            if (PrimitivesChanged(this.State.Primitives, this.State.LastPrimitives))
            {
                this.State.LastPrimitives.Clear();
                this.State.LastPrimitives.AddRange(this.State.Primitives);
                this.ElementChoicesChanged();
            }
        }

        private void SlottedModsChanged()
        {
            using (this.MassStateChange)
            {
                this.State.MainMods.Clear();
                bool enableRivens = false;
                foreach (var slot in this.ModSlots)
                {
                    if (slot.Value != null)
                    {
                        this.State.MainMods.AddValues(slot.Value);
                        if (slot.Value == MainMods.Riven) { enableRivens = true; }
                    }
                }
                if (this.chkArcaneMomentum1.Checked) { this.State.MainMods.AddValues(Arcanes.Momentum); }
                if (this.chkArcaneMomentum2.Checked) { this.State.MainMods.AddValues(Arcanes.Momentum); }
                this.EnableRivens(enableRivens);
            }
        }

        private void EidolonChoiceChanged()
        {
            if (this.eidoChoiceTerry.Checked) { this.State.Eidolon = Eidolon.Terry; }
            if (this.eidoChoiceGary.Checked) { this.State.Eidolon = Eidolon.Gary; }
            if (this.eidoChoiceHarry.Checked) { this.State.Eidolon = Eidolon.Harry; }
        }
        private void DPSCaseChanged()
        {
            if (this.outputCaseAverage.Checked) { this.State.DPSCase = DPSCase.Average; }
            if (this.outputCaseBest.Checked) { this.State.DPSCase = DPSCase.Best; }
            if (this.outputCaseWorst.Checked) { this.State.DPSCase = DPSCase.Worst; }
        }

        private void sniperSelectionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                var lastClass = this.State.Weapon == null ? WeaponClass.None : this.State.Weapon.WeaponClass;

                this.State.Weapon = (Weapon)this.sniperSelectionBox.SelectedItem;

                if(lastClass!=this.State.Weapon.WeaponClass)
                {
                    bool isRifle = this.State.Weapon.WeaponClass.HasFlag(WeaponClass.Rifle);
                    this.chkOptimizeIncludesHeavyCaliber.Enabled = isRifle;
                    if (!isRifle)
                    {
                        this.chkOptimizeIncludesHeavyCaliber.Checked = false;
                    }

                    this.ModSlots.ClearSelectedMods();
                    this.ModSlots.SetModChoices(MainMods.AllMods(this.State.Weapon.WeaponClass, this.chkOptimizeIncludesHeavyCaliber.Checked, this.State.Weapon.AugmentNames));
                    this.ModSlots.SetToDefaults(this.State.Weapon.WeaponClass, this.chkOptimizeIncludesHeavyCaliber.Checked);
                }
            }
        }

        private void btnRiven1Clear_Click(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.State.Riven1.Clear();
            }
        }

        private void btnRiven2Clear_Click(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.State.Riven2.Clear();
            }
        }

        private void btnHealthRobotic_Click(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.State.Health.SetToRobotic();
            }
        }

        private void btnHealthClear_Click(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.State.Health.Clear();
            }
        }

        private void rivenChoice1_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange) { }
        }

        private void elementChoice1_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.ElementChoiceChanged();
            }
        }

        private void elementChoice2_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.ElementChoiceChanged();
            }
        }

        private void elementChoice3_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.ElementChoiceChanged();
            }
        }

        private void damageAfterNumberOfShotsInput_TextChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                int i = 0;
                int.TryParse(this.damageAfterNumberOfShotsInput.Text, out i);
                if (i > 100)
                {
                    this.damageAfterNumberOfShotsInput.Text = "100";
                }
            }
        }

        private void eidoChoiceTerry_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.EidolonChoiceChanged();
            }
        }

        private void eidoChoiceGary_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.EidolonChoiceChanged();
            }
        }

        private void eidoChoiceHarry_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.EidolonChoiceChanged();
            }
        }

        private void outputCaseAverage_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.DPSCaseChanged();
            }
        }

        private void outputCaseBest_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.DPSCaseChanged();
            }
        }

        private void outputCaseWorst_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.DPSCaseChanged();
            }
        }

        private void btnMainModsDefault_Click(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.State.MainMods.Clear();
                this.ModSlots.SetToDefaults(this.State.Weapon.WeaponClass, this.chkOptimizeIncludesHeavyCaliber.Checked);
                this.SlottedModsChanged();
            }
        }

        private void btnMainModsClear_Click(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.State.MainMods.Clear();
                this.ModSlots.ClearSelectedMods();
            }
        }

        private void btnMainModsOptimize_Click(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                int arcaneReload = 0;
                if (this.chkArcaneMomentum1.Checked) { arcaneReload += 100; }
                if (this.chkArcaneMomentum2.Checked) { arcaneReload += 100; }
                Mod arcanes = new Mod("Arcanes", WeaponClass.Sniper, 0, 0, 0, 0, 0, 0, 0, 0, 0, arcaneReload, 0, 0, 0, 0, 0);
                var bestMods = DPSCalculation.FindBestMods(
                    this.State.Eidolon, this.State.DPSCase, this.State.Weapon, this.State.Health.ToHealth(),
                    this.State.Riven1.HasValue() ? this.State.Riven1.ToMod("Riven1") : null,
                    this.State.Riven2.HasValue() ? this.State.Riven2.ToMod("Riven2") : null,
                    arcanes, this.chkOptimizeIncludesHeavyCaliber.Checked
                    );

                Mod riven = null;
                for (int i = bestMods.Mods.Count - 1; i >= 0; i--)
                {
                    if (bestMods.Mods[i].Name.StartsWith("Riven"))
                    {
                        riven = bestMods.Mods[i];
                        bestMods.Mods.RemoveAt(i);
                        break;
                    }
                }

                this.State.MainMods.Clear();
                this.ModSlots.SetSelectedMods(bestMods.Mods);

                if (riven != null)
                {
                    this.ModSlots.Last().Text = MainMods.Riven.Name;
                    if (riven.Name.EndsWith("1"))
                    {
                        this.rivenChoice1.PerformClick();
                    }
                    else if (riven.Name.EndsWith("2"))
                    {
                        this.rivenChoice2.PerformClick();
                    }
                    else
                    { throw new NotImplementedException(); }
                }

                this.SlottedModsChanged();

                this.ElementChoicesChanged();

                for (int i = 0; i < this.State.ElementChoices.Count; i++)
                {
                    bool match = true;
                    foreach (var elem in bestMods.Elements)
                    {
                        if (!this.State.ElementChoices[i].Contains(elem))
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        switch (i)
                        {
                            default: throw new NotImplementedException();
                            case 0:
                                this.elementChoice1.PerformClick();
                                break;
                            case 1:
                                this.elementChoice2.PerformClick();
                                break;
                            case 2:
                                this.elementChoice3.PerformClick();
                                break;
                        }
                        break;
                    }
                }
            }
        }

        private void chkOptimizeIncludesHeavyCaliber_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
                this.ModSlots.ClearSelectedMods();
                this.ModSlots.SetModChoices(MainMods.AllMods(this.State.Weapon.WeaponClass, this.chkOptimizeIncludesHeavyCaliber.Checked, this.State.Weapon.AugmentNames));
                this.ModSlots.SetToDefaults(this.State.Weapon.WeaponClass, this.chkOptimizeIncludesHeavyCaliber.Checked);
            }
        }

        private void chkArcaneMomentum1_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
            }
        }

        private void chkArcaneMomentum2_CheckedChanged(object sender, EventArgs e)
        {
            using (this.MassStateChange)
            {
            }
        }
    }
}
