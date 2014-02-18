using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;

using Kinect.Helper.Controller.Common;

using Kinect.Helper.Controller.Util;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Windows.Controls;


namespace Kinect.Helper.Controller
{
    public sealed class KinectManager : ObservableObject
    {
        #region Singleton Instance Declartion

        private static KinectManager _Instance;
        public static KinectManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new KinectManager();
                return _Instance;
            }
        }

        #endregion

        #region Public Properties for Sensor Instance

        /// <summary>
        /// Retruns the instance of current KinectSensor
        /// </summary>       
        private KinectSensor _Sensor;
        public KinectSensor Sensor 
        {
            get
            {
                return _Sensor;
            }
            set
            {
                _Sensor = value;
                RaisePropertyChangedEvent("Sensor");
            }
        }

        /// <summary>
        /// Return the instance of current SensorChooser
        /// </summary>
        public KinectSensorChooser SensorChooser
        {
            get;
            set;
        }

        /// <summary>
        /// Status of current KinectSensor
        /// </summary>
        public KinectStatus SensorStatus
        {
            get
            {
                try
                {
                    return Sensor.Status;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("ERR: Unable to get sensor status. Message - " + e.ToString());
                    return KinectStatus.Error;
                }
            }
        }

        /// <summary>
        /// Sensor's Depth Range
        /// </summary>
        public DepthRange SensorDepthRange { get; set; }

        /// <summary>
        /// Sensor's DepthImageFormat
        /// </summary>
        public DepthImageFormat SensorDepthImageFormat { get; set; }

        /// <summary>
        /// Sensor's ColorImageFormat
        /// </summary>
        public ColorImageFormat SensorColorImageFormat { get; set; }

        /// <summary>
        /// Flag of whether main application is closing
        /// </summary>
        public bool Closing { get; set; }

        #endregion

        #region Private variables
        
        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] _ColorPixels;

        /// <summary>
        /// Flag for ColorViewer
        /// </summary>
        private bool _IsColorViewerEnable = false;

        /// <summary>
        /// Flag for SkeletonViewer
        /// </summary>
        private bool _IsSkeletonViewerEnable = false;
        #endregion

        #region Public Properties for ColorStream
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap _ColorBitmap;
        public WriteableBitmap ColorBitmap 
        {
            get
            {
                return _ColorBitmap;
            }
            set
            {
                _ColorBitmap = value;
                RaisePropertyChangedEvent("ColorBitmap");
            }
        }

        /// <summary>
        /// Canvas to display user skeleton
        /// </summary>
        private Canvas _SkeletonCanvas;
        public Canvas SkeletonCanvas
        {
            get
            {
                return _SkeletonCanvas;
            }
            set
            {
                _SkeletonCanvas = value;
                RaisePropertyChangedEvent("SkeletonCanvas");
            }
        }

        #endregion

        #region Public Properties for SkeletonStream

        /// <summary>
        /// Set default Skeleton Tracking mode to default.
        /// </summary>
        private SkeletonTrackingMode _SkeletonTrackMode = SkeletonTrackingMode.Default;
        public SkeletonTrackingMode SkeletonTrackMode
        {
            get
            {
                return _SkeletonTrackMode;
            }
            set
            {
                _SkeletonTrackMode = value;
            }
        }

        /// <summary>
        /// Skeleton which have been tracked by Kinect sensor
        /// </summary>
        public Collection<Skeleton> TrackedSkeletons { get; set; }

        /// <summary>
        /// Skeleton which only have position tracked by Kinect sensor
        /// </summary>
        public Collection<Skeleton> PositionOnlySkeletons { get; set; }

        /// <summary>
        /// Set the joint filtering of the skeleton
        /// </summary>
        private TransformSmoothParameters _SmoothParameter = SmoothingParameter.Default;
        public JointFiltering SmoothParameter
        {
            set
            { 
                switch(value)
                {
                    case JointFiltering.Default:
                        _SmoothParameter = SmoothingParameter.Default;
                        break;
                    case JointFiltering.Smooth:
                        _SmoothParameter = SmoothingParameter.Smooth;
                        break;
                    case JointFiltering.VerySmooth:
                        _SmoothParameter = SmoothingParameter.VerySmooth;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor, and set the DepthImageFormat to 640x480Fps30 and DepthRange to Near
        /// </summary>
        public KinectManager() 
        {
            // star the sensor Chooser and Set the Sensor instance
            if (SensorChooser == null)
            {
                SensorChooser = new KinectSensorChooser();
            }
            SensorChooser.KinectChanged += this.SensorChooserOnKinectChanged;
            SensorChooser.Start();
            if (SensorChooser.Status == ChooserStatus.SensorStarted)
            {
                Sensor = SensorChooser.Kinect;
            }

            // set the default Sensor Stream format
            SensorDepthImageFormat = DepthImageFormat.Resolution640x480Fps30;
            SensorColorImageFormat = ColorImageFormat.RgbResolution640x480Fps30;
        }

        /// <summary>
        /// Constructor takes argument to set Sensor's DepthImageFormat and DepthRange properties
        /// </summary>
        /// <param name="depthImageFormat"></param>
        /// <param name="depthRange"></param>
        public KinectManager(DepthImageFormat depthImageFormat, DepthRange depthRange)
        {
            // TODO: Initialize the Sensor with Parameters?
        
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Start the Kinect Sensor
        /// </summary>
        public void Start()
        {
            // start the Sensor
            if (Sensor != null)
                Sensor.Start();
        }

        /// <summary>
        /// Stop the Kinect Sensor
        /// </summary>
        public void Stop()
        { 
            // Stop the Kinect Sensor and Uninitialize all streams
            Sensor.Stop();
            UninitializeKinectStreams();
        }

        /// <summary>
        /// Initialize All Kinect Streams
        /// </summary>
        public void InitializeAllKinectStreams()
        {
            EnableDepthStream();
            EnableSkeletonStream();
            EnableColorStream();
        }

        /// <summary>
        /// Uninitialize All Kinect Streams
        /// </summary>
        public void UninitializeKinectStreams()
        {
            if (Sensor == null)
            {
                return;
            }

            // Stop Streaming
            if (Sensor.SkeletonStream != null)
            {
                Sensor.SkeletonFrameReady -= Sensor_SkeletonFrameReady;
                Sensor.SkeletonStream.Disable();
            }
            if (Sensor.DepthStream != null)
            {
                Sensor.DepthFrameReady -= Sensor_DepthFrameReady;
                Sensor.DepthStream.Disable();
            }
            if (Sensor.ColorStream != null)
            {
                Sensor.ColorFrameReady -= Sensor_ColorFrameReady;
                Sensor.ColorStream.Disable();                
            }

        }

        /// <summary>
        /// Enable Color Viewer for player
        /// </summary>
        /// <param name="visible">If the colorviewer is visible</param>
        public void SetColorViewerVisibility(bool visible)
        {
            this._IsColorViewerEnable = visible;
            // update the color viewer if is enabled
            if (_IsColorViewerEnable)
            {
                // Allocate space to put the pixels we'll receive
                this._ColorPixels = new byte[this.Sensor.ColorStream.FramePixelDataLength];

                // Set the bitmap to be displayed on screen
                this._ColorBitmap = new WriteableBitmap(this.Sensor.ColorStream.FrameWidth, this.Sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            }
        }

        public void SetSkeletonViewerVisibility(bool visible)
        {
            this._IsSkeletonViewerEnable = visible;
        }

        /// <summary>
        /// Enable the Color stream of kinect sensor
        /// </summary>
        public void EnableColorStream()
        {
            try
            {
                if (Sensor != null)
                {
                    Sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(Sensor_ColorFrameReady);
                    Sensor.ColorStream.Enable(SensorColorImageFormat);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("ERR: Unable to enable ColorStream. Message: " + e.ToString());
                return;
            }
        }

        /// <summary>
        /// Enable Depth Stream of kinect sensor
        /// </summary>
        public void EnableDepthStream()
        {
            try
            {
                if (Sensor != null)
                {
                    Sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(Sensor_DepthFrameReady);
                    Sensor.DepthStream.Enable(SensorDepthImageFormat);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERR: Unable to enable DepthStream. Message: " + e.ToString());
                return;
            }

        }

        /// <summary>
        /// Enable Skeleton STream of kinect sensor
        /// </summary>
        public void EnableSkeletonStream()
        {
            try
            {
                if (Sensor != null || Sensor.DepthStream.IsEnabled)
                {
                    Sensor.SkeletonStream.TrackingMode = SkeletonTrackMode;
                    Sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Sensor_SkeletonFrameReady);
                    Sensor.SkeletonStream.Enable(_SmoothParameter);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERR: Unable to enable SkeletonStream. Message: " + e.ToString());
                return;
            }
        }

        #endregion

        #region Private EventHandler Methods

        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (Closing)
            {
                return;
            }

            // TODO - Set the Sensor with User defined Sensor Parameters

            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (!error)
            {
                Sensor = args.NewSensor;
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">>object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (Closing)
            {
                return;
            }

            // Even though we un-register all our event handlers when the sensor
            // changes, there may still be an event for the old sensor in the queue
            // due to the way the KinectSensor delivers events.  So check again here.
            if (this.Sensor != sender)
            {
                return;
            }
            
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null && _ColorPixels != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this._ColorPixels);

                    // Write the pixel data into our bitmap
                    this._ColorBitmap.WritePixels(
                        new Int32Rect(0, 0, this._ColorBitmap.PixelWidth, this._ColorBitmap.PixelHeight),
                        this._ColorPixels,
                        this._ColorBitmap.PixelWidth * sizeof(int),
                        0);

                    // set the property of the ColorBitMap
                    this.ColorBitmap = _ColorBitmap;
                }
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">>object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            if (Closing)
            {
                return;
            }

            // Even though we un-register all our event handlers when the sensor
            // changes, there may still be an event for the old sensor in the queue
            // due to the way the KinectSensor delivers events.  So check again here.
            if (this.Sensor != sender)
            {
                return;
            }

            // TODO - handle depth data

        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">>object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (Closing)
            {
                return;
            }

            // Even though we un-register all our event handlers when the sensor
            // changes, there may still be an event for the old sensor in the queue
            // due to the way the KinectSensor delivers events.  So check again here.
            if (this.Sensor != sender)
            {
                return;
            }

            Skeleton[] skeletons = null;
            TrackedSkeletons = new Collection<Skeleton>();
            PositionOnlySkeletons = new Collection<Skeleton>();

            // Process all skeleton datas
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }

                if (skeletons != null)
                {
                    foreach (Skeleton skeleton in skeletons)
                    {
                        // get the skeletons that have been tracked
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            TrackedSkeletons.Add(skeleton);
                            RaisePropertyChangedEvent("TrackedSkeletons");
                            // update the skeletonviewer canvas
                            if (_IsSkeletonViewerEnable)
                            {
                                if (this._SkeletonCanvas == null) _SkeletonCanvas = new Canvas();
                                this._SkeletonCanvas.ClearSkeletons();
                                this._SkeletonCanvas.DrawSkeleton(skeleton);
                                RaisePropertyChangedEvent("SkeletonCanvas");
                            }

                        }
                        // get the skeletons that have been indentified by position
                        else if (skeleton.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            PositionOnlySkeletons.Add(skeleton);
                            RaisePropertyChangedEvent("PositionOnlySkeletons");
                        }
                    }
                }

            }
        }

        #endregion

    }
}
