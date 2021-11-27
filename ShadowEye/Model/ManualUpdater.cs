using OpenCvSharp;
using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;

namespace ShadowEye.Model
{
    internal class ManualUpdater : Updater
    {

        public ManualUpdater(AnalyzingSource target) : base(target)
        {
        }

        public override void ForceStop()
        {
            TargetSource.Deactivate();
            InUseCount = 0;
        }

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
            return new ManualUpdater(target);
        }
    }
}