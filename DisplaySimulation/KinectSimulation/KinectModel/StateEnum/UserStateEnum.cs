using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectModel.StateEnum
{
    /// <summary>
    /// 包凡使用者在畫面中所有可能的狀態
    /// </summary>
    public enum UserStateEnum
    {
        /// <summary>
        /// 偵測校正
        /// </summary>
        Calibrating = 0,
        /// <summary>
        /// 尋找手勢
        /// </summary>
        LookingForPose,
        /// <summary>
        /// 追蹤骨架
        /// </summary>
        Tracking,
        /// <summary>
        /// 追蹤骨架但是處在警告範圍區
        /// </summary>
        TrackingButWaring,
        /// <summary>
        /// 進入重新校正
        /// </summary>
        ReTracking,
        /// <summary>
        /// 停止骨架追蹤
        /// </summary>
        StopTracking,
        /// <summary>
        /// 增加新的使用者
        /// </summary>
        NewUser,
        /// <summary>
        /// 遺失使用者
        /// </summary>
        LostUser,
        /// <summary>
        /// 使用者再次進入畫面
        /// </summary>
        UserReEnter,
        /// <summary>
        /// 無骨架的使用者離開畫面
        /// </summary>
        UserExit,
        /// <summary>
        /// 有骨架的使用者離開畫面
        /// </summary>
        JointUserExit,
        /// <summary>
        /// 有骨架的使用者遺失
        /// </summary>
        JointUserMissed,
        /// <summary>
        /// 有骨架的使用者不見
        /// </summary>
        JointUserLost,
    }
}
