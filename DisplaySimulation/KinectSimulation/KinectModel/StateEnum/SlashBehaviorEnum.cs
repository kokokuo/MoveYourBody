using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectModel.StateEnum
{
    /// <summary>
    /// 砍的所有可能動作
    /// </summary>
    public enum SlashBehaviorEnum
    {
        /// <summary>
        /// 向左橫砍
        /// </summary>
        LeftHorizonSlash = 0,
        /// <summary>
        /// 向右橫砍
        /// </summary>
        RightHorizonSlash,
        /// <summary>
        /// 垂直下砍
        /// </summary>
        VerticalSlash,
        /// <summary>
        /// 向左斜砍
        /// </summary>
        LeftRakedSlash,
        /// <summary>
        /// 向右斜砍
        /// </summary>
        RightRakedSlash,
    }
}
