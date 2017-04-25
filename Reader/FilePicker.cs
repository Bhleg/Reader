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
using System.Threading;
using System.Threading.Tasks;

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
                Items.Add(new Item() { Name = DirectorieName, Path = DirectoriePath, Type = "Folder", Icon = "\uE188" });
            }

            // Process the list of files found in the directory.
            string[] FileEntries = Directory.GetFiles(Path).Where(s => s.EndsWith(".cbz") || s.EndsWith("cbr") || s.EndsWith("zip") || s.EndsWith("rar")).ToArray();
            foreach (string FilePath in FileEntries)
            {

                string FileName = System.IO.Path.GetFileName(FilePath);
                Items.Add(new Item() { Name = FileName, Path = FilePath, Type = "File", Icon = Char.ConvertFromUtf32(0x1f4d6) });

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
            
            
            List<string> SortedOrder = new List<string>();
            
            foreach (var entry in archive.Entries)
            {
                //Check if the entries in the File are : not a directoy AND contain in their name .jpg OR .png
                if (!entry.IsDirectory & (entry.Key.ToLower().Contains(".jpg") | entry.Key.ToLower().Contains(".png")))
                {

                        i++;


                    //SortedOrder.FindIndex(s => s.Equals(entry.ToString()));
                    using (MemoryStream MemStream = new MemoryStream())
                    {
                         entry.WriteTo(MemStream);
                         MemStream.Seek(0, SeekOrigin.Begin);
                         byte[] bytes = MemStream.ToArray();
                        //MessageBox.Show(entry.Key.ToString());
                        //MessageBox.Show(SortedOrder.FindIndex(s => s.Equals(entry.ToString())).ToString());
                         Pages.Add(i, bytes);
                    }
                        
                    
                    


                }

            }
            archive = null;
            TotalPages = i;
            BookName = System.IO.Path.GetFileName(FilePath);
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
                string Path = FilePickerT.SelectedValue.ToString();
                FileLoader(Path);  
               
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
           
            CreateLibrary();
            MenuPanel();
            MenuGrid.Items.Refresh();


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