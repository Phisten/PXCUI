using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;

namespace PlusObj
{
    public class Bitmap_Histogram
    {
        /*======================================*/
        //建構
        /*======================================*/
        public Bitmap_Histogram() { ;}




        /*======================================*/
        //屬性
        /*======================================*/
        /// <summary>直方圖寬度，根據線條寬度間距建立，唯讀</summary>
        public int Width
        {
            get
            { return LineWidth * 256 + LineSpace * 255; }
        }

        private int _Height = 256;
        /// <summary>直方圖高度</summary>
        public int Height
        {
            get { return _Height; }
            set
            {
                if (value > 0)
                { _Height = value; }
                else
                { _Height = 1; }
            }
        }

        /// <summary>直方圖線條色彩</summary>
        public List<Bitmap_Color> LineColors = new List<Bitmap_Color>() { Bitmap_Color.FromArgb(255, 64, 64, 64), };

        /// <summary>直方圖背景色彩</summary>
        public Bitmap_Color BackgroundColor = Bitmap_Color.FromArgb(64, 255, 255, 255);

        /// <summary>直方圖線條寬度</summary>
        public int LineWidth = 2;

        /// <summary>直方圖線條間距</summary>
        public int LineSpace = 0;

        /// <summary>色彩通道資料
        private Int32[] ChannelData = new Int32[256];

        /// <summary>直方圖</summary>
        public Bitmap Bitmap
        {
            get
            {
                Bitmap_ARGB RETURN = new Bitmap_ARGB(Width, Height);
                RETURN.Clear(BackgroundColor);

                Int32 MaxChannelData = 0;

                for (int i = 0; i < 256; i++)
                {
                    if (MaxChannelData < ChannelData[i])
                    { MaxChannelData = ChannelData[i]; }
                }

                double unitHeight = 1.0 / MaxChannelData * Height;

                for (int i = 0; i < 256; i++)
                {
                    int LineHeight = (int)(ChannelData[i] * unitHeight);
                    int startY = Height - LineHeight;
                    RETURN.Clear(LineWidth * i + i * LineSpace, startY, LineWidth, LineHeight, LineColors[i % LineColors.Count]);
                }

                return RETURN.Bitmap;
            }
        }



        /*======================================*/
        //公開函式
        /*======================================*/
        /// <summary>將圖片資料讀入直方圖中</summary>
        public void Read(Bitmap_Gray bitmap)
        { ChannelData = GetChannelData(bitmap.Pixels); }
        /// <summary>將圖片資料讀入直方圖中 Channel:"Red", "Green", "Blue"</summary>
        public void Read(Bitmap_ARGB bitmap, string channel)
        { ChannelData = GetChannelData(bitmap.Pixels, channel); }
        /// <summary>將圖片資料讀入直方圖中 "Green"</summary>
        public void Read(Bitmap_ARGB bitmap)
        { Read(bitmap, "Green"); }


        /*======================================*/
        //靜態函式
        /*======================================*/
        /// <summary>計算該圖色彩通道之單位色彩數量</summary>
        public static Int32[] GetChannelData(int[] pixels, string type)
        {
            Int32[] RETURN = new Int32[256];
            int moveAmount = -1;

            if (type == "Red" || type == "red" || type == "R" || type == "r")
            { moveAmount = 16; }
            else if (type == "Green" || type == "green" || type == "G" || type == "g")
            { moveAmount = 8; }
            else if (type == "Blue" || type == "blue" || type == "B" || type == "b")
            { moveAmount = 0; }
            else
            { MessageBox.Show("搜尋條件不符!!"); }

            if (moveAmount != -1)
            {
                for (int i = 0; i < pixels.Length; i++)
                { ++RETURN[((byte)(pixels[i] >> moveAmount))]; }
            }

            return RETURN;
        }
        /// <summary>計算該圖色彩通道之單位色彩數量(預設Green通道)</summary>
        public static Int32[] GetChannelData(int[] pixels)
        { return GetChannelData(pixels, "Green"); }
        /// <summary>計算該圖色彩通道之單位色彩數量</summary>
        public static int[] GetChannelData(byte[] pixels)
        {
            Int32[] RETURN = new Int32[256];

            for (int i = 0; i < pixels.Length; i++)
            { ++RETURN[(pixels[i] & 0xff)]; }

            return RETURN;
        }
    }
}
