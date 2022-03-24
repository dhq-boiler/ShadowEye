using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace libcamenmCore
{
    public class DeviceEnumerator
    {
        public static IEnumerable<DsDevice> EnumVideoInputDevice()
        {
            return DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
        }

        public static List<Resolution> GetAllAvailableResolution(int cameraNumber, DsDevice vidDev)
        {
            try
            {
                int hr;
                int max = 0;
                int bitCount = 0;

                IBaseFilter sourceFilter = null;

                var m_FilterGraph2 = new FilterGraph() as IFilterGraph2;

                hr = m_FilterGraph2.AddSourceFilterForMoniker(vidDev.Mon, null, vidDev.Name, out sourceFilter);

                var pRaw2 = DsFindPin.ByCategory(sourceFilter, PinCategory.Capture, 0);

                var AvailableResolutions = new List<Resolution>();

                VideoInfoHeader v = new VideoInfoHeader();
                IEnumMediaTypes mediaTypeEnum;
                hr = pRaw2.EnumMediaTypes(out mediaTypeEnum);

                AMMediaType[] mediaTypes = new AMMediaType[1];
                IntPtr fetched = IntPtr.Zero;
                hr = mediaTypeEnum.Next(1, mediaTypes, fetched);

                while (fetched != null && mediaTypes[0] != null)
                {
                    Marshal.PtrToStructure(mediaTypes[0].formatPtr, v);
                    if (v.BmiHeader.Size != 0 && v.BmiHeader.BitCount != 0)
                    {
                        if (v.BmiHeader.BitCount > bitCount)
                        {
                            AvailableResolutions.Clear();
                            max = 0;
                            bitCount = v.BmiHeader.BitCount;


                        }
                        AvailableResolutions.Add(new Resolution(vidDev, cameraNumber, v.BmiHeader.Width, v.BmiHeader.Height));
                        if (v.BmiHeader.Width > max || v.BmiHeader.Height > max)
                            max = (Math.Max(v.BmiHeader.Width, v.BmiHeader.Height));
                    }
                    hr = mediaTypeEnum.Next(1, mediaTypes, fetched);
                }
                return AvailableResolutions;
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<Resolution>();
            }
        }
    }
}
