using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace StasConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] originalFileNames;
        List<int>[] readedBytesTable;
        int bottomLimit = 100;
        int topLimit = 130;
        
        public MainWindow()
        {
            InitializeComponent();

            bottomTextBox.Text = bottomLimit.ToString();
            topTextBox.Text = topLimit.ToString();
        }

        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SetLimits())
                return;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.ShowDialog();

            originalFileNames = dlg.SafeFileNames;
            readedBytesTable = new List<int>[dlg.FileNames.Length];

            int i = 0;
            foreach (string path in dlg.FileNames)
               readedBytesTable[i++] = ReadFromRobFile(path);

            foreach (List<int> readedBytes in readedBytesTable)
                ConvertData(readedBytes);
            
            saveInRobFile();
            
            textBox.Text += "Koniec konwersji.";
        }

        private bool SetLimits()
        {
            try
            {
                bottomLimit = int.Parse(bottomTextBox.Text);
            }
            catch
            {
                MessageBox.Show("W polu dolna granica podano nieprawidłową wartość!");
                return false;
            }
            try
            {
                topLimit = int.Parse(topTextBox.Text);
            }
            catch
            {
                MessageBox.Show("W polu górna granica podano nieprawidłową wartość!");
                return false;
            }
            return true;
        }

        private List<int> ReadFromRobFile(string path)
        {
            List<int> readedBytes = new List<int>();
                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                {
                    int value = 0;
                    while ((value = fileStream.ReadByte()) != -1)
                    {
                        readedBytes.Add(value);
                    }
                }
            return readedBytes;
        }
        
        private void saveInRobFile()
        {
            int i = 0;
            Directory.CreateDirectory("C:\\Robocze\\");

            foreach (List<int> readedBytes in readedBytesTable)
                using (FileStream fileStream = new FileStream("C:\\Robocze\\" + originalFileNames[i++], FileMode.Create))
                {
                    BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                    binaryWriter.Write(ConvertIntToByteArray(readedBytes));
                    binaryWriter.Close();
                }
        }
        
        private byte[] ConvertIntToByteArray(List<int> listInt)
        {
            byte[] bytetable = new byte[listInt.Count];
            for (int i = 0; i < listInt.Count; i++)
                bytetable[i] = (byte)listInt[i];
            return bytetable;
        }

        private void ConvertData(List<int> dataToConvert)
        {
            Random rand = new Random();
            for (int i = 5; i < dataToConvert.Count; i++)
                if (dataToConvert[i] > topLimit || dataToConvert[i] < bottomLimit)
                    dataToConvert[i] = rand.Next(bottomLimit, topLimit);
        }
    }

}
