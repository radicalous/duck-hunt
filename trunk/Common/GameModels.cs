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
    enum ModelType { NONE, CLOUD, SKY, GRASS,FORGROUND, DUCK, DOG, BULLET, HITBOARD,
        DUCKICON, BULLETBOARD, BULLETICON, SCOREBOARD, SCORELISTBOARD, TIMEBOARD, 
        LOSTDUCKBOARD, MENUITEM, TITLEITEM, KEYBORD, KEYITEM, CHECKBOX, BUTTON,  PANDA,
        FIREWORK,PLANE, BALOON, LEVELUPBOARD,PARROT,SMOKE
    };
    
    enum ResourceType { TEXTURE, SOUND, FONT };
    class ResourceItem
    {
        public ResourceType type;
        public string path;
    }


    abstract class ModelObject
    {
        public abstract ModelType Type();

        public abstract void Initialize(ModelObject parent, Rectangle rect, int seed); // Rect is the rect range based on parent object
        public abstract void Cleanup(); 

        public abstract void Update(GameTime gameTime);

        public abstract List<ResourceItem> GetResourceList();

        public abstract Vector2 GetAbsolutePosition(); // the lefttop cornor based on the parent object
        public abstract Rectangle GetSpace();   // space is the rect the object may cover, the lefttop is Zero
        public abstract float GetSacle();       // scale is the scale to scale textures 


        public abstract List<AnimationInfo> GetAnimationInfoList(); // this is the animation information, include itself and it's children's
        public abstract int GetCurrentAnimationIndex(); // current animation index
        public abstract float GetAnimationDepth();      // animation depth

        public abstract int GetSoundIndex();
        public abstract float GetSoundVolumn();


        public abstract ModelObject GetParentObject();
        public abstract List<ModelObject> GetChildrenObjects();

        // assist function, improve performance
        public abstract ViewObject GetViewObject();
        public abstract void SetViewObject(ViewObject viewObject);

    }



    abstract class BaseModel : ModelObject
    {
        ViewObject viewObject;
        Vector2 relativePostionInParent;

        ModelObject parent = null;

        override public void Initialize(ModelObject parent1, Rectangle itemSpace, int seed)
        {
            parent = parent1;
            relativePostionInParent.X = itemSpace.Left;
            relativePostionInParent.Y = itemSpace.Top;
        }

        public override void Cleanup()
        {
            
        }

        override public void Update(GameTime gameTime)
        {


        }
        override public int GetSoundIndex()
        {
            return -1;
        }
        override public float GetSoundVolumn()
        {
            return 0.3f;
        }
        override public ModelObject GetParentObject()
        {
            return parent;
        }
        override public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }


        override public ViewObject GetViewObject()
        {
            return viewObject;
        }
        
        override public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }

    abstract class BackgroundModel : BaseModel
    {
        ModelObject parent = null;
        Vector2 relativePos;
        Rectangle space;

        // Animation representing the player
        List<AnimationInfo> anationInfoList;

        public BackgroundModel()
        {
            anationInfoList = new List<AnimationInfo>();
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = animationInfo.frameHeight = 0;
            anationInfoList.Add(animationInfo);
        }


        override public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            parent = null;
            space = rect;
            relativePos.X = rect.Left;
            relativePos.Y = rect.Top;
            space.Offset(-space.Left, -space.Y);
        }

        override public void Update(GameTime gameTime)
        {
            // no animation
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePos;
            if (parent != null)
            {
                absPos += parent.GetAbsolutePosition();
            }
            return absPos;
        }
        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1.0f;
        }


        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return 1.0f;
        }

        override public int GetSoundIndex()
        {
            return -1;
        }


        override public ModelObject GetParentObject()
        {
            return null;
        }


        override public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }
    }

    class SkyModel : BackgroundModel
    {
        List<AnimationInfo> anationInfoList;

        public SkyModel()
        {

            anationInfoList = new List<AnimationInfo>();
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1; // 5;
            animationInfo.frameWidth = animationInfo.frameHeight = 0;
            animationInfo.frameTime = 500;
            anationInfoList.Add(animationInfo);
        }

        // interfaces implementation
        override public ModelType Type()
        {
            // sky 
            return ModelType.SKY;
        }

        override public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            base.Initialize(parent, rect, seed);
        }

        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky";
            resourceList.Add(resourceItm);

            return resourceList;
        }
    }



    class SmokeModel : BackgroundModel
    {
        List<AnimationInfo> anationInfoList;

        public int BgRcWidth
        {
            get
            {
                return 1920;
            }
        }

        public int BgRcHight
        {
            get
            {
                return 1200;
            }
        }

        public int XOffInBg
        {
            get
            {
                return 1530;
            }
        }

        public int YOffInBg
        {
            get
            {
                return 238;
            }
        }

        public SmokeModel()
        {

            anationInfoList = new List<AnimationInfo>();
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 210;
            animationInfo.frameHeight = 267;
            animationInfo.frameTime = 500;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 210;
            animationInfo.frameHeight = 267;
            animationInfo.frameTime = 500;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 210;
            animationInfo.frameHeight = 267;
            animationInfo.frameTime = 500;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 210;
            animationInfo.frameHeight = 267;
            animationInfo.frameTime = 500;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 210;
            animationInfo.frameHeight = 267;
            animationInfo.frameTime = 500;
            anationInfoList.Add(animationInfo);
        }

        // interfaces implementation
        override public ModelType Type()
        {
            // sky 
            return ModelType.SMOKE;
        }

        override public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            base.Initialize(parent, rect, seed);
        }

        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky_smoke_3";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky_smoke_4";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky_smoke_5";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky_smoke_1";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky_smoke_2";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public float GetAnimationDepth()
        {
            return 0.999f;
        }

    }



    class GrassMountainModel : BackgroundModel
    {
        public GrassMountainModel()
        {
        }

        // interfaces implementation
        override public ModelType Type()
        {
            // sky 
            return ModelType.SKY;
        }

        override public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            base.Initialize(parent, rect, seed);
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky_2";
            resourceList.Add(resourceItm);
            return resourceList;
        }
    }


    class CloudModel : BaseModel
    {
        ModelObject parent = null;
        AiPilot pilot;
        Rectangle space;

        List<AnimationInfo> anationInfoList;
        Vector2 relativePosInParent;

        public CloudModel()
        {
            pilot = PilotManager.GetInstance().CreatePilot(PilotType.CLOUD);

            anationInfoList = new List<AnimationInfo>();
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.animation = true;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 518;
            animationInfo.frameHeight = 398;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);
        }


        override public ModelType Type()
        {
            return ModelType.CLOUD;
        }

        override public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            base.Initialize(null, rect, seed);
            space = rect;
            relativePosInParent.X = rect.Left;
            relativePosInParent.Y = rect.Top;
            space.Offset(-space.Left, -space.Y);
            pilot.Initialize(space, 0);
        }

        override public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // no animation
            pilot.Update(gameTime);
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\Cloud";
            resourceList.Add(resourceItm);
            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = pilot.GetPosition();
            absPos += relativePosInParent;
            if (parent != null)
            {
                absPos += parent.GetAbsolutePosition();
            }
            absPos.X -= anationInfoList[0].frameWidth / 2;
            absPos.Y -= anationInfoList[0].frameHeight / 2;

            return absPos;
        }
        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 0.5f;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return 0.9f;
        }
    }




    class GrassModel : BackgroundModel
    {
        public GrassModel()
        {
        }

        override public ModelType Type()
        {
            return ModelType.GRASS;
        }


        override public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            base.Initialize(parent, rect, seed);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\bg_grass";
            resourceList.Add(resourceItm);
            return resourceList;
        }

        override public float GetAnimationDepth()
        {
            return 0.5F;
        }
    }



    class ForegroundGrassModel : BackgroundModel
    {
        public ForegroundGrassModel()
        {
        }

        override public ModelType Type()
        {
            return ModelType.FORGROUND;
        }

        override public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            base.Initialize(parent, rect, seed);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\bg_tree";
            resourceList.Add(resourceItm);
            return resourceList;
        }
        override public float GetAnimationDepth()
        {
            return 0.2F;
        }
    }
 
    class AnimationInfo
    {
        public bool fontTexture = false;
        public bool animation = true;

        public int frameWidth = 115;
        public int frameHeight = 69;
        public int frameCount = 8;
        public int frameTime = 30;
        public Color backColor = Color.White;
    }



    class MenuItemModel : BaseModel
    {

        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        List<Vector2> boundingTrigle1;
        List<Vector2> boundingTrigle2;

        Rectangle _itemspace = new Rectangle(0, 0, 240, 137);

        float scale = 1.0f;
        float depth = 0.6f;

        bool onHover = false;


        string content = "test";
        public string Conent
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        Vector2 relativePostionInParent;


        public MenuItemModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // 0. unselected duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 240;
            animationInfo.frameHeight = 137;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 600;
            anationInfoList.Add(animationInfo);

            //1. hover duck
            animationInfo = new AnimationInfo();
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 240;
            animationInfo.frameHeight = 137;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            boundingTrigle1 = new List<Vector2>();
            boundingTrigle2 = new List<Vector2>();

        }

        // interfaces implementation
        override public ModelType Type()
        {
            return ModelType.MENUITEM;
        }

        override public void Initialize(ModelObject parent1, Rectangle itemSpace, int seed)
        {
            base.Initialize(parent1, itemSpace, seed);

            relativePostionInParent.X = itemSpace.Left;
            relativePostionInParent.Y = itemSpace.Top;

            _itemspace = itemSpace;
            _itemspace.X -= (int)relativePostionInParent.X;
            itemSpace.Y -= (int)relativePostionInParent.Y;
            //_itemspace.Offset((int)-postion.X, (int)-postion.Y);

            Vector2 pos1 = new Vector2();
            pos1.X = GetAbsolutePosition().X + 13;
            pos1.Y = GetAbsolutePosition().Y + 84;
            boundingTrigle1.Add(pos1);
            pos1.X = GetAbsolutePosition().X + 120;
            pos1.Y = GetAbsolutePosition().Y + 121;
            boundingTrigle1.Add(pos1);
            pos1.X = GetAbsolutePosition().X + 230;
            pos1.Y = GetAbsolutePosition().Y + 70;
            boundingTrigle1.Add(pos1);

            pos1.X = GetAbsolutePosition().X + 13;
            pos1.Y = GetAbsolutePosition().Y + 84;
            boundingTrigle2.Add(pos1);
            pos1.X = GetAbsolutePosition().X + 230;
            pos1.Y = GetAbsolutePosition().Y + 70;
            boundingTrigle2.Add(pos1);
            pos1.X = GetAbsolutePosition().X + 91;
            pos1.Y = GetAbsolutePosition().Y + 8;
            boundingTrigle2.Add(pos1);
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();

            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\MenuItem";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\Cloud";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\menu_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }


        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePostionInParent;

            return absPos;
        }
        override public Rectangle GetSpace()
        {
            return _itemspace;
        }
        override public float GetSacle()
        {
            if (onHover)
            {
                return 0.5f;
            }
            else
            {
#if WINDOWS_PHONE
                return 1.2f;
#else
                return 1.0f;
#endif

            }
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {

                if (onHover)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }


        public bool Hit(Vector2 absposition)
        {
            if (CollectionDetect.PointInTriangle(absposition, boundingTrigle1))
            {
                return true;
            }
            if (CollectionDetect.PointInTriangle(absposition, boundingTrigle2))
            {
                return true;
            }
            return false;
        }
    }


    class TitleItemModel : BaseModel
    {

        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        List<Vector2> boundingTrigle1;
        List<Vector2> boundingTrigle2;

        Rectangle _itemspace = new Rectangle(0, 0, 240, 137);

        float scale = 1.0f;
        float depth = 0.6f;

        bool onHover = false;


        string content = "test";
        public string Conent
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        Vector2 relativePostionInParent;


        public TitleItemModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

        }

        // interfaces implementation
        override public ModelType Type()
        {
            return ModelType.TITLEITEM;
        }

        override public void Initialize(ModelObject parent1, Rectangle itemSpace, int seed)
        {
            base.Initialize(parent1, itemSpace, seed);

            relativePostionInParent.X = itemSpace.Left;
            relativePostionInParent.Y = itemSpace.Top;

            _itemspace = itemSpace;
            _itemspace.X -= (int)relativePostionInParent.X;
            itemSpace.Y -= (int)relativePostionInParent.Y;
            //_itemspace.Offset((int)-postion.X, (int)-postion.Y);
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();

            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\menu_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }


        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePostionInParent;

            return absPos;
        }
        override public Rectangle GetSpace()
        {
            return _itemspace;
        }
        override public float GetSacle()
        {
            {
#if WINDOWS_PHONE
                return 3.0f;
#else
                return 2.0f;
#endif

            }
        }


        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return null;
        }

        override public int GetCurrentAnimationIndex()
        {
            return -1;
        }
        override public float GetAnimationDepth()
        {
            return depth;
        }
    }

    class KeyItemModel : BaseModel
    {
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        Rectangle _itemspace = new Rectangle(0, 0, 240, 137);

        float scale = 2.0f;
        float depth = 0.1f;

        bool onHover = false;

        ModelObject parent;


        string content = "test";
        public string Conent
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }



        Vector2 relativePostionInParent;


        public KeyItemModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // 0. unselected duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 64;
            animationInfo.frameHeight = 45;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 600;
            anationInfoList.Add(animationInfo);

            //1. hover duck
            animationInfo = new AnimationInfo();
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 64;
            animationInfo.frameHeight = 45;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

        }

        // interfaces implementation
        override public ModelType Type()
        {
            return ModelType.KEYITEM;
        }

        override public void Initialize(ModelObject parent1, Rectangle itemSpace, int seed)
        {
            base.Initialize(parent1, itemSpace, seed);

            parent = parent1;

            relativePostionInParent.X = itemSpace.Left;
            relativePostionInParent.Y = itemSpace.Top;

            _itemspace = itemSpace;
            _itemspace.X -= (int)relativePostionInParent.X;
            itemSpace.Y -= (int)relativePostionInParent.Y;
            //_itemspace.Offset((int)-postion.X, (int)-postion.Y);
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();

            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\KeyItem";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\cnt_font_20";
#else
            resourceItm.path = "Graphics\\cnt_font_30";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }


        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePostionInParent;
            absPos.X = (int)(absPos.X * scale);
            absPos.Y = (int)(absPos.Y * scale);
            if (parent != null)
            {
                // calculate parent center
                Vector2 parentcenter = Vector2.Zero;
                parentcenter.X = parent.GetAbsolutePosition().X;


                parentcenter.Y = parent.GetAbsolutePosition().Y + parent.GetSpace().Height - (int)(parent.GetSpace().Height * parent.GetSacle())
                    + parent.GetSpace().Height*parent.GetSacle()/2;
                parentcenter.X += parent.GetSpace().Width * 1.0f / 2;
                //parentcenter.Y += parent.GetSpace().Height * 1.0f / 2;

                // calcuate lefttop conner;
                Vector2 lefttopconner = parentcenter;
                lefttopconner.X -= parent.GetSpace().Width * 1.0f / 2 * parent.GetSacle();
                lefttopconner.Y -= parent.GetSpace().Height * 1.0f / 2 * parent.GetSacle();

                absPos += lefttopconner;
            }

            return absPos;
        }
        override public Rectangle GetSpace()
        {
            Rectangle rc = new Rectangle();
            rc = _itemspace;
            return rc;
        }
        override public float GetSacle()
        {
                return scale;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {

            if (onHover)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }

        override public int GetSoundIndex()
        {
            return -1;
        }

        override public ModelObject GetParentObject()
        {
            return parent;
        }

        public bool Hit(Vector2 absposition)
        {
            return false;
        }
    }

    class KeyboardModel : BaseModel
    {
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        Rectangle _itemspace = new Rectangle(0, 0, 797, 268);

        float scale = 2.0f;
        float depth = 0.11f;

        bool onHover = false;

        Vector2 relativePostionInParent;

        List<KeyItemModel> keyList;


        public KeyboardModel()
        {
            //
            keyList = new List<KeyItemModel>();

            anationInfoList = new List<AnimationInfo>();

            // 0. unselected duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 797;
            animationInfo.frameHeight = 268;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 600;
            anationInfoList.Add(animationInfo);

        }

        // interfaces implementation
        override public ModelType Type()
        {
            return ModelType.KEYBORD;
        }

        override public void Initialize(ModelObject parent1, Rectangle itemSpace, int seed)
        {
            base.Initialize(null, itemSpace, seed);

            relativePostionInParent.X = itemSpace.Left;
            relativePostionInParent.Y = itemSpace.Top;

            //scale = itemSpace.Width * 1.0f / _itemspace.Width;

            _itemspace = itemSpace;
            _itemspace.X -= (int)relativePostionInParent.X;
            itemSpace.Y -= (int)relativePostionInParent.Y;


            // add the key item
            // y = 11, x(15) (84), (154), (225), (294), (364), (433), (504), (574), (644), (714), 
            // y = 62, 
            // y = 113, x(51),(121), (191), (262), (331), (401), (470), (541), (611), (681)
            // y = 166, x(86), (155), (226), (296), (367), (436), (506), (577), (647)
            // y = 220, x(53), (124), (194), (265), (334), (404), (475), (546), (621)
            KeyItemModel keyItem = null;

            Rectangle keyspace = new Rectangle();

            keyItem = new KeyItemModel();
            keyspace.Y = (int)(11 );
            keyspace.X = (int)(15 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "GAMIL";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(84 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "OUTLOOK";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(154 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Yahoo";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(225 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Live";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(294 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "MAIL";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(364 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "@";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(433 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = ".com";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(504 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = ".co.uk";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(574 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = ".eu";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(644 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "-";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(714 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "_";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.Y = (int)(62 );
            keyspace.X = (int)(15 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "1";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(84 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "2";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(154 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "3";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(225 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "4";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(294 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "5";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(364 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "6";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(433 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "7";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(504 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "8";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(574 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "9";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(644 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "0";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(714 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "X";
            keyList.Add(keyItem);

            // y = 113, x(51),(121), (191), (262), (331), (401), (470), (541), (611), (681)
            keyItem = new KeyItemModel();
            keyspace.Y = (int)(113 );
            keyspace.X = (int)(51 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Q";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(121 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "W";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(191 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "E";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(262 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "R";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(331 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "T";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(401 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Y";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(470 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "U";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(541 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "I";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(611 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "O";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(682 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "P";
            keyList.Add(keyItem);

            // y = 166, x(86), (155), (226), (296), (367), (436), (506), (577), (647)
            keyItem = new KeyItemModel();
            keyspace.Y = (int)(166 );
            keyspace.X = (int)(86 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "A";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(155 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "S";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(226 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "D";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(296 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "F";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(367 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "G";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(436 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "H";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(506 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "J";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(577 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "K";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(647 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "L";
            keyList.Add(keyItem);

            // y = 220, x(53), (124), (194), (265), (334), (404), (475), (546), (621)
            keyItem = new KeyItemModel();
            keyspace.Y = (int)(220 );
            keyspace.X = (int)(53 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Z";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(124 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "X";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(194 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "C";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(265 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "V";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(334 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "B";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(404 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "N";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(475 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "M";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(546 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = ".";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = (int)(621 );
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "SPACE";
            keyList.Add(keyItem);
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();

            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\Keyboardbg";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\cnt_font_20";
#else
            resourceItm.path = "Graphics\\cnt_font_30";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }


        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePostionInParent;

            return absPos;
        }
        override public Rectangle GetSpace()
        {
            Rectangle rc = new Rectangle();
            rc = _itemspace;
            return rc;
        }
        override public float GetSacle()
        {
            return scale;
        }

        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {

            if (onHover)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }


        override public List<ModelObject> GetChildrenObjects()
        {
            List<ModelObject> objlst = new List<ModelObject>();
            foreach (KeyItemModel keyitm in this.keyList)
            {
                objlst.Add(keyitm);
            }
            return objlst;
        }

        public bool Hit(Vector2 absposition)
        {
            return false;
        }
    }

    abstract class AnimalModel : BaseModel
    {
        public abstract void StartPilot();
        public abstract void StartPilot(Vector2 pos);
        public abstract void Shoot(BulletModel bullet);
        public abstract void SetSpeedRatio(float ratio);
        public abstract bool Gone();
        public abstract bool Dead();
        public abstract void SetStartPos(Vector2 startPos);
        public abstract void SetEndPos(Vector2 endPos);
        public abstract void SetShowTime(int seconds);

    }

    class DuckModel : AnimalModel
    {
        AiPilot flyduckPilot;
        AiPilot goneduckPilot;

        public bool Active = true;
        public bool dead = false;

        public bool gone = false;

        // Amount of hit points that player has
        public int Health;

        int deadstopcount = 0;

        List<Vector2> boundingTrigle1;
        List<Vector2> boundingTrigle2;

        Rectangle duckspace;

        int randomseed = 0;
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        float scale = 1.0f;
        float depth = 0.6f;

        int showTime = 15;
        public int ShowTime
        {
            get
            {
                return showTime;
            }
            set
            {
                showTime = value;
            }
        }


        Vector2 RelativePosition
        {
            get 
            {
                if (flyduckPilot == null)
                {
                    return Vector2.Zero;
                }

                if (Active)
                    return flyduckPilot.GetPosition();
                else
                    return goneduckPilot.GetPosition();
            }
        }


        public DuckModel()
        {
            //
            flyduckPilot = PilotManager.GetInstance().CreatePilot(PilotType.DUCKNORMAL);
            anationInfoList = new List<AnimationInfo>();

            // a. normal duck

            // 0. flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            //animationInfo.frameWidth = 180;
            // animationInfo.frameHeight = 202;
            //animationInfo.frameCount = 4;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            //animationInfo.frameWidth = 180;
            //animationInfo.frameHeight = 202;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            // animationInfo.frameWidth = 180;
            // animationInfo.frameHeight = 202;
            // animationInfo.frameCount = 4;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            boundingTrigle1 = new List<Vector2>();
            boundingTrigle2 = new List<Vector2>();
        }

        int duckstyle = 0;

        override public void SetSpeedRatio(float ratio)
        {
            if (ratio < 1)
            {
                ratio = 1 ;
            }
            if (flyduckPilot != null)
            {
                flyduckPilot.SetSpeedRatio(ratio);
            }
        }
        string pilotgroupname = "";
        public DuckModel(PilotType type, string groupname)
        {
            //
            pilotgroupname = groupname;
            flyduckPilot = PilotManager.GetInstance().CreatePilot(type, groupname);
            switch(type)
            {
                case PilotType.DUCKEIGHT:
                case PilotType.DUCKELLIPSE:
                case PilotType.DUCKCIRCLE:
                    {
                        duckstyle = 1;
                    }
                    break;
                case PilotType.DUCKEIGHTDEPTH:
                    {
                        duckstyle = 2;
                    }
                    break;
                case PilotType.PARROT:
                    {
                        duckstyle = 3;

                    }
                    break;
                default:
                    {
                        duckstyle = 0;
                    }
                    break;

            }
            anationInfoList = new List<AnimationInfo>();

            // a. normal duck

            // 0. flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            //animationInfo.frameWidth = 180;
            //animationInfo.frameHeight = 202;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);


            // b. blue duck 

            // 0. flying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            //animationInfo.frameWidth = 180;
            //animationInfo.frameHeight = 202;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);


            // c red duck

            // 0. flying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            // d. red bird


            // 0. flying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 6;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 6;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            boundingTrigle1 = new List<Vector2>();
            boundingTrigle2 = new List<Vector2>();
        }


        override public ModelType Type()
        {
            return ModelType.DUCK;
        }


        override public void Initialize(ModelObject parent1, Rectangle duckSpace, int seed)
        {
            base.Initialize(null, duckspace, seed);

            // Set the player to be active
            Active = true;

            // Set the player health
            Health = 100;
            randomseed = seed;
            Random radom = new Random(seed);

            duckspace = duckSpace;

            Vector2 pos1 = new Vector2();
            pos1.X = 16;
            pos1.Y = 60;
            boundingTrigle1.Add(pos1);
            pos1.X = 26;
            pos1.Y = 19;
            boundingTrigle1.Add(pos1);
            pos1.X = 86;
            pos1.Y = 40;
            boundingTrigle1.Add(pos1);

            pos1.X = 26;
            pos1.Y = 19;
            boundingTrigle2.Add(pos1);
            pos1.X = 86;
            pos1.Y = 40;
            boundingTrigle2.Add(pos1);

            pos1.X = 75;
            pos1.Y = 81;
            boundingTrigle2.Add(pos1);
        }

        public override void Cleanup()
        {
            PilotManager.GetInstance().ReturnPilot(pilotgroupname, flyduckPilot);

            this.flyduckPilot = null;
            this.goneduckPilot = null;
        }


        override public List<ResourceItem> GetResourceList()
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

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_blue_flying";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_blue_shot";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_blue_dead";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_blue_flying_r";
            resourceList.Add(resourceItm);


            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_red_flying";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_red_shot";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_red_dead";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\duck_red_flying_r";
            resourceList.Add(resourceItm);



            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\red_bird_r";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\red_bird_dying";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\red_bird_dead";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\red_bird";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\duck_live";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\duck_dead";
            resourceList.Add(resourceItm);


            return resourceList;
        }


        //Rectangle space;
        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = RelativePosition;

            // pilot return center postion, adjust it to left top conner
            absPos.X -= 105 / 2;
            absPos.Y -= 102 / 2;

            return absPos;
        }
        override public Rectangle GetSpace()
        {
            return duckspace;
        }
        override public float GetSacle()
        {
            if (Active)
            {
                // get depth, calculate the scale

                //scale = autoPilot.scale;
                scale = 1 - flyduckPilot.GetDepth() * 1.0f / 100;
#if WINDOWS_PHONE
                scale *= 1.5f;
#endif
            }

            return scale;
        }

        override public void Update(GameTime gameTime)
        {
            if (Active)
            {
                flyduckPilot.Update(gameTime);

                // check if it need to go
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime > 1000 * showTime)
                {
                    Active = false;
                    goneduckPilot = PilotManager.GetInstance().CreatePilot(PilotType.DUCKFLYAWAY, flyduckPilot.GetPosition());
                }
            }
            else
            {
                if (deadstopcount < 10)
                {
                    deadstopcount++;
                }
                goneduckPilot.Update(gameTime);
                if (goneduckPilot.GetPosition().Y > duckspace.Height ||
                    goneduckPilot.GetPosition().Y < 0 - anationInfoList[GetCurrentAnimationIndex()].frameHeight)
                {
                    gone = true;
                }
            }

        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {  
            if(dead)
            {
                if (deadstopcount < 10)
                {
                    return 1 + this.duckstyle*4;
                }
                else
                {
                    return 2 + this.duckstyle * 4;
                }
            }
            else
            {
                if (flyduckPilot.GetHorizationDirection() == Direction.LEFT)
                {
                    if (flyduckPilot.GetZDirection() ==  Direction.IN)
                    {
                        return 0 + this.duckstyle * 4;
                    }
                    else if (flyduckPilot.GetZDirection() == Direction.OUT)
                    {
                        return 0 + this.duckstyle * 4;
                    }
                    else
                    {
                        return 0 + this.duckstyle * 4;
                    }
                }
                else
                {
                    if ( flyduckPilot.GetZDirection() == Direction.IN)
                    {
                        return 3 + this.duckstyle * 4;
                    }
                    else if (flyduckPilot.GetZDirection() == Direction.OUT)
                    {
                        return 3 + this.duckstyle * 4;
                    }
                    else 
                    {
                        return 3 + this.duckstyle * 4;
                    }
                }
            }
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }

        bool barked = false;
        override public int GetSoundIndex()
        {
            if (Active)
            {
                //
                if (elapsedTime > 500)
                {
                    if (!barked)
                    {
                        barked = true;
                        return 0;
                    }
                }
            }
            else if (dead)
            {
                if (!barked)
                {
                    barked = true;
                    return 1;
                }
            }
            return -1;
        }


        /// <summary>
        ///  specific functions
        /// </summary>

        override public void StartPilot()
        {
            // Set the starting position of the player around the middle of the screen and to the back

            flyduckPilot.Initialize(duckspace, randomseed);
        }

        override public void StartPilot(Vector2 pos)
        {
            // Set the starting position of the player around the middle of the screen and to the back

            flyduckPilot.Initialize(duckspace, randomseed);
            flyduckPilot.SetStartPos(pos);
        }


        override public void Shoot(BulletModel bullet)
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

            Vector2 duckCenter = GetAbsolutePosition();
            Vector2 bullet2DuckPos = bulletCenter - duckCenter;


            //
            float r = anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2; // 
            r = 40;
            duckCenter.X += anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            duckCenter.Y += anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;

            Vector2 subpos = bulletCenter - duckCenter;
            if (subpos.Length() < r * scale)
            {
                Active = false;
                dead = true;
                barked = false;
                goneduckPilot = PilotManager.GetInstance().CreatePilot(PilotType.DUCKDEAD, flyduckPilot.GetPosition());

                // new a bullet  
                bullet.SetTarget(this);
            }
        }

        override public bool Gone()
        {
            return gone;
        }
        override public bool Dead()
        {
            return dead;
        }

        override public void SetStartPos(Vector2 startPos)
        {
            flyduckPilot.SetStartPos(startPos);
        }

        override public void SetEndPos(Vector2 endPos)
        {
            flyduckPilot.SetEndPos(endPos);
        }

        public override void SetShowTime(int seconds)
        {
            ShowTime = seconds;
        }
    }


    class DogModel : BaseModel
    {
        //DogPilot pilot;
        AiPilot seekPilot;

        int foundmaxstoptime = 50;
        int midseekmaxstoptime = 100;
        int midseekstopcount = 0;
        int foundstopcount = 0;

        //DogJumpPilot jumpPilot;
        AiPilot jumpPilot;
        bool jumpup = true;

        AiPilot showPilot;
        //DogShowPilot showPilot;

        enum DOGSTATE { FindingDuck, Jumping, Showing };
        DOGSTATE state = DOGSTATE.FindingDuck;


        // Animation representing the player
        public string animationTexturesPath = "Graphics\\dogs";

        List<AnimationInfo> anationInfoList;
        Rectangle dogspace;

        bool gone = false;

        int dog_sound_index = -1;

        public bool Gone
        {
            get { return gone; }
        }

        float depth = 0.4F;
        int deadDuck = 0;

        Vector2 RelativePosition
        {
            get
            {
                if (state == DOGSTATE.FindingDuck)
                {
                    //
                    return seekPilot.GetPosition();
                }
                else if (state == DOGSTATE.Jumping)
                {
                    return jumpPilot.GetPosition();
                }
                else if (state == DOGSTATE.Showing)
                {
                    return showPilot.GetPosition();
                }
                else
                {
                    return seekPilot.GetPosition();
                }
            }
        }

        public DogModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 221;
            animationInfo.frameHeight = 200;
            animationInfo.frameCount = 4;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 221;
            animationInfo.frameHeight = 200;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 221;
            animationInfo.frameHeight = 200;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 30000;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 221;
            animationInfo.frameHeight = 200;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 30000;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 221;
            animationInfo.frameHeight = 200;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 30000;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 221;
            animationInfo.frameHeight = 200;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 150;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 221;
            animationInfo.frameHeight = 200;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 221;
            animationInfo.frameHeight = 200;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            seekPilot = PilotManager.GetInstance().CreatePilot(PilotType.DOGSEEK);
        }

        override public ModelType Type()
        {
            return ModelType.DOG;
        }

        override public void Initialize(ModelObject parent1, Rectangle dogSpace, int seed)
        {
            base.Initialize(parent1, dogspace, seed);
            dogspace = dogSpace;
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\baby_pluto";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\plutoseeking";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\pluto_found";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\plutojumpup";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\plutojumpdown";
            resourceList.Add(resourceItm);
            
    
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\dog_found";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = RelativePosition;
            // adjust from lefttop conner to center
            abspos.X -= anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            abspos.Y -= anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            Rectangle space = new Rectangle(0, 0
            , anationInfoList[GetCurrentAnimationIndex()].frameWidth
            , anationInfoList[GetCurrentAnimationIndex()].frameHeight);

            return space;
        }
        override public float GetSacle()
        {
            return 1.0f;
        }


        override public void Update(GameTime gameTime)
        {
            if (state == DOGSTATE.FindingDuck)
            {
                //
                if (seekPilot.GetPosition().X >= dogspace.Width / 4)
                {
                    // sleep some time
                    if (midseekstopcount < midseekmaxstoptime)
                    {
                        midseekstopcount++;
                        return;
                    }
                }

                if (seekPilot.GetPosition().X >= dogspace.Width / 2)
                {
                    // sleep some time
                    if (dog_sound_index == -1)
                    {
                        dog_sound_index = 0;
                    }
                    if (foundstopcount < foundmaxstoptime)
                    {
                        foundstopcount++;
                        return;
                    }

                    state = DOGSTATE.Jumping;
                    jumpPilot = PilotManager.GetInstance().CreatePilot(PilotType.DOGJUMP, seekPilot.GetPosition());
                    jumpPilot.Initialize(dogspace, 0);
                }
                seekPilot.Update(gameTime);
                depth = 0.4F;
            }
            else if (state == DOGSTATE.Jumping)
            {
                jumpPilot.Update(gameTime);
                if (jumpPilot.GetPosition().Y <= dogspace.Top)
                {
                    depth = 0.6F;
                    jumpup = false;
                }
                if (jumpPilot.GetPosition().Y > dogspace.Bottom)
                {
                    state = DOGSTATE.Showing;
                    showPilot = PilotManager.GetInstance().CreatePilot(PilotType.DOGSHOW, seekPilot.GetPosition());
                    showPilot.Initialize(dogspace, 0);
                    gone = true;
                }
            }
            else if (state == DOGSTATE.Showing)
            {
                showPilot.Update(gameTime);
                if (showPilot.GetPosition().Y > dogspace.Bottom)
                {
                    state = DOGSTATE.Showing;
                    showPilot = PilotManager.GetInstance().CreatePilot(PilotType.DOGSHOW, seekPilot.GetPosition());
                    showPilot.Initialize(dogspace, 0);
                    gone = true;
                }
            }

        }

        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
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

        override public float GetAnimationDepth()
        {
            return depth;
        }

        override public int GetSoundIndex()
        {
            if (dog_sound_index >= 0)
            {
                int returnindex = dog_sound_index;
                dog_sound_index = -2;
                return returnindex;
            }
            return -1;
        }
        override public float GetSoundVolumn()
        {
            return 1.0f;
        }

        public void ShowDog(int deadduck)
        {
            state = DOGSTATE.Showing;
            showPilot = PilotManager.GetInstance().CreatePilot(PilotType.DOGSHOW, seekPilot.GetPosition());
            showPilot.Initialize(dogspace, 0);
            deadDuck = deadduck;

            gone = false;
        }

        public void StartPilot()
        {
            // Set the starting position of the player around the middle of the screen and to the back
            seekPilot.Initialize(dogspace, 0);
        }

    }


    class BulletModel : BaseModel
    {
       ModelObject parent = null;
        Vector2 relativePositionInParent = Vector2.Zero;
        Vector2 targetposition = Vector2.Zero;
        List<AnimalModel> shootduckList;

        ParrotModel parrot;
        BaloonModel baloon;

        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        Rectangle bulletspace;

        bool gone = false;
        float scale = 1.0f;

        public bool Gone
        {
            get { return gone; }
        }

        float depth = 0.6F;

        int sound_index = 0;


        public BulletModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 47;
            animationInfo.frameHeight = 23;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            bulletspace.X = 0;
            bulletspace.Y = 0;
            bulletspace.Width = animationInfo.frameWidth;
            bulletspace.Height = animationInfo.frameHeight;

            shootduckList = new List<AnimalModel>();
        }

        public BulletModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 35;
            animationInfo.frameHeight = 27;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            relativePositionInParent = position1;
            relativePositionInParent.X -= animationInfo.frameWidth / 2;
            relativePositionInParent.Y -= animationInfo.frameHeight / 2;
            bulletspace.X = 0;
            bulletspace.Y = 0;
            bulletspace.Width = animationInfo.frameWidth;
            bulletspace.Height = animationInfo.frameHeight;

            shootduckList = new List<AnimalModel>();
        }

        float deltax = 48f; //40f;
        float deltay = -18f; //-20f;

        override public ModelType Type()
        {
            return ModelType.BULLET;
        }


        override public void Initialize(ModelObject parent1, Rectangle space, int seed)
        {
            base.Initialize(null, space, seed);

            bulletspace = new Rectangle();
            bulletspace = space;
            relativePositionInParent.X = bulletspace.Left;
            relativePositionInParent.Y = bulletspace.Y;

            bulletspace.X = 0;
            bulletspace.Y = 0;
            bulletspace.Width = anationInfoList[GetCurrentAnimationIndex()].frameWidth;
            bulletspace.Height = anationInfoList[GetCurrentAnimationIndex()].frameHeight;

            // Set the starting position of the player around the middle of the screen and to the back
        }


        override public void Update(GameTime gameTime)
        {
            if (shootduckList.Count > 0)
            {

                if (relativePositionInParent.X < targetposition.X)
                {
                    relativePositionInParent.X += deltax;
                }
                if (relativePositionInParent.Y > targetposition.Y)
                {
                    relativePositionInParent.Y += deltay;
                }
                if (relativePositionInParent.X >= targetposition.X && relativePositionInParent.Y <= targetposition.Y)
                {
                    scale = 0;
                }
            }
            else
            {
                    relativePositionInParent.X += deltax;

                    relativePositionInParent.Y += deltay;

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

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\laser1";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\shoot";
            //resourceItm.path = "Sound\\laserFire";
            resourceList.Add(resourceItm);

            return resourceList;
        }


        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePositionInParent;
            if (parent != null)
            {
                abspos += parent.GetAbsolutePosition();
            }
            if (shootduckList.Count > 0)
            {
                //return shootduck.GetAbsolutePosition();
            }
            return abspos;
        }
        override public Rectangle GetSpace()
        {
            return bulletspace;
        }
        override public float GetSacle()
        {
            return scale;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }

        override public int GetSoundIndex()
        {
            if (sound_index >= 0)
            {
                int returnindex = sound_index;
                sound_index = -1;
                return returnindex;
            }
            return -1;
        }



        // specific functions
        public List<AnimalModel> GetShootDucks()
        {
            if (shootduckList.Count > 0)
            {
                return shootduckList;
            }

            return null;
        }

        public BaloonModel GetBaloon()
        {
            return baloon;
        }

        public ParrotModel GetParrot()
        {
            return parrot;
        }
        /*
        public void SetTarget(ParrotModel parrot)
        {
            if (parrot != null)
            {
                this.parrot = parrot;
                depth = parrot.GetAnimationDepth() + 0.1f;
            }
        }
        */

        public void SetTarget(BaloonModel baloon)
        {
            if (baloon != null)
            {
                this.baloon = baloon;
                depth = baloon.GetAnimationDepth() + 0.1f;
            }
        }

        public void SetTarget(AnimalModel duck)
        {
            if (duck != null)
            {
                shootduckList.Add(duck);
                depth = duck.GetAnimationDepth() + 0.1f;
            }
        }
        public void AdjustForFlyEffect()
        {
            if (shootduckList.Count > 0 || parrot != null || baloon != null)
            {
                targetposition = relativePositionInParent;
                relativePositionInParent.X = relativePositionInParent.X - 20 * 6;
                relativePositionInParent.Y = relativePositionInParent.Y + 10 * 6;

            }
        }
    }


    class DuckIconModel : BaseModel
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
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            state = DuckIconState.Alive;

            space.Width = 19;
            space.Height = 19;
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            return resourceList;
        }

        override public ModelType Type()
        {
            return ModelType.DUCKICON;
        }

        override public void Initialize(ModelObject parent1, Rectangle rect, int seed)
        {
            base.Initialize(parent1, rect, seed);

            space = rect;
            relativePos.X = rect.Left;
            relativePos.Y = rect.Top;
            rect.Offset(-rect.Left, -rect.Top);

            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 19;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            state = DuckIconState.Alive;

            space.Width = 19;
            space.Height = 19;

        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absposition = relativePos;
            // adjust from lefttop conner to center
            if (GetParentObject() != null)
            {
                absposition += GetParentObject().GetAbsolutePosition();
            }
            return absposition;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1.0f;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
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
        override public float GetAnimationDepth()
        {
            return 0.3f;
        }

        override public int GetSoundIndex()
        {
            return -1;
        }


        public void SetState(DuckIconState state1)
        {
            state = state1;
        }
    }

    class HitBoardModel : BaseModel
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;
        List<DuckIconModel> duckIcons;
        int hitcount = 0;

        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        public int HitCount
        {
            get
            {
                return hitcount;
            }
        }

        public HitBoardModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // background
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 318;
            animationInfo.frameHeight = 63;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 200;
            space.Height = 63;

            duckIcons = new List<DuckIconModel>();

        }


        public HitBoardModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 318;
            animationInfo.frameHeight = 63;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            relativePosition = position1;

            space.Width = 200;
            space.Height = 63;

            duckIcons = new List<DuckIconModel>();
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\cnt_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }


        override public ModelType Type()
        {
            return ModelType.HITBOARD;
        }


        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            base.Initialize(null, rangespace, seed);
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return 0.35f;
        }


        override public List<ModelObject> GetChildrenObjects()
        {
            List<ModelObject> childrenlst = new List<ModelObject>();
            foreach (DuckIconModel child in duckIcons)
            {
                childrenlst.Add(child);
            }
            return childrenlst;
        }


        public int GetHitCount()
        {
            return hitcount;
        }
        public void AddHitCount(int hitCount)
        {
            hitcount += hitCount;
        }
    }



    class BulletIconModel : BaseModel
    {
        List<AnimationInfo> anationInfoList;
        Rectangle space;
        Vector2 relativePos;

        public BulletIconModel()
        {
            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 10;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            space.Width = 10;
            space.Height = 19;
        }


        override public ModelType Type()
        {
            return ModelType.BULLETICON;
        }

        override public void Initialize(ModelObject parent1, Rectangle rect, int seed)
        {
            base.Initialize(parent1, rect, seed);

            space = rect;
            relativePos.X = rect.Left;
            relativePos.Y = rect.Top;
            rect.Offset(-rect.Left, -rect.Top);

            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 10;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            space.Width = 10;
            space.Height = 19;

        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            return resourceList;
        }


        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absposition = relativePos;
            if (GetParentObject() != null)
            {
                absposition += GetParentObject().GetAbsolutePosition();
            }
            return absposition;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1.0f;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }
        override public float GetAnimationDepth()
        {
            return 0.3f;
        }
    }


    class BulletBoardModel : BaseModel
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;
        List<BulletIconModel> bulletIcons;
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        public BulletBoardModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // background
            AnimationInfo animationInfo = new AnimationInfo();
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

        public BulletBoardModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 78;
            animationInfo.frameHeight = 58;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            relativePosition = position1;

            space.Width = 318;
            space.Height = 63;

            bulletIcons = new List<BulletIconModel>();
        }


        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            return resourceList;
        }


        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1;
        }


        override public ModelType Type()
        {
            return ModelType.BULLETBOARD;
        }

        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {

            return 0.35f;
        }


        override public List<ModelObject> GetChildrenObjects()
        {
            List<ModelObject> childrenlst = new List<ModelObject>();
            foreach (BulletIconModel child in bulletIcons)
            {
                childrenlst.Add(child);
            }
            return childrenlst;
        }
           
        /// <summary>
        /// 
        /// </summary>
        public void RemoveFirstBullet()
        {
            if (bulletIcons.Count > 0)
            {
                bulletIcons.RemoveAt(0);
                SetViewObject(null);
            }
        }

        int bulletcount = 3;
        public void LoadBullet(int count)
        {
            if (bulletIcons.Count > 0)
            {
                bulletIcons.RemoveRange(0, bulletIcons.Count - 1);
            }
            SetViewObject(null);
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
    }



    class ScroeBoardModel : BaseModel
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;
        List<BulletIconModel> bulletIcons;

        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use


        public ScroeBoardModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // background
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 148;
            animationInfo.frameHeight = 65;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 200;
            space.Height = 63;

            bulletIcons = new List<BulletIconModel>();

        }

        public ScroeBoardModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 148;
            animationInfo.frameHeight = 65;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 200;
            space.Height = 63;

            bulletIcons = new List<BulletIconModel>();
        }


        override public ModelType Type()
        {
            return ModelType.SCOREBOARD;
        }

        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            base.Initialize(null, rangespace, seed);
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\cnt_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return 0.35f;
        }

        override public List<ModelObject> GetChildrenObjects()
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
            if (totalscore < 0)
            {
                totalscore = 0;
            }
        }

        public int TotalScore
        {
            get
            {
                return totalscore;
            }
        }

        int level = 1;
        public int GetLevel()
        {
            return level;
        }

        public void IncreaseLevel()
        {
            level++;
        }
    }

    class ScroeListBoardModel : BaseModel
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        Dictionary<int, string> scorelist;
        Dictionary<int, string> levellist;

        // score list
        public ScroeListBoardModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // background
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 500;
            animationInfo.frameHeight = 500;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 652;
            space.Height = 644;

            scorelist = new Dictionary<int, string>();
            levellist = new Dictionary<int, string>();

            this.AddScore("Penner", 3565);
            this.AddScore("Fallson", 5000);
        }

        public ScroeListBoardModel(Vector2 position1)
        {
            //
            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 652;
            animationInfo.frameHeight = 644;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 300;
            space.Height = 644;

            scorelist = new Dictionary<int, string>();
            levellist = new Dictionary<int, string>();

        }


        override public ModelType Type()
        {
            return ModelType.SCORELISTBOARD;
        }

        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            base.Initialize(null, rangespace, seed);
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();

            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT; resourceItm.path = "Graphics\\cnt_font_30";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT; resourceItm.path = "Graphics\\cnt_font_25";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT; resourceItm.path = "Graphics\\cnt_font_20";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT; resourceItm.path = "Graphics\\cnt_font_15";
            resourceList.Add(resourceItm);


            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return null;
        }
        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return 0.35f;
        }

        public void AddScore(string name, int score)
        {
            scorelist[score] = name;
        }
        public void AddLevel(string name, int level)
        {
            levellist[level] = name;
        }

        public Dictionary<int, string> ScoreList
        {
            get
            {
                return scorelist;
            }
            set
            {
                scorelist = value;
            }
        }

        public Dictionary<int, string> LevelList
        {
            get
            {
                return levellist;
            }
            set
            {
                levellist = value;
            }
        }
    }


    class TimeBoardModel : BaseModel
    {
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        public TimeBoardModel()
        {

            space.Width = 380;
            space.Height = 63;

        }

        public TimeBoardModel(Vector2 position1)
        {
            // get least of duck icon

            space.Width = 380;
            space.Height = 63;
        }


        override public ModelType Type()
        {
            return ModelType.TIMEBOARD;
        }

        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            base.Initialize(null, rangespace, seed);
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            //resourceItm.path = "Graphics\\font";
            resourceItm.path = "Graphics\\cnt_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1;
        }


        override public void Update(GameTime gameTime)
        {
            // no update for itself
            if (lefttime >= 0)
            {
                lefttime -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (lefttime < 0)
            {
                lefttime = 0;
            }

            // update child object
        }

        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return null;
        }
        override public int GetCurrentAnimationIndex()
        {
            return -1;
        }

        override public float GetAnimationDepth()
        {
            return 0.35f;
        }


        double lefttime = 0;
        public void SetTime(int time)
        {
            lefttime = time;
        }

        public int LeftTime
        {
            get
            {
                return (int)lefttime;
            }
        }
    }



    class LevelUpBoardModel : BaseModel
    {
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        float scale = 1.0f;
        float rotate = 0.0f;
        float deltascale = 0.1f;

        int stopcount = 0;

        public LevelUpBoardModel()
        {

            space.Width = 300;
            space.Height = 63;

        }

        override public ModelType Type()
        {
            return ModelType.LEVELUPBOARD;
        }

        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            base.Initialize(null, rangespace, seed);
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\cnt_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1;
        }


        override public void Update(GameTime gameTime)
        {
            // no update for itself
            if (lefttime >= 0)
            {
                lefttime -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (lefttime < 0)
            {
                lefttime = 0;
            }

            if (stopcount > 0)
            {
                stopcount--;
                return;
            }

            scale += deltascale;
            if (scale > 1.5f && deltascale > 0)
            {
                deltascale = -1.5f * deltascale;
                stopcount = 30;
            }
            rotate = 0f;
            if (scale < 0)
            {
                scale = 0;
            }
            /*
            rotate += 0.4f;
            if (rotate >= 6.28f*4)
            {
                rotate = 6.28f*4 + 0.01f;
            }
             */
        }

        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return null;
        }
        override public int GetCurrentAnimationIndex()
        {
            return -1;
        }

        override public float GetAnimationDepth()
        {
            return 0.35f;
        }

        double lefttime = 0;
        public void SetTime(int time)
        {
            lefttime = time;
        }

        public bool Show()
        {
            if (lefttime > 0)
            {
                return true;
            }
            return false;
        }

        public float Scale
        {
            get
            {
                return scale;
            }
        }

        public float Rotate
        {
            get
            {
                return rotate;
            }
        }


        public void Reset()
        {
            lefttime = 3;
            scale = 1.0f;
            rotate = 0f;
            deltascale = 0.01f;
            stopcount = 0;
        }
    }

    enum ButtonStyle
    {
        RESTART, PAUSE
    }

    class ButtonModel : BaseModel
    {
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        bool _checked = true;

        List<AnimationInfo> anationInfoList;

        ButtonStyle style = ButtonStyle.PAUSE;

        public ButtonModel()
        {
            space.Width = 256;
            space.Height = 256;

            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 128;
            animationInfo.frameHeight = 128;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo.frameWidth = 128;
            animationInfo.frameHeight = 128;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo.frameWidth = 128;
            animationInfo.frameHeight = 128;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);
        }


        public ButtonModel(ButtonStyle style)
        {
            this.style = style;

            space.Width = 256;
            space.Height = 256;

            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 128;
            animationInfo.frameHeight = 128;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo.frameWidth = 128;
            animationInfo.frameHeight = 128;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo.frameWidth = 128;
            animationInfo.frameHeight = 128;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);
        }

        override public ModelType Type()
        {
            return ModelType.BUTTON;
        }

        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            base.Initialize(null, rangespace, seed);
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\Pause";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\continue";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\continue";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\cnt_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 0.5f;
        }


        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {
            if (style == ButtonStyle.PAUSE)
            {
                if (_checked)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 2;
            }
        }

        override public float GetAnimationDepth()
        {
            return 0.01f;
        }



        string content;
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
            }
        }

        public bool Click(Vector2 logicpos)
        {
            // check if it clicked
            Vector2 lefttop = this.GetAbsolutePosition();
            lefttop.X += 128 / 2;
            lefttop.Y += 128 / 2;
            lefttop.X -= (int)(128 * GetSacle()/2);
            lefttop.Y -= (int)(128 * GetSacle()/2);
            int boxsize = (int)(128 * GetSacle());
            if (logicpos.X > lefttop.X && logicpos.X - lefttop.X <= boxsize &&
                logicpos.Y > lefttop.Y && logicpos.Y - lefttop.Y <= boxsize)
            {
                // clicked
                _checked = !_checked;
                return true;
            }

            return false;
        }
    }



    class CheckBoxModel : BaseModel
    {
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        bool _checked = false;

        List<AnimationInfo> anationInfoList;

        public CheckBoxModel()
        {
            space.Width = 300;
            space.Height = 63;

            anationInfoList = new List<AnimationInfo>();

            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 86;
            animationInfo.frameHeight = 107;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            animationInfo.frameWidth = 86;
            animationInfo.frameHeight = 107;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);


        }

        override public ModelType Type()
        {
            return ModelType.CHECKBOX;
        }

        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            base.Initialize(null, rangespace, seed);
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\checkbox_checked";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\checkbox_unchecked";
            resourceList.Add(resourceItm);
            
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\cnt_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1;
        }

        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {
            if (_checked)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        override public float GetAnimationDepth()
        {
            return 0.35f;
        }

        override public int GetSoundIndex()
        {
            return -1;
        }


        string content;
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
            }
        }

        public bool Click(Vector2 logicpos)
        {
             // check if it clicked
            Vector2 lefttop = this.GetAbsolutePosition();
            lefttop.Y += 35;
            int boxsize = 60;
            if (logicpos.X > lefttop.X && logicpos.X - lefttop.X <= boxsize &&
                logicpos.Y > lefttop.Y && logicpos.Y - lefttop.Y <= boxsize)
            {
                // clicked
                _checked = !_checked;
                return true;
            }

            return false;
        }
    }



    class LostDuckBoardModel : BaseModel
    {
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        public LostDuckBoardModel()
        {
            //
  
            // get least of duck icon

            space.Width = 220;
            space.Height = 63;

        }

        public LostDuckBoardModel(Vector2 position1)
        {
            //
            // get least of duck icon

            space.Width = 220;
            space.Height = 63;
        }


        override public ModelType Type()
        {
            return ModelType.LOSTDUCKBOARD;
        }

        override public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            base.Initialize(null, rangespace, seed);
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
            resourceItm.path = "Graphics\\cnt_font_30";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            return space;
        }
        override public float GetSacle()
        {
            return 1;
        }


        double lasttime = 0;
 
        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return null;
        }
        override public int GetCurrentAnimationIndex()
        {
            return -1;
        }

        override public float GetAnimationDepth()
        {
            return 0.35f;
        }



        int lostDuckCount = 0;
        public void AddDuck(int count)
        {
            lostDuckCount += count;
        }
        public void ResetLostCount()
        {
            lostDuckCount = 0;
        }

        public int LostDuckCount
        {
            get
            {
                return lostDuckCount;
            }
        }
    }

    class PandaModel : BaseModel
    {
        enum PANDASTATE { Running, Dancing, Laughing };
        PANDASTATE state = PANDASTATE.Running;


        // Animation representing the player

        List<AnimationInfo> anationInfoList;
        Rectangle pandaspace;

        bool gone = false;


        float depth = 0.4F;

        Vector2 relativePos;

        Vector2 RelativePosition
        {
            get
            {
                return  relativePos;
            }
        }

        public PandaModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 150;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 5;
            animationInfo.frameTime = 150;
            anationInfoList.Add(animationInfo);

        }



        override public ModelType Type()
        {
            return ModelType.PANDA;
        }


        override public void Initialize(ModelObject parent1, Rectangle dogSpace, int seed)
        {
            base.Initialize(null, dogSpace, seed);

            pandaspace = dogSpace;
            relativePos.X = dogSpace.Right;
            relativePos.Y = dogSpace.Bottom;
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\panda_run";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = RelativePosition;
            // adjust from lefttop conner to center
            abspos.X -= anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            abspos.Y -= anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            Rectangle space = new Rectangle(0, 0
            , anationInfoList[GetCurrentAnimationIndex()].frameWidth
            , anationInfoList[GetCurrentAnimationIndex()].frameHeight);

            return space;
        }
        override public float GetSacle()
        {
            return 1.0f;
        }


        override public void Update(GameTime gameTime)
        {
            relativePos.X -= 3;
            if (relativePos.X <= 0)
            {
                relativePos.X = pandaspace.Right;
            }


        }

        override public List<AnimationInfo>  GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }
    }


    class FireworkModel : BaseModel
    {
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        Rectangle fireworkspace;

        bool gone = false;

        float depth = 0.001F;

        Vector2 relativePos;

        Vector2 RelativePosition
        {
            get
            {
                return relativePos;
            }
        }

        public FireworkModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // firework
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 1101;
            animationInfo.frameHeight = 742;
            animationInfo.frameCount = 10;
            animationInfo.frameTime = 150;
            anationInfoList.Add(animationInfo);
        }



        override public ModelType Type()
        {
            return ModelType.FIREWORK;
        }


        override public void Initialize(ModelObject parent1, Rectangle space, int seed)
        {
            base.Initialize(null, space, seed);

            fireworkspace = space;
            relativePos.X = space.Left;
            relativePos.Y = space.Top;
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks1";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks2";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks3";
            resourceList.Add(resourceItm);



            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks5";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks7";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks8";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks9";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks10";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\fireworks11";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = RelativePosition;
            // adjust from lefttop conner to center
            /*
            abspos.X -= anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            abspos.Y -= anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;
             */
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            Rectangle space = new Rectangle(0, 0
            , anationInfoList[GetCurrentAnimationIndex()].frameWidth
            , anationInfoList[GetCurrentAnimationIndex()].frameHeight);

            return space;
        }
        override public float GetSacle()
        {
            return 1.0f;
        }


        override public void Update(GameTime gameTime)
        {
            /*
            relativePos.X -= 3;
            if (relativePos.X <= 0)
            {
                relativePos.X = fireworkspace.Left;
            }
             */
        }

        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }
    }



    class PlaneModel : BaseModel
    {
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        Rectangle planespace;
        Rectangle crashspace;
        float depth = 0.8F;

        Vector2 relativePos;

        Vector2 RelativePosition
        {
            get
            {
                return relativePos;
            }
        }

        List<Vector2> triangle = null;

        bool gone = false;
        bool dead = false;

        public bool Gone
        {
            get
            {
                return gone;
            }
        }

        int planeindex = 0;


        public PlaneModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 500;
            animationInfo.frameHeight = 100;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 250;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 500;
            animationInfo.frameHeight = 100;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 250;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 500;
            animationInfo.frameHeight = 100;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 250;
            anationInfoList.Add(animationInfo);

            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 500;
            animationInfo.frameHeight = 100;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 250;
            anationInfoList.Add(animationInfo);

            triangle = new List<Vector2>();




        }



        override public ModelType Type()
        {
            return ModelType.PLANE;
        }


        override public void Initialize(ModelObject parent1, Rectangle space, int seed)
        {
            base.Initialize(null, space, seed);

            planespace = space;
            crashspace = space;

            planespace.Y = 100;
            planespace.Height = 150;

            relativePos.X = planespace.Left;
            relativePos.Y = planespace.Bottom;

            Random radom = new Random(seed);

            planeindex = radom.Next(3);

        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\plane11";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\plane22";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\plane33";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\plane44";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = RelativePosition;
            // adjust from lefttop conner to center
            abspos.X -= anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            abspos.Y -= anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            Rectangle space = new Rectangle(0, 0
            , anationInfoList[GetCurrentAnimationIndex()].frameWidth
            , anationInfoList[GetCurrentAnimationIndex()].frameHeight);

            return space;
        }
        override public float GetSacle()
        {
            return 1.0f;
        }


        override public void Update(GameTime gameTime)
        {
            if (dead == false)
            {
                relativePos.X += 3;
            }
            else
            {
                relativePos.X += 3;
                relativePos.Y += 3;
            }
            if (relativePos.X >= crashspace.Right /*|| relativePos.Y >= crashspace.Bottom*/)
            {
                //relativePos.X = planespace.Left;
                gone = true;
            }
        }

        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {
            return planeindex;
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }


        public void Shoot(BulletModel bullet)
        {
            // check if it's shoot

            Vector2 position = bullet.GetAbsolutePosition();
            Rectangle bulletRc = bullet.GetSpace();
            Vector2 bulletCenter = position;
            bulletCenter.X += bulletRc.Width / 2;
            bulletCenter.Y += bulletRc.Height / 2;

            Vector2 planeLeftTop = GetAbsolutePosition();
            planeLeftTop.X += anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            planeLeftTop.Y += anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;
            planeLeftTop.X -= anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2 * this.GetSacle();
            planeLeftTop.Y -= anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2 * this.GetSacle();

            Vector2 p1 = Vector2.Zero, p2 = Vector2.Zero, p3 = Vector2.Zero, p4 = Vector2.Zero;
            p1.X = planeLeftTop.X + 273 * GetSacle();
            p1.Y = planeLeftTop.Y + 119 * GetSacle();
            p2.X = planeLeftTop.X + 332 * GetSacle();
            p2.Y = planeLeftTop.Y + 98 * GetSacle();
            p3.X = planeLeftTop.X + 390 * GetSacle();
            p3.Y = planeLeftTop.Y + 128 * GetSacle();
            p4.X = planeLeftTop.X + 309 * GetSacle();
            p4.Y = planeLeftTop.Y + 144 * GetSacle();

            triangle.Clear();
            triangle.Add(p1);
            triangle.Add(p2);
            triangle.Add(p3);
            if (CollectionDetect.PointInTriangle(bulletCenter, triangle))
            {
                //
               // bullet.SetTarget(this);
                dead = true;
                return;
            }

            triangle.Clear();
            triangle.Add(p2);
            triangle.Add(p3);
            triangle.Add(p4);
            if (CollectionDetect.PointInTriangle(bulletCenter, triangle))
            {
                //
              //  bullet.SetTarget(this);
                dead = true;
                return;
            }

        }
    }

 
    class ParrotModel : AnimalModel
    {
        DuckModel internalModel;

        AiPilot flyPilot;
        AiPilot deadPilot;

        public bool Active = true;
        public bool dead = false;
        bool gone = false;

        int deadstopcount = 0;
        Rectangle flyspace;

        int randomseed = 0;
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        float scale = 1.0f;
        float depth = 0.6f;

        public ParrotModel()
        {
            anationInfoList = new List<AnimationInfo>();
            //
            flyPilot = PilotManager.GetInstance().CreatePilot(PilotType.PARROT, "");
            // d. red bird


            // 0. flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 6;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 6;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);
        }


        string pilotgroupname = "";
        PilotType pilotType = PilotType.DUCKNORMAL;
        public ParrotModel(PilotType type, string groupname)
        {
            pilotgroupname = groupname;
            anationInfoList = new List<AnimationInfo>();
            //
            flyPilot = PilotManager.GetInstance().CreatePilot(type, groupname);
            // d. red bird


            // 0. flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 6;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);

            //1. dying duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
            anationInfoList.Add(animationInfo);

            // 2. dead duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 2;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 160;
            animationInfo.frameHeight = 172;
            animationInfo.frameCount = 6;
            animationInfo.frameTime = 200;
            anationInfoList.Add(animationInfo);

        }

        override public ModelType Type()
        {
            return ModelType.PARROT;
        }


        Random radom ;
        override public void Initialize(ModelObject parent1, Rectangle duckSpace, int seed)
        {
            base.Initialize(null, duckSpace, seed);

            // Set the player to be active
            Active = true;

            // Set the player health
            randomseed = seed;
            radom = new Random(seed);

            flyspace = duckSpace;
        }


        public override void Cleanup()
        {
            PilotManager.GetInstance().ReturnPilot(pilotgroupname, this.flyPilot);

            this.flyPilot = null;
            this.deadPilot = null;
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();

            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\red_bird_r";
            //resourceItm.path = "Graphics\\duckrflying";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\red_bird_dying";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\red_bird_dead";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\red_bird";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\parrot";
            resourceList.Add(resourceItm);
            /*
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\godbless";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\hatedog";
            resourceList.Add(resourceItm);
            */
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\helpme";
            resourceList.Add(resourceItm);


            return resourceList;
        }



        Vector2 RelativePosition
        {
            get
            {
                if (flyPilot == null)
                {
                    return Vector2.Zero;
                }

                if (Active)
                    return flyPilot.GetPosition();
                else
                    return deadPilot.GetPosition();
            }
        }


        //Rectangle space;
        override public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = RelativePosition;

            // pilot return center postion, adjust it to left top conner
            absPos.X -= 105 / 2;
            absPos.Y -= 102 / 2;

            return absPos;
        }
        override public Rectangle GetSpace()
        {
            return flyspace;
        }
        override public float GetSacle()
        {
            if (Active)
            {
                // get depth, calculate the scale

                //scale = autoPilot.scale;
                scale = 1 - flyPilot.GetDepth() * 1.0f / 100;
            }

            return scale;
        }

        override public void SetSpeedRatio(float ratio)
        {
        }


        override public void Update(GameTime gameTime)
        {
            if (Active)
            {
                flyPilot.Update(gameTime);

                // check if it need to go
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime > 1000 * 15)
                {
                    Active = false;
                    deadPilot = PilotManager.GetInstance().CreatePilot(PilotType.PARROT, flyPilot.GetPosition());
                    deadPilot.Initialize(this.flyspace, 0);
                }
            }
            else
            {
                if (deadstopcount < 10)
                {
                    deadstopcount++;
                }
                deadPilot.Update(gameTime);
                if (deadPilot.GetPosition().Y > flyspace.Height ||
                    deadPilot.GetPosition().Y < 0 - anationInfoList[GetCurrentAnimationIndex()].frameHeight)
                {
                    gone = true;
                }
            }

        }


        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        override public int GetCurrentAnimationIndex()
        {
            if (dead)
            {
                if (deadstopcount < 10)
                {
                    return 1 ;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                if (Active)
                {
                    if (flyPilot.GetHorizationDirection() == Direction.LEFT)
                    {
                        return 0;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    if (deadPilot.GetHorizationDirection() == Direction.LEFT)
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

        override public float GetAnimationDepth()
        {
            return depth;
        }

        bool barked = false;
        override public int GetSoundIndex()
        {
            if (!dead)
            {
                if (!barked && elapsedTime > 500)
                {
                    barked = true;
                    return radom.Next(2);
                }
            }
            return -1;
        }

        override public float GetSoundVolumn()
        {
            return 1.0f;
        }

        /// <summary>
        ///  specific functions
        /// </summary>

        override public void StartPilot()
        {
            // Set the starting position of the player around the middle of the screen and to the back

            flyPilot.Initialize(flyspace, randomseed);
        }

        override public void StartPilot(Vector2 pos)
        {
            flyPilot.Initialize(flyspace, randomseed);
        }

        override public void Shoot(BulletModel bullet)
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

            Vector2 duckCenter = GetAbsolutePosition();
            Vector2 bullet2DuckPos = bulletCenter - duckCenter;


            //
            float r = anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2; // 
            r = 40;
            duckCenter.X += anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            duckCenter.Y += anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;

            Vector2 subpos = bulletCenter - duckCenter;
            if (subpos.Length() < r * scale)
            {
                Active = false;
                dead = true;
                deadPilot = PilotManager.GetInstance().CreatePilot(PilotType.DUCKDEAD, flyPilot.GetPosition());

                // new a bullet  
                bullet.SetTarget(this);
            }

        }


        override public bool Gone()
        {
            return gone;
        }
        override public bool Dead()
        {
            return dead;
        }

        override public void SetStartPos(Vector2 startPos)
        {
            flyPilot.SetStartPos(startPos);
        }

        override public void SetEndPos(Vector2 endPos)
        {
            flyPilot.SetEndPos(endPos);
        }

        public override void SetShowTime(int seconds)
        {
            
        }
    }

    class BaloonModel : BaseModel
    {
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        Rectangle planespace;
        float depth = 0.4F;

        Vector2 relativePos;

        bool gone = false;
        bool active = true;


        Vector2 RelativePosition
        {
            get
            {
                return relativePos;
            }
        }


        public bool Gone
        {
            get
            {
                return gone;
            }
        }

        public BaloonModel()
        {
            //
            anationInfoList = new List<AnimationInfo>();

            // flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 402;
            animationInfo.frameHeight = 595;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

        }



        override public ModelType Type()
        {
            return ModelType.BALOON;
        }


        override public void Initialize(ModelObject parent1, Rectangle space, int seed)
        {
            base.Initialize(null, space, seed);

            planespace = space;
            relativePos.X = space.Left;
            relativePos.Y = space.Bottom;
        }

        override public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\balloon";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        override public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = RelativePosition;
            // adjust from lefttop conner to center
            abspos.X -= anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            abspos.Y -= anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;
            return abspos;
        }

        override public Rectangle GetSpace()
        {
            Rectangle space = new Rectangle(0, 0
            , anationInfoList[GetCurrentAnimationIndex()].frameWidth
            , anationInfoList[GetCurrentAnimationIndex()].frameHeight);

            return space;
        }
        override public float GetSacle()
        {
            return 0.5f;
        }


        override public void Update(GameTime gameTime)
        {
            relativePos.X += 3;
            if (relativePos.X >= planespace.Right)
            {
                //relativePos.X = planespace.Left;
                gone = true;
            }
        }

        override public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        override public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        override public float GetAnimationDepth()
        {
            return depth;
        }


        public void Shoot(BulletModel bullet)
        {
            if (!active)
            {
                return;
            }
            // check if it's shoot
            // calculate baloon center ( 190, 164), r = 150
            Vector2 baloonCenter = GetAbsolutePosition();
            baloonCenter.X += anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            baloonCenter.Y += anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;
            baloonCenter.Y -= anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2 * GetSacle();
            baloonCenter.Y += 164 * GetSacle();

            Vector2 bulletCenter = bullet.GetAbsolutePosition();
            bulletCenter.X += bullet.GetSpace().Width / 2;
            bulletCenter.Y += bullet.GetSpace().Height / 2;

            Vector2 subpos = bulletCenter - baloonCenter;
            float r = 150;
            if (subpos.Length() > r * GetSacle())
            {
                return;
            }

            bullet.SetTarget(this);
            // if shoot, 
            active = false;
        }
    }

    class CollectionDetect
    {
        public static bool PointInTriangle(Vector2 p1, List<Vector2> triangle)
        {
            return _isPointInsideTriangle(triangle, p1);
        }
        public static bool BoundingTriangles(List<Vector2> p1, List<Vector2> p2)
        {
            for (int i = 0; i < 3; i++)
                if (_isPointInsideTriangle(p1, p2[i])) return true;

            for (int i = 0; i < 3; i++)
                if (_isPointInsideTriangle(p2, p1[i])) return true;
            return false;
        }
        private static bool _isPointInsideTriangle(List<Vector2> TrianglePoints, Vector2 p)
        {
            // Translated to C# from: http://www.ddj.com/184404201
            Vector2 e0 = p - TrianglePoints[0];
            Vector2 e1 = TrianglePoints[1] - TrianglePoints[0];
            Vector2 e2 = TrianglePoints[2] - TrianglePoints[0];

            float u, v = 0;
            if (e1.X == 0)
            {
                if (e2.X == 0) return false;
                u = e0.X / e2.X;
                if (u < 0 || u > 1) return false;
                if (e1.Y == 0) return false;
                v = (e0.Y - e2.Y * u) / e1.Y;
                if (v < 0) return false;
            }
            else
            {
                float d = e2.Y * e1.X - e2.X * e1.Y;
                if (d == 0) return false;
                u = (e0.Y * e1.X - e0.X * e1.Y) / d;
                if (u < 0 || u > 1) return false;
                v = (e0.X - e2.X * u) / e1.X;
                if (v < 0) return false;
                if ((u + v) > 1) return false;
            }

            return true;
        }
    }
}
