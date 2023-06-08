using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace ABFormatter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextToTranslate.TextChanged += TextToTranslate_TextChanged;
        }
        string RemoveAllInvalidSufixes(string str, char[] sufixes)
        {
            while (str.Length > 0)
            {
                bool bAnySufixFound = false;
                foreach(var suf in sufixes)
                {
                    if (str[str.Length - 1] == suf)
                    {
                        bAnySufixFound = true;
                        break;
                    }
                }

                if(!bAnySufixFound)
                {
                    break;
                }

                try
                {
                    str = str.Remove(str.Length - 1);

                }
                catch
                {
                    break;
                }
            }
            return str;
        }
        string FormatText()
        {
            string formattedText = string.Empty;
            string cleanedText = string.Empty;
            var sufixesToAvoid = new char[] { ' ', '_' };
            try
            {
                cleanedText = Regex.Replace(TextToTranslate.Text, @"\s+", " ");
            }
            catch
            {
            }
            string activityNumber = string.Empty;
            bool startNumbering = false;
            int lastIndex = 0;
            
            for(int i=0; i < cleanedText.Length;i++)
            {
                var ch = cleanedText[i];
                if (ch == '#')
                {
                    startNumbering = true;
                    continue;
                }
                if(startNumbering)
                {
                    if(char.IsAsciiDigit(ch))
                    {
                        activityNumber += ch;
                    }
                    else if(activityNumber.Length > 0)
                    {
                        lastIndex = i+1;
                        break;
                    }
                }
            }
            if(activityNumber.Length > 0)
            {
                formattedText += ("activity/" + activityNumber + '/');
            }
            string originalDescription = string.Empty;
            for (int i = lastIndex;  i < cleanedText.Length; i++)
            {
                if (lastIndex == 0 && !string.IsNullOrEmpty(formattedText))
                        break;
                var ch = cleanedText[i];
                if (char.IsAsciiLetterOrDigit(ch))
                {
                    formattedText += char.ToLower(ch);
                    originalDescription += ch;
                }
                else
                {
                    formattedText += '_';
                    originalDescription += ch;

                }
            }
            string secondFormmattedString;
            try
            {
                secondFormmattedString = Regex.Replace(formattedText, @"_+", "_");
            }
            catch
            {
                return formattedText;
                
            }
            string[] prParams = secondFormmattedString.Split('/');
            if (prParams?.Length == 3)
            {
                string activityNr = prParams[1];
                string prTemplate = string.Empty;
                try
                {
                    prTemplate = string.Format("Activity({0}), {1}", activityNr, originalDescription);
                }
                catch
                {
                }
                PRName.Text = RemoveAllInvalidSufixes(prTemplate, sufixesToAvoid);
            }
            secondFormmattedString = RemoveAllInvalidSufixes(secondFormmattedString, sufixesToAvoid);
            return secondFormmattedString;

        }

        private void TextToTranslate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextToTranslate.Text))
                return;
            TranslatedText.Text = FormatText();
        }
        void SaveFile(string filePath)
        {
            try
            {
                // Create a new text file and open it for writing
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write some text to the file
                    if (!string.IsNullOrEmpty(TranslatedText.Text))
                        writer.WriteLine(TranslatedText.Text);

                    if (!string.IsNullOrEmpty(PRName.Text))
                        writer.WriteLine(PRName.Text);
                }
            }
            catch
            {
            }
        }
        private void OnSaveCLick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PRName.Text) && string.IsNullOrEmpty(TranslatedText.Text))
            {
                MessageBox.Show("you didn't provided proper activity name to save it !");
            }
            // Create a new SaveFileDialog instance
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set the properties of the SaveFileDialog
            saveFileDialog.Filter = "Text Files|*.txt";
            saveFileDialog.Title = "Create Text File";

            // Show the SaveFileDialog and wait for the user's response
            var result = saveFileDialog.ShowDialog();

            // Process the user's response
            if (result == true && !string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                SaveFile(saveFileDialog.FileName);
                MessageBox.Show("File Saved");
            }
        }
    }
}
