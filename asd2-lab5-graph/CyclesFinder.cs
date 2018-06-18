using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class CyclesFinder : MarshalByRefObject
    {
        /// <summary>
        /// Sprawdza czy graf jest drzewem
        /// </summary>
        /// <param name="g">Graf</param>
        /// <returns>true jeśli graf jest drzewem</returns>

        //void writeG(Graph g)
        //{
        //    Console.WriteLine("--------------START ------------");

        //    for (int v = 0; v < g.VerticesCount; ++v)
        //    {
        //        foreach(Edge e in g.OutEdges(v))
        //        {
        //            if (e.From < e.To)
        //                Console.Write(" ({0},{1}) ", e.From, e.To);
        //        }
        //        Console.WriteLine();
        //    }
        //    Console.WriteLine("--------------END ------------");
        //}

        //public void pp(List<int> t)
        //{
        //    Console.WriteLine("CYKL ------------------------");
        //    foreach (int i in t)
        //    {
        //        Console.Write("{0} -> ", i);
        //    }
        //    Console.WriteLine("\n--------------------");
        //}

        //public void ppp(List<Edge> t)
        //{
        //    Console.WriteLine("CYKL ------------------------");
        //    foreach (Edge e in t)
        //    {
        //        Console.Write("({0},{1}) -> ", e.From, e.To);
        //    }
        //    Console.WriteLine("\n--------------------");
        //}

        public bool IsTree(Graph g)
        {
            if (g.Directed == true) throw new ArgumentException();
            
            int n = g.VerticesCount;  
            int cc = 0;
            GeneralSearchGraphExtender.GeneralSearchAll<EdgesStack>(g, null, null, null, out cc, null);
            if (cc == 1 && g.EdgesCount == n - 1) return true; // drzewo musi miec V-1 krawedzi i miec 1 spojna skladowa
            else return false; 
            
        }




        /// <summary>
        /// Wyznacza cykle fundamentalne grafu g względem drzewa t.
        /// Każdy cykl fundamentalny zawiera dokadnie jedną krawędź spoza t.
        /// </summary>
        /// <param name="g">Graf</param>
        /// <param name="t">Drzewo rozpinające grafu g</param>
        /// <returns>Tablica cykli fundamentalnych</returns>
        /// <remarks>W przypadku braku cykli zwracać pustą (0-elementową) tablicę, a nie null</remarks>
        public Edge[][] FindFundamentalCycles(Graph g, Graph t)
        {
            if (g == null || t == null) throw new ArgumentException();
            if (IsTree(t) == false) throw new ArgumentException();
            if (g.Directed == true) throw new ArgumentException();
            if (true == IsTree(g)) { return new Edge[0][]; }
            int n = g.VerticesCount;
            bool[] visitedV = new bool[n];
            for (int i = 0; i < n; ++i) // do sprawdzenia czy drzewo t rozpina g
                visitedV[i] = false; 

            List<int> p = new List<int>();
            Edge[][] arr;// = new Edge[n][];
            int[] V = new int[t.VerticesCount] ;
            List<Edge> le = new List<Edge>();
            List<List<Edge>> list = new List<List<Edge>>();
            for (int i = 0; i < t.VerticesCount; ++i)
                V[i] = i;
            if (false == GeneralSearchGraphExtender.GeneralSearchFrom<EdgesStack>(t, 0,
                            delegate (int u)
                            {
                                visitedV[u] = true;
                                return true;
                            }, null
                            , 
                            delegate (Edge e)
                            {
                                if (double.IsNaN(g.GetEdgeWeight(e.From, e.To))) return false;
                                else return true;
                            }, null

                            )) throw new ArgumentException();
            for (int i = 0; i < n; ++i)
            {
                if (visitedV[i] == false) throw new ArgumentException(); // t nie rozpina g
            }

            bool[] nr = new bool[n];
            for (int i = 0; i < n; ++i) nr[i] = false;
            Graph gg = g.Clone();
           

            for (int v = 0; v < n; ++v)
            {
                foreach(Edge e in gg.OutEdges(v))
                {
                    if (e.From < e.To && double.IsNaN(t.GetEdgeWeight(e.From,e.To)))
                    {
                        bool[] nrr = new bool[n];
                        bool[] visited = new bool[n];
                        List<Edge> tmp = new List<Edge>();
                        for (int i = 0; i < n; ++i)
                        {
                            nrr[i] = false;
                            visited[i] = false;
                        }
                        bool search = true;

                        GeneralSearchGraphExtender.GeneralSearchFrom<EdgesStack>(t, e.To,
                            delegate (int u)
                            {
                                if (!search) return true;//true;
                                visited[u] = true;
                                if (u == v)
                                {
                                    nrr[u] = true;

                                    search = false;
                                    return true;
                                }
                                else return true;
                            }, delegate (int w)
                            {

                                if (visited[w] == true)
                                {
                                    foreach (Edge e1 in t.OutEdges(w))
                                    {

                                        if (nrr[e1.To] == true)
                                        {
                                            tmp.Add(new Edge(e1.To, w ));
                                            nrr[w] = true;

                                            break;
                                        }
                                    }
                                }
                                return true;
                            }, null, null

                            );
                        tmp.Add(new Edge(e.To, e.From));
                        list.Add(tmp);
                   }
                }
                
            }
            arr = list.Select(a => a.ToArray()).ToArray();
           
            return arr;
        }

        /// <summary>
        /// Dodaje 2 cykle fundamentalne
        /// </summary>
        /// <param name="c1">Pierwszy cykl</param>
        /// <param name="c2">Drugi cykl</param>
        /// <returns>null, jeśli wynikiem nie jest cykl i suma cykli, jeśli wynik jest cyklem</returns>
        public Edge[] AddFundamentalCycles(Edge[] c1, Edge[] c2)
        {

            int max = -1;
            List<int> vList = new List<int>();
            foreach (Edge e1 in c1)
            {
                if (e1.From > max) max = e1.From;
                if (e1.To > max) max = e1.To;
            }

            foreach (Edge e1 in c2)
            {
                if (e1.From > max) max = e1.From;
                if (e1.To > max) max = e1.To;
            }
            bool[] b1 = new bool[max + 1];
            bool[] b2 = new bool[max + 1];

            foreach (Edge e1 in c1)
            {
                
                if (e1.From > max) max = e1.From;
                if (e1.To > max) max = e1.To;
            }

            var G = new AdjacencyListsGraph<HashTableAdjacencyList>(false, max+1);
            int n = G.VerticesCount;
            bool[] edgeVisited = new bool[G.EdgesCount];
            bool[] vVisited = new bool[n];
            for (int j = 0; j < edgeVisited.Length; ++j)
                edgeVisited[j] = false;
            List<Edge> newCycle = new List<Edge>();
            int vtmp = -1;
            foreach (Edge e1 in c1)
            {
                if (!c2.Contains(e1) && !c2.Contains(new Edge(e1.To, e1.From)))
                {
                    G.AddEdge(e1);
                    vVisited[e1.From] = true;
                    vVisited[e1.To] = true;
                    vtmp = e1.From;
                } 
            }
            foreach (Edge e1 in c2)
            {
                if (!c1.Contains(e1) && !c1.Contains(new Edge(e1.To, e1.From)))
                {
                    G.AddEdge(e1);
                    vVisited[e1.From] = true;
                    vVisited[e1.To] = true;
                    vtmp = e1.From;
                }
            }

            bool[] vChecked = new bool[n];
            
            for (int j = 0; j < n; ++j) vChecked[j] = false;
            if (G.EdgesCount < 3) return null;
            List<Edge> edgeList = new List<Edge>();
            bool repeatedVertice = GeneralSearchGraphExtender.GeneralSearchFrom<EdgesStack>(G, vtmp,
                            delegate (int u)
                            {
                               if (G.OutEdges(u).Count() != 2) return false; 
                               vChecked[u] = true;       
                               return true;
                            }, delegate (int w)
                            {
                                return true;
                            }, 
                            delegate (Edge e)
                            {

                                if (!edgeList.Contains(new Edge(e.To,e.From)))//(e.From < e.To)
                                {
                                    edgeList.Add(e);
                                }
                                return true;
                            }
                            ,null);

            if (repeatedVertice == false) return null;

            for(int j = 0; j < n; ++j)
            {
                if (vChecked[j] != vVisited[j]) return null;
            }          
            return edgeList.ToArray();
        }

    }

}
