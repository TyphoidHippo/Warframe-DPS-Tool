//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using WFLib;

//namespace WarframeSnipers.Tests
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        private static readonly Eidolon Eidolon = Eidolon.Terry;
//        private static readonly DPSCase Case = DPSCase.Average;
//        private static readonly Sniper Sniper = WFLib.Sniper.AllSnipers.Where((x)=>x.Name == "Rubico").First();
//        private static readonly Elements[] Elements = { WFLib.Elements.Radiation, WFLib.Elements.Cold };
//        private static readonly Health Health = Health.Robotic;
//        private static readonly Mod Arcanes = (new Mod[] { WFLib.Arcanes.Momentum, WFLib.Arcanes.Momentum }).Consolidate("Arcanes");
//        private static readonly bool AllowHeavyCaliber = true;

//        private static readonly List<Mod> _Mods = new List<Mod> {
//            MainMods.Serration,
//            MainMods.SplitChamber,
//            MainMods.HeavyCaliber,
//            MainMods.PointStrike,
//            MainMods.VitalSense,
//            MainMods.PrimedCryoRounds,
//            MainMods.VigilanteArmaments
//        };

//        private static void DoCalculation(Mod[] pMods, Elements[] pElements = null, [CallerMemberName] string pCaller = "")
//        {
//            if (pElements == null) { pElements = Elements; }
//            var mods = new List<Mod>(pMods);
//            mods.Add(Arcanes);
//            var dps = DPSCalculation.MainCalculation(Eidolon, Case, Sniper, pElements, Health, 0, mods.ToArray());
//            Console.WriteLine("{0}: {1}", pCaller, dps.ToLimbBreak_TimeSeconds);
//        }

//        [TestMethod]
//        public void VileAcceleration()
//        {
//            _Mods.Add(MainMods.VileAcceleration);
//            DoCalculation(_Mods.ToArray());
//        }

//        [TestMethod]
//        public void PrimedFastHands()
//        {
//            _Mods.Add(MainMods.PrimedFastHands);
//            DoCalculation(_Mods.ToArray());
//        }
//        [TestMethod]
//        public void FindBestMods()
//        {
//            var bestMods = DPSCalculation.FindBestMods(Eidolon, Case, Sniper, Health, null, null, Arcanes, AllowHeavyCaliber);
//            foreach (var mod in bestMods.Mods)
//            {
//                Console.WriteLine(mod.Name);
//            }
//            DoCalculation(bestMods.Mods.ToArray(), bestMods.Elements);
//        }


//        private static void PermutationsLogic(params int[] p)
//        {
//            Console.Write("Input: ");
//            Console.WriteLine(string.Join(", ", p));

//            var perms = DPSCalculation.Permutations(p, 8, 8);

//            Console.WriteLine("\r\n\r\n{0} Permutations found\r\n\r\n", perms.Count);

//            foreach (var perm in perms)
//            {
//                Console.WriteLine(string.Join(", ", perm));
//            }
//        }
//        [TestMethod]
//        public void PermutationsLogic()
//        {
//            //PermutationsLogic(0, 1);
//            //PermutationsLogic(0, 1, 2);
//            //PermutationsLogic(0, 1, 2, 3);
//            //PermutationsLogic(0, 1, 2, 3, 4);
//            //PermutationsLogic(0, 1, 2, 3, 4, 5);

//            List<int> l = new List<int>();
//            for(int i=0;i<19;i++)
//            {
//                l.Add(i);
//            }
//            PermutationsLogic(l.ToArray());
//        }
//    }
//}