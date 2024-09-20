using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public class Controller
    {
        public void Execute()
        {

        }

        private int GetFibonacciTerm(int termNumber)
        {
            int a0 = 0;
            int a1 = 1;
            int a2 = a0 + a1;

            if (termNumber <= 1) return a0;
            if (termNumber == 2) return a1;

            for (int i = 3; i <= termNumber; i++)
            {
                a2 = a1 + a0;

                a0 = a1;
                a1 = a2;
            }

            return a2;
        }

        #region OptionHandlers
        private int ChooseNumberOption(int maxOptionNumber)
        {
            bool validOption = false;
            int option = 0;

            while (!validOption)
            {
                try
                {
                    int desiredOption = int.Parse(Console.ReadLine());

                    if (desiredOption > 0 && desiredOption <= maxOptionNumber)
                    {
                        validOption = true;
                        option = desiredOption;
                    }
                    else
                    {
                        Console.WriteLine($"Selecciona una opcion válida:");
                    }
                }
                catch
                {
                    Console.WriteLine($"Selecciona una opcion válida:");
                }
            }

            return option;
        }
        private bool ChooseYNOption()
        {
            Console.WriteLine("(Y/N):");

            bool desiredOption = false;
            bool validYN = false;

            while (!validYN)
            {
                string answer = Console.ReadLine();

                switch (answer)
                {
                    case "Y":
                    case "y":
                        validYN = true;
                        desiredOption = true;
                        break;
                    case "N":
                    case "n":
                        validYN = true;
                        desiredOption = false;
                        break;
                    default:
                        Console.WriteLine("\nEscoge una opción válida:");
                        break;
                }
            }

            return desiredOption;
        }
        #endregion
    }
}
