using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using OpenNI;
namespace KinectModel.BehaviorStruct
{
    public struct JumpBehaviorData
    {
        private double yOriVelocity;
        private double yOriDisplacement;
        private LinkedList<SkeletonJointPosition> realpoints;
        /// <summary>
        /// 向上跳時Y軸的瞬間速度(數值應為正)
        /// </summary>
        public double YOriVelocity 
        { get { return yOriVelocity; } }
        /// <summary>
        /// 向上跳,Y軸的位移量
        /// </summary>
        public double YOriDisplacement 
        { get { return yOriDisplacement; } }
        /// <summary>
        /// 跳躍時過程中所有的三維點
        /// </summary>
        public LinkedList<SkeletonJointPosition> RealPointRecorder
        { get { return realpoints; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="velocity">Y軸方向的瞬間速度(應為正值)</param>
        /// <param name="displacementY">Y軸方向的位移量</param>
        /// <param name="real">跳躍時過程中所有的三維點</param>
        public JumpBehaviorData(double velocity, double displacementY, LinkedList<SkeletonJointPosition> real)
        {
            yOriVelocity = velocity;
            yOriDisplacement = displacementY;
            realpoints = real;
        }
    }
}
