using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
// ARKADIUSZ SIKORA
namespace ASD
{

    // Klasy Lab03Helper NIE WOLNO ZMIENIAĆ !!!
    public class Lab03Helper : System.MarshalByRefObject
    {

        public Graph LineGraph(Graph graph, out (int x, int y)[] names) => graph.LineGraph(out names);
        public Graph SquareOfGraph(Graph graph) => graph.SquareOfGraph();

        public Graph LineGraph(Graph graph, out int[,] names)
        {
            Graph g = graph.LineGraph(out (int x, int y)[] _names);
            if (_names == null)
                names = null;
            else
            {
                names = new int[_names.Length, 2];
                for (int i = 0; i < _names.Length; ++i)
                {
                    names[i, 0] = _names[i].x;
                    names[i, 1] = _names[i].y;
                }
            }
            return g;
        }
        public int VertexColoring(Graph graph, out int[] colors) => graph.VertexColoring(out colors);
        public int StrongEdgeColoring(Graph graph, out Graph coloredGraph) => graph.StrongEdgeColoring(out coloredGraph);

    }

    // Uwagi do wszystkich metod
    // 1) Grafy wynikowe powinny być reprezentowane w taki sam sposób jak grafy będące parametrami
    // 2) Grafów będących parametrami nie wolno zmieniać
    static class Lab03
    {

        // 0.5 pkt
        // Funkcja zwracajaca kwadrat grafu graph.
        // Kwadratem grafu nazywamy graf o takim samym zbiorze wierzcholkow jak graf bazowy,
        // 2 wierzcholki polaczone sa krawedzia jesli w grafie bazowym byly polaczone krawedzia badz sciezka zlozona z 2 krawedzi
        public static Graph SquareOfGraph(this Graph graph)
        {
            if (graph.VerticesCount < 3) return graph.Clone();
            Graph k = graph.Clone();
            
            for (int i = 0; i < graph.VerticesCount; ++i) // dla kazdego wierzcholka
            {
                foreach(Edge e in graph.OutEdges(i)) // dla kazdej krawedzi wychodzacej z tego wierzcholka
                {
                    int middleV = e.To; // i--(e)-> middleV--(e1)-> j
                    foreach (Edge e1 in graph.OutEdges(middleV)) // dla kazdej krawedzi sasiadujacej z krawedzia, dodajemy krawedz-skrot
                    {
                        if (e1.To != i)
                        {
                            k.AddEdge(i, e1.To);                           
                        }
                    }
                } 
            }  
            return k;
        }

        // 2 pkt
        // Funkcja zwracająca Graf krawedziowy grafu graph
        // Wierzcholki grafu krawedziwego odpowiadaja krawedziom grafu bazowego,
        // 2 wierzcholki grafu krawedziwego polaczone sa krawedzia
        // jesli w grafie bazowym z krawędzi odpowiadającej pierwszemu z nic można przejść 
        // na krawędź odpowiadającą drugiemu z nich przez wspólny wierzchołek.
        //
        // (w grafie skierowanym: 2 wierzcholki grafu krawedziwego polaczone sa krawedzia
        // jesli wierzcholek koncowy krawedzi odpowiadajacej pierwszemu z nich
        // jest wierzcholkiem poczatkowym krawedzi odpowiadajacej drugiemu z nich)
        //
        // do tablicy names nalezy wpisac numery wierzcholkow grafu krawedziowego,
        // np. dla wierzcholka powstalego z krawedzi <0,1> do tabeli zapisujemy krotke (0, 1) - przyda się w dalszych etapach
        //
        // UWAGA: Graf bazowy może być skierowany lub nieskierowany, graf krawędziowy zawsze jest nieskierowany.
        public static Graph LineGraph(this Graph graph, out (int x, int y)[] names)
        {
            // Moze warto stworzyc...
            // graf pomocniczy o takiej samej strukturze krawedzi co pierwotny, 
            // waga krawedzi jest numer krawedzi w grafie (taka sztuczka - to beda numery wierzcholkow w grafie krawedzowym)
            int n = graph.VerticesCount;
            int p = 0; // liczba i numer krawedzi dodawanej do grafu pomocniczego
            Graph tmp = graph.IsolatedVerticesGraph(graph.Directed, n); // graf pomocniczy
            if (graph.Directed == false) // graf nieskierowany
            {
                for (int i = 0; i < graph.VerticesCount; ++i)
                    foreach (Edge e in graph.OutEdges(i)) // konstrukcja grafu pomocniczego
                    {
                        if (i < e.To)
                        {
                            tmp.AddEdge(i, e.To, p++);
                        }// p to liczba krawedzi
                        else
                            tmp.AddEdge(i, e.To, tmp.GetEdgeWeight(e.To, i));
                    }
                Graph r = graph.IsolatedVerticesGraph(false, p); // graf krawedziowy
                names = new(int x, int y)[p]; // tablica na nazwy wierzcholkow
                int j = 0;             
                    for (int i = 0; i < n; ++i)
                    {
                        foreach (Edge e in tmp.OutEdges(i))
                        {
                            if (i < e.To)
                            {
                                names[j++] = (i, e.To); // wpisanie nazwy, porzadek (0, 1), (0,2), (1,2), (4,0) itd..
                                foreach (Edge e1 in tmp.OutEdges(i))
                                {
                                    if (i < e1.To && e.To != e1.To && Double.IsNaN(r.GetEdgeWeight((int)e.Weight, (int)e1.Weight)) && Double.IsNaN(r.GetEdgeWeight((int)e1.Weight, (int)e.Weight)))
                                    {
                                        r.AddEdge((int)e.Weight, (int)e1.Weight,1);
                                    }                         
                                }
                                foreach (Edge e2 in tmp.OutEdges(e.To))
                                {
                                    if (Double.IsNaN(r.GetEdgeWeight((int)e2.Weight, (int)e.Weight)) && Double.IsNaN(r.GetEdgeWeight((int)e.Weight, (int)e2.Weight)) && (i < e2.To))
                                    {
                                        r.AddEdge((int)e.Weight, (int)e2.Weight,1);
                                    }
                                }
                            }
                        }
                    }
                    return r;
            }
            else // GRAF SKIEROWANY
            {
                    for (int i = 0; i < graph.VerticesCount; ++i)
                        foreach (Edge e in graph.OutEdges(i))
                        {  
                                tmp.AddEdge(i, e.To, p++);
                            // p to liczba krawedzi
                        }
                    Graph rp = graph.IsolatedVerticesGraph(false, p); // graf krawedziowy

                    names = new(int x, int y)[p];
                    int jj = 0; // zmienna do dodawania nazw wierzcholkow na odpowiedni index
                    for (int i = 0; i < n; ++i)
                    {
                        foreach (Edge e in tmp.OutEdges(i))
                        {
                            names[jj++] = (i, e.To); // porzadek od najmniejszego wierzcholka
                            foreach (Edge e2 in tmp.OutEdges(e.To))
                            {
                                    rp.AddEdge((int)e.Weight, (int)e2.Weight); // a do grafu dodajemy krawedzie
                            }
                        }
                    }
                    return rp;
            }
        }

       

        // 1 pkt
        // Funkcja znajdujaca poprawne kolorowanie wierzcholkow grafu graph
        // Kolorowanie wierzcholkow jest poprawne, gdy kazde dwa sasiadujace wierzcholki maja rozne kolory
        // Funkcja ma szukać kolorowania wedlug nastepujacego algorytmu zachlannego:
        //
        // Dla wszystkich wierzcholkow (od 0 do n-1) 
        //      pokoloruj wierzcholek v na najmniejszy mozliwy kolor (czyli taki, na ktory nie sa pomalowani jego sasiedzi)
        //
        // Nalezy zwrocic liczbe kolorow, a w tablicy colors zapamietac kolory dla poszczegolnych wierzcholkow
        //
        // UWAGA: Dla grafów skierowanych metoda powinna zgłaszać wyjątek ArgumentException
        public static int VertexColoring(this Graph graph, out int[] colors)
        {
            colors = null;
            if (graph == null) return 0;
            if (graph.Directed == true) throw new ArgumentException();
            
            int n = graph.VerticesCount;
            if (n == 0) { colors = new int[0]; return 0; }
            colors = new int[n];
            bool[] available = new bool[n];
            for (int i = 0; i < n; ++i) colors[i] = -1;
            for (int v = 0; v < n; ++v)
            {
                for (int i = 0; i < n; ++i) { available[i] = true; } // wszystkie kolory sa dostepne na start kazdej petli
                foreach (Edge e in graph.OutEdges(v)) // badamy kolory sasiadow
                {    
                        if (colors[e.To] != -1) // jesli sasiad ma kolor
                            available[colors[e.To]] = false; // to jego kolor jest niedostepny
                }
                for (int j = 0; j < n; ++j) // przegladamy wszystkie kolory, z zamiarem wziecia najmniejszego dostepnego
                {
                    if (available[j] == true) // bierzemy pierwszy dostepny kolor ( najmniejszy )
                    {
                        colors[v] = j;
                        break; // wyjscie z fora wybierajacego kolor
                    }
                }
            }
            return colors.Max() + 1;
            //return colors.Distinct().ToArray().Length; // zwraca ilosc roznych zuzytych kolorow
        }
        // graf krawedziowy -> kwadrat -> kolorowanie -> powrocic do
        // 0.5 pkt
        // Funkcja znajdujaca silne kolorowanie krawedzi grafu graph
        // Silne kolorowanie krawedzi grafu jest poprawne gdy kazde dwie krawedzie, ktore sa ze soba sasiednie
        // albo sa polaczone inna krawedzia, maja rozne kolory.
        //
        // Nalezy zwrocic nowy graf, ktory bedzie kopia zadanego grafu, ale w wagach krawedzi zostana zapisane znalezione kolory
        // 
        // Wskazowka - to bardzo proste. Nalezy tu wykorzystac wszystkie poprzednie funkcje. 
        // Zastanowic sie co mozemy powiedziec o kolorowaniu wierzcholkow kwadratu grafu krawedziowego - jak sie ma do silnego kolorowania krawedzi grafu bazowego
        public static int StrongEdgeColoring(this Graph graph, out Graph coloredGraph) // test 8 error domowe silne kolorowanie
        {
            coloredGraph = graph;
            if (graph == null || graph.VerticesCount == 0) return 0;

            (int x, int y)[] names = null;
            int[] colors = null;

            int n = VertexColoring(SquareOfGraph(LineGraph(graph, out names)), out colors); // tworzymy graf krawedziowy, nastepnie tworzymy jego kwadrat, i to kolorujemy
            coloredGraph = graph.IsolatedVerticesGraph();

            long j = 0;
            while (j < names.Length)
            {
                coloredGraph.AddEdge(names[j].x, names[j].y, colors[j]);
                ++j;
            }
            
            return n; // liczba zuzytych kolorow
        }
    }
}
