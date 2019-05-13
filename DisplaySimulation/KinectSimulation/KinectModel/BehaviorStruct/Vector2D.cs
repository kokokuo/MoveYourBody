using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectModel.BehaviorStruct
{
    /// <summary>
    /// 二維向量用的資料結構
    /// </summary>
    public struct Vector2D
    {
        private float x;
        private float y;
        public float X
        {
            get { return x; }
        }
        public float Y
        {
            get { return y; }
        }
        public Vector2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
