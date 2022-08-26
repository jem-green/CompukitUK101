using System;
using System.IO;

namespace RomLibrary
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Utility u = new Utility();
            string fileName = "MONUK01";
            string filePath = @"C:\SOURCE\GIT\cs.net\CompukitUK101\UK101Library\Monitor";
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

            filePath = @"C:\SOURCE\GIT\cs.net\CompukitUK101\UK101Library\SysROMs";
            fileName = "BASICX";
            fileNamePath = Path.Combine(filePath, fileName);
            u.LoadRom(fileNamePath);
            u.OutputDeclaration(fileNamePath);

        

        }
    }
}
