// Copyright © 2015 dhq_boiler.

using System;

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
            switch (method)
            {
                case Interpolation.NearestNeighbor:
                    return NearestNeighbor(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0);
                case Interpolation.Bilinear:
                    return Bilinear(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0);
                case Interpolation.Bicubic:
                    return Bicubic(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0);
                default:
                    throw new NotSupportedException();
            }
        }

        internal unsafe static bool Interplate(Interpolation method, byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0, out int p1, out int p2)
        {
            switch (method)
            {
                case Interpolation.NearestNeighbor:
                    return NearestNeighbor(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2);
                case Interpolation.Bilinear:
                    return Bilinear(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2);
                case Interpolation.Bicubic:
                    return Bicubic(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2);
                default:
                    throw new NotSupportedException();
            }
        }

        internal unsafe static bool Interplate(Interpolation method, byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0, out int p1, out int p2, out int p3)
        {
            switch (method)
            {
                case Interpolation.NearestNeighbor:
                    return NearestNeighbor(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2, out p3);
                case Interpolation.Bilinear:
                    return Bilinear(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2, out p3);
                case Interpolation.Bicubic:
                    return Bicubic(p_s, x, y, min_x, max_x, min_y, max_y, step, channels, out p0, out p1, out p2, out p3);
                default:
                    throw new NotSupportedException();
            }
        }

        private static unsafe bool Bicubic(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0)
        {
            int px = (int)x;
            int py = (int)y;

            if (min_x > px || max_x < px || min_y > py || max_y < py)
            {
                p0 = 0;
                return false;
            }

            int[] buf0 = new int[16];

            for (int ly2 = 0; ly2 < 4; ++ly2)
            {
                int adr2 = ly2 * 4;
                for (int lx2 = 0; lx2 < 4; ++lx2)
                {
                    int pxlx2 = px + lx2 - 1;
                    int pyly2 = py + ly2 - 1;

                    if (pxlx2 >= min_x && pxlx2 <= max_x && pyly2 >= min_y && pyly2 <= max_y)
                    {
                        buf0[adr2] = *(p_s + pyly2 * step + pxlx2);
                    }
                    else
                    {
                        buf0[adr2] = 255 / 2;
                    }
                    ++adr2;
                }
            }

            p0 = biCubic(x, y, buf0);
            return true;
        }

        private static unsafe bool Bicubic(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0, out int p1, out int p2)
        {
            int px = (int)x;
            int py = (int)y;

            if (min_x > px || max_x < px || min_y > py || max_y < py)
            {
                p0 = p1 = p2 = 0;
                return false;
            }

            int[] buf0 = new int[16];
            int[] buf1 = new int[16];
            int[] buf2 = new int[16];

            for (int ly2 = 0; ly2 < 4; ++ly2)
            {
                int adr2 = ly2 * 4;
                for (int lx2 = 0; lx2 < 4; ++lx2)
                {
                    int pxlx2 = px + lx2 - 1;
                    int pyly2 = py + ly2 - 1;

                    if (pxlx2 >= min_x && pxlx2 <= max_x && pyly2 >= min_y && pyly2 <= max_y)
                    {
                        byte* p = p_s + pyly2 * step + pxlx2 * channels;
                        buf0[adr2] = *p;
                        buf1[adr2] = *(p + 1);
                        buf2[adr2] = *(p + 2);
                    }
                    else
                    {
                        buf0[adr2] = 255 / 2;
                        buf1[adr2] = 255 / 2;
                        buf2[adr2] = 255 / 2;
                    }
                    ++adr2;
                }
            }

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

            int[] buf0 = new int[16];
            int[] buf1 = new int[16];
            int[] buf2 = new int[16];
            int[] buf3 = new int[16];

            for (int ly2 = 0; ly2 < 4; ++ly2)
            {
                int adr2 = ly2 * 4;
                for (int lx2 = 0; lx2 < 4; ++lx2)
                {
                    int pxlx2 = px + lx2 - 1;
                    int pyly2 = py + ly2 - 1;

                    if (pxlx2 >= min_x && pxlx2 <= max_x && pyly2 >= min_y && pyly2 <= max_y)
                    {
                        byte* p = p_s + pyly2 * step + pxlx2 * channels;
                        buf0[adr2] = *p;
                        buf1[adr2] = *(p + 1);
                        buf2[adr2] = *(p + 2);
                        buf3[adr2] = *(p + 3);
                    }
                    else
                    {
                        buf0[adr2] = 255 / 2;
                        buf1[adr2] = 255 / 2;
                        buf2[adr2] = 255 / 2;
                        buf3[adr2] = 255 / 2;
                    }
                    ++adr2;
                }
            }

            p0 = biCubic(x, y, buf0);
            p1 = biCubic(x, y, buf1);
            p2 = biCubic(x, y, buf2);
            p3 = biCubic(x, y, buf3);
            return true;
        }

        private static int biCubic(double x, double y, int[] buf)
        {
            double[] fx = new double[4];
            double[] fy = new double[4];
            double[] mat = new double[4];
            double[] tmp = new double[4];

            int px = (int)x;
            int py = (int)y;
            double dx = x - (double)px;
            double dy = y - (double)py;

            fx[0] = cubicFunc(1.0 + dx);
            fx[1] = cubicFunc(dx);
            fx[2] = cubicFunc(1.0 - dx);
            fx[3] = cubicFunc(2.0 - dx);

            fy[0] = cubicFunc(1.0 + dy);
            fy[1] = cubicFunc(dy);
            fy[2] = cubicFunc(1.0 - dy);
            fy[3] = cubicFunc(2.0 - dy);

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    mat[j] = (double)buf[j * 4 + i];
                }
                tmp[i] = naiseki(fy, mat);
            }

            int res = (int)naiseki(tmp, fx);
            if (res >= byte.MinValue && res <= byte.MaxValue)
            {
                return res;
            }
            else if (res < byte.MinValue)
            {
                return byte.MinValue;
            }
            else
            {
                return byte.MaxValue;
            }
        }

        private static double naiseki(double[] x, double[] y)
        {
            double res = 0.0;
            for (int i = 0; i < 4; ++i)
            {
                res += (x[i] * y[i]);
            }
            return res;
        }

        private static double cubicFunc(double x)
        {
            double ax = default(double);

            if (x < 0) ax = -x;
            else ax = x;

            if (0.0 <= ax && ax < 1.0)
            {
                return 1.0 - 2.0 * ax * ax + ax * ax * ax;
            }
            else if (1.0 <= ax && ax < 2.0)
            {
                return 4.0 - 8.0 * ax + 5.0 * ax * ax - ax * ax * ax;
            }
            else
            {
                return 0.0;
            }
        }

        private static unsafe bool Bilinear(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0)
        {
            int px, py;
            double xrate, yrate;

            px = (int)x;
            py = (int)y;

            xrate = x - (double)px;
            yrate = y - (double)py;

            bool leftTop = IsOnRegion(px, py, min_x, max_x, min_y, max_y);
            bool leftBottom = IsOnRegion(px, py + 1, min_x, max_x, min_y, max_y);
            bool rightTop = IsOnRegion(px + 1, py, min_x, max_x, min_y, max_y);
            bool rightBottom = IsOnRegion(px + 1, py + 1, min_x, max_x, min_y, max_y);

            if (!leftTop && !leftBottom && !rightTop && !rightBottom)
            {
                p0 = 0;
                return false;
            }

            byte* p = p_s + py * step + px * channels;
            int B0 = 128;
            if (leftTop)
            {
                B0 = *p;
            }
            else if (leftBottom)
            {
                B0 = *(p + channels);
            }
            else if (rightTop)
            {
                B0 = *(p + step);
            }
            else if (rightBottom)
            {
                B0 = *(p + step + channels);
            }

            p = p_s + (py + 1) * step + px * channels;
            int B1 = 128;
            if (leftBottom)
            {
                B1 = *p;
            }
            else if (rightBottom)
            {
                B1 = *(p + channels);
            }
            else if (leftTop)
            {
                B1 = *(p - step);
            }
            else if (rightTop)
            {
                B0 = *(p - step + channels);
            }

            p = p_s + py * step + (px + 1) * channels;
            int B2 = 128;
            if (rightTop)
            {
                B2 = *p;
            }
            else if (leftTop)
            {
                B2 = *(p - channels);
            }
            else if (rightBottom)
            {
                B2 = *(p + step);
            }
            else if (leftBottom)
            {
                B0 = *(p + step - channels);
            }

            p = p_s + (py + 1) * step + (px + 1) * channels;
            int B3 = 128;
            if (rightBottom)
            {
                B3 = *p;
            }
            else if (leftBottom)
            {
                B3 = *(p - channels);
            }
            else if (rightTop)
            {
                B3 = *(p - step);
            }
            else if (leftTop)
            {
                B0 = *(p - step - channels);
            }

            p0 = linarPol(B0, B1, B2, B3, xrate, yrate);
            return true;
        }

        private static unsafe bool Bilinear(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0, out int p1, out int p2)
        {
            int px, py;
            double xrate, yrate;

            px = (int)x;
            py = (int)y;

            xrate = x - (double)px;
            yrate = y - (double)py;

            bool leftTop = IsOnRegion(px, py, min_x, max_x, min_y, max_y);
            bool leftBottom = IsOnRegion(px, py + 1, min_x, max_x, min_y, max_y);
            bool rightTop = IsOnRegion(px + 1, py, min_x, max_x, min_y, max_y);
            bool rightBottom = IsOnRegion(px + 1, py + 1, min_x, max_x, min_y, max_y);

            if (!leftTop && !leftBottom && !rightTop && !rightBottom)
            {
                p0 = p1 = p2 = 0;
                return false;
            }

            byte* p = p_s + py * step + px * channels;
            int B0 = 128, G0 = 128, R0 = 128;
            if (leftTop)
            {
                B0 = *p;
                G0 = *(p + 1);
                R0 = *(p + 2);
            }
            else if (leftBottom)
            {
                B0 = *(p + channels);
                G0 = *(p + channels + 1);
                R0 = *(p + channels + 2);
            }
            else if (rightTop)
            {
                B0 = *(p + step);
                G0 = *(p + step + 1);
                R0 = *(p + step + 2);
            }
            else if (rightBottom)
            {
                B0 = *(p + step + channels);
                G0 = *(p + step + channels + 1);
                R0 = *(p + step + channels + 2);
            }

            p = p_s + (py + 1) * step + px * channels;
            int B1 = 128, G1 = 128, R1 = 128;
            if (leftBottom)
            {
                B1 = *p;
                G1 = *(p + 1);
                R1 = *(p + 2);
            }
            else if (rightBottom)
            {
                B1 = *(p + channels);
                G1 = *(p + channels + 1);
                R1 = *(p + channels + 2);
            }
            else if (leftTop)
            {
                B1 = *(p - step);
                G1 = *(p - step + 1);
                R1 = *(p - step + 2);
            }
            else if (rightTop)
            {
                B0 = *(p - step + channels);
                G0 = *(p - step + channels + 1);
                R0 = *(p - step + channels + 2);
            }

            p = p_s + py * step + (px + 1) * channels;
            int B2 = 128, G2 = 128, R2 = 128;
            if (rightTop)
            {
                B2 = *p;
                G2 = *(p + 1);
                R2 = *(p + 2);
            }
            else if (leftTop)
            {
                B2 = *(p - channels);
                G2 = *(p - channels + 1);
                R2 = *(p - channels + 2);
            }
            else if (rightBottom)
            {
                B2 = *(p + step);
                G2 = *(p + step + 1);
                R2 = *(p + step + 2);
            }
            else if (leftBottom)
            {
                B0 = *(p + step - channels);
                G0 = *(p + step - channels + 1);
                R0 = *(p + step - channels + 2);
            }

            p = p_s + (py + 1) * step + (px + 1) * channels;
            int B3 = 128, G3 = 128, R3 = 128;
            if (rightBottom)
            {
                B3 = *p;
                G3 = *(p + 1);
                R3 = *(p + 2);
            }
            else if (leftBottom)
            {
                B3 = *(p - channels);
                G3 = *(p - channels + 1);
                R3 = *(p - channels + 2);
            }
            else if (rightTop)
            {
                B3 = *(p - step);
                G3 = *(p - step + 1);
                R3 = *(p - step + 2);
            }
            else if (leftTop)
            {
                B0 = *(p - step - channels);
                G0 = *(p - step - channels + 1);
                R0 = *(p - step - channels + 2);
            }

            p0 = linarPol(B0, B1, B2, B3, xrate, yrate);
            p1 = linarPol(G0, G1, G2, G3, xrate, yrate);
            p2 = linarPol(R0, R1, R2, R3, xrate, yrate);
            return true;
        }

        private static unsafe bool Bilinear(byte* p_s, double x, double y, int min_x, int max_x, int min_y, int max_y, long step, int channels, out int p0, out int p1, out int p2, out int p3)
        {
            int px, py;
            double xrate, yrate;

            px = (int)Math.Round(x);
            py = (int)Math.Round(y);

            xrate = x - (double)px;
            yrate = y - (double)py;

            bool leftTop = IsOnRegion(px, py, min_x, max_x, min_y, max_y);
            bool leftBottom = IsOnRegion(px, py + 1, min_x, max_x, min_y, max_y);
            bool rightTop = IsOnRegion(px + 1, py, min_x, max_x, min_y, max_y);
            bool rightBottom = IsOnRegion(px + 1, py + 1, min_x, max_x, min_y, max_y);

            if (!leftTop && !leftBottom && !rightTop && !rightBottom)
            {
                p0 = p1 = p2 = p3 = 0;
                return false;
            }

            byte* p = p_s + py * step + px * channels;
            int B0 = 128, G0 = 128, R0 = 128, A0 = 128;
            if (leftTop)
            {
                B0 = *p;
                G0 = *(p + 1);
                R0 = *(p + 2);
                A0 = *(p + 3);
            }
            else if (leftBottom)
            {
                B0 = *(p + channels);
                G0 = *(p + channels + 1);
                R0 = *(p + channels + 2);
                A0 = *(p + channels + 3);
            }
            else if (rightTop)
            {
                B0 = *(p + step);
                G0 = *(p + step + 1);
                R0 = *(p + step + 2);
                A0 = *(p + step + 3);
            }
            else if (rightBottom)
            {
                B0 = *(p + step + channels);
                G0 = *(p + step + channels + 1);
                R0 = *(p + step + channels + 2);
                A0 = *(p + step + channels + 3);
            }

            p = p_s + (py + 1) * step + px * channels;
            int B1 = 128, G1 = 128, R1 = 128, A1 = 128;
            if (leftBottom)
            {
                B1 = *p;
                G1 = *(p + 1);
                R1 = *(p + 2);
                A1 = *(p + 3);
            }
            else if (rightBottom)
            {
                B1 = *(p + channels);
                G1 = *(p + channels + 1);
                R1 = *(p + channels + 2);
                A1 = *(p + channels + 3);
            }
            else if (leftTop)
            {
                B1 = *(p - step);
                G1 = *(p - step + 1);
                R1 = *(p - step + 2);
                A1 = *(p - step + 3);
            }
            else if (rightTop)
            {
                B0 = *(p - step + channels);
                G0 = *(p - step + channels + 1);
                R0 = *(p - step + channels + 2);
                A0 = *(p - step + channels + 3);
            }

            p = p_s + py * step + (px + 1) * channels;
            int B2 = 128, G2 = 128, R2 = 128, A2 = 128;
            if (rightTop)
            {
                B2 = *p;
                G2 = *(p + 1);
                R2 = *(p + 2);
                A2 = *(p + 3);
            }
            else if (leftTop)
            {
                B2 = *(p - channels);
                G2 = *(p - channels + 1);
                R2 = *(p - channels + 2);
                A2 = *(p - channels + 3);
            }
            else if (rightBottom)
            {
                B2 = *(p + step);
                G2 = *(p + step + 1);
                R2 = *(p + step + 2);
                A2 = *(p + step + 3);
            }
            else if (leftBottom)
            {
                B0 = *(p + step - channels);
                G0 = *(p + step - channels + 1);
                R0 = *(p + step - channels + 2);
                A0 = *(p + step - channels + 3);
            }

            p = p_s + (py + 1) * step + (px + 1) * channels;
            int B3 = 128, G3 = 128, R3 = 128, A3 = 128;
            if (rightBottom)
            {
                B3 = *p;
                G3 = *(p + 1);
                R3 = *(p + 2);
                A3 = *(p + 3);
            }
            else if (leftBottom)
            {
                B3 = *(p - channels);
                G3 = *(p - channels + 1);
                R3 = *(p - channels + 2);
                A3 = *(p - channels + 3);
            }
            else if (rightTop)
            {
                B3 = *(p - step);
                G3 = *(p - step + 1);
                R3 = *(p - step + 2);
                A3 = *(p - step + 3);
            }
            else if (leftTop)
            {
                B0 = *(p - step - channels);
                G0 = *(p - step - channels + 1);
                R0 = *(p - step - channels + 2);
                A0 = *(p - step - channels + 3);
            }

            p0 = linarPol(B0, B1, B2, B3, xrate, yrate);
            p1 = linarPol(G0, G1, G2, G3, xrate, yrate);
            p2 = linarPol(R0, R1, R2, R3, xrate, yrate);
            p3 = linarPol(A0, A1, A2, A3, xrate, yrate);
            return true;
        }

        private static bool IsOnRegion(int x, int y, int min_x, int max_x, int min_y, int max_y)
        {
            return min_x <= x && x <= max_x && min_y <= y && y <= max_y;
        }

        private static byte linarPol(int p0, int p1, int p2, int p3, double xrate, double yrate)
        {
            double d = (double)p0 * (1.0 - xrate) + (double)p2 * xrate;
            double e = (double)p1 * (1.0 - xrate) + (double)p3 * xrate;
            double f = d * (1.0 - yrate) + e * yrate;
            int res = (int)Math.Round(f);
            if (res >= byte.MinValue && res <= byte.MaxValue)
            {
                return (byte)res;
            }
            else if (res > byte.MaxValue)
            {
                return byte.MaxValue;
            }
            else
            {
                return byte.MinValue;
            }
        }

        private unsafe static bool NearestNeighbor(byte* p_s,
            double x, double y,
            int min_x, int max_x,
            int min_y, int max_y,
            long step, int channels,
            out int p0)
        {
            int px = (int)Math.Round(x);
            int py = (int)Math.Round(y);

            if (px >= min_x && px <= max_x && py >= min_y && py <= max_y)
            {
                p0 = *(p_s + py * step + px);
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
            int px = (int)Math.Round(x);
            int py = (int)Math.Round(y);

            if (px >= min_x && px <= max_x && py >= min_y && py <= max_y)
            {
                byte* p = p_s + py * step + px * channels;
                p0 = *p;
                p1 = *(p + 1);
                p2 = *(p + 2);
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
            int px = (int)Math.Round(x);
            int py = (int)Math.Round(y);

            if (px >= min_x && px <= max_x && py >= min_y && py <= max_y)
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
