using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Helper.Gestures
{
    public enum GestureType
    {
        /// <summary>
        /// XBox360 like menu gesture
        /// </summary>
        Menu,

        /// <summary>
        /// Wave right hand
        /// </summary>
        WaveRight,

        /// <summary>
        /// Wave left hand
        /// </summary>
        WaveLeft,

        /// <summary>
        /// Cross two hands together in front of chest
        /// </summary>
        JoinedHands,

        /// <summary>
        /// Swipe right hand from right side to left side
        /// </summary>
        SwipeLeft,

        /// <summary>
        /// Swipe left hand from left side to right side
        /// </summary>
        SwipeRight,

        /// <summary>
        /// Swipe hand up
        /// </summary>
        SwipeUp,

        /// <summary>
        /// Swipe hand down
        /// </summary>
        SwipeDown,

        /// <summary>
        /// Open two hands in front of chest
        /// </summary>
        ZoomIn,

        /// <summary>
        /// Close two hands in front of chest
        /// </summary>
        ZoomOut,

        /// <summary>
        /// Grip right hand, and swipe to left
        /// </summary>
        GripSwipeLeft,

        /// <summary>
        /// Grip left hand and swipe to right
        /// </summary>
        GripSwipeRight,

        /// <summary>
        /// Grip two hands, and extends two hands further out of chest
        /// </summary>
        GripZoomIn,

        /// <summary>
        /// Grip two hands and extends two hands further close to the center of chest
        /// </summary>
        GripZoomOut,

        /// <summary>
        /// All avaliable gestures
        /// </summary>
        All,

        /// <summary>
        /// None gesture dected
        /// </summary>
        None

    }
}
