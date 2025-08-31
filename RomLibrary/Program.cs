using System;
using System.IO;

namespace RomLibrary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Utilities u = new Utilities();

            // Monitors

            string fileName = "MONUK01";
            string filePath = @"C:\SOURCE\GIT\cs.net\CompukitUK101\RomLibrary\Monitor";
            string fileNamePath = Path.Combine(filePath, fileName);
            u.LoadRom(fileNamePath);
            u.OutputDeclaration(fileNamePath);

            fileName = "MONUK02";
            fileNamePath = Path.Combine(filePath, fileName);
            u.LoadRom(fileNamePath);
            u.OutputDeclaration(fileNamePath);

            fileName = "WEMON";
            fileNamePath = Path.Combine(filePath, fileName);
            u.LoadRom(fileNamePath);
            u.OutputDeclaration(fileNamePath);

            // Chargen

            fileName = "CHGUK101";
            filePath = @"C:\SOURCE\GIT\cs.net\CompukitUK101\RomLibrary\Chargen";
            fileNamePath = Path.Combine(filePath, fileName);
            u.LoadRom(fileNamePath);
            u.OutputDeclaration(fileNamePath);


            // Basic

            filePath = @"C:\SOURCE\GIT\cs.net\CompukitUK101\RomLibrary\Basic";
            fileName = "BASICX";
            fileNamePath = Path.Combine(filePath, fileName);
            u.LoadRom(fileNamePath);
            u.OutputDeclaration(fileNamePath);

        

        }
    }
}
