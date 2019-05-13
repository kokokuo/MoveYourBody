

//*****************
//blog.arbuzz.eu
//*****************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OpenNI;

namespace MrozKinect
{
    public enum MrJoints
    {
        Root = 0,
        Head = 1,
        Neck = 2,
        Torso = 3,
        Waist = 4,
        LeftCollar = 5,
        LeftShoulder = 6,
        LeftElbow = 7,
        LeftWrist = 8,
        LeftHand = 9,
        LeftFingertip = 10,
        RightCollar = 11,
        RightShoulder = 12,
        RightElbow = 13,
        RightWrist = 14,
        RightHand = 15,
        RightFingertip = 16,
        LeftHip = 17,
        LeftKnee = 18,
        LeftAnkle = 19,
        LeftFoot = 20,
        RightHip = 21,
        RightKnee = 22,
        RightAnkle = 23,
        RightFoot = 24
    }

    public class Joint
    {
        public static DepthGenerator depth;
        public Matrix orientation;
        public Vector3 position;
        public Vector3 projectivePosition;
        public Joint(SkeletonJointOrientation orientation, SkeletonJointPosition position)
        {
            this.position = new Vector3(position.Position.X, position.Position.Y, position.Position.Z);
            this.orientation = KinectToXna(orientation);
            if (depth != null)
            {
                Point3D temp = depth.ConvertRealWorldToProjective(position.Position);
                projectivePosition = new Vector3(temp.X, temp.Y, temp.Z);
            }
        }

        private static Matrix KinectToXna(SkeletonJointOrientation matrix3X3)
        {
            Matrix m = new Matrix(matrix3X3.X1, -matrix3X3.Y1, matrix3X3.Z1, 0,
                                 -matrix3X3.X2, matrix3X3.Y2, -matrix3X3.Z2, 0,
                                 matrix3X3.X3, -matrix3X3.Y3, matrix3X3.Z3, 0,
                                0, 0, 0, 1);
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            q.Y = -q.Y;
            m = Matrix.CreateFromQuaternion(q);
            return m;
        }

    }
}
