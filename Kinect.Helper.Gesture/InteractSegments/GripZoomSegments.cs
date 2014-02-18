using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Helper.Gestures.InteractSegments
{
    public class GripZoomSegment1 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // Check whether both left and right hand gripped
            if (HandState.LeftHandGrip && HandState.RightHandGrip)
            {
                // Right and Left Hand in front of Shoulders
                if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z &&
                    skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ElbowRight].Position.Z)
                {
                    // Hands between shoulders
                    if (skeleton.Joints[JointType.HandRight].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X &&
                        skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X &&
                        skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X &&
                        skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X)
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

    public class GripZoomSegment2 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // Check whether both left and right hand gripped
            if (HandState.LeftHandGrip && HandState.RightHandGrip)
            {
                // Right and Left Hand in front of Shoulders
                if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z &&
                    skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ElbowRight].Position.Z)
                {
                    // Hands outside shoulders
                    if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X && skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderLeft].Position.X)
                    {
                        return GesturePartResult.Succeed;
                    }
                    return GesturePartResult.Pausing;
                }
            }
            return GesturePartResult.Fail; 
        }

    }

    public class GripZoomSegment3 : IRelativeGestureSegment
    {
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // Check whether both left and right hand gripped
            if (HandState.LeftHandGrip && HandState.RightHandGrip)
            {
                // Right and Left Hand in front of Shoulders
                if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z &&
                    skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ElbowRight].Position.Z)
                {
                    // Hands outside elbows
                    if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ElbowRight].Position.X
                        && skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ElbowLeft].Position.X)
                    {
                        return GesturePartResult.Succeed;
                    }
                    return GesturePartResult.Pausing;
                }
            }

            return GesturePartResult.Fail; 
        }

    }
 

}
