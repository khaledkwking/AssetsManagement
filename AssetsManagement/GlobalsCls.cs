using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BOL;
using PagedList;
using nsAlienRFID2;
using MessagingToolkit.QRCode.Codec;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using System.Runtime.InteropServices;
//using ZXing;
//using ZXing.Common;
using System.Drawing.Printing;

using System.Windows.Forms;

namespace AssetsManagement
{

    public class GlobalsCls
    {
        private UnitOfWork unitWork = new UnitOfWork();
        public GlobalsCls()
        {

        }
       
        // reader variables
        enum eReaderType
        {
            x780,
            x800,
            x900
        }
        eReaderType meReaderType = eReaderType.x900;
        private clsReader mReader;

        private ReaderInfo mReaderInfo;
        private ComInterface meReaderInterface = ComInterface.enumTCPIP;
        private bool mbDisposing = false;
        private decimal mSavedNUDvalue = 0;
        private string msLastTagID = null;
        private const string G2MASK = "'bank#' as deximal, 'bitPtr' as decimal, 'bitLen' as decimal, 'bytes' as hexadecimal";
        private const string C1MASK = "'bitPtr' as decimal, 'bitLen' as decimal, 'bytes' as hexadecimal";
        private String msTags;
        //-------------------------------end of reader variables---------------------------------------

        public static string LangMenuDir(string SiteLanguage)
        {
            string MenuDir = "";
            if (SiteLanguage == "EnglishTheme")
            {
                MenuDir = "dropdown-menu dropdown-menu-Left";
            }
            else
            {
                MenuDir = "dropdown-menu dropdown-menu-right";
            }
            return MenuDir;

        }
        public string DisconnectReader()
        {
            return  mReader.Disconnect();
        }
        public bool Connect(string IpAddress, bool TcpFlag, ref string ReaderStatus)
        {
            if (mReader == null)
            {
                mReader = new clsReader();
            }
            else
            {
                mReader.Disconnect();
            }

            bool ret = false;
            //      mReader.Disconnect();
            String result;

            bool TCPFlag = true;
            bool serailFlag = false;
            int PortNum = 1;
            int PortUD = 23;
            ReaderStatus = "";
            try		// extra precausion though it shouldn't throw exceptions
            {

                string ReaderUserName = Properties.Settings.Default.ReaderUsername;
                string ReaderPassword = Properties.Settings.Default.ReaderPassword;

                if (!mReader.IsConnected)
                {
                    if (Properties.Settings.Default.TCP == TcpFlag)
                    {
                        TCPFlag = true;
                        //IpAddress // Properties.Settings.Default.IPAddress;
                        meReaderInterface = ComInterface.enumTCPIP;
                    }
                    else if (Properties.Settings.Default.Serial == true)
                    {
                        serailFlag = true;
                        PortNum = Properties.Settings.Default.potNumber - 1;
                        meReaderInterface = ComInterface.enumSerial;
                    }

                    if (meReaderInterface == ComInterface.enumTCPIP)
                    {
                        PortUD = Properties.Settings.Default.PortUD;
                        mReader.InitOnNetwork(IpAddress, Convert.ToInt32(PortUD));
                    }
                    else
                        mReader.InitOnCom();
                    ReaderStatus = "Connecting to the reader...";
                    result = mReader.Connect();
                    if (mReader.IsConnected)
                    {
                        if (meReaderInterface == ComInterface.enumTCPIP)
                        {
                            ReaderStatus = "Logging in...";

                            if (!mReader.Login(ReaderUserName, ReaderPassword))		//returns result synchronously
                            {
                                ReaderStatus = "Login failed! Calling Disconnect()...";
                                mReader.Disconnect();
                                return ret;				//------------>
                            }
                        }
                        //ManageGUI(true);
                        // to make it faster and not to lose any tag
                        mReader.AutoMode = "On";
                    }
                    ReaderStatus = result;

                }
                ret = true;

            }
            catch (Exception ex)
            {
                ReaderStatus = "Exception in btnConnect_Click(): " + ex.Message;
                ret = false;
            }
            return ret;
        }
        public List<tbl_ItemsStock> ParsTages(long? StoreId)
        {
            string ReadStatus = "";
            int KR = 1;
            // 1- read All tags the Reader See
            List<tbl_ItemsStock> ItemsStockListAll = new List<tbl_ItemsStock>();
            if (mReader.IsConnected)
            {
                mReader.TagListFormat = "Text";
                //mReader.TagListCustomFormat = "%i, %a, %D %T, %p, %l";
                string result = null;
                try
                {
                    result = mReader.TagList;
                }
                catch (Exception ex)
                {
                    ReadStatus = ex.Message;
                }
                if ((result != null) &&
                    (result.Length > 0) &&
                    (result.IndexOf("No Tags") == -1))
                    msTags = result;
                ////////////////- 2 start to pas and assign


                if ((msTags != null) && (msTags.Length > 0))
                {
                    TagInfo[] aTags;
                    try
                    {

                        int cnt = AlienUtils.ParseTagList(msTags, out aTags);
                        if (cnt == 0)
                        {
                            ReadStatus = Resources.DefaultResource.ReadNoItemsFound;
                        }
                        else
                        {

                            //malTags = new ArrayList(aTags);
                            // aTags[0].GetField(aTags[0].TagID);
                            for (int i = 0; i < aTags.Length; i++)
                            {
                                // here return list of Rfid Tage at once
                                //   string RFID = "3000" + aTags[i].TagID.Replace(" ","");
                                if (aTags[i].TagDataArray != null)
                                {
                                    string RFID = aTags[i].TagDataArray[0].Replace(" ", "");
                                    //string CRC = aTags[i].TagCRC;
                                    //DataRow[] d = DT.Select("Item_RFID like " + "'" + RFID + "'");
                                    List<tbl_ItemsStock> ItemsStockList = unitWork.ItemsStockManager.GetNotDelAllLike(RFID, StoreId);
                                    if (ItemsStockList.Count > 0 && ItemsStockList != null)
                                    {
                                        List<tbl_ItemsStock> RowsStockList = ItemsStockListAll.Where(c => c.Item_RFID.ToUpper()== RFID.ToUpper ()).ToList();

                                        //DataRow[] rows = DTemp.Select(" RFID  like " + "'" + RFID + "'");
                                        if (RowsStockList.Count() == 0)
                                        {
                                            ItemsStockListAll.Add(ItemsStockList.FirstOrDefault());

                                            //int count = 0;
                                            //if (d[0][3].ToString() != string.Empty && d[0][3] != null)
                                            //{
                                            //    count = Convert.ToInt32(d[0][3].ToString());
                                            //}
                                            //else
                                            //{ count = 0; }
                                            //DTemp.Rows.Add(DTemp.Rows.Count + 1, d[0][0].ToString(), d[0][1].ToString(), d[0][2].ToString(), count, d[0][4].ToString(), false, d[0]["Item_OtheroutPut"]);

                                        }
                                        else
                                        {
                                            //  KR = KR - 1;
                                        }
                                        RFID = "";
                                        KR = ItemsStockListAll.Count + 1;
                                        //LblActulaitem.Text = (KR - 1).ToString();
                                    }
                                }
                                else
                                {
                                    //    MessageBox.Show(aTags[i].TagID.ToString());
                                }
                            }

                            KR = ItemsStockListAll.Count + 1;
                            if (cnt == 1)
                            {
                                ReadStatus = Resources.DefaultResource.ReadOneItem;
                            }
                            else
                            {
                                ReadStatus = Resources.DefaultResource.ReadNofItems;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        // e.g.: in case of malformed XML
                        //    MessageBox.Show(XX.ToString()+"Tag:-"+CC);
                        ReadStatus = "Exception parsing tag list " + ex.Message;
                    }

                }
            }
            return ItemsStockListAll;

        }
        public List<tbl_ItemsStock> ParsInOrderTages(long ? StoreId, bool NewItemOnly)
        {
            string ReadStatus = "";
            int KR = 1;
            // 1- read All tags the Reader See
            List<tbl_ItemsStock> ItemsStockListAll = new List<tbl_ItemsStock>();
            if (mReader.IsConnected)
            {
                mReader.TagListFormat = "Text";
                //mReader.TagListCustomFormat = "%i, %a, %D %T, %p, %l";
                string result = null;
                try
                {
                    result = mReader.TagList;
                }
                catch (Exception ex)
                {
                    ReadStatus = ex.Message;
                }
                if ((result != null) &&
                    (result.Length > 0) &&
                    (result.IndexOf("No Tags") == -1))
                    msTags = result;
                ////////////////- 2 start to pas and assign


                if ((msTags != null) && (msTags.Length > 0))
                {
                    TagInfo[] aTags;
                    try
                    {
                        int cnt = AlienUtils.ParseTagList(msTags, out aTags);
                        if (cnt == 0)
                        {
                            ReadStatus = Resources.DefaultResource.ReadNoItemsFound;
                        }
                        else
                        {

                            //malTags = new ArrayList(aTags);
                            // aTags[0].GetField(aTags[0].TagID);
                            for (int i = 0; i < aTags.Length; i++)
                            {
                                // here return list of Rfid Tage at once
                                //   string RFID = "3000" + aTags[i].TagID.Replace(" ","");
                                if (aTags[i].TagDataArray != null) // loop for every item in the list
                                {
                                    string RFID = aTags[i].TagDataArray[0].Replace(" ", "");
                                    //string CRC = aTags[i].TagCRC;
                                    //DataRow[] d = DT.Select("Item_RFID like " + "'" + RFID + "'");
                                    List<tbl_ItemsStock> ItemsStockList = unitWork.ItemsStockManager.GetNotDelAllLike(RFID, StoreId);
                                    
                                    if (ItemsStockList.Count > 0 && ItemsStockList != null )
                                    {
                                        if (!NewItemOnly)
                                        {
                                            List<tbl_ItemsStock> RowsStockList = ItemsStockListAll.Where(c => c.Item_RFID.ToUpper() == RFID.ToUpper()).ToList();

                                            //DataRow[] rows = DTemp.Select(" RFID  like " + "'" + RFID + "'");
                                            if (RowsStockList.Count == 0)
                                            {
                                                if (ItemsStockList.FirstOrDefault().Item_tbl.CountableFlag)
                                                {
                                                    ItemsStockListAll.Add(ItemsStockList.FirstOrDefault());
                                                }
                                            }
                                            else
                                            {
                                                //  KR = KR - 1;
                                            }
                                        }
                                        //LblActulaitem.Text = (KR - 1).ToString();
                                    }
                                    else 
                                    {
                                        tbl_ItemsStock Item = new tbl_ItemsStock();
                                        Item.Item_RFID = RFID;
                                        ItemsStockListAll.Add(Item);
                                    }
                                    RFID = "";
                                    KR = ItemsStockListAll.Count + 1;

                                }
                                else
                                {
                                    //    MessageBox.Show(aTags[i].TagID.ToString());
                                }
                            }

                            KR = ItemsStockListAll.Count + 1;
                            if (cnt == 1)
                            {
                                ReadStatus = Resources.DefaultResource.ReadOneItem;
                            }
                            else
                            {
                                ReadStatus = Resources.DefaultResource.ReadNofItems;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        // e.g.: in case of malformed XML
                        //    MessageBox.Show(XX.ToString()+"Tag:-"+CC);
                        ReadStatus = "Exception parsing tag list " + ex.Message;
                    }

                }
            }
            return ItemsStockListAll;

        }

        public List<tbl_ItemsStock> ParsHandOverInOrderTages(long? StoreId)
        {
            string ReadStatus = "";
            int KR = 1;
            // 1- read All tags the Reader See
            List<tbl_ItemsStock> ItemsStockListAll = new List<tbl_ItemsStock>();
            if (mReader.IsConnected)
            {
                mReader.TagListFormat = "Text";
                //mReader.TagListCustomFormat = "%i, %a, %D %T, %p, %l";
                string result = null;
                try
                {
                    result = mReader.TagList;
                }
                catch (Exception ex)
                {
                    ReadStatus = ex.Message;
                }
                if ((result != null) &&
                    (result.Length > 0) &&
                    (result.IndexOf("No Tags") == -1))
                    msTags = result;
                ////////////////- 2 start to pas and assign


                if ((msTags != null) && (msTags.Length > 0))
                {
                    TagInfo[] aTags;
                    try
                    {
                        int cnt = AlienUtils.ParseTagList(msTags, out aTags);
                        if (cnt == 0)
                        {
                            ReadStatus = Resources.DefaultResource.ReadNoItemsFound;
                        }
                        else
                        {

                            //malTags = new ArrayList(aTags);
                            // aTags[0].GetField(aTags[0].TagID);
                            for (int i = 0; i < aTags.Length; i++)
                            {
                                // here return list of Rfid Tage at once
                                //   string RFID = "3000" + aTags[i].TagID.Replace(" ","");
                                if (aTags[i].TagDataArray != null)
                                {
                                    string RFID = aTags[i].TagDataArray[0].Replace(" ", "");
                                    //string CRC = aTags[i].TagCRC;
                                    //DataRow[] d = DT.Select("Item_RFID like " + "'" + RFID + "'");
                                    List<tbl_ItemsStock> ItemsStockList = unitWork.ItemsStockManager.GetNotDelOutsideStore(RFID, StoreId);
                                    if (ItemsStockList.Count > 0 && ItemsStockList != null)
                                    {
                                        List<tbl_ItemsStock> RowsStockList = ItemsStockListAll.Where(c => c.Item_RFID.ToUpper() == RFID.ToUpper()).ToList();

                                        //DataRow[] rows = DTemp.Select(" RFID  like " + "'" + RFID + "'");
                                        if (RowsStockList.Count() == 0)
                                        {
                                            if (!ItemsStockList.FirstOrDefault().Item_tbl.CountableFlag)
                                            {
                                                ItemsStockListAll.Add(ItemsStockList.FirstOrDefault());
                                            }
                                        }
                                        else
                                        {
                                            //  KR = KR - 1;
                                        }

                                        //LblActulaitem.Text = (KR - 1).ToString();
                                    }
                                    else
                                    {
                                        //tbl_ItemsStock Item = new tbl_ItemsStock();
                                        //Item.Item_RFID = RFID;
                                        //ItemsStockListAll.Add(Item);
                                    }
                                    RFID = "";
                                    KR = ItemsStockListAll.Count + 1;

                                }
                                else
                                {
                                    //    MessageBox.Show(aTags[i].TagID.ToString());
                                }
                            }

                            KR = ItemsStockListAll.Count + 1;
                            if (cnt == 1)
                            {
                                ReadStatus = Resources.DefaultResource.ReadOneItem;
                            }
                            else
                            {
                                ReadStatus = Resources.DefaultResource.ReadNofItems;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        // e.g.: in case of malformed XML
                        //    MessageBox.Show(XX.ToString()+"Tag:-"+CC);
                        ReadStatus = "Exception parsing tag list " + ex.Message;
                    }

                }
            }
            return ItemsStockListAll;

        }

        public static byte[] GetQRCodeBytes(string url)
        {
            ////generate and display QR Code and pin
            //string encodeQR = rollCallID.ToString();
            QRCodeEncoder encoder = new QRCodeEncoder();
            Bitmap bi = encoder.Encode(url);
            MemoryStream tmpSteam = new MemoryStream();
            bi.Save(tmpSteam, ImageFormat.Jpeg);
            byte[] byteImage = tmpSteam.ToArray();

            //bi.Save(FilePath, ImageFormat.Jpeg);
            return byteImage;
        }

        public static byte[] GetBarcodeImage(string txt)
        {
            try
            {
                BarcodeWriter writer = new BarcodeWriter()
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        
                        Height = 40,
                        Width = 130,
                        PureBarcode = false,
                        Margin = 0,
                    },

                    
                };
                EncodingOptions f = new EncodingOptions();
               
                var bitmap = writer.Write(txt);
                MemoryStream tmpSteam = new MemoryStream();
                bitmap.Save(tmpSteam, ImageFormat.Png);
                byte[] byteImage = tmpSteam.ToArray();
                //bitmap.Save("c:\\123.png", ImageFormat.Png);

                return byteImage;

                    //bitmap.Save(HttpContext.Response.Body, System.Drawing.Imaging.ImageFormat.Png);
                    //ViewBag.BarcodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray()); // there's no need to return a `FileContentResult` by `File(...);`
                    //return View();
                
            }
            catch (Exception ex)
            {
                //ErrorLog.WriteErrorLog("Barcode", ex.ToString(), ex.Message);  
            }
            return null;
        }







    }

                
public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }

   
}