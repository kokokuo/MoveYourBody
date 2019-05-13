using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNI;

namespace KinectModel.BehaviorStruct
{
    /// <summary>
    /// 此為記錄左右移動的資料結構
    /// </summary>
    public struct MoveBehaviorData
    {
       
        private Point3D  currentPosition;
        private int xAxiesMaxBorder;
        private int xAxiesMinBorder;
        private int centerPosition;

        /// <summary>
        /// 投影後的資訊,使用者目前的位置
        /// </summary>
        public Point3D CurrentUserPosition
        { get { return currentPosition; } }

        /// <summary>
        /// 移動時的畫面最大的邊界
        /// </summary>
        public int XAxiesMaxBorder
        { get { return xAxiesMaxBorder; } }
        /// <summary>
        /// 移動時的畫面最小的邊界
        /// </summary>
        public int XAxiesMinBorder
        { get { return xAxiesMinBorder; } }

        /// <summary>
        /// 畫面的中心
        /// </summary>
        public int CenterPosition
        { get { return centerPosition; } }
        /// <summary>
        /// 記錄移動的資料結構
        /// </summary>
        /// <param name="deviceXAxiesMinBorder"></param>
        /// <param name="deviceXAxiesMaxBorder"></param>
        /// <param name="center"></param>
        /// <param name="torsoPosition"></param>
        public MoveBehaviorData(int deviceXAxiesMinBorder, int deviceXAxiesMaxBorder,int center, Point3D torsoPosition)
        {
            xAxiesMaxBorder = deviceXAxiesMaxBorder;
            xAxiesMinBorder = deviceXAxiesMinBorder;
            centerPosition = center;
            currentPosition = torsoPosition;
        }
    }
}
