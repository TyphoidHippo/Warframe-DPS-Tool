using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFLib
{
    public enum Eidolon
    {
        Terry,
        Gary,
        Harry
    }
    public enum DPSCase
    {
        Average,
        Best,
        Worst
    }
    public static class DPSCalculation
    {
        public struct DPSResult
        {
            public int ToLimbBreak_NumShots;
            public int ToLimbBreak_NumReloads;
            public double ToLimbBreak_TotalDamage;
            public double ToLimbBreak_TimeSeconds;

            public double EidolonLimbHealth;

            public TimeSpan ReloadTime;

            public double WFBuilderSustainedRaw;
            public double WFBuilderSustainedDetails;

            public double DamageAfterVariableNumberOfShots;
        }
        public class BestModsResult
        {
            public BestModsResult(List<Mod> pMods, Elements[] pElements, DPSResult pDPSResult)
            {
                this.Mods = pMods;
                this.Elements = pElements;
                this.DPSResult = pDPSResult;
            }
            public readonly List<Mod> Mods;
            public readonly Elements[] Elements;
            public readonly DPSResult DPSResult;
        }

        public static DPSResult MainCalculation(Eidolon pEidolon, DPSCase pCase, Sniper pSniper, Elements[] pElements, Health pHealthType, int pNumberOfUserShotsToCalculateDamageFor, params Mod[] pMods)
        {
            var dps = DPSBreakdown.FromPrimitives(pSniper, pCase, pElements, pHealthType, pMods);

            double MagazineTime = (dps.Magazine / dps.FireRate) + dps.Reload;
            double BulletsPerSecond = dps.Magazine / MagazineTime;

            int baseLevel = 0;
            switch (pEidolon)
            {
                case Eidolon.Terry: baseLevel = 50; break;
                case Eidolon.Gary: baseLevel = 55; break;
                case Eidolon.Harry: baseLevel = 60; break;
                default: throw new NotImplementedException("Unknown Eidolon: " + pEidolon.ToString());
            }

            DPSResult result = new DPSResult();
            //Current Health = Base Health × (1 + (Current Level − Base Level )^2 × 0.015 )
            result.EidolonLimbHealth = 15000 * (1.0 + ((baseLevel - 1) * (baseLevel - 1)) * 0.015);


            var simInput = new DPSCalcPerformanceCritical.SimulationInput();
            simInput.LimbHealth = result.EidolonLimbHealth;
            simInput.Magazine = dps.Magazine;
            simInput.FireRate = dps.FireRate;
            simInput.NumberOfUserShotsToCalculateDamageFor = pNumberOfUserShotsToCalculateDamageFor;
            simInput.PelletsTillx15 = pSniper.PelletsTillx15;
            simInput.Case = pCase;
            simInput.TotalMultishot = dps.TotalMultishot;
            simInput.TotalDamageWithAllFactorsIncluded = dps.TotalDamage_6_WithHealthTypeFactors;

            var simResult = DPSCalcPerformanceCritical.Simulate(simInput);

            result.ToLimbBreak_NumReloads = simResult.NumReloads;
            result.ToLimbBreak_NumShots = simResult.NumShots;
            result.ToLimbBreak_TotalDamage = simResult.TotalDamage;
            result.ToLimbBreak_TimeSeconds = (simResult.NumShots * (1.0 / dps.FireRate)) + (simResult.NumReloads * dps.Reload);
            result.DamageAfterVariableNumberOfShots = simResult.DamageAfterUserNumberOfShots;

            result.WFBuilderSustainedRaw = dps.TotalDamage_5_WithMultishot * BulletsPerSecond;
            result.WFBuilderSustainedDetails = dps.TotalDamage_6_WithHealthTypeFactors * BulletsPerSecond;
            result.ReloadTime = TimeSpan.FromSeconds(dps.Reload);
            return result;
        }


        public static List<List<T>> Permutations<T>(
            IReadOnlyCollection<T> inputList, int minimumItems = 1, int maximumItems = int.MaxValue)
        {
            int comboCount = (int)Math.Pow(2, inputList.Count) - 1;
            List<List<T>> result = new List<List<T>>(comboCount + 1);

            if (minimumItems == 0)  // Optimize default case
                result.Add(new List<T>());

            for (int i = 1; i <= comboCount; i++)
            {
                List<T> thisCombination = new List<T>(inputList.Count);
                for (int j = 0; j < inputList.Count; j++)
                {
                    if ((i >> j & 1) == 1)
                        thisCombination.Add(inputList.ElementAt(j));
                }

                if (thisCombination.Count >= minimumItems && thisCombination.Count <= maximumItems)
                {
                    result.Add(thisCombination);
                }
            }

            return result;
        }


        private static BestModsResult FindBestMods(Eidolon pEidolon, DPSCase pCase, Sniper pSniper, Health pHealthType, IReadOnlyCollection<Mod> pMods, Mod pArcanes)
        {
            var perms = Permutations(pMods.ToList(), 8, 8);
            int permCount = perms.Count;

            BestModsResult bestResult = null;

            for (int i = 0; i < perms.Count; i++)
            {
                var perm = perms[i];
                perm.Add(pArcanes);
                var elems = ElementCombinations.PossibleCombinations(perm);
                foreach (var elem in elems)
                {
                    var thisResult = MainCalculation(pEidolon, pCase, pSniper, elem, pHealthType, 0, perm.ToArray());
                    if (bestResult == null || thisResult.ToLimbBreak_TimeSeconds < bestResult.DPSResult.ToLimbBreak_TimeSeconds)
                    {
                        bestResult = new BestModsResult(perm, elem, thisResult);
                    }
                }
            }
            bestResult.Mods.Remove(pArcanes);
            return bestResult;
        }
        public static BestModsResult FindBestMods(Eidolon pEidolon, DPSCase pCase, Sniper pSniper, Health pHealthType, Mod pRiven1, Mod pRiven2, Mod pArcanes, bool pAllowHeavyCaliber)
        {
            //List<Mod> allMods = new List<Mod>(MainMods.TestMods);
            List<Mod> allMods = new List<Mod>(MainMods.AllMods(pAllowHeavyCaliber));
            allMods.Remove(allMods.Where((x)=>x.Name == "Riven").Single());

            for(int i=allMods.Count-1;i>=0;i--)
            {
                var mod = allMods[i];
                if(mod.IsAugment)
                {
                    if(!pSniper.AugmentNames.Contains(mod.Name))
                    {
                        allMods.RemoveAt(i);
                    }
                }
            }

            if (pRiven1 == null && pRiven2 == null)
            {
                return FindBestMods(pEidolon, pCase, pSniper, pHealthType, allMods, pArcanes);
            }

            BestModsResult result1 = null;
            BestModsResult result2 = null;
            if (pRiven1 != null)
            {
                allMods.Add(pRiven1);
                result1 = FindBestMods(pEidolon, pCase, pSniper, pHealthType, allMods, pArcanes);
                allMods.Remove(pRiven1);
            }
            if (pRiven2 != null)
            {
                allMods.Add(pRiven2);
                result2 = FindBestMods(pEidolon, pCase, pSniper, pHealthType, allMods, pArcanes);
                allMods.Remove(pRiven2);
            }

            if (result1 == null) { return result2; }
            if (result2 == null) { return result1; }

            return
                result1.DPSResult.ToLimbBreak_TimeSeconds <
                result2.DPSResult.ToLimbBreak_TimeSeconds ?
                result1 : result2;
        }

        private class DPSBreakdown
        {
            public int Magazine;
            public double FireRate;
            public double Reload;
            public Percent AugmentBonusDamage;

            public double TotalMultishot;
            public double TotalCritChance;
            public double TotalCritMultiplier;

            public double TotalDamage_1_WithDamageMods;
            public double TotalDamage_2_WithElementMods;
            public double TotalDamage_3_WithCriticalHits;
            public double TotalDamage_4_WithAugmentBonus;
            public double TotalDamage_5_WithMultishot;
            public double TotalDamage_6_WithHealthTypeFactors;

            public double Impact;
            public double Puncture;
            public double Slash;

            public double Electric;
            public double Heat;
            public double Cold;
            public double Toxin;

            public double Magnetic;
            public double Gas;
            public double Radiation;
            public double Viral;
            public double Corrosive;
            public double Blast;

            public static DPSBreakdown FromPrimitives(Sniper pSniper, DPSCase pCase, Elements[] pElements, Health pHealthType, params Mod[] pMods)
            {
                //First do base damage mods (Serration, Heavy Caliber)
                double baseImpact = pSniper.Impact;
                double basePuncture = pSniper.Puncture;
                double baseSlash = pSniper.Slash;

                double baseElectric = pSniper.Electric;

                double baseCold = pSniper.Cold;
                double baseHeat = pSniper.Heat;
                double baseToxin = pSniper.Toxin;
                double baseMagnetic = pSniper.Magnetic;
                double baseBlast = pSniper.Blast;
                double baseViral = pSniper.Viral;
                double baseRadiation = pSniper.Radiation;
                double baseCorrosive = pSniper.Corrosive;
                double baseGas = pSniper.Gas;



                foreach (var mod in pMods)
                {
                    baseImpact += pSniper.Impact * mod.Damage.AsDecimal0to1;
                    basePuncture += pSniper.Puncture * mod.Damage.AsDecimal0to1;
                    baseSlash += pSniper.Slash * mod.Damage.AsDecimal0to1;

                    baseElectric += pSniper.Electric * mod.Damage.AsDecimal0to1;
                    baseCold += pSniper.Cold * mod.Damage.AsDecimal0to1;
                    baseHeat += pSniper.Heat * mod.Damage.AsDecimal0to1;
                    baseToxin += pSniper.Toxin * mod.Damage.AsDecimal0to1;

                    baseMagnetic += pSniper.Magnetic * mod.Damage.AsDecimal0to1;
                    baseBlast += pSniper.Blast * mod.Damage.AsDecimal0to1;
                    baseViral += pSniper.Viral * mod.Damage.AsDecimal0to1;
                    baseRadiation += pSniper.Radiation * mod.Damage.AsDecimal0to1;
                    baseCorrosive += pSniper.Corrosive * mod.Damage.AsDecimal0to1;
                    baseGas += pSniper.Gas * mod.Damage.AsDecimal0to1;
                }

                DPSBreakdown result = new DPSBreakdown();
                result.Impact = baseImpact;
                result.Puncture = basePuncture;
                result.Slash = baseSlash;

                result.Cold = baseCold;
                result.Electric = baseElectric;
                result.Heat = baseHeat;
                result.Toxin = baseToxin;
                result.Magnetic = baseMagnetic;
                result.Blast = baseBlast;
                result.Viral = baseViral;
                result.Radiation = baseRadiation;
                result.Corrosive = baseCorrosive;
                result.Gas = baseGas;

                result.TotalDamage_1_WithDamageMods = baseImpact
                    + basePuncture
                    + baseSlash
                    + baseElectric
                    + baseCold
                    + baseHeat
                    + baseToxin
                    + baseMagnetic
                    + baseBlast
                    + baseViral
                    + baseRadiation
                    + baseCorrosive
                    + baseGas;


                //Now do mods that can be calculated independent of each other
                double fMagazine = pSniper.Magazine;
                result.FireRate = pSniper.FireRate;
                result.Reload = pSniper.Reload;
                result.TotalMultishot = 0.0;
                result.TotalCritChance = pSniper.CritChance.AsDecimal0to1;
                result.TotalCritMultiplier = pSniper.CritMultiplier;
                result.AugmentBonusDamage = Percent.Zero;

                double reloadModifierDecimal0to1 = 0.0;
                foreach (var mod in pMods)
                {
                    fMagazine += (pSniper.Magazine * mod.MagazineSize.AsDecimal0to1);
                    result.FireRate += pSniper.FireRate * mod.FireRate.AsDecimal0to1;
                    reloadModifierDecimal0to1 += mod.Reload.AsDecimal0to1;
                    result.Reload += pSniper.Reload * mod.Reload.AsDecimal0to1;
                    result.TotalMultishot += mod.Multishot.AsDecimal0to1;
                    result.TotalCritChance += pSniper.CritChance.AsDecimal0to1 * mod.CritChance.AsDecimal0to1;
                    result.TotalCritMultiplier += pSniper.CritMultiplier * mod.CritDamage.AsDecimal0to1;

                    result.Impact += baseImpact * mod.Impact.AsDecimal0to1;
                    result.Puncture += basePuncture * mod.Puncture.AsDecimal0to1;
                    result.Slash += baseSlash * mod.Slash.AsDecimal0to1;

                    result.Heat += result.TotalDamage_1_WithDamageMods * mod.Heat.AsDecimal0to1;
                    result.Cold += result.TotalDamage_1_WithDamageMods * mod.Cold.AsDecimal0to1;
                    result.Toxin += result.TotalDamage_1_WithDamageMods * mod.Toxin.AsDecimal0to1;
                    result.Electric += result.TotalDamage_1_WithDamageMods * mod.Electric.AsDecimal0to1;

                    if (mod.AugmentBonus != null)
                    {
                        double abd = result.AugmentBonusDamage.AsDecimal0to1;
                        result.AugmentBonusDamage = Percent.FromDecimal0to1(abd + mod.AugmentBonus.AsDecimal0to1);
                    }
                }

                result.Magazine = (int)fMagazine;
                result.Reload = pSniper.Reload / (1.0 + reloadModifierDecimal0to1);

                result.TotalDamage_2_WithElementMods = result.Impact;
                result.TotalDamage_2_WithElementMods += result.Puncture;
                result.TotalDamage_2_WithElementMods += result.Slash;
                result.TotalDamage_2_WithElementMods += result.Heat;
                result.TotalDamage_2_WithElementMods += result.Cold;
                result.TotalDamage_2_WithElementMods += result.Toxin;
                result.TotalDamage_2_WithElementMods += result.Electric;

                result.TotalDamage_2_WithElementMods += result.Magnetic;
                result.TotalDamage_2_WithElementMods += result.Blast;
                result.TotalDamage_2_WithElementMods += result.Viral;
                result.TotalDamage_2_WithElementMods += result.Radiation;
                result.TotalDamage_2_WithElementMods += result.Corrosive;
                result.TotalDamage_2_WithElementMods += result.Gas;


                //Now combine multiple elements however the user has them arranged
                MergeElements(pElements.Contains(Elements.Magnetic), baseMagnetic, ref result.Cold, ref result.Electric, out result.Magnetic);
                MergeElements(pElements.Contains(Elements.Gas), baseGas, ref result.Heat, ref result.Toxin, out result.Gas);

                MergeElements(pElements.Contains(Elements.Viral), baseViral, ref result.Cold, ref result.Toxin, out result.Viral);
                MergeElements(pElements.Contains(Elements.Radiation), baseRadiation, ref result.Heat, ref result.Electric, out result.Radiation);

                MergeElements(pElements.Contains(Elements.Corrosive), baseCorrosive, ref result.Electric, ref result.Toxin, out result.Corrosive);
                MergeElements(pElements.Contains(Elements.Blast), baseBlast, ref result.Cold, ref result.Heat, out result.Blast);

                result.TotalCritChance += pSniper.ZoomBonusCC.AsDecimal0to1;

                int CritChanceLevel = 0;
                double CritChanceRemainder = result.TotalCritChance;
                while (CritChanceRemainder > 1.0)
                {
                    CritChanceRemainder -= 1.0;
                    CritChanceLevel++;
                }
                double CritChanceFloor = 1.0 - CritChanceRemainder;
                double CritChanceCeiling = CritChanceRemainder;

                result.TotalCritMultiplier += pSniper.ZoomBonusCD.AsDecimal0to1 * pSniper.CritMultiplier;

                double CritMultiFloor = CritChanceLevel * (result.TotalCritMultiplier - 1) + 1;
                double CritMultiCeiling = (CritChanceLevel + 1) * (result.TotalCritMultiplier - 1) + 1;

                result.TotalDamage_3_WithCriticalHits = 0.0;
                switch (pCase)
                {
                    case DPSCase.Average:
                        result.TotalDamage_3_WithCriticalHits += CritMultiFloor * CritChanceFloor * result.TotalDamage_2_WithElementMods;
                        result.TotalDamage_3_WithCriticalHits += CritMultiCeiling * CritChanceCeiling * result.TotalDamage_2_WithElementMods;
                        break;
                    case DPSCase.Best:
                        result.TotalDamage_3_WithCriticalHits = CritMultiCeiling * result.TotalDamage_2_WithElementMods;
                        break;
                    case DPSCase.Worst:
                        result.TotalDamage_3_WithCriticalHits = CritMultiFloor * result.TotalDamage_2_WithElementMods;
                        break;
                    default: throw new NotImplementedException("Unknown DPSCase: " + pCase.ToString());
                }

                result.TotalDamage_4_WithAugmentBonus = result.TotalDamage_3_WithCriticalHits * (1.0 + result.AugmentBonusDamage.AsDecimal0to1);

                switch (pCase)
                {
                    case DPSCase.Average:
                        result.TotalDamage_5_WithMultishot = result.TotalDamage_4_WithAugmentBonus * (1.0 + result.TotalMultishot);
                        break;
                    case DPSCase.Best:
                        result.TotalDamage_5_WithMultishot = result.TotalDamage_4_WithAugmentBonus * (1.0 + Math.Ceiling(result.TotalMultishot));
                        break;
                    case DPSCase.Worst:
                        result.TotalDamage_5_WithMultishot = result.TotalDamage_4_WithAugmentBonus * (1.0 + Math.Floor(result.TotalMultishot));
                        break;
                    default: throw new NotImplementedException("Unknown DPSCase: " + pCase.ToString());
                }

                result.TotalDamage_6_WithHealthTypeFactors = VsHealthType(result, pHealthType);


                return result;
            }

            private static void MergeElements(bool pDoCombine, double pBaseInnate, ref double pElement1, ref double pElement2, out double pOUT_Combined)
            {
                if (pDoCombine)
                {
                    pOUT_Combined = pBaseInnate + pElement1 + pElement2;
                    pElement1 = pElement2 = 0.0;
                }
                else { pOUT_Combined = pBaseInnate; }
            }

            private static double VsHealthType(DPSBreakdown p, Health pHealthType)
            {
                double TotalVsImpact = p.Impact * (1.0 + pHealthType.VsImpact.AsDecimal0to1);
                double TotalVsPuncture = p.Puncture * (1.0 + pHealthType.VsPuncture.AsDecimal0to1);
                double TotalVsSlash = p.Slash * (1.0 + pHealthType.VsSlash.AsDecimal0to1);

                double TotalVsElectric = p.Electric * (1.0 + pHealthType.VsElectric.AsDecimal0to1);
                double TotalVsCold = p.Cold * (1.0 + pHealthType.VsCold.AsDecimal0to1);
                double TotalVsHeat = p.Heat * (1.0 + pHealthType.VsHeat.AsDecimal0to1);
                double TotalVsToxin = p.Toxin * (1.0 + pHealthType.VsToxin.AsDecimal0to1);


                double TotalVsMag = p.Magnetic * (1.0 + pHealthType.VsMagnetic.AsDecimal0to1);
                double TotalVsGas = p.Gas * (1.0 + pHealthType.VsGas.AsDecimal0to1);
                double TotalVsRad = p.Radiation * (1.0 + pHealthType.VsRadiation.AsDecimal0to1);
                double TotalVsVrl = p.Viral * (1.0 + pHealthType.VsViral.AsDecimal0to1);
                double TotalVsCrsv = p.Corrosive * (1.0 + pHealthType.VsCorrosive.AsDecimal0to1);
                double TotalVsBlst = p.Blast * (1.0 + pHealthType.VsBlast.AsDecimal0to1);

                double TotalDamageVsHealthType =
                    TotalVsImpact +
                    TotalVsPuncture +
                    TotalVsSlash +
                    TotalVsElectric +
                    TotalVsCold +
                    TotalVsHeat +
                    TotalVsToxin +
                    TotalVsVrl +
                    TotalVsRad +
                    TotalVsGas +
                    TotalVsMag +
                    TotalVsCrsv +
                    TotalVsBlst;

                double finalMulti = p.TotalDamage_5_WithMultishot / p.TotalDamage_2_WithElementMods;

                var result = TotalDamageVsHealthType * finalMulti;
                if (result == 0)
                {
                    Debugger.Break();
                }
                return result;
            }
        }



        static class DPSCalcPerformanceCritical
        {
            private static readonly double[] _ComboSteps = { 1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0, 5.0, 5.0, 5.0, 5.0 };
            private static readonly int[] ComboTriggers = new int[_ComboSteps.Length];

            public struct SimulationResult
            {
                public int NumShots;
                public int NumReloads;
                public double TotalDamage;
                public double DamageAfterUserNumberOfShots;
            }

            public struct SimulationInput
            {
                public double LimbHealth;
                public int Magazine;
                public double FireRate;
                public int NumberOfUserShotsToCalculateDamageFor;
                public int PelletsTillx15;
                public DPSCase Case;
                public double TotalMultishot;
                public double TotalDamageWithAllFactorsIncluded;
            }


            public static SimulationResult Simulate(SimulationInput p)
            {
                ComboTriggers[0] = 1;
                ComboTriggers[1] = p.PelletsTillx15;
                for (int iCT = 2; iCT < _ComboSteps.Length; iCT++)
                {
                    ComboTriggers[iCT] = ComboTriggers[iCT - 1] * 3;
                }

                double multiShotPerStep = 0.0;
                switch (p.Case)
                {
                    case DPSCase.Average:
                        multiShotPerStep = p.TotalMultishot;
                        break;
                    case DPSCase.Best:
                        multiShotPerStep = Math.Ceiling(p.TotalMultishot);
                        break;
                    case DPSCase.Worst:
                        multiShotPerStep = Math.Floor(p.TotalMultishot);
                        break;
                    default: throw new NotImplementedException("Unknown DPSCase: " + p.Case.ToString());
                }

                SimulationResult result = new SimulationResult();
                int currentMagazine = p.Magazine;
                double PelletsHitSoFar = 0.0;

                int i = 0;
                while (result.TotalDamage < p.LimbHealth || (i < p.NumberOfUserShotsToCalculateDamageFor))
                {

                    PelletsHitSoFar += (1.0 + multiShotPerStep);
                    int j = 0;
                    while (PelletsHitSoFar > ComboTriggers[j]) { j++; }

                    var ComboMultiplier = _ComboSteps[j == 0 ? 0 : j - 1];
                    double damageThisShot = p.TotalDamageWithAllFactorsIncluded * ComboMultiplier;


                    if (result.TotalDamage < p.LimbHealth)
                    {
                        result.TotalDamage += damageThisShot;
                        result.NumShots++;

                        currentMagazine--;
                        if (currentMagazine == 0)
                        {
                            currentMagazine = p.Magazine;
                            result.NumReloads++;
                        }
                    }

                    if (i < p.NumberOfUserShotsToCalculateDamageFor)
                    {
                        result.DamageAfterUserNumberOfShots += damageThisShot;
                    }

                    i++;
                }

                //Now remove the last reload if a shot isn't actually needed after it
                if (currentMagazine == p.Magazine && result.NumReloads > 0)
                {
                    result.NumReloads--;
                }

                return result;
            }
        }
    }
}