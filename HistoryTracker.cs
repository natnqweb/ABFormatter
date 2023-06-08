using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABFormatter
{
    static public class HistoryTracker
    {
        public static string RegistryKey = "HKEY_CURRENT_USER\\Software\\ABFormatter\\ABFormatterSettings";
        public static string ValueName = "AutoSave";
        static public void Track(string msg)
        {
            // Specify the Registry key and value names
            string historyFileName = string.Empty;
            // Read the flag value from the Registry
            int autoSave = 0;
            try
            {
                autoSave = (int)Registry.GetValue(RegistryKey, ValueName, 0);

            }
            catch
            {}

            if (autoSave == 0)
            {
                return;
            }

            try
            {
                string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string executableFolder = Path.GetDirectoryName(executablePath);
                historyFileName = executableFolder + "\\workHistory.txt";
            }
            catch { return; }
            if (historyFileName == "\\workHistory.txt")
                return;

            string dateAndTime = DateTime.Now.ToString("dddd, MMMM d, yyyy h:mm tt");
            if (string.IsNullOrEmpty(msg))
                return;

            string msgOut = string.Empty;
            try
            {
                msgOut = string.Format("[{0}]\n\t{1}", dateAndTime, msg);
            }
            catch
            {
                return;
            }

            // Check if the file exists
            if (File.Exists(historyFileName))
            {
                // Append content to an existing file
                using (StreamWriter writer = File.AppendText(historyFileName))
                {
                    writer.WriteLine(msgOut);
                }
            }
            else
            {
                // Create a new file and add content
                using (StreamWriter writer = File.CreateText(historyFileName))
                {
                    writer.WriteLine(msgOut);
                }
            }
        }
    }

}
