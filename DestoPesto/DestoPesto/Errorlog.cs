using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DestoPesto
{
 
    public class Errorlog
    {




        static Errorlog _Current = new Errorlog();
        public static Errorlog Current
        {
            get
            {
                return _Current;
            }
        }

     

        List<string> CachedLines = new List<string>();


        public void Log(List<string> lines)
        {
            foreach (var line in lines.ToList())
            {
                var index = lines.IndexOf(line);
                lines[index]=DateTime.Now.ToString()+" : "+line;
            }
            lock (this)
            {

                CachedLines.AddRange(lines);
                int count = 5;
                //do
                //{
                try
                {

                    const string errorFileName = "Common.log";
                    var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                    var errorFilePath = Path.Combine(libraryPath, errorFileName);
                    File.AppendAllLines(errorFilePath, CachedLines);

                    var liness = File.ReadAllLines(errorFilePath);
                    CachedLines.Clear();
                    return;
                }
                catch (Exception error)
                {
                    // System.Threading.Thread.Sleep(200);

                }
                //    count--;
                //} while (count>0);     
            }
        }

        public string ReadLog()
        {
            lock (this)
            {
                const string errorFileName = "Common.log";
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                if (File.Exists(errorFilePath))
                    return File.ReadAllText(errorFilePath);
                else
                    return "";
            }
        }

        public void ClearLog()
        {
            lock (this)
            {
                const string errorFileName = "Common.log";
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                File.Delete(errorFilePath);
            }
        }

    }
}
