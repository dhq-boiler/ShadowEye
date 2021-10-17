

using OpenCvSharp;
using System;
using System.Linq;
using System.Windows.Controls;
using ShadowEye.Model;

namespace ShadowEye.ViewModel
{
    public class EdgeExtractionDialogViewModel : CommandSink
    {
        private MainWorkbenchViewModel _imageContainerVM;
        private AnalyzingSource _TargetImage;
        private EEdgeExtractionType _EdgeExtractionType = EEdgeExtractionType.Sobel;
        private MatType _DDepth;
        private int _XOrder;
        private int _YOrder;
        private int _KSize = 3;
        private double _Scale = 1.0;
        private double _Delta = 0.0;
        private BorderTypes _BorderType = OpenCvSharp.BorderTypes.Default;
        private static int s_createCount;

        public EdgeExtractionDialogViewModel(MainWorkbenchViewModel _imageContainerVM)
        {
            this._imageContainerVM = _imageContainerVM;
        }

        public ComboBoxItem[] Items
        {
            get
            {
                return _imageContainerVM.Tabs.Select(a => new ComboBoxItem() { Content = a.Source }).ToArray();
            }
        }

        public AnalyzingSource TargetImage
        {
            get { return _TargetImage; }
            set { SetProperty(ref _TargetImage, value, "TargetImage"); }
        }

        public EEdgeExtractionType[] EdgeExtractionTypes
        {
            get { return Enum.GetValues(typeof(EEdgeExtractionType)).OfType<EEdgeExtractionType>().ToArray(); }
        }

        public MatType[] MatTypes
        {
            get
            {
                return new MatType[]
                {
                    MatType.CV_8SC1,
                    MatType.CV_8SC2,
                    MatType.CV_8SC3,
                    MatType.CV_8SC4,
                    MatType.CV_8UC1,
                    MatType.CV_8UC2,
                    MatType.CV_8UC3,
                    MatType.CV_8UC4,
                    MatType.CV_16SC1,
                    MatType.CV_16SC2,
                    MatType.CV_16SC3,
                    MatType.CV_16SC4,
                    MatType.CV_16UC1,
                    MatType.CV_16UC2,
                    MatType.CV_16UC3,
                    MatType.CV_16UC4,
                    MatType.CV_32FC1,
                    MatType.CV_32FC2,
                    MatType.CV_32FC3,
                    MatType.CV_32FC4,
                    MatType.CV_32SC1,
                    MatType.CV_32SC2,
                    MatType.CV_32SC3,
                    MatType.CV_32SC4,
                    MatType.CV_64FC1,
                    MatType.CV_64FC2,
                    MatType.CV_64FC3,
                    MatType.CV_64FC4
                };
            }
        }

        public int[] KSizes
        {
            get
            {
                return new int[]
                {
                    1,3,5,7
                };
            }
        }

        public BorderTypes[] BorderTypes
        {
            get
            {
                return new OpenCvSharp.BorderTypes[]
                {
                    OpenCvSharp.BorderTypes.Constant,
                    OpenCvSharp.BorderTypes.Default,
                    OpenCvSharp.BorderTypes.Isolated,
                    OpenCvSharp.BorderTypes.Reflect,
                    OpenCvSharp.BorderTypes.Reflect101,
                    OpenCvSharp.BorderTypes.Replicate,
                    OpenCvSharp.BorderTypes.Wrap
                };
            }
        }

        public EEdgeExtractionType EdgeExtractionType
        {
            get { return _EdgeExtractionType; }
            set { SetProperty(ref _EdgeExtractionType, value, "EdgeExtractionType"); }
        }

        public MatType DDepth
        {
            get { return _DDepth; }
            set { SetProperty(ref _DDepth, value, "DDepth"); }
        }

        public int XOrder
        {
            get { return _XOrder; }
            set { SetProperty(ref _XOrder, value, "XOrder"); }
        }

        public int YOrder
        {
            get { return _YOrder; }
            set { SetProperty(ref _YOrder, value, "YOrder"); }
        }

        public int KSize
        {
            get { return _KSize; }
            set { SetProperty(ref _KSize, value, "KSize"); }
        }

        public double Scale
        {
            get { return _Scale; }
            set { SetProperty(ref _Scale, value, "Scale"); }
        }

        public double Delta
        {
            get { return _Delta; }
            set { SetProperty(ref _Delta, value, "Delta"); }
        }

        public BorderTypes BorderType
        {
            get { return _BorderType; }
            set { SetProperty(ref _BorderType, value, "BorderType"); }
        }

        internal void AddComputingTab()
        {
            try
            {
                EdgeExtractionSource source = null;
                switch (EdgeExtractionType)
                {
                    case EEdgeExtractionType.Sobel:
                        source = EdgeExtractionSource.CreateInstanceSobel($"EdgeExtracted-{ ++s_createCount}", TargetImage, DDepth, XOrder, YOrder, KSize, Scale, Delta, BorderType);
                        break;
                    case EEdgeExtractionType.Canny:
                        break;
                    case EEdgeExtractionType.Laplacian:
                        break;
                }
                _imageContainerVM.AddOrActive(source);
            }
            catch (OpenCVException)
            {
                throw;
            }
        }

        public enum EEdgeExtractionType
        {
            Sobel,
            Canny,
            Laplacian
        }
    }
}