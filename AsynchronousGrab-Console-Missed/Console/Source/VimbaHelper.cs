/*=============================================================================
  Copyright (C) 2012 - 2016 Allied Vision Technologies.  All Rights Reserved.

  Redistribution of this file, in original or modified form, without
  prior written consent of Allied Vision Technologies is prohibited.

-------------------------------------------------------------------------------

  File:        VimbaHelper.cs

  Description: Implementation file for the VimbaHelper class that demonstrates
               how to implement an asynchronous, continuous image acquisition
               with VimbaNET.

-------------------------------------------------------------------------------

  THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR IMPLIED
  WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF TITLE,
  NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS FOR A PARTICULAR  PURPOSE ARE
  DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
  AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

=============================================================================*/

using OpenCvSharp;

namespace AsynchronousGrabConsole
{
    using AVT.VmbAPINET;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// A helper class as a wrapper around Vimba
    /// </summary>
    public class VimbaHelper
    {
        private Vimba m_Vimba = null;                           // Main Vimba API entry object
        private Camera m_Camera = null;                         // Camera object if camera is open
        private bool m_Acquiring = false;                       // Flag to remember if acquisition is running

        static Semaphore semMultiFrames = new Semaphore(0, 1);
        private bool m_Queuing = false;                       // Flag to remember if acquisition is running
        private static object s_lock = new object();
        private bool m_PrintedTag = false;
        long m_RoundIndex = 0;
        double m_Temp = 0;

        private FrameInfos m_ShowFrameInfo = FrameInfos.Off;    // Indicates if frame info is shown in the console
        private long systemTime = DateTime.Now.Ticks;  // Hold the system time to calculate the frame rate
        private ulong m_FrameID = 0;

        // Joe GE: Statistics for failures
        private int m_FramesMissed = 0; // Not consecutive frames
        private int m_FramesFailed = 0; // Not completed frame status
        private ulong m_FrameID_Prev = 0;
        private ulong m_FrameTimeStampPre = 0;

        private long tkCaptureStart = DateTime.Now.Ticks;
        private long tkCaptureComplete = DateTime.Now.Ticks;

        Queue queueFrames = new Queue(16);

        StreamWriter swLog = new StreamWriter("truking-log.csv");


        /// <summary>
        /// Initializes a new instance of the VimbaHelper class
        /// </summary>
        public VimbaHelper()
        {
        }

        /// <summary>
        /// Finalizes an instance of the VimbaHelper class and shuts down Vimba
        /// </summary>
        ~VimbaHelper()
        {
            // Release Vimba API if user forgot to call Shutdown
            ReleaseVimba();
        }

        /// <summary>
        /// Frame Infos Enumeration
        /// </summary>
        public enum FrameInfos
        {
            /// <summary>
            /// Do not Show frame information
            /// </summary>
            Off = 0,

            /// <summary>
            /// Show Frame information
            /// </summary>
            Show,

            /// <summary>
            /// Show frame information, if a frame is lost or a frame is not complete
            /// </summary>
            Automatic
        }

        /// <summary>
        /// Gets the current camera list
        /// </summary>
        public List<Camera> CameraList
        {
            get
            {
                // Check if API has been started up at all
                if (null == m_Vimba)
                {
                    throw new Exception("Vimba is not started.");
                }

                List<Camera> cameraList = new List<Camera>();
                CameraCollection cameras = m_Vimba.Cameras;
                foreach (Camera camera in cameras)
                {
                    cameraList.Add(camera);
                }

                return cameraList;
            }
        }

        /// <summary>
        /// Starts up Vimba API and loads all transport layers
        /// </summary>
        public void Startup()
        {
            // Instantiate main Vimba object
            Vimba vimba = new Vimba();

            // Start up Vimba API
            vimba.Startup();
            m_Vimba = vimba;
        }

        /// <summary>
        /// Shuts down Vimba API
        /// </summary>
        public void Shutdown()
        {
            // Check if API has been started up at all
            if (null == m_Vimba)
            {
                throw new Exception("Vimba has not been started.");
            }

            ReleaseVimba();
        }

        /// <summary>
        /// Returns the version of Vimba API
        /// </summary>
        /// <returns></returns>
        public String GetVersion()
        {
            if (null == m_Vimba)
            {
                throw new Exception("Vimba has not been started.");
            }
            VmbVersionInfo_t version_info = m_Vimba.Version;
            return String.Format("{0:D}.{1:D}.{2:D}", version_info.major, version_info.minor, version_info.patch);
        }

        /// <summary>
        /// Starts the continuous image acquisition and opens the camera
        /// Registers the event handler for the new frame event
        /// </summary>
        /// <param name="id">The camera ID</param>
        /// <param name="showCameraInfo">Flag to indicate if the Camera info data shall be shown or not</param>
        public void StartContinuousImageAcquisition(string id, FrameInfos showCameraInfo)
        {
            // Check parameters
            if (null == id)
            {
                throw new ArgumentNullException("id");
            }

            // Check if API has been started up at all
            if (null == m_Vimba)
            {
                throw new Exception("Vimba is not started.");
            }

            // Check if a camera is already open
            if (null != m_Camera)
            {
                throw new Exception("A camera is already open.");
            }

            // show camera info's in the console Output :
            m_ShowFrameInfo = showCameraInfo;

            // Open camera
            m_Camera = m_Vimba.OpenCameraByID(id, VmbAccessModeType.VmbAccessModeFull);
            if (null == m_Camera)
            {
                throw new NullReferenceException("No camera retrieved.");
            }

            // Set the GeV packet size to the highest possible value
            // (In this example we do not test whether this cam actually is a GigE cam)
            try
            {
                m_Camera.Features["GVSPAdjustPacketSize"].RunCommand();
                while (false == m_Camera.Features["GVSPAdjustPacketSize"].IsCommandDone())
                {
                    // Do nothing
                }
            }

            catch
            {
                // Do nothing
            }

            bool error = true;
            try
            {
                // Register frame callback
                m_Camera.OnFrameReceived += this.OnFrameReceived;

                // Reset member variables
                m_Acquiring = true;

                // Start synchronous image acquisition (grab)
                m_Camera.StartContinuousImageAcquisition(3);

                error = false;
            }
            finally
            {
                // Close camera already if there was an error
                if (true == error)
                {
                    ReleaseCamera();
                }
            }
        }

        /// <summary>
        /// Stops the image acquisition
        /// </summary>
        public void StopContinuousImageAcquisition()
        {
            // Check if API has been started up at all
            if (null == m_Vimba)
            {
                throw new Exception("Vimba is not started.");
            }

            // Check if no camera is open
            if (null == m_Camera)
            {
                throw new Exception("No camera open.");
            }

            // Close camera
            ReleaseCamera();
        }

        public void OpenCamera(string id, FrameInfos showCameraInfo)
        {
            // Check parameters
            if (null == id)
            {
                throw new ArgumentNullException("id");
            }

            // Check if API has been started up at all
            if (null == m_Vimba)
            {
                throw new Exception("Vimba is not started.");
            }

            // Check if a camera is already open
            if (null != m_Camera)
            {
                throw new Exception("A camera is already open.");
            }

            // show camera info's in the console Output :
            m_ShowFrameInfo = showCameraInfo;

            // Open camera
            m_Camera = m_Vimba.OpenCameraByID(id, VmbAccessModeType.VmbAccessModeFull);
            if (null == m_Camera)
            {
                throw new NullReferenceException("No camera retrieved.");
            }

            // Set the GeV packet size to the highest possible value
            // (In this example we do not test whether this cam actually is a GigE cam)
            try
            {

                // Joe: set multiframe 16 photos mode
                //m_Camera.Features["AcquisitionMode"].EnumValue = "MultiFrame";
                //m_Camera.Features["AcquisitionFrameCount"].IntValue = 16;

                // Joe: use FreeRun mode

                m_Camera.Features["DeviceLinkThroughputLimit"].IntValue = 440000000;

                m_Camera.Features["Width"].IntValue = 1200;
                m_Camera.Features["Height"].IntValue = 800;

                m_Camera.Features["OffsetX"].IntValue = (m_Camera.Features["WidthMax"].IntValue - m_Camera.Features["Width"].IntValue) / 2 / 4 * 4;
                m_Camera.Features["OffsetY"].IntValue = (m_Camera.Features["HeightMax"].IntValue - m_Camera.Features["Height"].IntValue) / 2 / 4 * 4;

            }
            catch
            {
                // Do nothing
                Console.WriteLine("Failed to set parameters for the camera !");
            }

            bool error = true;
            try
            {
                // Register frame callback
                m_Camera.OnFrameReceived += this.OnFrameReceived;

                // Reset member variables
                //m_Acquiring = true;
                m_Acquiring = false;
                error = false;
            }
            finally
            {
                // Close camera already if there was an error
                if (true == error)
                {
                    ReleaseCamera();
                }
            }
        }

        public void CloseCamera()
        {
            ReleaseCamera();

        }

        public void StartCapture(long iteration, double temp)
        {

            // Reset new round of capturing
            m_Acquiring = true;
            m_FrameID = 0;
            systemTime = DateTime.Now.Ticks;  // Hold the system time to calculate the frame rate

            tkCaptureStart = DateTime.Now.Ticks;


            // Start asynchronous image acquisition (grab)
            m_Camera.StartContinuousImageAcquisition(16);
        }

        public void StartCapture()
        {
            // swLog.Write("index, {0:0000000000}, temp, {1:0.000}, ", iteration, temp);

            // Reset new round of capturing
            m_Acquiring = true;
            m_FrameID = 0;
            systemTime = DateTime.Now.Ticks;  // Hold the system time to calculate the frame rate

            tkCaptureStart = DateTime.Now.Ticks;


            // Start asynchronous image acquisition (grab)
            m_Camera.StartContinuousImageAcquisition(16);
        }

        public void StopCapture()
        {
            if (true == m_Acquiring)
            {
                m_Camera.StopContinuousImageAcquisition();
                m_Acquiring = false;
            }
        }


        public double GetCameraTemprature()
        {
            return m_Camera.Features["DeviceTemperature"].FloatValue;
        }

        /// <summary>
        /// Display the image as dots or show the FrameInfo
        /// </summary>
        /// <param name="frame">The received frame</param>
        private void OutPutFrameInfo(Frame frame, bool bFirstFrame, ulong nFirstFrameID)
        {
            bool showFrameInfo = false;
            double frameRate = 0;
            string status = string.Empty;

            if (null == frame)
            {
                throw new ArgumentNullException("frame");
            }

            // Remember the first frame id for the 16 frames group
            if (bFirstFrame)
                m_FrameID = nFirstFrameID;

            frameRate = CalcFPS(); // Get frame rate

            status = GetFrameStatus(frame); // Get frame status

            ulong nFramesMissing = GetFramesMissing(frame);
            if (0 < nFramesMissing)
            {
                if (1 == nFramesMissing)
                {
                    Console.WriteLine("1 missing frame detected\n");
                }
                else
                {
                    Console.WriteLine("{0} missing frames detected\n", nFramesMissing);
                }

                // Joe: Accumulate all missed frames
                m_FramesMissed += (int)nFramesMissing;
            }

            showFrameInfo = status != "Complete" || 0 < nFramesMissing;  // Show frame infos for incomplete frame or if a frame is lost

            // Joe: Accumulate incomplete frames
            if (status != "Complete")
            {
                m_FramesFailed++;
            }


            if (m_ShowFrameInfo == FrameInfos.Show || (showFrameInfo && m_ShowFrameInfo == FrameInfos.Automatic))
            {
                if (m_ShowFrameInfo == FrameInfos.Automatic)
                    Console.WriteLine(string.Empty);

                Console.Write("FrameID: {0:000}", frame.FrameID);

                // Joe: print out accumulated missed/incomplete frames counts
                Console.Write(" [Missed: {0}, Incomplete: {1}] ", m_FramesMissed, m_FramesFailed);

                Console.Write(" Status:");

                Console.Write(status);

                Console.Write(" Size:");
                Console.Write(frame.Width);

                Console.Write("x");
                Console.Write(frame.Height);

                Console.Write(" Format:");
                Console.Write(frame.PixelFormat);

                Console.Write(" Stamp:");
                Console.Write(frame.Timestamp);

                ulong ulCameraStDiff = frame.Timestamp - m_FrameTimeStampPre;

                if (bFirstFrame)
                {
                }
                else
                {
                    Console.Write(" StDiff:");
                    Console.Write(ulCameraStDiff);
                }
                m_FrameTimeStampPre = frame.Timestamp;

                if (bFirstFrame)
                {
                    long firstArrivedFrameTicks = DateTime.Now.Ticks;
                    TimeSpan elapsedSpanFirstFrame = new TimeSpan(firstArrivedFrameTicks - tkCaptureStart);
                    Console.Write(" FirstFrameConsume: {0:000.000} ms", elapsedSpanFirstFrame.TotalMilliseconds);

                    swLog.Write("firstframe, {0:000.000}, ", elapsedSpanFirstFrame.TotalMilliseconds);
                }
                else
                {
                    Console.Write(" CameraFPS: {0:.00},", (double)1000000000 / ulCameraStDiff);
                }

                /*
                Console.Write(" ClientFPS:");

                if (!double.IsNaN(frameRate) && !double.IsInfinity(frameRate) && nFramesMissing <= 0)
                {
                    Console.WriteLine(frameRate.ToString("0.0"));
                }
                else
                {
                    Console.WriteLine("?");
                }
                */
                Console.WriteLine(" ");
            }
            else
            {
                Console.Write(".");
            }
        }

        /// <summary>
        /// Checks if a frame is missing
        /// </summary>
        /// <param name="frame">The received frame</param>
        /// <returns>True if a frame is missing</returns>
        private ulong GetFramesMissing(Frame frame)
        {
            ulong missingFrames;
            // For USB the very first frame ID is 0
            if (frame.FrameID == 0 && m_FrameID == 0)
            {
                missingFrames = 0;
            }
            else if (frame.FrameID - m_FrameID == 1)
            {
                missingFrames = 0;
            }
            else
            {
                missingFrames = frame.FrameID - m_FrameID;
            }

            m_FrameID = frame.FrameID;

            return missingFrames;
        }

        /// <summary>
        /// Calculates the Frame rate
        /// </summary>
        /// <returns>The frame rate</returns>
        private double CalcFPS()
        {
            long sytemTimeLocal = DateTime.Now.Ticks;

            double fps = 1000 / (double)(sytemTimeLocal - systemTime);

            systemTime = sytemTimeLocal;

            return fps;
        }

        /// <summary>
        /// Gets the frame Status as a string
        /// </summary>
        /// <param name="frame">The received frame</param>
        /// <returns>Frame Status string</returns>
        private string GetFrameStatus(Frame frame)
        {
            string status = string.Empty;

            switch (frame.ReceiveStatus)
            {
                case VmbFrameStatusType.VmbFrameStatusComplete:
                    status = "Complete";
                    break;

                case VmbFrameStatusType.VmbFrameStatusIncomplete:
                    status = "Incomplete";
                    break;

                case VmbFrameStatusType.VmbFrameStatusTooSmall:
                    status = "Too small";
                    break;

                case VmbFrameStatusType.VmbFrameStatusInvalid:
                    status = "Invalid";
                    break;

                default:
                    status = "?";
                    break;
            }

            return status;
        }



        /// <summary>
        /// 使用OpenCvSharp处理16张图片 
        /// </summary>
        /// <returns>时间消耗ms</returns>
        private double ProcessImages()
        {
            Console.WriteLine("queueFrames.Count = {0}", queueFrames.Count);

            long tkImgProcStart = DateTime.Now.Ticks;


            long idx = 0;
            while (queueFrames.Count > 0)
            {
                Frame frame = (Frame)queueFrames.Dequeue();
                idx++;
                String strName = String.Format("opencv{0:00}.png", idx);
                String strNameCanny = String.Format("opencv{0:00}-canny.png", idx);

                // Try OpenCvSharp processing
                Mat img = new Mat((int)frame.Height, (int)frame.Width, MatType.CV_8U, frame.Buffer);
                Mat res = new Mat();

                Cv2.Canny(img, res, 50, 200);

                // 保存最近一张
                if (queueFrames.Count == 0)
                {
                    img.SaveImage(strName);
                    res.SaveImage(strNameCanny);
                }
            }

            long tkImgProcEnd = DateTime.Now.Ticks;

            TimeSpan tmSpanImgProc = new TimeSpan(tkImgProcEnd - tkImgProcStart);

            return tmSpanImgProc.TotalMilliseconds;
        }

        /// <summary>
        /// Handles the frame received event
        /// The frame is displayed and eventually queued
        /// </summary>
        /// <param name="frame">The received frame</param>
        private void OnFrameReceived(Frame frame)
        {
            const int simulate_image_processing = 200; // ms
            try
            {
                lock (s_lock)
                {
                    if (m_Queuing && queueFrames.Count < 16)
                    {
                        queueFrames.Enqueue(frame);

                        if (queueFrames.Count == 1)
                        {
                            m_RoundIndex++;
                            swLog.Write("index, {0:0000000000}, temp, {1:00.000}, ", m_RoundIndex, m_Temp);
                        }

                        // Convert frame into displayable image
                        OutPutFrameInfo(frame, queueFrames.Count == 1, frame.FrameID);
                    } 
                }




                // Joe: 
                // 图像处理 ------------------------------------------------------------------ //
                // 图像处理 ------------------------------------------------------------------ //
                // 图像处理 ------------------------------------------------------------------ //
                // automatically stop capturing when 16 maximum photos captured.
                lock (s_lock)
                {


                    if (queueFrames.Count == 16 && m_PrintedTag == false)
                    {
                        m_PrintedTag = true;
                        m_Queuing = false;

                        // 得到全部16张
                        semMultiFrames.Release(1);

                        // 获取当前时间的毫秒计数
                        tkCaptureComplete = DateTime.Now.Ticks;
                        TimeSpan elapsedSpan = new TimeSpan(tkCaptureComplete - tkCaptureStart);


                        // 模拟图像处理 200ms
                        // 注意：当前处理图片接收回调函数，为避免影响图像接收，在最后一张（第16张图像）接收完成后，再统一做图像处理方面的工作。
                        //Thread.Sleep(simulate_image_processing);
                        double tmImgProc = ProcessImages();


                        Console.WriteLine("Time consumption of receiving 16 photos: {0:000.000} ms, and additional {1} ms for image processing!",
                            elapsedSpan.TotalMilliseconds, tmImgProc);

                        //swLog.WriteLine("Time consumption of receiving 16 photos: {0:000.000} ms, and additional {1} ms for image processing!",
                        //    elapsedSpan.TotalMilliseconds, tmImgProc);
                        swLog.WriteLine("16frames_time, {0:000.000}, imgproc_time, {1:000.000}, ", elapsedSpan.TotalMilliseconds, tmImgProc);
                        swLog.Flush();

                    }
                }
                // 图像处理 ------------------------------------------------------------------ //

            }
            finally
            {
                // We make sure to always return the frame to the API
                try
                {
                    m_Camera.QueueFrame(frame);
                }
                catch
                {
                    // Do nothing
                }
            }

        }

        public bool Waiting16Frames(int timeout)
        {
            return semMultiFrames.WaitOne(timeout);
        }

        public bool Start16FramesCapturing(double temp)
        {
            // Start new round of capturing 
            tkCaptureStart = DateTime.Now.Ticks;

            lock (s_lock)
            {
                queueFrames.Clear();
                m_PrintedTag = false;
                m_Queuing = true;
                m_Temp = temp;

                m_FramesFailed = 0;
                m_FramesMissed = 0;
            }

            return true;
        }

        /// <summary>
        ///  Unregisters the new frame event
        ///  Stops the capture engine
        ///  Closes the camera
        /// </summary>
        private void ReleaseCamera()
        {
            if (null != m_Camera)
            {
                // We can use cascaded try-finally blocks to release the
                // camera step by step to make sure that every step is executed.
                try
                {
                    try
                    {
                        try
                        {
                            m_Camera.OnFrameReceived -= this.OnFrameReceived;
                        }
                        finally
                        {
                            if (true == m_Acquiring)
                            {
                                m_Camera.StopContinuousImageAcquisition();
                            }
                        }
                    }
                    finally
                    {
                        m_Acquiring = false;
                        m_Camera.Close();
                    }
                }
                finally
                {
                    m_Camera = null;
                }
            }
        }

        /// <summary>
        ///  Releases the camera
        ///  Shuts down Vimba
        /// </summary>
        private void ReleaseVimba()
        {
            if (null != m_Vimba)
            {
                // We can use cascaded try-finally blocks to release the
                // Vimba API step by step to make sure that every step is executed.
                try
                {
                    try
                    {
                        // First we release the camera (if there is one)
                        ReleaseCamera();
                    }
                    finally
                    {
                        // Now finally shutdown the API
                        m_Vimba.Shutdown();
                    }
                }
                finally
                {
                    m_Vimba = null;
                }
            }
        }
    }
}