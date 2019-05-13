using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectModel.BehaviorStruct
{
    /// <summary>
    /// 三維向量用的資料結構
    /// </summary>
    public struct Vector3D
    {
        private float x;
        private float y;
        private float z;
        public float X
        {
            get { return x; }
        }
        public float Y
        {
            get { return y; }
        }
        public float Z
        {
            get { return z; }
        }
        public Vector3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

    }
}
