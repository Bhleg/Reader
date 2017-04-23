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
        void Viewer(string ViewerType, string Action, int Page = 0)
        {

            if (ViewerType == "Single")
            {
                SinglePageViewer(Action);
            }
            else if (ViewerType == "Double")
            {
                DoublePageViewer(Action);
            }

            void SinglePageViewer(string z, int i = 0)
            {
                DoubePage.Visibility = Visibility.Collapsed;
                SinglePage.Visibility = Visibility.Visible;
                i = CurrentPage;
                if (z == "Load")
                {
                    i = 1;
                    
                    

                }
                else if (z == "Next")
                {
                    if (i + 1 <= TotalPages)
                    {
                        i = i + 1;
                    }
                    else
                    {
                        MessageBox.Show("Last Page !");
                    }

                }
                else if (z == "Previous")
                {
                    if (i == 1)
                    {
                        MessageBox.Show("First Page !");
                    }
                    else
                    {
                        i = i - 1;
                    }
                }

               var memoryStream = new MemoryStream(Pages[i]);
               memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapImage a = new BitmapImage();
                a.BeginInit();
                a.StreamSource = memoryStream;
                a.CacheOption = BitmapCacheOption.OnLoad;
                a.EndInit();
                SinglePage.Source = a;
                CurrentPage = i;

            }

            void DoublePageViewer(string z, int i = 0)
            {

                i = CurrentPage;


                if (z == "Start")
                {
                    i = 1;

                }
                else if (z == "Next")
                {
                    if (i + 1 == TotalPages)
                    {

                        return;

                    }
                    i = i + 1;
                    //MessageBox.Show(i.ToString());

                }
                else if (z == "Previous")
                {
                    if (i <= 2)
                    {

                        return;

                    }
                    else
                    {
                        i = i - 2;
                    }
                }

                
                LeftPage.Source = null;
                RightPage.Source = null;
                SinglePage.Source = null;
                

                int ia = i;
                int ib = i + 1;

                BitmapImage a = CreatePage(ia);
                BitmapImage b = CreatePage(ib);
                a.Freeze();
                b.Freeze();

                if (a.Width < a.Height && b.Width < b.Height)
                {
                    SetLeftPage(a);
                    SetRightPage(b);
                    i = i + 1;
                }
                else if (a.Width > a.Height | (a.Width < a.Height && b.Width > b.Height))
                {
                    SetSinglePage(a);

                }


                a = null;
                b = null;
                GC.Collect();
                CurrentPage = i;
                //MessageBox.Show("i = " + i + "\n" + CurrentPage.ToString() + " sur " + TotalPages.ToString() + " Pages");



                BitmapImage CreatePage(int p)
                {

                    using (var memoryStream = new MemoryStream(Pages[p]))
                    using (WrappingStream wrapper = new WrappingStream(memoryStream))
                    {
                        BitmapImage Image = new BitmapImage();
                        Image.BeginInit();
                        Image.StreamSource = wrapper;
                        Image.CacheOption = BitmapCacheOption.OnLoad;
                        Image.DecodePixelWidth = Convert.ToInt32(FilePickerT.ActualWidth);
                        Image.EndInit();
                        Image.Freeze();
                        return Image;

                    }
                    
                }

                void SetLeftPage(BitmapImage Image)
                {
                    LeftPage.Visibility = Visibility.Visible;
                    RightPage.Visibility = Visibility.Visible;
                    SinglePage.Visibility = Visibility.Collapsed;
                    LeftPage.Source = Image;

                }

                void SetRightPage(BitmapImage Image)
                {
                    LeftPage.Visibility = Visibility.Visible;
                    RightPage.Visibility = Visibility.Visible;
                    SinglePage.Visibility = Visibility.Collapsed;
                    RightPage.Source = Image;


                }

                void SetSinglePage(BitmapImage Image)
                {
                    LeftPage.Visibility = Visibility.Collapsed;
                    RightPage.Visibility = Visibility.Collapsed;
                    SinglePage.Visibility = Visibility.Visible;
                    SinglePage.Source = Image;

                }


            }



        }
    }
 }