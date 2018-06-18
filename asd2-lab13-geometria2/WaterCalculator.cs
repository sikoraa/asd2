using System;
using System.Collections.Generic;

namespace ASD
{
    public class WaterCalculator : MarshalByRefObject
    {

        /*
         * Metoda sprawdza, czy przechodząc p1->p2->p3 skręcamy w lewo 
         * (jeżeli idziemy prosto, zwracany jest fałsz).
         */
        private bool leftTurn(Point p1, Point p2, Point p3)
        {
            Point w1 = new Point(p2.x - p1.x, p2.y - p1.y);
            Point w2 = new Point(p3.x - p2.x, p3.y - p2.y);
            double vectProduct = w1.x * w2.y - w2.x * w1.y;
            return vectProduct > 0;
        }


        /*
         * Metoda wyznacza punkt na odcinku p1-p2 o zadanej współrzędnej y.
         * Jeżeli taki punkt nie istnieje (bo cały odcinek jest wyżej lub niżej), zgłaszany jest wyjątek ArgumentException.
         */
        private Point getPointAtY(Point p1, Point p2, double y)
        {
            if (p1.y != p2.y)
            {
                double newX = p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y);
                if ((newX - p1.x) * (newX - p2.x) > 0)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point(p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y), y);
            }
            else
            {
                if (p1.y != y)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point((p1.x + p2.x) / 2, y);
            }
        }


        /// <summary>
        /// Funkcja zwraca tablice t taką, że t[i] jest głębokością, na jakiej znajduje się punkt points[i].
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// 
        /// 
        /// </summary>
        /// 
        



        public double[] PointDepths(Point[] points)
        {
            int n = points.Length;
            double[] ret = new double[n]; // wynik
            // tablica left tak naprawde tylko potrzebna
            bool[] left = new bool[n]; // zatopione pkty
            bool[] forwards = new bool[n]; // zatopione pkty
            bool[] backwards = new bool[n]; // zatopione pkty
            bool[] underwater = new bool[n]; // zatopione pkty 
           
            int lefti = 0;
            int righti = -1;
            for(int i = 1; i < n; ++i)
            {
                if (points[i].x < points[i - 1].x)
                {
                    lefti = -1;
                    continue;
                }
                if (lefti == -1)
                    lefti = i - 1;
                // jesli jestesmy o 1 pkt w prawo od lewej granicy i idziemy do gory, tzn. ze trzeba przesunac lewa granice
                if (lefti == i - 1 && points[i].y >= points[lefti].y)
                {
                    lefti++;
                }
                else if (points[i].y >= points[lefti].y)
                {
                    for (int j = lefti + 1; j < i; ++j) // oznaczamy punkty jako zatopione pomiedzy lefti, i
                        forwards[j] = true;
                    lefti = i;
                }                
            }
            righti = n - 1;
            for (int i = n-2; i >= 0; --i)
            {
                if (points[i].x > points[i + 1].x)
                {
                    righti = -1;
                    continue;
                }
                if (righti == -1)
                    righti = i + 1;
                // jesli jestesmy o 1 pkt w lewo od prawej granicy i idziemy do gory, tzn. ze trzeba przesunac prawa granice
                if (forwards[i]) { underwater[i] = true; continue; }
                if (righti == i + 1 && points[righti].y <= points[i].y)
                {
                    righti--;
                    //under = false;
                }
                else if (points[i].y >= points[righti].y)
                {
                    for (int j = righti - 1; j > i; --j) // oznaczamy punkty jako zatopione pomiedzy i, righti
                    {
                        backwards[j] = true;
                        underwater[j] = true;
                    }
                    righti = i;
                }
            }
            left = underwater;
            int ll = 0;
            int rr = -1;
            bool between = false;
            double[] waterH = new double[n]; // 
            for(int i = 1; i < n; ++i) // szukamy dwoch sasiednich niezatopionych pktow
            {
                if (left[i]) // jesli pkt zatopiony to ustawiamy flage between na true
                {
                    between = true;
                    continue;
                }
                if (rr == -1) // jesli niezatopiony pkt to albo jest on lewa granica albo prawa
                {
                    if (!between) // przesuwamy lewa granice, jesli nie wykrylismy zatopionych pktow
                    {
                        ll = i;
                        continue;
                    }
                    // jesli wykrylismy zatopione pkty, tzn. ze jest to prawa granica
                    rr = i;
                    double minH = Math.Min(points[ll].y, points[rr].y);
                    // dla kazdego pktu pomiedzy lewa a prawa granica ustawiamy wysokosc nizszej granicy
                    for (int j = ll; j <= rr; ++j)
                        waterH[j] = minH;
                    ll = i;
                    rr = -1;
                    between = false;
                }              
            }
            // odejmujemy od wysokosci nizszej granicy wysokosc danego pktu, zeby obliczyc ile jest zatopiony
            for(int i = 1; i < n; ++i)
            {
                if (left[i])
                {
                    ret[i] = waterH[i] - points[i].y;
                }
            }
            return ret;
        }

        /// <summary>
        /// Funkcja zwraca objętość wody, jaka zatrzyma się w górach.
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double WaterVolume(Point[] points)
        {
            double v = 0;
            int n = points.Length;
            double[] h = PointDepths(points);
            int ll = -1; // jesli ll == -1 tzn. ze nie jest znaleziona lewa granica dolka zalanego woda
            int rr = -1; // prawa granica dolka
            double hmax = double.MaxValue; // wysokosc trapezu, najwyzszy z zatopionych punktow, reszte objetosci to trojkaty
            for (int i = 1; i < n; ++i)
            {
                if (h[i] > 0 && ll == -1) // dodanie pierwszego trojkata, wykrycie zatopionego terenu
                {
                    Point p = getPointAtY(points[i], points[i - 1], points[i].y + h[i]);
                    v += (points[i].x - p.x) * h[i] / 2;
                    ll = i;
                    hmax = points[i].y;
                    continue;
                }
                else if(h[i] > 0)
                {
                    v+= (h[i] + h[i-1])/2 * (points[i].x - points[i-1].x); // dodawanie trapezow
                    rr = i;
                    if (points[i].y > hmax)
                        hmax = points[i].y;
                    continue;
                }
                else if (h[i] == 0 && ll != 0) // znaleziony koniec zatopionego terenu, dodanie drugiego trojkata
                {
                    Point p = getPointAtY(points[i - 1], points[i], points[i-1].y + h[i-1]);
                    v += (-points[i-1].x + p.x) * h[i-1] / 2;
                    ll = -1; rr = -1;
                }
            }
            return v;
        }
    }

    [Serializable]
    public struct Point
    {
        public double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
