using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RomLibrary
{
    public class Utility
    {

        byte[] _data;

        public void LoadRom(string fileNamePath)
        {
            string path = Path.GetDirectoryName(fileNamePath);
            string fileNmae = Path.GetFileName(fileNamePath) + ".rom";
            if (path.Length == 0)
            {
                path = ".";
            }
            fileNamePath = Path.Combine(path, fileNmae);

            BinaryReader binaryReader = new BinaryReader(File.Open(fileNamePath, FileMode.Open, FileAccess.Read));
            long length = binaryReader.BaseStream.Length;
            binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);

            _data = new byte[length];
            _data = binaryReader.ReadBytes((int)length);
            binaryReader.Close();

        }



        public void OutputDeclaration(string fileNamePath)
        {
            string path = Path.GetDirectoryName(fileNamePath);
            string fileNmae = Path.GetFileName(fileNamePath) + ".txt";
            if (path.Length == 0)
            {
                path = ".";
            }
            fileNamePath = Path.Combine(path, fileNmae);

            if (File.Exists(fileNamePath))
            {
                File.Delete(fileNamePath);
            }

            StreamWriter sw = new StreamWriter(fileNamePath);
            sw.WriteLine(@"public byte[] pData = new byte[] {");
            sw.Write("\t");
            int j = 0;
            for (int i = 0; i < _data.Length; i++)
            {
                sw.Write(string.Format("0x{0:x2}", _data[i]));
                sw.Write(", ");
                j++;
                if (j == 16)
                {
                    j = 0;
                    sw.WriteLine("");
                    sw.Write("\t");
                }
            }
            sw.WriteLine(@"};");
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
