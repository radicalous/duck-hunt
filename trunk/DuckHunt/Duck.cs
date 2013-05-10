using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GameCommon;

namespace DuckHuntCommon 
{
    enum ModelType { NONE, SKY, GRASS, DUCK, DOG, BULLET, HITBOARD};

    interface ModelObject
    {
        void Initialize(Rectangle rect, int seed);
        void Update(GameTime gameTime);
        ModelType Type();

        Vector2 GetPosition();
        Rectangle GetSpace();
        float GetSacle();

        List<AnimationInfo> GetAnimationInfoList();
        int GetCurrentAnimationIndex();
        float GetAnimationDepth();
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
        void Init(ModelObject model, List<Texture2D> texturesList, Rectangle space);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }

    class CommonViewObject: ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;

        ModelObject model;

        public CommonViewObject(ModelObject model1)
        {
            model = model1;
        }

        public void Init(ModelObject model1, List<Texture2D> texturesList, Rectangle space)
        {
            animationList = model.GetAnimationInfoList();
            viewItmList = new List<ViewItem>();
            int i = 0;
            foreach (AnimationInfo animationInfo in animationList)
            {
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
                i++;
            }
        }

        public void Update(GameTime gameTime)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = model.GetPosition();
                viewItm.animation.scale = model.GetSacle();
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
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
        }
    }

    class HitBoardViewObject: ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;

        ModelObject model;

        public HitBoardViewObject(ModelObject model1)
        {
            model = model1;
        }

        public void Init(ModelObject model1, List<Texture2D> texturesList, Rectangle space)
        {

        }

        public void Update(GameTime gameTime)
        {
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw background


            // draw duck status


        }
    }


    class ForestModel
    {
        Rectangle sceneRc;
        Rectangle duckFlySpace;
        Rectangle dogRunSpace;

        string skyTexture;
        string forestTexture;

        public void Init(Rectangle screenRc)
        {
            sceneRc = new Rectangle(screenRc.X, screenRc.Y, screenRc.Width, screenRc.Height);
            duckFlySpace = new Rectangle();
            dogRunSpace = new Rectangle();

        }
    }

    class SkyModel : ModelObject
    {
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

        public void Initialize(Rectangle rect, int seed)
        {
            space = rect;
        }

        public void Update(GameTime gameTime)
        {
        }

        public ModelType Type()
        {
            // sky 

            return ModelType.SKY;
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
        public Vector2 GetPosition()
        {
            return Vector2.Zero;
        }
        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1.0f;
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
        string texturesPath = "Graphics\\duckForest";

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


        public void Initialize(Rectangle rect, int seed)
        {
            space = rect;
        }

        public void Update(GameTime gameTime)
        {
        }

        public ModelType Type()
        {
            // sky 

            return ModelType.GRASS;
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
        public Vector2 GetPosition()
        {
            return Vector2.Zero;
        }
        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1.0f;
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
                        return 0;
                    }
                    else
                    {
                        return 3;
                    }
                }
            }
        }   


        Vector2 Position
        {
            get 
            {
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
            animationInfo.texturesPath = "Graphics\\ducks";
            animationInfo.frameWidth = 78;
            animationInfo.frameHeight = 88;
            animationInfo.frameCount = 4;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\dyingduck";
            animationInfo.frameWidth = 101;
            animationInfo.frameHeight = 72;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\deadduck";
            animationInfo.frameWidth = 39;
            animationInfo.frameHeight = 83;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\ducksreverse";
            animationInfo.frameWidth = 78;
            animationInfo.frameHeight = 88;
            animationInfo.frameCount = 4;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);
        }


        public void Shoot(BulletModel bullect)
        {
            // check if it's shoot
            if (Active == false)
            {
                return;
            }

            Vector2 position = bullect.GetPosition();

            Vector2 subpos = position - autoPilot.Position;
            if (subpos.Length() < 20)
            {
                Active = false;
                dead = true;
                deadPilot = new DeadPilot(this.autoPilot, false);

                // new a bullet  
                bullect.SetTarget(this);
            }

        }


        public void Initialize(Rectangle duckFlySpace, int seed)
        {
            // Set the starting position of the player around the middle of the screen and to the back
            autoPilot.Initialize(duckFlySpace,seed);
            Rectangle startSpace = new Rectangle(duckFlySpace.X, duckFlySpace.Y + (int)(0.9*duckFlySpace.Height),
                duckFlySpace.Width, (int)(duckFlySpace.Height*0.1));
            autoPilot.LeadDirection(AutoPilot.Direction.RANDOM, AutoPilot.Direction.UP);
            autoPilot.RadomStartPos(startSpace);

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;

            Random radom = new Random(seed);
            //scale = radom.Next(5,10) * 1.0f / 10;
            //depth += 1.0f / (scale*20);
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
                    deadPilot.Position.Y < autoPilot.boundaryRect.Top)
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
        public Vector2 GetPosition()
        {
            return Position;
        }
        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return scale;
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


        public void ShowDog(int deadduck)
        {
            state = DOGSTATE.Showing;
            showPilot = new DogShowPilot(pilot);

            deadDuck = deadduck;

            gone = false;
        }


        public void Initialize(Rectangle dogSpace, int seed)
        {          
            dogspace = new Rectangle();
            dogspace.X = dogSpace.X;
            dogspace.Y = dogSpace.Y;
            dogspace.Width = dogSpace.Width;
            dogspace.Height = dogSpace.Height;

            // Set the starting position of the player around the middle of the screen and to the back
            pilot.Initialize(dogSpace);
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

        public Vector2 GetPosition()
        {
            return Position;
        }
        public Rectangle GetSpace()
        {
            return dogspace;
        }
        public float GetSacle()
        {
            return 1.0f;
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
        Rectangle dogspace;

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
        DuckModel shootduck;


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
        }

        float deltax = 40f; //40f;
        float deltay = -20f; //-20f;

        public void SetTarget(DuckModel duck)
        {
            shootduck = duck;
            if (duck != null)
            {
                depth = duck.GetAnimationDepth() + 0.1f;
                position = duck.GetPosition();
                targetposition = position;
                position.X = position.X - 20*6 ;
                position.Y = position.Y + 10*6 ;
            }   
        }


        public void Initialize(Rectangle dogSpace, int seed)
        {
            dogspace = new Rectangle();
            dogspace.X = dogSpace.X;
            dogspace.Y = dogSpace.Y;
            dogspace.Width = dogSpace.Width;
            dogspace.Height = dogSpace.Height;

            // Set the starting position of the player around the middle of the screen and to the back
        }

        float scale = 1.0f;

        public void Update(GameTime gameTime)
        {
            if (shootduck != null)
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

        public Vector2 GetPosition()
        {
            if (shootduck != null)
            {
                //return shootduck.GetPosition();
            }
            return Position;
        }
        public Rectangle GetSpace()
        {
            return dogspace;
        }
        public float GetSacle()
        {
            return scale;
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


    class HitBoardModel : ModelObject
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;

        int AnimationIndex
        {
            get
            {
                return 0;
            }
        }

        float Depth
        {
            get { return 0.1f; }
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

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\laser1";
            animationInfo.frameWidth = 47;
            animationInfo.frameHeight = 23;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);
        }
        public HitBoardModel(Vector2 position1)
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
        }
      

        public void Initialize(Rectangle rangespace, int seed)
        {
            space = rangespace;
        }

        public void Update(GameTime gameTime)
        {
 
        }


        public ModelType Type()
        {
            // sky 

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

        public Vector2 GetPosition()
        {
            if (shootduck != null)
            {
                //return shootduck.GetPosition();
            }
            return Position;
        }
        public Rectangle GetSpace()
        {
            return dogspace;
        }
        public float GetSacle()
        {
            return 1;
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
