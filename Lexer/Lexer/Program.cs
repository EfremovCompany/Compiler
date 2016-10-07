using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class Program
    {
        static int Main(string[] args)
        {
            using (StreamReader file = new StreamReader(@args[0]))
            {
                Reader reader = new Reader();
                int lineCounter = 1;
                while(!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    if (!reader.CheckString(line, lineCounter))
                    {
                        Console.WriteLine("LEXICAL ERROR: " + reader.GetNameOfError());
                        return -1;
                    }
                    ++lineCounter;
                }
            }
            Console.WriteLine("Success!");
            Console.ReadKey();
            return 0;
        }
    }
}
