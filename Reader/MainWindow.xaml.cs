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
        string ViewerType = "Double";
        //string ViewerType = Properties.Settings.Default.DefaultViewer;
        // Dictionary<int, BitmapImage> Pages = new Dictionary<int, BitmapImage>();
        Dictionary<int, byte[]> Pages = new Dictionary<int, byte[]>();

        //Load a File (cbz or cbr), and put all of their relevant entry in a bitmapimage then in a Dictionary
        public void FileLoader(string FilePath)
        {
            Pages.Clear();
            CurrentPage = 0;
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
           Viewer(ViewerType, "Start");
            
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
                    if (i+1 <= TotalPages)
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

                var memoryStream = new MemoryStream(Pages[i]);
                memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapImage a = new BitmapImage();
                a.BeginInit();
                a.StreamSource = memoryStream;
                a.CacheOption = BitmapCacheOption.OnLoad;
                a.EndInit();
                SinglePage.Source = a;
                CurrentPage = i;
                
            }

            void DoublePageViewer(string z,int i = 0)
            {

                i = CurrentPage;
                

                if (z == "Start")
                {
                    i = 1;

                }
                else if (z == "Next")
                {
                    if (i+1 == TotalPages)
                    {
                        
                        return;

                    }
                    i = i + 1;
                    //MessageBox.Show(i.ToString());

                }
                else if (z == "Previous")
                {
                    if (i <= 2)
                    {
                        
                        return;
                        
                    }
                    else
                    {
                        i = i - 2;
                    }
                }


                LeftPage.Source = null;
                RightPage.Source = null;
                SinglePage.Source = null;

                int ia = i;
                int ib = i + 1;

                BitmapImage a = CreatePage(ia);
                BitmapImage b = CreatePage(ib);


                if (a.Width < a.Height && b.Width < b.Height)
                {
                    SetLeftPage(a);
                    SetRightPage(b);
                    i = i + 1;
                }
                else if (a.Width > a.Height | (a.Width < a.Height && b.Width > b.Height))
                {
                    SetSinglePage(a);
                    
                }
                
                CurrentPage = i;
                //MessageBox.Show("i = " + i + "\n" + CurrentPage.ToString() + " sur " + TotalPages.ToString() + " Pages");




                BitmapImage CreatePage(int p)
                {
                    var memoryStream = new MemoryStream(Pages[p]);
                    BitmapImage Image = new BitmapImage();
                    Image.BeginInit();
                    Image.StreamSource = memoryStream;
                    Image.CacheOption = BitmapCacheOption.OnLoad;
                    Image.EndInit();
                    return Image;
                }

                void SetLeftPage(BitmapImage Image)
                {
                    LeftPage.Visibility = Visibility.Visible;
                    RightPage.Visibility = Visibility.Visible;
                    SinglePage.Visibility = Visibility.Collapsed;
                    LeftPage.Source = Image;

                }

                void SetRightPage(BitmapImage Image)
                {
                    LeftPage.Visibility = Visibility.Visible;
                    RightPage.Visibility = Visibility.Visible;
                    SinglePage.Visibility = Visibility.Collapsed;


                    RightPage.Source = Image;


                }

                void SetSinglePage(BitmapImage Image)
                {
                    LeftPage.Visibility = Visibility.Collapsed;
                    RightPage.Visibility = Visibility.Collapsed;
                    SinglePage.Visibility = Visibility.Visible;

                    SinglePage.Source = Image;

                }


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
                StackMenu.Visibility = Visibility.Collapsed;
            }
            else if (MainMenu.Opacity == 0)
            {
                MainMenu.Opacity = 0.75;
                StackMenu.Visibility = Visibility.Visible;
            }

        }

        private void PreviousPage_Event(object sender, RoutedEventArgs e)
        {

                Viewer(ViewerType, "Previous");

        }

        private void Quit_Event(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
    








}
