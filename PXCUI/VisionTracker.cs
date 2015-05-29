using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PXCUI
{
    class VisionTracker
    {



        internal bool TrackingTest(Rectangle VisionCenter, Rectangle TargetBoundingBox, out Point Offset)
        {

            Offset = new Point(0, 0);

            if (VisionCenter == null || TargetBoundingBox == null)
            {
                Console.WriteLine("Error :TrackingTest // VisionCenter == null || TargetBoundingBox == null");
                return false;
                //throw new ApplicationException("Error :TrackingTest // VisionCenter == null || TargetBoundingBox == null");
            }

            Point TargetBottomCenter = new Point((int)(TargetBoundingBox.X + TargetBoundingBox.Width / 2), TargetBoundingBox.Y + TargetBoundingBox.Height);
            
            int BoundW = VisionCenter.Width / 2;
            int BoundH = VisionCenter.Height / 2;
            Point Center = new Point(VisionCenter.X + BoundW , VisionCenter.Y + BoundH);

            if (VisionCenter.Contains(TargetBottomCenter))
            {
                Offset = new Point(0,0);
            }
            else
            {
                Offset = new Point(VisionCenter.X - Center.X, VisionCenter.Y - Center.Y);
            }

            return true;
        }

        /// <summary>距離</summary>
        internal int Distence(Rectangle BoundingBox ,Point TargetPosition,double VehicleHeight, double VehiclePitch, double CradleHeadPtich)
        {
            int distance = -1;
            if (BoundingBox.Contains(TargetPosition))
            {
                


            }




            return distance;
        }
    }
}
