Vimba .NET 图像采集状态统计例程 - vmbnet_freerun_missing_frames_statistics
---

# 简介

FreeRun version:  
VimbaNET_Examples\AsynchronousGrab-Console Example with missing/incomplete frames counting functions.  
基于VimbaNET_Examples\AsynchronousGrab-Console例程，增加图像接收统计功能。  
通过每张图片的连续递增序号以及每张图片的状态值来判断丢失或者不完整的图片数量。  

每次取图16张。 


# 相机参数设置
在相机缺省参数基础上，需要设置如下参数
* `AcquisitionMode`: `MultiFrame`
* `AcquisitionFrameCount`: `16`


# Visual Studio 设置
此程序使用了`OpenCvSharp`组件。
可以使用Visual Studio 2017或者更高版本，从NuGet中安装 `OpenCvSharp` 组件。
https://github.com/shimat/opencvsharp  
使用方法：https://blog.csdn.net/bayinglong/article/details/84258687




# 例程使用说明

![Vmbnet-async-console-sample-missing-incomplete-frames-screenshot.png](Vmbnet-async-console-sample-missing-incomplete-frames-screenshot.png)
* FrameID: 自从相机上电运行后，每一张图片都具有一个连续递增的序列号。
* Missed: 已经累积丢失的图片数量。
* Incomplete: 已经累积收到的不完整图片数量。  
        例如，下面是主要的采图失败状态码：  
        `VmbFrameStatusFault = -4,`  
        `VmbFrameStatusInvalid = -3,`   
        `VmbFrameStatusTooSmall = -2,`    
        `VmbFrameStatusIncomplete = -1,`  
        `VmbFrameStatusComplete = 0`  
* Stamp: 相机拍摄图片时的时间戳，也就是相机感光芯片直接成像的时刻。
* StDiff: 与前一张图片时间戳的差。


# 代码及编译  
## 基于下面代码
在这里：https://github.com/avtcn/vmbnet_freerun_missing_frames_statistics
## 编译运行
可以使用[Visual Studio 2010](https://visualstudio.microsoft.com/) 或者更高版本，Vimba SDK 建议使用 [2.1.3或者更高版本](https://www.alliedvision.com/en/products/software.html)。
## 运行结果
```


/////////////////////////////////////////////////////////////
///                                                       ///
/// Vimba NET API Asynchronous Console Grab Example       ///
/// with missing/incomplete frames statistics functions   ///
///                                                       ///
/// For Truking, 16 multiframes test                      ///
///                                                       ///
/////////////////////////////////////////////////////////////

Vimba .NET API Version 1.8.0
Finding camera with ID: DEV_1AB22C000484
Opening camera with ID: DEV_1AB22C000484

Press <c> to capture 16 photos OR <q> to stop acquisition ...
Frame receiving ...



Start capturing 16 photos ..., i = 0000000001, Temp = 48.7000
FrameID: 000 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354376613500 FirstFrameConsume: 56.607 ms
FrameID: 001 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354382907000 StDiff:6293500 CameraFPS: 158.89,
FrameID: 002 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354389200600 StDiff:6293600 CameraFPS: 158.89,
FrameID: 003 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354395494100 StDiff:6293500 CameraFPS: 158.89,
FrameID: 004 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354401787600 StDiff:6293500 CameraFPS: 158.89,
FrameID: 005 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354408081200 StDiff:6293600 CameraFPS: 158.89,
FrameID: 006 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354414374700 StDiff:6293500 CameraFPS: 158.89,
FrameID: 007 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354420668200 StDiff:6293500 CameraFPS: 158.89,
FrameID: 008 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354426961800 StDiff:6293600 CameraFPS: 158.89,
FrameID: 009 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354433255300 StDiff:6293500 CameraFPS: 158.89,
FrameID: 010 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354439548800 StDiff:6293500 CameraFPS: 158.89,
FrameID: 011 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354445842400 StDiff:6293600 CameraFPS: 158.89,
FrameID: 012 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354452135900 StDiff:6293500 CameraFPS: 158.89,
FrameID: 013 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354458429400 StDiff:6293500 CameraFPS: 158.89,
FrameID: 014 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354464723000 StDiff:6293600 CameraFPS: 158.89,
FrameID: 015 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1354471016500 StDiff:6293500 CameraFPS: 158.89,
queueFrames.Count = 16
Time consumption of receiving 16 photos: 142.495 ms, and additional 141.5199 ms for image processing!

Start capturing 16 photos ..., i = 0000000002, Temp = 48.7000
FrameID: 000 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355005975100 FirstFrameConsume: 33.184 ms
FrameID: 001 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355012268600 StDiff:6293500 CameraFPS: 158.89,
FrameID: 002 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355018562200 StDiff:6293600 CameraFPS: 158.89,
FrameID: 003 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355024855700 StDiff:6293500 CameraFPS: 158.89,
FrameID: 004 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355031149200 StDiff:6293500 CameraFPS: 158.89,
FrameID: 005 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355037442800 StDiff:6293600 CameraFPS: 158.89,
FrameID: 006 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355043736300 StDiff:6293500 CameraFPS: 158.89,
FrameID: 007 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355050029800 StDiff:6293500 CameraFPS: 158.89,
FrameID: 008 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355056323400 StDiff:6293600 CameraFPS: 158.89,
FrameID: 009 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355062616900 StDiff:6293500 CameraFPS: 158.89,
FrameID: 010 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355068910500 StDiff:6293600 CameraFPS: 158.89,
FrameID: 011 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355075204000 StDiff:6293500 CameraFPS: 158.89,
FrameID: 012 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355081497500 StDiff:6293500 CameraFPS: 158.89,
FrameID: 013 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355087791100 StDiff:6293600 CameraFPS: 158.89,
FrameID: 014 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355094084600 StDiff:6293500 CameraFPS: 158.89,
FrameID: 015 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355100378200 StDiff:6293600 CameraFPS: 158.89,
queueFrames.Count = 16
Time consumption of receiving 16 photos: 127.854 ms, and additional 69.2965 ms for image processing!

Start capturing 16 photos ..., i = 0000000003, Temp = 48.7000
FrameID: 000 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355625136600 FirstFrameConsume: 30.381 ms
FrameID: 001 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355631430100 StDiff:6293500 CameraFPS: 158.89,
FrameID: 002 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355637723700 StDiff:6293600 CameraFPS: 158.89,
FrameID: 003 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355644017200 StDiff:6293500 CameraFPS: 158.89,
FrameID: 004 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355650310800 StDiff:6293600 CameraFPS: 158.89,
FrameID: 005 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355656604300 StDiff:6293500 CameraFPS: 158.89,
FrameID: 006 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355662897800 StDiff:6293500 CameraFPS: 158.89,
FrameID: 007 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355669191400 StDiff:6293600 CameraFPS: 158.89,
FrameID: 008 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355675484900 StDiff:6293500 CameraFPS: 158.89,
FrameID: 009 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355681778400 StDiff:6293500 CameraFPS: 158.89,
FrameID: 010 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355688072000 StDiff:6293600 CameraFPS: 158.89,
FrameID: 011 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355694365500 StDiff:6293500 CameraFPS: 158.89,
FrameID: 012 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355700659000 StDiff:6293500 CameraFPS: 158.89,
FrameID: 013 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355706952600 StDiff:6293600 CameraFPS: 158.89,
FrameID: 014 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355713246100 StDiff:6293500 CameraFPS: 158.89,
FrameID: 015 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1355719539600 StDiff:6293500 CameraFPS: 158.89,
queueFrames.Count = 16
Time consumption of receiving 16 photos: 124.111 ms, and additional 61.4901 ms for image processing!

Start capturing 16 photos ..., i = 0000000004, Temp = 48.7000
FrameID: 000 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356246381000 FirstFrameConsume: 32.214 ms
FrameID: 001 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356252674500 StDiff:6293500 CameraFPS: 158.89,
FrameID: 002 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356258968000 StDiff:6293500 CameraFPS: 158.89,
FrameID: 003 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356265261600 StDiff:6293600 CameraFPS: 158.89,
FrameID: 004 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356271555100 StDiff:6293500 CameraFPS: 158.89,
FrameID: 005 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356277848700 StDiff:6293600 CameraFPS: 158.89,
FrameID: 006 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356284142200 StDiff:6293500 CameraFPS: 158.89,
FrameID: 007 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356290435700 StDiff:6293500 CameraFPS: 158.89,
FrameID: 008 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356296729300 StDiff:6293600 CameraFPS: 158.89,
FrameID: 009 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356303022800 StDiff:6293500 CameraFPS: 158.89,
FrameID: 010 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356309316400 StDiff:6293600 CameraFPS: 158.89,
FrameID: 011 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356315609900 StDiff:6293500 CameraFPS: 158.89,
FrameID: 012 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356321903400 StDiff:6293500 CameraFPS: 158.89,
FrameID: 013 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356328197000 StDiff:6293600 CameraFPS: 158.89,
FrameID: 014 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356334490500 StDiff:6293500 CameraFPS: 158.89,
FrameID: 015 [Missed: 0, Incomplete: 0]  Status:Complete Size:1200x800 Format:VmbPixelFormatMono8 Stamp:1356340784000 StDiff:6293500 CameraFPS: 158.89,
queueFrames.Count = 16
Time consumption of receiving 16 photos: 125.942 ms, and additional 63.4386 ms for image processing!




```




