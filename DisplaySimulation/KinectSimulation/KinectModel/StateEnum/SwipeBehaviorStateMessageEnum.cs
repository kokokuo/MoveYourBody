using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectModel.StateEnum
{
    /// <summary>
    /// Swipe訊號所有可能的錯誤狀態
    /// </summary>
    public enum SwipeBehaviorStateMessageEnum
    {
        Init = 0,
        Correct,
        Swipe_Up_AngleBetweenVerticalOrientation_Over,
        Swipe_Up_AngleBetweenHorizonOrientation_Over,
        Swipe_Down_AngleBetweenVerticalOrientation_Over,
        Swipe_Down_AngleBetweenHorizonOrientation_Over,
        Swipe_Right_AngleBetweenVerticalOrientation_Over,
        Swipe_Right_AngleBetweenHorizonOrientation_Over,
        Swipe_Left_AngleBetweenVerticalOrientation_Over,
        Swipe_Left_AngleBetweenHorizonOrientation_Over,
        Swipe_Velocity_Not_Enough,
        Swipe_Length_Not_Enough
    }
}
