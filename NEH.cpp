#include <iostream>
#include <string>
#include <fstream>
#include <vector>
#include <cmath>
#include <algorithm>
#include <numeric>

using namespace std;


/*sortowanie po sumie czasow wykonywania
na wszystkich maszynach od malej¹co
Uzywana raz po wczytaniu danych*/
bool sorting(const vector< int >& a, const vector< int >& b)
{
	int sum1 = accumulate(a.begin(), a.end(), 0);
	int sum2 = accumulate(b.begin(), b.end(), 0);
	return sum1 > sum2;
}

/*dodawanie zadania do permutacji, wykorzystywane przy generowaniu permutacji
zadanie to tak naprawde wketor czasow wykonywania na kazdej maszynie*/
void insert(vector<vector <int>> & C, vector <vector <int>> & P, int pos, int num_of_job)
{
	C.insert(C.begin() + pos, P[num_of_job]);
	P.insert(P.begin() + pos, P[num_of_job]);
	P.erase(P.begin() + 1 + num_of_job);
}
/*przywracanie stanu wczesniejszej permutacji po sprawdzeniu C_max*/
void restore(vector<vector <int>> & C, vector <vector <int>> & P, int pos, int num_of_job)
{
	C.erase(C.begin() + pos);
	P.insert(P.begin() + num_of_job + 1, P[pos]);
	P.erase(P.begin() + pos);
}

/*obliczanie C_max dla danej permutacji*/
int find_c_max(vector<vector <int>> & C, vector <vector <int>> & P, int n, int m)
{
	int j, k;
	for ( j = 1; j <= n; ++j)
	{
		for ( k = 1; k <= m; ++k)
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

int main()
{
	string files[] = { "NEH1.DAT", "NEH2.DAT" , "NEH3.DAT" , "NEH4.DAT" , "NEH5.DAT" , "NEH6.DAT" , "NEH7.DAT" , "NEH8.DAT" , "NEH9.DAT" };
	fstream file;
	int n, m, p, c_max, min_c_max, min_j, min_k;
	vector <vector<int>> P, C;
	
	for (int in = 0; in < 9; ++in)
	{	
		file.open(files[in], ios::in);
		if (file.good())
		{
			//pobieramy dane o ilosci zadan i maszyn
			file >> n >> m;
			/*ustalanie rozmiaru: o 1 tablica 2d wiersz o kolumna o 1 wieksza od ilosci danych aby w elemencie zerowym bylo 0.
			Jest to potrzebne by zachowaæ warunki brzegowe. W funkcji find_max_c odwolujemy siê do elementu poprzedniego tablicy
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
			sort(P.begin() + 1, P.end(), sorting);

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
				c_max = 9999999999;
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
			cout << "Wynik dla danych z pliku \"" << files[in] << "\": " << c_max << endl;
			file.close();
		}
		else
		{
			cout << "Nie mozna otworzyc pliku \"" << files[in] << "\"" << endl;
			system("pause");
			return -1;
		}
	
	}

	system("pause"); 
	return 0;
}


