using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using Microsoft.Xna.Framework;
using MrozKinect;
using Primitives3D;


namespace MrozKinect.KinectModels
{
    public abstract class KinectModel : SkinnedModel
    {
        internal Dictionary<MrJoints, int> boneIndexes = new Dictionary<MrJoints, int>();
        private KinectSkeleton kinectSkeleton;
        private Matrix[] boneTransforms;
        private Matrix[] absoluteTransforms;        
        private bool[] changeBone;
        private float modelScale = 0;
        private SpherePrimitive sphere;

        internal abstract void PrepareBoneIndexes();
        internal abstract void ResetModelToTPose();

        public KinectModel(Game game, string modelName, KinectSkeleton kinectSceleton)
            : base(game, modelName)
        {
            this.kinectSkeleton = kinectSceleton;
            boneTransforms = new Matrix[skinningData.BindPose.Count];
            absoluteTransforms = new Matrix[skinningData.BindPose.Count];

            PrepareBoneIndexes();
            ResetToTPose();
            sphere = new SpherePrimitive(game.GraphicsDevice, 100,10);
        }

        void ComputeScale()
        {
            if (modelScale != 0)
                return;
            float f1 = (kinectSkeleton.GetPosition(MrJoints.LeftElbow) - kinectSkeleton.GetPosition(MrJoints.LeftShoulder)).Length();
            float f2 = (absoluteTransforms[boneIndexes[MrJoints.LeftElbow]].Translation - absoluteTransforms[boneIndexes[MrJoints.LeftShoulder]].Translation).Length();
            modelScale = f1 / f2;
        }


        public void UpdatePose(GameTime gameTime)
        {
            ResetToTPose();
            animationPlayer.AbsoluteWorldTransforms.CopyTo(absoluteTransforms, 0);
            changeBone = new bool[absoluteTransforms.Length];
            if (kinectSkeleton.IsTrackingUser)
            {
                ComputeScale();
                UpdateBoneMatrix(MrJoints.Torso, .25f);
                UpdateBoneMatrix(MrJoints.Neck, -.1f);
                UpdateBoneMatrix(MrJoints.RightShoulder);
                UpdateBoneMatrix(MrJoints.RightElbow);
                //UpdateBoneMatrix( MrJoints.RightElbow, SkeletonJoint.RightHand, 0);
                UpdateBoneMatrix(MrJoints.RightHip);
                UpdateBoneMatrix(MrJoints.RightKnee);
                UpdateBoneMatrix(MrJoints.RightFoot, MrJoints.RightKnee, MrJoints.RightFoot);


                UpdateBoneMatrix(MrJoints.LeftShoulder);
                UpdateBoneMatrix(MrJoints.LeftElbow);
                //UpdateBoneMatrix( MrJoints.LeftElbow, SkeletonJoint.LeftHand, 0);
                UpdateBoneMatrix(MrJoints.LeftHip);
                UpdateBoneMatrix(MrJoints.LeftKnee);


                UpdateBoneMatrix(MrJoints.LeftFoot, MrJoints.LeftKnee, MrJoints.LeftFoot);
            }
            animationPlayer.UpdateAbsolutWorldTransforms(Matrix.Identity, boneTransforms, absoluteTransforms, changeBone);
            animationPlayer.UpdateSkinTransforms();
        }

        private Matrix UpdateBoneMatrix(MrJoints sJ)
        {
            return UpdateBoneMatrix(sJ, 0);
        }

        private Matrix UpdateBoneMatrix(MrJoints sJ, float m)
        {
            int jointIndex;
            jointIndex = boneIndexes[sJ];
            return UpdateBoneMatrix(jointIndex, kinectSkeleton.GetOrientation(sJ), kinectSkeleton.GetPosition(sJ), m);
        }

        private Matrix UpdateBoneMatrix(MrJoints sJ, MrJoints sJorientation, MrJoints sJposition)
        {
            int jointIndex;
            jointIndex = boneIndexes[sJ];
            return UpdateBoneMatrix(jointIndex, kinectSkeleton.GetOrientation(sJorientation), kinectSkeleton.GetPosition(sJposition), 0);
        }

        private Matrix UpdateBoneMatrix(MrJoints sJ, Matrix orientation, Vector3 position, float m)
        {
            return UpdateBoneMatrix(boneIndexes[sJ], orientation, position, 0);
        }

        private Matrix UpdateBoneMatrix(int jointIndex, Matrix orientation, Vector3 position, float m)
        {

            SetOrientationMatrix(jointIndex, orientation);
            SetTranslationMatrix(jointIndex, position, m);

            changeBone[jointIndex] = true;
            return absoluteTransforms[jointIndex];
        }

        private void SetTranslationMatrix(int jointIndex, Vector3 position, float m)
        {
            if (m != 0)
                position += (Matrix.CreateTranslation(0, 1, 0) * absoluteTransforms[jointIndex]).Translation * -m;
            absoluteTransforms[jointIndex].Translation = position;
        }

        private void SetOrientationMatrix(int jointIndex, Matrix orientation)
        {
            absoluteTransforms[jointIndex] = absoluteTransforms[jointIndex] * orientation * Matrix.CreateScale(modelScale);
            changeBone[jointIndex] = true;
        }

        private void ResetToTPose()
        {
            animationPlayer.ResetToBindPose(Matrix.Identity);
            animationPlayer.GetBoneTransforms().CopyTo(boneTransforms, 0);
            ResetModelToTPose();
            animationPlayer.UpdateWorldTransforms(Matrix.Identity, boneTransforms);
        }
       
        internal int UpdateBone(int boneIndex, Matrix matrix)
        {
            boneTransforms[boneIndex] = matrix * boneTransforms[boneIndex];
            return boneIndex;
        }

        internal int UpdateBone(MrJoints joint, Matrix matrix)
        {
            return UpdateBone(boneIndexes[joint], matrix);
        }

        public Vector3 GetJointPosition(MrJoints joint)
        {
            return kinectSkeleton.GetPosition(joint);
        }

        private void DrawSpheres(Matrix projection, Matrix view)
        {
            try
            {
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.Head)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.LeftHip)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.LeftFoot)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.LeftElbow)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.LeftKnee)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.LeftShoulder)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.LeftHand)), view, projection, Color.Red);

                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.RightHip)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.RightFoot)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.RightElbow)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.RightKnee)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.RightShoulder)), view, projection, Color.Red);
                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.RightHand)), view, projection, Color.Red);

                sphere.Draw(Matrix.CreateTranslation(kinectSkeleton.GetPosition(MrJoints.Neck)), view, projection, Color.Red);
            }
            catch { }
        }

        public override void Draw(Matrix world, Matrix projection, Matrix view, GameTime gameTime)
        {
            UpdatePose(gameTime);
            //DrawSpheres(projection, view);
            base.Draw(world, projection, view, gameTime);
        }

        public bool IsModelReady
        {
            get { return kinectSkeleton.IsTrackingUser; }
        }
    }
}
