// Copyright © 2015 dhq_boiler.

using System;
using System.Runtime.CompilerServices;

namespace libSevenToolsCore.WPFControls.Imaging
{
    internal static class Scaler
    {
        internal unsafe static bool Interplate(Interpolation method, byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0)
        {
            return method switch
            {
                Interpolation.NearestNeighbor => NearestNeighbor(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0),
                Interpolation.Bilinear => Bilinear(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0),
                Interpolation.Bicubic => Bicubic(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0),
                _ => throw new NotSupportedException(),
            };
        }

        internal unsafe static bool Interplate(Interpolation method, byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0, out int p1, out int p2)
        {
            return method switch
            {
                Interpolation.NearestNeighbor => NearestNeighbor(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2),
                Interpolation.Bilinear => Bilinear(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2),
                Interpolation.Bicubic => Bicubic(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2),
                _ => throw new NotSupportedException(),
            };
        }

        internal unsafe static bool Interplate(Interpolation method, byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0, out int p1, out int p2, out int p3)
        {
            return method switch
            {
                Interpolation.NearestNeighbor => NearestNeighbor(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2, out p3),
                Interpolation.Bilinear => Bilinear(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2, out p3),
                Interpolation.Bicubic => Bicubic(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2, out p3),
                _ => throw new NotSupportedException(),
            };
        }

        private static unsafe bool Bicubic(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0)
        {
            int px = (int)x;
            int py = (int)y;

            // Check if the coordinates are within the specified bounds
            if (min_x > px || max_x < px || min_y > py || max_y < py)
            {
                p0 = 0;
                return false;
            }

            // Initialize buffer for the bicubic interpolation
            int[] buf0 = new int[16];

            // Populate the buffer with the pixel values around the target (x, y)
            for (int ly2 = 0; ly2 < 4; ++ly2)
            {
                for (int lx2 = 0; lx2 < 4; ++lx2)
                {
                    int pxlx2 = px + lx2 - 1;
                    int pyly2 = py + ly2 - 1;
                    int adr2 = ly2 * 4 + lx2;

                    // Use the pixel value if it's within the image bounds, otherwise use the default value
                    buf0[adr2] = (pxlx2 >= min_x && pxlx2 <= max_x && pyly2 >= min_y && pyly2 <= max_y) ?
                        *(p_s + pyly2 * step + pxlx2 * channels) : 255 / 2;
                }
            }

            // Perform bicubic interpolation
            p0 = biCubic(x, y, buf0);
            return true;
        }


        private static unsafe bool Bicubic(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0, out int p1, out int p2)
        {
            int px = (int)x;
            int py = (int)y;

            // Check if the coordinates are within the specified bounds
            if (min_x > px || max_x < px || min_y > py || max_y < py)
            {
                p0 = p1 = p2 = 0;
                return false;
            }

            // Initialize buffers for each color channel
            int[] buf0 = new int[16];
            int[] buf1 = new int[16];
            int[] buf2 = new int[16];

            // Populate the buffers with pixel values around the target (x, y)
            for (int ly2 = 0; ly2 < 4; ++ly2)
            {
                int adr2 = ly2 * 4;
                for (int lx2 = 0; lx2 < 4; ++lx2)
                {
                    int pxlx2 = px + lx2 - 1;
                    int pyly2 = py + ly2 - 1;

                    // Check if the neighbor pixel is within bounds
                    if (pxlx2 >= min_x && pxlx2 <= max_x && pyly2 >= min_y && pyly2 <= max_y)
                    {
                        byte* p = p_s + pyly2 * step + pxlx2 * channels;
                        buf0[adr2] = *p;
                        buf1[adr2] = *(p + 1);
                        buf2[adr2] = *(p + 2);
                    }
                    else
                    {
                        // Use a default value for out-of-bounds pixels
                        buf0[adr2] = 255 / 2;
                        buf1[adr2] = 255 / 2;
                        buf2[adr2] = 255 / 2;
                    }
                    ++adr2;
                }
            }

            // Perform bicubic interpolation for each channel
            p0 = biCubic(x, y, buf0);
            p1 = biCubic(x, y, buf1);
            p2 = biCubic(x, y, buf2);
            return true;
        }


        private static unsafe bool Bicubic(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0, out int p1, out int p2, out int p3)
        {
            int px = (int)x;
            int py = (int)y;

            if (min_x > px || max_x < px || min_y > py || max_y < py)
            {
                p0 = p1 = p2 = p3 = 0;
                return false;
            }

            int[][] buffers = { new int[16], new int[16], new int[16], new int[16] };

            for (int ly2 = 0; ly2 < 4; ++ly2)
            {
                for (int lx2 = 0; lx2 < 4; ++lx2)
                {
                    int pxlx2 = px + lx2 - 1;
                    int pyly2 = py + ly2 - 1;
                    int adr2 = ly2 * 4 + lx2;

                    if (pxlx2 >= min_x && pxlx2 <= max_x && pyly2 >= min_y && pyly2 <= max_y)
                    {
                        byte* p = p_s + pyly2 * step + pxlx2 * channels;
                        for (int i = 0; i < 4; ++i)
                        {
                            buffers[i][adr2] = *(p + i);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            buffers[i][adr2] = 255 / 2; // Or another default value as needed
                        }
                    }
                }
            }

            // Parallel processing (if applicable)
            p0 = biCubic(x, y, buffers[0]);
            p1 = biCubic(x, y, buffers[1]);
            p2 = biCubic(x, y, buffers[2]);
            p3 = biCubic(x, y, buffers[3]);

            return true;
        }


        private static int biCubic(double x, double y, int[] buf)
        {
            if (buf == null || buf.Length != 16)
            {
                throw new ArgumentException("Buffer must be a 16-element array.");
            }

            double[] fx = new double[4];
            double[] fy = new double[4];
            double[] colResults = new double[4];

            int px = (int)x;
            int py = (int)y;
            double dx = x - px;
            double dy = y - py;

            for (int i = 0; i < 4; i++)
            {
                fx[i] = cubicFunc(1.0 - dx + i);
                fy[i] = cubicFunc(1.0 - dy + i);
            }

            for (int i = 0; i < 4; ++i)
            {
                double[] column = new double[4];
                for (int j = 0; j < 4; ++j)
                {
                    column[j] = buf[j * 4 + i];
                }
                colResults[i] = naiseki(fy, column);
            }

            int res = (int)naiseki(colResults, fx);

            // Clamping the result to the range of byte
            return Math.Clamp(res, byte.MinValue, byte.MaxValue);
        }


        private static double naiseki(double[] x, double[] y)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentException("Arrays must be of the same length.");
            }

            double res = 0.0;
            for (int i = 0; i < x.Length; ++i)
            {
                res += (x[i] * y[i]);
            }
            return res;
        }


        private static double cubicFunc(double x)
        {
            double ax = Math.Abs(x);

            if (ax < 1.0)
            {
                return 1.0 - 2.0 * ax * ax + ax * ax * ax;
            }
            else if (ax < 2.0)
            {
                return 4.0 - 8.0 * ax + 5.0 * ax * ax - ax * ax * ax;
            }

            return 0.0;
        }


        private static unsafe bool Bilinear(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0)
        {
            int px = (int)x;
            int py = (int)y;
            double xrate = x - px;
            double yrate = y - py;

            // Initialize pixel values to a default (consider if 128 is appropriate)
            int B0 = 128, B1 = 128, B2 = 128, B3 = 128;

            // Assign values to the corners if they are within the region
            if (IsOnRegion(px, py, min_x, max_x, min_y, max_y))
            {
                byte* p = p_s + py * step + px * channels;
                B0 = *p;
            }
            if (IsOnRegion(px, py + 1, min_x, max_x, min_y, max_y))
            {
                byte* p = p_s + (py + 1) * step + px * channels;
                B1 = *p;
            }
            if (IsOnRegion(px + 1, py, min_x, max_x, min_y, max_y))
            {
                byte* p = p_s + py * step + (px + 1) * channels;
                B2 = *p;
            }
            if (IsOnRegion(px + 1, py + 1, min_x, max_x, min_y, max_y))
            {
                byte* p = p_s + (py + 1) * step + (px + 1) * channels;
                B3 = *p;
            }

            // If none of the neighboring pixels are in the region, return false
            if (B0 == 128 && B1 == 128 && B2 == 128 && B3 == 128)
            {
                p0 = 0;
                return false;
            }

            // Perform bilinear interpolation
            p0 = linarPol(B0, B1, B2, B3, xrate, yrate);
            return true;
        }


        private static unsafe bool Bilinear(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0, out int p1, out int p2)
        {
            int px = (int)x;
            int py = (int)y;
            double xrate = x - px;
            double yrate = y - py;

            // 範囲チェックの効率化
            bool isTopInRange = IsOnRegion(px, py, min_x, max_x, min_y, max_y);
            bool isBottomInRange = IsOnRegion(px, py + 1, min_x, max_x, min_y, max_y);
            bool isRightInRange = IsOnRegion(px + 1, py, min_x, max_x, min_y, max_y);
            bool isRightBottomInRange = IsOnRegion(px + 1, py + 1, min_x, max_x, min_y, max_y);

            if (!(isTopInRange || isBottomInRange || isRightInRange || isRightBottomInRange))
            {
                p0 = p1 = p2 = 0;
                return false;
            }

            // 各方向のピクセル値を一度に取得
            byte* p;
            int[] values = new int[12]; // 各ピクセルのRGB値を格納
            if (isTopInRange)
            {
                p = p_s + py * step + px * channels;
                for (int i = 0; i < 3; i++)
                {
                    values[i] = p[i];
                }
            }
            if (isBottomInRange)
            {
                p = p_s + (py + 1) * step + px * channels;
                for (int i = 0; i < 3; i++)
                {
                    values[3 + i] = p[i];
                }
            }
            if (isRightInRange)
            {
                p = p_s + py * step + (px + 1) * channels;
                for (int i = 0; i < 3; i++)
                {
                    values[6 + i] = p[i];
                }
            }
            if (isRightBottomInRange)
            {
                p = p_s + (py + 1) * step + (px + 1) * channels;
                for (int i = 0; i < 3; i++)
                {
                    values[9 + i] = p[i];
                }
            }

            // バイリニア補間の適用
            p0 = linarPol(values[0], values[3], values[6], values[9], xrate, yrate);
            p1 = linarPol(values[1], values[4], values[7], values[10], xrate, yrate);
            p2 = linarPol(values[2], values[5], values[8], values[11], xrate, yrate);

            return true;
        }


        private static unsafe bool Bilinear(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0, out int p1, out int p2, out int p3)
        {
            int px = (int)Math.Floor(x);
            int py = (int)Math.Floor(y);
            double xrate = x - px;
            double yrate = y - py;

            // ピクセル位置が画像の範囲内にあるかどうかのチェック
            bool isLeftTop = IsOnRegion(px, py, min_x, max_x, min_y, max_y);
            bool isRightTop = IsOnRegion(px + 1, py, min_x, max_x, min_y, max_y);
            bool isLeftBottom = IsOnRegion(px, py + 1, min_x, max_x, min_y, max_y);
            bool isRightBottom = IsOnRegion(px + 1, py + 1, min_x, max_x, min_y, max_y);

            if (!isLeftTop && !isRightTop && !isLeftBottom && !isRightBottom)
            {
                p0 = p1 = p2 = p3 = 0;
                return false;
            }

            // 各色チャネルの値を取得
            byte* p;
            int[] values = new int[16]; // 各ピクセルのRGBA値を格納
            if (isLeftTop)
            {
                p = p_s + py * step + px * channels;
                for (int i = 0; i < 4; i++)
                {
                    values[i] = p[i];
                }
            }
            if (isRightTop)
            {
                p = p_s + py * step + (px + 1) * channels;
                for (int i = 0; i < 4; i++)
                {
                    values[4 + i] = p[i];
                }
            }
            if (isLeftBottom)
            {
                p = p_s + (py + 1) * step + px * channels;
                for (int i = 0; i < 4; i++)
                {
                    values[8 + i] = p[i];
                }
            }
            if (isRightBottom)
            {
                p = p_s + (py + 1) * step + (px + 1) * channels;
                for (int i = 0; i < 4; i++)
                {
                    values[12 + i] = p[i];
                }
            }

            // バイリニア補間の適用
            p0 = linarPol(values[0], values[8], values[4], values[12], xrate, yrate);
            p1 = linarPol(values[1], values[9], values[5], values[13], xrate, yrate);
            p2 = linarPol(values[2], values[10], values[6], values[14], xrate, yrate);
            p3 = linarPol(values[3], values[11], values[7], values[15], xrate, yrate);

            return true;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsOnRegion(int x, int y, int min_x, int max_x, int min_y, int max_y)
        {
            return min_x <= x && x <= max_x && min_y <= y && y <= max_y;
        }

        private static byte linarPol(int p0, int p1, int p2, int p3, double xrate, double yrate)
        {
            double d = p0 * (1.0 - xrate) + p2 * xrate;
            double e = p1 * (1.0 - xrate) + p3 * xrate;
            double f = d * (1.0 - yrate) + e * yrate;

            // 四捨五入の代わりに整数演算で近似する場合
            int res = (int)(f + 0.5);

            // 結果をバイト値の範囲にクランプ
            if (res < 0) return 0;
            if (res > 255) return 255;
            return (byte)res;
        }


        private unsafe static bool NearestNeighbor(byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0)
        {
            int px = (int)(x + 0.5); // 四捨五入の高速化
            int py = (int)(y + 0.5);

            // 境界チェックを簡素化
            if (px >= min_x && px < max_x && py >= min_y && py < max_y)
            {
                p0 = *(p_s + py * step + px * channels);
                return true;
            }
            else
            {
                p0 = 0;
                return false;
            }
        }


        private unsafe static bool NearestNeighbor(byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0, out int p1, out int p2)
        {
            int px = (int)(x + 0.5); // 四捨五入の高速化
            int py = (int)(y + 0.5);

            // 境界チェックを簡素化
            if (px >= min_x && px < max_x && py >= min_y && py < max_y)
            {
                byte* p = p_s + py * step + px * channels;
                p0 = p[0];
                p1 = p[1];
                p2 = p[2];
                return true;
            }
            else
            {
                p0 = p1 = p2 = 0;
                return false;
            }
        }


        private unsafe static bool NearestNeighbor(byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0, out int p1, out int p2, out int p3)
        {
            int px = (int)(x + 0.5); // 四捨五入の代わりに整数演算で近似
            int py = (int)(y + 0.5);

            // 境界チェックを最適化
            if ((uint)px <= (uint)max_x && (uint)py <= (uint)max_y)
            {
                byte* p = p_s + py * step + px * channels;
                p0 = *p;
                p1 = *(p + 1);
                p2 = *(p + 2);
                p3 = *(p + 3);
                return true;
            }
            else
            {
                p0 = p1 = p2 = p3 = 0;
                return false;
            }
        }
    }
}
