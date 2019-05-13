using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectModel.StateEnum
{
    /// <summary>
    /// 飛行訊號所有可能的錯誤狀態
    /// </summary>
    public enum FlyBehaviorStateMessageEnum
    {
        RightHand_Z_Point_Over_Shoulder = 0,
        LeftHand_Z_Point_Over_Shoulder,
        RightHand_UpLift_Over_High,
        LefttHand_UpLift_Over_High,
        BothtHands_UpLift_Over_High,
        RightHand_Position_Not_In_RightZone,
        LeftHand_Position_Not_In_LeftZone,
        RightHand_Not_UpLift,
        LeftHand_Not_UpLift,
        RightElbow_And_RightShoulder_Not_Vetical,
        LeftElbow_And_LeftShoulder_Not_Vetical,
        RightHand_And_LeftHand_Z_Axies_Difference_Over,
        Correct,
        Init,
        User_Z_Position_Over_TwoDotFour_Meter 
    }
}
