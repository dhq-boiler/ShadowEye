using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowEye.Model
{
    public class DummySource : AnalyzingSource
    {
        public DummySource() : base("dummy")
        { }

        public override bool UpdateOnce => false;

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void UpdateImage()
        {
        }
    }
}
