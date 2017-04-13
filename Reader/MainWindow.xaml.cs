using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using SharpCompress.Archives;
using System.Windows.Controls;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow ()
        {
            InitializeComponent();
            MenuPanel();
            File_Picker();
        }
        int CurrentPage = 0;
        int TotalPages = 0;
        string ViewerType = "Double";
        //string ViewerType = Properties.Settings.Default.DefaultViewer;
        // Dictionary<int, BitmapImage> Pages = new Dictionary<int, BitmapImage>();
        Dictionary<int, byte[]> Pages = new Dictionary<int, byte[]>();
        List<MenuPanelItem> Menu = new List<MenuPanelItem>();
        

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
            Viewer(ViewerType, "Start");
            ShowReader();


        }

        public void MenuPanel()
        {
            
            Menu.Add(new MenuPanelItem() { Name = "Explorer", Command = "Explorer", Icon = "/Icons/Folder.png" });
            Menu.Add(new MenuPanelItem() { Name = "Reader",  Command = "Reader", Icon = "/Icons/Book.png" });
            Menu.Add(new MenuPanelItem() { Name = "setting", Command = "Setting", Icon = "/Icons/Settings.png" });
            GenerateLibrary();
            
            MenuItem.ItemsSource = Menu;
            //MenuIcon.ItemsSource = Menu;
            InitializeComponent();



        }

        void GenerateLibrary()
        {
            // BookmarkPanel.Children.Add(new Button { Content = "Button" });
            foreach (string item in Properties.Settings.Default.Library)
            {
                string DirectorieName = System.IO.Path.GetFileName(item);
                //string Command = "GetContent(" + item + ")";

                Menu.Add(new MenuPanelItem() { Name = DirectorieName, Command = item, Type = "Library", Icon = "/Icons/Library.png" });
            }

        }

        class MenuPanelItem
        {
            public string Name { get; set; }
            public string Command { get; set; }
            public string Type { get; set; }
            public string Icon { get; set; }


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

        void ShowExplorer()
        {
            Settings.Visibility = Visibility.Collapsed;
            Reader.Visibility = Visibility.Collapsed;
            FilePicker.Visibility = Visibility.Visible;
        }
        void ShowReader()
        {
            Settings.Visibility = Visibility.Collapsed;
            FilePicker.Visibility = Visibility.Collapsed;
            Reader.Visibility = Visibility.Visible;
        }
        void ShowSetting()
        {
            FilePicker.Visibility = Visibility.Collapsed;
            Reader.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Visible;
        }
        ////////////////////////////EVENT//////////////////////////////  



        private void NextPage_Event(object sender, EventArgs e)
        {
            Viewer(ViewerType, "Next",CurrentPage);

        }

        private void FilePickerT_Event(object sender, EventArgs e)
        {
            //File_Picker FilePickerWindow = new File_Picker();
            //FilePickerWindow.Show();
        }

        private void MainMenu_Event(object sender, EventArgs e)
        {
            
           if (MainMenu.Opacity == 0.75)
            {
                MainMenu.Opacity = 0;
                StackMenu.Visibility = Visibility.Collapsed;
                MenuGrid.Visibility = Visibility.Collapsed;
            }
            else if (MainMenu.Opacity == 0)
            {
                MainMenu.Opacity = 0.75;
                StackMenu.Visibility = Visibility.Visible;
                MenuGrid.Visibility = Visibility.Visible;
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

        private void MenuPanelbtn_Event(object sender, RoutedEventArgs e)
        {
            Button bn = sender as Button;
            string Command = bn.Tag.ToString();
            if (Command == "Explorer")
            {
                ShowExplorer();
            }
            else if (Command == "Reader")
            {
                ShowReader();
            }
            else if (Command == "Setting")
            {
                ShowSetting();
            }
            else
            {
                ShowExplorer();
                GetContent(bn.Tag.ToString());
                FilePickerT.ItemsSource = Items;
                FilePickerT.Items.Refresh();
            }
        }
    }
    








}
