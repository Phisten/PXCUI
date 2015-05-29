using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


//影像資料轉換
namespace PlusObj
{
    /// <summary>影像資料轉換</summary>
    public class Bitmap_Convert
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        /// <summary>建構</summary>
        public Bitmap_Convert() { ;}




        /*=============================================*/
        //靜態方法
        /*=============================================*/
        /// <summary>值轉Byte</summary>
        public static byte Byte_By(float value)
        {
            if (value > 255)
            { return 255; }
            else if (value < 0)
            { return 0; }
            return (byte)value;
        }
        /// <summary>值轉Byte</summary>
        public static byte Byte_By(double value)
        {
            if (value > 255)
            { return 255; }
            else if (value < 0)
            { return 0; }
            return (byte)value;
        }
        /// <summary>值轉Byte</summary>
        public static byte Byte_By(int value)
        {
            {
                if (value > 255)
                { return (byte)255; }
                else if (value < 0)
                { return (byte)0; }
                return (byte)value;
            }
        }

        /// <summary>8位元整數轉32位元整數(陣列)</summary>
        public static int[] IntArray_By(byte[] byteArray)
        {
            int[] intArray = new int[(byteArray.Length >> 2)];
            Bitmap_Convert.IntArray_By(byteArray, intArray);
            return intArray;
        }
        /// <summary>8位元整數轉32位元整數(陣列)</summary>
        public static void IntArray_By(byte[] byteArray, int[] intArray)
        {
            if ((byteArray.Length >> 2) != intArray.Length)
            { MessageBox.Show("長度不符合!"); }
            else
            {
                for (int i=0; i < intArray.Length; i += 4)
                { intArray[(i >> 2)] = byteArray[i] << 24 | byteArray[i + 1] << 16 | byteArray[i + 2] << 8 | (byteArray[i + 3]); }
            }
        }

        /// <summary>32位元整數轉8位元整數(陣列)</summary>
        public static byte[] ByteArray_By(int[] intArray)
        {
            byte[] RETURN = new byte[intArray.Length << 2];
            int index_byte;

            for (int i = 0; i < intArray.Length; i++)
            {
                index_byte = i << 2;

                //A
                RETURN[index_byte] = (byte)(intArray[i] >> 24);
                //R
                RETURN[index_byte + 1] = (byte)(intArray[i] >> 16);
                //G
                RETURN[index_byte + 2] = (byte)(intArray[i] >> 8);
                //B
                RETURN[index_byte + 3] = (byte)(intArray[i]);
            }
            return RETURN;
        }

        /// <summary>取得圖片</summary>
        public static Bitmap Bitmap_By(Bitmap_Gray bitmap)
        {
            Bitmap RETURN = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap_OtherTool.PixelsToBitmap(RETURN, bitmap.Pixels);
            return RETURN;
        }

        /// <summary>取得圖片</summary>
        public static Bitmap Bitmap_By(Bitmap_ARGB bitmap)
        {
            Bitmap RETURN = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap_OtherTool.PixelsToBitmap(RETURN, bitmap.Pixels);
            return RETURN;
        }

        /// <summary>取得一維陣列</summary>
        public static int[] IntArray_By(int[,] intArray)
        {
            int[] RETURN = new int[intArray.Length];
            int index_1D = 0;

            for (int y = 0; y < intArray.GetLength(0); y++)
            {
                for (int x = 0; x < intArray.GetLength(1); x++,index_1D++)
                {
                    RETURN[index_1D] = intArray[y, x];
                }
            }

            return RETURN;
        }

        /// <summary>取得網路傳輸的影像</summary>
        public static Bitmap Bitmap_By(byte[] data)
        {
            Bitmap bitmap = null;

            //將網路串流讀取成為影像
            using (MemoryStream stream = new MemoryStream(data))
            {
                try
                { bitmap = Image.FromStream(stream) as Bitmap; }
                catch { ;}
            }

            return bitmap;
        }

        /// <summary>讀取圖像像素並輸出</summary>
        public static int[] Pixels_By(Bitmap bitmap)
        {
            int[] RETURN = new int[bitmap.Width * bitmap.Height];
            Bitmap_OtherTool.BitmapToPixels(bitmap, RETURN);
            return RETURN;
        }
    }
}
