

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowEye.Model
{
    public class DynamicUpdater : Updater
    {
        public DynamicUpdater(AnalyzingSource target) : base(target)
        { }

        public override void Request()
        {
            if (InUseCount++ == 0)
            {
                TargetSource.Activate();
            }
            Trace.WriteLine(InUseCount, "DynamicUpdater.InUseCount");
        }

        public override void RequestAccomplished()
        {
            if (--InUseCount == 0)
            {
                TargetSource.Deactivate();
            }
            Trace.WriteLine(InUseCount, "DynamicUpdater.InUseCount");
        }

        public override Updater SameUpdater(AnalyzingSource target)
        {
            return new DynamicUpdater(target);
        }

        public override void ForceStop()
        {
            TargetSource.Deactivate();
            InUseCount = 0;
        }
    }
}
