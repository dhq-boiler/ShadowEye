

using System.Windows.Input;

namespace ShadowEye.ViewModel
{
    public static class RoutedCommands
    {
        #region ファイルメニュー

        public static readonly RoutedCommand FileOpenCommand = new RoutedCommand();
        public static readonly RoutedCommand CameraOpenCommand = new RoutedCommand();
        public static readonly RoutedCommand ScreenShotCommand = new RoutedCommand();

        #endregion

        #region タブメニュー

        public static readonly RoutedCommand StoreDiscadedImageCommand = new RoutedCommand();
        public static readonly RoutedCommand SaveAsCommand = new RoutedCommand();
        public static readonly RoutedCommand CloseThisTabCommand = new RoutedCommand();
        public static readonly RoutedCommand TweetCommand = new RoutedCommand();

        #endregion

        #region 画像処理メニュー

        public static readonly RoutedCommand ImageProcessing_SubtractionCommand = new RoutedCommand();
        public static readonly RoutedCommand ImageProcessing_IntegrateChannelCommand = new RoutedCommand();
        public static readonly RoutedCommand ImageProcessing_ThresholdingCommand = new RoutedCommand();
        public static readonly RoutedCommand ImageProcessing_ScalingCommand = new RoutedCommand();
        public static readonly RoutedCommand ImageProcessing_MultiplicationCommand = new RoutedCommand();
        public static readonly RoutedCommand ImageProcessing_ColorConversionCommand = new RoutedCommand();
        public static readonly RoutedCommand ImageProcessing_ExtractChannelCommand = new RoutedCommand();

        #endregion //画像処理メニュー

        #region //Filterメニュー

        public static readonly RoutedCommand Filter_EdgeExtractionCommand = new RoutedCommand();

        #endregion
    }
}
