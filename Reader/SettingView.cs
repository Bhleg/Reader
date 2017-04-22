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
        void Setting()
        {
            
             List<string> LybraryList = new List<string>();
             foreach (string item in Properties.Settings.Default.Library)
             {

             }

            
        }
    }

}
