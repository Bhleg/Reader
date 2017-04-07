using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ionic.Zip;
using System.IO;
using Ionic.Crc;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int CurrentPage = 1;
        
        Reader.Window1
       

        public MainWindow()
        {
           

        }

        Dictionary<int, BitmapImage> Pages = new Dictionary<int, BitmapImage>();


        void CbzLoader(string CbzPath)
        {

            int i = 1;

            using (ZipFile zip = ZipFile.Read(CbzPath))
            {

                foreach (ZipEntry e1 in zip)
                {
                    
                    CrcCalculatorStream reader = e1.OpenReader();
                    MemoryStream memstream = new MemoryStream();
                    reader.CopyTo(memstream);
                    byte[] bytes = memstream.ToArray();
                    memstream.Seek(0, SeekOrigin.Begin);

                    

                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.StreamSource = memstream;
                    b.CacheOption = BitmapCacheOption.OnLoad;
                    b.EndInit();
                    InitializeComponent();

                    Pages.Add(i, b);

                    i++;

                    //MainReader.Source = b;
       
                }
            }
        }
        

        void SinglePageViewer(int CurrentPage)
        {

            // Code à exécuter quand la méthode est appelée.
            //BitmapImage b = new BitmapImage();
            // b.BeginInit();
            //b.UriSource = new Uri(Page);
            //b.EndInit();
            ///InitializeComponent();
            // MainReader.Source = b;
            MainReader.Source = Pages[CurrentPage];
        }

        private void Open_File_Event(object sender, MouseButtonEventArgs e)
        {
            
            // SinglePageViewer("C:\\002.png");
            CbzLoader("c:\\test.cbz");
            CurrentPage = 1;
            SinglePageViewer(CurrentPage);

           
        }


        void DoublePageViewer(int CurrentPage)
        {
            MainReader.Source = Pages[CurrentPage];
        }

        private void PreviousPage(object sender, MouseButtonEventArgs e)
        {
            if(CurrentPage == 1)
            {
                MessageBox.Show("First Page !");
            }

            CurrentPage--;
            SinglePageViewer(CurrentPage);
        }

        private void NextPage(object sender, MouseButtonEventArgs e)
        {
            CurrentPage++;
            SinglePageViewer(CurrentPage);
        }

        private void FilePickerT(object sender, MouseButtonEventArgs e)
        {

        }
    }

    
}

