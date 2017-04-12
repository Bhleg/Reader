using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using SharpCompress.Archives;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        int CurrentPage = 0;
        int TotalPages = 0;
        //string ViewerType = "Simple";
        string ViewerType = Properties.Settings.Default.DefaultViewer;
        // Dictionary<int, BitmapImage> Pages = new Dictionary<int, BitmapImage>();
        Dictionary<int, byte[]> Pages = new Dictionary<int, byte[]>();

        //Load a File (cbz or cbr), and put all of their relevant entry in a bitmapimage then in a Dictionary
        public void FileLoader(string FilePath)
        {
            Pages.Clear();
            int i = 0;
            var archive = ArchiveFactory.Open(FilePath) ;
            foreach (var entry in archive.Entries)
            {
                //Check if the entries in the File are : not a directoy AND contain in their name .jpg OR .png
                if (!entry.IsDirectory & (entry.Key.ToLower().Contains(".jpg") | entry.Key.ToLower().Contains(".png")) )
                {
                   
                        i++;
                        MemoryStream memstream = new MemoryStream();
                        entry.WriteTo(memstream);
                        memstream.Seek(0, SeekOrigin.Begin);
                        byte[] bytes = memstream.ToArray();
                        Pages.Add(i, bytes);

                        // set variable to null to avoid memory hog
                        memstream = null;
                        bytes = null;

                    
                        
                    
                }
                
            }
            archive = null;
            TotalPages = i;

           // archive = null;
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
                DoubePage.Visibility = Visibility.Collapsed;
                SinglePage.Visibility = Visibility.Visible;
                i = CurrentPage;
                if (z == "Load")
                {
                    i = 1;
                }
                else if (z == "Next")
                {
                    if (i <= TotalPages--)
                    {
                        i = i + 1;
                    }
                    else
                    {
                        MessageBox.Show("Last Page !");
                    }
                    
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
              //  a = Pages[i];
                CurrentPage = i;
                SinglePage.Source = a;
            }

            void DoublePageViewer(string z,int i = 0)
            {
                LeftPage.Source = null;
                RightPage.Source = null;
                SinglePage.Visibility = Visibility.Collapsed;
                DoubePage.Visibility = Visibility.Visible;
                i = CurrentPage;
                if (z == "Load")
                {
                    i = 1;

                }
                else if (z == "Next")
                {
                    if (i+1 == TotalPages)
                    {
                        MessageBox.Show("Last Page !");
                        return;

                    }
                    i = i + 2;
                 
                    
                    

                }
                else if (z == "Previous")
                {
                    if (i <= 2)
                    {
                        MessageBox.Show("First Page !");
                        return;
                        
                    }
                    else
                    {
                        i = i - 2;
                    }
                }

                
                

                int ai = i;

                var amemoryStream = new MemoryStream(Pages[ai]);
                amemoryStream.Seek(0, SeekOrigin.Begin);
                BitmapImage a = new BitmapImage();
                a.BeginInit();
                a.StreamSource= amemoryStream ;
                a.CacheOption = BitmapCacheOption.OnLoad;
                a.EndInit();
               // a.Freeze();
                LeftPage.Source = a;
                a = null;
                amemoryStream = null;




                int bi = i;
                bi++;
                if (bi <= TotalPages)
                {


                    var bmemoryStream = new MemoryStream(Pages[bi]);
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.StreamSource = bmemoryStream;
                    b.CacheOption = BitmapCacheOption.OnLoad;

                    b.EndInit();
                    //b.Freeze();
                    RightPage.Source = b;
                    b = null;
                    bmemoryStream = null;

                }
                else
                {
                    RightPage.Source = null;
                }

                CurrentPage = i;

            }

        }


        private void NextPage_Event(object sender, EventArgs e)
        {
            Viewer(ViewerType, "Next",CurrentPage);

        }

        private void FilePickerT_Event(object sender, EventArgs e)
        {
            File_Picker FilePickerWindow = new File_Picker();
            FilePickerWindow.Show();
        }

        private void MainMenu_Event(object sender, EventArgs e)
        {
            
           if (MainMenu.Opacity == 0.75)
            {
                MainMenu.Opacity = 0;
            }
            else if (MainMenu.Opacity == 0)
            {
                MainMenu.Opacity = 0.75;
            }

        }

        private void PreviousPage_Event(object sender, RoutedEventArgs e)
        {

                Viewer(ViewerType, "Previous");

        }
    }
    








}
