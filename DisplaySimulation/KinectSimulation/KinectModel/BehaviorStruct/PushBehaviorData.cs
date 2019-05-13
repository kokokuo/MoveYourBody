using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNI;
namespace KinectModel.BehaviorStruct
{
    /// <summary>
    /// 紀錄Push的資料結構
    /// </summary>
    public struct PushBehaviorData
    {
        private string hand;
        private double zOriVelocity;
        private Point3D realpoint;
        private double pushAngle;

        /// <summary>
        /// 分成"LeftHand"和"RightHand"
        /// </summary>
        public string Hand
        { get { return hand; } }
        /// <summary>
        /// 速度應為negative
        /// </summary>
        public double ZOriVelocity
        { get { return zOriVelocity; } }
        /// <summary>
        /// Push時手的位置(三維座標)
        /// </summary>
        public Point3D RealHandPoint
        { get { return realpoint; } }
        /// <summary>
        /// Push時手的角度
        /// </summary>
        public double PushAngle
        { get { return pushAngle; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="whichHand">左手(LeftHand)或右手(RightHand)</param>
        /// <param name="velocity">Z軸方向的速度(應為負值)</param>
        /// <param name="angle">Push時的角度</param>
        /// <param name="realpos">Push時手的位置(三維座標)</param>
        public PushBehaviorData(string whichHand, double velocity, double angle,Point3D realpos) 
        {
            hand = whichHand;
            zOriVelocity = velocity;
            pushAngle = angle;
            realpoint = realpos;
        }
    }
}
