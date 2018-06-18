
namespace ASD
{
using System;

public class CarpentersBenchTestCase : TestCase
    {

    private (int length, int width) sheet;
    private (int length, int width, int price)[] elements1;
    private (int length, int width, int price)[] elements2;
    private int expectedResult;
    private int result;
    private Cut cuts;
        // nie
    public CarpentersBenchTestCase(double timeLimit, Exception expectedException, string description, (int length, int width) sheet, (int length, int width, int price)[] elements, int result)
           : base(timeLimit,expectedException,description)
        {
        this.sheet = sheet;
        this.elements1 = ((int length, int width, int price)[])elements.Clone();
        this.elements2 = ((int length, int width, int price)[])elements.Clone();
        this.expectedResult = result;
        }

    public override void PerformTestCase()
        {
        result = new CarpentersBench().Cut(sheet,((int length, int width, int price)[])elements2, out cuts);
        }

    public override void VerifyTestCase(out Result resultCode, out string message, object settings=null)
        {
        if ( result!=expectedResult )
            {
            resultCode = Result.BadResult;
            message = $"incorrect result: {result} (expected: {expectedResult})";
            return;
            }
        for ( int i=0 ; i<elements1.Length ; ++i )
            if ( !elements1[i].Equals(elements2[i]) )
                {
                resultCode = Result.BadResult;
                message = $"illegal parameter change: elements[{i}]";
                return;
                }
        if ( (bool)settings )
            {
            if ( cuts==null || sheet.length!=cuts.length || sheet.width!=cuts.width || result!=cuts.price || !VerifyCutting(cuts) )
                {
                resultCode = Result.BadResult;
                message = $"invalid cutting description";
                if (cuts == null) message = $"invalid cutting description1";
                if (sheet.length != cuts.length) message = $"invalid cutting description2";
                if (sheet.width != cuts.width) message = $"invalid cutting description3";
                    if (result != cuts.price) { Console.WriteLine("oczek: {0} - result,  {1} cutsprice  ",result,cuts.price); message = $"invalid cutting description4"; }
                if (!VerifyCutting(cuts)) message = $"invalid cutting description5";
                    return;
                }
            }
        resultCode = Result.Success;
        message = "OK";
        }

    private bool VerifyCutting(Cut cuts)
        {
        if ( cuts.n==0 )
            {
            if ( cuts.topleft!=null || cuts.bottomright!=null ) { Console.WriteLine("tutaj-2: L:{0} W:{1}", cuts.length, cuts.width); return false; }
                for (int i = 0; i < elements1.Length; ++i)
                    if (elements1[i].length == cuts.length && elements1[i].width == cuts.width)
                    { if (!(elements1[i].price == cuts.price)) { Console.WriteLine("tutaj"); } return elements1[i].price == cuts.price; }
                {  return cuts.price == 0; }
            }
        if (cuts.topleft == null || cuts.bottomright == null) { Console.WriteLine("tutaj1: L:{0} W:{1}", cuts.length, cuts.width); if (cuts.topleft != null && cuts.bottomright == null) Console.WriteLine("fail prawa"); else if (cuts.topleft == null && cuts.bottomright != null) Console.WriteLine("fail lewa"); else if (cuts.topleft == null && cuts.bottomright == null) Console.WriteLine("fail obie"); return false; }
        if ( cuts.topleft.price+cuts.bottomright.price!=cuts.price ) { Console.WriteLine("tutaj2: L:{0} W:{1} price:{2}, leftprice:{3}, rightprice:{4},   leftsize:({5},{6}), rightsize:({7},{8})", cuts.length, cuts.width, cuts.price, cuts.topleft.price, cuts.bottomright.price, cuts.topleft.length, cuts.topleft.width, cuts.bottomright.length, cuts.bottomright.width); return false; }
            if ( cuts.vertical ) // pionowe
            {
            if ( cuts.n<1 || cuts.n>=cuts.width ) { Console.WriteLine("tutaj3a"); return false; }
                // 1 linijka nizej ok, tylko zamieniona logika width z length
                if ( cuts.topleft.width!=cuts.n || cuts.bottomright.width!=cuts.width-cuts.n ) { Console.WriteLine("tutaj3333b"); return false; }
                if ( cuts.topleft.length!=cuts.length || cuts.bottomright.length!=cuts.length ) { Console.WriteLine("tutaj3333c"); return false; }
            }
        else // poziome
            {
            if ( cuts.n<1 || cuts.n>cuts.length ) { Console.WriteLine("tutaj4444a"); return false; }
                // 1 linijka nizej ok, tylko zamieniona logika width z length
                if ( cuts.topleft.length!=cuts.n || cuts.bottomright.length!=cuts.length-cuts.n ) { Console.WriteLine("tutaj44444b ({4},{5}) {0} != {1} || {2} != {3}", cuts.topleft.length, cuts.n, cuts.bottomright.length, cuts.length - cuts.n, cuts.length, cuts.width); return false; }
                if ( cuts.topleft.width!=cuts.width || cuts.bottomright.width!=cuts.width ) { Console.WriteLine("tutaj44444c"); return false; }
            }
        return VerifyCutting(cuts.topleft) && VerifyCutting(cuts.bottomright);
        }

    }

class CarpentersBenchTestModule : TestModule
    {

        public override void PrepareTestSets()
        {
            TestSets["LabResultOnlyTests"] = new TestSet("Part A - result only tests", null, false);
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (4, 4), new(int length, int width, int price)[] { (3, 3, 6), (1, 4, 1) }, 7));

            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (5, 5, 1) }, 1));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (2, 3, 1) }, 2));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 11), new(int length, int width, int price)[] { (4, 4, 21), (3, 3, 13), (2, 4, 10), (5, 1, 6), (1, 2, 2) }, 71));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (4, 5, 30) }, 0));

            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (4, 5, 30), (1, 1, 2), (2, 1, 1) }, 12));

            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (2, 1, 1), (1, 2, 2) }, 5));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (1, 11), new(int length, int width, int price)[] { (1, 3, 5), (1, 2, 3), (1, 1, 1) }, 18));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (11, 1), new(int length, int width, int price)[] { (3, 1, 5), (2, 1, 3), (1, 1, 1) }, 18));
            TestSets["LabResultOnlyTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (500, 500), new(int length, int width, int price)[] { (1, 1, 2), (10, 10, 11), (100, 100, 100), (500, 500, 100) }, 500000));

            TestSets["LabFullTests"] = new TestSet("Part B - full tests", null, true);

            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (4, 3), new(int length, int width, int price)[] { (2, 2, 300), (4, 1, 4) }, 604));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (3, 3), new(int length, int width, int price)[] { (1, 1, 1), (1, 2, 10), (2, 1, 10) }, 41));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (3, 3), new(int length, int width, int price)[] { (1, 2, 10), (3, 1, 5) }, 35));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (7, 7), new(int length, int width, int price)[] { (2, 1, 1), (1, 3, 2), (3, 3, 8), (2, 2, 3), (3, 2, 5) }, 40));

            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 4), new(int length, int width, int price)[] { (3, 3, 300), (1, 4, 4), (4, 1, 10) }, 314));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (3, 5), new(int length, int width, int price)[] { (2, 4, 300), (1, 1, 4), (4, 1, 10) }, 328));


            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 1), new(int length, int width, int price)[] { (4, 5, 30), (1, 1, 2), (2, 1, 1) }, 4));

            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (5, 5, 2) }, 2));

            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (5, 5, 1) }, 1));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 5), new(int length, int width, int price)[] { (2, 3, 1) }, 2));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (5, 11), new(int length, int width, int price)[] { (4, 4, 21), (3, 3, 13), (2, 4, 10), (5, 1, 6), (1, 2, 2) }, 71));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (4, 5, 30) }, 0));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (4, 5, 30), (1, 1, 2), (2, 1, 1) }, 12));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (2, 3), new(int length, int width, int price)[] { (2, 1, 1), (1, 2, 2) }, 5));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (1, 11), new(int length, int width, int price)[] { (1, 3, 5), (1, 2, 3), (1, 1, 1) }, 18));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (11, 1), new(int length, int width, int price)[] { (3, 1, 5), (2, 1, 3), (1, 1, 1) }, 18));
            TestSets["LabFullTests"].TestCases.Add(new CarpentersBenchTestCase(1, null, "", (500, 500), new(int length, int width, int price)[] { (1, 1, 2), (10, 10, 11), (100, 100, 100), (500, 500, 100) }, 500000));
        }

        public override double ScoreResult(out string message)
        {
        message = "OK";
        return 1;
        }

    }

public class Cut
    {

    public int length;       // wymiar pionowy elementu (przed cięciem)
    public int width;        // wymiar poziomy elementu (przed cięciem)
    public int price;        // sumaryczna wartość wszystkich elementów uzyskanych z pocięcia tego elementu

    public bool vertical;    // true dla cięcia pionowego, false dla cięcia poziomego
    public int n;            // odległość od lewej (dla cięcia pionowego) lub górnej (dla cięcia poziomego) krawędzi elementu
                             // UWAGA:  wartość 0 oznacza brak cięcia, składowe topleft i bottomright muszą być równe null,
                             //         a do składowej price wpisujemy zadaną wartość elementu (gdy jest jednym z pożądanych
                             //         lub 0 gdy nie jest (czyli jest bezwartościowym ścinkiem)

    public Cut topleft;      // informacje o lewym/górnym elemencie uzustanym w wyniku dokonanego cięcia
    public Cut bottomright;  // informacje o prawym/dolnym elemencie uzustanym w wyniku dokonanego cięcia

    public Cut(int length, int width, int price, bool vertical=true, int n=0, Cut topleft=null, Cut bottomright=null)
        {
        this.length = length;
        this.width = width;
        this.price = price;
        this.vertical = vertical;
        this.n = n;
        this.topleft = topleft;
        this.bottomright = bottomright;
        }

    }

class Lab02Main
    {

    public static void Main()
        {
        CarpentersBenchTestModule carpentersBenchTest = new CarpentersBenchTestModule();
        carpentersBenchTest.PrepareTestSets();

        foreach ( var ts in carpentersBenchTest.TestSets )
            {
            Console.WriteLine($"\n{ts.Value.Description}");
            ts.Value.PerformTests(verbose: true, checkTimeLimit: false);
            }

//        double grade = carpentersBenchTest.ScoreResult(out string _);
//        Console.Out.WriteLine($"\nOcena: {grade}\n");
        }

    }

}