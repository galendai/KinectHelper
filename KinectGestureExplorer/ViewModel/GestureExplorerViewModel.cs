using GalaSoft.MvvmLight;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Collections.ObjectModel;

using Kinect.Helper.Controller;
using Kinect.Helper.Controller.Body;
using Kinect.Helper.Controller.Util;
using Kinect.Helper.Gestures;
using KinectGestureExplorer.Model;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System;
using System.Windows;

namespace KinectGestureExplorer.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class GestureExplorerViewModel : ViewModelBase
    {
        #region Private members

        /// <summary>
        /// Kinect Sensor
        /// </summary>
        private KinectSensor _Sensor = null;

        /// <summary>
        /// Kinect manager of Kinect.Helper.Controller
        /// </summary>
        private KinectManager _KinectManager = null;

        /// <summary>
        /// Kinect Interaction Helper
        /// </summary>
        private KinectInteractionHelper _KinectInteractionHelper = null;

        private Hand _LeftHand;

        private Hand _RightHand;

        #endregion

        #region Properties of Kinect Sensor Status and Hand Status

        /// <summary>
        /// The <see cref="KinectSkeletonCanvas" /> property's name.
        /// </summary>
        public const string KinectViewerCanvasPropertyName = "KinectSkeletonCanvas";

        private Canvas _KinectSkeletonCanvas = null;

        /// <summary>
        /// Sets and gets the KinectViewerCanvas property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Canvas KinectSkeletonCanvas
        {
            get
            {
                return _KinectSkeletonCanvas;
            }
            set
            {
                Set(KinectViewerCanvasPropertyName, ref _KinectSkeletonCanvas, value);
            }
        }

        /// <summary>
        /// The <see cref="KinectColorImage" /> property's name.
        /// </summary>
        public const string KinectColorImagePropertyName = "KinectColorImage";

        private WriteableBitmap _KinectColorImage = null;

        /// <summary>
        /// Sets and gets the KinectColorImage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public WriteableBitmap KinectColorImage
        {
            get
            {
                return _KinectColorImage;
            }
            set
            {
                Set(KinectColorImagePropertyName, ref _KinectColorImage, value);
            }
        }

        /// <summary>
        /// The <see cref="SensorChooser" /> property's name.
        /// </summary>
        public const string SensorChooserPropertyName = "SensorChooser";

        private KinectSensorChooser _SensorChooser = null;

        /// <summary>
        /// Sets and gets the SensorChooser property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public KinectSensorChooser SensorChooser
        {
            get
            {
                return _SensorChooser;
            }
            set
            {
                Set(SensorChooserPropertyName, ref _SensorChooser, value);
            }
        }

        /// <summary>
        /// The <see cref="SensorStatus" /> property's name.
        /// </summary>
        public const string SensorStatusPropertyName = "SensorStatus";

        private string _SensorSatus = "Not Connected";

        /// <summary>
        /// Sets and gets the SensorStatus property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SensorStatus
        {
            get
            {
                return _SensorSatus;
            }

            set
            {
                if (_SensorSatus == value)
                {
                    return;
                }

                RaisePropertyChanging(SensorStatusPropertyName);
                _SensorSatus = value;
                RaisePropertyChanged(SensorStatusPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="LeftHandX" /> property's name.
        /// </summary>
        public const string LeftHandXPropertyName = "LeftHandX";

        private double _LeftHandX = 0;

        /// <summary>
        /// Sets and gets the LeftHandX property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double LeftHandX
        {
            get
            {
                return _LeftHandX;
            }
            set
            {
                Set(LeftHandXPropertyName, ref _LeftHandX, value);
            }
        }

        /// <summary>
        /// The <see cref="LeftHandY" /> property's name.
        /// </summary>
        public const string LeftHandYPropertyName = "LeftHandY";

        private double _LeftHandY = 0;

        /// <summary>
        /// Sets and gets the LeftHandY property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double LeftHandY
        {
            get
            {
                return _LeftHandY;
            }
            set
            {
                Set(LeftHandYPropertyName, ref _LeftHandY, value);
            }
        }

        /// <summary>
        /// The <see cref="RightHandX" /> property's name.
        /// </summary>
        public const string RightHandXPropertyName = "RightHandX";

        private double _RightHandX = 0;

        /// <summary>
        /// Sets and gets the RightHandX property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double RightHandX
        {
            get
            {
                return _RightHandX;
            }
            set
            {
                Set(RightHandXPropertyName, ref _RightHandX, value);
            }
        }

        /// <summary>
        /// The <see cref="RightHandY" /> property's name.
        /// </summary>
        public const string RightHandYPropertyName = "RightHandY";

        private double _RightHandY = 0;

        /// <summary>
        /// Sets and gets the RightHandY property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double  RightHandY
        {
            get
            {
                return _RightHandY;
            }
            set
            {
                Set(RightHandYPropertyName, ref _RightHandY, value);
            }
        }

        /// <summary>
        /// The <see cref="LeftHandState" /> property's name.
        /// </summary>
        public const string LeftHandStatePropertyName = "LeftHandState";

        private string _LeftHandState = "None";

        /// <summary>
        /// Sets and gets the LeftHandState property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LeftHandState
        {
            get
            {
                return _LeftHandState;
            }
            set
            {
                Set(LeftHandStatePropertyName, ref _LeftHandState, value);
            }
        }

        /// <summary>
        /// The <see cref="RightHandState" /> property's name.
        /// </summary>
        public const string RightHandStatePropertyName = "RightHandState";

        private string _RightHandState = "None";

        /// <summary>
        /// Sets and gets the RightHandState property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string RightHandState
        {
            get
            {
                return _RightHandState;
            }
            set
            {
                Set(RightHandStatePropertyName, ref _RightHandState, value);
            }
        }

        /// <summary>
        /// The <see cref="GestureText" /> property's name.
        /// </summary>
        public const string GestureTextPropertyName = "GestureText";

        private string _GestureText = "None";

        /// <summary>
        /// Sets and gets the Gesture property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string GestureText
        {
            get
            {
                return _GestureText;
            }
            set
            {
                Set(GestureTextPropertyName, ref _GestureText, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the GestureExplorerViewModel class.
        /// </summary>
        public GestureExplorerViewModel()
        {
            if (!ViewModelBase.IsInDesignModeStatic)
            {
                // Add PropertyChange event handler
                _KinectManager = KinectManager.Instance;
                _KinectManager.PropertyChanged += KinectManager_PropertyChanged;

                InitKinectSensor();
            }

        }

        /// <summary>
        /// Event handler method for KinectInteractionHelper propertychanged
        /// </summary>
        /// <param name="sender">Object of sender</param>
        /// <param name="e">Event arguments</param>
        private void KinectInteractionHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // get the left and right hand
            _LeftHand = _KinectInteractionHelper.LeftHand;
            _RightHand = _KinectInteractionHelper.RightHand;

            // get the hand position
            Point leftHand;
            Point rightHand;
            if (_KinectInteractionHelper.Container != null)
            {
                // map the left hand position using depthdata
                leftHand = KinectControllerUtil.GetDepth2DPosition(
                    _Sensor,
                    this._KinectInteractionHelper.Container.RenderSize,
                    _LeftHand.HandJoint.Position,
                    _Sensor.DepthStream.Format,
                    _Sensor.DepthStream.FrameWidth,
                    _Sensor.DepthStream.FrameHeight);

                // map the right hand position using color data
                rightHand = KinectControllerUtil.GetColor2DPosition(
                    _Sensor,
                    this._KinectInteractionHelper.Container.RenderSize,
                    _RightHand.HandJoint.Position,
                    _Sensor.ColorStream.Format,
                    _Sensor.ColorStream.FrameWidth,
                    _Sensor.ColorStream.FrameHeight);

                // set the hand positions value
                LeftHandX = leftHand.X;
                LeftHandY = leftHand.Y;
                RightHandX = rightHand.X;
                RightHandY = rightHand.Y;
            }


            // get the hand state
            LeftHandState = _LeftHand.EventType.ToString();
            RightHandState = _RightHand.EventType.ToString();

            // update the gesture collection if gesture is updated
            this.GestureText = _KinectInteractionHelper.Gesture;
        }

        /// <summary>
        /// Event handler method for KinectManager PropertyChanged
        /// </summary>
        /// <param name="sender">Object of sender</param>
        /// <param name="e">Event arguments</param>
        private void KinectManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // update the sensor status
            SensorStatus = _KinectManager.SensorStatus.ToString();

            // update the player image
            KinectColorImage = _KinectManager.ColorBitmap;

            // update the skeleton canvas
            KinectSkeletonCanvas = _KinectManager.SkeletonCanvas;
        }

        /// <summary>
        /// Initialize kinect sensor
        /// </summary>
        private void InitKinectSensor()
        { 
            // Initialize the Sensor
            _KinectManager.EnableColorStream();
            _KinectManager.EnableDepthStream();
            _KinectManager.EnableSkeletonStream();

            // Start the sensor
            _KinectManager.Start();
            this._Sensor = _KinectManager.Sensor;
            this.SensorChooser = _KinectManager.SensorChooser;

            // initialize the interaction helper
            if (this._Sensor != null)
            {
                _KinectInteractionHelper = KinectInteractionHelper.Instance;
                _KinectInteractionHelper.Init(this._Sensor);
            }

            // enable colorviewer
            _KinectManager.SetColorViewerVisibility(true);

            //enable skeletonviewer
            _KinectManager.SetSkeletonViewerVisibility(true);

            if (_KinectInteractionHelper.Enabled)
            {
                // enable gesture controller
                _KinectInteractionHelper.EnableGestureController(true);
                _KinectInteractionHelper.RegisterGestures(GestureType.GripZoomIn);
                _KinectInteractionHelper.RegisterGestures(GestureType.GripZoomOut);
                _KinectInteractionHelper.RegisterGestures(GestureType.JoinedHands);
                _KinectInteractionHelper.RegisterGestures(GestureType.Menu);
                _KinectInteractionHelper.RegisterGestures(GestureType.WaveLeft);
                _KinectInteractionHelper.RegisterGestures(GestureType.WaveRight);

                // add property changed event handler
                _KinectInteractionHelper.PropertyChanged += this.KinectInteractionHelper_PropertyChanged;
            }

        }


    }
}