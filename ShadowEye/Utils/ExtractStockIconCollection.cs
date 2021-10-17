

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ShadowEye.Utils
{
    public class ExtractStockIconCollection : ObservableCollection<BitmapSource>
    {
        // コンストラクタ
        public ExtractStockIconCollection()
        {
            // 大きなアイコンのハンドルを取得する
            WinApi.StockIconFlags flags = WinApi.StockIconFlags.Handle | WinApi.StockIconFlags.Large;

            var info = new WinApi.StockIconInfo();
            info.cbSize = (uint)Marshal.SizeOf(typeof(WinApi.StockIconInfo));

            foreach (WinApi.StockIconId id in Enum.GetValues(typeof(WinApi.StockIconId)))
            {
                IntPtr result = WinApi.SHGetStockIconInfo(id, flags, ref info);

                if (info.hIcon != IntPtr.Zero)
                {
                    BitmapSource source = Imaging.CreateBitmapSourceFromHIcon(info.hIcon, Int32Rect.Empty, null);

                    this.Add(source);

                    WinApi.DestroyIcon(info.hIcon);
                }
            }
        }
    }
}
