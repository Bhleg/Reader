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
using SharpCompress;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;
using SharpCompress.Archives;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        int CurrentPage = 0;
        string ViewerType = "Double";
        Dictionary<int, BitmapImage> Pages = new Dictionary<int, BitmapImage>();


        public void FileLoader(string FilePath)
        {
            Pages.Clear();
            int i = 1;

            var archive = ArchiveFactory.Open(FilePath);
            foreach (var entry in archive.Entries)
            {
                if (!entry.IsDirectory)
                {
                        MemoryStream memstream = new MemoryStream();
                        entry.WriteTo(memstream);
                        byte[] bytes = memstream.ToArray();
                        memstream.Seek(0, SeekOrigin.Begin);


                        InitializeComponent();
                        BitmapImage b = new BitmapImage();
                        b.BeginInit();

                        b.StreamSource = memstream;
                        b.CacheOption = BitmapCacheOption.OnLoad;
                        b.EndInit();

                        Pages.Add(i, b);

                        i++;
                    
                }
            }
          Viewer(ViewerType, "Load");
        }

        
        void Viewer(string ViewerType,string Action, int Page = 0)
        {
              
            if (ViewerType=="Single")
            {
                SinglePageViewer(Action);
            }
            else if (ViewerType=="Double")
            {
                DoublePageViewer(Action);
            }

            void SinglePageViewer(string z,int i = 0)
            {
                i = CurrentPage;
                if (z == "Load")
                {
                    i = 1;
                }
                else if (z == "Next")
                {
                    i = i + 1;
                }
                else if (z == "Previous")
                {
                    if (i == 1)
                    {
                        MessageBox.Show("First Page !");
                    }
                    else
                    {
                        i = i - 1;
                    }
                }
                BitmapImage a = new BitmapImage();
                a = Pages[i];
                CurrentPage = i;
                MainReader.Source = a;
            }

            void DoublePageViewer(string z,int i = 0)
            {
                i = CurrentPage;
                if (z == "Load")
                {
                    i = 1;

                }
                else if (z == "Next")
                {
                    
                    i = i + 2;
                    

                }
                else if (z == "Previous")
                {
                    if (i == 1)
                    {
                        MessageBox.Show("First Page !");
                    }
                    else
                    {
                        i--;
                    }
                }

                int ai = i;
                BitmapImage a = new BitmapImage();
                a = Pages[ai];
                //MainReader.Source = a;

                int bi = i;
                bi++;
                BitmapImage b = new BitmapImage();
                b = Pages[bi];
                //MainReader.Source = b;

                int astride = a.PixelWidth * a.Format.BitsPerPixel;
                int bstride = b.PixelWidth * b.Format.BitsPerPixel;

                // Create data array to hold source pixel data

                byte[] adata = new byte[astride * a.PixelHeight];
                byte[] bdata = new byte[bstride * b.PixelHeight];

                // Copy source image pixels to the data array
                a.CopyPixels(adata, astride, 0);
                //b.CopyPixels(bdata, bstride, 0);
                b.CopyPixels(bdata, bstride, 0);



                double abWidth = a.Width + b.Width;
                int abPixelWidth = a.PixelWidth + b.PixelWidth;
                int abPixelHeight = a.PixelHeight;

                //int abHeight = Convert.ToInt32(a.Height > b.Height ? a.Height : b.Height);
                int abHeight = Convert.ToInt32(a.Height);



                //Create the WriteableBitmap that will hold the two Page stitched together
               // WriteableBitmap ab = new WriteableBitmap(abPixelWidth, abPixelHeight, a.DpiX, a.DpiY, a.Format, a.Palette);
                WriteableBitmap ab = new WriteableBitmap(abPixelWidth, abPixelHeight, a.DpiX, a.DpiY, a.Format, BitmapPalettes.WebPaletteTransparent);

                // Write the pixel data to the WriteableBitmap.
                ab.WritePixels(
                  new Int32Rect(0, 0, a.PixelWidth, a.PixelHeight),
                  adata, astride, 0);
                ab.WritePixels(
                  new Int32Rect(a.PixelWidth, 0, b.PixelWidth, b.PixelHeight),
                  bdata, bstride, 0);
                CurrentPage = i;
                MainReader.Source = ab;
                




            }

        }

        private void PreviousPage_Event(object sender, MouseButtonEventArgs e)
        {
            if (CurrentPage == 1)
            {
                MessageBox.Show("First Page !");
            }
            else
            {
                //CurrentPage--;
                Viewer(ViewerType, "Previous");
            }

            
        }

        private void NextPage_Event(object sender, MouseButtonEventArgs e)
        {
            Viewer(ViewerType, "Next",CurrentPage);

        }

        private void FilePickerT_Event(object sender, MouseButtonEventArgs e)
        {
            File_Picker FilePickerWindow = new File_Picker();
            FilePickerWindow.Show();
        }


    }
    








}
