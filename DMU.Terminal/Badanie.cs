using DMU.Math;
using System;
using System.IO;

namespace DMU.Terminal {
  internal class Badanie {
    internal static void Uruchom(string mode, int m = 1) {

      StreamWriter file = null;
      double[] matrixData = {};
      double[,] summary = new double[8, 4]{{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0}};        //tablica z podsumowaniem dla losowych macierzy (1.el - stały, 2.el - zbieżny, 3.el - tworzy cykl, 4.el - wpada w cykl)
      double [] exampleData = {0, 1 ,2, 1, 0, 3, 2, 3, 0 };     //przykładowe wagi macierzy wag
      Matrix example = new Matrix(exampleData, 3, 3);       //przykładowa macierz wag wyświetlana w konsoli aby użytkownik wiedział jak wprowadzić dane
      int dimension = 3;    //zakładamy 3-wymiarową macierz
      string randFileName = "./RaportRandom-" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".txt";
      string syncAsyncFileName = "./RaportSyncAsync-" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".txt";

      if (mode == "rand") {
        file = new StreamWriter(randFileName);
      }
      if(mode == "sync") {
        file = new StreamWriter(syncAsyncFileName, append: true);
        Console.WriteLine("Podaj elementy macierzy symetrycznej oddzielając je znakiem spacji. Liczba elementów musi być równa {0}", dimension * dimension);
        Console.WriteLine("\nWprowadź dane według wzoru:");
        Console.WriteLine("Macierz symetryczna: \n {0}", example.ToString("F0", "\t", "\n"));
        Console.WriteLine("Należy wprowadzić: 0 1 2 1 0 3 2 3 0");
        string[] array = Console.ReadLine().Split(' ');     //podanie elementów macierzy wag przez użytkownika
        matrixData = Array.ConvertAll(array, el => double.Parse(el));   //rzutowanie podanych elementów na double
        if (matrixData.Length != dimension * dimension) {       //jeśli liczba elementów nie jest równa 9 należy uruchomić program jeszcze raz
          Console.WriteLine("Błędnie podana macierz. Uruchom prorgam jeszcze raz");
          Console.ReadLine();
        }
      }
     
      //oblicznie energii kroku dla trybu synchronicznego
      double getEnergySync(Matrix matrix, Matrix vn, Matrix vn_1, Matrix I) {
        double energy = 0.0;
        double pom = 0.0;

        for (int i = 0; i < dimension; i++) {
          for (int j = 0; j < dimension; j++) {
            energy += matrix.GetElement(i, j) * vn.GetElement(0, i) * vn_1.GetElement(0, j);
          }
          pom +=  I.GetElement(0,i) * (vn.GetElement(0, i) + vn_1.GetElement(0, i));
        }
        return -energy+pom;
      }

       //obliczanie energii dla trybu asynchronicznego
     double getEnergyAsync(Matrix matrix, Matrix vn)
    {
        double energy = 0.0;
                
        for(int i = 0; i < dimension;i++) {
            for(int j = 0; j < dimension;j++) {
                energy += matrix.GetElement(i, j) * vn.GetElement(0, i) * vn.GetElement(0, j);
            }
        }
        return (-0.5) * energy;
    }
    //wektory wejściowe
    double [,] vectors_I = new double[,] { {-1,-1,-1 }, { -1,-1,1 }, {-1,1,-1}, { -1,1,1}, { 1,-1,-1}, { 1,-1,1}, { 1,1,-1}, { 1,1,1} };
      
    //algorytm dla trybu synchronicznego
    void runHopfieldSync(double[] data) {

    //wektor zaszumienia
      Matrix I = new Matrix(new double[] {
          0.5,
          0.5,
          0.5
      }, true);

        file.WriteLine("###################### TRYB SYNCHRONICZNY ######################");
        Console.WriteLine("###################### TRYB SYNCHRONICZNY ######################");

        //pobieranie każdego wektora wejściowego
        for (int i = 0; i < vectors_I.GetLength(0); i++) {
          file.WriteLine("------------------------ Badanie numer {0} ------------------------", i + 1);
          Console.WriteLine("------------------------ Badanie numer {0} ------------------------", i + 1);
          //wektor wejściowy
          Matrix V0 = new Matrix(new double[] {
            vectors_I[i, 0], vectors_I[i, 1], vectors_I[i, 2]
          }, true);
       
          //macierz wag tworzona na podstawie tablicy elelmentów podanych przez użytkownika
          Matrix W = new Matrix(data, dimension, dimension);
          file.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          Console.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          file.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          Console.WriteLine("Macierz wag:\n {0}", W.ToString("F1", "\t", "\n"));
          file.WriteLine("Macierz wag:\n {0}", W.ToString("F1", "\t", "\n"));

          bool repeat = true;   //zmienna mówiąca czy nastąpiło koniec badania
          int index = 1;    //numer badanego kroku

          double energy_V, energy_V_1;  //wartość energii obecnego potencjału wyjściowego oraz z poprzedniego kroku
          energy_V_1 = 0;
          Matrix Un;    //macierz po przmnożeniu macierzy wag przez wektor wejściowy
          Matrix Vn_1, Vn;  //potencjał wyjściowy obecnego kroku oraz poprzedniego
          Vn_1 = V0;    //początkowo potencjał wyściowy kroku poprzedniego jest równy wektorowi wejściowemu
           
        //wykonywanie kolejnych kroków w danym badaniu
          do {
            Console.WriteLine("\nKrok {0}", index);
            file.WriteLine("\nKrok {0}", index);

            Un = Matrix.Multiply(W, Vn_1);      //mnożenie macierzy wag przez potencjał wejściowy
            Un = Matrix.Add(Un, I);     //dodanie wektora zaszumienia w celu usunięcia wag o wartości 0
            Console.WriteLine("Potencjał wejściowy:\n{0}", Un.ToString("F1", "\n", " "));
            file.WriteLine("Potencjał wejściowy:\n{0}", Un.ToString("F1", "\n", " "));

            Vn = Un.ToBiPolar();    //wyznaczenie potencjału wyjściowego za pomocą bipolarnej funkcji aktywacji
            Console.WriteLine("Potencjał wyjściowy:\n{0}", Vn.ToString("F1", "\n", " "));
            file.WriteLine("Potencjał wyjściowy:\n{0}", Vn.ToString("F1", "\n", " "));
            
            energy_V = getEnergySync(W, Vn, Vn_1, I);   //obliczenie energii dla obecnego kroku
            Console.WriteLine("\nEnergia({0}) = {1}", index, energy_V);
            file.WriteLine("\nEnergia({0}) = {1}", index, energy_V);

            if (index == 1) {   //warunki stopu dla kroku pierwszego
              if (Vn_1.Equals(Vn)) {    //sprawdzenie czy potencjał wyjściowy równa się potencjałowi wyjściowemu z poprzedniego kroku(dla kroku 1 jest to wektor wejściowy)
                Console.WriteLine("Wniosek: Punkt [{0}] jest stały!", Vn.ToString("F0", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] jest stały!", Vn.ToString("F0", "\n", " "));
                summary[i, 0]++;    //zwiększenie wartości w tablicy podsumowującej (wykorzytywane przy generowaniu macierzy losowych)
                repeat = false;     //jeśli warunek spełniony zatrzymanie pętli i przejście do kolejnego badania
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
            } else if (index <= 2) {    //warunki stopu dla kroku drugiego
              energy_V_1 = getEnergySync(W, Vn, Vn_1, I);   //przypisanie energii kroku poprzeniego do zmiennej
              if (!Vn_1.Equals(Vn) && Vn.Equals(V0) && energy_V == energy_V_1) {    //jeśli potencjał wyjściowy z poprzedniego kroku jest różny od obecnego oraz obecny potencjał wyjściowy jset równy wektorowi wejściowemu oraz obecna energia jest równa energii kroku poprzedniego
                Console.WriteLine("Wniosek: Punkt [{0}] tworzy cykl [{1}] <-> [{2}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "), Vn_1.ToString("F0", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] tworzy w cykl [{1}] <-> [{2}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "), Vn_1.ToString("F0", "\n", " "));
                summary[i, 2]++;
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
              if (Vn_1.Equals(Vn)) {    //jeśli potencjał wyjściowy z poprzedniego kroku jest równy obecnemu
                Console.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "));
                summary[i, 1]++;
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
            } else {    //warunki stopu dla kolejnych kroków czyli krok 3, 4, 5...
              energy_V_1 = getEnergySync(W, Vn, Vn_1, I);       //przypisanie energii kroku poprzeniego do zmiennej
              if (!Vn_1.Equals(Vn) && energy_V == energy_V_1) {     //jeśli potencjał wyjściowy z poprzedniego kroku jest różny od obecnego oraz obecna energia jest równa energii kroku poprzedniego
                Console.WriteLine("Wniosek: Punkt [{0}] wpada cykl [{1}] <-> [{2}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "), Vn_1.ToString("F0", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] wpada w cykl [{1}] <-> [{2}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "), Vn_1.ToString("F0", "\n", " "));
                summary[i, 3]++;
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
              if (Vn_1.Equals(Vn)) { //jeśli potencjał wyjściowy z poprzedniego kroku jest równy obecnemu
                Console.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "));
                summary[i, 1]++;
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
            }

            index++;        //inkrementacja kroku

            //jeśli nie nastąpił żaden warunek stopu badanie zatrzyma się na 8 krokach
            if (index == System.Math.Pow(2, dimension) + 1) {
              repeat = false;
              Console.WriteLine("\n----- Koniec badania! -----");
              file.WriteLine("\n----- Koniec badania! -----");
            }
            Vn_1 = Vn;      //przypisanie potencjału wyjściowego jako dla kroku poprzedniego
          } while (repeat);
        }       
      }

    //algorytm dla trybu asynchronicznego
    void runHopfieldAsync(double[] data, int[,] sequences, int sequence) {
        
        file.WriteLine("###################### TRYB ASYNCHRONICZNY ######################");
        Console.WriteLine("###################### TRYB ASYNCHRONICZNY ######################");
        //Wyświetlenie wybranej sekwencji
        Console.WriteLine("\nWybrana sekwencja - n{0}, n{1}, n{2}\n", sequences[sequence-1,0], sequences[sequence-1,1], sequences[sequence-1,2]);
        file.WriteLine("\nWybrana sekwencja - n{0}, n{1}, n{2}\n", sequences[sequence-1,0], sequences[sequence-1,1], sequences[sequence-1,2]);
        
        //pobieranie każdego wektora wejściowego
        for (int i = 0; i < vectors_I.GetLength(0); i++) {
          file.WriteLine("------------------------ Badanie numer {0} ------------------------", i + 1);
          Console.WriteLine("------------------------ Badanie numer {0} ------------------------", i + 1);
         //wektor wejściowy 
         Matrix V0 = new Matrix(new double[] {
            vectors_I[i, 0], vectors_I[i, 1], vectors_I[i, 2]
          }, true);
        
          //macierz wag tworzona na podstawie tablicy elelmentów podanych przez użytkownika
          Matrix W = new Matrix(data, dimension, dimension);
          file.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          Console.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          file.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          Console.WriteLine("Macierz wag:\n {0}", W.ToString("F1", "\t", "\n"));
          file.WriteLine("Macierz wag:\n {0}", W.ToString("F1", "\t", "\n"));

          bool repeat = true;       //zmienna mówiąca czy nastąpiło koniec badania
          int index = 1;    //numer badanego kroku
          int index2 = 1;   //każda sekwencja ma 3 kroki i ten index wskazuje który krok w sekwencji się wykonuje
          
          double energy_V, energy_V_1;      //wartość energii obecnego potencjału wyjściowego oraz z poprzedniego kroku

          Matrix Un;        //macierz po przmnożeniu macierzy wag przez wektor wejściowy
          Matrix Vn_3, Vn_2, Vn_1;  //potencjały wyjściowe trzech poprzednich kroków 
          Matrix Vn = null;     //potencjał wyjściowy obecnego kroku

          Vn_1 = V0;    //przypisanie jako potencjał poprzedniego kroku wartości wektora wejściowego
          Vn_2 = null;  //na początku nie ma wartości potencjałów wyjściowych dla kroków poprzednich
          Vn_3 = null;

          //wykonywanie kolejnych kroków w danym badaniu
          do {
            Console.WriteLine("\nKrok {0}", index);
            file.WriteLine("\nKrok {0}", index);
            Un = Matrix.Multiply(W, Vn_1);  //mnożenie macierzy wag przez potencjał wyjściowy z poprzedniego kroku

            if(index2 == 1)     //pierwszy krok sekwencji
            {
                    if(sequences[sequence-1,0] == 1)        //jeśli pierwszy element wybranej sekwencji równa się 1
                    {
                        Console.WriteLine("Potencjał wejściowy:\n{0}   NW   NW", Un.GetElement(0, sequences[sequence-1,0]-1));
                        file.WriteLine("Potencjał wejściowy:\n{0}   NW   NW", Un.GetElement(0, sequences[sequence-1,0]-1));
                        Vn = new Matrix(new double[] { Un.GetElement(0, sequences[sequence-1,0]-1), Vn_1.GetElement(0, 1), Vn_1.GetElement(0, 2) }, true);      //potencjał wyjściowy tworzymy na podstawie pierwszego elementu po przemnożeniu macierzy wag przez potencjał wejsciowy a pozostałe elemeny przepisujemy z potencjału wejściowego
                    } else if(sequences[sequence-1,0] == 2)     //jeśli pierwszy element wybranej sekwencji równa się 1
                    {
                        Console.WriteLine("Potencjał wejściowy:\nNW   {0}   NW", Un.GetElement(0, sequences[sequence-1,0]-1));
                        file.WriteLine("Potencjał wejściowy:\nNW   {0}   NW", Un.GetElement(0, sequences[sequence-1,0]-1));
                        Vn = new Matrix(new double[] { Vn_1.GetElement(0, 0), Un.GetElement(0, sequences[sequence-1,0]-1), Vn_1.GetElement(0, 2) }, true);      //potencjał wyjściowy tworzymy na podstawie drugiego elementu po przemnożeniu macierzy wag przez potencjał wejsciowy a pozostałe elemeny przepisujemy z potencjału wejściowego
                    }else if(sequences[sequence-1,0] == 3)      //jeśli pierwszy element wybranej sekwencji równa się 1
                    {
                        Console.WriteLine("Potencjał wejściowy:\nNW   NW   {0}", Un.GetElement(0, sequences[sequence-1,0]-1));
                        file.WriteLine("Potencjał wejściowy:\nNW   NW   {0}", Un.GetElement(0, sequences[sequence-1,0]-1));
                        Vn = new Matrix(new double[] { Vn_1.GetElement(0, 0), Vn_1.GetElement(0, 1), Un.GetElement(0, sequences[sequence-1,0]-1) }, true);      //potencjał wyjściowy tworzymy na podstawie trzeciego elementu po przemnożeniu macierzy wag przez potencjał wejsciowy a pozostałe elemeny przepisujemy z potencjału wejściowego
                    }
            }
            if(index2 == 2)     //drugi krok sekwencji
            {
                    if(sequences[sequence-1,1] == 1)    //jeśli drugi element wybranej sekwencji równa się 1
                    {
                        Console.WriteLine("Potencjał wejściowy:\n{0}   NW   NW", Un.GetElement(0, sequences[sequence-1,1]-1));
                        file.WriteLine("Potencjał wejściowy:\n{0}   NW   NW", Un.GetElement(0, sequences[sequence-1,1]-1));
                        Vn = new Matrix(new double[] { Un.GetElement(0, sequences[sequence-1,1]-1), Vn_1.GetElement(0, 1), Vn_1.GetElement(0, 2) }, true);
                    } else if(sequences[sequence-1,1] == 2)     //jeśli drugi element wybranej sekwencji równa się 2
                    {
                        Console.WriteLine("Potencjał wejściowy:\nNW   {0}   NW", Un.GetElement(0, sequences[sequence-1,1]-1));
                        file.WriteLine("Potencjał wejściowy:\nNW   {0}   NW", Un.GetElement(0, sequences[sequence-1,1]-1));
                        Vn = new Matrix(new double[] {Vn_1.GetElement(0, 0), Un.GetElement(0, sequences[sequence-1,1]-1), Vn_1.GetElement(0, 2) }, true);
                    }else if(sequences[sequence-1,1] == 3)      //jeśli drugi element wybranej sekwencji równa się 3
                    {
                        Console.WriteLine("Potencjał wejściowy:\nNW   NW   {0}", Un.GetElement(0, sequences[sequence-1,1]-1));
                        file.WriteLine("Potencjał wejściowy:\nNW   NW   {0}", Un.GetElement(0, sequences[sequence-1,1]-1));
                        Vn = new Matrix(new double[] { Vn_1.GetElement(0, 0), Vn_1.GetElement(0, 1), Un.GetElement(0, sequences[sequence-1,1]-1) }, true);
                    }
            }
            if(index2 == 3)     //trzeci krok sekwencji
            {
                    if(sequences[sequence-1,2] == 1)        //jeśli trzeci element wybranej sekwencji równa się 1
                    {
                        Console.WriteLine("Potencjał wejściowy:\n{0}   NW   NW", Un.GetElement(0, sequences[sequence-1,2]-1));
                        file.WriteLine("Potencjał wejściowy:\n{0}   NW   NW", Un.GetElement(0, sequences[sequence-1,2]-1));
                        Vn = new Matrix(new double[] { Un.GetElement(0, sequences[sequence-1,2]-1), Vn_1.GetElement(0, 1), Vn_1.GetElement(0, 2) }, true);
                    } else if(sequences[sequence-1,2] == 2)     //jeśli trzeci element wybranej sekwencji równa się 1
                    {
                        Console.WriteLine("Potencjał wejściowy:\nNW   {0}   NW", Un.GetElement(0, sequences[sequence-1,2]-1));
                        file.WriteLine("Potencjał wejściowy:\nNW   {0}   NW", Un.GetElement(0, sequences[sequence-1,2]-1));
                        Vn = new Matrix(new double[] {Vn_1.GetElement(0, 0), Un.GetElement(0, sequences[sequence-1,2]-1), Vn_1.GetElement(0, 2) }, true);
                    }else if(sequences[sequence-1,2] == 3)      //jeśli trzeci element wybranej sekwencji równa się 1
                    {
                        Console.WriteLine("Potencjał wejściowy:\nNW   NW   {0}", Un.GetElement(0, sequences[sequence-1,2]-1));
                        file.WriteLine("Potencjał wejściowy:\nNW   NW   {0}", Un.GetElement(0, sequences[sequence-1,2]-1));
                        Vn = new Matrix(new double[] { Vn_1.GetElement(0, 0), Vn_1.GetElement(0, 1), Un.GetElement(0, sequences[sequence-1,2]-1) }, true);
                    }
               
            }

            Vn = Vn.ToBiPolar();        //wyznaczenie potencjału wyjściowego za pomocą bipolarnej funkcji aktywacji
            Console.WriteLine("Potencjał wyjściowy:\n{0}", Vn.ToString("F1", "\n", " "));
            file.WriteLine("Potencjał wyjściowy:\n{0}", Vn.ToString("F1", "\n", " "));
            
            energy_V = getEnergyAsync(W, Vn);       //obliczenie energii dla trybu asynchroniczego w obecnym kroku

            Console.WriteLine("\nEnergia({0}) = {1}", index, energy_V);
            file.WriteLine("\nEnergia({0}) = {1}", index, energy_V);

            //warunki stopu
            if(index == 3)      //jeśli wykonujemy trzeci krok
            {
                if(Vn_2.Equals(Vn_1) && Vn_1.Equals(Vn) && Vn.Equals(V0) )      //sprawdzamy czy potencjały wyjściowe z 2 kroków są sobie równe oraz czy są równe wektorowi wejściowemu
                {
                    Console.WriteLine("Wniosek: Punkt [{0}] jest stały!", Vn.ToString("F0", "\n", " "));
                    file.WriteLine("Wniosek: Punkt [{0}] jest stały!", Vn.ToString("F0", "\n", " "));
                    repeat = false;
                    Console.WriteLine("\n----- Koniec badania! -----");
                    file.WriteLine("\n----- Koniec badania! -----");
                }
            }

            else if(index > 3)      //dla kroków 4, 5 ...
            {
                if(Vn_3.Equals(Vn_2) && Vn_2.Equals(Vn_1) && Vn_1.Equals(Vn))       //sprawdzamy czy potencjały wyjściowe z 3 poprzednich kroków są sobie równe
                {
                    Console.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "));
                    file.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F0", "\n", " "), Vn.ToString("F0", "\n", " "));
                    repeat = false;
                    Console.WriteLine("\n----- Koniec badania! -----");
                    file.WriteLine("\n----- Koniec badania! -----");
                }
            }

            index++;        //inkrementacja kroku

            if(index2 == 3)     //jeśli sekwencja dobiegła końca procedura się powtarz od początku jeśli nie następuje przejście do następnego kroku sekwencji
            {
                index2 = 1;
            }else
            {
                index2++;
            }
            
            //jeśli nie nastąpił żaden warunek stopu badanie zatrzyma się na 24 krokach
            if (index == (dimension * System.Math.Pow(2, dimension)) + 1) {
              repeat = false;
              Console.WriteLine("\n----- Koniec badania! -----");
              file.WriteLine("\n----- Koniec badania! -----");
            }
            if(index == 1)      //przypisania poprzdnich potencjałów wyjściowych do kolejnych zmiennych w zależności w którym kroku znajduje się algorytm 
            {
                Vn_1 = Vn;
            }
            if (index2 == 2)
            {
                Vn_2 = Vn_1;
                Vn_1 = Vn;
            }
            if (index >= 3)
            {
                Vn_3 = Vn_2;
                Vn_2 = Vn_1;
                Vn_1 = Vn;
            }
   
          } while (repeat);
        }       
      }
      //wywołanie algorytmu w trybie synchronicznych dla m macierzy losowych
      if (mode == "rand") {
        Random rnd = new Random();
        //wylosowanie wag dla podanej liczby macierzy
        for (int i = 0; i < m; i++) {
          double firstRand = rnd.Next(-99, 99);
          double secoundRand = rnd.Next(-99, 99);
          double thirdRand = rnd.Next(-99, 99);
          //dane dla macierzy symetrycznej wykorzystujące wylosowane wagi
          double [] randMatrixData = {0.0, firstRand, secoundRand, firstRand, 0.0, thirdRand, secoundRand, thirdRand, 0.0}; 

          Console.WriteLine("-------------------------------------------------------------------");
          file.WriteLine("-------------------------------------------------------------------");
          Console.WriteLine("***************** Wygenerowana macierz numer {0} *****************", i + 1);
          file.WriteLine("***************** Wygenerowana macierz numer {0} *****************", i + 1);
          Console.WriteLine("-------------------------------------------------------------------");
          file.WriteLine("-------------------------------------------------------------------");
          runHopfieldSync(randMatrixData);
        }
  
        //wypisanie podsumowania na koniec badania 
        Console.WriteLine("\n***********########### PODSUMOWANIE ###########***********");
        file.WriteLine("\n***********########### PODSUMOWANIE ###########***********");
        Console.WriteLine("Liczba losowań: {0} \n", m);
        file.WriteLine("Liczba losowań: {0} \n", m);
        Console.WriteLine("Wektor \t \t Stały \t Zbieżny \t Tworzy cykl \t Wpada w cykl");
        file.WriteLine("Wektor \t \t Stały \t Zbieżny \t Tworzy cykl \t Wpada w cykl");
        for(int k = 0; k < summary.GetLength(0); k++) {
            Console.Write("[{0, 2}, {1, 2}, {2, 2}] \t", vectors_I[k,0], vectors_I[k,1], vectors_I[k,2]);
            file.Write("[{0, 2}, {1, 2}, {2, 2}] \t", vectors_I[k,0], vectors_I[k,1], vectors_I[k,2]);
            Console.WriteLine("{0, 5} {1, 7} {2, 18} {3, 15}", summary[k,0], summary[k,1], summary[k,2], summary[k,3]);
            file.WriteLine("{0, 5} {1, 7} {2, 18} {3, 15}", summary[k,0], summary[k,1], summary[k,2], summary[k,3]);  
        }
      }
      //wywołanie algorytmu w trybie synchronicznym oraz asynchronicznym dla macierzy wag podanej przez użytkownika
      if (mode == "sync") {
        //dostępne sekwencje trybu asynchronicznego
        int [,] sequences = new int [6,3] { { 1,2,3}, { 1,3,2}, { 2,1,3}, { 2,3,1}, { 3,1,2}, { 3,2,1} };

        Console.WriteLine("Wybierz sekwencje trybu asynchronicznego:");
        //wypisanie w konsoli dostępnych sekwencji
        for (int i = 0; i< sequences.GetLength(0); i++)
                {
                    Console.Write("{0} - ", i+1);
                    for (int j = 0; j < sequences.GetLength(1); j++)
                    {
                        Console.Write(" n{0} ", sequences[i, j]);
                        if(j == 2)
                        {
                            Console.WriteLine();
                        }
                    }
                    
                }
        //użytkownik wybiera sekwencje
        int sequence = Convert.ToInt32(Console.ReadLine());
          
        //wywołanie trybu synchronicznego
        runHopfieldSync(matrixData);
        //wywołanie trybu asynchronicznego
        runHopfieldAsync(matrixData, sequences, sequence);
      }

      Console.WriteLine("Wciśnij Enter, aby zapisać wyniki do pliku");
      Console.ReadLine();
      file.Close();
    }
  }
}