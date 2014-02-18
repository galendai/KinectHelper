using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Kinect.Helper.Controller;
using Kinect.Helper.Controller.Body;
using Kinect.Helper.Gestures;

using Microsoft.Kinect;

namespace KinectControllerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectManager _Kinect = KinectManager.Instance;
        private KinectInteractionHelper _InteractionHelper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // init Kinect Sensor
            _Kinect.InitializeAllKinectStreams();

            // Init Kinect interaction helper
            _InteractionHelper = KinectInteractionHelper.Instance;
            _InteractionHelper.Init(_Kinect.Sensor);

            // enable Gesture Controller
            if (_InteractionHelper.Enabled)
            {
                _InteractionHelper.EnableGestureController(true);
                _InteractionHelper.RegisterGestures(GestureType.All);
            }

            // start the kinect
            _Kinect.Start();

            _InteractionHelper.PropertyChanged += InteractionHelper_PropertyChanged;
        }


        private void InteractionHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            try
            {
                Hand LeftHand = _InteractionHelper.LeftHand;
                Hand RightHand = _InteractionHelper.RightHand;
                LeftHandInfo.Text = "Left Hand: X = " + LeftHand.X + "; Y = " + LeftHand.Y + "; IsGrip = " + LeftHand.IsGrip;
                RightHandInfo.Text = "Right Hand: X = " + RightHand.X + "; Y = " + RightHand.Y + "; IsGrip = " + RightHand.IsGrip;

                GestureName.Text = "Gesture: " + _InteractionHelper.Gesture;

            }
            catch(InvalidOperationException)
            {
                /// TO-DO: Temporarily catch invalidate operation caused by GestureController
            }

        }
        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _Kinect.Closing = true;
            _Kinect.Stop();
        }

    }
}
