
using System;
using System.Collections;
using System.Collections.Generic;

namespace ASD
{
   
    public class CarpentersBench : System.MarshalByRefObject
    {

        /// <summary>
        /// Wyznaczanie optymalnego sposobu pocięcia płyty
        /// </summary>
        /// <param name="sheet">Rozmiary płyty</param>
        /// <param name="elements">Tablica zawierająca informacje o wymiarach i wartości przydatnych elementów</param>
        /// <param name="cuts">Opis cięć prowadzących do uzyskania optymalnego rozwiązania</param>
        /// <returns>Maksymalna sumaryczna wartość wszystkich uzyskanych w wyniku cięcia elementów</returns>

        public int Cut(int length, int width, int[,] elements, out Cut cuts)
        {
            (int length, int width, int price)[] _elements = new(int length, int width, int price)[elements.GetLength(0)];
            for (int i = 0; i < _elements.Length; ++i)
            {
                _elements[i].length = elements[i, 0];
                _elements[i].width = elements[i, 1];
                _elements[i].price = elements[i, 2];
            }
            return Cut((length, width), _elements, out cuts);
        }

        public int Cut((int length, int width) sheet, (int length, int width, int price)[] elements, out Cut cuts)
        {
            int[,] value = new int[sheet.length + 1, sheet.width + 1];
            
            Cut[,] cutz = new Cut[sheet.length + 1, sheet.width + 1];
            for (int i = 0; i < sheet.length + 1; ++i)
                for (int j = 0; j < sheet.width + 1; ++j)
                {
                    value[i, j] = 0;
                    cutz[i, j] = new Cut(i, j, 0, false, 0, null, null);
                }
            int n = elements.Length;
            foreach((int length, int width, int price) e in elements) // zlozonosc O(E + ... )   jednokrotne przejscie elementow
            {
                if (e.length <= sheet.length && e.width <= sheet.width)
                {
                    if (e.price > value[e.length, e.width])
                    {
                        value[e.length, e.width] = e.price;
                        cutz[e.length, e.width] = new Cut(e.length, e.width, e.price, false, 0, null, null);
                    }
                }
            }
            int tmp = 0;
            int W = sheet.width;
            int L = sheet.length;
            for (int i = 1; i <= L; ++i) // Y // zlozonosc O(E + L*...) for po calym wymiarze L
                for (int j = 1; j <= W; ++j) // X // zlozonosc O(E + L*W*...) for po calym wymiarze W
                {
                    for (int y = 1; y <= i/2; ++y) // zmieniamy po Y // O(E + L*W*(L/2+ ...)
                    {
                        tmp = value[y, j] + value[i - y, j]; // ciecie poziome
                        if (tmp > value[i,j])
                        {
                            value[i, j] = tmp;
                            cutz[i, j] = new Cut(i, j, value[i, j], false, i - y, cutz[i - y, j], cutz[y, j]);
                        }
                    }
                    for (int x = 1; x <= j/2; ++x) // zmieniamy po X // zlozonosc O(E + L*W*(L/2 + W/2))
                    {
                        tmp = value[i, x] + value[i, j - x]; // ciecie pionowe
                        if (tmp > value[i,j])
                        {
                            value[i, j] = tmp;
                            cutz[i, j] = new Cut(i, j, value[i, j], true, x, cutz[i, x], cutz[i, j - x]);
                        }
                    }
                    tmp = 0;
                }// ostatecznie O(E + L*W*(L+W)) ale czemu rekursja nie zadzialala to ja nie wiem
            cuts = cutz[sheet.length, sheet.width];
            return value[sheet.length, sheet.width];   
        }

    }

}
