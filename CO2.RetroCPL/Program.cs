using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CO2.RetroCPL.FrontEnd;
using CO2.RetroCPL.Commons;

namespace CO2.RetroCPL
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream file = null;
            //Scanner scn = null;
            //Parser parser = null;

            try
            {
                //file = new FileStream("GCD_Example.ss", FileMode.Open);
                //scn = new Scanner(file);
                //parser = new Parser(scn);
                //parser.Parse();
                //program = parser.program;

                //if (program != null)
                //{
                //    program.Execute();
                //    Console.WriteLine();
                //}
            }
            finally
            {
                file.Close();
            }
        }
    }
}