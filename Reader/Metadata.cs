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

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public void Metadata(string path, string action, string propertie, string value)
        {
            string MetadataPath = System.IO.Path.GetDirectoryName(path) + "/.metadata";
            Directory.CreateDirectory(MetadataPath);


        }
    }
}