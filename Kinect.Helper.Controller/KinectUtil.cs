using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kinect.Helper.Controller
{
    public static class KinectControllerUtil
    {
        /// <summary>
        /// get the skeleton closest to the Kinect sensor
        /// </summary>
        /// <param name="skeletons">all avaliable skeletons</param>
        /// <returns></returns>
        public static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }
            return skeleton;
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point with default depth scale (640*480)
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="skelpoint"></param>
        /// <returns></returns>
        public static Point SkeletonPointToScreen(KinectSensor sensor, SkeletonPoint skelpoint)
        {
            return SkeletonPointToScreen(sensor, skelpoint, 1);
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="sensor">Instance of Kinect Sensor</param>
        /// <param name="skelpoint">point to map</param>
        /// <param name="scale">Scaling the pointer</param>
        /// <returns>mapped point</returns>
        public static Point SkeletonPointToScreen(KinectSensor sensor, SkeletonPoint skelpoint, double scale)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X * scale, depthPoint.Y * scale);
        }

        /// <summary>
        /// Returns the 2D position of the provided 3D SkeletonPoint.
        /// The result will be in in either Color coordinate space or Depth coordinate space, depending on 
        /// the current value of this.ImageType.
        /// Only those parameters associated with the current ImageType will be used.
        /// </summary>
        /// <param name="sensor">The KinectSensor for which this mapping is being performed.</param>
        /// <param name="renderSize">The target dimensions of the visualization</param>
        /// <param name="skeletonPoint">The source point to map</param>
        /// <param name="depthFormat">The format of the target depth image, if the imageType is Depth</param>
        /// <param name="depthWidth">The width of the target depth image, if the imageType is Depth</param>
        /// <param name="depthHeight">The height of the target depth image, if the imageType is Depth</param>
        /// <returns>Returns the 2D position of the provided 3D SkeletonPoint.</returns>
        public static Point GetDepth2DPosition(
            KinectSensor sensor,
            Size renderSize,
            SkeletonPoint skeletonPoint,
            DepthImageFormat depthFormat,
            int depthWidth,
            int depthHeight)
        {
            try
            {
                if (DepthImageFormat.Undefined != depthFormat)
                {
                    var depthPoint = sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, depthFormat);

                    return new Point(
                        (int)(renderSize.Width * depthPoint.X / depthWidth),
                        (int)(renderSize.Height * depthPoint.Y / depthHeight));
                }
            }
            catch (InvalidOperationException)
            {
                // The stream must have stopped abruptly
                // Handle this gracefully
            }

            return new Point();
        }

        /// <summary>
        /// Returns the 2D position of the provided 3D SkeletonPoint.
        /// The result will be in in either Color coordinate space or Depth coordinate space, depending on 
        /// the current value of this.ImageType.
        /// Only those parameters associated with the current ImageType will be used.
        /// </summary>
        /// <param name="sensor">The KinectSensor for which this mapping is being performed.</param>
        /// <param name="renderSize">The target dimensions of the visualization</param>
        /// <param name="skeletonPoint">The source point to map</param>
        /// <param name="colorFormat">The format of the target color image, if imageType is Color</param>
        /// <param name="colorWidth">The width of the target color image, if the imageType is Color</param>
        /// <param name="colorHeight">The height of the target color image, if the imageType is Color</param>
        /// <returns>Returns the 2D position of the provided 3D SkeletonPoint.</returns>
        public static Point GetColor2DPosition(
            KinectSensor sensor,
            Size renderSize,
            SkeletonPoint skeletonPoint,
            ColorImageFormat colorFormat,
            int colorWidth,
            int colorHeight)
        {
            try
            {
                if (ColorImageFormat.Undefined != colorFormat)
                {
                    var colorPoint = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, colorFormat);

                    // map back to skeleton.Width & skeleton.Height
                    return new Point(
                        (int)(renderSize.Width * colorPoint.X / colorWidth),
                        (int)(renderSize.Height * colorPoint.Y / colorHeight));
                }

            }
            catch (Exception)
            {
                // The stream must have stopped abruptly
                // Handle this gracefully
            }

            return new Point();
        }
    }
}
