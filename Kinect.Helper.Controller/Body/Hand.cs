using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kinect.Helper.Controller.Common;
using System.Windows;
using Microsoft.Kinect;

namespace Kinect.Helper.Controller.Body
{
    public class Hand : ObservableObject
    {
        public HandEventType EventType { get; set; }
        public HandType Type { get; set; }

        private Joint _HandJoint;
        public Joint HandJoint
        {
            get
            {
                return _HandJoint;
            }
            set
            {
                _HandJoint = value;
                RaisePropertyChangedEvent("HandJoint");
            }
        }

        private double _X = 0.0;
        public double X
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
                RaisePropertyChangedEvent("X");
            }
        }

        private double _Y = 0.0;
        public double Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
                RaisePropertyChangedEvent("Y");
            }
        }

        private bool _IsGrip = false;
        public bool IsGrip
        {
            get
            {
                return _IsGrip;
            }
            set
            {
                if (_IsGrip != value)
                {
                    _IsGrip = value;
                    RaisePropertyChangedEvent("IsGrip");
                }
            }
        }

    }
}
