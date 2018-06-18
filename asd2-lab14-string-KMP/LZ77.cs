using System;
using System.Collections.Generic;
using System.Text;

namespace ASD
{
    public class LZ77 : MarshalByRefObject
    {
        /// <summary>
        /// Odkodowywanie napisu zakodowanego algorytmem LZ77. Dane kodowanie jest poprawne (nie trzeba tego sprawdzać).
        /// </summary>
        /// 


        public string Decode(List<EncodingTriple> encoding)
        {
            string s = "";
            
            if (encoding.Count == 0) return s;
            int n = encoding.Count;
            int len = 0;
            foreach (var e in encoding)
                len += e.c + 1;
            StringBuilder sb = new StringBuilder(len);
            for(int i = 0; i < n; ++i)
            {
                if (sb.Length == 0)
                {
                    sb.Append(encoding[i].s);
                    //s += encoding[i].s;
                }
                else
                {
                    int tmp = sb.Length;
                    int t = tmp - 1 - encoding[i].p;
                    int p = t;
                    for (int k = 0; k < encoding[i].c; ++k)
                    {
                        if (t == -1) p = t = 0;
                        if (p == tmp) p = t;
                        sb.Append(sb[p]);
                        //s += s[p];
                        ++p;
                    }
                    sb.Append(encoding[i].s);
                    //s += encoding[i].s;
                }
            }
            s = sb.ToString();
            return s;
        }

        /// <summary>
        /// Kodowanie napisu s algorytmem LZ77
        /// </summary>
        /// <returns></returns>


        int lastIndex;
        private static IEnumerable<int> getLPS(int start, int length, string s)
        {
            int[] Lsp = new int[length + 1];
            int t;
            Lsp[0] = Lsp[1] = t = 0;
            int j = 0;
            foreach(var p in Lsp)
            {
                if (j == 0 || j == 1)
                {
                    ++j;
                    yield return p;
                }
                else
                {
                    while (t > 0 && s[start + t] != s[start + j - 1])
                        t = Lsp[t];
                    if (s[start + t] == s[start + j - 1]) ++t; 
                    Lsp[j] = t;
                    // 
                    yield return Lsp[j];
                }
            }
            
        }


        public static (int c, int j) KMP(string s, int wStart, int rStart, int rEnd, int maxP)
        {
            int S = s.Length;
            
            int t = 0;
            int lastComputedLps = 1;
            int di; // przyrost, obliczany na koncu zewnetrznego fora, bo nie wiadomo jaka ilosc znakow przejdziemy

            int c, j, WR, W, R, i, q;

            c = -1; // wyniki, najdluzsze porownanie
            j = -1; // index na ktorym sie zaczyna najwieksze porownanie

            WR = S - wStart - 1; // ilosc znakow w WR, czyli |s| - poczatek w
            R = S - rStart - 1;  // ilosc znakow w R, czyli |s| - poczatek r
            W = rStart - wStart; // ilosc znakow w W
            W = Math.Min(maxP, W); // ilosc znakow do sprawdzenia, albo W albo maxP jesli W jest wieksze

            int[] Lps = new int[R + 1];



            for (q = i = 0; (i <= W) && (i < rStart); i += di) // q - liczba pasujacych symboli
            {
                for (q = Lps[q]; (q < R) && (s[wStart + i + q] == s[rStart + q]); ++q) // doliczanie Lps
                {
                    if ((q + 1 > lastComputedLps) && (q + 1 < R) && (s[wStart + i + q + 1] == s[rStart + q + 1])) 
                    {
                        t = Lps[q];
                        while ((t > 0) && (s[rStart + t] != s[rStart + q])) // symbol nie pasuje
                            t = Lps[t];
                        if (s[rStart + t] == s[rStart + q]) // symbol pasuje
                            ++t;  
                        Lps[q + 1] = t;
                        lastComputedLps++;
                    }
                }

                if (q > c) // znalezlismy dluzsze wystapienie, trzeba je zapisac
                {
                    c = q;
                    j = i;
                }
                 
                if (q > lastComputedLps) // doliczenie Lps
                {
                    t = Lps[q - 1];
                    while (t > 0 && s[rStart + t] != s[rStart + q - 1]) // symbol nie pasuje
                        t = Lps[t];
                    if (s[rStart + t] == s[rStart + q - 1]) ++t;  // symbol pasuje
                    Lps[q] = t;
                    lastComputedLps++;
                }
                if (q > 0)
                    di = q - Lps[q];
                else
                    di = 1;
            }
            return (c, j + 1); // bo j indeksem liczonym od 1
        }



       

        public List<EncodingTriple> Encode(string s, int maxP)
        {
            List<EncodingTriple> l = new List<EncodingTriple>();
            int n = s.Length;

            l.Add(new EncodingTriple(0, 0, s[0]));
            //Console.WriteLine("Added: (" + 0 + ", " + 0 + ", " + s[0] + ")");

            int wStart = 0;
            int wEnd = 0;
            int w;

            int rStart = 1;
            int rEnd;
            int r;

            int i = 1;

            while (i < n)
            {
                if (i == n - 1)
                {
                    l.Add(new EncodingTriple(0, 0, s[s.Length - 1]));
                    return l;
                }

                // 1
                if (i - maxP - 1 > 0)
                    wStart = i - maxP - 1;
                else
                    wStart = 0;
                wEnd = i - 1;
                w = i - wStart;
                
                // 2
                rStart = i;
                //rEnd = Math.Min(maxP, )
                if (rStart + maxP - 1 < n)
                    rEnd = rStart + maxP - 1;
                else
                    rEnd = n - 1;

                // 3
                (int c, int j) ret = KMP(s, wStart, rStart, rEnd, maxP); 
                
                // 4
                l.Add(new EncodingTriple(w -ret.j, ret.c, s[i + ret.c]));
                //Console.WriteLine("Added: (" + w-ret.j-1 + ", " + ret.c + ", " + s[i+ret.c] + ")");

                // 5
                i += ret.c + 1;
            }
            return l;
        }
    }

    [Serializable]
    public struct EncodingTriple
    {
        public int p, c;
        public char s;

        public EncodingTriple(int p, int c, char s)
        {
            this.p = p;
            this.c = c;
            this.s = s;
        }
    }
}
