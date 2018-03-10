using System;
using System.Collections.Generic;
using System.Windows;
using System.Configuration;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using SharpCompress.Archives;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using System.Windows.Controls.Primitives;
using MupdfSharp;
using System.ComponentModel;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        
        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = currentBook;
            MenuPanel();
            File_Picker();
            ShowFilePicker();

        }

        public static Book currentBook = new Book();
        public static Dictionary<int, byte[]> Pages = new Dictionary<int, byte[]>();
        List<MenuPanelItem> Menu = new List<MenuPanelItem>();

        public void MenuPanel()
        {
            
            Menu.Clear();
            TopPanel.ItemsSource = Menu;
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
            if (Properties.Settings.Default.Library != null) // catch null if library is empty
            {
                foreach (string item in Properties.Settings.Default.Library)
                {
                    string DirectorieName = System.IO.Path.GetFileName(item);
                    //string Command = "GetContent(" + item + ")";

                    Menu.Add(new MenuPanelItem() { Name = DirectorieName, Command = item, Type = "Library", Icon = "\uE1D3" });
                }
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
            TopPanel.Visibility = Visibility.Visible;
            BottomPanel.Visibility = Visibility.Collapsed;
        }
        void ShowReader()
        {
            Settings.Visibility = Visibility.Collapsed;
            FilePicker.Visibility = Visibility.Collapsed;
            Reader.Visibility = Visibility.Visible;
            MenuView.Visibility = Visibility.Collapsed;
        }
        void ShowSetting()
        {
            FilePicker.Visibility = Visibility.Collapsed;
            Reader.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Visible;
            TopPanel.Visibility = Visibility.Visible;
            BottomPanel.Visibility = Visibility.Collapsed;
        }
        void ShowFilePicker()
        {
            FilePicker.Visibility = Visibility.Visible;
            Reader.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
            TopPanel.Visibility = Visibility.Visible;
            BottomPanel.Visibility = Visibility.Collapsed;
        }
        void SetDirection(string t)
        {
            if (t == "LtR")
            {
                bNext.SetValue(Grid.ColumnProperty, 4);
                bPrevious.SetValue(Grid.ColumnProperty, 0);
                LeftPage.HorizontalAlignment = HorizontalAlignment.Right;
                RightPage.HorizontalAlignment = HorizontalAlignment.Left;
                LeftPage.SetValue(Grid.ColumnProperty, 0);
                RightPage.SetValue(Grid.ColumnProperty, 1);
            }
            else if (t == "RtL")
            {
                bNext.SetValue(Grid.ColumnProperty, 0);
                bPrevious.SetValue(Grid.ColumnProperty, 4);
                LeftPage.HorizontalAlignment = HorizontalAlignment.Left;
                RightPage.HorizontalAlignment = HorizontalAlignment.Right;
                LeftPage.SetValue(Grid.ColumnProperty, 1);
                RightPage.SetValue(Grid.ColumnProperty, 0);
            }
        }
        ////////////////////////////EVENT//////////////////////////////  



        private void NextPage_Event(object sender, EventArgs e)
        {
            Viewer("Next", currentBook.CurrentPage);
        }

        private void FilePickerT_Event(object sender, EventArgs e)
        {
            //File_Picker FilePickerWindow = new File_Picker();
            //FilePickerWindow.Show();
        }

        private void MainMenu_Event(object sender, EventArgs e)
        {
            
           if (MenuView.Visibility == Visibility.Visible)
            {
                //MainMenu.Opacity = 0;
                MenuView.Visibility = Visibility.Collapsed;
                TopPanel.Visibility = Visibility.Collapsed;
                BottomPanel.Visibility = Visibility.Collapsed;
            }
            else if (MenuView.Visibility == Visibility.Collapsed)
            {
                //MainMenu.Opacity = 0.75;
                MenuView.Visibility = Visibility.Visible;
                TopPanel.Visibility = Visibility.Visible;
                BottomPanel.Visibility = Visibility.Visible;
            }

        }

        private void PreviousPage_Event(object sender, RoutedEventArgs e)
        {
                Viewer("Previous");
        }

        private void Quit_Event(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SwitchReadingDirection_Event(object sender, RoutedEventArgs e)
        {
            if (readingDirection == "Left to Right")
            {
                readingDirection = "Right to Left";
                SetMetadata(currentBook.Path, "ReadDirection", "RtL");
                SetDirection("RtL");
            }
            else
            {
                readingDirection = "Left to Right";
                SetMetadata(currentBook.Path, "ReadwDirection", "LtR");
                SetDirection("LtR");
            }
            tbReadingDirection.Content = "Current : "+readingDirection;           
            //Viewer("Start", currentBook.CurrentPage);
        }

        private void SwitchViewingMode_Event(object sender, RoutedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;
            // Get the Tag of selected item
            string value = comboBox.SelectedValue as string;

            if (value == "DPdc")
            {
                currentViewer = "Double";
                currentViewerOption = "dc";
                if (currentViewer == "Double (Single Cover)" && currentBook.CurrentPage > 1)
                {
                    currentBook.CurrentPage = currentBook.CurrentPage - 1;
                }
                Viewer("Start", currentBook.CurrentPage);
                SetMetadata(currentBook.Path, "Viewer", "DPdc");
                //DoublePageViewer("Start", currentBook.CurrentPage);
            }
            else if (value == "DPsc")
            {
                currentViewer = "Double";
                currentViewerOption = "sc";
                if (currentBook.CurrentPage > 1)
                {
                    currentBook.CurrentPage = currentBook.CurrentPage - 1;
                }
                
                Viewer("Start", currentBook.CurrentPage);
                SetMetadata(currentBook.Path, "Viewer", "DPsc");
                //DoublePageViewer("Start", currentBook.CurrentPage);
            }
            else if (value == "SP")
            {
                currentViewer = "Single";
                Viewer("Start", currentBook.CurrentPage);
                //SinglePageViewer("Start", currentBook.CurrentPage);
            }
          
        }

        private void MenuPanelbtn_Event(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("ok!");
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

        private void ContextMenuLibraryList_Event(object sender, RoutedEventArgs e)
        {
           
            List<MenuPanelItem> Librarylist = new List<MenuPanelItem>();
           
            foreach (string item in Properties.Settings.Default.Library)
            {
                string DirectorieName = System.IO.Path.GetFileName(item);
                //string Command = "GetContent(" + item + ")";

                Librarylist.Add(new MenuPanelItem() { Name = DirectorieName, Command = item });
            }
            //(sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;           
            (sender as Button).ContextMenu.IsOpen = true;
            (sender as Button).ContextMenu.ItemsSource = Librarylist;



        }

        private void DeleteLibrary_Event(object sender, RoutedEventArgs e)
        {
            Button bn = sender as Button;
            Properties.Settings.Default.Library.Remove(bn.Tag.ToString());
            Properties.Settings.Default.Save();
            MenuPanel();
            TopPanel.Items.Refresh();
            DeleteLibrary_Button.ContextMenu.IsOpen = false;
        }

        private void OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
    








}
