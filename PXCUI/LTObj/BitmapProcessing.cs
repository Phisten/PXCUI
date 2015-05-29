using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


//影像處理_底層
namespace PlusObj
{
    //影像處理_灰階
    namespace PlusObj
    {
        /// <summary>影像處理_灰階</summary>
        public class Bitmap_Gray_Processing
        {
            /*=============================================*/
            //建構
            /*=============================================*/
            /// <summary>建構</summary>
            public Bitmap_Gray_Processing() { ;}



            /*=============================================*/
            //靜態方法
            /*=============================================*/
            /// <summary>負片</summary>
            public static void Negative(Bitmap_Gray bitmap)
            {
                byte[] pixels = bitmap.Pixels;
                for (int i = 0; i < pixels.Length; i++)
                { pixels[i] = (byte)(pixels[i] ^ 0xff); }
            }

            /// <summary>二值化</summary>
            public static void Binary(Bitmap_Gray bitmap, byte thresholdValue)
            {
                //色採映射表
                byte[] colorMap = new byte[256];
                for (int i = 0; i < thresholdValue; i++)
                { colorMap[i] = (byte)0; }
                for (int i = thresholdValue; i < 256; i++)
                { colorMap[i] = (byte)255; }

                Bitmap_ColorMap.Use_ColorMap(bitmap, colorMap);
            }

            /// <summary>遮罩</summary>
            public static void Mask(Bitmap_Gray bitmap, int[,] mask)
            {
                byte[] pixels = bitmap.Pixels;
                byte[] RETURN = new byte[pixels.Length];

                //將二維陣列轉換成一維陣列
                int[] mask_1D = Bitmap_Convert.IntArray_By(mask);

                //影像長度步矩
                int width = bitmap.Width;
                int height = bitmap.Height;

                int mask_len_y = (int)((mask.GetLength(0) - 1) * 0.5);
                int mask_len_x = (int)((mask.GetLength(1) - 1) * 0.5);

                int start_mask_y = -mask_len_y * width;
                int end_mask_y = (mask_len_y + 1) * width;

                int start_mask_x = -mask_len_x;
                int end_mask_x = mask_len_x + 1;

                int start_y = mask_len_y * width;
                int end_y = pixels.Length - start_y;

                int start_x = mask_len_x;
                int end_x = width - start_x;

                for (int y = start_y; y < end_y; y += width)
                {
                    for (int x = start_x; x < end_x; x++)
                    {
                        int res = 0;
                        int grayValueTemp = 0;

                        for (int mask_y = start_mask_y; mask_y < end_mask_y; mask_y += width)
                        {
                            for (int mask_x = start_mask_x; mask_x < end_mask_x; mask_x++, res++)
                            { grayValueTemp += ((int)(pixels[y + x + mask_y + mask_x] & 0xff)) * mask_1D[res]; }
                        }
                        RETURN[y + x] = Bitmap_Convert.Byte_By(grayValueTemp);
                    }
                }
                bitmap.Pixels = RETURN;
            }

            /// <summary>鏡像遮罩</summary>
            public static void MirrorMask(Bitmap_Gray bitmap, int[,] mask)
            {
                //圖源大小
                int width = bitmap.Width;
                int height = bitmap.Height;

                //半徑
                int radiusX = (int)((mask.GetLength(1) - 1) * 0.5);
                int radiusY = (int)((mask.GetLength(0) - 1) * 0.5);

                //水平映射表
                int[] table_Horizontal = Bitmap_MirrorMap.Get_MirrorMap(width, radiusX);
                //垂直映射表
                int[] table_Vertical = Bitmap_MirrorMap.Get_MirrorMap(height, radiusY);
                for (int i = 0; i < table_Vertical.Length; i++)
                { table_Vertical[i] *= width; }

                //影像陣列
                byte[] pixels = bitmap.Pixels;
                byte[] RETURN = new byte[pixels.Length];

                //紀錄目前陣列的加權值
                int plusValue;

                //計數器
                int res_mask = 0;
                int res_pixel = 0;

                //通道累積值
                int grayValue;

                //將位置一開始就做初階轉換以提升效能
                int start_y = radiusY;
                int start_x = radiusX;
                int end_y = radiusY + height;
                int end_x = radiusX + width;

                int start_mask_y = -radiusY;
                int start_mask_x = -radiusX;
                int end_mask_y = radiusY + 1;
                int end_mask_x = radiusX + 1;


                //將2維陣列遮罩轉換1維
                int[] mask_1D = Bitmap_Convert.IntArray_By(mask);

                //需鏡像區域進行轉換，模擬全局
                for (int y = start_y; y < end_y; y++)
                {
                    for (int x = start_x; x < end_x; x++)
                    {
                        grayValue = 0;
                        res_mask = 0;

                        for (int mask_y = start_mask_y; mask_y < end_mask_y; mask_y++)
                        {
                            for (int mask_x = start_mask_x; mask_x < end_mask_x; mask_x++)
                            {
                                int temp_pixel = pixels[table_Vertical[y + mask_y] + table_Horizontal[x + mask_x]];
                                byte b = (byte)(temp_pixel);

                                plusValue = mask_1D[res_mask];

                                grayValue += b * plusValue;
                                res_mask++;
                            }
                        }
                        RETURN[res_pixel] = Bitmap_Convert.Byte_By(grayValue);
                        res_pixel++;
                    }
                }
                bitmap.Pixels = RETURN;
            }

            /// <summary>直方圖等化</summary>
            public static void Equlize(Bitmap_Gray bitmap)
            {
                Int32[] channelData = Bitmap_Histogram.GetChannelData(bitmap.Pixels);
                Int32 pixels_All = bitmap.Pixels.Length;
                Int32 pixels_Sum = 0;

                byte[] colorMap = new byte[256];

                //產生色彩映射表
                for (int i = 0; i < 256; i++)
                {
                    pixels_Sum += channelData[i];
                    colorMap[i] = (byte)(((float)pixels_Sum / (float)pixels_All) * 255);
                }

                Bitmap_ColorMap.Use_ColorMap(bitmap, colorMap);
            }
        }
    }
}




//影像處理_全彩
namespace PlusObj
{
    /// <summary>影像處理_全彩</summary>
    public class Bitmap_ARGB_Processing
    {
        /*=============================================*/
        //建構
        /*=============================================*/
        /// <summary>建構</summary>
        public Bitmap_ARGB_Processing() { ;}



        /*=============================================*/
        //靜態方法
        /*=============================================*/
        /// <summary>負片</summary>
        public static void Negative(Bitmap_ARGB bitmap)
        {
            int[] pixels = bitmap.Pixels;
            for (int i = 0; i < pixels.Length; i++)
            { pixels[i] = pixels[i] ^ 0x00ffffff; }
        }

        /// <summary>二值化，Type:Red Green Blue</summary>
        public static void Binary(Bitmap_ARGB bitmap, byte thresholdValue, string type)
        {
            int[] pixels = bitmap.Pixels;

            //色採映射表
            byte[] colorMap = new byte[256];
            for (int i = 0; i < thresholdValue; i++)
            { colorMap[i] = (byte)0; }
            for (int i = thresholdValue; i < 256; i++)
            { colorMap[i] = (byte)255; }

            Bitmap_ColorMap.Use_ColorMap(bitmap, colorMap, type);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>遮罩底層_原圖非取代</summary>
        public static void BaseMask(Bitmap_ARGB bitmap, int[] RETURN, int[] mask, Bitmap_IntPoint radiusSize)
        {
            int[] pixels = bitmap.Pixels;

            //影像長度步矩
            int width = bitmap.Width;
            int height = bitmap.Height;

            int mask_len_y = radiusSize.Y;
            int mask_len_x = radiusSize.X;

            int start_mask_y = -mask_len_y * width;
            int end_mask_y = (mask_len_y + 1) * width;

            int start_mask_x = -mask_len_x;
            int end_mask_x = mask_len_x + 1;

            int start_y = mask_len_y * width;
            int end_y = pixels.Length - start_y;

            int start_x = mask_len_x;
            int end_x = width - start_x;

            for (int y = start_y; y < end_y; y += width)
            {
                for (int x = start_x; x < end_x; x++)
                {
                    int res = 0;
                    int redValueTemp = 0;
                    int blueValueTemp = 0;
                    int greenValueTemp = 0;
                    int copyPixel;

                    for (int mask_y = start_mask_y; mask_y < end_mask_y; mask_y += width)
                    {
                        for (int mask_x = start_mask_x; mask_x < end_mask_x; mask_x++, res++)
                        {
                            copyPixel = pixels[y + x + mask_y + mask_x];
                            redValueTemp += ((byte)(copyPixel >> 16)) * mask[res];
                            greenValueTemp += ((byte)(copyPixel >> 8)) * mask[res];
                            blueValueTemp += ((byte)(copyPixel)) * mask[res];
                        }
                    }
                    RETURN[y + x] = (pixels[y + x] >> 24 << 24) | (Bitmap_Convert.Byte_By(redValueTemp) << 16) | (Bitmap_Convert.Byte_By(greenValueTemp) << 8) | Bitmap_Convert.Byte_By(blueValueTemp);
                }
            }
        }

        /// <summary>鏡像遮罩底層_原圖非取代</summary>
        public static void BaseMirrorMask(Bitmap_ARGB bitmap, int[] RETURN, int[] mask, Bitmap_IntPoint radiusSize, Bitmap_MirrorMap mirrorMap, Bitmap_IntRect roiRect)
        {
            int[] pixels = bitmap.Pixels;

            int mirror_start_x = roiRect.X + radiusSize.X;
            int mirror_end_x = mirror_start_x + roiRect.Width;

            int mirror_start_y = roiRect.Y + radiusSize.Y;
            int mirror_end_y = mirror_start_y + roiRect.Height;

            int mirror_start_mask_x = -radiusSize.X;
            int mirror_end_mask_x = radiusSize.X + 1;

            int mirror_start_mask_y = -radiusSize.Y;
            int mirror_end_mask_y = radiusSize.Y + 1;

            int[] table_Vertical = mirrorMap.Map_Vertical;
            int[] table_Horizontal = mirrorMap.Map_Horizontal;

            //需鏡像區域進行轉換，模擬全局
            for (int y = mirror_start_y; y < mirror_end_y; y++)
            {
                int res_pixel = (y - radiusSize.Y) * bitmap.Width + (mirror_start_x - radiusSize.X);
                for (int x = mirror_start_x; x < mirror_end_x; x++)
                {
                    int redValue = 0;
                    int greenValue = 0;
                    int blueValue = 0;
                    int res_mask = 0;

                    for (int mask_y = mirror_start_mask_y; mask_y < mirror_end_mask_y; mask_y++)
                    {
                        for (int mask_x = mirror_start_mask_x; mask_x < mirror_end_mask_x; mask_x++)
                        {
                            int temp_pixel = pixels[table_Vertical[y + mask_y] + table_Horizontal[x + mask_x]];
                            byte r = (byte)(temp_pixel >> 16);
                            byte g = (byte)(temp_pixel >> 8);
                            byte b = (byte)(temp_pixel);
                            int plusValue = mask[res_mask];

                            redValue += r * plusValue;
                            greenValue += g * plusValue;
                            blueValue += b * plusValue;
                            res_mask++;
                        }
                    }
                    RETURN[res_pixel] = ((byte)(pixels[res_pixel] >> 24)) << 24 | Bitmap_Convert.Byte_By(redValue) << 16 | Bitmap_Convert.Byte_By(greenValue) << 8 | Bitmap_Convert.Byte_By(blueValue);
                    res_pixel++;
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>(權重值)遮罩底層_原圖非取代</summary>
        public static void BaseMaskX(Bitmap_ARGB bitmap, int[] RETURN, int[] mask, Bitmap_IntPoint radiusSize, double xValue)
        {
            int[] pixels = bitmap.Pixels;

            //影像長度步矩
            int width = bitmap.Width;
            int height = bitmap.Height;

            int mask_len_y = radiusSize.Y;
            int mask_len_x = radiusSize.X;

            int start_mask_y = -mask_len_y * width;
            int end_mask_y = (mask_len_y + 1) * width;

            int start_mask_x = -mask_len_x;
            int end_mask_x = mask_len_x + 1;

            int start_y = mask_len_y * width;
            int end_y = pixels.Length - start_y;

            int start_x = mask_len_x;
            int end_x = width - start_x;

            for (int y = start_y; y < end_y; y += width)
            {
                for (int x = start_x; x < end_x; x++)
                {
                    int res = 0;
                    int redValueTemp = 0;
                    int blueValueTemp = 0;
                    int greenValueTemp = 0;
                    int copyPixel;

                    for (int mask_y = start_mask_y; mask_y < end_mask_y; mask_y += width)
                    {
                        for (int mask_x = start_mask_x; mask_x < end_mask_x; mask_x++, res++)
                        {
                            copyPixel = pixels[y + x + mask_y + mask_x];
                            redValueTemp += ((byte)(copyPixel >> 16)) * mask[res];
                            greenValueTemp += ((byte)(copyPixel >> 8)) * mask[res];
                            blueValueTemp += ((byte)(copyPixel)) * mask[res];
                        }
                    }
                    RETURN[y + x] = (pixels[y + x] >> 24 << 24) | (Bitmap_Convert.Byte_By(redValueTemp * xValue) << 16) | (Bitmap_Convert.Byte_By(greenValueTemp * xValue) << 8) | Bitmap_Convert.Byte_By(blueValueTemp * xValue);
                }
            }
        }

        /// <summary>(權重值)鏡像遮罩底層_原圖非取代</summary>
        public static void BaseMirrorMaskX(Bitmap_ARGB bitmap, int[] RETURN, int[] mask, Bitmap_IntPoint radiusSize, Bitmap_MirrorMap mirrorMap, Bitmap_IntRect roiRect, double xValue)
        {
            int[] pixels = bitmap.Pixels;

            int mirror_start_x = roiRect.X + radiusSize.X;
            int mirror_end_x = mirror_start_x + roiRect.Width;

            int mirror_start_y = roiRect.Y + radiusSize.Y;
            int mirror_end_y = mirror_start_y + roiRect.Height;

            int mirror_start_mask_x = -radiusSize.X;
            int mirror_end_mask_x = radiusSize.X + 1;

            int mirror_start_mask_y = -radiusSize.Y;
            int mirror_end_mask_y = radiusSize.Y + 1;

            int[] table_Vertical = mirrorMap.Map_Vertical;
            int[] table_Horizontal = mirrorMap.Map_Horizontal;

            //需鏡像區域進行轉換，模擬全局
            for (int y = mirror_start_y; y < mirror_end_y; y++)
            {
                int res_pixel = (y - radiusSize.Y) * bitmap.Width + (mirror_start_x - radiusSize.X);
                for (int x = mirror_start_x; x < mirror_end_x; x++)
                {
                    int redValue = 0;
                    int greenValue = 0;
                    int blueValue = 0;
                    int res_mask = 0;

                    for (int mask_y = mirror_start_mask_y; mask_y < mirror_end_mask_y; mask_y++)
                    {
                        for (int mask_x = mirror_start_mask_x; mask_x < mirror_end_mask_x; mask_x++)
                        {
                            int temp_pixel = pixels[table_Vertical[y + mask_y] + table_Horizontal[x + mask_x]];
                            byte r = (byte)(temp_pixel >> 16);
                            byte g = (byte)(temp_pixel >> 8);
                            byte b = (byte)(temp_pixel);
                            int plusValue = mask[res_mask];

                            redValue += r * plusValue;
                            greenValue += g * plusValue;
                            blueValue += b * plusValue;
                            res_mask++;
                        }
                    }
                    RETURN[res_pixel] = ((byte)(pixels[res_pixel] >> 24)) << 24 | Bitmap_Convert.Byte_By(redValue * xValue) << 16 | Bitmap_Convert.Byte_By(greenValue * xValue) << 8 | Bitmap_Convert.Byte_By(blueValue * xValue);
                    res_pixel++;
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>遮罩_原圖非取代</summary>
        public static void Mask(Bitmap_ARGB bitmap, int[] RETURN, int[,] mask)
        {
            Bitmap_IntPoint radiusSize = new Bitmap_IntPoint((mask.GetLength(1) - 1) >> 1, (mask.GetLength(0) - 1) >> 1);
            int[] arrayMask = Bitmap_Convert.IntArray_By(mask);
            BaseMask(bitmap, RETURN, arrayMask, radiusSize);
        }

        /// <summary>遮罩</summary>
        public static void Mask(Bitmap_ARGB bitmap, int[,] mask)
        {
            int[] RETURN = new int[bitmap.Pixels.Length];
            Mask(bitmap, RETURN, mask);
            bitmap.Pixels = RETURN;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>(權重值)遮罩_原圖非取代</summary>
        public static void MaskX(Bitmap_ARGB bitmap, int[] RETURN, int[,] mask, double xValue)
        {
            int[] arrayMask = Bitmap_Convert.IntArray_By(mask);
            Bitmap_IntPoint radiusSize = new Bitmap_IntPoint((mask.GetLength(1) - 1) >> 1, (mask.GetLength(0) - 1) >> 1);
            BaseMaskX(bitmap, RETURN, arrayMask, radiusSize, xValue);
        }

        /// <summary>(權重值)遮罩</summary>
        public static void MaskX(Bitmap_ARGB bitmap, int[,] mask, double xValue)
        {
            int[] RETURN = new int[bitmap.Pixels.Length];
            MaskX(bitmap, RETURN, mask, xValue);
            bitmap.Pixels = RETURN;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>鏡像遮罩_原圖非取代</summary>
        public static void MirrorMask(Bitmap_ARGB bitmap, int[] RETURN, int[,] mask, Bitmap_MirrorMap mirrorMap)
        {
            //遮罩半徑
            Bitmap_IntPoint radiusSize = new Bitmap_IntPoint((int)((mask.GetLength(1) - 1) * 0.5), (int)((mask.GetLength(0) - 1) * 0.5));

            //指定矩形位置
            Bitmap_IntRect roiRect = new Bitmap_IntRect();

            //將2維陣列遮罩轉換1維
            int[] arrayMask = Bitmap_Convert.IntArray_By(mask);

            //非鏡射遮罩
            BaseMask(bitmap, RETURN, arrayMask, radiusSize);

            //鏡射遮罩
            roiRect.X = 0;
            roiRect.Y = 0;
            roiRect.Width = bitmap.Width;
            roiRect.Height = radiusSize.Y;
            BaseMirrorMask(bitmap, RETURN, arrayMask, radiusSize, mirrorMap, roiRect);

            roiRect.X = 0;
            roiRect.Y = bitmap.Height - radiusSize.Y;
            roiRect.Width = bitmap.Width;
            roiRect.Height = radiusSize.Y;
            BaseMirrorMask(bitmap, RETURN, arrayMask, radiusSize, mirrorMap, roiRect);

            roiRect.X = 0;
            roiRect.Y = radiusSize.Y;
            roiRect.Width = radiusSize.X;
            roiRect.Height = bitmap.Height - (radiusSize.Y * 2);
            BaseMirrorMask(bitmap, RETURN, arrayMask, radiusSize, mirrorMap, roiRect);

            roiRect.X = bitmap.Width - radiusSize.X;
            roiRect.Y = radiusSize.Y;
            roiRect.Width = radiusSize.X;
            roiRect.Height = bitmap.Height - (radiusSize.Y * 2);
            BaseMirrorMask(bitmap, RETURN, arrayMask, radiusSize, mirrorMap, roiRect);
        }

        /// <summary>鏡像遮罩_原圖非取代</summary>
        public static void MirrorMask(Bitmap_ARGB bitmap, int[] RETURN, int[,] mask)
        {
            Bitmap_MirrorMap mirrorMap = new Bitmap_MirrorMap(bitmap, mask);
            MirrorMask(bitmap, RETURN, mask, mirrorMap);
        }

        /// <summary>鏡像遮罩</summary>
        public static void MirrorMask(Bitmap_ARGB bitmap, int[,] mask, Bitmap_MirrorMap mirrorMap)
        {
            //影像緩衝陣列
            int[] RETURN = new int[bitmap.Pixels.Length];
            MirrorMask(bitmap, RETURN, mask, mirrorMap);
            bitmap.Pixels = RETURN;
        }

        /// <summary>鏡像遮罩</summary>
        public static void MirrorMask(Bitmap_ARGB bitmap, int[,] mask)
        { MirrorMask(bitmap, mask, new Bitmap_MirrorMap(bitmap, mask)); }

        /// <summary>(權重值)鏡像遮罩_原圖非取代</summary>
        public static void MirrorMaskX(Bitmap_ARGB bitmap, int[] RETURN, int[,] mask, Bitmap_MirrorMap mirrorMap, double xValue)
        {
            //遮罩半徑
            Bitmap_IntPoint radiusSize = new Bitmap_IntPoint((int)((mask.GetLength(1) - 1) * 0.5), (int)((mask.GetLength(0) - 1) * 0.5));

            //指定矩形位置
            Bitmap_IntRect roiRect = new Bitmap_IntRect();

            //將2維陣列遮罩轉換1維
            int[] arrayMask = Bitmap_Convert.IntArray_By(mask);

            //非鏡射遮罩
            BaseMaskX(bitmap, RETURN, arrayMask, radiusSize, xValue);

            //鏡射遮罩
            roiRect.X = 0;
            roiRect.Y = 0;
            roiRect.Width = bitmap.Width;
            roiRect.Height = radiusSize.Y;
            BaseMirrorMaskX(bitmap, RETURN, arrayMask, radiusSize, mirrorMap, roiRect, xValue);

            roiRect.X = 0;
            roiRect.Y = bitmap.Height - radiusSize.Y;
            roiRect.Width = bitmap.Width;
            roiRect.Height = radiusSize.Y;
            BaseMirrorMaskX(bitmap, RETURN, arrayMask, radiusSize, mirrorMap, roiRect, xValue);

            roiRect.X = 0;
            roiRect.Y = radiusSize.Y;
            roiRect.Width = radiusSize.X;
            roiRect.Height = bitmap.Height - (radiusSize.Y * 2);
            BaseMirrorMaskX(bitmap, RETURN, arrayMask, radiusSize, mirrorMap, roiRect, xValue);

            roiRect.X = bitmap.Width - radiusSize.X;
            roiRect.Y = radiusSize.Y;
            roiRect.Width = radiusSize.X;
            roiRect.Height = bitmap.Height - (radiusSize.Y * 2);
            BaseMirrorMaskX(bitmap, RETURN, arrayMask, radiusSize, mirrorMap, roiRect, xValue);
        }

        /// <summary>(權重值)鏡像遮罩_原圖非取代</summary>
        public static void X_MirrorMask(Bitmap_ARGB bitmap, int[] RETURN, int[,] mask, double xValue)
        { MirrorMaskX(bitmap, RETURN, mask, new Bitmap_MirrorMap(bitmap, mask), xValue); }

        /// <summary>(權重值)鏡像遮罩</summary>
        public static void X_MirrorMask(Bitmap_ARGB bitmap, int[,] mask, Bitmap_MirrorMap mirrorMap, double xValue)
        {
            //影像緩衝陣列
            int[] RETURN = new int[bitmap.Pixels.Length];
            MirrorMaskX(bitmap, RETURN, mask, mirrorMap, xValue);
            bitmap.Pixels = RETURN;
        }

        /// <summary>(權重值)鏡像遮罩</summary>
        public static void X_MirrorMask(Bitmap_ARGB bitmap, int[,] mask, double xValue)
        { X_MirrorMask(bitmap, mask, new Bitmap_MirrorMap(bitmap, mask), xValue); }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>為圖片保留指定色彩</summary>
        public static void Reserve(Bitmap_ARGB bitmap, List<Bitmap_Color> colors, int permissible)
        {
            //不符合條件轉換色彩
            byte changeR = 0;
            byte changeG = 0;
            byte changeB = 0;

            //像素陣列
            int[] pixels = bitmap.Pixels;
            int[] copy_pixels = new int[pixels.Length];
            for (int i = 0; i < pixels.Length; i++)
            { copy_pixels[i] = (pixels[i] >> 24 << 24) | (changeR << 16) | (changeG << 8) | (changeB); }

            //誤差變數
            int temp;

            for (int index_c = 0; index_c < colors.Count; index_c++)
            {
                byte ifR = colors[index_c].R;
                byte ifG = colors[index_c].G;
                byte ifB = colors[index_c].B;

                byte pR;
                byte pG;
                byte pB;

                for (int i = 0; i < pixels.Length; i++)
                {
                    pR = ((byte)(pixels[i] >> 16));
                    pG = ((byte)(pixels[i] >> 8));
                    pB = ((byte)(pixels[i]));

                    //R色彩的差距
                    temp = pR - ifR;
                    //負轉正數 Math.abs作用
                    temp = (temp ^ (temp >> 31)) - (temp >> 31);
                    if (temp <= permissible)
                    {
                        //G色彩的差距
                        temp = pG - ifG;
                        temp = (temp ^ (temp >> 31)) - (temp >> 31);
                        if (temp <= permissible)
                        {
                            //B色彩的差距
                            temp = pB - ifB;
                            temp = (temp ^ (temp >> 31)) - (temp >> 31);
                            if (temp <= permissible)
                            { copy_pixels[i] = 255 << 24 | 255 << 16 | 255 << 8 | 255; }
                        }
                    }
                }
            }
            bitmap.Pixels = copy_pixels;
        }

        /// <summary>亮度調整 -100 ~ 100</summary>
        public static void Brightness(Bitmap_ARGB bitmap, int value)
        {
            int[] pixels = bitmap.Pixels;
            int convertValue = (int)(value * 2.55);
            byte[] colorMap = new byte[256];

            for (int i = 0; i < 256; i++)
            { colorMap[i] = Bitmap_Convert.Byte_By(i + convertValue); }

            Bitmap_ColorMap.Use_ColorMap(bitmap, colorMap);
        }

        /// <summary>對比調整 -100 ~ 100</summary>
        public static void Contrast(Bitmap_ARGB bitmap, int value)
        {
            double convertValue = (value + 100) * 0.01;
            int[] pixels = bitmap.Pixels;
            byte[] colorMap = new byte[256];

            for (int i = 0; i < 256; i++)
            { colorMap[i] = Bitmap_Convert.Byte_By((i - 127) * convertValue + 127); }

            Bitmap_ColorMap.Use_ColorMap(bitmap, colorMap);
        }

        /// <summary>灰階化</summary>
        public static void Gray(Bitmap_ARGB bitmap)
        {
            int[] intPixels = bitmap.Pixels;

            for (int i = 0; i < intPixels.Length; i++)
            {
                int intPixel = intPixels[i];

                int a = (byte)(intPixel >> 24);
                int r = (byte)(intPixel >> 16);
                int g = (byte)(intPixel >> 8);
                int b = (byte)(intPixel);

                byte gray = (byte)((r * 19595 + g * 38469 + b * 7472) >> 16);

                intPixels[i] = a << 24 | gray << 16 | gray << 8 | gray;
            }
        }

        /// <summary>銳利化</summary>
        public static void Sharpen(Bitmap_ARGB bitmap)
        {
            int[] RETURN = new int[bitmap.Pixels.Length];
            int[] Pixels = bitmap.Pixels;


            X_MirrorMask(bitmap, RETURN, Bitmap_MaskData.Sharpen, 0.125);

            int canValue = 32;

            for (int i = 0; i < Pixels.Length; i++)
            {
                int nowRePixel = RETURN[i];
                int nowPixel = Pixels[i];

                int nowPixelA = (byte)(nowPixel >> 24);
                int nowPixelR = (byte)(nowPixel >> 16);
                int nowPixelG = (byte)(nowPixel >> 8);
                int nowPixelB = (byte)(nowPixel);

                int spaceR = (byte)(nowRePixel >> 16) - nowPixelR;
                int spaceG = (byte)(nowRePixel >> 8) - nowPixelG;
                int spaceB = (byte)(nowRePixel) - nowPixelB;

                if (spaceR > canValue || spaceR < -canValue)
                { spaceR = spaceR >> 3; }

                if (spaceG > canValue || spaceR < -canValue)
                { spaceG = spaceG >> 3; }

                if (spaceB > canValue || spaceR < -canValue)
                { spaceB = spaceB >> 3; }

                Pixels[i] = (nowPixelA << 24) | ((nowPixelR + spaceR) << 16) | ((nowPixelG + spaceG) << 8) | (nowPixelB + spaceB);
            }
        }

        /// <summary>直方圖等化</summary>
        public static void Equlize(Bitmap_ARGB bitmap)
        {
            Int32 pixels_All = bitmap.Pixels.Length;
            Int32 pixels_Sum_R = 0;
            Int32 pixels_Sum_G = 0;
            Int32 pixels_Sum_B = 0;


            Int32[] channelData_R = Bitmap_Histogram.GetChannelData(bitmap.Pixels, "Red");
            Int32[] channelData_G = Bitmap_Histogram.GetChannelData(bitmap.Pixels, "Green");
            Int32[] channelData_B = Bitmap_Histogram.GetChannelData(bitmap.Pixels, "Blue");
            byte[] colorMapR = new byte[256];
            byte[] colorMapG = new byte[256];
            byte[] colorMapB = new byte[256];

            //產生色彩映射表
            for (int i = 0; i < 256; i++)
            {
                pixels_Sum_R += channelData_R[i];
                pixels_Sum_G += channelData_G[i];
                pixels_Sum_B += channelData_B[i];

                colorMapR[i] = (byte)(((float)pixels_Sum_R / (float)pixels_All) * 255);
                colorMapG[i] = (byte)(((float)pixels_Sum_G / (float)pixels_All) * 255);
                colorMapB[i] = (byte)(((float)pixels_Sum_B / (float)pixels_All) * 255);

            }

            Bitmap_ColorMap.Use_ColorMap(bitmap, colorMapR, colorMapG, colorMapB);
        }
    }
}


//特殊工具
namespace PlusObj
{
    public class Bitmap_OtherTool
    {
        /// <summary>複製特定區域像素到指定像素陣列</summary>
        public static void CopyRoiToAll(int[] roiPixels, Bitmap_IntRect roiSize, Bitmap_ARGB bitmap)
        {
            int bitmapWidth = bitmap.Width;
            int[] allPixels = bitmap.Pixels;

            int startY = roiSize.Y;
            int endY = startY + roiSize.Height;

            int startX = roiSize.X;
            int endX = startX + roiSize.Width;

            for (int y = startY; y < endY; y++)
            {
                int indexPixels = y * bitmap.Width + startX;

                for (int x = startX; x < endX; x++)
                {
                    allPixels[indexPixels] = roiPixels[indexPixels];
                    ++indexPixels;
                }
            }
        }

        /// <summary>檢查圖片是否相同</summary>
        public static void CheckBitmap(int[] roiPixels, int[] allPixels)
        {
            for (int i = 0; i < roiPixels.Length; i++)
            {
                if (roiPixels[i] == allPixels[i])
                { 
                    allPixels[i] = 255 << 24 | 255 << 8;
                }
                else
                {
                    ;
                }
            }
        }

        /// <summary>像素寫入到指定圖像</summary>
        public static void PixelsToBitmap(Bitmap bitmap, int[] pixels)
        {
            Rectangle bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData bitmapData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr Scan0 = bitmapData.Scan0;

            unsafe
            {
                int* pixelsPtr = (int*)(void*)Scan0;
                for (int i = 0; i < pixels.Length; i++)
                { pixelsPtr[i] = pixels[i]; }
            }
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>像素寫入到指定圖像</summary>
        public static void PixelsToBitmap(Bitmap bitmap, byte[] pixels)
        {
            Rectangle bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData bitmapData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr Scan0 = bitmapData.Scan0;

            unsafe
            {
                int* pixelsPtr = (int*)(void*)Scan0;

                int res = 0;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++, res++)
                    {
                        byte bitmapPixel = pixels[res];

                        pixelsPtr[res] = 255 << 24 | bitmapPixel << 16 | bitmapPixel << 8 | bitmapPixel;
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>圖像寫入到指定像素</summary>
        public static void BitmapToPixels(Bitmap bitmap, int[] pixels)
        {
            Rectangle bitmapRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            BitmapData bitmapData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            IntPtr Scan0 = bitmapData.Scan0;

            unsafe
            {
                int* pixelsPtr = (int*)(void*)Scan0;

                int res = 0;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++, res++)
                    { pixels[res] = pixelsPtr[res]; }
                }
            }

            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>讀取網路影像寫入像素</summary>
        public static void JpgArrayToPixels(byte[] data, int[] pixels)
        {
            //將網路串流讀取成為影像
            using (MemoryStream stream = new MemoryStream(data))
            {
                Bitmap bitmap = Image.FromStream(stream) as Bitmap;
                Bitmap_OtherTool.BitmapToPixels(bitmap, pixels);
            }
        }

        /// <summary>網路影像寫入ARGB圖_擁有較高容錯率</summary>
        public static void JpgArrayToBitmapARGB(byte[] data, Bitmap_ARGB bitmapARGB)
        {
            using (Bitmap bitmap = Bitmap_Convert.Bitmap_By(data))
            {
                if (bitmapARGB == null)
                { bitmapARGB.Create(bitmap.Width, bitmap.Height); }

                //大小不合符合
                else if (bitmap.Width != bitmapARGB.Width || bitmap.Height != bitmapARGB.Height)
                { bitmapARGB.Create(bitmap.Width, bitmap.Height); }

                BitmapToPixels(bitmap, bitmapARGB.Pixels);
            }
        }
    }

    public class Bitmap_MaskData
    {
        public static int[,] Sharpen = new int[3, 3]
        {
            {-1,-2,-1},
            {-2,20,-2},
            {-1,-2,-1}
        };

        public static int[,] GaussMask = new int[3, 3]
        {
            {-1,-2,-1},
            {-2,12,-2},
            {-1,-2,-1}
        };
    }
}