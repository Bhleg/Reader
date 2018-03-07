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

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static Book currentBook = new Book();
        
        public MainWindow()
        {

            
            InitializeComponent();
            MenuPanel();
            File_Picker();
           


            // Program p = new Program();
            //Program.RenderPage();

            // LeftPage.Visibility = Visibility.Collapsed;
            // RightPage.Visibility = Visibility.Collapsed;
            // SinglePage.Visibility = Visibility.Visible;
            //SinglePage.Source = Program.GetPdF(@"C:\Users\XXXXX\Desktop\Matt\FMD\downloads\Canard_PC_Hardware_Janvier_Fevrier_2017.pdf");
            //PdfLoaderstring(@"C:\IMG_0001.pdf");


        }

        string ViewerType = "Double";
        public static Dictionary<int, byte[]> Pages = new Dictionary<int, byte[]>();
        List<MenuPanelItem> Menu = new List<MenuPanelItem>();
        



        public void MenuPanel()
        {
            
            Menu.Clear();
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
            Viewer("Next", currentBook.CurrentPage);
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
                bNext.SetValue(Grid.ColumnProperty, 0);
                bPrevious.SetValue(Grid.ColumnProperty, 4);
                LeftPage.HorizontalAlignment = HorizontalAlignment.Left;
                RightPage.HorizontalAlignment = HorizontalAlignment.Right;
                LeftPage.SetValue(Grid.ColumnProperty,1);
                RightPage.SetValue(Grid.ColumnProperty,0);
            }
            else
            {
                readingDirection = "Left to Right";
                bNext.SetValue(Grid.ColumnProperty, 4);
                bPrevious.SetValue(Grid.ColumnProperty, 0);
                LeftPage.HorizontalAlignment = HorizontalAlignment.Right;
                RightPage.HorizontalAlignment = HorizontalAlignment.Left;
                LeftPage.SetValue(Grid.ColumnProperty,0);
                RightPage.SetValue(Grid.ColumnProperty,1);
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
                Viewer("Start", currentBook.CurrentPage);
                //DoublePageViewer("Start", currentBook.CurrentPage);
            }
            else if (value == "DPsc")
            {
                currentViewer = "Double";
                Viewer("Start", currentBook.CurrentPage);
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
            MenuGrid.Items.Refresh();
            DeleteLibrary_Button.ContextMenu.IsOpen = false;
        }

        private void OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
    








}
