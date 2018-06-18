using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ARKADIUSZ SIKORA
namespace ASD
{
    /// <summary>
    /// struktura przechowująca punkt
    /// </summary>
    [Serializable]
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point (int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    public class AdventurePlanner: MarshalByRefObject
    {
        /// <summary>
        /// największy rozmiar tablicy, którą wyświetlamy
        /// ustaw na 0, żeby nic nie wyświetlać
        /// </summary>
        public int MaxToShow = 0;

        public void deepCopy(List<Point> from, List<Point> to) // bledna nazwa, bardziej kopiuje wszystkie elementy z listy from na koniec ISTNIEJACEJ listy to
        {
            foreach(Point p in from)
            {
                to.Add(new Point(p.X, p.Y));
            }
        }

        void p(int[,] A)
        {
            int x = A.GetLength(0);
            int y = A.GetLength(1);
            Console.WriteLine();
            for (int i = 0; i < y; ++i)
            {
                Console.Write("|  ");
                for (int j = 0; j < x; ++j)
                {
                    Console.Write("{0}  ", A[j, i]);
                }
                Console.WriteLine("|");
            }


        }
        void pp(List<Point> pL)
        {
            Console.WriteLine();
            Console.WriteLine("-------------------");

            Console.WriteLine("LISTA PKTOW:");
            foreach (Point p in pL)
            {
                Console.WriteLine("({0}, {1})", p.X, p.Y);
            }
            Console.WriteLine("-------------------");
        }
        /// <summary>
        /// Znajduje optymalną pod względem liczby znalezionych skarbów ścieżkę,
        /// zaczynającą się w lewym górnym rogu mapy (0,0), a kończącą się w prawym
        /// dolnym rogu (X-Y-1).
        /// Za każdym razem możemy wykonać albo krok w prawo albo krok w dół.
        /// Pierwszym polem ścieżki powinno być (0,0), a ostatnim polem (X-1,Y-1).        
        /// </summary>
        /// <param name="treasure">liczba znalezionych skarbów</param>
        /// <param name="path">znaleziona ścieżka</param>
        /// <remarks>
        /// Złożoność rozwiązania to O(X * Y).
        /// </remarks>
        /// <returns></returns>
        public int FindPathThere(int[,] treasure, out List<Point> path)
        {
            path = new List<Point>();
            
            int x = treasure.GetLength(0);
            int y = treasure.GetLength(1);
            if (x <= 0 || y <= 0) { path.Add(new Point(0, 0)); return 0; }
            
            int[,] v = new int[x, y];
            for (int i = 0; i < x; ++i)
                for (int j = 0; j < y; ++j)
                    v[i, j] = 0;
            v[0, 0] = treasure[0, 0];
            
            if (x == 1 && y == 1) { path.Add(new Point(0, 0)); return v[0, 0]; }// jeden punkt
            if (x == 1) // podlozny przypadek
            {
                path.Add(new Point(0, 0));
                for (int i = 1; i < y; ++i)
                {
                    v[0, i] = v[0, i - 1] + treasure[0, i];
                    path.Add(new Point(0, i));
                }
                return v[0, y - 1];
            }
            if (y == 1) // podlozny przypadek
            {
                path.Add(new Point(0, 0));
                for (int i = 1; i < x; ++i)
                {
                    v[i, 0] = v[i - 1, 0] + treasure[i, 0];
                    path.Add(new Point(i, 0));
                }
                return v[x - 1, 0];
            }
            for (int i = 1; i < y; ++i) // wypelnianie 1 kolumny tablicy 
            {
                v[0, i] = v[0, i - 1] + treasure[0, i];
            }
            for (int i = 1; i < x; ++i) // wypelnianie 1 wiersza tablicy
            {
                v[i, 0] = v[i - 1, 0] + treasure[i, 0];
            }
            // mamy pewnosc, ze 1 wiersz i 1 kolumna obliczona, wiec dla kazdego pozostalego punktu mozemy patrzec na lewo i do gory bez obawy o indeksy tablicy
            for (int yy = 1; yy < y; ++yy)
            {
                for (int xx = 1; xx < x; ++xx)
                {
                    if (v[xx - 1, yy] > v[xx, yy - 1]) // lepiej isc od lewej
                    {
                        v[xx, yy] = v[xx - 1, yy] + treasure[xx, yy];
                    }
                    else // lepiej isc od gory
                    {
                        v[xx, yy] = v[xx, yy - 1] + treasure[xx, yy];
                    }
               }
            }
            int x_ = x - 1;
            int y_ = y - 1;
            while (x_ > 0 && y_ > 0)
            {
                path.Add(new Point(x_, y_));
                if (v[x_ - 1, y_] > v[x_, y_ - 1])
                {
                    --x_;
                }
                else
                    --y_;
            }
            if (x_ == 0)
            {
                while (y_ >= 0)
                {
                    path.Add(new Point(0, y_--));
                }
            }
            else
            {
                while (x_ >= 0)
                {
                    path.Add(new Point(x_--,0));
                }
            }
            path.Reverse();
            return v[x - 1, y - 1];
        }

      
        /// <summary>
        /// Znajduje optymalną pod względem liczby znalezionych skarbów ścieżkę,
        /// zaczynającą się w lewym górnym rogu mapy (0,0), dochodzącą do prawego dolnego rogu (X-1,Y-1), a 
        /// następnie wracającą do lewego górnego rogu (0,0).
        /// W pierwszy etapie możemy wykonać albo krok w prawo albo krok w dół. Po osiągnięciu pola (x-1,Y-1)
        /// zacynamy wracać - teraz możemy wykonywać algo krok w prawo albo krok w górę.
        /// Pierwszym i ostatnim polem ścieżki powinno być (0,0).
        /// Możemy założyć, że X,Y >= 2.
        /// </summary>
        /// <param name="treasure">liczba znalezionych skarbów</param>
        /// <param name="path">znaleziona ścieżka</param>
        /// <remarks>
        /// Złożoność rozwiązania to O(X^2 * Y) lub O(X * Y^2).
        /// </remarks>
        /// <returns></returns>
        public int FindPathThereAndBack(int[,] treasure, out List<Point> path)
        {
            path = new List<Point>();
            int x = treasure.GetLength(0);
            int y = treasure.GetLength(1);
            List<Point> pThere = new List<Point>();
            List<Point> pBack = new List<Point>();
            if (x == 1 && y == 1) // przypadek z 1 skarbem
            {
                path.Add(new Point(0, 0));
                return treasure[0, 0];
            }
            if ( x == 1) // przypadek z wektorem skarbow
            {
                int tmp = 0;
                for(int i = 0; i < y; ++i)
                {
                    path.Add(new Point(0, i));
                    tmp += treasure[0, i];
                }
                for (int i = y - 2; i >= 0; --i)
                    path.Add(new Point(0, i));
                return tmp;
            }
            if ( y == 1) // przypadek z wektorem skarbow
            {
                int tmp = 0;
                for (int i = 0; i < x; ++i)
                {
                    path.Add(new Point(i, 0));
                    tmp += treasure[i, 0];
                }
                for (int i = x - 2; i >= 0; --i)
                    path.Add(new Point(i, 0));
                return tmp;
            }
            
            if (x == 2) // przypadek zdegenerowany krotki prostokat
            {
                int tmp = 0;
                for (int i = 0; i < y; ++i)
                {
                    tmp += treasure[0, i];
                    path.Add(new Point(0, i));
                    
                }
                for (int i = y - 1; i >= 0; --i)
                {
                    tmp += treasure[1, i];
                    path.Add(new Point(1, i)); 
                }
                path.Add(new Point(0, 0));
                return tmp;
            }
            else if(y == 2) // przypadek zdegenerowany niski prostokat
            {
                int tmp = 0;
                for (int i = 0; i < x; ++i)
                {
                    tmp += treasure[i, 0];
                    path.Add(new Point(i, 0));
                }
                for (int i = x - 1; i >= 0; --i)
                {
                    tmp += treasure[i, 1];
                    path.Add(new Point(i, 1));
                }
                path.Add(new Point(0, 0));
                return tmp;
            }
            // PRZYPADKI OGOLNE
            int[,,] v = new int[x, y,y]; 
            for (int i = 0; i < x; ++i)
                for(int j = 0; j < y; ++j)
                    for(int k = 0; k < y; ++k)
                    {
                        v[i, j, k] = 0;
                    }

            v[0, 0, 0] = treasure[0, 0];
            

            int zeroTreasure = treasure.Cast<int>().Max(); 
            if (zeroTreasure == 0) // jak na mapie nie ma skarbow to oddzielny przypadek, bo tamto cos nie dzialalo
            {
                for (int i = 0; i < y; ++i)
                    path.Add(new Point(0, i));
                for (int i = 1; i < x; ++i)
                    path.Add(new Point(i, y-1));
                for (int i = y - 2; i >= 0; --i)
                    path.Add(new Point(x - 1, i));
                for (int i = x - 2; i >= 0; --i)
                    path.Add(new Point(i, 0));
                return 0;
            }
            int moves = x - 1 + y - 1; // ilosc "przekatnych" w prostokacie nie liczac punktu (0,0)
            // rozbite na dwa zbiory przekatnych, jedne (PIERWSZY for) zaczynajace w x==0
            for (int k = 1; k < y; ++k) // iterujemy po przekatnych, poniewaz dla kazdego punktu na przekatnej
                                            // mamy pewnosc, ze punkty nam potrzebne leza na wczesniejszej przekatnej
                                            // (czyli sa obliczone). Przekatne biore takie '/' i zaczynam w lewym dolnym rogu przekatnej
            {
                int LL = -1;
                int LG = -1;
                int GL = -1;
                int GG = -1;
                bool odGory, odLewej, odGory2; // sprawdzenie czy do punktu mozna dojsc odgory(x1,y1), odlewej(x1,y1), odgory2(x2,y2)
                                               // od lewej dla (x2,y2) zawsze mozna dojsc, poniewaz zakladamy, ze punkt ten lezy na tej 
                                               // samej przekatnej co (x1,y1) i jest od niego wyzej na prawo (czyli x2 > x1, y2 < y1)
                int x1;
                int y1;
                           
                for (x1 = 0; x1 < k; ++x1) // dla danego (kazdego) punktu na przekatnej
                {
                    y1 = k - x1; // zaczynamy isc od lewego dolnego rogu przekatnej, x1 == 0, y1 == k
                    if (y1 < 0 || y1 > y-1) { continue; } // to zeby poza tablice nie wychodzic
                    if (x1 > x - 1 - 1) break;            // to zeby poza tablice nie wychodzic
                    odGory = odLewej = odGory2 = true;
                    LL = LG = GL = GG = -1;               // LG = od lewej (x1,y1), od gory (x2,y2)
                    if (x1 == 0) odLewej = false;         // skraj tablicy, nie mozna od lewej do (x1,y1)
                    if (y1 == 0) odGory = false;          // skraj tablicy, nie mozna od gory do (x1,y1)
                    for (int y2 = y1 - 1; y2 >= 0; --y2) // dla kazdego innego punktu na przekatnej (czyli para punktow)
                                                         // rozwazamy tylko punkty na prawo wyzej od punktu (x1,y1), zeby nie powtarzac par
                                                         // czyli x2 > x1,   y2 < y1
                    {
                        LL = LG = GL = GG = -1;

                        if (y2 < 0 || y2 > y - 1) { continue; }
                        if (k-y2 > x - 1) break;

                        LL = LG = GL = GG = -1;
                        if (y2 == 0)
                            odGory2 = false;
                        if(odLewej)
                        {
                            if (odGory2)
                                LG = v[x1 - 1, y1, y2 - 1];
                            LL = v[x1 - 1, y1, y2]; // od lewej zawsze mozna dla punktu (x2,y2) poniewaz jest on na prawo do gory od punktu (x1,y1)
                        }
                        if (odGory)
                        {
                            if (odGory2)
                                GG = v[x1, y1 - 1, y2 - 1];
                            GL = v[x1, y1 - 1, y2];
                        }
                        int max = LL;
                        if (LG > LL) max = LG;
                        if (GL > max) max = GL;
                        if (GG > max) max = GG;

                        v[x1, y1, y2] = max + treasure[x1,y1] + treasure[k-y2,y2]; // k to ilosc krokow zeby dojsc do pktu, dlatego x + y == k
                        int maxX1 = 0, maxY1 = 0, maxY2 = 0; // zapisanie wspolrzednych punktow o najwiekszej wartosci
                        if (max == GL && max == LG && max == LL && max == GG)
                        {
                            maxX1 = x1; maxY1 = y1 - 1; maxY2 = y2;

                        }
                        if (max == GL)
                        {

                            maxX1 = x1; maxY1 = y1-1; maxY2 = y2;
                        }                     
                        else if(max == LG)
                        {
                            maxX1 = x1 - 1; maxY1 = y1; maxY2 = y2-1;
                        }
                        else if (max == LL)
                        {
                            maxX1 = x1-1; maxY1 = y1; maxY2 = y2;
                        }
                        else if (max == GG)
                        {
                            maxX1 = x1 ; maxY1 = y1-1; maxY2 = y2-1;
                        }
                    }
                }
            }
            // drugi for po przekatnych, tutaj x nie zaczyna sie od 0 (z kazda iteracja zwiekszany o 1)
            for(int z = 1; z < x; ++z)
            //for (int k = 1; k < y; ++k) // iterujemy po przekatnych, poniewaz dla kazdego punktu na przekatnej
                                        // mamy pewnosc, ze punkty nam potrzebne leza na wczesniejszej przekatnej
                                        // (czyli sa obliczone). Przekatne biore takie '/' i zaczynam w lewym dolnym rogu przekatnej
            {
                int k = z + y - 1;
                int LL = -1;
                int LG = -1;
                int GL = -1;
                int GG = -1;
                bool odGory, odLewej, odGory2; // sprawdzenie czy do punktu mozna dojsc odgory(x1,y1), odlewej(x1,y1), odgory2(x2,y2)
                                               // od lewej dla (x2,y2) zawsze mozna dojsc, poniewaz zakladamy, ze punkt ten lezy na tej 
                                               // samej przekatnej co (x1,y1) i jest od niego wyzej na prawo (czyli x2 > x1, y2 < y1)
                int x1;
                int y1;

                for (x1 = z; x1 < x - 1; ++x1) // dla danego (kazdego) punktu na przekatnej
                {
                    y1 = k - x1; // zaczynamy isc od lewego dolnego rogu przekatnej, x1 == 0, y1 == k
                    if (y1 < 0 || y1 > y - 1) { continue; } // to zeby poza tablice nie wychodzic
                    if (x1 > x - 1 - 1) break;            // to zeby poza tablice nie wychodzic
                    odGory = odLewej = odGory2 = true;
                    LL = LG = GL = GG = -1;               // LG = od lewej (x1,y1), od gory (x2,y2)
                    if (x1 == 0) odLewej = false;         // skraj tablicy, nie mozna od lewej do (x1,y1)
                    if (y1 == 0) odGory = false;          // skraj tablicy, nie mozna od gory do (x1,y1)
                    for (int y2 = y1 - 1; y2 >= 0; --y2) // dla kazdego innego punktu na przekatnej (czyli para punktow)
                                                         // rozwazamy tylko punkty na prawo wyzej od punktu (x1,y1), zeby nie powtarzac par
                                                         // czyli x2 > x1,   y2 < y1
                    {
                        LL = LG = GL = GG = -1;

                        if (y2 < 0 || y2 > y - 1) { continue; }
                        if (k - y2 > x - 1) break;

                        LL = LG = GL = GG = -1;
                        if (y2 == 0)
                            odGory2 = false;
                        if (odLewej)
                        {
                            if (odGory2)
                                LG = v[x1 - 1, y1, y2 - 1];
                            LL = v[x1 - 1, y1, y2]; // od lewej zawsze mozna dla punktu (x2,y2) poniewaz jest on na prawo do gory od punktu (x1,y1)
                        }
                        if (odGory)
                        {
                            if (odGory2)
                                GG = v[x1, y1 - 1, y2 - 1];
                            GL = v[x1, y1 - 1, y2];
                        }
                        int max = LL;
                        if (LG > LL) max = LG;
                        if (GL > max) max = GL;
                        if (GG > max) max = GG;
                        v[x1, y1, y2] = max + treasure[x1, y1] + treasure[k - y2, y2]; // k to ilosc krokow zeby dojsc do pktu, dlatego x + y == k
                        int maxX1 = 0, maxY1 = 0, maxY2 = 0; // zapisanie wspolrzednych punktow o najwiekszej wartosci
                        if (max == GL && max == LG && max == LL && max == GG)
                        {
                            maxX1 = x1; maxY1 = y1 - 1; maxY2 = y2;
                        }
                        if (max == GL)
                        {
                            maxX1 = x1; maxY1 = y1 - 1; maxY2 = y2;
                        }
                        else if (max == LG)
                        {
                            maxX1 = x1 - 1; maxY1 = y1; maxY2 = y2 - 1;
                        }
                        else if (max == LL)
                        {
                            maxX1 = x1 - 1; maxY1 = y1; maxY2 = y2;
                        }
                        else if (max == GG)
                        {
                            maxX1 = x1; maxY1 = y1 - 1; maxY2 = y2 - 1;
                        }

                    }
                }
            }
            bool p1L, p1G, p2L, p2G; // czy z P1 mozna isc w lewo, do gory, czy z P2 mozna isc w lewo, do gory
            p1L = p1G = p2L = p2G = true;
            int x1_ = x - 2; int y1_ = y - 1; int y2_ = y - 2; 
            while (x1_ >= 0 && y1_ >= 0 && y2_ >= 0 && x1_+y1_-y2_ >= 0)
            {
                pThere.Add(new Point(x1_, y1_));
                pBack.Add(new Point(x1_ + y1_ - y2_, y2_));
                if (x1_ == 0) p1L = false;
                if (y1_ == 0) p1G = false;
                if (x1_+y1_-y2_ == 0) p2L = false;
                if (y2_ == 0) p2G = false;
                int ll, lg, gl, gg;
                ll = lg = gl = gg = -1;
                int maxp = 0;
                if (p1L)
                {
                    if (p2L)
                    {
                        ll = v[x1_ - 1, y1_, y2_];
                    }
                    if ( p2G)
                    {
                        lg = v[x1_ - 1, y1_, y2_-1];
                    }
                }
                if (p1G)
                {
                    if (p2L)
                    {
                        gl = v[x1_, y1_ - 1, y2_];
                    }
                    if (p2G)
                    {
                        gg = v[x1_, y1_ - 1, y2_ - 1];
                    }
                }
                if (lg > ll) // gl > ll
                {
                    maxp = lg;
                    
                }
                else
                {
                    maxp = ll ;

                }
                if (gl > maxp)
                { maxp = gl; }
                if (gg > maxp)
                {
                    maxp = gg;
                }
                if (maxp == lg) { --x1_; --y2_; }
                else if (maxp == gl) { --y1_; }
                else if (maxp == ll) { --x1_; }
                else if (maxp == gg) { --y1_; --y2_; }
            }
            v[x-1, y-1, y-1] = v[x - 2, y-1, y - 2] + treasure[x-1,y-1];
            pThere.Reverse();
            path.AddRange(pThere);
            path.Add(new Point(x - 1, y - 1));
            path.AddRange(pBack);
            return v[x-1,y-1,y-1]; 
        }
    }
}
