

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowEye.Model
{
    public abstract class Updater
    {
        private int _InUseCount;
        protected AnalyzingSource TargetSource;

        protected Updater(AnalyzingSource target)
        {
            this.TargetSource = target;
        }

        protected int InUseCount
        {
            get { return _InUseCount; }
            set { if (value >= 0) _InUseCount = value; }
        }

        public bool InUse
        {
            get { return _InUseCount > 0; }
        }

        public abstract void Request();
        public abstract void RequestAccomplished();
        public abstract void ForceStop();
        public abstract Updater SameUpdater(AnalyzingSource target);
    }
}
