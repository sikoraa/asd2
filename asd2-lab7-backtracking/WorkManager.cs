using System;
using System.Linq;

namespace ASD
{
    public class WorkManager : MarshalByRefObject
    {
        /// <summary>
        /// Implementacja wersji 1
        /// W tablicy blocks zapisane s¹ wagi wszystkich bloków do przypisania robotnikom.
        /// Ka¿dy z nich powinien mieæ przypisane bloki sumie wag równej expectedBlockSum.
        /// Metoda zwraca tablicê przypisuj¹c¹ ka¿demu z bloków jedn¹ z wartoœci:
        /// 1 - jeœli blok zosta³ przydzielony 1. robotnikowi
        /// 2 - jeœli blok zosta³ przydzielony 2. robotnikowi
        /// 0 - jeœli blok nie zosta³ przydzielony do ¿adnego robotnika
        /// Jeœli wymaganego podzia³u nie da siê zrealizowaæ metoda zwraca null.
        /// </summary>
        /// // ZMIENNE DO ETAPU 2
        int min = int.MaxValue; // najmniejsza roznica miedzy iloscia blokow robotnikow, czyli najlepsze rozwiazanie
        bool found = false; // znaleziono jakiekolwiek rozwiazanie
        bool finish = false; // znaleziono najlepsze rozwiazanie, roznica miedzy iloscia blokow robotnikow == 0
            
        // tablica blokow, tablica indexow, suma pozostala 1 pracownikowi, suma pozostala 2, aktualny index tablicy
        bool divide(int[] blocks, int[] tmp, int eSum1, int eSum2, int index)
        {
            if (eSum1 < 0 || eSum2 < 0) return false; // zle bloki przypisane
            if (eSum1 == 0 && eSum2 == 0) return true; // obydwoje pracownicy dostali odpowiednie bloki
            if (index == blocks.Length && (eSum1 != 0 || eSum2 != 0)) // przeszlismy wszystkie bloki, ale sie nie da ustawic
                return false;
                      
            tmp[index] = 1; // testujemy czy mozna blok przypisac pierwszemu pracownikowi
            if (divide(blocks, tmp, eSum1 - blocks[index], eSum2, index + 1)) return true;
            tmp[index] = 2; // testujemy czy mozna blok przypisac drugiemu pracownikowi
            if (divide(blocks, tmp, eSum1 , eSum2 - blocks[index], index + 1)) return true;
            // testujemy, czy mozna pominac dany blok
            tmp[index] = 0;
            if (divide(blocks, tmp, eSum1, eSum2, index + 1)) return true;
            return false;
        }

     
        public int[] DivideWorkersWork(int[] blocks, int expectedBlockSum)
        {
            int[] tmp = new int[blocks.Length];
            if (divide(blocks, tmp, expectedBlockSum, expectedBlockSum, 0))
                return tmp;
            return null;
        }



        (int c1, int c2) divide(int[] blocks, int[] tmp, int[] best, int eSum1, int eSum2, int C1, int C2, int index)
        {
            // tmp tablica tymczasowa indexow
            // best tablica najlepszych indexow znalezionych do tej pory
            // C1, C2 ilosc blokow u robotnikow
            // zwracana krotka chyba nie ma za bardzo znaczenia, wynik jest przekazywany przez zmienna FOUND i FINISH
            // FOUND == true - znaleziono jakies rozwiazanie
            // FINISH == true - znaleziono najlepsze rozwiazanie ( ilosc blokow taka sama u obu robotnikow)
            if (eSum1 < 0 || eSum2 < 0) return (0, 0); // zle bloki przypisane
            if (eSum1 == 0 && eSum2 == 0)
            {
                if (Math.Abs(C1 - C2) < min) // znaleziono optymalniejsze rozwiazanie
                {
                    found = true;
                    min = Math.Abs(C1 - C2);
                    if (min == 0)
                        finish = true;
                    for (int i = 0; i < best.Length; ++i)
                        best[i] = tmp[i];
                    return (C1, C2); // obydwoje pracownicy dostali odpowiednie bloki
                }
                return (0, 0); // gorsze rozwiazanie, mozna je pominac
            }
            if (index == blocks.Length && (eSum1 != 0 || eSum2 != 0)) // przeszlismy wszystkie bloki, ale sie nie da ustawic
                return (0, 0);//return false;
            if (Math.Abs(C1 - C2) + index - blocks.Length > min) return (0, 0); // jesli nie ma sensu dalej szukac, bo napewno nie znajdziemy lepszego rozwiazania
            (int a, int b) h;
            tmp[index] = 1; // testujemy czy mozna blok przypisac pierwszemu pracownikowi
            h = (divide(blocks, tmp, best, eSum1 - blocks[index], eSum2, C1 + 1, C2, index + 1));
            if (finish) return (0, 0); // znaleziono 0 roznicowy podzial 

            tmp[index] = 2; // testujemy czy mozna blok przypisac drugiemu pracownikowi
            h = (divide(blocks, tmp, best, eSum1, eSum2 - blocks[index], C1, C2 + 1, index + 1)); //return true;
            if (finish) return (0, 0); // znaleziono 0 roznicowy podzial 

            tmp[index] = 0;
            h = (divide(blocks, tmp, best, eSum1, eSum2, C1, C2, index + 1)); //return true;
            return (0,0);
        }
        /// <summary>
        /// Implementacja wersji 2
        /// Parametry i wynik s¹ analogiczne do wersji 1.
        /// </summary>
        public int[] DivideWorkWithClosestBlocksCount(int[] blocks, int expectedBlockSum)
        {
            int[] tmp = new int[blocks.Length];
            int possible = 0;
            // jak suma blokow np 10, a robotnik chce wziac > 5 no to napewno sie nie da, sprawdzam to
            foreach (int t in blocks) // sumuje wartosci blokow, zeby sprawdzic czy w ogole da sie robotnikom je przydzielic
                possible += t;
            if (possible < 2 * expectedBlockSum) return null; // nie ma po co sprawdzac bo i tak nie znajdzie wyniku
            int[] best = new int[blocks.Length];
            min = int.MaxValue;
            found = false; // znaleziono JAKIEKOLWIEK rozwiazanie 
            finish = false; // znaleziono rozwiazanie takie, ze roznica blokow == 0 czyli dalej nie ma co szukac
            divide(blocks, tmp, best, expectedBlockSum, expectedBlockSum, 0, 0, 0);
            if (found)
                return best;
            return null;
        }

// Mo¿na dopisywaæ pola i metody pomocnicze

    }
}

