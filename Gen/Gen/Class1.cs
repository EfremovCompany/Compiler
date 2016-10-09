using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gen
{
    class Class1
    {
        const int a = 0;
        const int b = 2;
        static void Mai()
        {
            int[] mas = { 8, 2, 4, 7, 3, 3, 9, 1, 2, 3 };
            for (int i = 0; i < 10; i++)
            {
                int f = 0;
                for (int j = 0; j < 10; j++)
                {
                    if (mas[j] > mas[j + 1])
                    {
                        int buf = mas[j];
                        mas[j] = mas[j + 1];
                        mas[j + 1] = mas[j];
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    Console.WriteLine(mas[j]);
                }
            }
        }
    }
}
