using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNI;
namespace KinectModel.BehaviorStruct
{
    /// <summary>
    /// 飛行行為會有一個範圍值判斷,此資料結構記錄範圍的核心及臨界值
    /// </summary>
    public struct FlyRangeBoxData
    {
        private Point3D distributionCenter;
        private double yLowerThreshold;
        private double yHeigherThreshold;
        private double xRightThreshold;
        private double xLeftThreshold;
        /// <summary>
        /// 核心點
        /// </summary>
        public Point3D DistributionCenter
        { get { return distributionCenter; } }
        /// <summary>
        /// Y軸低點臨界值
        /// </summary>
        public double YLowerThreshold
        { get { return yLowerThreshold; } }
        /// <summary>
        /// Y軸高點臨界值
        /// </summary>
        public double YHeigherThreshold
        { get { return yHeigherThreshold; } }
        /// <summary>
        /// X軸臨界值,為了避免右手跑離右手區
        /// </summary>
        public double XRightThreshold
        { get { return xRightThreshold; } }
        /// <summary>
        /// X軸臨界值,為了避免右左手跑離左手區
        /// </summary>
        public double XLeftThreshold
        { get { return xLeftThreshold; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center">核心點</param>
        /// <param name="yLower">臨界值Y軸低點</param>
        /// <param name="yHeigher">臨界值Y軸高點</param>
        /// <param name="xRight">右手X座標點臨界值</param>
        /// <param name="xLeft">左手X座標點臨界值</param>
        public FlyRangeBoxData(Point3D center, double yLower, double yHeigher, double xRight, double xLeft)
        { 
            distributionCenter = center;
            yLowerThreshold = yLower;
            yHeigherThreshold = yHeigher;
            xRightThreshold = xRight;
            xLeftThreshold = xLeft;
        }
    }
}
