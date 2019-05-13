#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//kinectPart
using KinectSimulation;
using OpenNI;
using KinectModel;
using KinectModel.BehaviorStruct;
using KinectModel.StateEnum;

#endregion



namespace Game_Kinect_Project
{
    public class InputState
    {
        public KeyboardState[] keyState;
        public MouseState mouseState; 
     
        public InputState()
        {          
            keyState = new KeyboardState[2];       
            GetInput(false);          
        }

        public void GetInput(bool singlePlayer)
        {
            mouseState = Mouse.GetState();
            if (singlePlayer)
                keyState[0] = Keyboard.GetState();
            else
                keyState[1] = Keyboard.GetState();
        }

        public void CopyInput(InputState state)
        {
            keyState[0] = state.keyState[0];
            keyState[1] = state.keyState[1];
        }
    }

    public class InputManager
    {
        InputState currentState;   // current frame input
        InputState lastState;      // last frame input
        KinectDisplay kinectDisplay;

        bool isMove = false;
        bool isRightHandSlash = false;
        bool isRightHandPush = false;
        bool isLeftHandPush = false;
        bool isJump = false;
        bool isRun = false;
        bool isSwipe;
        private bool isGetJoints=false;

        //���[
        Dictionary<SkeletonJoint, SkeletonJointPosition> drawSk;

        //Push����ƫ��A
        PushBehaviorData rightHandPush;
        PushBehaviorData leftHandPush;

        //���D����ƫ��A
        JumpBehaviorData jumpData;
        //���媺��ƫ��A
        SlashBehaviorData rightHandSlash;
        //�����]�B�C��񪺨B��
        private double runTimesForSpeed;
        //���k�{�����ʪ���ƫ��A
        MoveBehaviorData moveData;
        //Swipe����ƫ��A
        SwipeBehaviorData swipeData;

        //����Swipe�ɩMY�b��V���V�q����->���ե�
        private double swipeUpDownHorizonAngle;
        //����Swipe�ɩMZ�b��V���V�q����->���ե�
        private double swipeVerticalAngle;
        //����Swipe�ɩMX�b��V���V�q����->���ե�
        private double swipeRightLeftHorizonAngle;

        private string swipeMessgae = "";

        
        //�O�_�i�J����Pause
        private bool isWaitingPause;
        //�O�_Pause
        private bool isPause;

        private double pauseWaitingTime;

        private double verticalAngle;
        private double horizonAngle;


        /// <summary>
        /// Create a new input manager
        /// </summary>
        public InputManager(KinectDisplay kinectDisplay)
        {
            currentState = new InputState();
            lastState = new InputState();
            this.kinectDisplay = kinectDisplay;

            drawSk = new Dictionary<SkeletonJoint, SkeletonJointPosition>();

            //Kinect
            swipeUpDownHorizonAngle = new double();
            swipeRightLeftHorizonAngle = new double();
            swipeVerticalAngle = new double();

            //Menu�����i��|�Ψ쪺�ƥ�->Push�ΰ����,Swipe�γ\�i�H�ΨӰ��@�Ǳ��ʦ��e�����
            //////////////////////////////////////////////
            this.kinectDisplay.GetKinectAction.OnHandPush += new PushBehaviorEventHandler(KinectAction_OnHandPush);
            this.kinectDisplay.GetKinectAction.OnHandPushStable += new NoBehaviorMessageEventHandler(KinectAction_OnHandPushStable);
            this.kinectDisplay.GetKinectAction.OnSwipe += new SwipeBehaviorEventHandler(KinectAction_OnSwipe);
            this.kinectDisplay.GetKinectAction.OnNotSwipe += new NoBehaviorMessageEventHandler(KinectAction_OnNotSwipe);

            this.kinectDisplay.GetKinectAction.OnJointsUpdate += new SkeletonJointUpdateEventHandler(KinectAction_OnJointsUpdate);

            //���d�@
            ///////////////////////////////////////////////
            //���D�M�����ɪ��ƥ�
            this.kinectDisplay.GetKinectAction.OnJump += new JumpBehaviorEventHandler(KinectAction_OnJump);
            this.kinectDisplay.GetKinectAction.OnJumpStable += new NoBehaviorEventHandler(KinectAction_OnJumpStable);
            ////����M�S����ɪ��ƥ���U
            this.kinectDisplay.GetKinectAction.OnHandSlash += new SlashBehaviorEventHandler(KinectAction_OnHandSlash);
            this.kinectDisplay.GetKinectAction.OnHandSlashStable += new NoBehaviorEventHandler(KinectAction_OnHandSlashStable);
            ////�]�B��S�]�B�ɪ��ƥ���U
            this.kinectDisplay.GetKinectAction.OnFootRun += new TimesChangedEventHandler(KinectAction_OnFootRun);
            this.kinectDisplay.GetKinectAction.OnFootRunStable += new NoBehaviorEventHandler(KinectAction_OnFootRunStable);
            //���k���ʪ��ƥ���U
            this.kinectDisplay.GetKinectAction.OnMoveLeftOrRight += new MoveLeftAndRightEventHandler(KinectAction_OnMoveLeftOrRight);
            this.kinectDisplay.GetKinectAction.OnMoveLeftOrRightStable += new NoBehaviorEventHandler(KinectAction_OnMoveLeftOrRightStable);


            this.kinectDisplay.GetKinectAction.OnPauseAngleChanged += new TestAngleChangedEventHandler(KinectAction_OnAngelInvoke);

            this.kinectDisplay.GetKinectAction.OnPause += new PauseBehaviorEventHandler(KinectAction_OnPause);
            this.kinectDisplay.GetKinectAction.OnWaitingPause += new TimesChangedEventHandler(KinectAction_OnWaitingPause);
            this.kinectDisplay.GetKinectAction.OnNotPause += new NoBehaviorEventHandler(KinectAction_OnNotPause);
            pauseWaitingTime = 0;
            isPause = false;
            isWaitingPause = false;
        
        }

        private void KinectAction_OnAngelInvoke(double angle, string whichAngle)
        {
            if (whichAngle == "vertical")
                verticalAngle = angle;
            if (whichAngle == "horizon")
                horizonAngle = angle;
        }
        private void KinectAction_OnPause(double vertical, double horizon)
        {
            isPause = true;
            verticalAngle = vertical;
            horizonAngle = horizon;
        }

        private void KinectAction_OnNotPause()
        {
            isWaitingPause = false;
            pauseWaitingTime = 0;
        }
        private void KinectAction_OnWaitingPause(double waitingTime)
        {
            isWaitingPause = true;
            pauseWaitingTime = waitingTime;
        }

        private void KinectAction_OnJointsUpdate(Dictionary<SkeletonJoint, SkeletonJointPosition> pos)
        {
            drawSk = pos;
            isGetJoints = true;
        }

        private Vector2 GetPos(Dictionary<SkeletonJoint, SkeletonJointPosition> dict, SkeletonJoint j)
        {
            //Get
            if (isGetJoints && dict.Count > 0)
            {
                Point3D pos = this.kinectDisplay.GetKinectAction.ConvertRealWorldToProjective(dict[j].Position);
                //Point3D pos = dict[j].Position;
                //if (dict[j].Confidence == 0)
                //    return;
                Vector2 GetSkeletonPosition = new Vector2(pos.X, (-pos.Y));
                return GetSkeletonPosition;
            }
            else return Vector2.One;
        }

        private void KinectAction_OnSwipeAngleChanged(double angle, string message)
        {
            if (message == "SwipeUpDownAngleBetweenHorizonOrientation") swipeUpDownHorizonAngle = angle;
            if (message == "AngleBetweenVerticalOrientation") swipeVerticalAngle = angle;
            if (message == "SwipeRightLeftAngleBetweenHorizonOrientation") swipeRightLeftHorizonAngle = angle;
        }
        private void KinectAction_OnSwipe(SwipeBehaviorData swipe)
        {
            swipeData = swipe;
            isSwipe = true;
        }
        private void KinectAction_OnNotSwipe(string message)
        {
            swipeMessgae = message;
            isSwipe = false;
        }
        private void KinectAction_OnHandPushStable(string hand)
        {
            if (hand == "RightHand") isRightHandPush = false;
            else if (hand == "LeftHand") isLeftHandPush = false;
        }
        private void KinectAction_OnHandPush(PushBehaviorData push)
        {
            if (push.Hand == "RightHand")
            {
                isRightHandPush = true;
                rightHandPush = push;
            }
            else if (push.Hand == "LeftHand")
            {
                isLeftHandPush = true;
                leftHandPush = push;
            }
        }

        private void KinectAction_OnJump(JumpBehaviorData jump)
        {
            jumpData = jump;
            isJump = true;
        }

        private void KinectAction_OnJumpStable()
        {
            isJump = false;
        }

        private void KinectAction_OnHandSlash(SlashBehaviorData slash)
        {

            isRightHandSlash = true;
            rightHandSlash = slash;

        }

        private void KinectAction_OnHandSlashStable()
        {
            isRightHandSlash = false;
        }

        private void KinectAction_OnFootRun(double runTime)
        {
            isRun = true;
            runTimesForSpeed = runTime;
        }

        private void KinectAction_OnFootRunStable()
        {
            isRun = false;
        }

        Vector3 currentPosition;
        int centerPosition;
        //TEST
        public Vector3 CurrentUserPosition
        { get { return currentPosition; } }
        /// <summary>
        /// �e��������
        /// </summary>
        public int CenterPosition
        { get { return centerPosition; } }
        private void KinectAction_OnMoveLeftOrRight(MoveBehaviorData move)
        {
            
            moveData = move;
            isMove = true;
            Vector3 temp=Vector3.Zero;
            temp.X = move.CurrentUserPosition.X;
            temp.Y = move.CurrentUserPosition.Y;
            temp.Z = move.CurrentUserPosition.Z;
            currentPosition = temp;
            
            centerPosition = move.CenterPosition;
            //if (moveData.MoveOrienation == "MoveLeft")
            //{
            //    isLeftMove = true;
            //    isRightMove = false;
            //}
            //else if (moveData.MoveOrienation == "MoveRight")
            //{
            //    isRightMove = true;
            //    isLeftMove = false;
            //}
            //else 
            //{
                isLeftMove = false;
                isRightMove = false;
            //}
        }
        private void KinectAction_OnMoveLeftOrRightStable()
        {
            isMove = false;
        }

        bool isLeftMove = false;
        bool isRightMove = false;

        /// <summary>
        /// Begin input (aqruire input from all controlls)
        /// </summary>
        public void BeginInputProcessing(bool singlePlayer)
        {
            currentState.GetInput(singlePlayer);
        }

        /// <summary>
        /// End input (save current input to last frame input)
        /// </summary>
        public void EndInputProcessing()
        {
            lastState.CopyInput(currentState);
        }

        /// <summary>
        /// Get the current input state
        /// </summary>
        public InputState CurrentState
        {
            get { return currentState; }
        }

        /// <summary>
        /// Get last frame input state
        /// </summary>
        public InputState LastState
        {
            get { return lastState; }
        }

        /// <summary>
        /// Check if a key is down in current frame for a given player
        /// </summary>
        public bool IsKeyDown(int player, Keys key)
        {
            return currentState.keyState[player].IsKeyDown(key);
        }

        /// <summary>
        /// Check if a key was pressed in this frame for a given player
        /// (down in this frame and up in last frame)
        /// </summary>
        public bool IsKeyPressed(int player, Keys key)
        {
            return currentState.keyState[player].IsKeyDown(key) &&
                lastState.keyState[player].IsKeyUp(key);
        }
     
        public Vector2 GetMousePosittion(int player) 
        {
            return  new Vector2(currentState.mouseState.X, currentState.mouseState.Y);
        }

        public Vector2 GetRightHand(int player) 
        {
            return GetPos(drawSk, SkeletonJoint.RightHand);
        }

        public Vector2 GetHead() 
        {
            return GetPos(drawSk, SkeletonJoint.Head);
        }

        //public DepthGenerator Depth { get { return kinectDisplay.GetKinectAction.Depth; } }
        public int[] Users { get { return kinectDisplay.GetKinectAction.GetUsers(); } }

        public bool GetLeftPushSignal
        {
            get
            {
                return isLeftHandPush;
            }
            set
            {
                isLeftHandPush = value;
            }
        }

        public bool GetSwip
        {
            get
            {
                return isSwipe;
            }
            set 
            {
                isSwipe = value;
            }
        }

        public bool IsMouseClickLeft(int player)
        {
            return currentState.mouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsMouseClickRight(int player) 
        {
            return currentState.mouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsJump
        {
            get
            {
                return isJump;
            }
        }

        public bool IsRun
        {
            get
            {
                return isRun;
            }
        }

        public double Speed
        {
            get
            {
                //30
                return runTimesForSpeed;
            }
        }

        public bool IsMoveToLeft
        {
            get
            {
                return isLeftMove;
            }
        }

        public bool IsMoveToRight
        {
            get
            {
                return isRightMove;
            }
        }

        public bool ISRightHandSlash
        {
            get
            {
                return isRightHandSlash;
            }
        }

        public bool IsPause
        {
            get
            {
                return isPause;
            }
            set
            {
                isPause = value;
            }
        }

        //�o��ثekinect�缾�a�����A
        public UserStateEnum UserState
        {
            get
            {
                if (this.kinectDisplay.GetKinectAction.UserCurrentState.ContainsKey(this.kinectDisplay.UserNowID)) return this.kinectDisplay.GetKinectAction.UserCurrentState[this.kinectDisplay.UserNowID];
                else return UserStateEnum.NewUser;
            }     
        }

        //�Ǧ^�O�_����착�[���A
        public bool Tracking
        {
            get
            {

                if (this.kinectDisplay.GetKinectAction.UserCurrentState.ContainsKey(this.kinectDisplay.UserNowID))
                    if (this.kinectDisplay.GetKinectAction.UserCurrentState[this.kinectDisplay.UserNowID].ToString() == "Tracking")
                        return true;
                    else return false;
                else return false;
            }
        }

        public bool IsTracking
        {
            get
            {
                if (this.kinectDisplay.GetKinectAction.UserCurrentState.ContainsKey(this.kinectDisplay.UserNowID)) return true; else return false;
            }
        }

        //�Ǧ^�ثe��USER ID
        public int UserID 
        {
            get { return this.kinectDisplay.UserNowID; }
        }

        public Dictionary<int, Dictionary<SkeletonJoint, SkeletonJointPosition>> Joints 
        {
            get { return kinectDisplay.GetKinectAction.Joints; }
        }

        public SkeletonCapability SkeletonCapbility 
        {
            get { return kinectDisplay.GetKinectAction.SkeletonCapbility; }
        }

      //  public Context Context { get { return kinectDisplay.GetKinectAction.Context; } }

        //public double test {
        //    get { return kinectDisplay.OriDisplacement; }
        //}

        //public bool MoveChange { get { return kinectDisplay.OriDisplacement < 0 && kinectDisplay.OriDisplacement != 0 ? true : false; } }

        //public bool Move { get { return kinectDisplay.Move; } }

        //public bool Slash { get { return kinectDisplay.Slash; } }

        //public bool Run { get { return kinectDisplay.Run; } }

        //public bool GetRun 
        //{
        //    get { return kinectDisplay.Run; }
        //    set { kinectDisplay.Run = value; }
        //}
    }
}
