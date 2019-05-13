using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectModel.StateEnum
{
    /// <summary>
    /// 飛行訊號可能飛行的方向
    /// </summary>
    public enum FlyBehaviorActionEnum
    {
        FlyDown = 0,
        FlyLeft,
        FlyRight,
        FlyStraight,
        NotFly
    }
}
