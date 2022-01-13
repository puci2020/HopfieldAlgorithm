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
            try
            {
                Badanie.Uruchom();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Wystąpił wyjątek {0}",ex.Message);
            }

           
        }
    }
}
