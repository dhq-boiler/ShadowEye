# ShadowEye の使い方

## Webカメラから入力する

1. カメラをクリックします。
2. カメラデバイスの選択ダイアログで入力したいWebカメラデバイスを選択し、ダブルクリックするかOKボタンを押下します。
3. メインワークベンチ領域に選択したWebカメラの入力画像が逐次出力されます。

![input_from_webcam](https://github.com/dhq-boiler/ShadowEye/blob/develop/WebComponents/input_from_webcam.gif)

## ファイルから入力する

1. ファイルをクリックします。
2. 開くダイアログで入力したい画像ファイルを選択します。
3. メインワークベンチ領域に選択した画像ファイルが出力されます。

![input_from_jpg_file](https://github.com/dhq-boiler/ShadowEye/blob/develop/WebComponents/input_from_jpg_file.gif)

## 画像処理

### 減算、乗算

1. Webカメラやファイルから減算対象となる画像と減算する画像の2つを入力しておきます。（例ではWebカメラからの入力画像とWebカメラからの入力を1フレーム分溜めた画像の２つを使用しています）
2. 画像処理→減算...をクリックします。
3. 減算対象となる画像と減算する画像の2つを選択します。
4. 出力形式をBGR, RGB, Grayから選択します。
5. オプションで絶対値計算をする場合はチェックを入れます。
6. 計算ボタンを押下します。
7. メインワークベンチ領域に減算された画像が出力されます。

![subtraction](https://github.com/dhq-boiler/ShadowEye/blob/develop/WebComponents/subtraction.gif)