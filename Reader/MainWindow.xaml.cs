using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using SharpCompress.Archives;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
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
        



        public void MenuPanel()
        {
            MenuGrid.ItemsSource = Menu;
            Menu.Add(new MenuPanelItem() { Name = "Explorer", Command = "Explorer", Icon = "\uE188" });
            Menu.Add(new MenuPanelItem() { Name = "Reader",  Command = "Reader", Icon = Char.ConvertFromUtf32(0x1f4d6) });
            Menu.Add(new MenuPanelItem() { Name = "Setting", Command = "Setting", Icon = "\uE115" });
            GenerateLibrary();
            
            
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

                Menu.Add(new MenuPanelItem() { Name = DirectorieName, Command = item, Type = "Library", Icon = "\uE1D3" });
            }

        }

        class MenuPanelItem
        {
            public string Name { get; set; }
            public string Command { get; set; }
            public string Type { get; set; }
            public string Icon { get; set; }


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
