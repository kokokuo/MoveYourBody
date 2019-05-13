using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Utils;
using Game_Kinect_Project.PhysicObjects;
using JigLibX.Vehicles;
using System.Diagnostics;

//Animation
using Xclna.Xna.Animation;

using KinectModel.StateEnum;

using MrozKinect;
using MrozKinect.KinectModels;

using OpenNI;

namespace Game_Kinect_Project.GameSpace
{
    //第二個遊戲所有的規則與實作接寫入於此
    public class GameSpace2
    {
        //取得聲音
        AudioManager audio;
            
        //物理碰撞的線
        DebugDrawer debugDrawer;

        //遊戲中的視野
        Camera camera;

        //處理KINECT轉XNA座標+模型轉換
        MrKinect kinect;
        KinectSkeleton skeleton;
        Effect kinectEffect;
        MrozKinect.KinectModels.KinectModel modelK;

        //地圖
        Model level;

        //TEST
        public InputManager test { get { return input; } }

        //後製畫面
        BloomComponent bloom;

        /// <summary>
        /// 管理整個3D遊戲
        /// </summary>
        GameComponentCollection Components;
        Game1 game;
        public GameSpace2(Game1 game, GameComponentCollection Components,AudioManager audio,Camera camera)
        {
            //聲音
            this.audio = audio;
            this.camera = camera;

            //抓XNA framework的東西
            this.game = game;
            this.Components = Components;

            //碰撞線
            debugDrawer = new DebugDrawer(game);
            debugDrawer.Enabled = false;
            Components.Add(debugDrawer);

            debugDrawer.DrawOrder = 10;
        }

        /// <summary>
        /// DebugDrawer
        /// </summary>
        public DebugDrawer DebugDrawer
        {
            get { return debugDrawer; }
        }

        /// <summary>
        /// 攝影機
        /// </summary>
        public Camera Camera
        {
            get { return camera; }
        }

        SphereObject Head;//光頭王測試
        //給予關節碰撞點
        SphereObject Neck,
        LeftShoulder,
        LeftElbow,
        LeftHand,
        RightShoulder,
        RightElbow,
        RightHand,
        LeftHip,
        LeftKnee,
        LeftFoot,
        RightHip,
        RightKnee,
        RightFoot;

        List<int> UserScore2 = new List<int>();
        public List<int> GetUserScore2 { get { return UserScore2; } }

        //關節點的模型
        Model sphereModel;
        TriangleMeshObject com;
        //儲存關節點的LIST
        List<SphereObject> BoneCollision = new List<SphereObject>();
        
        //人形牆
        Model wallModel;
        //儲存每個人形牆
        List<Model> wallModelList = new List<Model>();

        public int coll = 0;

        /// <summary>
        /// 讀取遊戲中的資源
        /// </summary>
        public void LoadFiles(ContentManager content)
        {
            camera.Position = new Vector3(1.096f, 16.6f, 48.32f);
            camera.Angles = new Vector2(-0.3853667f, 0.01183f);
            GameOptions.passCount = 0;
                         
            level = content.Load<Model>("level/level1");
            
            Model groundModel = content.Load<Model>("animation/flat2");
            for (int i = 0; i < 8; i++)
            {
                wallModel = content.Load<Model>("wall/wall" + (i + 1));
                wallModelList.Add(wallModel);
                com = new TriangleMeshObject(game, wallModel, Matrix.Identity, new Vector3(-10, -15, -210));
                Components.Add(com);
                testC.Add(com);
            }

            for (int i = 0; i < BoneCollision.Count; i++)
                BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn -= handleCollisionDetection2;


                //TriangleMeshObject com2 = new TriangleMeshObject(game, wallModel, Matrix.Identity, new Vector3(-10, -15, -160));
                //Components.Add(com2);
                //testC.Add(com2);

                //TriangleMeshObject com3 = new TriangleMeshObject(game, wallModel, Matrix.Identity, new Vector3(-10, -15, -200));
                //Components.Add(com3);
                //testC.Add(com3);

            Model terrainModel = content.Load<Model>("terr");
            HeightmapObject heightmapObj = new HeightmapObject(game, terrainModel, Vector2.Zero);
            heightmapObj.drawing = false;
            this.Components.Add(heightmapObj);

            kinectEffect = content.Load<Effect>("kinectBody/KinectEffect");

            kinect = new MrKinect(game);
          //  kinect.StartKinect(input.Depth);

            skeleton = new KinectSkeleton(kinect, game);
            Components.Add(skeleton);
            modelK = new DudeModel(game, @"kinectBody/dude", skeleton);

            sphereModel = content.Load<Model>("sphere");

            //光頭王
            //Vector3 head = new Vector3(skeleton.GetPosition(MrJoints.Head).X * 0.01f, skeleton.GetPosition(MrJoints.Head).Y * 0.01f, skeleton.GetPosition(MrJoints.Head).Z * 0.01f);
            Head = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            Neck = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);

            LeftShoulder = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            LeftElbow = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            LeftHand = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            RightShoulder = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            RightElbow = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            RightHand = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            LeftHip = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            LeftKnee = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            LeftFoot = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            RightHip = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            RightKnee = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            RightFoot = new SphereObject(game, sphereModel, 0.2f, Matrix.Identity, Vector3.One);
            //new SphereObject(this, sphereModel, 0.5f, ori, pos);

            this.Components.Add(Head);
            this.Components.Add(Neck);
            this.Components.Add(LeftShoulder);
            this.Components.Add(LeftElbow);
            this.Components.Add(LeftHand);
            this.Components.Add(RightShoulder);
            this.Components.Add(RightElbow);
            this.Components.Add(RightHand);
            this.Components.Add(LeftHip);
            this.Components.Add(LeftKnee);
            this.Components.Add(LeftFoot);
            this.Components.Add(RightHip);
            this.Components.Add(RightKnee);
            this.Components.Add(RightFoot);

            this.BoneCollision.Add(Head);
            this.BoneCollision.Add(Neck);
            this.BoneCollision.Add(LeftShoulder);
            this.BoneCollision.Add(LeftElbow);
            this.BoneCollision.Add(LeftHand);
            this.BoneCollision.Add(RightShoulder);
            this.BoneCollision.Add(RightElbow);
            this.BoneCollision.Add(RightHand);
            this.BoneCollision.Add(LeftHip);
            this.BoneCollision.Add(LeftKnee);
            this.BoneCollision.Add(LeftFoot);
            this.BoneCollision.Add(RightHip);
            this.BoneCollision.Add(RightKnee);
            this.BoneCollision.Add(RightFoot);

            CharacterObject character = new CharacterObject(game, new Vector3(0, 0, 220));
            this.Components.Add(character);

            boxObj0 = new BoxObject(game, sphereModel, new Vector3(1, 1f, 3), Matrix.Identity, new Vector3(2f, 1f - 14, 15));

            this.Components.Add(boxObj0);

            for (int i = 0; i < BoneCollision.Count; i++)
                BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection2);
            for (int i = 0; i < BoneCollision.Count; i++)
                BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection3);
            for (int i = 0; i < BoneCollision.Count; i++)
                BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection4);


            ReSet();

            audio.PlayGM2BGM();

            GameOptions.isShowPic = false;
        }
        BoxObject boxObj0;

        /// <summary>
        /// Unload game files
        /// </summary>
        public void UnloadFiles()
        {
            audio.StopGM2BGM();
        }

        public void ClearComponents()
        {
        
            List<PhysicObject> toBeRemoved = new List<PhysicObject>();
            foreach (GameComponent gc in this.Components)
            {
                if (gc is PhysicObject && !(gc is HeightmapObject)
                    && !(gc is CarObject) && (gc is TriangleMeshObject)
                    && !(gc is PlaneObject))
                {
                    PhysicObject physObj = gc as PhysicObject;
                    TriangleMeshObject triangleObj = gc as TriangleMeshObject;
                    toBeRemoved.Add(physObj);
                    toBeRemoved.Add(triangleObj);
                }
            }

            //foreach (GameComponent gc in this.Components)
            //{
            //    if (gc is SphereObject)
            //    {
            //        SphereObject physObj = gc as SphereObject;                 
            //        toBeRemoved.Add(physObj);
            //    }
            //}

            foreach (SphereObject physObj in BoneCollision)
            {
                physObj.drawing = false;
                physObj.PhysicsBody.DisableBody();
                this.Components.Remove(physObj);
                
                physObj.Dispose();
            }

            //把障礙物的skin移除
            foreach (TriangleMeshObject triangleObj in toBeRemoved)
            {
                triangleObj.drawing = false;
                triangleObj.PhysicsSkin.CollisionSystem.RemoveCollisionSkin(triangleObj.PhysicsSkin);
                this.Components.Remove(triangleObj);
                testC.Remove(triangleObj);
                triangleObj.Dispose();
            }

            for (int i = this.BoneCollision.Count - 1; i >= 0; i--)
            {
                this.BoneCollision.RemoveAt(i);
            }

            //int count = physicSystem.Controllers.Count;
            //for (int i = 0; i < count; i++) physicSystem.Controllers[0].DisableController();
            //count = physicSystem.Constraints.Count;
            //for (int i = 0; i < count; i++) physicSystem.RemoveConstraint(physicSystem.Constraints[0]);

            
        }

        //碰撞事件
        bool isColl = false;
        int count = 0;
        public bool handleCollisionDetection2(CollisionSkin owner, CollisionSkin collidee)
        {
            //狀道障礙物
            for (int i = 0; i < testC.Count; i++)
            {
                if (collidee.Equals(testC[i].PhysicsSkin))
                {
                    isColl = true;
                    if (count >= 10)
                        count = 0;

                    
                    for (int j = 0; j < BoneCollision.Count; j++)
                    {
                        if (BoneCollision[j].PhysicsBody.CollisionSkin.Equals( collidee))
                        {
                            
                        }
                    }

                    return true;
                }
            }

            
            //if (collidee.Equals(testC[1].PhysicsSkin))
            //{
            //    isColl = true;
            //    if (count >= 10)
            //        count = 0;
            //    return true;
            //}
            //if (collidee.Equals(testC[1].PhysicsSkin))
            //{
            //    isColl = true;
            //    if (count >= 10)
            //        count = 0;
            //    return true;
            //}
            //其他物件
            return true;
        }

        public bool handleCollisionDetection3(CollisionSkin owner, CollisionSkin collidee)
        {
            //狀道障礙物
            for (int i = 0; i < testC.Count; i++)
                if (collidee.Equals(testC[0].PhysicsSkin))
                {
                    isColl = true;
                    if (count >= 10)
                        count = 0;
                    return true;
                }
            //其他物件
            return true;
        }

        public bool handleCollisionDetection4(CollisionSkin owner, CollisionSkin collidee)
        {
            //狀道障礙物
            for (int i = 0; i < testC.Count; i++)
                if (collidee.Equals(testC[0].PhysicsSkin))
                {
                    isColl = true;
                    if (count >= 10)
                        count = 0;
                    return true;
                }
            //其他物件
            return true;
        }

        InputManager input;
        public void GetInput(InputManager input)
        {
            this.input = input;
        }

        GraphicsDevice gd;
        public void GetGraphicsDevice(GraphicsDevice gd)
        {
            this.gd = gd;
        }

        int gameTimeCount = 0;
        int wallQueue = -1;

        public int GameTimeCount { get { return gameTimeCount; } set { gameTimeCount = value; } }
        public int WallQueue { get { return testC.Count; } }
      //  public DepthGenerator Depth { get { return input.Depth; } }
        public Context Context { get { return null; } }

        TriangleMeshObject nowWall = null;
    
        int queueFlag = 500;
        int imageCounter = 0;
        public float x = 0f, y = 0f;
        public void Update(float elapsedTime)
        {
            //GAmeTime
            GameOptions.isCatchPic = false;
            gameTimeCount++;
            imageCounter++;
            if (imageCounter % 300 == 0 && gameTimeCount > 500 && imageCounter < 400)
            {
                GameOptions.isShowPic = true;
            }
            if (imageCounter % 300 == 0 && gameTimeCount > 500)
            {
                audio.PlayAudio("camera");
            }
            if (imageCounter % 200 == 0 && gameTimeCount > 500 && imageCounter < 250)
            {
                GameOptions.isCatchPic = true;
            }
            if (gameTimeCount % queueFlag == 0)
            {
                lockPass = false;
                GameOptions.isPass = false;
                GameOptions.isBodyPass = false;
                playPassAudio = true;
                wallQueue++;
                isColl = false;
                GameOptions.isShowPic = false;
                imageCounter = 0;
                //for (int i = 0; i < BoneCollision.Count; i++)
                //    BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn -= handleCollisionDetection2;
                ////GameOptions.isBodyPass = false;
                //Components.Remove(testC[0]);
                //testC[0].Dispose();
                //testC.RemoveAt(0);
                //TriangleMeshObject com = new TriangleMeshObject(game, wallModel, Matrix.Identity, new Vector3(-10, -15, -100));
                //Components.Add(com);
                //testC.Add(com);
                //for (int i = 0; i < BoneCollision.Count; i++)
                //    BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection2);
                if (wallQueue > 0)
                    Components.Remove(nowWall);
                if (wallQueue < 8)
                {
                    nowWall = new TriangleMeshObject(game, wallModelList[wallQueue], Matrix.Identity, Vector3.One);
                }
                if (!Components.Contains(nowWall))
                    Components.Add(nowWall);

                if (wallQueue == 8)
                {
                    ClearComponents();

                    gameTimeCount = 0;
                    wallQueue = -1;
                    imageCounter = 0;
                    UserScore2.Add(GameOptions.passCount);
                    GameOptions.GM2_Over = true;                  
                }
            }
            if (nowWall != null)
            {

                nowWall.view = Matrix.CreateLookAt(new Vector3(0, 0, 250.0f), new Vector3(-92,-67,0), Vector3.Up);
                nowWall.projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f, 800.0f / 600.0f, 1.0f, 1000.0f);
                //Vector3 temp = camera.Position;
                //temp.Z -= 50;
                //temp.Y -= 20;
                //temp.X -= 10;
                //nowWall.PhysicsBody.Position = temp;
                nowWall.PhysicsBody.Orientation *= Matrix.CreateRotationY(0.02f);
                
            }

            

            if (gameTimeCount == 400) { audio.PlayAudio("gos"); }
            
            //deBug線
            debugDrawer.Enabled = input.IsKeyDown(0, Keys.C);

            if (input.IsKeyDown(0, Keys.R)) ReSet();

            KeyboardState k = Keyboard.GetState();

            if (k.IsKeyDown(Keys.Up)) 
            {
                x++;
            }
            if (k.IsKeyDown(Keys.Down))
            {
                x--;
            }
            if (k.IsKeyDown(Keys.Left))
            {
                y++;
            }
            if (k.IsKeyDown(Keys.Right))
            {
                y--;
            }

            if (k.IsKeyDown(Keys.Escape) || GameOptions.isQuit)
            {
                ClearComponents();
                gameTimeCount = 0;
                wallQueue = -1;
                imageCounter = 0;             
            }

            //kinect.GetKinectData();

            //人物的UPDATE(KINECT TO XNA)
            skeleton.UpdateDate(input.Users, input.Joints, input.UserID, input.SkeletonCapbility, input.IsTracking,input.Tracking);
            
            //gameSpace1.Update(elapsedTime);
            //if (input != null)
            //if (input.IsTracking)
            //{

            //牆
            if (wallQueue >= 0)
            {
                // for (int i = 0; i < testC.Count; i++)
                //{
                Vector3 v = testC[wallQueue].PhysicsBody.Position;
                v.Z += 1f;
                testC[wallQueue].PhysicsBody.Velocity = v;
                testC[wallQueue].SetApplyLocalTransform(new Vector3(0, 0, 1f));
                testC[wallQueue].PhysicsBody.MoveTo(v, Matrix.Identity);

                if (testC[wallQueue].PhysicsBody.Position.Z > Neck.PhysicsBody.Position.Z)
                {
                    GameOptions.isPass = true;
                }
                if (testC[wallQueue].PhysicsBody.Position.Z >= Neck.PhysicsBody.Position.Z + 40)
                {
                    GameOptions.isPass = false;
                }
                if (!GameOptions.isBodyPass && GameOptions.isPass && !isColl)
                {
                    if (playPassAudio)
                    {
                        audio.PlayAudio("sectionpass");
                        GameOptions.passCount++;
                    }
                    playPassAudio = false;
                    lockPass = true;
                    
                }
                //}
            }
           // }

            //晃動相機
            
            //posCounter++;
            //if (posCounter < 30) 
            //{
            //    tempPos.X += 0.1f;
                
            //}
            //if (posCounter >= 30) 
            //{
            //    tempPos.X -= 0.1f;
            //}
            //if (posCounter == 60)
            //    posCounter = 0;
            //camera.Position = tempPos;

        }
        Vector3 tempPos = new Vector3(1.096f, 16.6f, 48.32f);
        int posCounter = 0;
        bool lockPass = false;
        bool playPassAudio = true;
        //public TriangleMeshObject TestCC { get { return testC[wallQueue]; } }
        List<TriangleMeshObject> testC = new List<TriangleMeshObject>();

        public int ID { get {return input!=null?input.UserID:50000; } }

        public void ReSet()
        {
            GameOptions.isPass = false;
            GameOptions.isBodyPass = false;
            gameTimeCount = 0;
            wallQueue = -1;

            for (int i = 0; i < BoneCollision.Count; i++)
                BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn -= handleCollisionDetection2;

            for (int i = 7; i >= 0; i--)
            {
                Components.Remove(testC[i]);
                testC[i].Dispose();
                testC.RemoveAt(i);
            }

            for (int i = 0; i < 8; i++)
            {
                com = new TriangleMeshObject(game, wallModelList[i], Matrix.Identity, new Vector3(-10, -15, -210));
                Components.Add(com);
                testC.Add(com);
            }

            for (int i = 0; i < BoneCollision.Count; i++)
                BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection2);

            //gameSpace1.ResetScene();

            //if (testC.Count <= 1)
            //{
            //    TriangleMeshObject com = new TriangleMeshObject(game, wallModel, Matrix.Identity, new Vector3(-10, -15, -100));
            //    Components.Add(com);
            //    for (int i = 0; i < BoneCollision.Count; i++)
            //        BoneCollision[i].PhysicsBody.CollisionSkin.callbackFn += new CollisionCallbackFn(handleCollisionDetection3); ;
            //    testC.Add(com);
            //}           
        }

        public Vector3 ScalePos(MrJoints joint)
        {
            return new Vector3(skeleton.GetPosition(joint).X * 0.01f - 1, skeleton.GetPosition(joint).Y * 0.01f, skeleton.GetPosition(joint).Z * 0.01f);
        }

        public void Draw(GraphicsDevice gd)
        {
            //bloom.BeginDraw();  
            if (!lockPass)
            {
                if (isColl)
                {

                    GameOptions.isBodyPass = true;
                    count++;
                    if (count == 10)
                    {
                        //isColl = false;
                        audio.PlayAudio("sectionfail");
                        //GameOptions.isBodyPass = false;
                    }
                }
            }
            if (isColl)
            {
                gd.Clear(Color.Azure);
            }
            else
                gd.Clear(Color.Blue);

            try
            {
                Vector3 head = new Vector3(skeleton.GetPosition(MrJoints.Head).X * 0.01f, skeleton.GetPosition(MrJoints.Head).Y * 0.01f, skeleton.GetPosition(MrJoints.Head).Z * 0.01f);
                BoneCollision[0].PhysicsBody.Position = head;
                Vector3 pos;
                pos = ScalePos(MrJoints.Neck);
                BoneCollision[1].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.LeftShoulder);
                BoneCollision[2].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.LeftElbow);
                BoneCollision[3].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.LeftHand);
                BoneCollision[4].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.RightShoulder);
                BoneCollision[5].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.RightElbow);
                BoneCollision[6].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.RightHand);
                BoneCollision[7].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.LeftHip);
                BoneCollision[8].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.LeftKnee);
                BoneCollision[9].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.LeftFoot);
                BoneCollision[10].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.RightHip);
                BoneCollision[11].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.RightKnee);
                BoneCollision[12].PhysicsBody.MoveTo(pos, Matrix.Identity);
                pos = ScalePos(MrJoints.RightFoot);
                BoneCollision[13].PhysicsBody.MoveTo(pos, Matrix.Identity);
            }
            catch
            {
            }

            //畫出背景地圖
            Matrix world = Matrix.CreateTranslation(-100.3f, -10.0f, 0.0f) * Matrix.CreateScale(0.01f);
            //world = Matrix.Identity;
            
            Vector3 posi = new Vector3(250f, 70.0f, 0);
            Matrix gameWorldRotation = Matrix.CreateWorld(posi, Vector3.Forward, Vector3.Up);
            Matrix[] transforms = new Matrix[level.Bones.Count];
            //float aspectRatio = gd.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height; 
            level.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in level.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //使用基本光源打光 
                    effect.EnableDefaultLighting();
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = gameWorldRotation * transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(Vector3.One) * Matrix.CreateScale(0.1f, 0.1f, 0.1f) * Matrix.CreateRotationY(1.57f);
                    
                    effect.FogEnabled = true;
                    effect.FogStart = 200;
                    effect.FogEnd = 300;
                    effect.FogColor = Color.White.ToVector3();
                }
                mesh.Draw();
            }

            for (int i = 0; i < testC.Count; i++)
            {
                testC[i].draww();
                //testC[i].isFrog = true;
            }

            KeyboardState keyState = Keyboard.GetState();                      

            if (keyState.IsKeyDown(Keys.M))
            for (int i = 0; i < BoneCollision.Count; i++)
            {
                BoneCollision[i].draww();
            }

            else
                modelK.Draw(world, camera.Projection, camera.View, game.GetGameTime);

            if (nowWall != null)
            {
                nowWall.isCameraView = false;
                nowWall.draww();                
            }
        }

        #region IDisposable Members

        bool isDisposed = false;
        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }

        #endregion

        #region ImmovableSkinPredicate
        class ImmovableSkinPredicate : CollisionSkinPredicate1
        {
            public override bool ConsiderSkin(CollisionSkin skin0)
            {
                if (skin0.Owner != null && !skin0.Owner.Immovable && !(skin0.Owner is Character))
                    return true;
                else
                    return false;
            }
        }
        #endregion

    }
}

