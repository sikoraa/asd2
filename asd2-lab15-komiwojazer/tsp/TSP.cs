
namespace ASD.Graphs
{

public class TSPHelper : System.MarshalByRefObject
    {

    /// <summary>
    /// Znajduje rozwiązanie dokładne problemu komiwojażera metodą podziału i ograniczeń
    /// </summary>
    /// <param name="g">Badany graf</param>
    /// <param name="cycle">Znaleziony cykl (parametr wyjściowy)</param>
    /// <returns>Długość znalezionego cyklu (suma wag krawędzi)</returns>
    /// <remarks>
    /// Metoda przeznaczona jest dla grafów z nieujemnymi wagami krawędzi.<br/>
    /// Uruchomiona dla grafu zawierającego krawędź o wadze ujemnej zgłasza wyjątek <see cref="System.ArgumentException"/>.<br/>
    /// <br/>
    /// Elementy (krawędzie) umieszczone są w tablicy <i>cycle</i> w kolejności swojego następstwa w znalezionym cyklu Hamiltona.<br/>
    /// <br/>
    /// Jeśli w badanym grafie nie istnieje cykl Hamiltona metoda zwraca wartość <b>NaN</b>,
    /// parametr wyjściowy <i>cycle</i> ma wówczas wartość <b>null</b>.<br/>
    /// <br/>
    /// Metodę można stosować dla grafów skierowanych i nieskierowanych.
    /// </remarks>
    public double TSP(Graph g, out Edge[] cycle)
        {

        cycle = null;  // zmienić
        long V = g.VerticesCount;
        double[,] G = new double[V, V];
        //if (g.Directed)
        {
                for (int i = 0; i < V; ++i)
                    foreach (Edge e in g.OutEdges(i))
                        if (e.Weight < 0) throw new System.ArgumentException();
                        else G[e.From, e.To] = e.Weight;
        }
        double ret = 0;

        if (cycle == null)
            return double.NaN;
        else
            return ret;
        }

    // wskazówka 1: wewnątrz algorytmu nie trzymać się "bibliotecznych" reprezantacji grafu,
    //              uzywać bezpośrednio tablicy opisanej w algorytmie
    // wskazówka 2: można zdefiniować pomocniczą prywatną klasę,
    //              jej metody powinny realizować poszczególne elementy omawianego algorytmu
    //              (w jednej monolitycznej metodzie będzie trudno)
    // wskazówka 3: nie zapominać o komentarzach (chciałbym rozumieć co się dzieje)

    }

}  // namespace ASD.Graphs
