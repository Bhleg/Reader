using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Threading.Tasks;

namespace MupdfSharp
{
	public class Program
	{
        static public void GetPdF(string path)
        {
            IntPtr ctx = NativeMethods.NewContext(); // Creates the context
            IntPtr stm = NativeMethods.OpenFile(ctx, path); // opens file test.pdf as a stream
            IntPtr doc = NativeMethods.OpenDocumentStream(ctx, stm); // opens the document
            int pn = NativeMethods.CountPages(doc); // gets the number of pages in the document
            Reader.MainWindow.Book.TotalPages = NativeMethods.CountPages(doc);
            for (int i = 1; i < pn; i++)
            { // iterate through each pages
                //Console.WriteLine("Rendering page " + (i + 1));
                IntPtr p = NativeMethods.LoadPage(doc, i); // loads the page (first page number is 1)
                Rectangle b = new Rectangle();
                NativeMethods.BoundPage(doc, p, ref b); // gets the page size
                
                
                var bmp = RenderPage(ctx, doc, p, b);  // renders the page and converts the result to Bitmap

                byte[] bi = StreamFromBitmapSource(bmp).ToArray();
                Reader.MainWindow.Pages.Add(i, bi);

                NativeMethods.FreePage(doc, p); // releases the resources consumed by the page
                GC.Collect();
            }
            NativeMethods.CloseDocument(doc); // releases the resources
            NativeMethods.CloseStream(stm);
            NativeMethods.FreeContext(ctx);
            // Console.WriteLine("Program finished. Press any key to quit.");
            // Console.ReadKey(true);
            MemoryStream StreamFromBitmapSource(BitmapSource writeBmp)
            {
                MemoryStream bmp = new MemoryStream();

                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(writeBmp));
                enc.Save(bmp);

                return bmp;
            }
            //return;
        }

        static public void GetPdFPageLazy(int page)
        {
            int a = page-1;
            int b = page + 2;
            if (a<1)
            {
                a = 1;
            }
            if (b > Reader.MainWindow.Book.TotalPages)
            {
                b = Reader.MainWindow.Book.TotalPages;
            }

             

            List<int>Keylist = new List<int>();
            foreach (int key in Reader.MainWindow.Pages.Keys)
            {
                Keylist.Add(key);
            }



            foreach (var item in Keylist)
            {
                if (!IsBetween(item,a,b))
                {
                    Reader.MainWindow.Pages.Remove(item);
                }
            }


            for (int i = a; i <= b; i++)
            { 
                if (!Reader.MainWindow.Pages.ContainsKey(i))
                {
                    int z = i-1; // account for the fact that NativeMethods.CountPages start counting at 0 (Page number start at 1)
                    IntPtr p = NativeMethods.LoadPage(PDFBook.doc, z); // loads the page (first page number is 1)
                    Rectangle r = new Rectangle();
                    NativeMethods.BoundPage(PDFBook.doc, p, ref r); // gets the page size


                    var bmp = RenderPage(PDFBook.ctx, PDFBook.doc, p, r);  // renders the page and converts the result to Bitmap
                    bmp.Freeze();
                    
                    byte[] bi = StreamFromBitmapSource(bmp).ToArray();
                    //if (!Reader.MainWindow.Pages.ContainsKey(i))
                    //{
                  //      Reader.MainWindow.Pages.Add(i, bi);
                  //  }

                   Reader.MainWindow.Pages.Add(i, bi);
                    bmp = null;
                    bi = null;

                    NativeMethods.FreePage(PDFBook.doc, p); // releases the resources consumed by the page
                }
                else
                {
                    continue;
                }
             
                
            }
            GC.Collect();
            // NativeMethods.CloseDocument(PDFBook.doc); // releases the resources
            // NativeMethods.CloseStream(PDFBook.stm);
            // NativeMethods.FreeContext(PDFBook.ctx);
            // Console.WriteLine("Program finished. Press any key to quit.");
            // Console.ReadKey(true);
            bool IsBetween(int i,int a1,int a2)
            {
                if (i>=a1 && i<=a2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            MemoryStream StreamFromBitmapSource(BitmapSource writeBmp)
            {
                using (MemoryStream bmp = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(writeBmp));
                    enc.Save(bmp);

                    return bmp;
                }

                
            }
            //return;
        }

        static public void GetPdFPage(int page)
        {
            
                if (!Reader.MainWindow.Pages.ContainsKey(page))
                {
                    int z = page - 1; // account for the fact that NativeMethods.CountPages start counting at 0 (Page number start at 1)
                    IntPtr p = NativeMethods.LoadPage(PDFBook.doc, z); // loads the page (first page number is 1)
                    Rectangle r = new Rectangle();
                    NativeMethods.BoundPage(PDFBook.doc, p, ref r); // gets the page size


                    var bmp = RenderPage(PDFBook.ctx, PDFBook.doc, p, r);  // renders the page and converts the result to Bitmap
                    bmp.Freeze();
                    byte[] bi = StreamFromBitmapSource(bmp).ToArray();
                    if (!Reader.MainWindow.Pages.ContainsKey(page))
                    {
                        Reader.MainWindow.Pages.Add(page, bi);
                    }
                    
                    bmp = null;
                    bi = null;

                    NativeMethods.FreePage(PDFBook.doc, p); // releases the resources consumed by the page
                }
                else
                {
                GC.Collect();
                return;
                }
             
            
            GC.Collect();
            MemoryStream StreamFromBitmapSource(BitmapSource writeBmp)
            {
                using (MemoryStream bmp = new MemoryStream())
                {
                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(writeBmp));
                    enc.Save(bmp);

                    return bmp;
                }


            }
            
        }

        static bool IsTrue()
        {
            bool True = true;
            return True;

        }
        static public void LoadPDF(string path)
        {
            PDFBook.ctx = NativeMethods.NewContext(); // Creates the context
            PDFBook.stm = NativeMethods.OpenFile(PDFBook.ctx, path); // opens file test.pdf as a stream
            PDFBook.doc = NativeMethods.OpenDocumentStream(PDFBook.ctx, PDFBook.stm); // opens the document
            PDFBook.TotalPage = NativeMethods.CountPages(PDFBook.doc); // gets the number of pages in the document
        }

        static public BitmapSource RenderPage (IntPtr context, IntPtr document, IntPtr page, Rectangle pageBound) {
			Matrix ctm = new Matrix ();
			IntPtr pix = IntPtr.Zero;
			IntPtr dev = IntPtr.Zero;

            float zoomX = 350 / 72;
            float zoomY = 350 / 72;

            int width = (int)(zoomX * (pageBound.Right - pageBound.Left)); // gets the size of the scaled page
            int height = (int)(zoomY * (pageBound.Bottom - pageBound.Top));
            ctm.A = zoomX;
            ctm.D = zoomY; // sets the matrix as (zoomX,0,0,zoomY,0,0) 

           // int width = (int)(pageBound.Right - pageBound.Left); // gets the size of the page
			//int height = (int)(pageBound.Bottom - pageBound.Top);

            const int depth = 24;
            int bmpstride = ((width * depth + 31) & ~31) >> 3;

           // ctm.A = ctm.D = 1; // sets the matrix as the identity matrix (1,0,0,1,0,0)

			// creates a pixmap the same size as the width and height of the page
			pix = NativeMethods.NewPixmap (context, NativeMethods.LookupDeviceColorSpace (context, "DeviceRGB"), width, height);
			// sets white color as the background color of the pixmap
           
			NativeMethods.ClearPixmap (context, pix, 0xFF);

			// creates a drawing device
			dev = NativeMethods.NewDrawDevice (context, pix);
            
			// draws the page on the device created from the pixmap
			NativeMethods.RunPage (document, page, dev, ref ctm, IntPtr.Zero);

			NativeMethods.FreeDevice (dev); // frees the resources consumed by the device
			dev = IntPtr.Zero;

            
			// creates a colorful bitmap of the same size of the pixmap
			//Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            //bmp.SetResolution(120, 120);
            //bmp.SetResolution(150, 150);
            WriteableBitmap write = new WriteableBitmap(width, height, 120, 120, System.Windows.Media.PixelFormats.Bgr24, null);
           
            //write.Lock();
           // var imageData = bmp.LockBits (new System.Drawing.Rectangle (0, 0, width, height), ImageLockMode.ReadWrite, bmp.PixelFormat);
			unsafe { // converts the pixmap data to Bitmap data


                byte* ptrSrc = (byte*)NativeMethods.GetSamples (context, pix); // gets the rendered data from the pixmap
                                                                               //byte* ptrDest = (byte*)imageData.Scan0;
                byte* ptrDest = (byte*)write.BackBuffer;
                for (int y = 0; y < height; y++) {
					byte* pl = ptrDest;
					byte* sl = ptrSrc;
					for (int x = 0; x < width; x++) {
						//Swap these here instead of in MuPDF because most pdf images will be rgb or cmyk.
						//Since we are going through the pixels one by one anyway swap here to save a conversion from rgb to bgr.
						pl[2] = sl[0]; //b-r
						pl[1] = sl[1]; //g-g
						pl[0] = sl[2]; //r-b
						//sl[3] is the alpha channel, we will skip it here
						pl += 3;
						sl += 4;
					}
					ptrDest += bmpstride;
					ptrSrc += width * 4;
				}
                
                
            }
			NativeMethods.DropPixmap (context, pix);

            
            //return bmp;
            write.Freeze();
            return write;
		}

        public static class PDFBook
        {
            public static IntPtr ctx { get; set; }

            public static IntPtr stm { get; set; }

            public static IntPtr doc { get; set; }

            public static int TotalPage { get; set; }

        }

    }

		public class NativeMethods
		{
			const uint FZ_STORE_DEFAULT = 256 << 20;
			const string DLL = "libmupdf.dll";
			// please modify the version number to match the FZ_VERSION definition in "fitz\version.h" file
			const string MuPDFVersion = "1.6";

			[DllImport (DLL, EntryPoint = "fz_new_context_imp", CallingConvention = CallingConvention.Cdecl)]
			static extern IntPtr NewContext (IntPtr alloc, IntPtr locks, uint max_store, string version);
			public static IntPtr NewContext () {
				return NewContext (IntPtr.Zero, IntPtr.Zero, FZ_STORE_DEFAULT, MuPDFVersion);
			}

			[DllImport (DLL, EntryPoint = "fz_free_context", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr FreeContext (IntPtr ctx);

			[DllImport (DLL, EntryPoint = "fz_open_file_w", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr OpenFile (IntPtr ctx, string fileName);

			[DllImport (DLL, EntryPoint = "pdf_open_document_with_stream", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr OpenDocumentStream (IntPtr ctx, IntPtr stm);

			[DllImport (DLL, EntryPoint = "fz_close", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr CloseStream (IntPtr stm);

			[DllImport (DLL, EntryPoint = "pdf_close_document", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr CloseDocument (IntPtr doc);

			[DllImport (DLL, EntryPoint = "pdf_count_pages", CallingConvention = CallingConvention.Cdecl)]
			public static extern int CountPages (IntPtr doc);

			[DllImport (DLL, EntryPoint = "pdf_bound_page", CallingConvention = CallingConvention.Cdecl)]
			public static extern void BoundPage (IntPtr doc, IntPtr page, ref Rectangle bound);

			[DllImport (DLL, EntryPoint = "fz_clear_pixmap_with_value", CallingConvention = CallingConvention.Cdecl)]
			public static extern void ClearPixmap (IntPtr ctx, IntPtr pix, int byteValue);

			[DllImport (DLL, EntryPoint = "fz_lookup_device_colorspace", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr LookupDeviceColorSpace (IntPtr ctx, string colorspace);

			[DllImport (DLL, EntryPoint = "fz_free_device", CallingConvention = CallingConvention.Cdecl)]
			public static extern void FreeDevice (IntPtr dev);

			[DllImport (DLL, EntryPoint = "pdf_free_page", CallingConvention = CallingConvention.Cdecl)]
			public static extern void FreePage (IntPtr doc, IntPtr page);

			[DllImport (DLL, EntryPoint = "pdf_load_page", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr LoadPage (IntPtr doc, int pageNumber);

			[DllImport (DLL, EntryPoint = "fz_new_draw_device", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr NewDrawDevice (IntPtr ctx, IntPtr pix);

			[DllImport (DLL, EntryPoint = "fz_new_pixmap", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr NewPixmap (IntPtr ctx, IntPtr colorspace, int width, int height);

			[DllImport (DLL, EntryPoint = "pdf_run_page", CallingConvention = CallingConvention.Cdecl)]
			public static extern void RunPage (IntPtr doc, IntPtr page, IntPtr dev, ref Matrix transform, IntPtr cookie);

			[DllImport (DLL, EntryPoint = "fz_drop_pixmap", CallingConvention = CallingConvention.Cdecl)]
			public static extern void DropPixmap (IntPtr ctx, IntPtr pix);

			[DllImport (DLL, EntryPoint = "fz_pixmap_samples", CallingConvention = CallingConvention.Cdecl)]
			public static extern IntPtr GetSamples (IntPtr ctx, IntPtr pix);

		}
	
}
