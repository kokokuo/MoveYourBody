using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrozKinect;
using Microsoft.Xna.Framework;

namespace MrozKinect.KinectModels
{
    public class LudekModel : KinectModel
    {
        public LudekModel(Game game, string modelName, KinectSkeleton kinect)
            : base(game, modelName, kinect)
        {
        }

        internal override void PrepareBoneIndexes()
        {
            boneIndexes.Add(MrJoints.Torso, skinningData.BoneIndices["Bip001_Pelvis"]);
            boneIndexes.Add(MrJoints.Neck, skinningData.BoneIndices["Bip001_Head"]);

            boneIndexes.Add(MrJoints.RightShoulder, skinningData.BoneIndices["Bip001_R_UpperArm"]);
            boneIndexes.Add(MrJoints.RightElbow, skinningData.BoneIndices["Bip001_R_Forearm"]);
            boneIndexes.Add(MrJoints.RightHand, skinningData.BoneIndices["Bip001_R_Hand"]);
            boneIndexes.Add(MrJoints.RightHip, skinningData.BoneIndices["Bip001_R_Thigh"]);
            boneIndexes.Add(MrJoints.RightKnee, skinningData.BoneIndices["Bip001_R_Calf"]);
            boneIndexes.Add(MrJoints.RightFoot, skinningData.BoneIndices["Bip001_R_Foot"]);

            boneIndexes.Add(MrJoints.LeftShoulder, skinningData.BoneIndices["Bip001_L_UpperArm"]);
            boneIndexes.Add(MrJoints.LeftElbow, skinningData.BoneIndices["Bip001_L_Forearm"]);
            boneIndexes.Add(MrJoints.LeftHand, skinningData.BoneIndices["Bip001_L_Hand"]);
            boneIndexes.Add(MrJoints.LeftHip, skinningData.BoneIndices["Bip001_L_Thigh"]);
            boneIndexes.Add(MrJoints.LeftKnee, skinningData.BoneIndices["Bip001_L_Calf"]);
            boneIndexes.Add(MrJoints.LeftFoot, skinningData.BoneIndices["Bip001_L_Foot"]);
        }

        internal override void ResetModelToTPose()
        {
            UpdateBone(MrJoints.Torso, Matrix.CreateRotationY(-(float)Math.PI / 2));
        }
    }
}
