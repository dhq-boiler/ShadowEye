using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcamenmCore
{
    public class Resolution
    {
        public DsDevice DsDevice { get; set; }
        public int CameraNumber { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Resolution(DsDevice device, int cameraNumber, int width, int height)
        {
            this.DsDevice = device;
            this.CameraNumber = cameraNumber;
            this.Width = width;
            this.Height = height;
        }

        public override string ToString()
        {
            return $"{DsDevice.Name}[{CameraNumber}] {Width}x{Height}";
        }
    }
}
