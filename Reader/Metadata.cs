﻿using System;
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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public void SetMetadata(string path, string propertie, string value)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string MetadataFile = Path.GetFileName(path);
            string MetadataFolder = String.Concat(Path.GetDirectoryName(path), "/.metadata");
            string MetadataFilePath = Path.ChangeExtension(Path.Combine(MetadataFolder, MetadataFile),".metadata");
            Directory.CreateDirectory(MetadataFolder);
            string line = propertie + " :" + value;

            stopwatch.Stop();

            // Write result.
            MessageBox.Show("Time elapsed: {0}", stopwatch.Elapsed.ToString());


            if (File.Exists(MetadataFilePath))
            {
                string text = File.ReadAllText(MetadataFilePath);

                if (text.Contains(propertie))
                {
                    text = Regex.Replace(text, @"(?i)(?<=^" + propertie + @"\s*?:\s*?)\w*?(?=\s*?$)", value,
                                     RegexOptions.Multiline);
                    File.WriteAllText(MetadataFilePath, text);
                }
                else
                {
                    using (var tw = new StreamWriter(MetadataFilePath, true))
                    {
                        tw.WriteLine(line);
                        tw.Close();
                    }
                }
            }
            else
            {
                using (var tw = new StreamWriter(MetadataFilePath, true))
                {
                    tw.WriteLine(line);
                    tw.Close();
                }
            }
                
                
                          



            


        }
        public string ReadMetadata(string path, string propertie)
        {
            string MetadataFile = Path.GetFileName(path);
            string MetadataFolder = String.Concat(Path.GetDirectoryName(path), "/.metadata");
            string MetadataFilePath = Path.ChangeExtension(Path.Combine(MetadataFolder, MetadataFile), ".metadata");
            Directory.CreateDirectory(MetadataFolder);
            //string line = propertie + " :" + value;


            if (File.Exists(MetadataFilePath))
            {
                string text = File.ReadAllText(MetadataFilePath);

                if (text.Contains(propertie))
                {
                    Match m = Regex.Match(text, @"(?i)(?<=^" + propertie + @"\s*?:\s*?)\w*?(?=\s*?$)");
                    //MessageBox.Show(m.ToString());
                    return m.ToString();
                   // var regex = new Regex(@"(?i)(?<=^" + propertie + @"\s*?:\s*?)\w*?(?=\s*?$)");
                    //File.WriteAllText(MetadataFilePath, text);
                }
                else
                {
                    return "null";
                }
            }
            else
            {
                return "null";
            }
            return "null";
        }
    }
}