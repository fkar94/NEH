using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public static class GlobalMembers
{
    /*sortowanie po sumie czasow wykonywania
    na wszystkich maszynach od malejaco
    Uzywana raz po wczytaniu danych*/

    public static bool sorting(List<int> a, List<int> b)
    {
        int sum1 = accumulate(a.GetEnumerator(), a.end(), 0);
        int sum2 = accumulate(b.GetEnumerator(), b.end(), 0);
        return sum1 > sum2;
    }

    /*dodawanie zadania do permutacji, wykorzystywane przy generowaniu permutacji
    zadanie to tak naprawde wketor czasow wykonywania na kazdej maszynie*/
    public static void insert(List<List<int>> C, List<List<int>> P, int pos, int num_of_job)
    {
        
        C.insert(C.GetEnumerator() + pos, P[num_of_job]);
        
        P.insert(P.GetEnumerator() + pos, P[num_of_job]);
        P.RemoveAt(1 + num_of_job);
    }
    /*przywracanie stanu wczesniejszej permutacji po sprawdzeniu C_max*/
    public static void restore(List<List<int>> C, List<List<int>> P, int pos, int num_of_job)
    {
        C.RemoveAt(pos);
        
        P.insert(P.GetEnumerator() + num_of_job + 1, P[pos]);
        P.RemoveAt(pos);
    }

    /*obliczanie C_max dla danej permutacji*/
    public static int find_c_max(List<List<int>> C, List<List<int>> P, int n, int m)
    {
        int j;
        int k;
        for (j = 1; j <= n; ++j)
        {
            for (k = 1; k <= m; ++k)
            {
                if (C[j][k - 1] > C[j - 1][k])
                {
                    C[j][k] = C[j][k - 1] + P[j][k];
                }
                else
                {
                    C[j][k] = C[j - 1][k] + P[j][k];
                }

            }
        }
        return C[j - 1][k - 1];
    }

    static int Main()
    {
        string[] files = { "NEH1.DAT", "NEH2.DAT", "NEH3.DAT", "NEH4.DAT", "NEH5.DAT", "NEH6.DAT", "NEH7.DAT", "NEH8.DAT", "NEH9.DAT" };
        fstream file = new fstream();

        int n;
        int m;
        int p;
        int c_max;
        int min_c_max;
        int min_j;
        int min_k;
        List<List<int>> P = new List<List<int>>();
        List<List<int>> C = new List<List<int>>();

        for (int w = 0; w < 9; ++w)
        {
            file.open(files[w], ios.w);
            if (file.good())
            {
                //pobieramy dane o ilosci zadan i maszyn
                file >> n >> m;
                /*ustalanie rozmiaru: o 1 tablica 2d wiersz o kolumna o 1 wieksza od ilosci danych aby w elemencie zerowym bylo 0.
                Jest to potrzebne by zachowa� warunki brzegowe. W funkcji find_max_c odwolujemy si� do elementu poprzedniego tablicy
                wiec nie mozemy uzywac elementu zerowego by nie wyjsc poza zakres*/
                P.resize(n + 1);
                C.resize(n + 1);
                for (int i = 0; i <= n; ++i)
                {
                    P[i].resize(m + 1);
                    C[i].resize(m + 1);
                }

                for (int i = 0; i <= n; ++i)
                {
                    for (int j = 0; j <= m; ++j)
                    {
                        /*warunki brzegowe, wypelniamy tablice zerami w nastepujacy sposob:
                        00000000000000
                        0
                        0
                        0
                        0
                        0
                        */
                        P[0][j] = 0;
                        P[i][0] = 0;
                        C[0][j] = 0;
                        C[i][0] = 0;
                    }
                }

                for (int i = 1; i <= n; ++i)
                {
                    for (int j = 1; j <= m; ++j)
                    {
                        //pobieramy dane o czasie wykonywania
                        file >> p;
                        P[i][j] = p;
                    }
                }

                //faza pierwsza: sortowanie malejaco po sumie czasow wykonywania na poszczegolnych maszynach
                sort(P.GetEnumerator() + 1, P.end(), sorting);

                /*generujemy pemutacje w sposob nastepujacy:
				
                                        1
                                12				21
                            123 312 132		213 321 231
                            itp.
	
                w kazdym etapie wybieramy permutacje o mniejszym cmax i z nia przechodzimy do kolejnego etapu
                czyli w powyzszym przykkladzie jesli 12 ma mniejsze cmax od 21, to w 3 etapie rozwazamy juz tylko lewa czesc powyzszego "drzewka"
                */

                for (int k = 1; k <= n; k++)
                {
                    //poczatkowa wartosc cmax baaaardzo duza
                    c_max = 999999999;
                    min_c_max = 999999999;
                    for (int j = 1; j <= k; j++)
                    {
                        //dodajemy element do permutacji
                        insert(C, P, j, k);
                        //liczymy cmax
                        c_max = find_c_max(C, P, k, m);
                        /*jesli cmax jest mniejsze od poprzedniego
                        to zapamietaujemy cmax oraz permutacje*/
                        if (c_max < min_c_max)
                        {
                            min_c_max = c_max;
                            min_j = j;
                            min_k = k;
                        }
                        /*
                        wracamy do permutacji poprzedniej, aby umiescic zadanie w na kolejnej pozycji, 
                        tzn wygenerowac nowa permutacje
                        */
                        restore(C, P, j, k);
                        /*
                        po wygenerowaniu wszystkich permutacji na danym etapie wracamy do tej, ktora 
                        miala najmniejszy cmax
                        */
                        if (j == k)
                        {
                            insert(C, P, min_j, min_k);
                        }

                    }
                }
                //obliczamy cmax dla finalnej permutacji i wypisujemy na ekran
                c_max = find_c_max(C, P, n, m);
                Console.Write("Wynik dla danych z pliku \"");
                Console.Write(files[w]);
                Console.Write("\": ");
                Console.Write(c_max);
                Console.Write("\n");
                file.close();
            }
            else
            {
                Console.Write("Nie mozna otworzyc pliku \"");
                Console.Write(files[w]);
                Console.Write("\"");
                Console.Write("\n");
                Console.ReadLine();
                return -1;
            }

        }

        Console.ReadLine();
        return 0;
    }
}