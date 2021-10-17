

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShadowEye.Utils;

namespace ShadowEye.Model
{
    public class ScreenOption
    {
        public ScreenOption()
        { }

        public ScreenOption(Screen target)
        {
            Target = target;
        }

        public ScreenOption(int index, Screen target, NativeMethods.DISPLAY_DEVICE monitor)
            : this(target)
        {
            Index = index;
            DisplayDevice = monitor;
        }

        public int Index { get; set; }
        public Screen Target { get; set; }
        public NativeMethods.DISPLAY_DEVICE DisplayDevice { get; set; }

        public override string ToString()
        {
            return string.Format("{0}. {1}{2}", Index + 1, DisplayDevice.DeviceString, Target.Primary ? "[\u30D7\u30E9\u30A4\u30DE\u30EA]" : "");
        }

        public static ScreenOption[] CreateArray()
        {
            var list = new List<ScreenOption>();
            var device = new NativeMethods.DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);

            for (uint i = 0; NativeMethods.EnumDisplayDevices(null, i, ref device, 1); ++i)
            {
                try
                {
                    if (device.StateFlags == NativeMethods.DisplayDeviceStateFlags.None
                        || (device.StateFlags & NativeMethods.DisplayDeviceStateFlags.MirroringDriver) == NativeMethods.DisplayDeviceStateFlags.MirroringDriver)
                    {
                        continue;
                    }
                    list.Add(GetDisplay(device, (int)i));
                }
                catch (InvalidOperationException)
                { }
            }
            return list.ToArray();
        }

        private static ScreenOption GetDisplay(NativeMethods.DISPLAY_DEVICE device, int index)
        {
            var monitor = new NativeMethods.DISPLAY_DEVICE();
            monitor.cb = Marshal.SizeOf(monitor);
            NativeMethods.EnumDisplayDevices(device.DeviceName, 0, ref monitor, 0);
            return new ScreenOption(index, Screen.AllScreens.Where(a => monitor.DeviceName.Contains(a.DeviceName)).First(), monitor);
        }
    }
}
