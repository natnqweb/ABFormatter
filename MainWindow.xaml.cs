using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                PRName.Text = RemoveAllInvalidSufixes(prTemplate, new char[]{' ', '_'});
            }
            secondFormmattedString = RemoveAllInvalidSufixes(secondFormmattedString, new char[] { ' ', '_' });
            return secondFormmattedString;

        }

        private void TextToTranslate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextToTranslate.Text))
                return;
            TranslatedText.Text = FormatText();
        }
    }
}
