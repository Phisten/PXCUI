using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

//點陣圖_基礎
namespace PlusObj
{
    /// <summary>點陣圖_基礎</summary>
    public abstract class Bitmap_Base
    {
        /*======================================*/
        //建構
        /*======================================*/
        /// <summary>建構</summary>
        public Bitmap_Base() { ;}
        /// <summary>建構</summary>
        public Bitmap_Base(int width, int height) { Create(width, height); }


        /*======================================*/
        //屬性
        /*======================================*/
        /// <summary>圖片寬度</summary>
        protected int _Width = 0;
        public int Width
        { get { return _Width; } }

        /// <summary>圖片高度</summary>
        protected int _Height = 0;
        public int Height
        { get { return _Height; } }




        /*======================================*/
        //公開函式
        /*======================================*/
        /// <summary>建立空白點陣圖</summary>
        public abstract void Create(int width, int height);

        /// <summary>清除為指定色彩</summary>
        public abstract void Clear(Bitmap_Color color);

        /// <summary>指定矩形區域上色</summary>
        public virtual void Clear(int x, int y, int width, int height, Bitmap_Color color)
        {
            int LeftTopX;
            int LeftTopY;

            int RightBottomX;
            int RightBottomY;

            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            //合理性判斷
            LeftTopX = CheckPoint(x, Width);
            LeftTopY = CheckPoint(y, Height);
            RightBottomX = CheckPoint(x + width, Width);
            RightBottomY = CheckPoint(y + height, Height);

            DrawRect(LeftTopX, LeftTopY, RightBottomX, RightBottomY, color);
        }

        /// <summary>指定矩形區域上色</summary>
        protected abstract void DrawRect(int LeftTopX, int LeftTopY, int RightBottonX, int RightBottonY, Bitmap_Color color);


        /*======================================*/
        //私有函式
        /*======================================*/
        /// <summary>檢查輸入的點資訊是否合理，並回傳修正點位</summary>
        private int CheckPoint(int point, int size)
        {
            if (0 > point)
            { return 0; }
            else if (size < point)
            { return size; }
            return point;
        }
    }
}



//點陣圖_灰階
namespace PlusObj
{
    /// <summary>點陣圖_灰階</summary>
    public class Bitmap_Gray : Bitmap_Base
    {
        /*======================================*/
        //建構
        /*======================================*/
        /// <summary>建構</summary>
        public Bitmap_Gray() { ;}
        /// <summary>建構</summary>
        public Bitmap_Gray(int width, int height) : base(width, height) { ;}
        /// <summary>建構</summary>
        public Bitmap_Gray(Uri uri)
        { INIT(uri); }
        /// <summary>建構</summary>
        public Bitmap_Gray(string path)
        {
            Uri uri = new Uri(Environment.CurrentDirectory + "/" + path, UriKind.Absolute);
            INIT(uri);
        }
        /// <summary>隱式建構</summary>
        private void INIT(Uri uri)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(uri.AbsolutePath);


            //設定公開屬性
            _Width = bitmap.Width;
            _Height = bitmap.Height;

            //建立實體記憶體空間
            Create(bitmap.Width, bitmap.Height);

            //讀取像素
            #region BitmapRead
            Rectangle bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData bitmapData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr Scan0 = bitmapData.Scan0;

            int[] intPixels = new int[bitmap.Width * bitmap.Height];

            unsafe
            {
                int* pixelsPtr = (int*)(void*)Scan0;

                int res = 0;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++, res++)
                    { intPixels[res] = pixelsPtr[res]; }
                }
            }

            bitmap.UnlockBits(bitmapData);
            #endregion


            for (int i = 0; i < intPixels.Length; i++)
            {
                int intPixel = intPixels[i];

                int r = (byte)(intPixel >> 16);
                int g = (byte)(intPixel >> 8);
                int b = (byte)(intPixel);
                int gray_Value = (r * 19595 + g * 38469 + b * 7472) >> 16;

                Pixels[i] = (byte)(gray_Value);
            }
        }

        /*======================================*/
        //屬性
        /*======================================*/
        /// <summary>設定圖片的背景色彩</summary>
        public byte BackgroundColor = (byte)96;

        /// <summary>像素資料</summary>
        public byte[] Pixels;

        /// <summary>像素轉圖片</summary>
        public Bitmap Bitmap
        {
            get { return Bitmap_Convert.Bitmap_By(this); }
        }



        /*======================================*/
        //公開函式
        /*======================================*/
        /// <summary>指定矩形區域上色</summary>
        protected override void DrawRect(int LeftTopX, int LeftTopY, int RightBottonX, int RightBottonY, Bitmap_Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            byte grayValue = ((byte)((int)(r * 0.299 + g * 0.587 + b * 0.114)));


            //將位置起點轉換成1維陣列
            for (int index_Y = LeftTopY * Width; index_Y < RightBottonY * Width; index_Y += Width)
            {
                for (int index_X = LeftTopX; index_X < RightBottonX; index_X++)
                { Pixels[index_Y + index_X] = grayValue; }
            }
        }

        /// <summary>指定所有像素色彩</summary>
        public override void Clear(Bitmap_Color color)
        {
            byte grayColor = color.ToGrayColor();

            for (int i = 0; i < Pixels.Length; i++)
            { Pixels[i] = grayColor; }
        }

        /// <summary>創造新的像素區域</summary>
        public override void Create(int width, int height)
        {
            if (width > 0)
            { _Width = width; }
            else
            { _Width = 1; }

            if (height > 0)
            { _Height = height; }
            else
            { _Height = 1; }

            Pixels = new byte[Width * Height];
        }
    }
}



//點陣圖_全彩
namespace PlusObj
{
    /// <summary>點陣圖_全彩</summary>
    public class Bitmap_ARGB : Bitmap_Base
    {
        /*======================================*/
        //建構
        /*======================================*/
        /// <summary>建構</summary>
        public Bitmap_ARGB() { ;}
        /// <summary>建構</summary>
        public Bitmap_ARGB(int width, int height) : base(width, height) { ;}
        /// <summary>建構</summary>
        public Bitmap_ARGB(Uri uri)
        { INIT(uri); }
        /// <summary>建構</summary>
        public Bitmap_ARGB(string path)
        {
            Uri uri = new Uri(Environment.CurrentDirectory + "/" + path, UriKind.Absolute);
            INIT(uri);
        }
        /// <summary>隱式建構</summary>
        private void INIT(Uri uri)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(uri.AbsolutePath);

            //設定公開屬性
            _Width = bitmap.Width;
            _Height = bitmap.Height;

            //建立實體記憶體空間
            Create(bitmap.Width, bitmap.Height);

            //讀取像素
            Bitmap_OtherTool.BitmapToPixels(bitmap, Pixels);
        }



        /*======================================*/
        //公開函式
        /*======================================*/
        /// <summary>像素資料</summary>
        public int[] Pixels;

        /// <summary>取得圖像</summary>
        public Bitmap Bitmap
        {
            get { return Bitmap_Convert.Bitmap_By(this); }
        }

        /// <summary>以指定通道取代圖層，Type:Red Green Blue</summary>
        public void ReplaceBy(string type)
        {
            byte colorValue = 0;
            int moveAmount = -1;

            if (type == "Red" || type == "red")
            { moveAmount = 16; }
            else if (type == "Green" || type =="green")
            {  moveAmount = 8; }
            else if (type == "Blue" || type == "blue")
            { moveAmount = 0; }
            else
            { MessageBox.Show("通道錯誤"); }

            if (moveAmount != -1)
            {
                for (int i = 0; i < this.Pixels.Length; i++)
                {
                    colorValue = (byte)(Pixels[i] >> moveAmount);
                    Pixels[i] = (Pixels[i] >> 24 << 24) | (colorValue << 16) | (colorValue << 8) | (colorValue & 0xff);
                }
            }
        }

        /// <summary>指定所有像素色彩</summary>
        public override void Clear(Bitmap_Color color)
        {
            int colorValue = color.ToIntColor();
            int length = Pixels.Length;

            for (int i = 0; i < length; i++)
            {
                Pixels[i] = colorValue;
            }
        }

        /// <summary>指定矩形區域上色</summary>
        protected override void DrawRect(int LeftTopX, int LeftTopY, int RightBottonX, int RightBottonY, Bitmap_Color color)
        {
            int colorValue = color.A << 24 | color.R << 16 | color.G << 8 | (color.B & 0xff);

            //將位置起點轉換成1維陣列
            for (int index_Y = LeftTopY * Width; index_Y < RightBottonY * Width; index_Y += Width)
            {
                for (int index_X = LeftTopX; index_X < RightBottonX; index_X++)
                { Pixels[index_Y + index_X] = colorValue; }
            }
        }

        /// <summary>創造新的像素區域</summary>
        public override void Create(int width, int height)
        {
            if (width > 0)
            { _Width = width; }
            else
            { _Width = 1; }

            if (height > 0)
            { _Height = height; }
            else
            { _Height = 1; }

            Pixels = new int[Width * Height];
        }
    }
}