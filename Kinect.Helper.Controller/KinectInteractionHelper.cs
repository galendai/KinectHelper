using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;

using Kinect.Helper.Controller.Common;
using Kinect.Helper.Controller.Body;
using Kinect.Helper.Gestures;
using Kinect.Helper.Gestures.Segments;
using Kinect.Helper.Gestures.InteractSegments;
using System.Timers;
using System.Diagnostics;
using System.Windows;

namespace Kinect.Helper.Controller
{
    public class KinectInteractionHelper : ObservableObject
    {
        #region Singleton declartion

        private static KinectInteractionHelper _Instance;
        public static KinectInteractionHelper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new KinectInteractionHelper();
                }
                return _Instance;
            }
        }

        #endregion

        #region Private variables

        /// <summary>
        /// Instance of kinect sensor
        /// </summary>
        private KinectSensor _Sensor;

        /// <summary>
        /// Interaction client
        /// </summary>
        private InteractionAdapter _InteractionAdapter = null;

        /// <summary>
        /// InteractionStream
        /// </summary>
        private InteractionStream _InteractionStream = null;

        /// <summary>
        /// Primary Skeleton
        /// </summary>
        private Skeleton _PrimarySkeleton;

        /// <summary>
        /// Skeleton data
        /// </summary>
        private Skeleton[] _Skeletons;

        /// <summary>
        /// Flag to enable gesture recognizer
        /// </summary>
        private bool _EnableGestureRecognize = false;

        /// <summary>
        /// Flag to enable interact gesture recognizer
        /// </summary>
        private bool _EnableInteractGestureRecognize = false;

        /// <summary>
        /// Instance of gesture contorller
        /// </summary>
        private GestureController _GestureController;

        /// <summary>
        /// Instance of clear timer
        /// </summary>
        private Timer _ClearTimer;
        
        #endregion

        #region Puclic properties

        /// <summary>
        /// Flag of whether KinectInteractionHelper is enabled
        /// </summary>
        private bool _Enabled = false;
        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
        }

        /// <summary>
        /// UIElement Container
        /// </summary>
        public UIElement Container
        {
            get;
            set;
        }

        /// <summary>
        /// Property holds UserInfo of interaction stream
        /// </summary>
        private UserInfo[] _UserInfos = null;
        public UserInfo[] UserInfos
        {
            get
            {
                return _UserInfos;
            }
            set
            {
                _UserInfos = value;
                RaisePropertyChangedEvent("UserInfos");
            }
        }

        /// <summary>
        /// Property of left hand
        /// </summary>
        private Hand _LeftHand = new Hand { Type = HandType.Left};
        public Hand LeftHand
        {
            get
            {
                return _LeftHand;
            }
        }

        /// <summary>
        /// Property of right hand
        /// </summary>
        private Hand _RightHand = new Hand { Type = HandType.Right };
        public Hand RightHand
        {
            get
            {
                return _RightHand;
            }
        }

        /// <summary>
        /// Current dectected Gesture
        /// </summary>
        private GestureType _DectectedGesture;
        public GestureType DectectedGesture
        {
            get 
            {
                return this._DectectedGesture;
            }
            set
            {
                _DectectedGesture = value;
                RaisePropertyChangedEvent("DectectedGesture");
            }
        }

        /// <summary>
        /// Current dected Gesture Name
        /// </summary>
        private string _Gesture;
        public string Gesture
        {
            get
            {
                return _Gesture;
            }
            set
            {
                if (_Gesture == value)
                    return;
                _Gesture = value;
                RaisePropertyChangedEvent("Gesture");
            }
        }

        #endregion

        /// <summary>
        /// Constructor of KinectInteractionHelper
        /// </summary>
        /// <param name="sensor">sensor instance to be used by the KinectInteractionHelper class</param>
        public KinectInteractionHelper()
        {            

        }
        
        /// <summary>
        /// Initialize KinectInteractionStream
        /// </summary>
        public void Init(KinectSensor sensor)
        {
            // initlize the interaction stream
            this._Sensor = sensor;
            _Skeletons = new Skeleton[0];
            _InteractionAdapter = new InteractionAdapter();
            _PrimarySkeleton = new Skeleton();

            // register the depth and skeleton stream event handler for interaction stream 
            _Sensor.DepthFrameReady += Sensor_DepthFrameReady;
            _Sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;
            if (!_Sensor.DepthStream.IsEnabled)
            {
                _Sensor.DepthStream.Enable();
            }
            if (!_Sensor.SkeletonStream.IsEnabled)
            {
                _Sensor.SkeletonStream.Enable();
            }

            // Initizlie the interaction stream
            _InteractionStream = new InteractionStream(_Sensor, _InteractionAdapter);
            _UserInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];

            // Registered event handler
            _InteractionStream.InteractionFrameReady += InteractionStream_InteractionFrameReady;

            this._Enabled = true;
        }

        /// <summary>
        /// Enable Fizbin Gesture Controller
        /// </summary>
        public void EnableGestureController(bool enableHandInteraction)
        {
            // initialize Gesture Controller and Register FizBin RelativeGestureSegaments
            _GestureController = new GestureController();
            _GestureController.GestureRecognized += GestureController_GestureRecognized;

            // Check Normal Gestures
            this._EnableGestureRecognize = true;

            // Check Interact Gestures
            if (enableHandInteraction)
                this._EnableInteractGestureRecognize = true;            

            // add timer for clearing last detected gesture
            _ClearTimer = new Timer(2000);
            _ClearTimer.Elapsed += ClearTimer_Elapsed;

        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">>object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (_InteractionStream == null) return;
            
            
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;
                                
                // resize the skeletons array if needed
                if (_Skeletons.Length != skeletonFrame.SkeletonArrayLength)
                    _Skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                
                // get the skeleton data
                skeletonFrame.CopySkeletonDataTo(_Skeletons);

                // get the primary skeleton and update the Hand property for user to interact
                _PrimarySkeleton = KinectControllerUtil.GetPrimarySkeleton(_Skeletons);
                if (_PrimarySkeleton != null)
                {
                    var accelerometerReading = _Sensor.AccelerometerGetCurrentReading();
                    _InteractionStream.ProcessSkeleton(_Skeletons, accelerometerReading, skeletonFrame.Timestamp);
                    LeftHand.HandJoint = _PrimarySkeleton.Joints[JointType.HandLeft];
                    RightHand.HandJoint = _PrimarySkeleton.Joints[JointType.HandRight];

                    // update gesture controller
                    if (_EnableGestureRecognize)
                        _GestureController.UpdateAllGestures(_PrimarySkeleton);

                    // update interactaction gestures
                    if (_EnableInteractGestureRecognize)
                    {
                        Kinect.Helper.Gestures.HandState.LeftHandGrip = LeftHand.IsGrip;
                        Kinect.Helper.Gestures.HandState.RightHandGrip = RightHand.IsGrip;
                        _GestureController.UpdateAllGestures(_PrimarySkeleton);
                    }
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
            // Even though we un-register all our event handlers when the sensor
            // changes, there may still be an event for the old sensor in the queue
            // due to the way the KinectSensor delivers events.  So check again here.
            if (this._Sensor != sender)
            {
                return;
            }

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (null != depthFrame)
                {
                    try
                    {
                        // Hand data to Interaction framework to be processed.
                        this._InteractionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
                    }
                    catch (InvalidOperationException)
                    {
                        // DepthFrame functions may throw when the sensor gets
                        // into a bad state.  Ignore the frame in that case.
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's InteractionFrameReady event
        /// </summary>
        /// <param name="sender">>object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void InteractionStream_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            if (_InteractionStream == null) return;

            // Get the userinfo from the Interaction Frame
            using (InteractionFrame interactionFrame = e.OpenInteractionFrame())
            {
                if (interactionFrame != null)
                {
                    interactionFrame.CopyInteractionDataTo(UserInfos);
                    RaisePropertyChangedEvent("UserInfos");
                }
                else
                {
                    return;
                }
            }

            // Processing the Hand interaction infomation
            foreach (UserInfo userInfo in _UserInfos)
            {
                // get the hand interaction information
                foreach (InteractionHandPointer interactionHandPointer in userInfo.HandPointers)
                {                    
                    // check the interaction type of Left Hand and Right Hand
                    if (interactionHandPointer.HandType == InteractionHandType.Left)
                    {
                        // Left hand interaction information
                        switch (interactionHandPointer.HandEventType)
                        { 
                            case InteractionHandEventType.Grip:
                                LeftHand.IsGrip = true;
                                LeftHand.EventType = HandEventType.Grip;
                                break;
                            case InteractionHandEventType.GripRelease:
                                LeftHand.IsGrip = false;
                                LeftHand.EventType = HandEventType.GripRelease;
                                break;
                        }
                        // Get the left hand position if the Joint is tracked
                        if (LeftHand.HandJoint.TrackingState == JointTrackingState.Tracked)
                        {                            
                            LeftHand.X = LeftHand.HandJoint.Position.X;
                            LeftHand.Y = LeftHand.HandJoint.Position.Y;
                        }

                    }
                    else if (interactionHandPointer.HandType == InteractionHandType.Right)
                    {
                        // Right hand interaction information
                        // Left hand interaction information
                        switch (interactionHandPointer.HandEventType)
                        {
                            case InteractionHandEventType.Grip:
                                RightHand.IsGrip = true;
                                RightHand.EventType = HandEventType.Grip;
                                break;
                            case InteractionHandEventType.GripRelease:
                                RightHand.IsGrip = false;
                                RightHand.EventType = HandEventType.GripRelease;
                                break;
                        }
                        // Get the Right hand position if the Joint is tracked
                        if (RightHand.HandJoint.TrackingState == JointTrackingState.Tracked)
                        {
                            RightHand.X = RightHand.HandJoint.Position.X;
                            RightHand.Y = RightHand.HandJoint.Position.Y;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Helper function to register all available 
        /// </summary>
        public void RegisterGestures(GestureType type)
        {
            if (type == GestureType.All)
            {
                foreach (GestureType gesture in Enum.GetValues(typeof(GestureType)))
                {
                    _GestureController.AddGesture(gesture);
                }
            }
            else
            {
                _GestureController.AddGesture(type);
            }
        }      

        /// <summary>
        /// Event handler for Fizbin Gesture Recognition Pack
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void GestureController_GestureRecognized(object sender, GestureEventArgs e)
        {

            this.DectectedGesture = e.Type;

            // Set active Gesture Name
            switch (e.GestureName)
            {
                case "Menu":
                    Gesture = "Menu";
                    break;
                case "WaveRight":
                    Gesture = "Wave Right";
                    break;
                case "WaveLeft":
                    Gesture = "Wave Left";
                    break;
                case "JoinedHands":
                    Gesture = "Joined Hands";
                    break;
                case "SwipeLeft":
                    Gesture = "Swipe Left";
                    break;
                case "SwipeRight":
                    Gesture = "Swipe Right";
                    break;
                case "SwipeUp":
                    Gesture = "Swipe Up";
                    break;
                case "SwipeDown":
                    Gesture = "Swipe Down";
                    break;
                case "ZoomIn":
                    Gesture = "Zoom In";
                    break;
                case "ZoomOut":
                    Gesture = "Zoom Out";
                    break;
                case "GripZoomIn":
                    Gesture = "Grip Zoom In";
                    break;
                case "GripZoomOut":
                    Gesture = "Grip Zoom Out";
                    break;
                case "GripSwipeLeft":
                    Gesture = "Grip Swipe Left";
                    break;
                case "GripSwipeRight":
                    Gesture = "Grip Swipe Right";
                    break;
                default:
                    break;
            }

            // start the timer
            _ClearTimer.Start();
        }

        /// <summary>
        /// Clear the gesture for 2 seconds to enable user check different gestures
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">Event arguments</param>
        private void ClearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Gesture = "None";
            DectectedGesture = GestureType.None;
            _ClearTimer.Stop();
        }

    }
}
