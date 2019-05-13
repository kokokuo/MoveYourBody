

//*****************
//blog.arbuzz.eu
//*****************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Input;
using MrozKinect;
using OpenNI;


namespace MrozKinect
{

    public class KinectSkeleton : GameComponent
    {
        private Game game;
        private MrKinect kinect;

        private UserGenerator userGenerator;
        private SkeletonCapability skeletonCapbility;
        private PoseDetectionCapability poseDetectionCapability;
        private string calibPose;

        public Dictionary<int, Dictionary<SkeletonJoint, Joint>> joints = new Dictionary<int, Dictionary<SkeletonJoint, Joint>>();

        public delegate void CalibrationEnded();
        public event CalibrationEnded CalibrationHasEnded;

        private Texture2D userTexture;
        private int frameNr = 0;
        bool isTracking = false;
        public bool IsTrackingUser
        {
            get { return joints.Count() > 0; }
        }
        private int activeUser = 0;

        int[] users;
        public KinectSkeleton(MrKinect kinect, Game game)
            : base(game)
        {
            this.kinect = kinect;
            this.game = game;


            // PrepareUserGenerator();
            //  PreparePoseDetectionCapability();
            // PrepareSkeletonCapbility();
            //  userTexture = new Texture2D(game.GraphicsDevice, 640, 480);//, false, SurfaceFormat.Single);
            // this.calibPose = this.skeletonCapbility.GetCalibrationPose();
        }



        //private void PreparePoseDetectionCapability()
        //{
        //    poseDetectionCapability = new PoseDetectionCapability(this.userGenerator);
        //    poseDetectionCapability.PoseDetected += new PoseDetectionCapability.PoseDetectedHandler(poseDetectionCapability_PoseDetected);
        //    poseDetectionCapability.PoseEnded += new PoseDetectionCapability.PoseEndedHandler(poseDetectionCapability_PoseEnded);
        //}

        //void poseDetectionCapability_PoseEnded(ProductionNode node, string pose, uint id)
        //{
        //    throw new NotImplementedException();
        //}

        //private void PrepareSkeletonCapbility()
        //{
        //    skeletonCapbility = new SkeletonCapability(this.userGenerator);
        //    skeletonCapbility.SetSmoothing(.6f);
        //    skeletonCapbility.CalibrationEnd += new SkeletonCapability.CalibrationEndHandler(skeletonCapbility_CalibrationEnd);
        //    skeletonCapbility.SetSkeletonProfile(SkeletonProfile.All);
        //}

        //private void PrepareUserGenerator()
        //{
        //    userGenerator = new UserGenerator(this.kinect.context);
        //    userGenerator.NewUser += new UserGenerator.NewUserHandler(userGenerator_NewUser);
        //    userGenerator.LostUser += new UserGenerator.LostUserHandler(userGenerator_LostUser);
        //    userGenerator.StartGenerating();
        //}


        //void userGenerator_NewUser(ProductionNode node, uint id)
        //{
        //    this.poseDetectionCapability.StartPoseDetection(this.calibPose, id);
        //}

        //void userGenerator_LostUser(ProductionNode node, uint id)
        //{
        //    this.joints.Remove(id);
        //}

        //void poseDetectionCapability_PoseDetected(ProductionNode node, string pose, uint id)
        //{
        //    if (joints.Count() == 0)
        //    {
        //        this.poseDetectionCapability.StopPoseDetection(id);
        //        this.skeletonCapbility.RequestCalibration(id, true);
        //    }
        //}


        //void skeletonCapbility_CalibrationEnd(ProductionNode node, uint id, bool success)
        //{
        //    if (IsTrackingUser)
        //        return;
        //    if (success)
        //    {
        //        activeUser = id;
        //        skeletonCapbility.StartTracking(id);
        //        joints.Add(id, new Dictionary<SkeletonJoint, Joint>());
        //        GetJointsOrientationAndPosition(id);
        //        if (CalibrationHasEnded != null)
        //            CalibrationHasEnded();
        //    }
        //    else
        //    {
        //        this.poseDetectionCapability.StartPoseDetection(calibPose, id);
        //    }
        //}
        int temp = 55;
        Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> J;
        public void UpdateDate(int[] users, Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> Joints, int ID, SkeletonCapability skeletonCapbility, bool isTracking, bool Tracking)
        {
            this.users = users;
            this.J = Joints;

            this.skeletonCapbility = skeletonCapbility;
            this.isTracking = isTracking;
            if (ID != temp)
            {
                a2 = 0;
                temp = ID;
            }
            if (isTracking && a2 < aaa && Tracking)
            {
                this.activeUser = ID;
                a2++;
                joints.Add(activeUser, new Dictionary<SkeletonJoint, Joint>());
            }
            //if (!isTracking) 
            //{
            //    a2 = 0;
            //}
        }
        int aaa = 1;
        int a2 = 0;
        public override void Update(GameTime gameTime)
        {
            int[] users = this.users;
            //if (users.Length > 0 && this.skeletonCapbility.IsTracking(users[0]))
            //if (users.Length > 0 && joints.Count() > 0)
            if (isTracking && a2 == 1)
                GetJointsOrientationAndPosition(activeUser);
        }

        private void GetJointsOrientationAndPosition(int user)
        {
            GetJointOrientationAndPosition(user, SkeletonJoint.Torso);
            GetJointOrientationAndPosition(user, SkeletonJoint.Head);
            GetJointOrientationAndPosition(user, SkeletonJoint.Neck);

            GetJointOrientationAndPosition(user, SkeletonJoint.LeftShoulder);
            GetJointOrientationAndPosition(user, SkeletonJoint.LeftElbow);
            GetJointOrientationAndPosition(user, SkeletonJoint.LeftHand);

            GetJointOrientationAndPosition(user, SkeletonJoint.RightShoulder);
            GetJointOrientationAndPosition(user, SkeletonJoint.RightElbow);
            GetJointOrientationAndPosition(user, SkeletonJoint.RightHand);

            GetJointOrientationAndPosition(user, SkeletonJoint.Torso);

            GetJointOrientationAndPosition(user, SkeletonJoint.LeftHip);
            GetJointOrientationAndPosition(user, SkeletonJoint.LeftKnee);
            GetJointOrientationAndPosition(user, SkeletonJoint.LeftFoot);

            GetJointOrientationAndPosition(user, SkeletonJoint.RightHip);
            GetJointOrientationAndPosition(user, SkeletonJoint.RightKnee);
            GetJointOrientationAndPosition(user, SkeletonJoint.RightFoot);
        }



        private void GetJointOrientationAndPosition(int user, SkeletonJoint joint)
        {
            SkeletonJointOrientation or = new SkeletonJointOrientation();
            or = skeletonCapbility.GetSkeletonJointOrientation(user, joint);

            SkeletonJointPosition pos = new SkeletonJointPosition();
            pos = skeletonCapbility.GetSkeletonJointPosition(user, joint);
            //J[user][joints].Position
            joints[user][joint] = new Joint(or, pos);
        }

        public Matrix GetOrientation(MrJoints joint)
        {
            return joints[activeUser][(SkeletonJoint)joint].orientation;
        }

        public Vector3 GetPosition(MrJoints joint)
        {
            return joints[activeUser][(SkeletonJoint)joint].position;
        }


        //public Texture2D GetUserTexture()
        //{
        //    if (frameNr != kinect.frameNr)
        //    {
        //        frameNr = kinect.frameNr;
        //        //userTexture = simpleCV.GetUserTexture(userGenerator.GetUserPixels(0).SceneMapPtr, game);
        //    }
        //    return userTexture;
        //}

        //public Texture2D UserTexture
        //{
        //    get { return GetUserTexture(); }
        //}
    }
}