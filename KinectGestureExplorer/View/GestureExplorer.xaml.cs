using Kinect.Helper.Controller;
using System.Windows;

namespace KinectGestureExplorer.View
{
    /// <summary>
    /// Description for GestureExplorer.
    /// </summary>
    public partial class GestureExplorer : Window
    {
        // instance of KinectManager and KinectInteractionHelper
        private KinectManager _KinectManager;
        private KinectInteractionHelper _KinectInteractionHelper;
        
        /// <summary>
        /// Initializes a new instance of the GestureExplorer class.
        /// </summary>
        public GestureExplorer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // init the kinect interaction helper
            _KinectManager = KinectManager.Instance;
            _KinectInteractionHelper = KinectInteractionHelper.Instance;

            // set the container of the Interaction Helper
            if (_KinectInteractionHelper.Enabled)
            {
                _KinectInteractionHelper.Container = this.RootCanvas;
            }
        }
    }
}