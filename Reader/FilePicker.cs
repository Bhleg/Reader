using MahApps.Metro.Controls;
using SharpCompress.Archives;
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
using Ghostscript.NET;
using Ghostscript.NET.Viewer;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace Reader
{
    /// <summary>
    /// Interaction logic for File_Picker.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        
        string CurrentPath = "";
        string DefaultPath = Environment.ExpandEnvironmentVariables("%HOMEPATH%");
        List<Item> Items = new List<Item>();
        //ObservableCollection<Library> LybraryCollection = new ObservableCollection<Library>();

        public void File_Picker()
        {
            InitializeComponent();
            FilePickerT.ItemsSource = Items;
            GetContent(DefaultPath);
            //GenerateLibrary();
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
                Items.Add(new Item() { Name = DirectorieName, Path = DirectoriePath, Type = "Folder", Icon = "/Icons/Folder.png" });
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

        //Load a File (cbz or cbr), and put all of their relevant entry in a bitmapimage then in a Dictionary
        public void FileLoader(string FilePath)
        {
            Pages.Clear();
            CurrentPage = 0;
            int i = 0;
            var archive = ArchiveFactory.Open(FilePath);
            foreach (var entry in archive.Entries)
            {
                //Check if the entries in the File are : not a directoy AND contain in their name .jpg OR .png
                if (!entry.IsDirectory & (entry.Key.ToLower().Contains(".jpg") | entry.Key.ToLower().Contains(".png")))
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

        public void PdfLoaderstring (string Filepath)
        {
            
        }


        private void CreateLibrary()
        {
            Properties.Settings.Default.Library.Add(CurrentPath);
            Properties.Settings.Default.Save(); // Saves settings in application configuration file
            string DirectorieName = System.IO.Path.GetFileName(CurrentPath);
            string Command = "GetContent(" + CurrentPath + ")";

            Menu.Add(new MenuPanelItem() { Name = DirectorieName, Command = CurrentPath });





        }

        

        //Item class which is used for each entry inside the datagrid Filepicker
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

        private void FilePickerT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Item File = (Item)FilePickerT.SelectedItem;
            string z = File.Type;



            //String trrrr = z.Type;
            // MessageBox.Show(trrrr);
            if (z == "File")
            {
                //MainWindow m = Application.ReferenceEquals MainWindow();
                //MainWindow m = (MainWindow)Application.Current.MainWindow;
                string Path = FilePickerT.SelectedValue.ToString();
                //m.CbzLoader(Path);
                FileLoader(Path);
                //m.CbzLoader(FilePickerT.SelectedValue.ToString());
                //FilePickerWindow.Hide();
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

           DeleteLibraryGrid.ItemsSource = Menu;


            //Button bn = sender as Button;
           // GetContent(bn.Tag.ToString());
            //FilePickerT.ItemsSource = Items;
            //FilePickerT.Items.Refresh();
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
            //DeleteLibraryGrid.Items.Refresh();
            //Menu.Clear();
            //MenuPanel();
            


            // Button bn = sender as Button;
            // GetContent(bn.Tag.ToString());
            // FilePickerT.ItemsSource = Items;
            // FilePickerT.Items.Refresh();
        }
    }

}