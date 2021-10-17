

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectShowLib;

namespace libcamenm
{
    public class DeviceEnumerator
    {
        public static IEnumerable<string> EnumVideoInputDevice()
        {
            List<string> ret = new List<string>();
            var collection = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            foreach (var device in collection)
            {
                ret.Add(device.Name);
            }
            return ret.ToArray();
        }
    }
}
