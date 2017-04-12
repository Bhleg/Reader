using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reader
{
    /// <summary>
    /// Interaction logic for File_Picker.xaml
    /// </summary>
    public partial class File_Picker : Window
    {
        
        string CurrentPath = "";
        string DefaultPath = Environment.ExpandEnvironmentVariables("%HOMEPATH%");
        List<Item> Items = new List<Item>();
        ObservableCollection<Library> LybraryCollection = new ObservableCollection<Library>();

        public File_Picker()
        {
            InitializeComponent();
            //List<Item> Items = new List<Item>();
            LibraryGrid.ItemsSource = LybraryCollection;
            FilePickerT.ItemsSource = Items;
            GetContent(DefaultPath);
            GenerateLibrary();



        }


        //The Function wich is used to get the content of a folder(Path) and fill the Items list with the content of the folder(Path)
        void GetContent(string Path)
        {
            //List<Item> Items = new List<Item>();
            Items.Clear();



            // Process the list of directory found in the directory.
            string[] DirectoryEntries = Directory.GetDirectories(Path);
            foreach (string DirectoriePath in DirectoryEntries)
            {
                string DirectorieName = System.IO.Path.GetFileName(DirectoriePath);
                Items.Add(new Item() { Name = DirectorieName, Path = DirectoriePath, Type = "Folder" , Icon = "/Icons/Folder.png"});
                }

            // Process the list of files found in the directory.
            string[] FileEntries = Directory.GetFiles(Path).Where(s => s.EndsWith(".cbz") || s.EndsWith("cbr") || s.EndsWith("zip") || s.EndsWith("rar")).ToArray();
            foreach (string FilePath in FileEntries)
            {

                string FileName = System.IO.Path.GetFileName(FilePath);
                Items.Add(new Item() { Name = FileName, Path = FilePath, Type = "File", Icon = "/Icons/Message.png" });
                
            }


            CurrentPath = Path;

        }


        private void CreateLibrary()
        {
            Properties.Settings.Default.Library.Add(CurrentPath);
            Properties.Settings.Default.Save(); // Saves settings in application configuration file
            string DirectorieName = System.IO.Path.GetFileName(CurrentPath);
            string Command = "GetContent(" + CurrentPath + ")";
            
            LybraryCollection.Add(new Library() { Name = DirectorieName, Path = CurrentPath});

            



        }

        void GenerateLibrary()
        {
            // BookmarkPanel.Children.Add(new Button { Content = "Button" });
            foreach (string item in Properties.Settings.Default.Library)
            {
                string DirectorieName = System.IO.Path.GetFileName(item);
                string Command = "GetContent(" + item + ")";

                LybraryCollection.Add(new Library() { Name = DirectorieName, Path = item });
            }
            
        }
        
        //Item class which is used for each entry inside the datagris Filepicker
        class Item
        {
            public string Name { get; set; }

            public string Path { get; set; }

            public string Type { get; set; }

            public string Icon { get; set; }
        }


        //Library class which is used for each Library entry at the left of the datagrid Filepicker
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        class Library
        {
            public string Name { get; set; }

            public string Path { get; set; }

            //public string Command { get; set; }


        }



        ////////////////////////////EVENT//////////////////////////////  
       
        //Event For the Buttons inside the File Picker datagrid
        private void ButtonClick_Event(object sender, EventArgs e)
        {
            Item i = (Item)FilePickerT.SelectedItem;
            String result = (i.Type).ToString();
            if (result == "File")
            {
                //MainWindow m = Application.ReferenceEquals MainWindow();
                MainWindow m = (MainWindow)Application.Current.MainWindow;
                string Path = FilePickerT.SelectedValue.ToString();
                //m.CbzLoader(Path);
                m.FileLoader(Path) ;
                //m.CbzLoader(FilePickerT.SelectedValue.ToString());
                FilePickerWindow.Hide();
            }
            else
            {
                GetContent(FilePickerT.SelectedValue.ToString());
                FilePickerT.ItemsSource = Items;
                FilePickerT.Items.Refresh();
            }

            
            
        }

        //Event for the GoUP Button inside the File Picker Window
        private void GoUp_Event(object sender, EventArgs e)
        {

            
            string GoUpPath = System.IO.Path.GetDirectoryName(CurrentPath);

            if (GoUpPath == null)
            {
                GoUpPath = CurrentPath;
                
            }
            else
            {
                
                GetContent(GoUpPath);
                FilePickerT.ItemsSource = Items;
                FilePickerT.Items.Refresh();
            
            }



        }

        //Event for the CreateLibrary Button inside the File Picker Window
        protected void CreateLibrary_Event(object sender, EventArgs e)
        {
            //Button bn = sender as Button;
            //GetContent(bn.Tag.ToString());
            CreateLibrary();
            //FilePickerT.ItemsSource = Items;
            //FilePickerT.Items.Refresh();
        }

        //Event for the DeleteLibrary Button inside the File Picker Window
        protected void PickerSettings_Event(object sender, EventArgs e)
        {
            if (Settings.Visibility == Visibility.Visible)
            {
                Settings.Visibility = Visibility.Collapsed;
            }
            else if (Settings.Visibility == Visibility.Collapsed)
            {
                Settings.Visibility = Visibility.Visible;
            }

            DeleteLibraryGrid.ItemsSource = LybraryCollection;


            // Button bn = sender as Button;
            // GetContent(bn.Tag.ToString());
            // FilePickerT.ItemsSource = Items;
            // FilePickerT.Items.Refresh();
        }

        //Event for the Library's Button inside the File Picker Window
        protected void GoToLibrary_Event(object sender, EventArgs e)
        {
            Button bn = sender as Button;
            GetContent(bn.Tag.ToString());
            FilePickerT.ItemsSource = Items;
            FilePickerT.Items.Refresh();
        }

        //Event for the DeleteLibrary Button inside the File Picker Window
        protected void DeleteLibrary_Event(object sender, EventArgs e)
        {
            Button bn = sender as Button;
            Properties.Settings.Default.Library.Remove(bn.Tag.ToString());
            Properties.Settings.Default.Save(); // Saves settings in application configuration file
            DeleteLibraryGrid.Items.Refresh();
            LybraryCollection.Clear();
            GenerateLibrary();
            //MessageBox.Show("DELETED!");


            // Button bn = sender as Button;
            // GetContent(bn.Tag.ToString());
            // FilePickerT.ItemsSource = Items;
            // FilePickerT.Items.Refresh();
        }
    }
}
    

