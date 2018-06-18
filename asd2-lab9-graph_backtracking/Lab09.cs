using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    // DEFINICJA
    // Skojarzeniem indukowanym grafu G nazywamy takie skojarzenie M,
    // ze żadne dwa konce roznych krawedzi z M nie sa polaczone krawedzia w G

    // Uwagi do obu metod
    // 1) Grafow bedacych parametrami nie wolno zmieniac
    // 2) Parametrami są zawsze grafy nieskierowane (nie trzeba tego sprawdzac)

    public class Lab09 : MarshalByRefObject
    {

        /// <summary>
        /// Funkcja znajduje dowolne skojarzenie indukowane o rozmiarze k w grafie graph
        /// </summary>
        /// <param name="graph">Badany graf nieskierowany</param>
        /// <param name="k">Rozmiar szukanego skojarzenia indukowanego</param>
        /// <param name="matching">Znalezione skojarzenie (lub null jeśli nie ma)</param>
        /// <returns>true jeśli znaleziono skojarzenie, false jesli nie znaleziono</returns>
        /// <remarks>
        /// Punktacja:  2 pkt, w tym
        ///     1.5  -  dzialajacy algorytm (testy podstawowe)
        ///     0.5  -  testy wydajnościowe
        /// </remarks>
        public List<Edge> ans;
        public double best_Val;
        public bool[] Checked;
    
        void p(List<Edge> M)
        {
            Console.WriteLine("start----");
            foreach (Edge e in M)
                Console.Write("({0},{1})  ", e.From, e.To);
            Console.WriteLine("\n------------");
        }

        public bool rek2(Graph graph, int v, int i, int k, List<Edge> M, bool[] vVisited) 
        {
            if (i == k) // jesli skojarzenie wielkosci k to konczymy i zapisujemy wynik do ans
            {
                foreach (Edge e in M)
                    ans.Add(e);
                return true;
            }
            int n = graph.VerticesCount;
            if (v >= n) return false; // wierzcholek nie istnieje
            if (vVisited[v]) return false; // wierzcholek odwiedzony lub sasiad odwiedzonego
            if (i + (n - v + 1)/2 < k) return false; // napewno juz nie osiagniemy skojarzenia wielkosci k, bo ilosc wierzcholkow / 2 nie dopelni k - i
            vVisited[v] = true;
            bool[] tmp = (bool[])vVisited.Clone(); // pomocnicza tablica booli, wszystkich sasiadow v oznaczamy tu na true
            foreach (Edge e in graph.OutEdges(v))
                 tmp[e.To] = true;
            foreach (Edge e in graph.OutEdges(v)) // przechodzimy jeszcze raz po krawedziach z v
            {
                if (e.From >= e.To) continue; // zeby nie przechodzic tych samych krawedzi pare razy
                if (vVisited[e.To]) continue; // jesli nie mozemy wykorzystac krawedzi to continue
                bool[] b = (bool[])tmp.Clone(); // kopiujemy tablice booli, do rekurencyjnego wywolania, oznaczamy sasiadow e.To, po to kopiujemy, zeby potem nie czyscic
                foreach (Edge e1 in graph.OutEdges(e.To))
                    b[e1.To] = true;
                M.Add(e); // dodajemy krawedz e do skojarzenia
                for(int vv = v+1; vv < n; ++vv) // rekurencyjnie wywolujemy dla kolejnych wierzcholkow
                    if (rek2(graph, vv, i + 1, k, M, b)) return true;
                M.Remove(e); // usuwamy krawedz e ze skojarzenia
             }
            vVisited[v] = false;        
            if (rek2(graph, v + 1, i, k, M, vVisited)) return true; // rekurencyjne wywolanie, jesli nie bierzemy krawedzi z v do skojarzenia
            return false;
        }

        public bool InducedMatching(Graph graph, int k, out Edge[] matching)
        {
            matching = null;
            ans = new List<Edge>();
            int n = graph.VerticesCount;
            bool answer;
            answer = rek2(graph, 0, 0, k, new List<Edge>(), new bool[n]);
            if (answer == true)
            {
                matching = ans.ToArray();
                return answer;
            }
            return false;
        }

        /// <summary>
        /// Funkcja znajduje skojarzenie indukowane o maksymalnej sumie wag krawedzi w grafie graph
        /// </summary>
        /// <param name="graph">Badany graf nieskierowany</param>
        /// <param name="matching">Znalezione skojarzenie (jeśli puste to tablica 0-elementowa)</param>
        /// <returns>Waga skojarzenia</returns>
        /// <remarks>
        /// Punktacja:  2 pkt, w tym
        ///     1.5  -  dzialajacy algorytm (testy podstawowe)
        ///     0.5  -  testy wydajnościowe
        /// </remarks>
        public double MaximalInducedMatching(Graph graph, out Edge[] matching)
        {
            matching = new Edge[0];
            ans = new List<Edge>();
            best_Val = Double.MinValue;
            int p = -1;
            Checked = new bool[graph.VerticesCount];
            //for (int i = 0; i < graph.VerticesCount; ++i)
            //{
            //    foreach (Edge e in graph.OutEdges(i))
            //    {
            //        if (e.Weight > 0 && p == -1) p = i;
            //    }
            //}
            //if (p == -1) return 0;
            bool answer = recFindMatching(graph, 0, 0, new List<Edge>(), new bool[graph.VerticesCount], Double.MinValue, false);
            if (best_Val > Double.MinValue)
            {
                matching = ans.ToArray();
                return best_Val;
            }
            return 0;
        }
        // argument bool problem, true jak odpalona funkcja bez dodania krawedzi, wtedy mozliwe powtarzanie tych samych rozwiazan
        public bool recFindMatching(Graph graph, int v, int i, List<Edge> M, bool[] vVisited, double tmpVal, bool problem) 
        {
            if (tmpVal > Double.MinValue && tmpVal < 0) return false;
            if (tmpVal > best_Val)
            {
                ans = new List<Edge>();
                best_Val = tmpVal;
                foreach (Edge e in M)
                    ans.Add(e);
            }
            int n = graph.VerticesCount;
            if (v >= n) return false;
            if (vVisited[v]) return false;
            vVisited[v] = true;
            bool[] tmp = (bool[])vVisited.Clone();
            foreach (Edge e in graph.OutEdges(v))
            {
                tmp[e.To] = true;
            }
            foreach (Edge e in graph.OutEdges(v))
            {
                if (e.From >= e.To) continue;
                if (vVisited[e.To]) continue;
                if (e.Weight <= 0) continue;
                if (tmpVal == Double.MinValue) tmpVal = 0;
                bool[] b = (bool[])tmp.Clone();
                foreach (Edge e1 in graph.OutEdges(e.To))
                    b[e1.To] = true;
                M.Add(e);
                for (int vv = v + 1; vv < n; ++vv)
                    recFindMatching(graph, vv, i + 1, M, b, tmpVal + e.Weight, false);
                M.Remove(e);
            }
            vVisited[v] = false;
            Checked[v] = true; // checked oznacza czy juz bylismy w tym wierzcholku i rozwaliz
            if (v + 1 < n)
                if (!Checked[v + 1] || problem)
                    recFindMatching(graph, v + 1, i, M, vVisited, tmpVal, true);  
            return false;
        }
    }
}


