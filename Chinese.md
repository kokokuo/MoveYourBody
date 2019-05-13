# 以體感器實作之 Kinect 體感遊戲 - Move Your Body


# 簡介
「以體感器實作之 Kinect 體感遊戲 - Move Your Body」 為大學一年的畢業專題，採用當時市面剛推出的 Kinect 裝置為體感訊號來源，所開發出的 3D 遊戲。

在體感互動方面，由於開發時微軟的 KinectSDK 在當時尚未釋出，因此採用製作 Kinect 的來源廠商 - PrimeSight 推出的 OpeNI/NITE 作為解析 Kinect RGB 與 Depth 來源訊號，並透過影像處理和電腦視覺解析影像、設計並開發動作偵測演算法。同時設計體感遊戲內容，採用 XNA 4.0 Game Framework 開發，並使用物件導向、 Design Pattern 打造出此 3D 遊戲。

遊戲從選單開始變皆是透過 Kinect 體感器操作：

**<p align="left">選單 - 骨架偵測與姿勢校正</p>**
**<p align="right">選單</p>**
<p align="left">
  <img src="../master/MenuOne.png?raw=true">
</p>
<p align="center">
  <img src="../master/MenuTwo.png?raw=true">
</p>

此外遊戲也提供了兩個關卡，分別是需要原地奔跑的逃離猛獸 和 穿越看板的姿勢模仿。

**<p align="left">逃離猛獸關卡</p>**
**<p align="right">姿勢模仿關卡</p>**

<p align="left">
  <img src="../master/GameOne.png?raw=true">
</p>

<p align="right">
  <img src="../master/GameTwo.png?raw=true">
</p>


# 開發環境
- 硬體裝置 : Kinect + AUX To USB 轉接線 
- Kinect 驅動程式 : SensorKinect-Win-OpenSource32-5.0.3.4
- 程式語言：C#
- Kinect 的開發套件工具：OpenNI-Win32-1.3.2.3-Dev.msi、 NITE-Win32-1.4.1.2-Dev.msi
- 遊戲的開發套件工具：XNA 4.0 Game Framework、JibLix 物理引擎
- IDE : Visual Studio 2010 • 作業系統 : Windows 7 • 顯示卡支援 : GT130 

# 團隊成員
- Eason Kuo : 團隊領導、專案管理、上台演講、處理 Kinect 來源影像訊號與設計開發姿勢演算法。
- Cheng-Yi Huang : 遊戲設計、遊戲開發、整合 Eason Kuo 開發的 Kinect 動作訊號子系統。

# Slide 連結
你可以觀看「以體感器實作之 Kinect 體感遊戲 - Move Your Body」 的 [Slide 簡報連結](https://www.slideshare.net/YiChengKuo1/20111027-graduation-project-motion-sensing-game-with-kinect-sensor-move-your-body)

# 影片連結
你可以觀看遊戲「以體感器實作之 Kinect 體感遊戲 - Move Your Body」的[影片介紹連結](https://youtu.be/Np3yjK-OHMM)