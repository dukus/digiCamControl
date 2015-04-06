using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CameraControl.Classes
{
    public static class WriteableBitmapExtensionsLocal 
    {
        public static unsafe void Blit(this WriteableBitmap bmp, Rect destRect, WriteableBitmap source, Rect sourceRect, Color color, WriteableBitmapExtensions.BlendMode blendMode)
        {
            if ((int)color.A == 0)
                return;
            bool flag1 = source.Format == PixelFormats.Pbgra32 || source.Format == PixelFormats.Prgba64 || source.Format == PixelFormats.Prgba128Float;
            int num1 = (int)destRect.Width;
            int num2 = (int)destRect.Height;
            using (BitmapContext bitmapContext1 = WriteableBitmapContextExtensions.GetBitmapContext(source, ReadWriteMode.ReadOnly))
            {
                using (BitmapContext bitmapContext2 = WriteableBitmapContextExtensions.GetBitmapContext(bmp))
                {
                    int width1 = bitmapContext1.Width;
                    int width2 = bitmapContext2.Width;
                    int height = bitmapContext2.Height;
                    Rect rect = new Rect(0.0, 0.0, (double)width2, (double)height);
                    rect.Intersect(destRect);
                    if (rect.IsEmpty)
                        return;
                    int* pixels1 = bitmapContext1.Pixels;
                    int* pixels2 = bitmapContext2.Pixels;
                    int length = bitmapContext1.Length;
                    int num3 = (int)destRect.X;
                    int num4 = (int)destRect.Y;
                    int num5 = 0;
                    int num6 = 0;
                    int num7 = 0;
                    int num8 = 0;
                    int num9 = (int)color.A;
                    int num10 = (int)color.R;
                    int num11 = (int)color.G;
                    int num12 = (int)color.B;
                    bool flag2 = color != Colors.White;
                    int num13 = (int)sourceRect.Width;
                    double num14 = sourceRect.Width / destRect.Width;
                    double num15 = sourceRect.Height / destRect.Height;
                    int num16 = (int)sourceRect.X;
                    int num17 = (int)sourceRect.Y;
                    int num18 = -1;
                    int num19 = -1;
                    double num20 = (double)num17;
                    int num21 = num4;
                    for (int index1 = 0; index1 < num2; ++index1)
                    {
                        if (num21 >= 0 && num21 < height)
                        {
                            double num22 = (double)num16;
                            int index2 = num3 + num21 * width2;
                            int num23 = num3;
                            int num24 = *pixels1;
                            if (blendMode == WriteableBitmapExtensions.BlendMode.None && !flag2)
                            {
                                int num25 = (int)num22 + (int)num20 * width1;
                                int num26 = num23 < 0 ? -num23 : 0;
                                int num27 = num23 + num26;
                                int num28 = width1 - num26;
                                int num29 = num27 + num28 < width2 ? num28 : width2 - num27;
                                if (num29 > num13)
                                    num29 = num13;
                                if (num29 > num1)
                                    num29 = num1;
                                BitmapContext.BlockCopy(bitmapContext1, (num25 + num26) * 4, bitmapContext2, (index2 + num26) * 4, num29 * 4);
                            }
                            else
                            {
                                for (int index3 = 0; index3 < num1; ++index3)
                                {
                                    if (num23 >= 0 && num23 < width2)
                                    {
                                        if ((int)num22 != num18 || (int)num20 != num19)
                                        {
                                            int index4 = (int)num22 + (int)num20 * width1;
                                            if (index4 >= 0 && index4 < length)
                                            {
                                                num24 = pixels1[index4];
                                                num8 = num24 >> 24 & (int)byte.MaxValue;
                                                num5 = num24 >> 16 & (int)byte.MaxValue;
                                                num6 = num24 >> 8 & (int)byte.MaxValue;
                                                num7 = num24 & (int)byte.MaxValue;
                                                if (flag2 && num8 != 0)
                                                {
                                                    num8 = num8 * num9 * 32897 >> 23;
                                                    num5 = (num5 * num10 * 32897 >> 23) * num9 * 32897 >> 23;
                                                    num6 = (num6 * num11 * 32897 >> 23) * num9 * 32897 >> 23;
                                                    num7 = (num7 * num12 * 32897 >> 23) * num9 * 32897 >> 23;
                                                    num24 = num8 << 24 | num5 << 16 | num6 << 8 | num7;
                                                }
                                            }
                                            else
                                                num8 = 0;
                                        }
                                        if (blendMode == WriteableBitmapExtensions.BlendMode.None)
                                            pixels2[index2] = num24;
                                        else if (blendMode == WriteableBitmapExtensions.BlendMode.ColorKeying)
                                        {
                                            num5 = num24 >> 16 & (int)byte.MaxValue;
                                            num6 = num24 >> 8 & (int)byte.MaxValue;
                                            num7 = num24 & (int)byte.MaxValue;
                                            if (num5 != (int)color.R || num6 != (int)color.G || num7 != (int)color.B)
                                                pixels2[index2] = num24;
                                        }
                                        else if (blendMode == WriteableBitmapExtensions.BlendMode.Mask)
                                        {
                                            int num25 = pixels2[index2];
                                            int num26 = num25 >> 24 & (int)byte.MaxValue;
                                            int num27 = num25 >> 16 & (int)byte.MaxValue;
                                            int num28 = num25 >> 8 & (int)byte.MaxValue;
                                            int num29 = num25 & (int)byte.MaxValue;
                                            int num30 = num26 * num8 * 32897 >> 23 << 24 | num27 * num8 * 32897 >> 23 << 16 | num28 * num8 * 32897 >> 23 << 8 | num29 * num8 * 32897 >> 23;
                                            pixels2[index2] = num30;
                                        }
                                        else if (num8 > 0)
                                        {
                                            int num25 = pixels2[index2];
                                            int num26 = num25 >> 24 & (int)byte.MaxValue;
                                            if ((num8 == (int)byte.MaxValue || num26 == 0) && (blendMode != WriteableBitmapExtensions.BlendMode.Additive && blendMode != WriteableBitmapExtensions.BlendMode.Subtractive) && blendMode != WriteableBitmapExtensions.BlendMode.Multiply)
                                            {
                                                pixels2[index2] = num24;
                                            }
                                            else
                                            {
                                                int num27 = num25 >> 16 & (int)byte.MaxValue;
                                                int num28 = num25 >> 8 & (int)byte.MaxValue;
                                                int num29 = num25 & (int)byte.MaxValue;
                                                if (blendMode == WriteableBitmapExtensions.BlendMode.Alpha)
                                                {
                                                    int num30 = (int)byte.MaxValue - num8;
                                                    num25 = !flag1 ? (num26 & (int)byte.MaxValue) << 24 | (num5 * num8 + num30 * num27 >> 8 & (int)byte.MaxValue) << 16 | (num6 * num8 + num30 * num28 >> 8 & (int)byte.MaxValue) << 8 | num7 * num8 + num30 * num29 >> 8 & (int)byte.MaxValue : (num26 & (int)byte.MaxValue) << 24 | ((num5 << 8) + num30 * num27 >> 8 & (int)byte.MaxValue) << 16 | ((num6 << 8) + num30 * num28 >> 8 & (int)byte.MaxValue) << 8 | (num7 << 8) + num30 * num29 >> 8 & (int)byte.MaxValue;
                                                }
                                                else if (blendMode == WriteableBitmapExtensions.BlendMode.Additive)
                                                {
                                                    int num30 = (int)byte.MaxValue <= num8 + num26 ? (int)byte.MaxValue : num8 + num26;
                                                    num25 = num30 << 24 | (num30 <= num5 + num27 ? num30 : num5 + num27) << 16 | (num30 <= num6 + num28 ? num30 : num6 + num28) << 8 | (num30 <= num7 + num29 ? num30 : num7 + num29);
                                                }
                                                else if (blendMode == WriteableBitmapExtensions.BlendMode.Subtractive)
                                                    num25 = num26 << 24 | (num5 >= num27 ? 0 : num5 - num27) << 16 | (num6 >= num28 ? 0 : num6 - num28) << 8 | (num7 >= num29 ? 0 : num7 - num29);
                                                else if (blendMode == WriteableBitmapExtensions.BlendMode.Multiply)
                                                {
                                                    int num30 = num8 * num26 + 128;
                                                    int num31 = num5 * num27 + 128;
                                                    int num32 = num6 * num28 + 128;
                                                    int num33 = num7 * num29 + 128;
                                                    int num34 = (num30 >> 8) + num30 >> 8;
                                                    int num35 = (num31 >> 8) + num31 >> 8;
                                                    int num36 = (num32 >> 8) + num32 >> 8;
                                                    int num37 = (num33 >> 8) + num33 >> 8;
                                                    num25 = num34 << 24 | (num34 <= num35 ? num34 : num35) << 16 | (num34 <= num36 ? num34 : num36) << 8 | (num34 <= num37 ? num34 : num37);
                                                }
                                                pixels2[index2] = num25;
                                            }
                                        }
                                    }
                                    ++num23;
                                    ++index2;
                                    num22 += num14;
                                }
                            }
                        }
                        num20 += num15;
                        ++num21;
                    }
                }
            }
        }

    }
}
