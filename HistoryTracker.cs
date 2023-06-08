using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABFormatter
{
    static public class HistoryTracker
    {
        static string RegistryKey = "HKEY_CURRENT_USER\\Software\\ABFormatter\\ABFormatterSettings";
        static string ValueName = "AutoSave";
        public static bool IsTrackingOn()
        {
            int autoSave = 0;
            try
            {
                autoSave = (int)Registry.GetValue(RegistryKey, ValueName, 0);

            }
            catch
            { }
            return autoSave != 0;
        }
        static public void SetTracking(bool val)
        {
            int value = val ? 1 : 0;
            try
            {
                Registry.SetValue(RegistryKey, ValueName, value);
            }
            catch
            { }
        }
        static public void Track(string msg)
        {
            // Specify the Registry key and value names
            string historyFileName = string.Empty;
            // Read the flag value from the Registry
            if (!IsTrackingOn())
            {
                return;
            }

            try
            {
                string executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string executableFolder = Path.GetDirectoryName(executablePath);
                historyFileName = executableFolder + "\\workHistory.txt";
            }
            catch { Debug.Assert(false); return; }
            if (historyFileName == "\\workHistory.txt")
            {
                Debug.Assert(false);
                return;
            }
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
