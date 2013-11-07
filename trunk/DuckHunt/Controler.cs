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
using Microsoft.Xna.Framework.GamerServices;
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
        /*
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

            Camera.CameraScale = s_playgroundDefScale;
            Camera.ViewPort = s_playgroundOrgPoint;
        }
        */
        public static CXShowNode CreateViewObject(ModelObject model)
        {
            CommonViewObject commViewObj = null;
            CXShowNode viewObject = null;
            switch (model.Type())
            {
                case ModelType.SMOKE:
                    {
                        viewObject = new SmokeViewObject(model);
                    }
                    break;
                case ModelType.CLOUD:
                case ModelType.GRASS:
                case ModelType.FORGROUND:
                case ModelType.SKY:
                    {
                        commViewObj = new CommonViewObject(model/*, s_backgroundOrgPoint, s_backgroundDefScale*/);
                        commViewObj.BackGround = true;
                        viewObject = commViewObj;
                    }
                    break;
                case ModelType.PANDA:
                case ModelType.DOG:
                case ModelType.DUCK:
                case ModelType.BULLET:
                case ModelType.PLANE:
                case ModelType.BALOON:
                case ModelType.PARROT:
                    {
                        viewObject = new CommonViewObject(model/*, s_playgroundOrgPoint, s_playgroundDefScale*/);
                    }
                    break;
                case ModelType.HITBOARD:
                    {
                        HitBoardViewObject hitBoardObj = new HitBoardViewObject(model);
                        //hitBoardObj.BgOrgPoint = s_backgroundOrgPoint;
                        viewObject = hitBoardObj;
                    }
                    break;
                case ModelType.SCORELISTBOARD:
                    {
                        viewObject = new ScoreListBoardViewObject(model);
                    }
                    break;
                case ModelType.LEVELUPBOARD:
                    {
                        viewObject = new LevelUpBoardViewObject(model);
                    }
                    break;
                case ModelType.DUCKICON:
                    {
                        viewObject = new CommonViewObject(model/*, s_playgroundOrgPoint, s_playgroundDefScale*/);
                    }
                    break;
                case ModelType.BULLETBOARD:
                    {
                        viewObject = new CommonViewObject(model/*, s_playgroundOrgPoint, s_playgroundDefScale*/);
                    }
                    break;
                case ModelType.BUTTON:
                    {
                        viewObject = new CommonViewObject(model/*, s_playgroundOrgPoint, s_playgroundDefScale*/);
                    }
                    break;
                    /*
                case ModelType.KEYITEM:
                    {
                        //Vector2 ogpoint = Vector2.Zero;
                        viewObject = new KeyItemViewObject(model);
                    }
                    break;
                    */
                    /*
                case ModelType.KEYBORD:
                    {
                        viewObject = new KeyboardViewObject(model, Vector2.Zero, 1.0f);
                    }
                    break;
                     */
                case ModelType.BULLETICON:
                    {
                        commViewObj = new CommonViewObject(model/*, s_playgroundOrgPoint, s_playgroundDefScale*/);
                    }
                    break;
                case ModelType.SCOREBOARD:
                    {
                        viewObject = new ScoreBoardViewObject(model);
                    }
                    break;
                case ModelType.MENUITEM:
                    {
                        viewObject = new MenuItemViewObject(model);
                    }
                    break;
                case ModelType.TITLEITEM:
                    {
                        viewObject = new TitleItemViewObject(model);
                    }
                    break;
                case ModelType.TIMEBOARD:
                    {
                        viewObject = new TimeBoardViewObject(model);
                        //((TimeBoardViewObject)viewObject).BgOrgPoint = s_backgroundOrgPoint;
                    }
                    break;
                case ModelType.LOSTDUCKBOARD:
                    {
                        viewObject = new LostDuckBoardViewObject(model);
                        //((LostDuckBoardViewObject)viewObject).BgOrgPoint = s_backgroundOrgPoint;
                    }
                    break;
                case ModelType.RESULTSUMMARY:
                    {
                        viewObject = new ResultSummaryViewObject(model);
                    }
                    break;
                case ModelType.CHECKBOX:
                    {
                        viewObject = new CheckBoxViewObject(model);
                    }
                    break;
                    /*
                case ModelType.FIREWORK:
                    {
                        viewObject = new FireworkViewObject();
                    }
                    break;
                     */
                case ModelType.INFOBORD:
                    {
                        viewObject = new InfoBoardViewObject(model);
                    }
                    break;
                case ModelType.HUNTER:
                    {
                        viewObject = new HunterViewObject(model);
                    }
                    break;
                case ModelType.ARROW:
                    {
                        viewObject = new ArrowViewObject(model);
                    }
                    break;
            }

            return viewObject;
        }
    }

    class DuckHuntGameControler
    {
        public static DuckHuntGameControler controler = null;

        ContentManager Content = null;
        DuckHuntGame game = null;
        Rectangle viewRect = new Rectangle();

        Dictionary<ModelType, ObjectTexturesItem> objTextureLst;

        CXLayer showlayer = null;

        bool pause = false;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize(Rectangle viewScene)
        {
            if (controler == null)
            {
                controler = this;
            }
            Camera.ViewHeight = viewScene.Height;
            Camera.ViewWidth = viewScene.Width;

            game = new DuckHuntGame();
            viewRect = viewScene;
            objTextureLst = new Dictionary<ModelType, ObjectTexturesItem>();

            _texturemap = new Dictionary<string, Texture2D>();
            _spritefontmap = new Dictionary<string, SpriteFont>();
            _soundmap = new Dictionary<string, SoundEffect>();



#if WINDOWS_PHONE
            game.TrialVersion = Guide.IsTrialMode;
#endif
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
            CXBaseShowNode.controler = this;

            LoadResources();

            List<GameSound> soundList = game.GetSoundList();
            gamebackgorundsound = Content.Load<Song>(soundList[0].soundpath);
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Volume = 50;
            game.StartGame(viewRect);


#if WINDOWS_PHONE
            if (game.IsExpired())
            {
                Guide.ShowMarketplace(PlayerIndex.One);
                return;
            }
#endif

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



        Dictionary<string, Texture2D> _texturemap;
        public Texture2D LoadTexture(string texturepath)
        {
            if (_texturemap.Keys.Contains<string>(texturepath))
            {
                return _texturemap[texturepath];
            }

            Texture2D texture = Content.Load<Texture2D>(texturepath);
            _texturemap[texturepath] = texture;
            return texture;
        }

        Dictionary<string, SpriteFont> _spritefontmap;
        public SpriteFont LoadSpriteFont(string fontpath)
        {
            if (_spritefontmap.Keys.Contains<string>(fontpath))
            {
                return _spritefontmap[fontpath];
            }

            SpriteFont font = Content.Load<SpriteFont>(fontpath);
            _spritefontmap[fontpath] = font;
            return font;
        }

        Dictionary<string, SoundEffect> _soundmap;
        public SoundEffect LoadSoundEffect(string soundpath)
        {
            if (_soundmap.Keys.Contains<string>(soundpath))
            {
                return _soundmap[soundpath];
            }

            SoundEffect sound = Content.Load<SoundEffect>(soundpath);
            _soundmap[soundpath] = sound;
            return sound;
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
            if (game.IsExpired())
            {
                return;
            }
            /*
            if ((gameTime.TotalGameTime - lastupdateTime).TotalMilliseconds < 25)
            {
                return;
            }
            lastupdateTime = gameTime.TotalGameTime;
            */
            game.Update(gameTime);

            CurrentLayer().visitForUpdate(gameTime);


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


        }


        public Object AddShowLayer()
        {
            CXLayer layer = new DuckHuntLayer();
            layer.Init();
            return layer;
        }

        public void AddObjectToShowLayer(Object showLayer1, ModelObject modelobj)
        {
            CXShowNode showNode = modelobj.GetShowNode();
            if (showNode == null)
            {
                showNode = ViewObjectFactory.CreateViewObject(modelobj);
                showNode.Init(null);
                modelobj.SetShowNode(showNode);

            }


            CXLayer layer = (CXLayer)showLayer1;
            layer.AddChild(showNode);
        }

        public void ClearObjectFromShowLayer(Object showLayer1)
        {
            CXLayer layer = (CXLayer)showLayer1;
            layer.RemoveAllChildren();
        }
        public void RemoveObjectFromShowLayer(Object showLayer1, ModelObject modelobj)
        {
            CXShowNode showNode = modelobj.GetShowNode();
            if (showNode == null)
            {
                return;
            }

            CXLayer layer = (CXLayer)showLayer1;
            layer.RemoveChild(showNode);
        }

        public void SwitchShowLayer(Object showLayer1)
        {
            showlayer = (CXLayer)showLayer1;
        }

        public CXLayer CurrentLayer()
        {
            return showlayer;
        }


        public void Press(List<Vector2> clickPositions)
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
            game.Press(globalpointposlst);
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

        public bool GoPrevious()
        {
            return game.ReturnToPrevious();
        }

        public void Pause(bool snapview)
        {
            game.SnapView = snapview;
            game.PauseGame(true);
            game.SwapSnap(snapview);
        }

        public void Resume()
        {
            game.SnapView = false;
            game.PauseGame(false);
            game.SwapSnap(true);
        }

    }


}
