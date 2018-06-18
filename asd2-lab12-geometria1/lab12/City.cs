using System;
using System.Collections.Generic;
using ASD.Graphs;

namespace asd2
{
    public class City : MarshalByRefObject
    {
        /// <summary>
        /// Sprawdza przecięcie zadanych ulic-odcinków. Zwraca liczbę punktów wspólnych.
        /// </summary>
        /// <returns>0 - odcinki rozłączne, 
        /// 1 - dokładnie jeden punkt wspólny, 
        /// int.MaxValue - odcinki częściowo pokrywają się (więcej niż 1 punkt wspólny)</returns>
        
        // zwraca punkt bardziej na lewo z odcinka (z mniejszym x)
        Point min(Street s)
        {
            Point p1 = s.p1; Point p2 = s.p2;
            if (p1.x > p2.x) return p2;
            else if (p1.x == p2.x)
                if (p1.y < p2.y) return p1;
            return p1;
        }

        // zwraca punkt bardziej na prawo (z wiekszym x)
        Point max(Street s)
        {
            Point p1 = s.p1; Point p2 = s.p2;
            Point c = min(p1, p2);
            if (c == p1)
                return p2;
            else
                return p1;
        }

        // zwraca punkt bardziej na lewo z odcinka (z mniejszym x)
        Point min(Point p1, Point p2)
        {
            if (p1.x > p2.x) return p2;
            else if (p1.x == p2.x)
                if (p1.y < p2.y) return p1;
            return p1;
        }

        // zwraca punkt bardziej na prawo (z wiekszym x)
        Point max(Point p1, Point p2)
        {
            Point c = min(p1, p2);
            if (c == p1)
                return p2;
            else
                return p1;
        }

        // isbetween sprawdza czy dany pkt jest w srodku lub na krawedzi danego odcinka
        bool isbetween(Point p1, Point a, Point b)
        {
            if (p1.x <= Math.Max(a.x, b.x) && p1.x >= Math.Min(a.x, b.x) && p1.y <= Math.Max(a.y, b.y) && p1.y >= Math.Min(a.y, b.y))
                return true;
            return false;
        }
        bool isbetween(Point p1, Street s)
        {
            Point a = s.p1; Point b = s.p2;
            if (p1.x <= Math.Max(a.x, b.x) && p1.x >= Math.Min(a.x, b.x) && p1.y <= Math.Max(a.y, b.y) && p1.y >= Math.Min(a.y, b.y))
                return true;
            return false;
        }

        // inMiddle sprawdza czy dany pkt p1 jest w srodku odcina [a,b] (w srodku, ale nie na krawedzi)
        bool inMiddle(Point p1, Point a, Point b)
        {
            if (p1.x < Math.Max(a.x, b.x) && p1.x > Math.Min(a.x, b.x) && p1.y < Math.Max(a.y, b.y) && p1.y > Math.Min(a.y, b.y))
                return true;
            return false;
        }

        bool inMiddle(Point p1, Street s)
        {
            Point a = s.p1;
            Point b = s.p2;
            if (p1.x <= Math.Max(a.x, b.x) && p1.x >= Math.Min(a.x, b.x) && p1.y < Math.Max(a.y, b.y) && p1.y > Math.Min(a.y, b.y))
                return true;
            if (p1.x < Math.Max(a.x, b.x) && p1.x > Math.Min(a.x, b.x) && p1.y <= Math.Max(a.y, b.y) && p1.y >= Math.Min(a.y, b.y))
                return true;
            return false;
        }

        // sprawdza czy ulice sa takie same
        bool exactStreets(Street s1, Street s2)
        {
            if ((s1.p1 == s2.p1 && s1.p2 == s2.p2) || (s1.p1 == s2.p2 && s1.p2 == s2.p1)) return true;
            return false;
        }

        // etap 1
        public int CheckIntersection(Street s1, Street s2)
        {

            double p1 = Point.CrossProduct(s1.p1 - s2.p1, s2.p2 - s2.p1);
            double p2 = Point.CrossProduct(s1.p2 - s2.p1, s2.p2 - s2.p1);

            double p3 = Point.CrossProduct(s2.p1 - s1.p1, s1.p2 - s1.p1);
            double p4 = Point.CrossProduct(s2.p2 - s1.p1, s1.p2 - s1.p1);
            
            // nie przecinaja sie napewno
            if (p1 * p2 > 0 || p3 * p4 > 0) return 0;
            
            // przecinaja sie odcinki
            if (p1*p2 < 0 || p3*p4 < 0)
                return 1;                      
            
            // takie same ulice
            if (exactStreets(s1, s2)) return int.MaxValue;
            
            // rownolegle ulice i jeden punkt zawiera sie w srodku drugiej ulicy 
            if ((p1 == 0 && p2 == 0 && p3 == 0 && p4 == 0) && (inMiddle(s1.p1, s2) || inMiddle(s1.p2, s2) || inMiddle(s2.p1, s1) || inMiddle(s2.p2, s1))) return int.MaxValue;  
            
            // p1 lezy na odcinku s2
            if (p1 == 0 && isbetween(s1.p1, s2))
                return 1;
            // p2 lezy na odcinku s2
            if (p2 == 0 && isbetween(s1.p2, s2))
                return 1;
            // p3 lezy na odcinku s1
            if (p3 == 0 && isbetween(s2.p1, s1))
                return 1;
            // p4 lezy na odcinku s1
            if (p4 == 0 && isbetween(s2.p2, s1))
                return 1;

            return 0; 
        }


        /// <summary>
        /// Sprawdza czy dla podanych par ulic możliwy jest przejazd między nimi (z użyciem być może innych ulic). 
        /// </summary>
        /// <returns>Lista, w której na i-tym miejscu jest informacja czy przejazd między ulicami w i-tej parze z wejścia jest możliwy</returns>
        // etap 2
        public bool[] CheckStreetsPairs(Street[] streets, int[] streetsToCheck1, int[] streetsToCheck2)
        {
            int n = streetsToCheck1.Length;        
            int S = streets.Length;
            if (S == 0 || n == 0) throw new ArgumentException();
            bool[] ret = new bool[n];
            UnionFind u = new UnionFind(S);
            for (int i = 0; i < S - 1; ++i)
            {
                for (int j = i + 1; j < S; ++j)
                {
                    if (CheckIntersection(streets[i], streets[j]) != 0)
                    {
                        u.Union(i,j);
                    }
                }
            }
            for (int i = 0; i < n; ++i)
            {
                if (u.Find(streetsToCheck1[i]) == u.Find(streetsToCheck2[i]))
                    ret[i] = true;
            }
            return ret;
        }


        /// <summary>
        /// Zwraca punkt przecięcia odcinków s1 i s2.
        /// W przypadku gdy nie ma jednoznacznego takiego punktu rzuć wyjątek ArgumentException
        /// </summary>
        // etap 3
        public Point GetIntersectionPoint(Street s1, Street s2)
        {
            //znajdź współczynniki a i b prostych y=ax+b zawierających odcinki s1 i s2
            //uwaga na proste równoległe do osi y
            //uwaga na odcinki równoległe o wspólnych końcu
            //porównaj równania prostych, aby znaleźć ich punkt wspólny
            if (CheckIntersection(s1, s2) != 1)
                throw new ArgumentException();

            Point p1 = min(s1); // max zwroci zawsze inny punkt niz min
            Point p2 = max(s1);

            Point p3 = min(s2);
            Point p4 = max(s2);

            // odcinki stykaja sie (tez dwa pionowe, dwa poziome, bo wtedy musza sie stykac jesli sie przeciely)
            if (p1 == p3)
                return new Point(p1.x, p1.y);
            if (p2 == p4)
                return new Point(p2.x, p2.y);
            if (p1 == p4)
                return new Point(p1.x, p1.y);
            if (p2 == p3)
                return new Point(p2.x, p2.y);  

            double a1 = (p2.y - p1.y) / (p2.x - p1.x);
            double b1 = p1.y - a1 * p1.x;

            double a2 = (p4.y - p3.y) / (p4.x - p3.x);
            double b2 = p3.y - a2 * p3.x;

            // jeden odcinek pionowy
            if (p1.x == p2.x)
            {
                return (new Point(p1.x, a2 * p1.x + b2));
            }
            if (p3.x == p4.x)
            {
                return (new Point(p3.x, a1 * p3.x + b1));
            }
            // jeden odcinek poziomy
            if (p1.y == p2.y)
            {
                return (new Point((p1.y-b2)/a2, p1.y));
            }
            if (p3.y == p4.y)
            {
                return (new Point((p3.y - b1) / a1, p3.y));
            }

            //double py = (b2 - b1) / (a1 - a2);
            //double px = (py - b2) / a2;

            double px = (b2 - b1) / (a1 - a2);
            double py = a1 * px + b1;
            return new Point(px, py);
        }


        /// <summary>
        /// Sprawdza możliwość przejazdu między dzielnicami-wielokątami district1 i district2,
        /// tzn. istnieją para ulic, pomiędzy którymi jest przejazd 
        /// oraz fragment jednej ulicy należy do obszaru jednej z dzielnic i fragment drugiej należy do obszaru drugiej dzielnicy
        /// </summary>
        /// <returns>Informacja czy istnieje przejazd między dzielnicami</returns>
        // etap 4
        public bool CheckDistricts(Street[] streets, Point[] district1, Point[] district2, out List<int> path, out List<Point> intersections)
        {
            path = new List<int>();
            intersections = new List<Point>();
            int S = streets.Length;
            int n1 = district1.Length;
            int n2 = district2.Length;
            Graph g = new AdjacencyListsGraph<SimpleAdjacencyList>(false, S+2); // 0..S-1 streets   S == district1, S + 1 == district2

            for(int i = 0; i < S; ++i )
            {
                // ulica wchodzi do district1
                for (int k = 0; k < n1; ++k)
                    if (CheckIntersection(streets[i], new Street(district1[k % n1], district1[(k + 1) % n1])) != 0)
                        g.AddEdge(i, S);
                
                // ulica wchodzi do district2
                for (int k = 0; k < n2; ++k)
                    if (CheckIntersection(streets[i], new Street(district2[k % n2], district2[(k + 1) % n2])) != 0)
                        g.AddEdge(i, S+1);

                // ulica krzyzuje sie z kolejna ulica
                if (i != S-1)
                    for(int j = i + 1; j < S; ++j)
                        if (CheckIntersection(streets[i],streets[j]) != 0)
                            g.AddEdge(i, j);
            }
            PathsInfo[] p = new PathsInfo[S+2];
            ShortestPathsGraphExtender.DijkstraShortestPaths(g, S, out p);
            Edge[] e = PathsInfo.ConstructPath(S, S + 1, p);

            // jesli istnieje sciezka od S do S+1
            if (!Double.IsNaN(p[S+1].Dist))
            {
                path = new List<int>();
                // pododawac kolejne pkty (bez S i bez S+1)
                for (int j = 0; j < e.Length - 1; ++j)
                    path.Add(e[j].To);

                intersections = new List<Point>();
                // przeciecia kolejnych ulic
                for(int j = 0; j < path.Count- 1; ++j)
                    intersections.Add(GetIntersectionPoint(streets[path[j]], streets[path[j+1]]));
                return true;
            }
            else
                return false;
        }

    }

    [Serializable]
    public struct Point
    {
        public double x;
        public double y;

        public Point(double px, double py) { x = px; y = py; }

        public static Point operator +(Point p1, Point p2) { return new Point(p1.x + p2.x, p1.y + p2.y); }

        public static Point operator -(Point p1, Point p2) { return new Point(p1.x - p2.x, p1.y - p2.y); }

        public static bool operator ==(Point p1, Point p2) { return p1.x == p2.x && p1.y == p2.y; }

        public static bool operator !=(Point p1, Point p2) { return !(p1 == p2); }

        public override bool Equals(object obj) { return base.Equals(obj); }

        public override int GetHashCode() { return base.GetHashCode(); }

        public static double CrossProduct(Point p1, Point p2) { return p1.x * p2.y - p2.x * p1.y; }

        public override string ToString() { return String.Format("({0},{1})", x, y); }
    }

    [Serializable]
    public struct Street
    {
        public Point p1;
        public Point p2;

        public Street(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
}