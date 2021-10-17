

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ShadowEye.Utils
{
    public class WinApi
    {
        [DllImport("Shell32.dll")]
        public static extern int ExtractIconEx(
            string szFile,      // アイコンを抽出するファイル名
            int nIconIndex,  // 取り出すアイコンのインデックス 
            out IntPtr phiconLarge, // 大きなアイコンへのポインタ（通常 32x32）
            out IntPtr phiconSmall, // 小さなアイコンへのポインタ（通常 16x16）
            int nIcons       // 取り出すアイコン数
        );

        [DllImport("User32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHGetStockIconInfo(
            StockIconId siid,   // 取得するアイコンの IDを指定する StockIconId enum 型
            StockIconFlags uFlags, // 取得するアイコンの種類を指定する StockIconFlags enum 型
            ref StockIconInfo psii    //（戻り値）StockIconInfo 型
        );

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct StockIconInfo
        {
            public uint cbSize;        // 構造体のサイズ（バイト数）
            public IntPtr hIcon;       //（戻り値）アイコンへのハンドル
            public int iSysImageIndex; //（戻り値）システムアイコンキャッシュ内のアイコンのインデックス
            public int iIcon;          //（戻り値）取り出したアイコンのインデックス
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;      //（戻り値）アイコンリソースを保持するファイル名 
        }

        [Flags]
        public enum StockIconFlags
        {
            Large = 0x000000000,       // 大きなアイコン
            Small = 0x000000001,       // 小さなアイコン
            ShellSize = 0x000000004,   // シェルのサイズのアイコン
            Handle = 0x000000100,      // 指定のアイコンのハンドル
            SystemIndex = 0x000004000, // システムのイメージリストのインデックス
            LinkOverlay = 0x000008000, // リンクを示すオーバーレイ付きのアイコン
            Selected = 0x000010000     // 選択状態のアイコン
        }

        public enum StockIconId
        {
            SIID_DOCNOASSOC = 0,
            SIID_DOCASSOC = 1,
            SIID_APPLICATION = 2,
            SIID_FOLDER = 3,
            SIID_FOLDEROPEN = 4,
            SIID_DRIVE525 = 5,
            SIID_DRIVE35 = 6,
            SIID_DRIVEREMOVE = 7,
            SIID_DRIVEFIXED = 8,
            SIID_DRIVENET = 9,
            SIID_DRIVENETDISABLED = 10,
            SIID_DRIVECD = 11,
            SIID_DRIVERAM = 12,
            SIID_WORLD = 13,
            SIID_SERVER = 15,
            SIID_PRINTER = 16,
            SIID_MYNETWORK = 17,
            SIID_FIND = 22,
            SIID_HELP = 23,
            SIID_SHARE = 28,
            SIID_LINK = 29,
            SIID_SLOWFILE = 30,
            SIID_RECYCLER = 31,
            SIID_RECYCLERFULL = 32,
            SIID_MEDIACDAUDIO = 40,
            SIID_LOCK = 47,
            SIID_AUTOLIST = 49,
            SIID_PRINTERNET = 50,
            SIID_SERVERSHARE = 51,
            SIID_PRINTERFAX = 52,
            SIID_PRINTERFAXNET = 53,
            SIID_PRINTERFILE = 54,
            SIID_STACK = 55,
            SIID_MEDIASVCD = 56,
            SIID_STUFFEDFOLDER = 57,
            SIID_DRIVEUNKNOWN = 58,
            SIID_DRIVEDVD = 59,
            SIID_MEDIADVD = 60,
            SIID_MEDIADVDRAM = 61,
            SIID_MEDIADVDRW = 62,
            SIID_MEDIADVDR = 63,
            SIID_MEDIADVDROM = 64,
            SIID_MEDIACDAUDIOPLUS = 65,
            SIID_MEDIACDRW = 66,
            SIID_MEDIACDR = 67,
            SIID_MEDIACDBURN = 68,
            SIID_MEDIABLANKCD = 69,
            SIID_MEDIACDROM = 70,
            SIID_AUDIOFILES = 71,
            SIID_IMAGEFILES = 72,
            SIID_VIDEOFILES = 73,
            SIID_MIXEDFILES = 74,
            SIID_FOLDERBACK = 75,
            SIID_FOLDERFRONT = 76,
            SIID_SHIELD = 77,
            SIID_WARNING = 78,
            SIID_INFO = 79,
            SIID_ERROR = 80,
            SIID_KEY = 81,
            SIID_SOFTWARE = 82,
            SIID_RENAME = 83,
            SIID_DELETE = 84,
            SIID_MEDIAAUDIODVD = 85,
            SIID_MEDIAMOVIEDVD = 86,
            SIID_MEDIAENHANCEDCD = 87,
            SIID_MEDIAENHANCEDDVD = 88,
            SIID_MEDIAHDDVD = 89,
            SIID_MEDIABLURAY = 90,
            SIID_MEDIAVCD = 91,
            SIID_MEDIADVDPLUSR = 92,
            SIID_MEDIADVDPLUSRW = 93,
            SIID_DESKTOPPC = 94,
            SIID_MOBILEPC = 95,
            SIID_USERS = 96,
            SIID_MEDIASMARTMEDIA = 97,
            SIID_MEDIACOMPACTFLASH = 98,
            SIID_DEVICECELLPHONE = 99,
            SIID_DEVICECAMERA = 100,
            SIID_DEVICEVIDEOCAMERA = 101,
            SIID_DEVICEAUDIOPLAYER = 102,
            SIID_NETWORKCONNECT = 103,
            SIID_INTERNET = 104,
            SIID_ZIPFILE = 105,
            SIID_SETTINGS = 106,
            SIID_DRIVEHDDVD = 132,
            SIID_DRIVEBD = 133,
            SIID_MEDIAHDDVDROM = 134,
            SIID_MEDIAHDDVDR = 135,
            SIID_MEDIAHDDVDRAM = 136,
            SIID_MEDIABDROM = 137,
            SIID_MEDIABDR = 138,
            SIID_MEDIABDRE = 139,
            SIID_CLUSTEREDDRIVE = 140,
        }
    }
}
