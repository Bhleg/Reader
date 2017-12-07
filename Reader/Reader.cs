using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using SharpCompress.Archives;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Task t = null;
        string currentViewer = "Double";
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
                
                i = CurrentPage;
                if (z == "Start") { i = 1; }
                else if (z == "Next")
                {
                    if (i + 1 <= Book.TotalPages) {    i = i + 1; }
                    else { MessageBox.Show("Last Page !");}

                }
                else if (z == "Previous")
                {
                    if (i == 1)  { MessageBox.Show("First Page !"); }
                    else {i = i - 1;}
                }

                //call the function that only load the needed pdf page
                if (Book.Type==".pdf")
                {
                    MupdfSharp.Program.GetPdFPage(i);
                    if (t == null || t.IsCompleted)
                    {
                        t = Task.Factory.StartNew(() => { MupdfSharp.Program.GetPdFPageLazy(i); });
                    }
                }
                


                // set source page to null to make sure that memory has been freed before re-assigning them
                LeftPage.Source = RightPage.Source = SinglePage.Source = null;

                BitmapImage a = CreatePage(i);
                a.Freeze();
                SetSinglePage(a);

                a = null;
               
                GC.Collect();
                CurrentPage = i;
              
            }

            void DoublePageViewer(string z, int i = 0)
            {
                
                i = CurrentPage;

                if (z == "Start")
                {
                    i = 1;

                    int ia = i;
                    int ib = i + 1;
                    BitmapImage a = CreatePage(ia);
                    a.Freeze();

                    //if last page only display one page (obviously)
                    if (ib > Book.TotalPages)
                    {
                        SetSinglePage(a);
                        a = null;
                        GC.Collect();
                        i = ia;
                        return;
                    }
                    BitmapImage b = CreatePage(ib);
                    b.Freeze();

                    //Double Page Detection logic
                    if (a.Width < a.Height && b.Width < b.Height)
                    {
                        SetLeftPage(a);
                        SetRightPage(b);
                        a = null;
                        b = null;
                        i = ib;
                        
                    }
                    else if (a.Width > a.Height | (a.Width < a.Height && b.Width > b.Height))
                    {
                        SetSinglePage(a);
                        a = null;
                        i = ia;

                    }
                    
                }

                else if (z == "Next")
                {
                    if (i + 1 <= Book.TotalPages)
                    {
                        

                        int ia = i + 1;
                        int ib = i + 2;
                        BitmapImage a = CreatePage(ia);
                        a.Freeze();

                        //if last page only display one page (obviously)
                        if (ib > Book.TotalPages)
                        {
                            SetSinglePage(a);
                            a = null;
                            GC.Collect();
                            i = ia;
                            return;
                        }
                        BitmapImage b = CreatePage(ib);
                        b.Freeze();

                        //Double Page Detection logic
                        if (a.Width < a.Height && b.Width < b.Height)
                        {
                            SetLeftPage(a);
                            SetRightPage(b);
                            a = null;
                            b = null;
                            i = ib;
                          //  MessageBox.Show("Page : " + ia.ToString()+" et "+ib.ToString());
                        }
                        else if (a.Width > a.Height | (a.Width < a.Height && b.Width > b.Height))
                        {
                            SetSinglePage(a);
                            a = null;
                           // MessageBox.Show("Page : " + ia.ToString());
                            i = ia;

                        }

                    }
                    else
                    {
                        SetMetadata(Book.Path, "ReadState", "Read");
                        MessageBox.Show("Complete!");
                        return;
                    }


                }
                else if (z == "Previous")
                {
                                        
                   
                        

                        int ib;
                        int ia;
                        if (currentViewer=="Double")
                        {
                            if (i - 2 < 1)
                            {
                                MessageBox.Show("First Page!");
                                return;
                            }
                            else
                            {
                                ib = i - 2;
                                ia = i - 3;
                            }
                        }
                        else
                        {
                            if (i - 1 < 1)
                            {
                                MessageBox.Show("First Page!");
                                return;
                            }
                            else
                            {
                                ib = i - 1;
                                ia = i - 2;
                            }
                        }

                    // set source page to null to make sure that memory has been freed before re-assigning them
                    LeftPage.Source = RightPage.Source = SinglePage.Source = null;

                    BitmapImage b = CreatePage(ib);
                        b.Freeze();

                        // if last page only display one page (obviously)
                        if (ia <= 0)
                        {
                            SetSinglePage(b);
                            b = null;
                            GC.Collect();
                            i = ib;
                            CurrentPage = i;
                            return;
                        }
                        BitmapImage a = CreatePage(ia);
                        a.Freeze();

                        //Double Page Detection logic
                        if (a.Width < a.Height && b.Width < b.Height)
                        {
                            SetLeftPage(a);
                            SetRightPage(b);
                            a = null;
                            b = null;
                            i = ib;
                        }
                        else if (b.Width > b.Height | (b.Width < b.Height && a.Width > a.Height))
                        {
                            SetSinglePage(b);
                            b = null;
                            i = ib;

                        }
                    
                }

                

                //call the function that only load the needed pdf page
                if (Book.Type == ".pdf")
                {

                    MupdfSharp.Program.GetPdFPage(i);
                    MupdfSharp.Program.GetPdFPage(i+1);
                    if (t == null || t.IsCompleted)
                    {
                        t = Task.Factory.StartNew(() => { MupdfSharp.Program.GetPdFPageLazy(i); });
                    }
                }


               
                
                GC.Collect();
              
                CurrentPage = i;
                


            }

            void SetLeftPage(BitmapImage Image)
            {
                LeftPage.Visibility = Visibility.Visible;
                RightPage.Visibility = Visibility.Visible;
                SinglePage.Visibility = Visibility.Collapsed;
                Image.Freeze();
                LeftPage.Source = Image;
                Image = null;
                currentViewer = "Double";
            }

            void SetRightPage(BitmapImage Image)
            {
                LeftPage.Visibility = Visibility.Visible;
                RightPage.Visibility = Visibility.Visible;
                SinglePage.Visibility = Visibility.Collapsed;
                Image.Freeze();
                RightPage.Source = Image;
                Image = null;
                currentViewer = "Double";

            }

            void SetSinglePage(BitmapImage Image)
            {
                LeftPage.Visibility = Visibility.Collapsed;
                RightPage.Visibility = Visibility.Collapsed;
                SinglePage.Visibility = Visibility.Visible;
                Image.Freeze();
                SinglePage.Source = Image;
                Image = null;
                currentViewer = "Single";

            }

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


        }
    }
 }