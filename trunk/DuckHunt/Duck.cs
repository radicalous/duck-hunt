using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;


using GameCommon;

namespace DuckHuntCommon 
{
    enum ModelType { NONE, SKY, GRASS, DUCK, DOG, BULLET, HITBOARD,
        DUCKICON, BULLETBOARD, BULLETICON, SCOREBOARD};
    
    enum ResourceType { TEXTURE, SOUND, FONT };
    class ResourceItem
    {
        public ResourceType type;
        public string path;
    }


    interface ModelObject
    {
        ModelType Type();

        void Initialize(ModelObject parent, Rectangle rect, int seed); // Rect is the rect range based on parent object
        void Update(GameTime gameTime);

        List<ResourceItem> GetResourceList();
        //Vector2 GetAbsolutePosition();  // position is the center of the object based on the parent object
        Vector2 GetAbsolutePosition(); // the lefttop cornor
        Rectangle GetSpace();   // space is the rect the object may cover, the lefttop is Zero
        float GetSacle();       // scale is the scale to scale textures 


        List<AnimationInfo> GetAnimationInfoList(); // this is the animation information, include itself and it's children's
        int GetCurrentAnimationIndex(); // current animation index
        float GetAnimationDepth();      // animation depth


        ModelObject GetParentObject();              
        List<ModelObject> GetChildrenObjects();

        // assist function, improve performance
        ViewObject GetViewObject();
        void SetViewObject(ViewObject viewObject);

    }

    class ViewItem
    {
        public Animation animation;
        public StaticBackground staticBackground;
    }

    interface ViewObject
    {
        void Init(ModelObject model, Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }

    class CommonViewObject: ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;

        ModelObject model;
        List<ViewObject> childViewObjectList;

        public CommonViewObject(ModelObject model1)
        {
            model = model1;
            List<ModelObject> childobjlst = model.GetChildrenObjects();
            if (childobjlst != null)
            {
                childViewObjectList = new List<ViewObject>();
                foreach (ModelObject obj in childobjlst)
                {
                    ViewObject viewobj = ViewObjectFactory.CreateViewObject(obj);
                    childViewObjectList.Add(viewobj);
                }
            }
        }

        public void Init(ModelObject model1, Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            // try to calculate how may textures are needed by children

            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();
            viewItmList = new List<ViewItem>();
            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = model.GetAnimationInfoList()[i];
                ViewItem viewItm = new ViewItem();
                if (animationInfo.animation)
                {
                    viewItm.animation = new Animation();
                    viewItm.animation.Initialize(
                        texturesList[i],
                        Vector2.Zero, animationInfo.frameWidth, animationInfo.frameHeight,
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);
                }
                else
                {
                    viewItm.staticBackground = new StaticBackground();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        space.Width, space.Height, 0);
                }
                viewItmList.Add(viewItm);
            }
            //play init sound
            if (objTextureLst[model.Type()].soundList.Count > 0)
            {//SoundEffect.MasterVolume
                //SoundEffect.
                //SoundEf
                float mastvol = SoundEffect.MasterVolume;
                objTextureLst[model.Type()].soundList[0].Play(1,0, 0);
            }
            

            
            // left textures are for children
            if (childViewObjectList != null)
            {
                foreach (ViewObject childviewobj in childViewObjectList)
                {
                    Rectangle rc = new Rectangle();
                    childviewobj.Init(null, objTextureLst, rc);
                }
                
            }
        }

        // local rect, global rect
        // (local rect - orgpoint ) = global rect * default scale
        // local rect = orgpoint + global rect * default scale
        //

        public void Update(GameTime gameTime)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = model.GetAbsolutePosition();
                viewItm.animation.Position.X += viewItm.animation.FrameWidth / 2;
                viewItm.animation.Position.Y += viewItm.animation.FrameHeight / 2;
                viewItm.animation.scale = model.GetSacle();
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
            }

            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Update(gameTime);
                }
            }

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            }
            else
            {
                viewItm.staticBackground.Draw(spriteBatch, model.GetAnimationDepth());
            }
            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Draw(spriteBatch);
                }
            }
        }
    }

    // draw the score myself
    class ScoreBoardViewObject : ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        ScroeBoardModel model;
        List<SpriteFont> fontList;

        Vector2 scoreposition;

        public ScoreBoardViewObject(ModelObject model1)
        {
            model = (ScroeBoardModel)model1;
        }

        public void Init(ModelObject model1, Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (ScroeBoardModel)model1;

            fontList = objTextureLst[model.Type()].fontList;


            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            animationList = model.GetAnimationInfoList();

            // background
            viewItmList = new List<ViewItem>();
            for (int i = 0; i < texturesList.Count; i++)
            {
                AnimationInfo animationInfo = model.GetAnimationInfoList()[i];
                ViewItem viewItm = new ViewItem();
                if (animationInfo.animation)
                {
                    viewItm.animation = new Animation();
                    viewItm.animation.Initialize(
                        texturesList[i],
                        Vector2.Zero, animationInfo.frameWidth, animationInfo.frameHeight,
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);
                }
                else
                {
                    viewItm.staticBackground = new StaticBackground();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        space.Width, space.Height, 0);
                }
                viewItmList.Add(viewItm);
            }
        }

        public void Update(GameTime gameTime)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = model.GetAbsolutePosition();
                viewItm.animation.Position.X += viewItm.animation.FrameWidth / 2;
                viewItm.animation.Position.Y += viewItm.animation.FrameHeight / 2;
                viewItm.animation.scale = model.GetSacle();
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
            }

            scoreposition = model.GetAbsolutePosition();
            scoreposition.X += 20;
            scoreposition.Y += 25;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            }
            else
            {
                viewItm.staticBackground.Draw(spriteBatch, model.GetAnimationDepth());
            }

            // draw score

            Vector2 pos1 = scoreposition;
            pos1.Y -= 10;
            string value = this.model.TotalScore.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(fontList[0], "SCORE:" + value, pos1, Color.White, 0, Vector2.Zero, 1, 
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }



    class SkyModel : ModelObject
    {
        ModelObject parent = null;
        string texturesPath = "Graphics\\sky";
        float Depth
        {
            get { return 1.0F; }
        }

        // Animation representing the player
        List<AnimationInfo> AnimationTexturesList
        {
            get
            {
                List<AnimationInfo> anationInfoList;
                anationInfoList = new List<AnimationInfo>();
                AnimationInfo animationInfo = new AnimationInfo();
                animationInfo.animation = false;
                animationInfo.texturesPath = texturesPath;
                anationInfoList.Add(animationInfo);
                return anationInfoList;
            }
        }

        Vector2 relativePos;
        public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            parent = null;
            space = rect;
            relativePos.X = rect.Left;
            relativePos.Y = rect.Top;
            space.Offset(-space.Left, -space.Y);
        }

        public void Update(GameTime gameTime)
        {
            // no animation
        }

        public ModelType Type()
        {
            // sky 

            return ModelType.SKY;
        }

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky";
            resourceList.Add(resourceItm);
            return resourceList;
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return AnimationTexturesList;
        }
        public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        public float GetAnimationDepth()
        {
            return Depth;
        }

        Rectangle space;

        public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePos;
            if (parent!=null)
            {
                absPos += parent.GetAbsolutePosition();
            }
            return absPos;
        }
        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1.0f;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }


        public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }

        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }


    class GrassModel : ModelObject
    {
        ModelObject parent = null;
        string texturesPath = "Graphics\\duckForest_2";

        float Depth
        {
            get { return 0.5F; }
        }

        public string Textures()
        {
            return texturesPath;
        }

        // Animation representing the player
        List<AnimationInfo> AnimationTexturesList
        {
            get
            {
                List<AnimationInfo> anationInfoList;
                anationInfoList = new List<AnimationInfo>();
                AnimationInfo animationInfo = new AnimationInfo();
                animationInfo.animation = false;
                animationInfo.texturesPath = texturesPath;
                anationInfoList.Add(animationInfo);
                return anationInfoList;
            }
        }


        Vector2 relativePos;
        public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            parent = null;
            space = rect;
            relativePos.X = rect.Left;
            relativePos.Y = rect.Top;
            space.Offset(-space.Left, -space.Y);
        }


        public void Update(GameTime gameTime)
        {
        }

        public ModelType Type()
        {
            // sky 

            return ModelType.GRASS;
        }

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duckForest_2";
            resourceList.Add(resourceItm);
            return resourceList;
        }
        public List<AnimationInfo> GetAnimationInfoList()
        {
            return AnimationTexturesList;
        }
        public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        public float GetAnimationDepth()
        {
            return Depth;
        }

        Rectangle space;
        public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePos;
            if (parent != null)
            {
                absPos += parent.GetAbsolutePosition();
            }
            return absPos;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1.0f;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }
        public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }

        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }

 
    class AnimationInfo
    {
        public bool fontTexture = false;
        public bool animation = true;
        public string texturesPath;

        public int frameWidth = 115;
        public int frameHeight = 69;
        public int frameCount = 8;
        public int frameTime = 30;
        public Color backColor = Color.White;
    }


    class DuckModel : ModelObject
    {
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        float scale = 1.0f;
        float depth = 0.6f;
        float Depth
        {
            get { return depth; }
        }

        List<AnimationInfo> AnimationTexturesList
        {
            get
            {
                return anationInfoList;
            }
        }

        int AnimationIndex
        {
            get
            {
                if(dead)
                {
                    if (deadstopcount < 10)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    if (autoPilot.HorizationDirection == AutoPilot.Direction.LEFT)
                    {
                        if (autoPilot.ZDirection == AutoPilot.Direction.IN)
                        {
                            return 0;
                        }
                        else if (autoPilot.ZDirection == AutoPilot.Direction.OUT)
                        {
                            return 0;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (autoPilot.ZDirection == AutoPilot.Direction.IN)
                        {
                            return 3;
                        }
                        else if (autoPilot.ZDirection == AutoPilot.Direction.OUT)
                        {
                            return 3;
                        }
                        else 
                        {
                            return 3;
                        }
                    }
                }
            }
        }   


        Vector2 RelativePosition
        {
            get 
            {
                if (autoPilot == null)
                {
                    return Vector2.Zero;
                }

                if (Active)
                    return autoPilot.Position;
                else
                    return deadPilot.Position;
            }
        }

        AutoPilot autoPilot;
        DeadPilot deadPilot;

        // State of the player
        public bool Active = true;
        public bool dead = false;

        public bool Gone = false;

        // Amount of hit points that player has
        public int Health;

        int deadstopcount = 0;

        public DuckModel()
        {
            //
            autoPilot = new AutoPilot();
            anationInfoList = new List<AnimationInfo>();

            // 0. flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\duck_black_flying";
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\duck_black_shot";
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\duck_black_dead";
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\duck_black_flying_r";
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);
        }

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_black_flying";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_black_shot";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_black_dead";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_black_flying_r";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public void Shoot(BulletModel bullet)
        {
            // check if it's shoot
            if (Active == false)
            {
                return;
            }

            Vector2 position = bullet.GetAbsolutePosition();
            Rectangle bulletRc = bullet.GetSpace();
            Vector2 bulletCenter = position;
            bulletCenter.X += bulletRc.Width / 2;
            bulletCenter.Y += bulletRc.Height / 2;

            Vector2 duckCenter = RelativePosition;
            if (parent != null)
            {
                duckCenter += parent.GetAbsolutePosition();
            }
            //
            duckCenter.X += anationInfoList[AnimationIndex].frameWidth / 2 ;
            duckCenter.Y += anationInfoList[AnimationIndex].frameHeight / 2 ;

            Vector2 subpos = bulletCenter - duckCenter;
            if (subpos.Length() < 20*scale)
            {
                Active = false;
                dead = true;
                deadPilot = new DeadPilot(this.autoPilot, false);

                // new a bullet  
                bullet.SetTarget(this);
            }

        }

        Rectangle duckspace;

        int randomseed = 0;
        ModelObject parent = null;
        public void Initialize(ModelObject parent1, Rectangle duckSpace, int seed)
        {
            parent = null;

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
            randomseed = seed;
            Random radom = new Random(seed);


        }

        public void StartPilot(Rectangle duckFlySpace)
        {
            // Set the starting position of the player around the middle of the screen and to the back
            autoPilot.Initialize(duckFlySpace, randomseed);
            Rectangle startSpace = new Rectangle(duckFlySpace.X, duckFlySpace.Y + (int)(0.9 * duckFlySpace.Height),
                duckFlySpace.Width, (int)(duckFlySpace.Height * 0.1));
            autoPilot.LeadDirection(AutoPilot.Direction.RANDOM, AutoPilot.Direction.UP);
            autoPilot.RadomStartPos(startSpace);
        }


        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                autoPilot.Update(gameTime);

                // check if it need to go
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime > 1000 * 10)
                {
                    Active = false;
                    deadPilot = new DeadPilot(this.autoPilot, true);
                }

            }
            else
            {
                if (deadstopcount < 10)
                {
                    deadstopcount++;
                }
                deadPilot.Update(gameTime);
                if (deadPilot.Position.Y > autoPilot.boundaryRect.Bottom ||
                    deadPilot.Position.Y < autoPilot.boundaryRect.Top - anationInfoList[AnimationIndex].frameHeight)
                {
                    Gone = true;
                }
            }

        }

        public ModelType Type()
        {
            // sky 

            return ModelType.DUCK;
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return AnimationTexturesList;
        }
        public int GetCurrentAnimationIndex()
        {
            return AnimationIndex;
        }

        public float GetAnimationDepth()
        {
            return Depth;
        }


        Rectangle space;

        public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = RelativePosition;
            if (parent != null)
            {
                absPos += parent.GetAbsolutePosition();
            }
            return absPos;
        }
        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            if (Active)
            {
                // get depth, calculate the scale

                //scale = autoPilot.scale;
                scale = 1 - autoPilot.depthpos/100; 
            }

            return scale;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }
        public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }

        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }



    class DogModel : ModelObject
    {
        // Animation representing the player
        public string animationTexturesPath = "Graphics\\dogs";

        List<AnimationInfo> anationInfoList;
        Rectangle dogspace;

        bool gone = false;

        public bool Gone
        {
            get { return gone; }
        }

        float depth = 0.4F;

        int deadDuck = 0;


        int AnimationIndex
        {
            get
            {
                if (state == DOGSTATE.FindingDuck)
                {
                    if (midseekstopcount > 0 && midseekstopcount < midseekmaxstoptime)
                    {
                        // stop animation
                        return 1;
                    }
                    else if (foundstopcount > 0)
                    {
                        return 2;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (state == DOGSTATE.Jumping)
                {
                    if (jumpup)
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }
                else if (state == DOGSTATE.Showing)
                {
                    if (deadDuck == 0)
                    {
                        // laughing
                        return 5;
                    }
                    else if (deadDuck == 1)
                    {
                        // show 1
                        return 6;
                    }
                    else if (deadDuck == 2)
                    {
                        return 6;
                    }

                }
                return 0;
            }
        } 

        float Depth
        {
            get { return depth; }
        }

        Vector2 Position
        {
            get
            {
                if (state == DOGSTATE.FindingDuck)
                {
                    //
                    return pilot.Position;
                }
                else if (state == DOGSTATE.Jumping)
                {
                    return jumpPilot.Position;
                }
                else if (state == DOGSTATE.Showing)
                {
                    return showPilot.Position;
                }
                else
                {
                    return pilot.Position;
                }
            }
        }

        DogPilot pilot;
        int foundmaxstoptime = 50;
        int midseekmaxstoptime = 100;
        int midseekstopcount = 0;
        int foundstopcount = 0;

        DogJumpPilot jumpPilot;
        bool jumpup = true;

        DogShowPilot showPilot;

        enum DOGSTATE { FindingDuck, Jumping, Showing };
        DOGSTATE state = DOGSTATE.FindingDuck;



        public DogModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\dogs";
            animationInfo.frameWidth = 142;
            animationInfo.frameHeight = 168;
            animationInfo.frameCount = 6;
            animationInfo.frameTime = 150;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\dogseeking";
            animationInfo.frameWidth = 147;
            animationInfo.frameHeight = 168;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\dogfound";
            animationInfo.frameWidth = 154;
            animationInfo.frameHeight = 168;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 30000;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\dogjumpup";
            animationInfo.frameWidth = 132;
            animationInfo.frameHeight = 168;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 30000;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\dogjumpdown";
            animationInfo.frameWidth = 116;
            animationInfo.frameHeight = 105;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 30000;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\doglaugh";
            animationInfo.frameWidth = 103;
            animationInfo.frameHeight = 130;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 150;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\dogshowduck1";
            animationInfo.frameWidth = 108;
            animationInfo.frameHeight = 130;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\dogshowduck1";
            animationInfo.frameWidth = 108;
            animationInfo.frameHeight = 130;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);


            pilot = new DogPilot();
        }


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\dogs";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\dogseeking";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\dogfound";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\dogjumpup";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\dogjumpdown";
            resourceList.Add(resourceItm);
            
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\doglaugh";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\dogshowduck1";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\dogshowduck1";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public void ShowDog(int deadduck)
        {
            state = DOGSTATE.Showing;
            showPilot = new DogShowPilot(pilot);

            deadDuck = deadduck;

            gone = false;
        }

        ModelObject parent = null;

        public void Initialize(ModelObject parent1, Rectangle dogSpace, int seed)
        {
            parent = null;
            //dogspace = dogSpace;
        }

        public void StartPilot(Rectangle dogrunspace)
        {
            // Set the starting position of the player around the middle of the screen and to the back
            dogspace = dogrunspace;
            pilot.Initialize(dogrunspace);
        }


        public void Update(GameTime gameTime)
        {
            if (state == DOGSTATE.FindingDuck)
            {
                //
                if (pilot.Position.X >= dogspace.Width / 4)
                {
                    // sleep some time
                    if (midseekstopcount < midseekmaxstoptime)
                    {
                        midseekstopcount++;
                        return;
                    }
                }

                if (pilot.Position.X >= dogspace.Width / 2)
                {
                    // sleep some time
                    if (foundstopcount < foundmaxstoptime)
                    {
                        foundstopcount++;
                        return;
                    }

                    state = DOGSTATE.Jumping;
                    jumpPilot = new DogJumpPilot(pilot);
                }
                pilot.Update(gameTime);
                depth = 0.4F;
            }
            else if (state == DOGSTATE.Jumping)
            {
                jumpPilot.Update(gameTime);
                if (jumpPilot.Position.Y <= dogspace.Top)
                {
                    depth = 0.6F;
                    jumpup = false;
                }
                if (jumpPilot.Position.Y > dogspace.Bottom)
                {
                    state = DOGSTATE.Showing;
                    showPilot = new DogShowPilot(pilot);

                    gone = true;
                }
            }
            else if (state == DOGSTATE.Showing)
            {
                showPilot.Update(gameTime);
                if (showPilot.Position.Y > dogspace.Bottom)
                {
                    state = DOGSTATE.Showing;
                    showPilot = new DogShowPilot(pilot);
                    gone = true;
                }
            }

        }


        public ModelType Type()
        {
            // sky 

            return ModelType.DOG;
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        public int GetCurrentAnimationIndex()
        {
            return AnimationIndex;
        }

        public float GetAnimationDepth()
        {
            return Depth;
        }

        public Vector2 GetAbsolutePosition()
        {
            return Position;
        }
        public Rectangle GetSpace()
        {
            Rectangle space = new Rectangle(0, 0
            , anationInfoList[AnimationIndex].frameWidth
            , anationInfoList[AnimationIndex].frameHeight);

            return space;
        }
        public float GetSacle()
        {
            return 1.0f;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }
        public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }


        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }




    class BulletModel : ModelObject
    {
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        Rectangle bulletspace;

        bool gone = false;

        public bool Gone
        {
            get { return gone; }
        }

        float depth = 0.6F;

        int AnimationIndex
        {
            get
            {
                return 0;
            }
        }

        float Depth
        {
            get { return depth; }
        }

        Vector2 Position
        {
            get
            {
                return position;
            }
        }

        Vector2 position = Vector2.Zero;
        Vector2 targetposition = Vector2.Zero;
        List<DuckModel> shootduckList;


        public BulletModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\laser1";
            animationInfo.frameWidth = 47;
            animationInfo.frameHeight = 23;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            bulletspace.X = 0;
            bulletspace.Y = 0;
            bulletspace.Width = animationInfo.frameWidth;
            bulletspace.Height = animationInfo.frameHeight;

            shootduckList = new List<DuckModel>();
        }
        public BulletModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\laser1";
            animationInfo.frameWidth = 35;
            animationInfo.frameHeight = 27;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            position = position1;
            position.X -= animationInfo.frameWidth / 2;
            position.Y -= animationInfo.frameHeight / 2;
            bulletspace.X = 0;
            bulletspace.Y = 0;
            bulletspace.Width = animationInfo.frameWidth;
            bulletspace.Height = animationInfo.frameHeight;

            shootduckList = new List<DuckModel>();
        }

        float deltax = 40f; //40f;
        float deltay = -20f; //-20f;

        public List<DuckModel> GetShootDucks()
        {
            if (shootduckList.Count > 0)
            {
                return shootduckList;
            }

            return null;
        }

        public void SetTarget(DuckModel duck)
        {
            if (duck != null)
            {
                shootduckList.Add(duck);
                depth = duck.GetAnimationDepth() + 0.1f;
                //position = duck.GetAbsolutePosition();
                targetposition = position;
                position.X = position.X - 20*6 ;
                position.Y = position.Y + 10*6 ;
            }   
        }

        ModelObject parent = null;

        public void Initialize(ModelObject parent1, Rectangle space, int seed)
        {
            parent = null;

            bulletspace = new Rectangle();
            bulletspace = space;
            position.X = bulletspace.Left;
            position.Y = bulletspace.Y;

            bulletspace.X = 0;
            bulletspace.Y = 0;
            bulletspace.Width = anationInfoList[AnimationIndex].frameWidth;
            bulletspace.Height = anationInfoList[AnimationIndex].frameHeight;

            // Set the starting position of the player around the middle of the screen and to the back
        }

        float scale = 1.0f;

        public void Update(GameTime gameTime)
        {
            if (shootduckList.Count > 0)
            {

                if (position.X < targetposition.X)
                {
                    position.X += deltax;
                }
                if (position.Y > targetposition.Y)
                {
                    position.Y += deltay;
                }
                if (position.X >= targetposition.X && position.Y <= targetposition.Y)
                {
                    scale = 0;
                }


            }
            else
            {

                if (scale >= 0)
                {
                    scale -= 0.1f;
                }
            }
            if (scale <= 0)
            {
                scale = 0f;
                gone = true;
            }
        }


        public ModelType Type()
        {
            // sky 

            return ModelType.BULLET;
        }



        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\laser1";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\laserFire";
            resourceList.Add(resourceItm);

            return resourceList;
        }


        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        public int GetCurrentAnimationIndex()
        {
            return AnimationIndex;
        }

        public float GetAnimationDepth()
        {
            return Depth;
        }


        public Vector2 GetAbsolutePosition()
        {
            if (shootduckList.Count > 0)
            {
                //return shootduck.GetAbsolutePosition();
            }
            return Position;
        }
        public Rectangle GetSpace()
        {
            return bulletspace;
        }
        public float GetSacle()
        {
            return scale;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }
        public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }

        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }


    class DuckIconModel : ModelObject
    {
        public enum DuckIconState { Alive, Ongoing, Dead };
        DuckIconState state;

        List<AnimationInfo> anationInfoList;
        ModelObject parent;
        Rectangle space;
        Vector2 relativePos;

        public  DuckIconModel()
        {
            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\duckIconAlive";
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\duckIconOngoing";
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\duckIconDead";
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            state = DuckIconState.Alive;

            space.Width = 19;
            space.Height = 19;
        }


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duckIconAlive";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duckIconOngoing";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duckIconDead";
            resourceList.Add(resourceItm);


            return resourceList;
        }


        public void Initialize(ModelObject parent1, Rectangle rect, int seed)
        {
            parent = parent1;
            space = rect;
            relativePos.X = rect.Left;
            relativePos.Y = rect.Top;
            rect.Offset(-rect.Left, -rect.Top);

            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\AliveDuckIcon";
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\OngoingDuckIcon";
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\DeadDuckIcon";
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            state = DuckIconState.Alive;

            space.Width = 19;
            space.Height = 19;

        }
        public void Update(GameTime gameTime)
        {

        }
        public ModelType Type()
        {
            return ModelType.DUCKICON;
        }

        public Vector2 GetAbsolutePosition()
        {
            Vector2 absposition = relativePos;
            if (parent != null)
            {
                absposition += parent.GetAbsolutePosition();
            }
            return absposition;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1.0f;
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        public int GetCurrentAnimationIndex()
        {
            if (state == DuckIconState.Alive)
            {
                return 0;
            }
            else if (state == DuckIconState.Ongoing)
            {
                return 1;
            }
            else if (state == DuckIconState.Dead)
            {
                return 2;
            }
            return 0;
        }
        public float GetAnimationDepth()
        {
            return 0.3f;
        }

        public ModelObject GetParentObject()
        {
            return parent;
        }
        public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }

        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }

        public void SetState(DuckIconState state1)
        {
            state = state1;
        }
    }

    class HitBoardModel : ModelObject
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;

        List<DuckIconModel> duckIcons;
       
        int AnimationIndex
        {
            get
            {
                return 0;
            }
        }

        float Depth
        {
            get { return 0.35f; }
        }

        Vector2 Position
        {
            get
            {
                return position;
            }
        }

        Rectangle space; //indicate the object view range
        Vector2 position = Vector2.Zero; // no use

        public HitBoardModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // background
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\HitBoardBackground";
            animationInfo.frameWidth = 318;
            animationInfo.frameHeight = 63;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 318;
            space.Height = 63;

            duckIcons = new List<DuckIconModel>();

        }

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\HitBoardBackground";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public HitBoardModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\HitBoardBackground";
            animationInfo.frameWidth = 318;
            animationInfo.frameHeight = 63;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            position = position1;

            space.Width = 318;
            space.Height = 63;

            duckIcons = new List<DuckIconModel>();
        }
      

        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            position.X = space.Left;
            position.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }

        int duckcount = 10;
        public void LoadDuckIconsModel(int duckcount1)
        {
            duckcount = 10;
            Rectangle duckIconRc = new Rectangle();
            duckIconRc.X = 90;
            duckIconRc.Y = 12;
            for (int i = 0; i < duckcount; i++)
            {
                DuckIconModel duckIconModel = new DuckIconModel();
                duckIconModel.Initialize(this, duckIconRc, 0);
                duckIcons.Add(duckIconModel);
                duckIconRc.Offset(22, 0);
            }
        }

        public void SetDuckIconsState(int duckIndex, DuckIconModel.DuckIconState state)
        {
            if (duckIcons == null || duckIndex >= duckIcons.Count)
            {
                return;
            }
            DuckIconModel duckIcon = duckIcons[duckIndex];
            duckIcon.SetState(state);
        }

        public void Update(GameTime gameTime)
        {
            // no update for itself


            // update child object
        }


        public ModelType Type()
        {
            return ModelType.HITBOARD;
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        public int GetCurrentAnimationIndex()
        {
            return AnimationIndex;
        }

        public float GetAnimationDepth()
        {
            return Depth;
        }

        public Vector2 GetAbsolutePosition()
        {
            return Position;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }

        public List<ModelObject> GetChildrenObjects()
        {
            List<ModelObject> childrenlst = new List<ModelObject>();
            foreach (DuckIconModel child in duckIcons)
            {
                childrenlst.Add(child);
            }
            return childrenlst;
        }



        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }




    class BulletIconModel : ModelObject
    {
        List<AnimationInfo> anationInfoList;
        ModelObject parent;
        Rectangle space;
        Vector2 relativePos;

        public BulletIconModel()
        {
            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\bulletIcon";
            animationInfo.frameWidth = 10;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            space.Width = 10;
            space.Height = 19;
        }


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\bulletIcon";
            resourceList.Add(resourceItm);

            return resourceList;
        }
        public void Initialize(ModelObject parent1, Rectangle rect, int seed)
        {
            parent = parent1;
            space = rect;
            relativePos.X = rect.Left;
            relativePos.Y = rect.Top;
            rect.Offset(-rect.Left, -rect.Top);

            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\bulletIcon";
            animationInfo.frameWidth = 10;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            space.Width = 10;
            space.Height = 19;

        }
        public void Update(GameTime gameTime)
        {

        }
        public ModelType Type()
        {
            return ModelType.BULLETICON;
        }

        public Vector2 GetAbsolutePosition()
        {
            Vector2 absposition = relativePos;
            if (parent != null)
            {
                absposition += parent.GetAbsolutePosition();
            }
            return absposition;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1.0f;
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        public int GetCurrentAnimationIndex()
        {
            return 0;
        }
        public float GetAnimationDepth()
        {
            return 0.3f;
        }

        public ModelObject GetParentObject()
        {
            return parent;
        }
        public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }

        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }


    class BulletBoardModel : ModelObject
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;
        List<BulletIconModel> bulletIcons;

        int AnimationIndex
        {
            get
            {
                return 0;
            }
        }

        float Depth
        {
            get { return 0.35f; }
        }

        Vector2 Position
        {
            get
            {
                return position;
            }
        }

        Rectangle space; //indicate the object view range
        Vector2 position = Vector2.Zero; // no use

        public BulletBoardModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // background
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\BulletBoard";
            animationInfo.frameWidth = 78;
            animationInfo.frameHeight = 58;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 318;
            space.Height = 63;

            bulletIcons = new List<BulletIconModel>();

        }
        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\BulletBoard";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public BulletBoardModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\BulletBoard";
            animationInfo.frameWidth = 78;
            animationInfo.frameHeight = 58;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            position = position1;

            space.Width = 318;
            space.Height = 63;

            bulletIcons = new List<BulletIconModel>();
        }


        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            position.X = space.Left;
            position.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }

        int bulletcount = 3;
        public void LoadBullet(int count)
        {
            if (bulletIcons.Count > 0)
            {
                bulletIcons.RemoveRange(0, bulletIcons.Count - 1);
            }
            viewObject = null;
            bulletcount = count;
            Rectangle bulletIconRc = new Rectangle();
            bulletIconRc.X = 14;
            bulletIconRc.Y = 8;
            for (int i = 0; i < bulletcount; i++)
            {
                BulletIconModel bulletIconModel = new BulletIconModel();
                bulletIconModel.Initialize(this, bulletIconRc, 0);
                bulletIcons.Add(bulletIconModel);
                bulletIconRc.Offset(20, 0);
            }
        }


        public void Update(GameTime gameTime)
        {
            // no update for itself


            // update child object
        }


        public ModelType Type()
        {
            return ModelType.BULLETBOARD;
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        public int GetCurrentAnimationIndex()
        {
            return AnimationIndex;
        }

        public float GetAnimationDepth()
        {
            return Depth;
        }

        public Vector2 GetAbsolutePosition()
        {
            return Position;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }

        public List<ModelObject> GetChildrenObjects()
        {
            List<ModelObject> childrenlst = new List<ModelObject>();
            foreach (BulletIconModel child in bulletIcons)
            {
                childrenlst.Add(child);
            }
            return childrenlst;
        }

        public void RemoveFirstBullet()
        {
            if (bulletIcons.Count > 0)
            {
                bulletIcons.RemoveAt(0);
                this.viewObject = null;
            }
        }


        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }



    class ScroeBoardModel : ModelObject
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;
        List<BulletIconModel> bulletIcons;

        int AnimationIndex
        {
            get
            {
                return 0;
            }
        }

        float Depth
        {
            get { return 0.35f; }
        }

        Vector2 Position
        {
            get
            {
                return position;
            }
        }

        Rectangle space; //indicate the object view range
        Vector2 position = Vector2.Zero; // no use

        public ScroeBoardModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // background
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\ScoreBoard";
            animationInfo.frameWidth = 148;
            animationInfo.frameHeight = 65;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 148;
            space.Height = 65;

            bulletIcons = new List<BulletIconModel>();

        }

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\ScoreBoard";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\gameFont";
            resourceList.Add(resourceItm);

            return resourceList;
        }


        public ScroeBoardModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\ScoreBoard";
            animationInfo.frameWidth = 148;
            animationInfo.frameHeight = 65;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 148;
            space.Height = 65;

            bulletIcons = new List<BulletIconModel>();
        }


        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            position.X = space.Left;
            position.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        public void Update(GameTime gameTime)
        {
            // no update for itself


            // update child object
        }


        public ModelType Type()
        {
            return ModelType.SCOREBOARD;
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        public int GetCurrentAnimationIndex()
        {
            return AnimationIndex;
        }

        public float GetAnimationDepth()
        {
            return Depth;
        }

        public Vector2 GetAbsolutePosition()
        {
            return Position;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }

        public List<ModelObject> GetChildrenObjects()
        {
            List<ModelObject> childrenlst = new List<ModelObject>();
            foreach (BulletIconModel child in bulletIcons)
            {
                childrenlst.Add(child);
            }
            return childrenlst;
        }


        int totalscore = 0;
        public void AddScore(int score)
        {
            totalscore += score;
        }

        public int TotalScore
        {
            get
            {
                return totalscore;
            }
        }



        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    } 
}
