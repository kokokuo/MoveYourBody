using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using OpenNI;
using System.Runtime.InteropServices;   // needed to call external application
using System.Windows.Forms;
using System.Threading;

namespace KinectModel.Device
{
    public delegate void UserEventHandler(int user);
    /// <summary>
    /// 1.此類別主要目的式取得Kinect骨架資料,剩餘訊號演算法接在KinectActionModel已實作了此類別的功能
    /// 2.此類別主要只有KinectActionModel看到
    /// </summary>
    public class KinectProcess
    {    
        //事件
        public event UserEventHandler OnLostUser;
        public event UserEventHandler OnNewUser;
        public event UserEventHandler OnUserExit;
        public event UserEventHandler OnUserReEnter;
        public event UserEventHandler OnUserTrackingResouceChangedtoAnotherUser;    //搶奪骨架資源的事件發生
        #region Const variable
        ////////////////////////////////////////////////////////////////////////

        private const string SAMPLE_XML_FILE =@"SamplesConfig.xml";
        private const int MAX_TRACKING_RESOURCE = 1;
        //用來設定進入偵測姿勢狀態所需要站在的範圍內
        private int poseDetectionMinRange;
        private int poseDetectionMaxRange;
        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Member variable
        ////////////////////////////////////////////////////////////////////////
       
        //skeleton joint
        private Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> joints;

        MapOutputMode mapMode;
        private ScriptNode scriptNode;
        private Context context;
        private DepthGenerator g_Depth;
        private ImageGenerator g_Image;
        private UserGenerator userGenerator;
        private SkeletonCapability skeletonCapbility;
        private PoseDetectionCapability poseDetectionCapability;
        private string calibPose;
        //TrackingCounter is the resource and set the resource is one
        private int TrackingCounter = 0;
        
        
        ////////////////////////////////////////////////////////////////////////
        #endregion

        #region Member Property
        ////////////////////////////////////////////////////////////////////////

        public Context KinectContext
        {
            set { this.context = value; }
            get { return this.context; }
        }
        public DepthGenerator GeneratorDepth
        {
            set { this.g_Depth = value; }
            get { return this.g_Depth; }
        }
        public ImageGenerator GeneratorImage
        {
            set { this.g_Image = value; }
            get { return this.g_Image; }
        }
        public MapOutputMode MapMode
        {
            get { return this.mapMode; }
        }
        public UserGenerator UserGenerator
        {
            get { return this.userGenerator; }
        }
        
        public SkeletonCapability SkeletonCapbility
        {
            get { return this.skeletonCapbility; }
        }
        
        /// <summary>
        /// 取得使用者的骨架各節點資料
        /// </summary>
        public Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> Joints
        { 
            get { return this.joints; }
        }
        /// <summary>
        /// 可以得知目前追蹤骨架的資源數,預設資源只有一具骨架
        /// </summary>
        public int CurrentTrackingResource
        {           
            get { return this.TrackingCounter; }
        }
        /// <summary>
        /// 最大的骨架資源,為一組,只能取得
        /// </summary>
        public int MaxTrackingResource
        { get { return MAX_TRACKING_RESOURCE; } }
       
        ////////////////////////////////////////////////////////////////////////
        #endregion  
        
        public KinectProcess(int poseDetectionMinRangeValue, int poseDetectionMaxRangeValue)
        {
            this.context = Context.CreateFromXmlFile(SAMPLE_XML_FILE, out scriptNode); 
            //Check whether he Label node is include Depth and transfomr data to DepthGenerator or not 
            this.g_Depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            //--close image
            //this.g_Image = context.FindExistingNode(NodeType.Image) as ImageGenerator;
            //this.g_Depth.AlternativeViewpointCapability.SetViewpoint(g_Image);
            
            if (g_Depth == null)
            {
                throw new Exception("Viewer must have a depth node!");
            }
            this.poseDetectionMinRange = poseDetectionMinRangeValue;
            this.poseDetectionMaxRange = poseDetectionMaxRangeValue;
            mapMode = this.g_Depth.MapOutputMode;
           
            this.userGenerator = new UserGenerator(this.context);
            
            this.skeletonCapbility = this.userGenerator.SkeletonCapability;
            this.poseDetectionCapability = this.userGenerator.PoseDetectionCapability;
            this.calibPose = this.skeletonCapbility.CalibrationPose;
            //設定骨架的平滑值
            this.skeletonCapbility.SetSmoothing(0.6F);
            joints = new Dictionary<int, Dictionary<OpenNI.SkeletonJoint, OpenNI.SkeletonJointPosition>>();
            //////////////////////////////////////////////////
            this.userGenerator.NewUser += userGenerator_NewUser;           
            this.userGenerator.UserExit += userGenerator_UserExit;
            this.userGenerator.UserReEnter += userGenerator_UserReEnter;
            this.userGenerator.LostUser += userGenerator_LostUser;
            ////////////////////////////////////////////////////////
            this.poseDetectionCapability.PoseDetected += poseDetectionCapability_PoseDetected;
            //刻意使用舊版本，因為發生事件時是屬於校正結束（失敗或成功皆可），
            this.skeletonCapbility.CalibrationEnd +=new EventHandler<CalibrationEndEventArgs>(skeletonCapbility_CalibrationEnd);
            this.skeletonCapbility.SetSkeletonProfile(SkeletonProfile.All);
            this.userGenerator.StartGenerating();
        }

        #region Public Function
        ///////////////////////////////////////////////////////////
        /// <summary>
        /// 更新Kinect畫面及資源
        /// </summary>
        public void UpdateKinectContext()
        {
            this.context.WaitOneUpdateAll(this.g_Depth);
        }

        /// <summary>
        /// 更新並取得使用者的骨架資訊
        /// </summary>
        /// <param name="user">使用者編號</param>
        public void GetJoints(int user)
        {
            GetJoint(user, SkeletonJoint.Head);
            GetJoint(user, SkeletonJoint.Neck);
            GetJoint(user, SkeletonJoint.LeftShoulder);
            GetJoint(user, SkeletonJoint.LeftElbow);
            GetJoint(user, SkeletonJoint.LeftHand);
            GetJoint(user, SkeletonJoint.RightShoulder);
            GetJoint(user, SkeletonJoint.RightElbow);
            GetJoint(user, SkeletonJoint.RightHand);
            GetJoint(user, SkeletonJoint.Torso);
            GetJoint(user, SkeletonJoint.LeftHip);
            GetJoint(user, SkeletonJoint.LeftKnee);
            GetJoint(user, SkeletonJoint.LeftFoot);
            GetJoint(user, SkeletonJoint.RightHip);
            GetJoint(user, SkeletonJoint.RightKnee);
            GetJoint(user, SkeletonJoint.RightFoot);

        }

        public void ReStart()
        {
            
            this.userGenerator.StopGenerating();
            this.userGenerator.StartGenerating();
            this.joints.Clear();
            this.TrackingCounter = 0;
        }
        //移除骨架和釋放骨架的佔用資源，呼叫此功能的情況是：使用者離開畫面或使用者已經完全遺失或系統找到的骨架有問題
        public bool RemoveJointAndResource(int userId)
        {
            //if the user has the joint data ,and the user removed,then tracking release
            if (joints.ContainsKey(userId))
            {
                Console.WriteLine("number " + userId.ToString() + " miss");
                joints.Remove(userId);
                //if the TrackingCounter is 1 then released
                if (TrackingCounter == 1)
                {
                   // Console.WriteLine("count:" + TrackingCounter.ToString());
                    TrackingCounter--;
                    Console.WriteLine("release resource,now is:" + TrackingCounter.ToString());
                }
                this.poseDetectionCapability.StartPoseDetection(this.calibPose, userId);
                return true;
            }
            return false;

        }
        ///////////////////////////////////////////////////////////
        #endregion

        /////////////////////////////////////////////
        //取得每一個關節的座鰾(三維)
        private void GetJoint(int user, SkeletonJoint joint)
        {           
            SkeletonJointPosition pos = this.skeletonCapbility.GetSkeletonJointPosition(user, joint);
            SkeletonJointPosition realPos = this.skeletonCapbility.GetSkeletonJointPosition(user, joint);
            if (pos.Position.Z == 0)
            {
                //if the posZ = 0 ,means kinect sense some problem at this frame ,so confidence set 0
                realPos.Confidence = 0;
                pos.Confidence = 0;
            }
            else
            {
                pos.Position = this.g_Depth.ConvertRealWorldToProjective(pos.Position);
            }
            //是三維值
            this.joints[user][joint] = realPos;
        }
 
        #region Event handler
        ////////////////////////////////////////////////////////////////////////

        //如果使用者從新進入畫面則會發生此事件，注意：使用者被判定遺失會有約10秒的收尋時間
        private void userGenerator_UserReEnter(object sender, UserReEnterEventArgs e)
        {
            if (OnUserReEnter != null)
            { 
                OnUserReEnter.Invoke(e.ID);
                Console.WriteLine("number " + e.ID.ToString() + "User ReEnter!");
            }
            //restart open PoseDetection
            this.poseDetectionCapability.StartPoseDetection(this.calibPose, e.ID);
        }
        //如果使用者離開畫面觸發，注意：此事件是離開畫面，而非離開後10秒找不到人的遺失事件
        private void userGenerator_UserExit(object sender, UserExitEventArgs e)
        {
            if (OnUserExit != null)
            {
                OnUserExit.Invoke(e.ID);
                Console.WriteLine("number " + e.ID.ToString() + "UserExit...");
            }
            //restart open PoseDetection
            this.poseDetectionCapability.StartPoseDetection(this.calibPose, e.ID);
        }     

        //Start detect pose after finding a user
        private void userGenerator_NewUser(object sender, NewUserEventArgs e)
        {
            if (OnNewUser != null)
            { OnNewUser.Invoke(e.ID); }
            this.poseDetectionCapability.StartPoseDetection(this.calibPose, e.ID);    
        }
        //Remove user after user is lost
        private void userGenerator_LostUser(object sender, UserLostEventArgs e)
        {
            if (OnLostUser != null)
            { OnLostUser.Invoke(e.ID); }
        }
        
        //if the pose detected,the start calibrate user pose
        private void poseDetectionCapability_PoseDetected(object sender, PoseDetectedEventArgs e)
        {
            float user_posZ = ((UserGenerator)sender).GetCoM(e.ID).Z;
            //若使用者站在此距離範圍內即可進入校正 
            if (user_posZ < poseDetectionMaxRange && user_posZ > poseDetectionMinRange)
            {
                this.skeletonCapbility.RequestCalibration(e.ID, true);
                this.poseDetectionCapability.StopPoseDetection(e.ID);
            }
            
        }

        private void skeletonCapbility_CalibrationEnd(object sender, CalibrationEndEventArgs e)
        {
            //若校正成功且骨架資源未使用
            if (e.Success)
            {
                //若資源未被使用
                if (TrackingCounter < MAX_TRACKING_RESOURCE)
                {
                    TrackingCounter++; //累加使用資源
                    this.skeletonCapbility.StartTracking(e.ID);
                    this.joints.Add(e.ID, new Dictionary<SkeletonJoint, SkeletonJointPosition>());
                }
                //若資源已滿
                else if(TrackingCounter == MAX_TRACKING_RESOURCE)
                {
                    this.skeletonCapbility.StartTracking(e.ID);
                    int[] users = userGenerator.GetUsers();
                    //尋找有骨架的使用者並釋放其資源，此部分會觸發搶奪骨架資源的事件
                    foreach (int user in users)
                    {
                        if (joints.ContainsKey(user))
                        {
                            if (OnUserTrackingResouceChangedtoAnotherUser != null)
                            { OnUserTrackingResouceChangedtoAnotherUser(user); }
                            joints.Clear();
                            break;
                        }
                    }
                    this.joints.Add(e.ID, new Dictionary<SkeletonJoint, SkeletonJointPosition>());
                }
            }
            else
            {
                this.poseDetectionCapability.StartPoseDetection(calibPose, e.ID);
            }
        }

        ////////////////////////////////////////////////////////////////////////
        #endregion
    }
}
