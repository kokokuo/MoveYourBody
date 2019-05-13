using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrozKinect;
using Microsoft.Xna.Framework;

namespace MrozKinect.KinectModels
{
    public class DudeModel : KinectModel
    {
        public DudeModel(Game game, string modelName, KinectSkeleton kinect)
            : base(game, modelName, kinect)
        {
        }

        internal override void PrepareBoneIndexes()
        {
            boneIndexes.Add(MrJoints.Torso, skinningData.BoneIndices["Pelvis"]);
            boneIndexes.Add(MrJoints.Neck, skinningData.BoneIndices["Head"]);

            boneIndexes.Add(MrJoints.RightShoulder, skinningData.BoneIndices["R_UpperArm"]);
            boneIndexes.Add(MrJoints.RightElbow, skinningData.BoneIndices["R_Forearm"]);
            boneIndexes.Add(MrJoints.RightHand, skinningData.BoneIndices["R_Hand"]);
            boneIndexes.Add(MrJoints.RightHip, skinningData.BoneIndices["R_Thigh"]);
            boneIndexes.Add(MrJoints.RightKnee, skinningData.BoneIndices["R_Knee"]);
            boneIndexes.Add(MrJoints.RightFoot, skinningData.BoneIndices["R_Ankle"]);

            boneIndexes.Add(MrJoints.LeftShoulder, skinningData.BoneIndices["L_UpperArm"]);
            boneIndexes.Add(MrJoints.LeftElbow, skinningData.BoneIndices["L_Forearm"]);
            boneIndexes.Add(MrJoints.LeftHand, skinningData.BoneIndices["L_Hand"]);
            boneIndexes.Add(MrJoints.LeftHip, skinningData.BoneIndices["L_Thigh1"]);
            boneIndexes.Add(MrJoints.LeftKnee, skinningData.BoneIndices["L_Knee2"]);
            boneIndexes.Add(MrJoints.LeftFoot, skinningData.BoneIndices["L_Ankle1"]);
        }

        internal override void ResetModelToTPose()
        {
            UpdateBone(MrJoints.Torso, Matrix.CreateRotationX((float)Math.PI));
            UpdateBone(MrJoints.LeftShoulder, Matrix.CreateRotationY(-.8f));
            UpdateBone(MrJoints.RightShoulder, Matrix.CreateRotationZ(.3f) * Matrix.CreateRotationY(.7f));
            UpdateBone(MrJoints.RightHip, Matrix.CreateRotationY(.15f));
            UpdateBone(MrJoints.LeftHip, Matrix.CreateRotationY(-.15f));


            UpdateBone(MrJoints.LeftHand, Matrix.CreateRotationX(-1.5f));
            UpdateBone(MrJoints.RightHand, Matrix.CreateRotationX(1.5f));

            UpdateBone(MrJoints.LeftElbow, Matrix.CreateRotationX(-.3f));
            UpdateBone(MrJoints.RightElbow, Matrix.CreateRotationX((float).3f));//gameTime.TotalGameTime.TotalSeconds ));//
        }
    }
}
