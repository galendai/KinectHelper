using Microsoft.Kinect;
using System.Diagnostics;

namespace Kinect.Helper.Gestures.InteractSegments
{
    /// <summary>
    /// The first part of the swipe left gesture
    /// </summary>
    public class GripSwipeRightSegment1 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // if right hand is grip
            if (HandState.LeftHandGrip)
            {
                // left hand in front of left Shoulder
                if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    // left hand left of left Shoulder
                    if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderLeft].Position.X)
                    {
                        return GesturePartResult.Succeed;
                    }
                    return GesturePartResult.Pausing;
                }
            }
            return GesturePartResult.Fail;
        }

    }

    /// <summary>
    /// The second part of the swipe right gesture
    /// </summary>
    public class GripSwipeRightSegment2 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // if Right hand is grip
            if (HandState.LeftHandGrip)
            {
                // //left hand in front of left Shoulder
                if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    // left hand left of left Shoulder
                    if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X && skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X)
                    {
                        return GesturePartResult.Succeed;
                    }

                    return GesturePartResult.Pausing;
                }
                return GesturePartResult.Fail;
            }
            return GesturePartResult.Fail;
        }

    }

}
