using System;
using System.Collections.Generic;
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
        string DefaultPath = "C:\\";
        List<Item> Items = new List<Item>();
        public File_Picker()
        {
            InitializeComponent();
            //List<Item> Items = new List<Item>();
            FilePickerT.ItemsSource = Items;
            GetContent(DefaultPath);



        }


        //The Function wich is used to get the content of a folder(Path) and fill the Items list with the content of the folder(Path)
        void GetContent(string Path)
        {
            //List<Item> Items = new List<Item>();
            Items.Clear();


            // Process the list of files found in the directory.
            string[] FileEntries = Directory.GetFiles(Path).Where(s => s.EndsWith(".cbz") || s.EndsWith("cbr") || s.EndsWith("zip") || s.EndsWith("rar")).ToArray();
            foreach (string FilePath in FileEntries)
            {

                string FileName = System.IO.Path.GetFileName(FilePath);
                Items.Add(new Item() { Name = FileName, Path = FilePath, Type = "File" });
                //ProcessFile(fileName);
            }


            // Process the list of directory found in the directory.
            string[] DirectoryEntries = Directory.GetDirectories(Path);
            foreach (string DirectoriePath in DirectoryEntries)
            {
                string DirectorieName = System.IO.Path.GetFileName(DirectoriePath);
                Items.Add(new Item() { Name = DirectorieName, Path = DirectoriePath, Type = "Folder" });
            }
            CurrentPath = Path;

        }


        public void CreateLibrary()
        {
            InitializeComponent();
            string DirectorieName = System.IO.Path.GetFileName(CurrentPath);
            Button bn = new Button();
            bn.Name = "btn" + DirectorieName;
            bn.Content = DirectorieName;
            bn.Tag = CurrentPath;
          
            bn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MyClick2);
    



            BookmarkPanel.Children.Add(new Button { Content = DirectorieName });
            
        }

        void GenerateLibrary()
        {
            BookmarkPanel.Children.Add(new Button { Content = "Button" });
        }
        
        //Item class which is used for each entry inside the datagris Filepicker
        class Item
        {
            public string Name { get; set; }

            public string Path { get; set; }

            public string Type { get; set; }
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
                m.FileLoader(Path);
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
        private void CreateLibrary_Event(object sender, EventArgs e)
        {
            CreateLibrary();
        }

        protected void MyClick2(object sender, EventArgs e)
        {
            Button bn = sender as Button;
            MessageBox.Show("hello");
        }

    }
}
    

