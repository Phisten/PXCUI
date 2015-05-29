using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Drawing;

namespace PlusObj
{
    public class DelegateType
    {
        /*=============================================*/
        //參數
        /*=============================================*/
        public delegate void VOID();

        public delegate void OBJ(object o);
    }
}

namespace PlusObj
{
    /// <summary>影像緩衝</summary>
    public class Bitmap_VedioBuffer
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        /// <summary>建構</summary>
        public Bitmap_VedioBuffer()
        {
            UpdateThead = new Thread(Update);
            UpdateThead.IsBackground = true;
            UpdateThead.Start();
        }




        /*=============================================*/
        //參數
        /*=============================================*/
        /// <summary>接收端暫存</summary>
        protected Bitmap_ARGB Bitmap_ReceiveBuffer = new Bitmap_ARGB();

        /// <summary>處理後像素陣列暫存</summary>
        protected int[] IntArray_ReceiveBuffer;

        /// <summary>緩衝影像</summary>
        protected Bitmap[] BitmapBuffers = new Bitmap[3];

        /// <summary>緩衝影像ARGB</summary>
        protected Bitmap_ARGB[] BitmapBuffers_ARGB = new Bitmap_ARGB[3];

        /// <summary>顯示影像的索引</summary>
        protected int Index_Show = 0;

        /// <summary>寫入完成的索引</summary>
        protected int Index_Write = 0;

        /// <summary>資料送出時間</summary>
        protected Int32 SendTime = 0;

        /// <summary>紀錄_每秒張數</summary>
        protected int Buffer_FPS = 0;
        protected int _FPS = 0;

        /// <summary>紀錄_資料流量</summary>
        protected int Buffer_DataSize = 0;
        protected int _DataSize = 0;

        /// <summary>紀錄_傳輸延遲</summary>
        protected int _Delay_Net = 0;

        /// <summary>紀錄_已取最新圖像</summary>
        protected bool _IsGeted = false;

        /// <summary>更新緒</summary>
        protected Thread UpdateThead;

        /// <summary>刷新資料</summary>
        protected void Update()
        {
            while (true)
            {
                _FPS = Buffer_FPS;
                Buffer_FPS = 0;

                _DataSize = Buffer_DataSize;
                Buffer_DataSize = 0;

                Thread.Sleep(1000);
            }
        }

        /// <summary>目前時間</summary>
        protected DateTime NowDateTime;



        /*=============================================*/
        //屬性
        /*=============================================*/
        /// <summary>傳輸資料量</summary>
        public int DataSize
        { get { return _DataSize; } }

        /// <summary>每秒接收張數</summary>
        public int FPS
        { get { return _FPS; } }

        /// <summary>修正延遲時間</summary>
        public Int32 Delay_Offer = 0;

        /// <summary>傳輸延遲</summary>
        public int Delay_Net
        { get { return _Delay_Net; } }

        /// <summary>已取最新圖像</summary>
        public bool IsGated
        { get { return _IsGeted; } }



        /*=============================================*/
        //方法
        /*=============================================*/
        /// <summary>JPG串流轉換圖片</summary>
        /// <param name="jpgData">串流資料</param>
        public void JpgToBitmap(byte[] jpgData)
        { JpgToBitmap(jpgData, -1); }

        /// <summary>JPG串流轉換圖片</summary>
        /// <param name="jpgData">串流資料</param>
        /// <param name="sendTime">發送時間</param>
        public void JpgToBitmap(byte[] jpgData, int sendTime)
        {
            #region 時間資料紀錄
            NowDateTime = DateTime.Now;
            if (sendTime != -1)
            {
                SendTime = sendTime;
                int nowTime_Int = ((NowDateTime.Hour * 3600000) + (NowDateTime.Minute * 60000) + (NowDateTime.Second) * 1000 + (NowDateTime.Millisecond));
                _Delay_Net = nowTime_Int - (sendTime + Delay_Offer);
                if (_Delay_Net < 4) { _Delay_Net = 4; }
            }
            #endregion
            ///////////////////////////////////////////////////////////////////////////
            #region 計算可寫入的影像緩衝索引
            int writeComIndexTemp;
            if ((Index_Show + 1) % 3 != Index_Write)
            { writeComIndexTemp = (Index_Show + 1) % 3; }
            else
            { writeComIndexTemp = (Index_Show + 2) % 3; }
            #endregion
            ///////////////////////////////////////////////////////////////////////////
            #region 解碼JPG並存為緩衝影像
            if (BitmapBuffers[writeComIndexTemp] != null)
            { BitmapBuffers[writeComIndexTemp].Dispose(); }
            BitmapBuffers[writeComIndexTemp] = Bitmap_Convert.Bitmap_By(jpgData);
            //更新顯示索引
            Index_Write = writeComIndexTemp;
            #endregion
            ///////////////////////////////////////////////////////////////////////////
            #region 累計FPS，傳輸資料量，處理延遲
            _IsGeted = false;
            ++Buffer_FPS;

            Buffer_DataSize += jpgData.Length;
            #endregion
        }

        /// <summary>取得完成圖像</summary>
        public Bitmap ReadBitmap()
        {
            Index_Show = Index_Write;
            _IsGeted = true;
            return BitmapBuffers[Index_Show];
        }





        /*=============================================*/
        //濾鏡類別
        /*=============================================*/
        /// <summary>濾鏡_基底</summary>
        public abstract class Effect_Base
        {
            /*=============================================*/
            //參數
            /*=============================================*/
            public abstract void Use(Bitmap_ARGB bitmap, int[] bufferPixels);
        }

        /// <summary>濾鏡_灰階化</summary>
        public class Effect_Gray : Effect_Base
        {
            /*=============================================*/
            //參數
            /*=============================================*/
            public override void Use(Bitmap_ARGB bitmap, int[] bufferPixels)
            { Bitmap_ARGB_Processing.Gray(bitmap); }
        }

        /// <summary>濾鏡_高通濾波</summary>
        public class Effect_HighPassMask : Effect_Base
        {
            /*=============================================*/
            //參數
            /*=============================================*/
            public override void Use(Bitmap_ARGB bitmap, int[] bufferPixels)
            {
                Bitmap_ARGB_Processing.MirrorMask(bitmap, bufferPixels, Mask);
                bufferPixels.CopyTo(bitmap.Pixels, 0);
            }

            protected int[,] Mask = new int[3, 3]
            {
                {-1,-2,-1},
                {-2,12,-2},
                {-1,-2,-1}
            };
        }

        /// <summary>濾鏡_對比度</summary>
        public class Effect_Contrast : Effect_Base
        {
            /*=============================================*/
            //參數
            /*=============================================*/
            public override void Use(Bitmap_ARGB bitmap, int[] bufferPixels)
            { Bitmap_ARGB_Processing.Contrast(bitmap, ContrastValue); }

            public int ContrastValue = 0;
        }

        /// <summary>濾鏡_銳利化</summary>
        public class Effect_Sharpen : Effect_Base
        {
            /*=============================================*/
            //參數
            /*=============================================*/
            public override void Use(Bitmap_ARGB bitmap, int[] bufferPixels)
            { Bitmap_ARGB_Processing.X_MirrorMask(bitmap, bufferPixels, Mask, 2); }

            public int[,] Mask = new int[3, 3]
            {
                {-1,-2,-1},
                {-2,20,-2},
                {-1,-2,-1}
            };
        }
    }
}


//ImageSocket
namespace PlusObj
{
    /// <summary>影像傳輸接口</summary>
    public class ImageSocket
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        /// <summary>建構</summary>
        public ImageSocket() { ;}



        /*=========================================*/
        //屬性
        /*=========================================*/
        /// <summary>緩衝影像類</summary>
        public Bitmap_VedioBuffer VedioBuffer = new Bitmap_VedioBuffer();



        /*=========================================*/
        //參數
        /*=========================================*/
        private Socket server;
        private Socket client;
        private string msg = "";

        /// <summary>紀錄_執行狀態</summary>
        private bool isRun = false;



        /*=========================================*/
        //函式
        /*=========================================*/
        #region 本體執行緒
        /// <summary>類別主執行緒</summary>
        private Thread thisThread;

        /// <summary>循環接收資料</summary>
        private void Loop_AcceptData()
        {
            //將IP位址和Port宣告為服務的連接點(所有網路介面卡 IP, Port)
            IPEndPoint ipont = new IPEndPoint(IPAddress.Any, 3145);

            //IPv4協定 通訊類型 通訊協定
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //完全接收失敗嘗試接收次數記錄
            int TryCount = 0;

            #region 嘗試建立伺服端，連接埠有無重疊
            try
            {
                //建立本機連線
                server.Bind(ipont);
                //偵測連接(最大連接數
                server.Listen(1);
            }
            catch
            {
                msg += "Port重複使用\n";
                Close();
                return;
            }
            #endregion

            #region 等待客戶端連線
            try
            { client = server.Accept(); }
            catch
            {
                msg += "停止等待Client\n";
                Close();
                return;
            }
            #endregion

            #region 標頭與資料內容
            byte[] data = new byte[0];

            byte[] data_Len = new byte[4] { 0, 0, 0, 0 };
            byte[] data_SendTime = new byte[4] { 0, 0, 0, 0 };


            int len = 0;
            #endregion

            #region 時間校正
            //紀錄傳送時間
            DateTime SendTime = DateTime.Now;
            client.Send(new byte[1] { 0 });
            client.Receive(data_SendTime);
            DateTime ReceiveTime = DateTime.Now;

            int NowClientTime = (data_SendTime[0] << 24 | data_SendTime[1] << 16 | data_SendTime[2] << 8 | data_SendTime[3]) - (int)((ReceiveTime - SendTime).TotalMilliseconds * 0.5);

            int NowServerTime = ((SendTime.Hour * 3600000) + (SendTime.Minute * 60000) + (SendTime.Second) * 1000 + (SendTime.Millisecond));

            VedioBuffer.Delay_Offer = NowServerTime - NowClientTime;
            #endregion

            //循環接收資料
            while (isRun)
            {

                try
                {
                    #region 嘗試從客戶端接收資料，要是從伺服端強制關閉客戶端會造成以下物件錯誤
                    TryCount = 0;
                    client.Receive(data_Len);
                    client.Receive(data_SendTime);

                    len = data_Len[0] << 24 | data_Len[1] << 16 | data_Len[2] << 8 | data_Len[3];
                    if (len > 10485760 || len < 0) //if  > 10MB 
                    {
                        Console.WriteLine("ErrorDetect: len=" + len);
                        Close();
                        return;
                    }
                    data = new byte[len];

                    int get = client.Receive(data);

                    //如果接收不完全
                    if (get != len)
                    {
                        int StartIndex = get;
                        int EndIndex = len - get;
                        //不斷等待進行接收
                        while (StartIndex != len)
                        {
                            byte[] tmpAcceptData = new byte[EndIndex];
                            get = client.Receive(tmpAcceptData);
                            Array.ConstrainedCopy(tmpAcceptData, 0, data, StartIndex, get); StartIndex += get;
                            EndIndex -= get;
                            if (get == 0)
                            {
                                ++TryCount;
                                if (TryCount > 100)
                                {
                                    msg += "Client斷線，或接收逾時";
                                    Close();
                                    return;
                                }
                            }
                        }
                    }


                    //回傳接收完成的訊息
                    //client.Send(new byte[1] { 0 });
                    client.Send(new byte[1] { 3 });
                }
                catch(Exception e)
                {
                    msg += "Server斷線(" + e.Message;
                    Console.WriteLine("ErrorDetect: Server斷線:" + e.Message);
                    Close();
                    return;
                }

                #endregion

                #region 當客戶端主動關閉時，收不到資訊
                if (data.Length == 0)
                {
                    msg += "Client斷線";
                    Close();
                    return;
                }
                #endregion

                #region 單圖接收完畢，解碼轉點陣圖像
                int data_SendTimeInt = data_SendTime[0] << 24 | data_SendTime[1] << 16 | data_SendTime[2] << 8 | data_SendTime[3];
                //將JPG串流寫入影像緩衝解碼
                VedioBuffer.JpgToBitmap(data, data_SendTimeInt);
                #endregion
            }
        }
        #endregion



        /*=========================================*/
        //公開方法
        /*=========================================*/
        /// <summary>運作</summary>
        public void Run()
        {
            if (isRun)
            { MessageBox.Show("重複執行 Socket"); }
            else
            {
                isRun = true;

                //本體執行緒建立
                thisThread = new Thread(Loop_AcceptData);
                thisThread.IsBackground = true;
                thisThread.Start();
            }
        }

        /// <summary>關閉</summary>
        public void Close()
        {
            isRun = false;

            if (client != null)
            {
                client.Close();
                client = null;
            }

            if (server != null)
            {
                server.Close();
                server = null;
            }

            if (msg != "")
            {
                //MessageBox.Show(msg);
                Console.WriteLine(msg);
                msg = "";
            }
        }

        /// <summary>執行狀態</summary>
        public bool IsRun
        { get { return isRun; } }
    }     
}