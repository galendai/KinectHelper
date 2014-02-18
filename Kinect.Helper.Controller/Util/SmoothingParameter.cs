using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Kinect.Helper.Controller.Util
{
    public static class SmoothingParameter
    {
        public static TransformSmoothParameters Default
        { 
            get
            {
                return new TransformSmoothParameters() 
                { 
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.05f
                };
            }
        }
        public static TransformSmoothParameters Smooth
        {
            get
            {
                return new TransformSmoothParameters() 
                { 
                    Smoothing = 0.5f,
                    Correction = 0.1f,
                    Prediction = 0.5f,
                    JitterRadius = 0.1f,
                    MaxDeviationRadius = 0.1f
                
                };
            }
        }

        public static TransformSmoothParameters VerySmooth
        {
            get
            {
                return new TransformSmoothParameters()
                {
                    Smoothing = 0.7f,
                    Correction = 0.3f,
                    Prediction = 1.0f,
                    JitterRadius = 1.0f,
                    MaxDeviationRadius = 1.0f
                };
            }
        
        }

    }
}
