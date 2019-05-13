using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNI;
using KinectModel.StateEnum;

namespace KinectModel.BehaviorStruct
{
    /// <summary>
    /// 紀錄揮砍的資料結構
    /// </summary>
    public struct SlashBehaviorData
    {
        private SlashBehaviorEnum slashType;
        private double xOriVelocity;
        private double yOriVelocity;
        private double zOriVelocity;
        private LinkedList<SkeletonJointPosition> realpoints;

        /// <summary>
        /// 砍的動作類型,參考SlashBehaviorEnum列舉
        /// </summary>
        public SlashBehaviorEnum SlashActionType
        { get { return slashType; } }
        /// <summary>
        /// x方向的瞬間速度(有向值:正值表向右砍,反之向左砍)
        /// </summary>
        public double XOriVelocity
        { get { return xOriVelocity; } }
        /// <summary>
        /// y方向的瞬間速度(有向值:正值表向下砍,反之向上砍)
        /// </summary>
        public double YOriVelocity
        { get { return yOriVelocity; } }
        /// <summary>
        /// z方向的瞬間速度(有向值:負值表向前砍)
        /// </summary>
        public double ZOriVelocity
        { get { return zOriVelocity; } }
        /// <summary>
        /// 訊號觸發時,紀錄的所有的手座標點(三維座標)
        /// </summary>
        public LinkedList<SkeletonJointPosition> RealPointRecorder 
        { get { return realpoints; } }
        /// <summary>
        ///  紀錄右手揮砍的資料
        /// </summary>
        /// <param name="slashActionType">SlashBehaviorEnum列舉中的某一個揮砍動作類型</param>
        /// <param name="xv">x方向的瞬間速度</param>
        /// <param name="yv">y方向的瞬間速度</param>
        /// <param name="zv">z方向的瞬間速度</param>
        /// <param name="realpos">訊號觸發時,紀錄的所有的手座標點</param>
        public SlashBehaviorData(SlashBehaviorEnum slashActionType,double xv, double yv, double zv, LinkedList<SkeletonJointPosition> realpos) 
        {
            slashType = slashActionType;
            xOriVelocity = xv;
            yOriVelocity = yv;
            zOriVelocity = zv;
            realpoints = realpos;
        }
    }
}
