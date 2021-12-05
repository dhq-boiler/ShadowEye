using DirectShowLib;
using System.Collections.Generic;

namespace libcamenmCore
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
