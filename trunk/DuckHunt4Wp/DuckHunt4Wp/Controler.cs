using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.GamerServices;
#if !WINDOWS_PHONE
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using System.Diagnostics;
using System.IO;
#endif
using System.Xml;
using GameCommon;
#if !WINDOWS_PHONE
using System.Threading.Tasks;
#else
using System.IO.IsolatedStorage;
#endif


namespace DuckHuntCommon 
{
    class ObjectTexturesItem
    {
        public ModelType objType;
        public List<Texture2D> textureList;
        public List<SpriteFont> fontList;
        public List<SoundEffect> soundList;
        
    }

    class ViewObjectFactory
    {
        static Vector2 s_playgroundOrgPoint;
        static float s_playgroundDefScale;
        static Vector2 s_backgroundOrgPoint;
        static float s_backgroundDefScale;
        static Rectangle s_localViewRect;

        public static void SetLocalViewInfo(Rectangle localViewRc, Vector2 playgroundOrgPoint, float playgroundDefScale, Vector2 backgroundOrgPoint, float backgroundDefScale)
        {
            s_playgroundOrgPoint = playgroundOrgPoint;
            s_playgroundDefScale = playgroundDefScale;
            s_backgroundOrgPoint = backgroundOrgPoint;
            s_backgroundDefScale = backgroundDefScale;
            s_localViewRect = localViewRc;
        }

        public static ViewObject CreateViewObject(ModelObject model)
        {
            CommonViewObject commViewObj = null;
            ScoreBoardViewObject scoreboardObj = null;
            HitBoardViewObject hitBoardObj = null;
            ScoreListBoardViewObject scorelistobj = null;
            ViewObject viewObject = null;
            switch (model.Type())
            {
                case ModelType.SKY:
                case ModelType.CLOUD:
                case ModelType.GRASS:
                case ModelType.FORGROUND:
                    {
                        commViewObj = new CommonViewObject(model, s_backgroundOrgPoint, s_backgroundDefScale);
                    }
                    break;
                case ModelType.PANDA:
                case ModelType.DOG:
                case ModelType.DUCK:
                case ModelType.BULLET:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.HITBOARD:
                    {
                        hitBoardObj = new HitBoardViewObject(model);
                    }
                    break;
                case ModelType.SCORELISTBOARD:
                    {
                        scorelistobj = new ScoreListBoardViewObject(model);
                    }
                    break;
                case ModelType.DUCKICON:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.BULLETBOARD:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.BUTTON:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.KEYITEM:
                    {
                        //Vector2 ogpoint = Vector2.Zero;
                        viewObject = new KeyItemViewObject(model/*, Vector2.Zero, 1.0f*/);
                    }
                    break;
                case ModelType.KEYBORD:
                    {
                        viewObject = new KeyboardViewObject(model, Vector2.Zero, 1.0f);
                    }
                    break;
                case ModelType.BULLETICON:
                    {
                        commViewObj = new CommonViewObject(model, s_playgroundOrgPoint, s_playgroundDefScale);
                    }
                    break;
                case ModelType.SCOREBOARD:
                    {
                        scoreboardObj = new ScoreBoardViewObject(model);
                    }
                    break;
                case ModelType.MENUITEM:
                    {
                        viewObject = new MenuItemViewObject(model);
                    }
                    break;
                case ModelType.TIMEBOARD:
                    {
                        viewObject = new TimeBoardViewObject(model);
                    }
                    break;
                case ModelType.LOSTDUCKBOARD:
                    {
                        viewObject = new LostDuckBoardViewObject(model);
                    }
                    break;
                case ModelType.CHECKBOX:
                    {
                        viewObject = new CheckBoxViewObject(model);
                    }
                    break;
                case ModelType.FIREWORK:
                    {
                        viewObject = new FireworkViewObject();
                    }
                    break;
            }
            if (commViewObj != null)
            {
                commViewObj.screenRc = s_localViewRect;
                viewObject = commViewObj;
            }
            if (scoreboardObj != null)
            {
                viewObject = scoreboardObj;
            }
            if( hitBoardObj != null )
            {
                viewObject = hitBoardObj;
            }
            if (scorelistobj != null)
            {
                viewObject = scorelistobj;
            }
            return viewObject;
        }
    }

    class DuckHuntGameControler
    {
        ContentManager Content = null;
        DuckHuntGame game = null;
        Rectangle viewRect = new Rectangle();

        Dictionary<ModelType, ObjectTexturesItem> objTextureLst;

        bool pause = false;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize(Rectangle viewScene)
        {
            game = new DuckHuntGame();
            viewRect = viewScene;
            objTextureLst = new Dictionary<ModelType, ObjectTexturesItem>();


        }

        Song gamebackgorundsound;
        bool bgSoundEnabled = false;
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent(ContentManager Content1)
        {
            // TODO: use this.Content to load your game content here
            Content = Content1;
            LoadResources();

            List<GameSound> soundList = game.GetSoundList();
            gamebackgorundsound = Content.Load<Song>(soundList[0].soundpath);
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Volume = 50;
            game.StartGame(viewRect);

            if (game.DuckHuntGameData.EnableBgMusic)
            {
                MediaPlayer.Play(gamebackgorundsound);
            }
            bgSoundEnabled = game.DuckHuntGameData.EnableBgMusic;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here


        }


        public void LoadResources()
        {
            List<ObjectResourceItem> resourceList;
            game.GetResources(out resourceList);
            if (resourceList != null)
            {
                foreach (ObjectResourceItem objResourceItm in resourceList)
                {
                    ObjectTexturesItem textureItm;
                    textureItm = new ObjectTexturesItem();
                    textureItm.objType = objResourceItm.objType;
                    textureItm.textureList = new List<Texture2D>();
                    textureItm.fontList = new List<SpriteFont>();
                    textureItm.soundList = new List<SoundEffect>();

                    foreach (ResourceItem resourceItm in objResourceItm.resourceList)
                    {
                        if (resourceItm.type == ResourceType.TEXTURE)
                        {
                            textureItm.textureList.Add(Content.Load<Texture2D>(resourceItm.path));
                        }
                        else if (resourceItm.type == ResourceType.FONT)
                        {
                            textureItm.fontList.Add(Content.Load<SpriteFont>(resourceItm.path));
                        }
                        else if (resourceItm.type == ResourceType.SOUND)
                        {
                            textureItm.soundList.Add(Content.Load<SoundEffect>(resourceItm.path));
                        }
                    }
                    objTextureLst[textureItm.objType] = textureItm;
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            game.Update(gameTime);
            if (game.DuckHuntGameData.EnableBgMusic != bgSoundEnabled)
            {
                //
                bgSoundEnabled = game.DuckHuntGameData.EnableBgMusic;
                if (bgSoundEnabled)
                {
                    MediaPlayer.Play(gamebackgorundsound);
                }
                else
                {
                    MediaPlayer.Pause();
                }
            }
          
            if (pause)
            {
                pause = game.Pause;
                return;
            }
            pause = game.Pause;

            List<ModelObject> objlst = null;
            game.GetObjects(out objlst);
            if (objlst == null)
            {
                return;
            }

            foreach (ModelObject obj in objlst)
            {
                ViewObject viewObject = obj.GetViewObject();
                if (viewObject == null)
                {
                    //viewObject = ViewObjectFactory.CreateViewObject(this.viewRect, obj, game.orgpoint, game.defscale);
                    Vector2 bgorgpoint = game.GetBgOrgPointInLocalView();
                    float bgdefscale = game.GetLogicBgDefScale();
                    viewObject = ViewObjectFactory.CreateViewObject(obj);

                    viewObject.Init(game.orgpoint, game.defscale, obj, objTextureLst, obj.GetSpace());
                    obj.SetViewObject(viewObject);
                }
                viewObject.Update(gameTime);

                if (game.DuckHuntGameData.EnableGameSound)
                {
                    viewObject.PlaySound();
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // TODO: Add your drawing code here
            List<ModelObject> objlst = null;
            game.GetObjects(out objlst);
            if (objlst == null)
            {
                return;
            }

            foreach (ModelObject obj in objlst)
            {
                ViewObject viewObject = obj.GetViewObject();
                if (viewObject == null)
                {
                    Vector2 bgorgpoint = game.GetBgOrgPointInLocalView();
                    float bgdefscale = game.GetLogicBgDefScale();
                    viewObject = ViewObjectFactory.CreateViewObject(obj);
                    viewObject.Init(game.orgpoint, game.defscale, obj, objTextureLst, obj.GetSpace());
                    obj.SetViewObject(viewObject);
                    obj.Update(gameTime);
                }
                viewObject.Draw(spriteBatch);
            }
        }


        public void Click(List<Vector2> clickPositions)
        {
            //
            // local rect, global rect
            // (local rect - orgpoint ) = global rect * default scale
            // local rect = orgpoint + global rect * default scale
            // global rect = (local rect - orgpoint)/ defalult scale
            //
            //Vector2 globalshotpos = (shootPosition - game.orgpoint) / game.defscale;
            List<Vector2> globalpointposlst = new List<Vector2>();
            foreach (Vector2 localclickpos in clickPositions)
            {
                globalpointposlst.Add((localclickpos - game.orgpoint) / game.defscale);
            }
            game.Click(globalpointposlst);
        }

    }


}
