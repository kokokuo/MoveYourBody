using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectModel.StateEnum;
namespace KinectModel.BehaviorStruct
{
    public struct SwipeBehaviorData
    {
        private SwipeBehaviorActionEnum whichOri;
        private double xOriVelocity;
        private double yOriVelocity;
        private double xOriDisplacement;
        private double yOriDisplacement;
        private double angleBetweenVerticalOrientation;
        private double angleBetweenHorizonOrientation;

        public SwipeBehaviorActionEnum SwipeOrientation
        { get { return whichOri; } }
        
        /// <summary>
        /// X方向的瞬間速度(有向值)
        /// </summary>
        public double XOrientationVelocity
        { get { return xOriVelocity; } }
        
        /// <summary>
        /// Y放向的瞬間速度(有向值)
        /// </summary>
        public double YOrientationVelocity
        { get { return yOriVelocity; } }
        
        /// <summary>
        /// X方向的位移量
        /// </summary>
        public double XOrientationDisplacement
        { get { return xOriDisplacement; } }
        
        /// <summary>
        /// Y方向的位移量
        /// </summary>
        public double YOrientationDisplacement
        { get { return yOriDisplacement; } }

        /// <summary>
        /// Swipe時和垂直方向向量的夾角
        /// </summary>
        public double AngleBetweenVerticalOrientation
        { get { return angleBetweenVerticalOrientation; } }
        /// <summary>
        /// Swipe時和水平方向向量的夾角(若是up/down則代表和Y方向的夾角,若是right/left則代表X方向的夾角)
        /// </summary>
        public double AngleBetweenHorizonOrientation
        { get { return angleBetweenHorizonOrientation; } }

        public SwipeBehaviorData(SwipeBehaviorActionEnum swipeOri, double xOriV, double yOriV, double xOriD, double yOriD, double angleBetweenVertical, double angleBetweenHorizon)
        {
            whichOri = swipeOri;
            xOriVelocity = xOriV;
            yOriVelocity = yOriV;
            xOriDisplacement = xOriD;
            yOriDisplacement = yOriD;
            angleBetweenHorizonOrientation = angleBetweenHorizon;
            angleBetweenVerticalOrientation = angleBetweenVertical;
        }
    }
   

   
}
