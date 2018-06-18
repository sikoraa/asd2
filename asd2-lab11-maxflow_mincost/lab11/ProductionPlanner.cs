using System;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class ProductionPlanner : MarshalByRefObject
    {
        /// <summary>
        /// Flaga pozwalająca na włączenie wypisywania szczegółów skonstruowanego planu na konsolę.
        /// Wartość <code>true</code> spoeoduje wypisanie planu.
        /// </summary>
        public bool ShowDebug { get; } = false;
        
        /// <summary>
        /// Część 1. zadania - zaplanowanie produkcji telewizorów dla pojedynczego kontrahenta.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających maksymalną produkcję i zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się maksymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateSimplePlan(PlanData[] production, PlanData[] sales, PlanData storageInfo,
            out SimpleWeeklyPlan[] weeklyPlan)
        {
            weeklyPlan = null;
            if (!isOK(production, sales, storageInfo)) throw new ArgumentException();
            int n = production.Length;
            weeklyPlan = new SimpleWeeklyPlan[n];
            for(int i = 0; i < n; ++i)
            {
                weeklyPlan[i].UnitsProduced = 0;
                weeklyPlan[i].UnitsSold = 0;
                weeklyPlan[i].UnitsStored = 0;
            }
            int k = n + 2;
            int source = 0; int sink = n + 1;
            Graph g = new AdjacencyListsGraph<SimpleAdjacencyList>(true, k);
            Graph r = new AdjacencyListsGraph<SimpleAdjacencyList>(true, k);
            

            for (int j = 0; j < n-1; ++j)
            {
                g.AddEdge(new Edge(source, j+1, production[j].Quantity)); // dodawanie produkcji      //produkcja+,   sales-,  magazynowanie-
                r.AddEdge(new Edge(source, j+1, production[j].Value));

                g.AddEdge(new Edge(j + 1, sink, sales[j].Quantity)); // dodawanie sprzedazy
                r.AddEdge(new Edge(j+1, sink, -sales[j].Value));

                g.AddEdge(new Edge(j + 1, j + 2, storageInfo.Quantity)); // dodawanie magazynu
                r.AddEdge(new Edge(j + 1, j + 2, storageInfo.Value));
            }
            
            g.AddEdge(source, n, production[n - 1].Quantity); // ostatni tydzien produkcji
            r.AddEdge(source, n, production[n - 1].Value);
            g.AddEdge(n, sink, sales[n - 1].Quantity); // ostatni tydzien sprzedazy
            r.AddEdge(n, sink, -sales[n - 1].Value);
            (double value, double cost, Graph flow) ret = MinCostFlowGraphExtender.MinCostFlow(g, r, source, sink, false, MaxFlowGraphExtender.PushRelabelMaxFlow, null, false);
            int produced = 0;
            for (int i = 0; i < n; ++i)
            {
                produced += (int)ret.flow.GetEdgeWeight(source, i + 1);
                weeklyPlan[i].UnitsProduced = (int)ret.flow.GetEdgeWeight(source, i + 1);
                weeklyPlan[i].UnitsSold = (int)ret.flow.GetEdgeWeight(i + 1, sink);
                if (i != n - 1)
                    weeklyPlan[i].UnitsStored = (int)ret.flow.GetEdgeWeight(i + 1, i + 2);
                else
                    weeklyPlan[i].UnitsStored = 0;
             }
            return new PlanData {Quantity = produced, Value = -ret.cost};
        }

        /// <summary>
        /// Część 2. zadania - zaplanowanie produkcji telewizorów dla wielu kontrahentów.
        /// </summary>
        /// <remarks>
        /// Do przeprowadzenia testów wyznaczających produkcję dającą maksymalny zysk wymagane jest jedynie zwrócenie obiektu <see cref="PlanData"/>.
        /// Testy weryfikujące plan wymagają przypisania tablicy z planem do parametru wyjściowego <see cref="weeklyPlan"/>.
        /// </remarks>
        /// <param name="production">
        /// Tablica obiektów zawierających informacje o produkcji fabryki w kolejnych tygodniach.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza limit produkcji w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - koszt produkcji jednej sztuki.
        /// </param>
        /// <param name="sales">
        /// Dwuwymiarowa tablica obiektów zawierających informacje o sprzedaży w kolejnych tygodniach.
        /// Pierwszy wymiar tablicy jest równy liczbie kontrahentów, zaś drugi - liczbie tygodni w planie.
        /// Wartości pola <see cref="PlanData.Quantity"/> oznaczają maksymalną sprzedaż w danym tygodniu,
        /// a pola <see cref="PlanData.Value"/> - cenę sprzedaży jednej sztuki.
        /// Każdy wiersz tablicy odpowiada jednemu kontrachentowi.
        /// </param>
        /// <param name="storageInfo">
        /// Obiekt zawierający informacje o magazynie.
        /// Wartość pola <see cref="PlanData.Quantity"/> oznacza pojemność magazynu,
        /// a pola <see cref="PlanData.Value"/> - koszt przechowania jednego telewizora w magazynie przez jeden tydzień.
        /// </param>
        /// <param name="weeklyPlan">
        /// Parametr wyjściowy, przez który powinien zostać zwrócony szczegółowy plan sprzedaży.
        /// </param>
        /// <returns>
        /// Obiekt <see cref="PlanData"/> opisujący wyznaczony plan.
        /// W polu <see cref="PlanData.Quantity"/> powinna znaleźć się optymalna liczba wyprodukowanych telewizorów,
        /// a w polu <see cref="PlanData.Value"/> - wyznaczony maksymalny zysk fabryki.
        /// </returns>
        public PlanData CreateComplexPlan(PlanData[] production, PlanData[,] sales, PlanData storageInfo,
            out WeeklyPlan[] weeklyPlan)
        {
            weeklyPlan = null;
            if (!isOK(production, sales, storageInfo))
                throw new ArgumentException();
            
            int count = sales.GetLength(0); // count ilosc kotrahentow
            int n = production.Length; // n ilosc tygodni
            weeklyPlan = new WeeklyPlan[n];
            for (int i = 0; i < n; ++i)
                weeklyPlan[i].UnitsSold = new int[count];
            int source = n;
            int sink = n + 1;
            int start = n + 2; // nr pierwszego kontrahenta
            int end = start + count - 1; // nr ostatniego kontrahenta
            
            int V = 2 + count + n + n ;
            
            int storageStart = end + 1;
            int storageEnd = end + n;

            Graph g = new AdjacencyListsGraph<SimpleAdjacencyList>(true, V);
            Graph r = new AdjacencyListsGraph<SimpleAdjacencyList>(true, V);
            for (int i = start; i <= end; ++i)
            {
                // ujscie od kontrahenta
                g.AddEdge(i, sink, Double.MaxValue);//Double.MaxValue);
                r.AddEdge(i, sink, 0);
                // polaczenie sprzedazy kontrahentom
                int buyer = i - start;
                for(int tydzien = 0; tydzien < n; ++tydzien)
                {
                    g.AddEdge(storageStart + tydzien, i, sales[buyer, tydzien].Quantity);
                    r.AddEdge(storageStart + tydzien, i, -sales[buyer, tydzien].Value);
                }
            }    
            for(int j = 0; j < n; ++j)
            {
                // dodanie produkcji (ze zrodla do tygodnia)
                g.AddEdge(source, j, production[j].Quantity);
                r.AddEdge(source, j, production[j].Value);
                // dodanie ujscia dla produkcji, ktorej sie nie oplaca produkowac
                g.AddEdge(j, sink, Double.MaxValue);
                r.AddEdge(j, sink, -production[j].Value);

                // dodanie krawedzi miedzy produkcja a wierzcholkiem kontrolujacym wyjscie do kontrahentow        
                g.AddEdge(j, storageStart + j, production[j].Quantity );
                r.AddEdge(j, storageStart + j, 0);
                
                // dodanie magazynu
                if (j != n - 1) // bez ostatniego dnia bo wtedy nic nie magazynujemy
                {
                    g.AddEdge(storageStart+j, storageStart+j + 1, storageInfo.Quantity);
                    r.AddEdge(storageStart + j, storageStart + j + 1, storageInfo.Value);
                }
            }
            
            (double value, double cost, Graph flow) ret = MinCostFlowGraphExtender.MinCostFlow(g, r, source, sink, false, MaxFlowGraphExtender.PushRelabelMaxFlow, null, false);
            int produced = 0;
            double profit = 0;
            for (int i = 0; i < n; ++i)
            {
                // produkcja
                produced += weeklyPlan[i].UnitsProduced = production[i].Quantity - (int)ret.flow.GetEdgeWeight(i, sink);
                profit -= weeklyPlan[i].UnitsProduced * production[i].Value;
                // sprzedaz
                for (int j = start; j <= end; ++j)
                {
                    weeklyPlan[i].UnitsSold[j - start] = (int)ret.flow.GetEdgeWeight(storageStart + i, j);
                    profit += ret.flow.GetEdgeWeight(storageStart + i, j) * (sales[j - start, i].Value); // (weeklyPlan[i].UnitsSold[j - start] * (sales[j - start, i].Value));
                }
                // magazyn
                if (i != n - 1)
                {
                    weeklyPlan[i].UnitsStored = (int)ret.flow.GetEdgeWeight(storageStart +i, storageStart +i + 1);
                    profit -= weeklyPlan[i].UnitsStored * storageInfo.Value;
                }
                
            }
            return new PlanData { Quantity = produced, Value = Math.Ceiling(profit) };
        }
        
        bool isOK(PlanData[] production, PlanData[,] sales, PlanData storageInfo)
        {
            if (production.Length != sales.GetLength(1)) return false;
            if (production.Length <= 0) return false;
            if (sales.GetLength(0) <= 0) return false;
            if (storageInfo.Value < 0 || storageInfo.Quantity < 0) return false;
            for(int i = 0; i < production.Length; ++i)
            {
                if (production[i].Quantity < 0 || production[i].Value < 0) return false;
                for (int j = 0; j < sales.GetLength(0); ++j)
                    if (sales[j, i].Value < 0 || sales[j, i].Quantity < 0) return false;
            }           
            return true;
        }

        bool isOK(PlanData[] production, PlanData[] sales, PlanData storageInfo)
        {
            if (production.Length != sales.Length) return false;
            if (production.Length <= 0) return false;
            if (sales.Length <= 0) return false;
            if (storageInfo.Value < 0 || storageInfo.Quantity < 0) return false;
            for (int i = 0; i < production.Length; ++i)
            {
                if (production[i].Quantity < 0 || production[i].Value < 0) return false;
                if (sales[i].Value < 0 || sales[i].Quantity < 0) return false;
            }
            return true;
        }
    }

    
}