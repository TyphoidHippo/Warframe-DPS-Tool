using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFLib
{
    public enum Elements
    {
        Electric = 1,
        Heat = 2,
        Cold = 4,
        Toxin = 8,
        Gas = 16,
        Magnetic = 32,
        Viral = 64,
        Radiation = 128,
        Corrosive = 256,
        Blast = 512
    }

    public class ElementCombinations
    {
        private static Elements[][] SingleElement(Elements p)
        {
            return new Elements[][] { new Elements[] { p } };
        }
        private static Elements[][] MultiElement(params Elements[] p)
        {
            if(p.Length % 2 != 0) { throw new Exception("Even number required"); }

            List<Elements[]> result = new List<Elements[]>();
            for(int i=0;i<p.Length;i+=2)
            {
                result.Add(new Elements[] { p[i], p[i + 1] });
            }
            return result.ToArray();
        }

        public static Elements[][] PossibleCombinations(Elements[] p)
        {
            if(p.Contains(Elements.Gas) ||
                p.Contains(Elements.Magnetic) ||
                p.Contains(Elements.Viral) ||
                p.Contains(Elements.Radiation) ||
                p.Contains(Elements.Corrosive) ||
                p.Contains(Elements.Blast))
            { throw new Exception("Only primitives should be passed in here."); }

            const int E = (int)Elements.Electric;
            const int H = (int)Elements.Heat;
            const int C = (int)Elements.Cold;
            const int T = (int)Elements.Toxin;

            const int EH = E + H;
            const int EC = E + C;
            const int ET = E + T;
            const int EHC = E + H + C;
            const int EHT = E + H + T;
            const int ECT = E + C + T;
            const int EHCT = E + H + C + T;

            //No Electric
            const int HC = H + C;
            const int HT = H + T;
            const int HCT = H + C + T;

            //No Electric or Heat
            const int CT = C + T;


            int combination = 0;
            foreach (var v in p) { combination += (int)v; }

            switch (combination)
            {
                case 0: return new Elements[][] { new Elements[] { } };
                case E: return SingleElement(Elements.Electric);
                case H: return SingleElement(Elements.Heat);
                case C: return SingleElement(Elements.Cold);
                case T: return SingleElement(Elements.Toxin);

                case EH: { return SingleElement(Elements.Radiation); }
                case EC: { return SingleElement(Elements.Magnetic); }
                case ET: { return SingleElement(Elements.Corrosive); }
                case HC: { return SingleElement(Elements.Blast); }
                case HT: { return SingleElement(Elements.Gas); }
                case CT: { return SingleElement(Elements.Viral); }

                case EHC:
                    {
                        return MultiElement(
                            Elements.Radiation, Elements.Cold,
                            Elements.Magnetic, Elements.Heat,
                            Elements.Blast, Elements.Electric);
                    }
                case EHT:
                    {
                        return MultiElement(
                            Elements.Radiation, Elements.Toxin,
                            Elements.Corrosive, Elements.Heat,
                            Elements.Gas, Elements.Electric);
                    }
                case ECT:
                    {
                        return MultiElement(
                            Elements.Magnetic, Elements.Toxin,
                            Elements.Corrosive, Elements.Cold,
                            Elements.Viral, Elements.Electric);
                    }
                case HCT:
                    {
                        return MultiElement(
                            Elements.Blast, Elements.Toxin,
                            Elements.Gas, Elements.Cold,
                            Elements.Viral, Elements.Heat);
                    }
                case EHCT:
                    {
                        return MultiElement(
                            Elements.Radiation, Elements.Viral,
                            Elements.Magnetic, Elements.Gas,
                            Elements.Corrosive, Elements.Blast);
                    }

                default: throw new NotImplementedException("Unknown Element Combination: " + combination.ToString());
            }
        }
        public static Elements[][] PossibleCombinations(IReadOnlyCollection<Mod> p)
        {
            List<Elements> elems = new List<Elements>();
            foreach(var mod in p)
            {
                if (mod.Electric.HasValue() && !elems.Contains(Elements.Electric)) { elems.Add(Elements.Electric); }
                if (mod.Heat.HasValue()  && !elems.Contains(Elements.Heat)) { elems.Add(Elements.Heat); }
                if (mod.Cold.HasValue()  && !elems.Contains(Elements.Cold)) { elems.Add(Elements.Cold); }
                if (mod.Toxin.HasValue() && !elems.Contains(Elements.Toxin)) { elems.Add(Elements.Toxin); }
            }
            return PossibleCombinations(elems.ToArray());
        }
    }
}
