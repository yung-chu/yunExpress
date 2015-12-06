using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Client.UpdateOutStoragePrice
{
    public class LogTime
    {
        private static readonly object _sync = new object();
        public static string ReadFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string str = File.ReadAllText(filePath);
                    return str;
                }
                return "";
            }
            catch (Exception ee)
            {
                return "";
            }

        }
        public static void WriteFile(string path, string fileContent)
        {
            lock (_sync)
            {
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine(fileContent);
                        sw.Close();
                    }
                }
                else
                {
                    File.WriteAllText(path, fileContent);
                }
            }
        }
    }
}
