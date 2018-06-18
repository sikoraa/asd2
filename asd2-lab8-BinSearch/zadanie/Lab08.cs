using System;
using System.Collections.Generic;

namespace Lab08
{

    public class Lab08 : MarshalByRefObject
    {

        /// <summary>
        /// funkcja do sprawdzania czy da się ustawić k elementów w odległości co najmniej dist od siebie
        /// </summary>
        /// <param name="a">posortowana tablica elementów</param>
        /// <param name="dist">zadany dystans</param>
        /// <param name="k">liczba elementów do wybrania</param>
        /// <param name="exampleSolution">Wybrane elementy</param>
        /// <returns>true - jeśli zadanie da się zrealizować</returns>
        /// 
        
        public bool CanPlaceElementsInDistance(int[] a, int dist, int k, out List<int> exampleSolution)
        {
            if (a == null) throw new ArgumentException();
            int N = a.Length;
            if (N < 2 || N > 100000) throw new ArgumentException();
            if (k < 2 || k > N) throw new ArgumentException();
            exampleSolution = null;
            if (a[N - 1] - a[0] < dist) return false;

            List<int> tmp = new List<int>();
            tmp.Add(a[0]);

            int i = k - 1; // ilosc elementow, ktore dodamy (a[0] dodamy zawsze, dlatego -1)
            int j = 1; // iterator po elementach, od 1 do N-1
            int C = 0; // ilosc dodanych elementow na drodze (a[0], ... ,                 a[N-1])
            while(j < N && C < i ) // jesli dobry indeks tablicy i ciagle brakuje nam elementow
            {
                if (a[j] - tmp[C] >= dist )
                {
                    tmp.Add(a[j]);
                    ++C;
                }
                ++j;
            }
            if (C == i) // jesli ilosc znalezionych elementow jest rowna oczekiwanej, to zwracamy wynik
            {
                exampleSolution = tmp;
                return true;
            }
            return false;
            
        }

        /// <summary>
        /// Funkcja wybiera k elementów tablicy a, tak aby minimalny dystans pomiędzy dowolnymi dwiema liczbami (spośród k) był maksymalny
        /// </summary>
        /// <param name="a">posortowana tablica elementów</param>
        /// <param name="k">liczba elementów do wybrania</param>
        /// <param name="exampleSolution">Wybrane elementy</param>
        /// <returns>Maksymalny możliwy dystans między wybranymi elementami</returns>
        public int LargestMinDistance(int[] a, int k, out List<int> exampleSolution)
        {
            exampleSolution = null;
            if (a == null) throw new ArgumentException();
            long N = a.Length;
            if (N < 2 || N > 100000) throw new ArgumentException();
            if (k < 2 || k > N) throw new ArgumentException();
            exampleSolution = null;           

            bool found = false;
            long start = a[0]; 
            long end = a[N - 1];
            long maxDist = (end - start) / (k-1) + 1;
            long bestDist = 0;

            List<int> tmpList = new List<int>();
            List<int> bestList = new List<int>();
            

            long l = 0;
            long p = maxDist;
            long m = (l + p) / 2;

            while (l < p)
            {
                m = (l + p) / 2;
                if (CanPlaceElementsInDistance(a, (int)m, k, out tmpList)) // sprawdzamy prawy podprzedzial
                {
                    found = true; // znalezlismy "jakies" rozwiazanie
                    long tmp2 = l; // do sprawdzenia, czy l sie zmieni, warunek wyjscia z petli
                    l = m;
                    bestList.Clear(); // znalezlismy lepsze rozwiazanie, trzeba je zapisac
                    foreach (int b in tmpList)
                        bestList.Add(b);
                    bestDist = m; // zapisanie lepszego dystansu
                    if (tmp2 == l) break; // bez tego program by sie zapetlal
                }
                else
                {
                    p = m; // dalej sprawdzamy lewy podprzedzial
                }
            }
            if (found) // jesli znalezlismy jakies rozwiazanie to je zwracamy
            {
                exampleSolution = bestList;
                return (int)bestDist;
            }
            exampleSolution = new List<int>(); // wpp zwracamy pusta liste i 0
            return 0;
        }
    }
}
