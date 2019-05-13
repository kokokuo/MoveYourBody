using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectModel.StateEnum;
using OpenNI;

namespace KinectModel.BehaviorStruct
{
    public struct FlyBehaviorData
    {
        private double yOriDisplacement;
        private FlyBehaviorActionEnum whichOrientaionFly;
        private double leftAngle;
        private double rightAngle;
        private double zOriDifference;
        /// <summary>
        /// means the fly action is correct or not (the value should be positive because you should raise hand)
        /// </summary>
        private double yOriVectorOfLeftHandAndLeftElbow;
        /// <summary>
        ///  means the fly action is correct or not (the value should be positive because you should raise hand)
        /// </summary>
        private double yOriVectorOfRightHandAndRightElbow;
        /// <summary>
        /// lefthand and righthand difference of z orientation
        /// </summary>
        public double ZOriDifference
        { get { return zOriDifference; } }
        /// <summary>
        /// the value should be negative because yhe fly action is pull your hand down
        /// </summary>
        public double YOriDisplacement
        { get { return yOriDisplacement; } }
        /// <summary>
        /// string message,tell you current fly orientaion, include "FlyLeft","FlyRight","FlyDown"
        /// </summary>
        public FlyBehaviorActionEnum OrientaionFly
        { get { return whichOrientaionFly; } }
        /// <summary>
        /// Angle combined by leftElbow to leftHand and leftElbow to leftShoulder 
        /// </summary>
        public double LeftAngle
        { get { return leftAngle; } }
        /// <summary>
        /// Angle combined by rightElbow to rightHand and rightElbow to rightShoulder
        /// </summary>
        public double RightAngle
        { get { return rightAngle; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lAngle">Angle combined by leftElbow to leftHand and leftElbow to leftShoulder </param>
        /// <param name="rAngle">Angle combined by rightElbow to rightHand and rightElbow to rightShoulder</param>
        /// <param name="leftHandAndLeftElboVector">the value should be positive</param>
        /// <param name="rightHandAndRightElbowVector">the value should be positive</param>
        /// <param name="handsDifference">lefthand and righthand difference of z orientation</param>
        /// <param name="d">the value should be negative</param>
        /// <param name="whichFly">message</param>
        public FlyBehaviorData(double lAngle, double rAngle, double leftHandAndLeftElboVector, double rightHandAndRightElbowVector, double handsDifference, double d, FlyBehaviorActionEnum whichFly)
        {
            leftAngle = lAngle;
            rightAngle = rAngle;
            yOriVectorOfLeftHandAndLeftElbow = leftHandAndLeftElboVector;
            yOriVectorOfRightHandAndRightElbow = rightHandAndRightElbowVector;
            zOriDifference = handsDifference;
            yOriDisplacement = d;
            whichOrientaionFly = whichFly;
        }
    }
    
    
}
