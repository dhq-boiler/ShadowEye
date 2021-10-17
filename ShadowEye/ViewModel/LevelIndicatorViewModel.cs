

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShadowEye.Utils;

namespace ShadowEye.ViewModel
{
    public class LevelIndicatorViewModel : Notifier
    {
        private int _Value1st;
        public int Value1st
        {
            get { return _Value1st; }
            set { SetProperty<int>(ref _Value1st, value, "Value1st"); }
        }

        private int _Value2nd;
        public int Value2nd
        {
            get { return _Value2nd; }
            set { SetProperty<int>(ref _Value2nd, value, "Value2nd"); }
        }

        private int _Value3rd;
        public int Value3rd
        {
            get { return _Value3rd; }
            set { SetProperty<int>(ref _Value3rd, value, "Value3rd"); }
        }

        private int _Value4th;
        public int Value4th
        {
            get { return _Value4th; }
            set { SetProperty<int>(ref _Value4th, value, "Value4th"); }
        }

        private int _Value5th;
        public int Value5th
        {
            get { return _Value5th; }
            set { SetProperty<int>(ref _Value5th, value, "Value5th"); }
        }

        private int _Value6th;
        public int Value6th
        {
            get { return _Value6th; }
            set { SetProperty<int>(ref _Value6th, value, "Value6th"); }
        }
    }
}
