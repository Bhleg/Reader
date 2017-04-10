using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace Reader
{
    /// <summary>
    /// Interaction logic for File_Picker.xaml
    /// </summary>
    public partial class File_Picker : Window
    {
        List<Item> Items = new List<Item>();
        public File_Picker()
        {
            InitializeComponent();
            //List<Item> Items = new List<Item>();
            string DirectoryPath = "C:\\";
            FilePickerT.ItemsSource = Items;
            GetContent(DirectoryPath);
        }

        void GetContent(string Path)
        {
            //List<Item> Items = new List<Item>();
            Items.Clear();
            // Create the List entrie that allow to go up in the driectory path
            string GoUpPath = System.IO.Path.GetDirectoryName(Path);
           if (GoUpPath == null)
           {
               GoUpPath = Path;
           }
            Items.Add(new Item() { Name = "Go up", Path = GoUpPath, Type = "Special" });

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

        }
        class Item
        {
            public string Name { get; set; }

            public string Path { get; set; }

            public string Type { get; set; }
        }
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

    }
}
    

