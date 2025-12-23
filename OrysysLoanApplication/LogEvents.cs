using System.IO;

namespace OrysysLoanApplication
{
    public class LogEvents
    {
        public static void LogToFile(string title, string logMessage)
        {
            StreamWriter logSW;
            string fileName = "ExeceptionLog.txt";

            if (!File.Exists(fileName))
             {
                logSW = new StreamWriter(fileName);
                
            }
            else
            {
                logSW = File.AppendText(fileName);
            }

            logSW.WriteLine("********** {0} **********", title);
            logSW.WriteLine("{0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            logSW.WriteLine(logMessage);
            logSW.Close();
        }
    }
}
