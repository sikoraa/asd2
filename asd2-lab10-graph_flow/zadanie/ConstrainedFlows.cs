using ASD.Graphs;
using System;
using System.Collections.Generic;
using static ASD.Graphs.MaxFlowGraphExtender;
namespace ASD
{
    public class ConstrainedFlows : System.MarshalByRefObject
    {
        // testy, dla których ma być generowany obrazek
        // graf w ostatnim teście ma bardzo dużo wierzchołków, więc lepiej go nie wyświetlać
        public static int[] circulationToDisplay = {  };
        public static int[] constrainedFlowToDisplay = { 4 };

        /// <summary>
        /// Metoda znajdująca cyrkulację w grafie, z określonymi żądaniami wierzchołków.
        /// Żądania opisane są w tablicy demands. Szukamy funkcji, która dla każdego wierzchołka będzie spełniała warunek:
        /// suma wartości na krawędziach wchodzących - suma wartości na krawędziach wychodzących = demands[v]
        /// </summary>
        /// <param name="G">Graf wejściowy, wagi krawędzi oznaczają przepustowości</param>
        /// <param name="demands">Żądania wierzchołków</param>
        /// <returns>Graf reprezentujący wynikową cyrkulację.
        /// Reprezentacja cyrkulacji jest analogiczna, jak reprezentacja przepływu w innych funkcjach w bibliotece.
        /// Należy zwrócić kopię grafu G, gdzie wagi krawędzi odpowiadają przepływom na tych krawędziach.
        /// Zwróć uwagę na rozróżnienie sytuacji, kiedy mamy zerowy przeływ na krawędzi (czyli istnieje
        /// krawędź z wagą 0) od sytuacji braku krawędzi.
        /// Jeśli żądana cyrkulacja nie istnieje, zwróć null.
        /// </returns>
        /// <remarks>
        /// Nie można modyfikować danych wejściowych!
        /// Złożoność metody powinna być asymptotycznie równa złożoności metody znajdującej największy przeływ (z biblioteki).
        /// </remarks>
        public Graph FindCirculation(Graph G, double[] demands)
        {
            int n = G.VerticesCount;
            double p = 0;
            foreach (double d in demands) p += d;
            if (p != 0) return null;
            Graph g = G.IsolatedVerticesGraph(G.Directed, n + 2); // graf ze zrodlem i ujsciem
            Graph gg = G.IsolatedVerticesGraph(); // graf na wynik
            // n - zrodlo, n+1 ujscie
            double[] tab = new double[n];

            for (int i = 0; i < G.VerticesCount; ++i)
            {
                if (demands[i] < 0)
                    g.AddEdge(n, i, -demands[i]);
                else
                    g.AddEdge(i, n + 1, demands[i]);//if (G.OutDegree(i) == 0) g.AddEdge(i, n + 1, demands[i]); // krawedz do zrodla, jaka wartosc? moze -

                foreach (Edge e in G.OutEdges(i))
                {
                    g.AddEdge(e);
                }
            }
            (double value, Graph tmp) = MaxFlowGraphExtender.PushRelabelMaxFlow(g, n, n+1, null , false);
            double[] tmpVal = new double[G.VerticesCount];
            for (int i = 0; i < n; ++i)
            {
                foreach (Edge e in tmp.OutEdges(i))
                {
                    if (e.From < n && e.To < n)
                    {
                        tmpVal[e.To] += e.Weight;
                        tmpVal[e.From] -= e.Weight;
                        gg.Add(e);
                    }
                }
            }
            for (int i = 0; i < n; ++i)
                if (demands[i] != tmpVal[i]) return null;
            return gg;
        }

        /// <summary>
        /// Funkcja zwracająca przepływ z ograniczeniami, czyli przepływ, który dla każdej z krawędzi
        /// ma wartość pomiędzy dolnym ograniczeniem a górnym ograniczeniem.
        /// Zwróć uwagę, że interesuje nas *jakikolwiek* przepływ spełniający te ograniczenia.
        /// </summary>
        /// <param name="source">źródło</param>
        /// <param name="sink">ujście</param>
        /// <param name="G">graf wejściowy, wagi krawędzi oznaczają przepustowości (górne ograniczenia)</param>
        /// <param name="lowerBounds">kopia grafu G, wagi krawędzi oznaczają dolne ograniczenia przepływu</param>
        /// <returns>Graf reprezentujący wynikowy przepływ (analogicznie do poprzedniej funkcji i do reprezentacji
        /// przepływu w funkcjach z biblioteki.
        /// Jeśli żądany przepływ nie istnieje, zwróć null.
        /// </returns>
        /// <remarks>
        /// Nie można modyfikować danych wejściowych!
        /// Złożoność metody powinna być asymptotycznie równa złożoności metody znajdującej największy przeływ (z biblioteki).
        /// </remarks>
        /// <hint>Wykorzystaj poprzednią część zadania.
        /// </hint>
        /// 

            public void d(double[] demands, int n)
        {
            Console.WriteLine();
            for (int i = 0; i < n; ++i)
            {
                Console.WriteLine("i: {0}    d: {1}", i, demands[i]);

            }
            Console.WriteLine("---------");
        }


        public bool ok(int s, int t, Graph g)
        {
            if (g == null) return false;
            double[] tmp = new double[g.VerticesCount];
            for (int i = 0; i < g.VerticesCount; ++i)
                foreach(Edge e in g.OutEdges(i))
                {
                    
                    tmp[e.To] += e.Weight;
                    tmp[e.From] -= e.Weight;
                }
            for(int i = 0; i < g.VerticesCount; ++i)
            {
                if (tmp[i] != 0 && i != s && i != t)
                    return false;
            }
            return true;
        }

        public Graph FindConstrainedFlow(int source, int sink, Graph G, Graph lowerBounds)
        {
            
            Graph g = G.IsolatedVerticesGraph();
            int n = G.VerticesCount;
            double[] demands = new double[G.VerticesCount];


            bool sourceToSink = false;
            Edge sourceToSink_;
            Edge lowerSourceToSink_ = new Edge(0,0);
            bool[] tsink = new bool[G.VerticesCount];
            bool[] fsource = new bool[G.VerticesCount];
            for (int i = 0; i < G.VerticesCount; ++i) demands[i] = 0;
            for (int i = 0; i < G.VerticesCount; ++i)
                foreach (Edge e in G.OutEdges(i))
                {
                    double lower = lowerBounds.GetEdgeWeight(e.From, e.To);
                    g.AddEdge(new Edge(e.From, e.To, e.Weight - lower));
                    if (e.From == source && e.To == sink) // nic nie robic jak krawedz od zrodla do ujscia
                    {
                        sourceToSink_ = new Edge(e.From, e.To, e.Weight - lower);
                        lowerSourceToSink_ = new Edge(e.From, e.To, lower);
                        sourceToSink = true;
                        g.DelEdge(sourceToSink_);
                    }
                    else if (e.From == source || e.To == sink) // krawedz od zrodla lub do ujscia to nie modyfikujemy demands zrodla/ujscia
                    {
                        
                        if (e.To == sink) // krawedz od v do ujscia
                        {
                            demands[e.From] += lower;
                            tsink[e.From] = true; //e.From polaczone z sinkiem
                        }
                        else // krawedz od zrodla do v
                        {
                            demands[e.To] -= lower;
                            fsource[e.To] = true; // e.To polaczone z source
                        }
                    }
                    else // normalna krawedz, modyfikujemy oba demands
                    {
                        demands[e.From] += lower;
                        demands[e.To] -= lower;
                    }                   
                }
            //d(demands, n);
            for (int i = 0; i < G.VerticesCount; ++i)
            {
                if (i!= source && i!= sink) 
                {
                    if (fsource[i]) // jest polaczony ze zrodlem
                    {
                        double lower = lowerBounds.GetEdgeWeight(source, i);
                        double max = G.GetEdgeWeight(source, i) - lower;
                        
                        if (demands[i] > 0)
                        {
                            if (demands[i] <= max)
                                max = demands[i];
                            demands[source] -= max;
                        }
                        
                    }
                    if (tsink[i]) // jest polaczony z ujsciem
                    {
                        double lower = lowerBounds.GetEdgeWeight(i, sink);
                        double max = G.GetEdgeWeight(i, sink) - lower;


                        if (demands[i] > 0)
                        {
                            if (demands[i] <= max)
                                max = demands[i];
                            demands[sink] = demands[sink] + max;
                        }
                        //if (demands[i] < 0)
                        //{
                        //    if (-demands[i] <= max)
                        //        max = -demands[i];
                        //    demands[sink] += max;
                        //}
                    }

                }
            }
            //d(demands, n);
            double tmp = 0;
            for (int i = 0; i < G.VerticesCount; ++i)
                tmp += demands[i];
            if (tmp > 0)
            {
                demands[sink] -= tmp;
                //demands[source] -= tmp;
                //Console.WriteLine(tmp);
            }
            //demands[sink] -= tmp;
            else if (tmp < 0)
            {
                //demands[sink] += tmp;
                demands[source] += tmp;
                //Console.WriteLine(tmp);
            }
            //tmp = 0;
            ////for (int i = 0; i < G.VerticesCount; ++i)
            ////    tmp += demands[i];
            Graph tmpG = FindCirculation(g, demands);
            
            if (tmpG != null)
                for (int i = 0; i < tmpG.VerticesCount; ++i)
                    foreach (Edge e in tmpG.OutEdges(i))
                    {
                            tmpG.ModifyEdgeWeight(e.From, e.To, lowerBounds.GetEdgeWeight(e.From,e.To));
                    } 
            if (sourceToSink)
            {
                tmpG.AddEdge(lowerSourceToSink_);
            }
            return tmpG;
        }

    }
}