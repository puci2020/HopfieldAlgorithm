using DMU.Math;
using System;
using System.IO;

namespace DMU.Terminal {
  internal class Badanie {
    internal static void Uruchom(bool rand = false, int m = 1) {

      StreamWriter file;
      double[] matrixData = {};
      int dimension = 3;

      if (rand) {
        file = new StreamWriter("RaportRandom.txt");
      } else {
        file = new StreamWriter("Raport1.txt");
        Console.WriteLine("Podaj elementy macierzy oddzielając je znakiem spacji. Liczba elementów musi być równa {0}", dimension * dimension);
        string[] array = Console.ReadLine().Split(' ');
        matrixData = Array.ConvertAll(array, el => double.Parse(el));
        if (matrixData.Length != dimension * dimension) {
          Console.WriteLine("Błędnie podana macierz. Uruchom prorgam jeszcze raz");
          Console.ReadLine();
        }
      }

      //Console.WriteLine("Podaj wymiar macierzy. Jeśli macierz 3x3 wpisz 3 itd.");
      //int dimension = Convert.ToInt32(Console.ReadLine());

      //string [] array = {"0", "-1", "-3", "-1", "0", "2", "-3", "2", "0"};
      //string [] array = {"0", "-1", "3", "-1", "0", "2", "3", "2", "0"};
      //string [] array = {"0", "-6", "6", "-6", "0", "4", "6", "4", "0"};
      //string [] array = {"0", "1", "2", "1", "0", "-1", "2", "-1", "0"};

      double getEnergySync(Matrix matrix, Matrix vn, Matrix vn_1) {
        double energy = 0.0;

        for (int i = 0; i < dimension; i++) {
          for (int j = 0; j < dimension; j++) {
            energy += matrix.GetElement(i, j) * vn.GetElement(0, i) * vn_1.GetElement(0, j);
          }
        }
        return -(energy);
      }

      void runHopfieldSync(double[] data) {
       
        double [,] vectors_I = new double[,] { {-1,-1,-1 }, { -1,-1,1 }, {-1,1,-1}, { -1,1,1}, { 1,-1,-1}, { 1,-1,1}, { 1,1,-1}, { 1,1,1} };

        Matrix I = new Matrix(new double[] {
          0.5,
          0.5,
          0.5
        }, true);

        file.WriteLine("###################### TRYB SYNCHRONICZNY ######################");
        Console.WriteLine("###################### TRYB SYNCHRONICZNY ######################");

        for (int i = 0; i < vectors_I.GetLength(0); i++) {
          file.WriteLine("------------------------ Wektor numer {0} ------------------------", i + 1);
          Console.WriteLine("------------------------ Wektor numer {0} ------------------------", i + 1);
          Matrix V0 = new Matrix(new double[] {
            vectors_I[i, 0], vectors_I[i, 1], vectors_I[i, 2]
          }, true);

          Matrix W = new Matrix(data, dimension, dimension);
          file.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          Console.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          file.WriteLine("Badany wektor:\n {0}", V0.ToString("F1", "\n", " "));
          Console.WriteLine("Macierz wag:\n {0}", W.ToString("F1", "\t", "\n"));
          file.WriteLine("Macierz wag:\n {0}", W.ToString("F1", "\t", "\n"));

          bool repeat = true;
          int index = 1;

          double energy_V, energy_V_1;
          energy_V_1 = 0;
          Matrix Un;
          Matrix Vn_1, Vn;
          Vn_1 = V0;

          do {
            Console.WriteLine("\nKrok {0}", index);
            file.WriteLine("\nKrok {0}", index);
            Un = Matrix.Multiply(W, Vn_1);
            Un = Matrix.Add(Un, I);
            Console.WriteLine("Potencjał wejściowy:\n{0}", Un.ToString("F1", "\n", " "));
            file.WriteLine("Potencjał wejściowy:\n{0}", Un.ToString("F1", "\n", " "));
            Vn = Un.ToBiPolar();
            Console.WriteLine("Potencjał wyjściowy:\n{0}", Vn.ToString("F1", "\n", " "));
            file.WriteLine("Potencjał wyjściowy:\n{0}", Vn.ToString("F1", "\n", " "));
            energy_V = getEnergySync(W, Vn, Vn_1);

            Console.WriteLine("\nEnergia({0}) = {1}", index, energy_V);
            file.WriteLine("\nEnergia({0}) = {1}", index, energy_V);

            if (index == 1) {
              if (Vn_1.Equals(Vn)) {
                Console.WriteLine("Wniosek: Punkt [{0}] jest stały!", Vn.ToString("F1", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] jest stały!", Vn.ToString("F1", "\n", " "));
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
            } else if (index <= 2) {
              energy_V_1 = getEnergySync(W, Vn, Vn_1);
              if (!Vn_1.Equals(Vn) && Vn.Equals(V0) && energy_V == energy_V_1) {
                Console.WriteLine("Wniosek: Punkt [{0}] tworzy cykl [{1}] <-> [{2}]", V0.ToString("F1", "\n", " "), Vn.ToString("F1", "\n", " "), Vn_1.ToString("F1", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] tworzy w cykl [{1}] <-> [{2}]", V0.ToString("F1", "\n", " "), Vn.ToString("F1", "\n", " "), Vn_1.ToString("F1", "\n", " "));
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
              if (Vn_1.Equals(Vn)) {
                Console.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F1", "\n", " "), Vn.ToString("F1", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F1", "\n", " "), Vn.ToString("F1", "\n", " "));
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
            } else {
              energy_V_1 = getEnergySync(W, Vn, Vn_1);
              if (!Vn_1.Equals(Vn) && energy_V == energy_V_1) {
                Console.WriteLine("Wniosek: Punkt [{0}] wpada cykl [{1}] <-> [{2}]", V0.ToString("F1", "\n", " "), Vn.ToString("F1", "\n", " "), Vn_1.ToString("F1", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] wpada w cykl [{1}] <-> [{2}]", V0.ToString("F1", "\n", " "), Vn.ToString("F1", "\n", " "), Vn_1.ToString("F1", "\n", " "));
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
              if (Vn_1.Equals(Vn)) {
                Console.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F1", "\n", " "), Vn.ToString("F1", "\n", " "));
                file.WriteLine("Wniosek: Punkt [{0}] jest zbieżny do punktu [{1}]", V0.ToString("F1", "\n", " "), Vn.ToString("F1", "\n", " "));
                repeat = false;
                Console.WriteLine("\n----- Koniec badania! -----");
                file.WriteLine("\n----- Koniec badania! -----");
              }
            }
           

            index++;

            if (index == System.Math.Pow(2, dimension) + 1) {
              repeat = false;
              Console.WriteLine("\n----- Koniec badania! -----");
              file.WriteLine("\n----- Koniec badania! -----");

            }

            Vn_1 = Vn;

          } while (repeat);

        }
      }
      if (rand) {
        Random rnd = new Random();

        double[] randMatrixData = new double[9];
        for (int i = 0; i < m; i++) {
          for (int j = 0; j < randMatrixData.Length; j++) {
            //randMatrixData.SetValue(rnd.Next(-99, 99), j);
            randMatrixData[j] = rnd.Next(-99, 99);

          }

          Console.WriteLine("-------------------------------------------------------------------");
          file.WriteLine("-------------------------------------------------------------------");
          Console.WriteLine("***************** Wygenerowana macierz numer {0} *****************", i + 1);
          file.WriteLine("***************** Wygenerowana macierz numer {0} *****************", i + 1);
          Console.WriteLine("-------------------------------------------------------------------");
          file.WriteLine("-------------------------------------------------------------------");
          runHopfieldSync(randMatrixData);

        }

      } else {
        runHopfieldSync(matrixData);
      }
      Console.WriteLine("Wciśnij Enter, aby zapisać wyniki do pliku");
      Console.ReadLine();
      file.Close();
    }
  }
}