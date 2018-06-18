using ASD.Graphs;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ASD
{
    public class Maze : MarshalByRefObject
    {     
        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 

        // sprawdza, czy dany wierzcholek to X(false) czy O,S,E (true)
        bool correctVertix(char[,] m, int y, int x)
        {
            if (m[y, x] == 'X') return false;
            return ((m[y, x]) == 'O' || m[y, x] == 'E' || m[y, x] == 'S');
        }

        // zamienia wspolrzedne na numer wierzcholka grafu
        int nr(int x, int y, int max)
        {
            return y*max + x;
        }
        

        void createGraph(char[,] m, Graph g, out int start, out int end) // etap 1, nie dodaje krawedzi dla X
        {
            int Y = m.GetLength(0);
            int X = m.GetLength(1);
            start = end = 0;

            for (int j = 0; j < Y; ++j) // [j,i] j - y,   i - x
            {
                for (int i = 0; i < X; ++i)
                {
                    if (!(correctVertix(m, j, i))) continue;
                    if (m[j, i] == 'S')
                        start = nr(i, j, X);
                    if (m[j, i] == 'E')
                        end = nr(i, j, X);
                    if (i > 0)
                    {
                        if (correctVertix(m, j, i - 1))
                            g.AddEdge(new Edge(nr(i, j, X), nr(i - 1, j, X)));
                    }
                    if (i < X - 1)
                    {
                        if (correctVertix(m, j, i + 1))
                            g.AddEdge(new Edge(nr(i, j, X), nr(i + 1, j, X)));
                    }
                    if (j > 0)
                    {
                        if (correctVertix(m, j - 1, i))
                            g.AddEdge(new Edge(nr(i, j, X), nr(i, j - 1, X)));
                    }
                    if (j < Y - 1)
                    {
                        if (correctVertix(m, j + 1, i))
                            g.AddEdge(new Edge(nr(i, j, X), nr(i, j + 1, X)));
                    }
                }
            }
        } 

        void createGraph(char[,] m, Graph g, int t, out int start, out int end) // etap 2, krawedzie wchodzace do X z waga t
        {
            int Y = m.GetLength(0);
            int X = m.GetLength(1);
            start = end = 0;

            for (int j = 0; j < Y; ++j) // [j,i] j - y,   i - x
            {
                for (int i = 0; i < X; ++i)
                {
                    if (m[j,i] == 'S')
                        start = nr(i, j, X);
                    if (m[j, i] == 'E')
                        end = nr(i, j, X);
                    if (i > 0)
                    {
                        if (correctVertix(m, j, i - 1))
                            g.AddEdge(new Edge(nr(i, j, X), nr(i - 1, j, X)));
                        else
                            g.AddEdge(new Edge(nr(i, j, X), nr(i - 1, j, X), t));
                    }
                    if (i < X - 1)
                    {
                        if (correctVertix(m, j, i + 1))
                            g.AddEdge(new Edge(nr(i, j, X), nr(i + 1, j, X)));
                        else
                            g.AddEdge(new Edge(nr(i, j, X), nr(i + 1, j, X), t));
                    }
                    if (j > 0)
                    {
                        if (correctVertix(m, j - 1, i))
                            g.AddEdge(new Edge(nr(i, j, X), nr(i, j - 1, X)));
                        else
                            g.AddEdge(new Edge(nr(i, j, X), nr(i, j - 1, X), t));
                    }
                    if (j < Y - 1)
                    {
                        if (correctVertix(m, j + 1, i))
                            g.AddEdge(new Edge(nr(i, j, X), nr(i, j + 1, X)));
                        else
                            g.AddEdge(new Edge(nr(i, j, X), nr(i, j + 1, X), t));
                    }
                }
            }
        }

        // etap 3, warstwy wygladaja jak graf z etapu 1 (bez krawedzi do X), a krawedzie do X to sa polaczenia miedzy warstwami i oraz (i+1)
        void createGraph(char[,] m, Graph g, int t, out int start, out int end, int k, int n)
        {
            int Y = m.GetLength(0);
            int X = m.GetLength(1);
            start = end = 0;

            for (int j = 0; j < Y; ++j)
            {
                for (int i = 0; i < X; ++i)
                {
                    if (m[j, i] == 'S')
                        start = nr(i, j, X);
                    if (m[j, i] == 'E')
                        end = nr(i, j, X);
                    if (i > 0) // mozna dodac krawedz po lewej
                    {
                        if (correctVertix(m, j, i - 1))
                            for (int z = 0; z <= k; ++z)
                                g.AddEdge(new Edge(nr(i,j,X)+z*n, nr(i - 1, j, X)+z*n)); // tworzenie warstw grafu
                        else
                            for (int z = 1; z <= k; ++z)
                                g.AddEdge(new Edge(nr(i, j, X)+(z-1)*n, nr(i - 1, j, X)+z*n, t)); // polaczenia miedzy warstwami
                    }
                    if (i < X - 1) // krawedz po prawej
                    {
                        if (correctVertix(m, j, i + 1))
                            for (int z = 0; z <= k; ++z)
                                g.AddEdge(new Edge(nr(i, j, X) + z * n, nr(i + 1, j, X) + z * n)); // tworzenie warstw grafu
                        else
                            for (int z = 1; z <= k; ++z)
                                g.AddEdge(new Edge(nr(i, j, X)+(z - 1) * n, nr(i + 1, j, X) + z* n, t)); // polaczenia miedzy warstwami
                    }
                    if (j > 0) // krawedz do gory
                    {
                        if (correctVertix(m, j - 1, i))
                            for (int z = 0; z <= k; ++z)
                                g.AddEdge(new Edge(nr(i, j, X)+z*n, nr(i, j - 1, X)+z*n)); // tworzenie warstw grafu
                        else
                            for (int z = 1; z <= k; ++z)
                                g.AddEdge(new Edge(nr(i, j, X) + (z - 1) * n, nr(i, j - 1, X) + z * n, t)); // polaczenia miedzy warstwami
                    }
                    if (j < Y - 1) // krawedz do dolu
                    {
                        if (correctVertix(m, j + 1, i))
                            for (int z = 0; z <= k; ++z)
                                g.AddEdge(new Edge(nr(i, j, X)+z*n, nr(i, j + 1, X)+z*n)); // tworzenie warstw grafu
                        else
                            for (int z = 1; z <= k; ++z)
                                g.AddEdge(new Edge(nr(i, j, X) + (z - 1) * n, nr(i, j + 1, X) + z * n, t)); // polaczenia miedzy warstwami
                    }
                }
            }
        }
        string constructString(Edge[] l)
        {
            string s = "";
            if (l.Length == 0) return s;

            foreach (Edge e in l)
            {
                if (e.From == e.To - 1)
                    s += "E";
                else if (e.From == e.To + 1)
                    s += "W";
                else if (e.From < e.To)
                    s += "S";
                else
                    s += "N";
            }
            return s;
        }

        string constructString1xN(Edge[] l) // gdzie kolejne wierzcholki sa pionowo obok siebie, a nie poziomo
        {
            string s = "";
            if (l.Length == 0) return s;

            foreach (Edge e in l)
            {
                if (e.From < e.To)
                    s += "S";
                else
                    s += "N";
            }
            return s;
        }

        string constructStringE34(Edge[] l, int rowElem, int n)
        {
            string s = "";
            foreach(Edge e in l)
            {
                if (e.From == e.To - 1 || e.From == e.To - 1 - n)
                    s += "E";
                else if (e.From == e.To + 1 || e.From == e.To + 1 - n)
                    s += "W";
                else if (e.From == e.To - rowElem || e.From == e.To - rowElem - n)
                    s += "S";
                else if (e.From == e.To + rowElem || e.From == e.To + rowElem - n)
                    s += "N";
                //else if
            }
            return s;
        }

        string constructStringE341xN(Edge[] l, int rowElem, int n) // etap 3, 4 gdzie kolejne wierzcholki sa pionowo obok siebie
        {
            string s = "";
            foreach (Edge e in l)
            {
                if (e.From == e.To - 1 || e.From == e.To - 1 - n)
                    s += "S";
                else if (e.From == e.To + 1 || e.From == e.To + 1 - n)
                    s += "N";
            }
            return s;
        }

        // ETAP 1 i 2
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            path = "";
            int X = maze.GetLength(1);
            int Y = maze.GetLength(0);
            
            int start = -1;
            int end = -1;
            AdjacencyListsGraph<AVLAdjacencyList> g = new AdjacencyListsGraph<AVLAdjacencyList>(true, X * Y);
            PathsInfo[] p = null;
            
            if (t == 0)
                createGraph(maze, g, out start, out end);
            else
                createGraph(maze, g, t, out start, out end);
            ShortestPathsGraphExtender.DijkstraShortestPaths(g, start, out p);
            int dist = (int)p[end].Dist;
            if (!Double.IsNaN(p[end].Dist))
            {
                if (X > 1)
                    path = constructString(PathsInfo.ConstructPath(start, end, p));
                else
                    path = constructString1xN(PathsInfo.ConstructPath(start, end, p));
                return dist;
            }
            else
                return -1;
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        /// // pomysl: k+1 takich samych grafow(warstw) bez krawedzi X , a krawedzie X to takie "mosty" miedzy warstwami
        // ETAP 3 i 4
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            path = "";

            int X = maze.GetLength(1);
            int Y = maze.GetLength(0);
            int n = X * Y; // liczba punktow w tablicy, dla ulatwienia w szukaniu numeru wierzcholka w innej warstwie grafu
            int start = -1;
            int end = -1;
            AdjacencyListsGraph<AVLAdjacencyList> g = new AdjacencyListsGraph<AVLAdjacencyList>(true, (k+1)*n);
            PathsInfo[] p = null;
            createGraph(maze, g, t, out start, out end, k, n);
            ShortestPathsGraphExtender.DijkstraShortestPaths(g, start, out p);
            int[] dist = new int[k + 1]; // tablica najlepszych drog dla uzycia dynamitow od 0 do k
            for (int i = 0; i <= k; ++i)
                dist[i] = -1;
            int min = int.MaxValue;
            int index = 0; // ilosc dynamitow zuzytych dla najlepszej drogi
            for(int i = 0; i <= k; ++i) // sprawdzenie dla kazdej ilosci uzytego dynamitu najlepszej drogi
            {
                int tmp = (int)p[end + i * n].Dist;
                if (!Double.IsNaN(p[end+i*n].Dist)) // jesli nie NaN to porownac z wartoscia najmniejsza
                {
                    dist[i] = tmp;
                    if (tmp < min) // jesli nowa wartosc lepsza to zapisac jako min
                    {
                        min = tmp;
                        index = i;
                    }
                }
            }
            if (min == int.MaxValue)
                return -1; // jesli nie znaleziono sciezki to -1
            else
            {
                if (X > 1)
                    path = constructStringE34(PathsInfo.ConstructPath(start, end + index * n, p), X, n); 
                else // jesli kolejne wierzcholki grafu nie sa na tablicy po prawo/lewo, tylko gora/dol (bo wymiar 1xN czy Nx1 nie wiem)
                    path = constructStringE341xN(PathsInfo.ConstructPath(start, end + index * n, p), X, n); 
                return min;
            }        
        }
    }
}