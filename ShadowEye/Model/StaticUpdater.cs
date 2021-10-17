

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowEye.Model
{
    public class StaticUpdater : Updater
    {
        public StaticUpdater(AnalyzingSource target) : base(target)
        { }

        public override void Request()
        { }

        public override void RequestAccomplished()
        { }

        public override Updater SameUpdater(AnalyzingSource target)
        {
            return new StaticUpdater(target);
        }

        public override void ForceStop()
        { }
    }
}
