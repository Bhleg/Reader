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
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using MupdfSharp;
using System.ComponentModel;

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
            //check if the path is part of a library
            bool IsLibrary = false;
            foreach (string sl in LibraryList)
            {

                if (Path.Contains(sl))
                {
                    IsLibrary = true;
                    break;
                } 
             
            }

            Items.Clear();

            // Process the list of directory found in the directory.
            string[] DirectoryEntries = Directory.GetDirectories(Path);
            foreach (string DirectoriePath in DirectoryEntries)
            {
                string DirectorieName = System.IO.Path.GetFileName(DirectoriePath);

                //ignore .metadate folder
                if (DirectorieName==".metadata")
                {
                    continue;
                }
                Items.Add(new Item() { Name = DirectorieName, Path = DirectoriePath, Type = "Folder", Icon = "\uE188" });
            }

            // Process the list of files found in the directory.
            string[] FileEntries = Directory.GetFiles(Path).Where(s => s.EndsWith(".cbz") || s.EndsWith("cbr") || s.EndsWith("zip") || s.EndsWith("rar") || s.EndsWith("pdf")).ToArray();
            foreach (string FilePath in FileEntries)
            {

                String Font = null;

                //If the path is part of a library the program should act diferently than a standard path
                if (IsLibrary)
                {
                    //set Read State
                    string ReadState = ReadMetadata(FilePath, "ReadState");
                    if (ReadState == "Read") { Font = "Light"; }
                    else { ReadState = "Unread"; }

                    //the thumbnail part
                    string Thumb = System.IO.Path.GetDirectoryName(FilePath) + "\\.metadata\\" + System.IO.Path.GetFileNameWithoutExtension(FilePath) + ".jpg";
                    if (!File.Exists(Thumb))
                    {
                        CreateThumbnail(FilePath);
                    }

                    // If the file exist get the thumbnail
                    if (File.Exists(Thumb))
                    {
                       BitmapImage t = GetThumbFromFile(Thumb);
                        string FileName = System.IO.Path.GetFileName(FilePath);
                       
                        Items.Add(new Item() { Name = FileName, Path = FilePath, Type = "File", Status = ReadState, FontWeight = Font, Thumb = t });
                    }
                    else
                    {
                        string FileName = System.IO.Path.GetFileName(FilePath);
                        
                        Items.Add(new Item() { Name = FileName, Path = FilePath, Type = "File", Status = ReadState, Icon = Char.ConvertFromUtf32(0x1f4d6), FontWeight = Font});
                    }
                    
                  
                        
                    

                }
                else
                {
                    string FileName = System.IO.Path.GetFileName(FilePath);
                    Items.Add(new Item() { Name = FileName, Path = FilePath, Type = "File", Icon = Char.ConvertFromUtf32(0x1f4d6), FontWeight = Font });
                }

                

            }


            CurrentPath = Path;

        }




        //Load book ... pretty straightforward
        public void LoadBook(string FilePath, int p = 1)
        {
            
            Pages.Clear();
            currentBook.CurrentPage = p;
            currentBook.Name = System.IO.Path.GetFileName(FilePath);            
            currentBook.Type = Path.GetExtension(FilePath).ToLower();
            currentBook.Path = FilePath;

            // use the apropriate function for compressed or pdf file
            if (currentBook.Type == ".pdf")
            {
                //Call the function to load the pdf (not the pages)
                //the function that load the page is called within the reader.cs
                //since on a pdf the page is loaded on demand for memory efficiency purpose
                Program.LoadPDF(FilePath);
                currentBook.TotalPages = Program.PDFBook.TotalPage;           
                
            }
            else
            {               
                ArchiveLoader(FilePath);
            }

            // Get The Reading direction from metadate and set it
            string ReadDirection = ReadMetadata(FilePath, "ReadDirection");
           // MessageBox.Show(ReadMetadata(FilePath, "ReadDirection"));
            if (ReadDirection == "RtL") { SetDirection("RtL"); }
            else { SetDirection("LtR"); }

            // Get The View Mode from metadate and set it
            string v = ReadMetadata(FilePath, "Viewer");
            // MessageBox.Show(ReadMetadata(FilePath, "ReadDirection"));
            if (v == "DPdc") { currentViewerOption = "dc"; }
            else { currentViewerOption = "sc"; }


            Viewer("Start");
            
            
            GC.Collect();
          
            ShowReader();
            PagesList.Clear();
            cPageMini2.ItemsSource = null;
            
            GenerateMiniPage();
            cPageMini2.ItemsSource = PagesList;
            cPageMini2.UpdateLayout();
            cPageMini2.SelectedIndex = 12;
            //ScrollToCenterOfView(cPageMini2.SelectedItem);





            void GenerateMiniPage()
            {
                int i = 0;
                foreach (KeyValuePair<int, byte[]> entry in Pages)
                {
                    
                    // do something with entry.Value or entry.Key
                    PagesList.Add(entry.Key,CreatePage(entry.Key,"Mini"));

                }
            }
            


            void ArchiveLoader(string Path)
            {
                int i = 0;
                var archive = ArchiveFactory.Open(Path);
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
                            Pages.Add(i, bytes);
                            

                           
                            bytes = null;



                        }


                    }

                }
                archive = null;
                currentBook.TotalPages = i;
            }

        }

        //Get Cover for Book
        public static Dictionary<int, byte[]> Cover = new Dictionary<int, byte[]>();
        public void CreateThumbnail(string FilePath, int p = 1)
        {
            Dictionary<int, byte[]> Cover = new Dictionary<int, byte[]>();
            Cover.Clear();

            string type = Path.GetExtension(FilePath).ToLower();

            // use the apropriate function for compressed or pdf file
            if (System.IO.Path.GetExtension(FilePath) == ".pdf")
            {
                //Call the function to load the pdf (not the pages)
                //the function that load the page is called within the reader.cs
                //since on a pdf the page is loaded on demand for memory efficiency purpose
                //  Program.LoadPDF(FilePath);
                //  currentBook.TotalPages = Program.PDFBook.TotalPage;
                return;
            }
            else
            {
                ArchiveLoader(FilePath);
            }

            BitmapImage a = CreateCover(1);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(a));
            string cover = Path.GetDirectoryName(FilePath)+"/.metadata/"+ Path.GetFileNameWithoutExtension(FilePath)+".jpg";
            using (var fileStream = new System.IO.FileStream(cover, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
                
            }

            GC.Collect();
            
            void ArchiveLoader(string Path)
            {
                int i = 0;
                var archive = ArchiveFactory.Open(Path);
                foreach (var entry in archive.Entries)
                {
                    //Check if the entries in the File are : not a directoy AND contain in their name .jpg OR .png
                    if (!entry.IsDirectory & (entry.Key.ToLower().Contains(".jpg") | entry.Key.ToLower().Contains(".png")))
                    {

                        i++;
                        if (i <= 1)
                        {                          
                            using (MemoryStream MemStream = new MemoryStream())
                            {
                                entry.WriteTo(MemStream);
                                MemStream.Seek(0, SeekOrigin.Begin);
                                byte[] bytes = MemStream.ToArray();
                                Cover.Add(i, bytes);
                                bytes = null;

                            }
                        }
                        else
                        {
                            break;
                        }



                    }

                }
                archive = null;
                
            }

            BitmapImage CreateCover(int c)
            {

                using (var memoryStream = new MemoryStream(Cover[c]))
                using (WrappingStream wrapper = new WrappingStream(memoryStream))
                {
                    BitmapImage Image = new BitmapImage();
                    Image.BeginInit();
                    Image.StreamSource = wrapper;
                    Image.CacheOption = BitmapCacheOption.OnLoad;
                    // Image.DecodePixelWidth = Convert.ToInt32(FilePickerT.ActualWidth);
                    Image.DecodePixelWidth = 200;
                    Image.EndInit();
                    Image.Freeze();
                    return Image;
                }

            }

        }
        public BitmapImage GetThumbFromFile(string Filepath)
        {           
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("C:"+Filepath);
            image.EndInit();
            //image2.Source = image;
            return image;
        }

       


        private void CreateLibrary()
        {
           
            Properties.Settings.Default.Library.Add(CurrentPath);
            Properties.Settings.Default.Save(); // Saves settings in application configuration file
            string DirectorieName = System.IO.Path.GetFileName(CurrentPath);
            string Command = "GetContent(" + CurrentPath + ")";

            Menu.Add(new MenuPanelItem() { Name = DirectorieName, Command = CurrentPath });
            





        }

        public class Book : INotifyPropertyChanged
        {
           
            
            public string Name { get; set; }

            public string Path { get; set; }

            public string Type { get; set; }

            private int _currentpage;
            public int CurrentPage
            {
                get { return _currentpage; }
                set {
                    _currentpage = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("CurrentPage"));
                }
            }

            public int TotalPages { get; set; }

            public string Status { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
                //MessageBox.Show(property);
            }
        }


        //Item class which is used for each entry inside the datagrid Filepicker
        class Item
        {
            public string Name { get; set; }

            public string Path { get; set; }

            public string Type { get; set; }

            public string Icon { get; set; }

            public string Status { get; set; }

            public string FontWeight { get; set; }

            public BitmapImage Thumb { get; set; }
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

            if (z == "File")
            {
                string Path = FilePickerT.SelectedValue.ToString();
                LoadBook(Path);
                MessageBox.Show("Name : " + currentBook.Name + "\n" + "Path : " + currentBook.Path);
               
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
            TopPanel.Items.Refresh();

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