using Kinect.Helper.Gestures.InteractSegments;
using Kinect.Helper.Gestures.Segments;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kinect.Helper.Gestures
{
    public class GestureController
    {
        /// <summary>
        /// The list of all gestures we are currently looking for
        /// </summary>
        private List<Gesture> gestures = new List<Gesture>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureController"/> class.
        /// </summary>
        public GestureController()
        {
        }

        /// <summary>
        /// Occurs when [gesture recognised].
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureRecognized;

        /// <summary>
        /// Updates all gestures.
        /// </summary>
        /// <param name="data">The skeleton data.</param>
        public void UpdateAllGestures(Skeleton data)
        {
            foreach (Gesture gesture in this.gestures)
            {
                gesture.UpdateGesture(data);
            }
        }

        public void AddGesture(GestureType gestureType)
        {
            IRelativeGestureSegment[] gestureDefinition = null;

            // TODO: Add all your predefined gestures here. 
            switch (gestureType)
            {
                case GestureType.Menu:
                    gestureDefinition = new IRelativeGestureSegment[20];
                    MenuSegment1 menuSegment = new MenuSegment1();
                    for (int i = 0; i < 20; i++)
                    {
                        // gesture consists of the same thing 20 times 
                        gestureDefinition[i] = menuSegment;
                    }
                    break;
                case GestureType.WaveRight:
                    gestureDefinition = new IRelativeGestureSegment[6];
                    WaveRightSegment1 waveRightSegment1 = new WaveRightSegment1();
                    WaveRightSegment2 waveRightSegment2 = new WaveRightSegment2();
                    gestureDefinition[0] = waveRightSegment1;
                    gestureDefinition[1] = waveRightSegment2;
                    gestureDefinition[2] = waveRightSegment1;
                    gestureDefinition[3] = waveRightSegment2;
                    gestureDefinition[4] = waveRightSegment1;
                    gestureDefinition[5] = waveRightSegment2;
                    break;
                case GestureType.WaveLeft:
                    gestureDefinition = new IRelativeGestureSegment[6];
                    WaveLeftSegment1 waveLeftSegment1 = new WaveLeftSegment1();
                    WaveLeftSegment2 waveLeftSegment2 = new WaveLeftSegment2();
                    gestureDefinition[0] = waveLeftSegment1;
                    gestureDefinition[1] = waveLeftSegment2;
                    gestureDefinition[2] = waveLeftSegment1;
                    gestureDefinition[3] = waveLeftSegment2;
                    gestureDefinition[4] = waveLeftSegment1;
                    gestureDefinition[5] = waveLeftSegment2;
                    break;
                case GestureType.JoinedHands:
                    gestureDefinition = new IRelativeGestureSegment[20];
                    JoinedHandsSegment1 joinedhandsSegment = new JoinedHandsSegment1();
                    for (int i = 0; i < 20; i++)
                    {
                        // gesture consists of the same thing 10 times 
                        gestureDefinition[i] = joinedhandsSegment;
                    }
                    break;
                case GestureType.SwipeLeft:
                    gestureDefinition = new IRelativeGestureSegment[3];
                    gestureDefinition[0] = new SwipeLeftSegment1();
                    gestureDefinition[1] = new SwipeLeftSegment2();
                    gestureDefinition[2] = new SwipeLeftSegment3();
                    break;
                case GestureType.SwipeRight:
                    gestureDefinition = new IRelativeGestureSegment[3];
                    gestureDefinition[0] = new SwipeRightSegment1();
                    gestureDefinition[1] = new SwipeRightSegment2();
                    gestureDefinition[2] = new SwipeRightSegment3();
                    break;
                case GestureType.SwipeUp:
                    gestureDefinition = new IRelativeGestureSegment[3];
                    gestureDefinition[0] = new SwipeUpSegment1();
                    gestureDefinition[1] = new SwipeUpSegment2();
                    gestureDefinition[2] = new SwipeUpSegment3();
                    break;
                case GestureType.SwipeDown:
                    gestureDefinition = new IRelativeGestureSegment[3];
                    gestureDefinition[0] = new SwipeDownSegment1();
                    gestureDefinition[1] = new SwipeDownSegment2();
                    gestureDefinition[2] = new SwipeDownSegment3();
                    break;
                case GestureType.ZoomIn:
                    gestureDefinition = new IRelativeGestureSegment[3];
                    gestureDefinition[0] = new ZoomSegment1();
                    gestureDefinition[1] = new ZoomSegment2();
                    gestureDefinition[2] = new ZoomSegment3();
                    break;
                case GestureType.ZoomOut:
                    gestureDefinition = new IRelativeGestureSegment[3];
                    gestureDefinition[0] = new ZoomSegment3();
                    gestureDefinition[1] = new ZoomSegment2();
                    gestureDefinition[2] = new ZoomSegment1();
                    break;
                case GestureType.GripSwipeLeft:
                    gestureDefinition = new IRelativeGestureSegment[2];
                    gestureDefinition[0] = new GripSwipeLeftSegment1();
                    gestureDefinition[1] = new GripSwipeLeftSegment2();
                    break;
                case GestureType.GripSwipeRight:
                    gestureDefinition = new IRelativeGestureSegment[2];
                    gestureDefinition[0] = new GripSwipeRightSegment1();
                    gestureDefinition[1] = new GripSwipeRightSegment2();
                    break;
                case GestureType.GripZoomIn:
                    gestureDefinition = new IRelativeGestureSegment[3];
                    gestureDefinition[0] = new GripZoomSegment1();
                    gestureDefinition[1] = new GripZoomSegment2();
                    gestureDefinition[2] = new GripZoomSegment3();
                    break;
                case GestureType.GripZoomOut:
                    gestureDefinition = new IRelativeGestureSegment[3];
                    gestureDefinition[0] = new GripZoomSegment3();
                    gestureDefinition[1] = new GripZoomSegment2();
                    gestureDefinition[2] = new GripZoomSegment1();
                    break;
                case GestureType.All:
                    break;
                case GestureType.None:
                    break;
                default:
                    break;
            }

            // regestier gestures
            if (gestureType != GestureType.All && gestureType != GestureType.None)
            {
                Gesture gesture = new Gesture(gestureType, gestureDefinition);
                gesture.GestureRecognized += OnGestureRecognized;
                this.gestures.Add(gesture);
            }
        }

        /// <summary>
        /// Adds the gesture.
        /// </summary>
        /// <param name="name">The gesture type.</param>
        /// <param name="gestureDefinition">The gesture definition.</param>
        public void AddGesture(string name, IRelativeGestureSegment[] gestureDefinition)
        {
            Gesture gesture = new Gesture(name, gestureDefinition);
            gesture.GestureRecognized += OnGestureRecognized;
            this.gestures.Add(gesture);
        }

        /// <summary>
        /// Handles the GestureRecognized event of the g control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KinectSkeltonTracker.GestureEventArgs"/> instance containing the event data.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            if (this.GestureRecognized != null)
            {
                this.GestureRecognized(this, e);
            }

            foreach (Gesture g in this.gestures)
            {
                g.Reset();
            }
        }
    }
}
