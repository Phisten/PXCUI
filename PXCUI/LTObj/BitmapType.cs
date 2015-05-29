using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


//Bitmap_Color
namespace PlusObj
{
    /// <summary>影像資料轉換</summary>
    public class Bitmap_Color
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        /// <summary>建構</summary>
        public Bitmap_Color() { ;}

        public static Bitmap_Color FromArgb(byte a, byte r, byte g, byte b)
        { return new Bitmap_Color() { A = a, R = r, G = g, B = b }; }

        public static Bitmap_Color FromRgb(byte r, byte g, byte b)
        {
            return new Bitmap_Color() { A = 255, R = r, G = g, B = b };
        }

        public static Bitmap_Color FromGray(byte g)
        {
            return new Bitmap_Color() { A = 255, R = g, G = g, B = g };
        }

        /*=============================================*/
        //屬性
        /*=============================================*/
        public byte A = 255;
        public byte R = 0;
        public byte G = 0;
        public byte B = 0;

        //將顏色轉換Int32類型資料
        public int ToIntColor()
        {
            return ((A << 24) | (R << 16) | (G << 8) | (B & 0xff));
        }
        //將顏色轉成灰階碼
        public byte ToGrayColor()
        {
            int r = (R & 0xff);
            int g = (G & 0xff);
            int b = (B & 0xff);
            int gray_Value = (r * 19595 + g * 38469 + b * 7472) >> 16;

            return (byte)gray_Value;
        }
    }
}

//Bitmap_MirrorMap
namespace PlusObj
{
    /// <summary>鏡像表</summary>
    public class Bitmap_MirrorMap
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        /// <summary>建構</summary>
        public Bitmap_MirrorMap() { ;}
        /// <summary>建構</summary>
        public Bitmap_MirrorMap(int width, int height, int radiusX, int radiusY)
        { INIT(width, height, radiusX, radiusY); }
        /// <summary>建構</summary>
        public Bitmap_MirrorMap(int width, int height, int[,] mask)
        {
            int radiusX = (int)((mask.GetLength(1) - 1) * 0.5);
            int radiusY = (int)((mask.GetLength(0) - 1) * 0.5);

            INIT(width, height, radiusX, radiusY);
        }
        /// <summary>建構</summary>
        public Bitmap_MirrorMap(Bitmap_Base bitmap, int[,] mask)
        {
            int radiusX = (int)((mask.GetLength(1) - 1) * 0.5);
            int radiusY = (int)((mask.GetLength(0) - 1) * 0.5);
            INIT(bitmap.Width, bitmap.Height, radiusX, radiusY);
        }

        /// <summary>隱式建構</summary>
        protected void INIT(int width, int height, int radiusX, int radiusY)
        {
            Map_Horizontal = Get_MirrorMap(width, radiusX);
            Map_Vertical = Get_MirrorMap(height, radiusY);

            for (int i = 0; i < Map_Vertical.Length; i++)
            {
                Map_Vertical[i] *= width;
            }

            Width = width;
            Height = height;

            RadiusX = radiusX;
            RadiusY = radiusY;
        }



        /*=============================================*/
        //屬性
        /*=============================================*/
        //水平映射表
        public int[] Map_Horizontal;
        //垂直映射表
        public int[] Map_Vertical;

        //原始寬度
        public int Width;
        //原始高度
        public int Height;
        //遮罩水平半徑
        public int RadiusX;
        //遮罩垂直半徑
        public int RadiusY;


        /*=============================================*/
        //靜態方法
        /*=============================================*/
        /// <summary>取得指定方向鏡像陣列</summary>
        public static int[] Get_MirrorMap(int length, int radius)
        {
            int[] RETURN = new int[length + radius * 2];

            int add_value = 1;

            int temp_fornt = 0;
            int temp_behind = length - 1;

            int index_fornt = radius - 1;
            int index_behind = radius + length;

            //鏡像部分
            for (int i = 0; i < radius; i++)
            {
                RETURN[index_fornt] = temp_fornt;
                RETURN[index_behind] = temp_behind;

                if ((temp_fornt + add_value) < 0 || (temp_fornt + add_value) >= length)
                { add_value = -add_value; }
                else
                {
                    temp_fornt += add_value;
                    temp_behind -= add_value;
                }

                index_fornt--;
                index_behind++;
            }

            //本體部分
            for (int i = radius, res = 0; i < length + radius; i++, res++)
            { RETURN[i] = res; }

            return RETURN;
        }
        /// <summary>檢查資料是否符合</summary>
        public static bool CheckData(Bitmap_Base bitmap, Bitmap_MirrorMap mirrorMap, int[,] mask)
        {
            if (bitmap.Width == mirrorMap.Width && bitmap.Height == mirrorMap.Height &&
                ((mirrorMap.RadiusX << 1) + 1) == mask.GetLength(1) && ((mirrorMap.RadiusY << 1) + 1) == mask.GetLength(0))
            { return true; }

            return false;
        }
    }
}

//Bitmap_ColorMap
namespace PlusObj
{
    /// <summary>色彩映射表</summary>
    public class Bitmap_ColorMap
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        public Bitmap_ColorMap()
        {
            byte[] r = new byte[256];
            byte[] g = new byte[256];
            byte[] b = new byte[256];

            #region Jet
            for (int i = 0; i < 128; i++)
            {
                Jet[i] = 255 << 24 | 0 | (i * 2) << 8 | (255 - i * 2);
            }
            for (int i = 128; i < 256; i++)
            {
                Jet[i] = 255 << 24 | ((i - 128) * 2) << 16 | (255 - (i - 128) * 2) << 8 | 0;
            }
            #endregion

            #region Winter
            for (int i = 0; i < 256; i++)
            {
                Winter[i] = 255 << 24 | 0 | (i * 2) << 8 | (255 - (i >> 1));
            }
            #endregion
        }



        /*=============================================*/
        //靜態參數
        /*=============================================*/
        public static int[] Jet = new int[256];

        public static int[] Winter = new int[256];

        /// <summary>使用色彩表</summary>
        public static void Use_ColorMap(int[] pixels, byte[] colorMap)
        {
            byte A = 0;
            byte R = 0;
            byte G = 0;
            byte B = 0;
            int pixel = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                pixel = pixels[i];
                

                A = (byte)(pixel >> 24);
                R = colorMap[(byte)(pixel >> 16)];
                G = colorMap[(byte)(pixel >> 8)];
                B = colorMap[(byte)(pixel)];

                pixels[i] = A << 24 | R << 16 | G << 8 | B;
            }
        }
        /// <summary>使用色彩表</summary>
        public static void Use_ColorMap(int[] pixels, byte[] colorMap, int moveCount)
        {
            byte A = 0;
            byte G = 0;
            int pixel = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                pixel = pixels[i];
                A = (byte)(pixel >> 24);
                G = colorMap[(byte)(pixel >> moveCount)];
                pixels[i] = A << 24 | G << 16 | G << 8 | G;
            }
        }
        /// <summary>使用色彩表</summary>
        public static void Use_ColorMap(Bitmap_ARGB bitmap, byte[] colorMap)
        { Use_ColorMap(bitmap.Pixels, colorMap); }
        /// <summary>使用色彩表 type: "R":"G":"B":"Gray"</summary>
        public static void Use_ColorMap(Bitmap_ARGB bitmap, byte[] colorMap, string type)
        {
            int moveCount = 0;
            if (type == "Red" || type == "red" || type == "r" || type == "R")
            { moveCount = 16; }
            else if (type == "Green" || type == "green" || type == "g" || type == "G")
            { moveCount = 8; }
            else if (type == "Blue" || type == "blue" || type == "b" || type == "B")
            { moveCount = 0; }
            else if (type == "Gray")
            {
                int[] pixels = bitmap.Pixels;
                byte A = 0;
                byte G = 0;
                int pixel = 0;

                for (int i = 0; i < pixels.Length; i++)
                {
                    pixel = pixels[i];
                    A = (byte)(pixel >> 24);
                    G = colorMap[(byte)pixel];
                    pixels[i] = A << 24 | G << 16 | G << 8 | G;
                }
            }

            Use_ColorMap(bitmap.Pixels, colorMap, moveCount);
        }
        /// <summary>使用色彩表</summary>
        public static void Use_ColorMap(Bitmap_Gray bitmap, byte[] colorMap)
        {
            byte[] pixels = bitmap.Pixels;

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = colorMap[pixels[i]];
            }
        }
        /// <summary>使用色彩表</summary>
        public static void Use_ColorMap(Bitmap_ARGB bitmap, byte[] colorMapR, byte[] colorMapG, byte[] colorMapB)
        {
            int[] pixels = bitmap.Pixels;
            byte A = 0;
            byte R = 0;
            byte G = 0;
            byte B = 0;
            int pixel = 0;

            for (int i = 0; i < pixels.Length; i++)
            {
                pixel = pixels[i];


                A = (byte)(pixel >> 24);
                R = colorMapR[(byte)(pixel >> 16)];
                G = colorMapG[(byte)(pixel >> 8)];
                B = colorMapB[(byte)(pixel)];

                pixels[i] = A << 24 | R << 16 | G << 8 | B;
            }
        }

        /// <summary>使用色彩表</summary>
        public static void Use_ColorMap(Bitmap_ARGB bitmap, int[] colorMap)
        {
            int[] pixels = bitmap.Pixels;

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = colorMap[(byte)pixels[i]];
            }
        }
    }
}

//Bitmap_IntRect
namespace PlusObj
{
    /// <summary>整數區域表示結構</summary>
    public class Bitmap_IntRect
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        public Bitmap_IntRect() { ;}

        public Bitmap_IntRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }




        /*=============================================*/
        //屬性
        /*=============================================*/
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;





        /*=============================================*/
        //方法
        /*=============================================*/
        public int Get_StartX()
        { return X; }

        public int Get_EndX()
        { return X + Width; }

        public int Get_StartY()
        { return Y; }

        public int Get_EndY()
        { return Y + Height; }
    }
}

//Bitmap_IntPoint
namespace PlusObj
{
    /// <summary>整數型態位置</summary>
    public class Bitmap_IntPoint
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        public Bitmap_IntPoint() { ;}

        public Bitmap_IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }



        /*=============================================*/
        //屬性
        /*=============================================*/
        public int X = 0;
        public int Y = 0;
    }
}