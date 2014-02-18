using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect.Helper.Controller.Common
{
    public class InteractionAdapter : IInteractionClient
    {
        public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
        {
            var interactionInfo = new InteractionInfo
            {
                IsGripTarget = false,
                IsPressTarget = false
            };

            return interactionInfo;
        }
    }
}
