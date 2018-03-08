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
using System.Linq;

namespace Reader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        Task t = null;
        string currentViewer = "Double";
        string currentViewerOption = "dc";
        public string readingDirection = "Left to Right";

        // list of currently displayed page (usualy 1 or 2)
        public List<int> displayedPages = new List<int>();

        void Viewer(string Action, int Page = 1)
        {
           
            if (currentViewer == "Single")
            {
                SinglePageViewer(Action, Page);
            }
            else if (currentViewer == "Double")
            {
                DoublePageViewer(Action, Page, currentViewerOption);
            }

        }

        void SinglePageViewer(string z, int i = 1)
        {

           // i = currentBook.CurrentPage;
            if (z == "Start")
            {
               // i = 1;
            }
            else if (z == "Next")
            {
                i = currentBook.CurrentPage;
                if (i + 1 <= currentBook.TotalPages) { i = i + 1; }
                else { MessageBox.Show("Last Page !"); }

            }
            else if (z == "Previous")
            {
                i = currentBook.CurrentPage;
                if (i == 1) { MessageBox.Show("First Page !"); }
                else { i = i - 1; }
            }
          //  else if (z == "GoTo") { i = 1; }

            //call the function that only load the needed pdf page
            if (currentBook.Type == ".pdf")
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
            currentBook.CurrentPage = i;

        }

        void DoublePageViewer(string z, int i = 1, string y = "dc")
        {

           // i = currentBook.CurrentPage;


            if (z == "Start")
            {

                
                //call the function that only load the needed pdf page
                if (currentBook.Type == ".pdf")
                {

                    MupdfSharp.Program.GetPdFPage(i);
                    MupdfSharp.Program.GetPdFPage(i + 1);
                    if (t == null || t.IsCompleted)
                    {
                        t = Task.Factory.StartNew(() => { MupdfSharp.Program.GetPdFPageLazy(i); });
                    }
                }

                int ia = i;
                int ib = i + 1;
                BitmapImage a = CreatePage(ia);
                a.Freeze();

                //if last page OR first page in single cover =>  only display one page
                if (ib > currentBook.TotalPages || y == "sc" && i == 1)
                {
                    displayedPages.Clear();
                    displayedPages.Add(ia);
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
                        displayedPages.Clear();
                        displayedPages.Add(ia);
                        displayedPages.Add(ib);
                        // i = ib;      
                }
                // else if (a.Width > a.Height | (a.Width < a.Height && b.Width > b.Height)) GOOD BUT DONT COVER ALL FILE ex: square last page(fansub team)
                else
                {
                    SetSinglePage(a);
                    displayedPages.Clear();
                    displayedPages.Add(ia);
                    a = null;
                    //i = ia;
                }

            }
            else if (z == "Next")
            {
                //i = currentBook.CurrentPage;
                // Get highest page number currently displayed
                i = displayedPages.Max();
                // set the new current page as the last page displayed + 1
                currentBook.CurrentPage = i + 1;

                if (i + 1 <= currentBook.TotalPages)
                {
                    //call the function that only load the needed pdf page
                    if (currentBook.Type == ".pdf")
                    {

                        MupdfSharp.Program.GetPdFPage(i + 1);
                        MupdfSharp.Program.GetPdFPage(i + 2);
                        if (t == null || t.IsCompleted)
                        {
                            t = Task.Factory.StartNew(() => { MupdfSharp.Program.GetPdFPageLazy(i); });
                        }
                    }


                    int ia = i + 1;
                    int ib = i + 2;
                    BitmapImage a = CreatePage(ia);
                    a.Freeze();
                    currentBook.CurrentPage = ia;
                    //if last page only display one page (obviously)
                    if (ia == currentBook.TotalPages)
                    {
                        SetSinglePage(a);
                        a = null;
                        displayedPages.Clear();
                        displayedPages.Add(ia);
                        GC.Collect();
                        // i = ia;
                        // currentBook.CurrentPage = i;
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
                        displayedPages.Clear();
                        displayedPages.Add(ia);
                        displayedPages.Add(ib);
                    }


                    //else if (a.Width > a.Height | (a.Width < a.Height && b.Width > b.Height)) GOOD BUT DONT COVER ALL FILE ex : square last page (fansub team)
                    else
                    {
                        SetSinglePage(a);
                        a = null;
                        displayedPages.Clear();
                        displayedPages.Add(ia);
                        // MessageBox.Show("Page : " + ia.ToString());
                        // i = ia;

                    }

                }
                else
                {
                    SetMetadata(currentBook.Path, "ReadState", "Read");
                    MessageBox.Show("Complete!");
                    return;
                }


            }
            else if (z == "Previous")
            {

               // i = currentBook.CurrentPage;
                i = displayedPages.Min();
                int ib;
                int ia;
                if (currentViewer == "Double")
                {
                    if (i - 2 < 1)
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
                else
                {
                    if (i - 2 < 1)
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
                    displayedPages.Clear();
                    displayedPages.Add(ib);
                    b = null;
                    GC.Collect();
                    // i = ib;
                    currentBook.CurrentPage = ib;
                    return;
                }
                BitmapImage a = CreatePage(ia);
                a.Freeze();

                //Double Page Detection logic
                if (a.Width < a.Height && b.Width < b.Height)
                {
                    SetLeftPage(a);
                    SetRightPage(b);
                    displayedPages.Clear();
                    displayedPages.Add(ib);
                    displayedPages.Add(ia);
                    a = null;
                    b = null;
                    //i = ia;
                    currentBook.CurrentPage = ia;
                }
                else if (b.Width > b.Height | (b.Width < b.Height && a.Width > a.Height))
                {
                    SetSinglePage(b);
                    displayedPages.Clear();
                    displayedPages.Add(ib);
                    b = null;
                    GC.Collect();
                    // i = ib;
                    currentBook.CurrentPage = ib;
                    return;

                }

            }
            //i = currentBook.CurrentPage;
            i = currentBook.CurrentPage;
            //MessageBox.Show(currentBook.CurrentPage.ToString());
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