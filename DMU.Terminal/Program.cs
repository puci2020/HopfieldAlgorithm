using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMU.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wciśnij 1, aby uruchomić algorytm w trybie synchronicznym");
            Console.WriteLine("Wciśnij 2, aby uruchomić algorytm w trybie asynchronicznym");
            Console.WriteLine("Wciśnij 3, aby uruchomić algorytm dla m losowych macierzy 3x3");
            int mode = Convert.ToInt32(Console.ReadLine());

            try
            {   
                if (mode == 1)
                {
                    Badanie.Uruchom("sync");
                }
                if (mode == 2)
                {
                    Badanie.Uruchom("sync");
                }
                if (mode == 3)
	            {   
                    Console.WriteLine("Podaj liczbe macierzy do wygenerowania");
                    int m = Convert.ToInt32(Console.ReadLine());
                 
                        Badanie.Uruchom("rand" , m);
			
                    

	            }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Wystąpił wyjątek {0}",ex.Message);
            }

           
        }
    }
}
