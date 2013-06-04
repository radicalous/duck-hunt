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
        LOSTDUCKBOARD, MENUITEM, KEYBORD, KEYITEM
    };
    
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

        Vector2 GetAbsolutePosition(); // the lefttop cornor based on the parent object
        Rectangle GetSpace();   // space is the rect the object may cover, the lefttop is Zero
        float GetSacle();       // scale is the scale to scale textures 


        List<AnimationInfo> GetAnimationInfoList(); // this is the animation information, include itself and it's children's
        int GetCurrentAnimationIndex(); // current animation index
        float GetAnimationDepth();      // animation depth

        int GetSoundIndex();


        ModelObject GetParentObject();              
        List<ModelObject> GetChildrenObjects();

        // assist function, improve performance
        ViewObject GetViewObject();
        void SetViewObject(ViewObject viewObject);
    }

    class ViewItem
    {
        public Animation animation;
        public bool backGroundAnimation;
        public StaticBackground2 staticBackground;
        public Animation bganimation;

    }

    interface ViewObject
    {
        void Init(Vector2 orgpoint1, float defscale1, ModelObject model, Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }

    class CommonViewObject: ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;

        ModelObject model;
        List<ViewObject> childViewObjectList;

        Vector2 _orgpointinscreen;
        float _defscaleinscreen;

        // screen rect
        public Rectangle screenRc = new Rectangle();

        public CommonViewObject(ModelObject model1, Vector2 orgpointinscreen, float defscaleinscreen)
        {
            _orgpointinscreen = orgpointinscreen;
            _defscaleinscreen = defscaleinscreen;

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

        Dictionary<ModelType, ObjectTexturesItem> _resLst;
        public void Init(Vector2 orgpointinscreen, float defscaleinscreen, ModelObject model1, 
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle spaceInLogic)
        {
            _orgpointinscreen = orgpointinscreen;
            _defscaleinscreen = defscaleinscreen;

            _resLst = objTextureLst;
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
                        Vector2.Zero, (int)(animationInfo.frameWidth ),
                        (int)(animationInfo.frameHeight ),
                        animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                        model.GetSacle(), true);
                }
                else
                {
                    viewItm.backGroundAnimation = true;
                    viewItm.bganimation = new Animation();
                    if (animationInfo.frameHeight == 0)
                    {
                        viewItm.bganimation.Initialize(
                            texturesList[i],
                            Vector2.Zero, (int)(texturesList[i].Width/*animationInfo.frameWidth*/),
                            (int)(texturesList[i].Height/*animationInfo.frameHeight*/),
                            1/*animationInfo.frameCount*/, 1/*animationInfo.frameTime*/, animationInfo.backColor,
                            model.GetSacle(), true);


                        float scale = 1.0f;
                        if (texturesList[i].Width * 1.0f / texturesList[i].Height > screenRc.Width * 1.0 / screenRc.Height)
                        {
                            // the text wider, should extend according height
                            scale = screenRc.Height * 1.0f / texturesList[i].Height;

                            int offx = (int)((texturesList[i].Width * scale - screenRc.Width) / 2 / scale);
                            offx = (int)(offx * scale);
                            offx = -offx;
                            int centerx = (int)(offx + texturesList[i].Width * scale / 2);
                            int centery = screenRc.Height / 2;
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                        else
                        {
                            // the texture is higher, should extend according width
                            scale = screenRc.Width * 1.0f / texturesList[i].Width;

                            int offy = (int)((texturesList[i].Height * scale - screenRc.Height) / scale);
                            offy = (int)(offy * scale);
                            offy = -offy;
                            int centerx = screenRc.Width / 2;
                            int centery = (int)(offy + texturesList[i].Height * scale / 2);
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                    }
                    else
                    {
                        viewItm.bganimation.Initialize(
                            texturesList[i],
                            Vector2.Zero, animationInfo.frameWidth,
                            animationInfo.frameHeight,
                            animationInfo.frameCount, animationInfo.frameTime, animationInfo.backColor,
                            model.GetSacle(), true);


                        float scale = 1.0f;
                        if (animationInfo.frameWidth * 1.0f / animationInfo.frameHeight > screenRc.Width * 1.0 / screenRc.Height)
                        {
                            // the text wider, should extend according height
                            scale = screenRc.Height * 1.0f / animationInfo.frameHeight;

                            int offx = (int)((animationInfo.frameWidth * scale - screenRc.Width) / 2 / scale);
                            offx = (int)(offx * scale);
                            offx = -offx;
                            int centerx = (int)(offx + animationInfo.frameWidth * scale / 2);
                            int centery = screenRc.Height / 2;
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }
                        else
                        {
                            // the texture is higher, should extend according width
                            scale = screenRc.Width * 1.0f / animationInfo.frameWidth;

                            int offy = (int)((animationInfo.frameHeight * scale - screenRc.Height) / scale);
                            offy = (int)(offy * scale);
                            offy = -offy;
                            int centerx = screenRc.Width / 2;
                            int centery = (int)(offy + animationInfo.frameHeight * scale / 2);
                            viewItm.bganimation.Position.X = centerx;
                            viewItm.bganimation.Position.Y = centery;
                            viewItm.bganimation.scale = scale;

                        }

                    }


                        
                    /*
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        Vector2.Zero,
                        screenRc.Width, 
                        screenRc.Height,
                        0);
                     */
                }
                viewItmList.Add(viewItm);
            }
            //play init sound
            /*
            if (objTextureLst[model.Type()].soundList.Count > 0)
            {//SoundEffect.MasterVolume
                //SoundEffect.
                //SoundEf
                float mastvol = SoundEffect.MasterVolume;
                objTextureLst[model.Type()].soundList[0].Play(1,0, 0);
            }
             */
            

            
            // left textures are for children
            if (childViewObjectList != null)
            {
                foreach (ViewObject childviewobj in childViewObjectList)
                {
                    Rectangle rc = new Rectangle();
                    childviewobj.Init(_orgpointinscreen, _defscaleinscreen, null, objTextureLst, rc);
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
                viewItm.animation.Position =  _orgpointinscreen +
                    model.GetAbsolutePosition() * _defscaleinscreen;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2) * _defscaleinscreen;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) * _defscaleinscreen;
                viewItm.animation.scale = model.GetSacle() * _defscaleinscreen;
                viewItm.animation.Update(gameTime);
            }
            else
            {
                if (viewItm.backGroundAnimation)
                {
                    viewItm.bganimation.Update(gameTime);
                }
                else
                {
                    viewItm.staticBackground.Update(gameTime);
                }
            }

            if (childViewObjectList != null)
            {
                foreach (ViewObject viewObj in childViewObjectList)
                {
                    viewObj.Update(gameTime);
                }
            }

            // check need to play audio
            //play init sound
            if (_resLst[model.Type()].soundList.Count > 0)
            {//SoundEffect.MasterVolume
                //SoundEffect.
                //SoundEf
                int soundindex = model.GetSoundIndex();
                if (soundindex >= 0 && soundindex < _resLst[model.Type()].soundList.Count)
                {
                    float mastvol = SoundEffect.MasterVolume;
                    _resLst[model.Type()].soundList[0].Play(1, 0, 0);
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
                if (viewItm.backGroundAnimation)
                {
                    viewItm.bganimation.Draw(spriteBatch, model.GetAnimationDepth());
                }
                else
                {
                    viewItm.staticBackground.Draw(spriteBatch, model.GetAnimationDepth());
                }
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

        Vector2 _orgpoint;
        float _defscale;

        public ScoreBoardViewObject(ModelObject model1)
        {
            model = (ScroeBoardModel)model1;
        }

        public void Init(Vector2 orgpoint, float defscale, ModelObject model1, 
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (ScroeBoardModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

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
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        orgpoint,
                        (int)(space.Width*defscale), (int)(space.Height*defscale), 0);
                }
                viewItmList.Add(viewItm);
            }

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 20 * _defscale;
            scoreposition.Y += 25 * _defscale;

        }

        public void Update(GameTime gameTime)
        {
            return;

            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpoint + model.GetAbsolutePosition() * _defscale;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2) * _defscale;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) * _defscale;
                viewItm.animation.scale = model.GetSacle() * _defscale;
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
            }

        }

        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle coords, Color color)
        {
            var rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            // draw left
            Rectangle rcline = new Rectangle();
            rcline.X = coords.Left;
            rcline.Y = coords.Top;
            rcline.Width = 1;
            rcline.Height = coords.Height;
            spriteBatch.Draw(rect, rcline, color);
            rcline.X += coords.Width;
            spriteBatch.Draw(rect, rcline, color);
            rcline.X = coords.Left;
            rcline.Width = coords.Width;
            rcline.Height = 1;
            spriteBatch.Draw(rect, rcline, color);
            rcline.Y += coords.Height;
            spriteBatch.Draw(rect, rcline, color);
        }


        private void DrawRectangle2(SpriteBatch spriteBatch, Rectangle coords, Color color)
        {
            var rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            // draw left

            spriteBatch.Draw(rect, coords, color);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];

            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * _defscale);
            rc.Height = (int)(rc.Height * _defscale);
            rc.X += (int)scoreposition.X ; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;

            Color color = new Color(167,167,167);
            color.A = 10;

            color = new Color(253, 253, 253);

            Color color1 = Color.Yellow;
            color1.A = 80;
            //DrawRectangle2(spriteBatch, rc, color1);
            //DrawRectangle(spriteBatch, rc, Color.Blue);


            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10*_defscale;
            pos1.X += 10 * _defscale;
            string value = this.model.TotalScore.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(fontList[0], "SCORE: " + value, pos1, Color.Yellow, 0, Vector2.Zero, 1, 
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }


    // draw the score myself
    class KeyItemViewObject : ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        KeyItemModel model;
        List<SpriteFont> fontList;

        Vector2 scoreposition;

        Vector2 _orgpoint;
        float _defscale;

        public KeyItemViewObject(ModelObject model1)
        {
            model = (KeyItemModel)model1;
        }

        public void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
           // model = (KeyItemModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

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
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        orgpoint,
                        (int)(space.Width * defscale), (int)(space.Height * defscale), 0);
                }
                viewItmList.Add(viewItm);
            }

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 0 * _defscale;
            scoreposition.Y += 0 * _defscale;

        }

        public void Update(GameTime gameTime)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpoint + model.GetAbsolutePosition() * _defscale;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2) * _defscale;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) * _defscale;
                viewItm.animation.scale = model.GetSacle() * _defscale;
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
            }

        }

        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle coords, Color color)
        {
            var rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            // draw left
            Rectangle rcline = new Rectangle();
            rcline.X = coords.Left;
            rcline.Y = coords.Top;
            rcline.Width = 1;
            rcline.Height = coords.Height;
            spriteBatch.Draw(rect, rcline, color);
            rcline.X += coords.Width;
            spriteBatch.Draw(rect, rcline, color);
            rcline.X = coords.Left;
            rcline.Width = coords.Width;
            rcline.Height = 1;
            spriteBatch.Draw(rect, rcline, color);
            rcline.Y += coords.Height;
            spriteBatch.Draw(rect, rcline, color);
        }


        private void DrawRectangle2(SpriteBatch spriteBatch, Rectangle coords, Color color)
        {
            var rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            // draw left

            spriteBatch.Draw(rect, coords, color);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];

            viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());

            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * _defscale);
            rc.Height = (int)(rc.Height * _defscale);
            rc.X += (int)scoreposition.X; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;

            Color color = new Color(167, 167, 167);
            color.A = 10;

            color = new Color(253, 253, 253);

            Color color1 = Color.Yellow;
            color1.A = 80;
            //DrawRectangle2(spriteBatch, rc, color1);
            //DrawRectangle(spriteBatch, rc, Color.Blue);


            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * _defscale;
            pos1.X += 10 * _defscale;
            string value = this.model.Conent.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(fontList[0], value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }


    // draw the score myself
    class TimeBoardViewObject : ViewObject
    {
        TimeBoardModel model;
        List<SpriteFont> fontList;

        Vector2 scoreposition;

        Vector2 _orgpoint;
        float _defscale;

        public TimeBoardViewObject(ModelObject model1)
        {
            model = (TimeBoardModel)model1;
        }

        public void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (TimeBoardModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

            fontList = objTextureLst[model.Type()].fontList;


            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            // background

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 20 * _defscale;
            scoreposition.Y += 25 * _defscale;
        }

        public void Update(GameTime gameTime)
        {

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * _defscale);
            rc.Height = (int)(rc.Height * _defscale);
            rc.X += (int)scoreposition.X; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;

            Color color = new Color(167, 167, 167);
            color.A = 10;

            color = new Color(253, 253, 253);

            Color color1 = Color.Blue;
            color1.A = 10;

            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * _defscale;
            pos1.X += 10 * _defscale;
            string value = this.model.LeftTime.ToString();

            spriteBatch.DrawString(fontList[0], "Left Time: " + value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }



    // draw the score myself
    class LostDuckBoardViewObject : ViewObject
    {
        LostDuckBoardModel model;
        List<SpriteFont> fontList;

        Vector2 scoreposition;

        Vector2 _orgpoint;
        float _defscale;

        public LostDuckBoardViewObject(ModelObject model1)
        {
            model = (LostDuckBoardModel)model1;
        }

        public void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (LostDuckBoardModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

            fontList = objTextureLst[model.Type()].fontList;


            // create view items for this object
            List<Texture2D> texturesList = objTextureLst[model.Type()].textureList;
            // background

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 20 * _defscale;
            scoreposition.Y += 25 * _defscale;
        }

        public void Update(GameTime gameTime)
        {

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // this rc is logic rc
            Rectangle rc = model.GetSpace();
            rc.Height = 63; // same height with hitboard
            rc.Width = 200;
            rc.Width = (int)(rc.Width * _defscale);
            rc.Height = (int)(rc.Height * _defscale);
            rc.X += (int)scoreposition.X; // scoreposition is position in local view
            rc.Y += (int)scoreposition.Y;

            Color color = new Color(167, 167, 167);
            color.A = 10;

            color = new Color(253, 253, 253);

            Color color1 = Color.Blue;
            color1.A = 10;

            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * _defscale;
            pos1.X += 10 * _defscale;
            string value = this.model.LostDuckCount.ToString();

            spriteBatch.DrawString(fontList[0], "Lost Duck Count: " + value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }



    // draw the score myself
    class HitBoardViewObject : ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        HitBoardModel model;
        List<SpriteFont> fontList;

        Vector2 scoreposition;

        Vector2 _orgpoint;
        float _defscale;

        public HitBoardViewObject(ModelObject model1)
        {
            model = (HitBoardModel)model1;
        }

        public void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (HitBoardModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

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
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        orgpoint,
                        (int)(space.Width * defscale), (int)(space.Height * defscale), 0);
                }
                viewItmList.Add(viewItm);
            }

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 20 * _defscale;
            scoreposition.Y += 25 * _defscale;
        }

        public void Update(GameTime gameTime)
        {
            /*
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpoint + model.GetAbsolutePosition() * _defscale;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2) * _defscale;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) * _defscale;
                viewItm.animation.scale = model.GetSacle() * _defscale;
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
            }

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 20 * _defscale;
            scoreposition.Y += 25 * _defscale;
             */
        }

        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle coords, Color color)
        {
            var rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            // draw left
            Rectangle rcline = new Rectangle();
            rcline.X = coords.Left;
            rcline.Y = coords.Top;
            rcline.Width = 1;
            rcline.Height = coords.Height;
            spriteBatch.Draw(rect, rcline, color);
            rcline.X += coords.Width;
            spriteBatch.Draw(rect, rcline, color);
            rcline.X = coords.Left;
            rcline.Width = coords.Width;
            rcline.Height = 1;
            spriteBatch.Draw(rect, rcline, color);
            rcline.Y += coords.Height;
            spriteBatch.Draw(rect, rcline, color);
        }


        private void DrawRectangle2(SpriteBatch spriteBatch, Rectangle coords, Color color)
        {
            var rect = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            rect.SetData(new[] { color });

            // draw left

            spriteBatch.Draw(rect, coords, color);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rc = model.GetSpace();
            rc.Width = (int) (rc.Width*_defscale);
            rc.Height = (int)(rc.Height*_defscale);
            rc.X += (int)scoreposition.X;
            rc.Y += (int)scoreposition.Y;

            Color color = Color.Blue;
            color.A = 10;
            //DrawRectangle2(spriteBatch, rc, color);
            //DrawRectangle(spriteBatch, rc, Color.Blue);

            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * _defscale;
            pos1.X += 10 * _defscale;
            string value = "Hit Count: " + model.GetHitCount().ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(fontList[0], value, pos1, Color.Yellow, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }


    // draw the score myself
    class MenuItemViewObject : ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        MenuItemModel model;
        List<SpriteFont> fontList;

        Vector2 menuContentPos;

        Vector2 _orgpoint;
        float _defscale;

        public MenuItemViewObject(ModelObject model1)
        {
            model = (MenuItemModel)model1;
        }

        public void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (MenuItemModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

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
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        orgpoint,
                        (int)(space.Width * defscale), (int)(space.Height * defscale), 0);
                }
                viewItmList.Add(viewItm);
            }

            menuContentPos = model.GetAbsolutePosition() * _defscale + _orgpoint;

            menuContentPos.X += (120 - model.Conent.Length * 10) * _defscale;
            menuContentPos.Y += 60 * _defscale;
        }

        public void Update(GameTime gameTime)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpoint + model.GetAbsolutePosition() * _defscale;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2) * _defscale;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) * _defscale;
                viewItm.animation.scale = model.GetSacle() * _defscale;
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

            // draw score
            Vector2 pos1 = menuContentPos;
            Rectangle space = model.GetSpace();
            //.Y += (space.Height/2 - 60) * _defscale;
            //pos1.X += 0 * _defscale;
            string value = this.model.Conent.ToString();
            spriteBatch.DrawString(fontList[0], value, pos1, Color.Blue, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
        }
    }


    // draw the score myself
    class ScoreListBoardViewObject : ViewObject
    {
        List<AnimationInfo> animationList;
        List<ViewItem> viewItmList;
        ScroeListBoardModel model;
        List<SpriteFont> fontList;

        Vector2 scoreposition;

        Vector2 _orgpoint;
        float _defscale;

        public ScoreListBoardViewObject(ModelObject model1)
        {
            model = (ScroeListBoardModel)model1;
        }

        public void Init(Vector2 orgpoint, float defscale, ModelObject model1,
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle space)
        {
            model = (ScroeListBoardModel)model1;

            _orgpoint = orgpoint;
            _defscale = defscale;

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
                    viewItm.staticBackground = new StaticBackground2();
                    viewItm.staticBackground.Initialize(
                        texturesList[i],
                        orgpoint,
                        (int)(space.Width * defscale), (int)(space.Height * defscale), 0);
                }
                viewItmList.Add(viewItm);
            }

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 0 * _defscale;
            scoreposition.Y += 0 * _defscale;
        }

        public void Update(GameTime gameTime)
        {
            /*
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Position = _orgpoint + model.GetAbsolutePosition() * _defscale;

                viewItm.animation.Position.X += (viewItm.animation.FrameWidth / 2) * _defscale;
                viewItm.animation.Position.Y += (viewItm.animation.FrameHeight / 2) * _defscale;
                viewItm.animation.scale = model.GetSacle() * _defscale;
                viewItm.animation.Update(gameTime);
            }
            else
            {
                viewItm.staticBackground.Update(gameTime);
            }
            */
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            //ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            //viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * _defscale;
            pos1.X += 10 * _defscale;
            //string value = this.model.TotalScore.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            for (int i = 0; i < model.ScoreList.Count; i++)
            {
                spriteBatch.DrawString(fontList[0], model.ScoreList[i].Key + ":     " + model.ScoreList[i].Value.ToString(),
                    pos1, Color.Yellow, 0, Vector2.Zero, 1,
                    SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
                pos1.Y += 30 * _defscale;
            }
        }
    }




    class SkyModel : ModelObject
    {
        ModelObject parent = null;
        ViewObject viewObject;

        Vector2 relativePos;
        Rectangle space;

        // Animation representing the player
        List<AnimationInfo> anationInfoList;

        public SkyModel()
        {
            anationInfoList = new List<AnimationInfo>();
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = animationInfo.frameHeight = 0;
            anationInfoList.Add(animationInfo);
        }


        // interfaces implementation
        public ModelType Type()
        {
            // sky 
            return ModelType.SKY;
        }

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

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky_2";
            resourceList.Add(resourceItm);
            return resourceList;
        }


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
            return 1.0f;
        }

        public int GetSoundIndex()
        {
            return -1;
        }


        public ModelObject GetParentObject()
        {
            return null;
        }


        public List<ModelObject> GetChildrenObjects()
        {
            return null;
        }

        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }
    }



    class GrassMountainModel : ModelObject
    {
        ModelObject parent = null;
        Vector2 relativePos;
        Rectangle space;
        
        // Animation representing the player
        List<AnimationInfo> anationInfoList;


        public GrassMountainModel()
        {
            anationInfoList = new List<AnimationInfo>();
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = animationInfo.frameHeight = 0;
            anationInfoList.Add(animationInfo);
        }

        // interfaces implementation

        public ModelType Type()
        {
            // sky 

            return ModelType.SKY;
        }

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

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\sky_2";
            resourceList.Add(resourceItm);
            return resourceList;
        }


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
            return 1.0f;
        }

        public int GetSoundIndex()
        {
            return -1;
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




    class CloudModel : ModelObject
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


        public ModelType Type()
        {
            return ModelType.CLOUD;
        }

        public void Initialize(ModelObject parent, Rectangle rect, int seed)
        {
            parent = null;
            space = rect;
            relativePosInParent.X = rect.Left;
            relativePosInParent.Y = rect.Top;
            space.Offset(-space.Left, -space.Y);
            pilot.Initialize(space, 0);
        }

        public void Update(GameTime gameTime)
        {
            // no animation
            pilot.Update(gameTime);
        }

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\Cloud";
            resourceList.Add(resourceItm);
            return resourceList;
        }

        public Vector2 GetAbsolutePosition()
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
        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 0.5f;
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
            return 0.9f;
        }

        public int GetSoundIndex()
        {
            return -1;
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
        Vector2 relativePos;
        Rectangle space;

        // Animation representing the player
        List<AnimationInfo> anationInfoList;


        public GrassModel()
        {
            anationInfoList = new List<AnimationInfo>();
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 1600; //1600; 
            animationInfo.frameHeight = 900; // 900;
            animationInfo.frameTime = 500;
            anationInfoList.Add(animationInfo);
        }

        public ModelType Type()
        {
            return ModelType.GRASS;
        }


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


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\bg_grass";
            resourceList.Add(resourceItm);
            return resourceList;
        }

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
            return 0.5F;
        }

        public int GetSoundIndex()
        {
            return -1;
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



    class ForegroundGrassModel : ModelObject
    {
        ModelObject parent = null;
        Vector2 relativePos;
        Rectangle space;


        // Animation representing the player
        List<AnimationInfo> anationInfoList;

        public ForegroundGrassModel()
        {
            anationInfoList = new List<AnimationInfo>();
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.animation = false;
            animationInfo.frameCount = 1;
            animationInfo.frameWidth = 1600; //1600; 
            animationInfo.frameHeight = 900; // 900;
            animationInfo.frameTime = 500;
            anationInfoList.Add(animationInfo);
        }

        public ModelType Type()
        {
            return ModelType.FORGROUND;
        }

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


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\bg_tree";
            resourceList.Add(resourceItm);
            return resourceList;
        }

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
            return 0.2F;
        }

        public int GetSoundIndex()
        {
            return -1;
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

        public int frameWidth = 115;
        public int frameHeight = 69;
        public int frameCount = 8;
        public int frameTime = 30;
        public Color backColor = Color.White;
    }



    class MenuItemModel : ModelObject
    {
        ModelObject parent = null;

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
        public ModelType Type()
        {
            return ModelType.MENUITEM;
        }

        public void Initialize(ModelObject parent1, Rectangle itemSpace, int seed)
        {
            parent = null;
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

        public List<ResourceItem> GetResourceList()
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
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\font";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }


        public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePostionInParent;
            if (parent != null)
            {
                absPos += parent.GetAbsolutePosition();
            }
            // pilot return center postion, adjust it to left top conner
            //absPos.X -= _itemspace.Width / 2;
            //absPos.Y -= _itemspace.Height / 2;

            return absPos;
        }
        public Rectangle GetSpace()
        {
            return _itemspace;
        }
        public float GetSacle()
        {
            if (onHover)
            {
                return 0.5f;
            }
            else
            {
                return 1.0f;
            }
        }


        public void Update(GameTime gameTime)
        {


        }


        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        public int GetCurrentAnimationIndex()
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

        public float GetAnimationDepth()
        {
            return depth;
        }

        public int GetSoundIndex()
        {
            return -1;
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



    class KeyItemModel : ModelObject
    {
        ModelObject parent = null;

        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        Rectangle _itemspace = new Rectangle(0, 0, 240, 137);

        float scale = 1.0f;
        float depth = 0.1f;

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
        public ModelType Type()
        {
            return ModelType.KEYITEM;
        }

        public void Initialize(ModelObject parent1, Rectangle itemSpace, int seed)
        {
            parent = parent1;
            relativePostionInParent.X = itemSpace.Left;
            relativePostionInParent.Y = itemSpace.Top;

            _itemspace = itemSpace;
            _itemspace.X -= (int)relativePostionInParent.X;
            itemSpace.Y -= (int)relativePostionInParent.Y;
            //_itemspace.Offset((int)-postion.X, (int)-postion.Y);
        }

        public List<ResourceItem> GetResourceList()
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
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\font";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }


        public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePostionInParent;
            if (parent != null)
            {
                absPos += parent.GetAbsolutePosition();
            }

            return absPos;
        }
        public Rectangle GetSpace()
        {
            return _itemspace;
        }
        public float GetSacle()
        {
                return 1.0f;
        }


        public void Update(GameTime gameTime)
        {


        }


        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        public int GetCurrentAnimationIndex()
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

        public float GetAnimationDepth()
        {
            return depth;
        }

        public int GetSoundIndex()
        {
            return -1;
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

        public bool Hit(Vector2 absposition)
        {
            return false;
        }
    }



    class KeyboardModel : ModelObject
    {
        ModelObject parent = null;

        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        Rectangle _itemspace = new Rectangle(0, 0, 797, 268);

        float scale = 1.0f;
        float depth = 0.2f;

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
        public ModelType Type()
        {
            return ModelType.KEYBORD;
        }

        public void Initialize(ModelObject parent1, Rectangle itemSpace, int seed)
        {
            parent = null;
            relativePostionInParent.X = itemSpace.Left;
            relativePostionInParent.Y = itemSpace.Top;

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
            keyspace.Y = 11;
            keyspace.X = 15;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "GAMIL";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 84;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "OUTLOOK";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 154;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Yahoo";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 225;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Live";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 294;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "MAIL";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 364;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "@";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 433;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = ".com";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 504;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = ".co.uk";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 574;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = ".eu";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 644;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "-";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 714;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "_";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.Y = 62;
            keyspace.X = 15;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "1";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 84;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "2";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 154;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "3";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 225;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "4";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 294;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "5";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 364;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "6";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 433;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "7";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 504;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "8";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 574;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "9";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 644;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "0";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 714;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "X";
            keyList.Add(keyItem);

            // y = 113, x(51),(121), (191), (262), (331), (401), (470), (541), (611), (681)
            keyItem = new KeyItemModel();
            keyspace.Y = 113;
            keyspace.X = 51;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Q";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 121;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "W";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 191;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "E";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 262;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "R";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 331;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "T";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 401;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Y";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 470;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "U";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 541;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "I";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 611;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "O";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 682;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "P";
            keyList.Add(keyItem);

            // y = 166, x(86), (155), (226), (296), (367), (436), (506), (577), (647)
            keyItem = new KeyItemModel();
            keyspace.Y = 166;
            keyspace.X = 86;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "A";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 155;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "S";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 226;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "D";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 296;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "F";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 367;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "G";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 436;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "H";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 506;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "J";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 577;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "K";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 647;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "L";
            keyList.Add(keyItem);

            // y = 220, x(53), (124), (194), (265), (334), (404), (475), (546), (621)
            keyItem = new KeyItemModel();
            keyspace.Y = 220;
            keyspace.X = 53;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "Z";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 124;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "X";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 194;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "C";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 265;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "V";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 334;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "B";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 404;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "N";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 475;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "M";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 546;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = ".";
            keyList.Add(keyItem);

            keyItem = new KeyItemModel();
            keyspace.X = 621;
            keyItem.Initialize(this, keyspace, 0);
            keyItem.Conent = "SPACE";
            keyList.Add(keyItem);
        }

        public List<ResourceItem> GetResourceList()
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
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\font";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }


        public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = relativePostionInParent;
            if (parent != null)
            {
                absPos += parent.GetAbsolutePosition();
            }

            return absPos;
        }
        public Rectangle GetSpace()
        {
            return _itemspace;
        }
        public float GetSacle()
        {
            return 1.0f;
        }


        public void Update(GameTime gameTime)
        {


        }


        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        public int GetCurrentAnimationIndex()
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

        public float GetAnimationDepth()
        {
            return depth;
        }

        public int GetSoundIndex()
        {
            return -1;
        }

        public ModelObject GetParentObject()
        {
            return null;
        }
        public List<ModelObject> GetChildrenObjects()
        {
            List<ModelObject> objlst = new List<ModelObject>();
            foreach (KeyItemModel keyitm in this.keyList)
            {
                objlst.Add(keyitm);
            }
            return objlst;
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

        public bool Hit(Vector2 absposition)
        {
            return false;
        }
    }

    class DuckModel : ModelObject
    {
        AiPilot flyduckPilot;
        AiPilot goneduckPilot;

        public bool Active = true;
        public bool dead = false;

        public bool Gone = false;

        // Amount of hit points that player has
        public int Health;

        int deadstopcount = 0;

        List<Vector2> boundingTrigle1;
        List<Vector2> boundingTrigle2;

        Rectangle duckspace;

        int randomseed = 0;
        ModelObject parent = null;
        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        float scale = 1.0f;
        float depth = 0.6f;


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

            // 0. flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 100;
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
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);

            boundingTrigle1 = new List<Vector2>();
            boundingTrigle2 = new List<Vector2>();
        }


        public DuckModel(PilotType type)
        {
            //
            flyduckPilot = PilotManager.GetInstance().CreatePilot(type);
            anationInfoList = new List<AnimationInfo>();

            // 0. flying duck
            AnimationInfo animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 100;
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
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // 3. reverse fly duck
            animationInfo = new AnimationInfo();
            animationInfo.frameWidth = 105;
            animationInfo.frameHeight = 102;
            animationInfo.frameCount = 3;
            animationInfo.frameTime = 100;
            anationInfoList.Add(animationInfo);

            boundingTrigle1 = new List<Vector2>();
            boundingTrigle2 = new List<Vector2>();
        }


        public ModelType Type()
        {
            return ModelType.DUCK;
        }


        public void Initialize(ModelObject parent1, Rectangle duckSpace, int seed)
        {
            parent = null;

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


        //Rectangle space;
        public Vector2 GetAbsolutePosition()
        {
            Vector2 absPos = RelativePosition;
            if (parent != null)
            {
                absPos += parent.GetAbsolutePosition();
            }
            // pilot return center postion, adjust it to left top conner
            absPos.X -= 105 / 2;
            absPos.Y -= 102 / 2;

            return absPos;
        }
        public Rectangle GetSpace()
        {
            return duckspace;
        }
        public float GetSacle()
        {
            if (Active)
            {
                // get depth, calculate the scale

                //scale = autoPilot.scale;
                scale = 1 - flyduckPilot.GetDepth() * 1.0f / 100;
            }

            return scale;
        }


        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                flyduckPilot.Update(gameTime);

                // check if it need to go
                elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime > 1000 * 10)
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
                    Gone = true;
                }
            }

        }



        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }
        public int GetCurrentAnimationIndex()
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
                if (flyduckPilot.GetHorizationDirection() == Direction.LEFT)
                {
                    if (flyduckPilot.GetZDirection() ==  Direction.IN)
                    {
                        return 0;
                    }
                    else if (flyduckPilot.GetZDirection() == Direction.OUT)
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
                    if ( flyduckPilot.GetZDirection() == Direction.IN)
                    {
                        return 3;
                    }
                    else if (flyduckPilot.GetZDirection() == Direction.OUT)
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

        public float GetAnimationDepth()
        {
            return depth;
        }

        public int GetSoundIndex()
        {
            return -1;
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



        /// <summary>
        ///  specific functions
        /// </summary>

        public void StartPilot()
        {
            // Set the starting position of the player around the middle of the screen and to the back

            flyduckPilot.Initialize(duckspace, randomseed);
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

            Vector2 duckCenter = GetAbsolutePosition();
            Vector2 bullet2DuckPos = bulletCenter - duckCenter;
            /*
            List<Vector2> boudingTrigle = new List<Vector2>();
            foreach (Vector2 triglepos in boundingTrigle1)
            {
                boudingTrigle.Add(new Vector2(triglepos.X * scale, triglepos.Y * scale));
            }
            if (CollectionDetect.PointInTriangle(bullet2DuckPos, boudingTrigle))
            {
                // shot
                Active = false;
                dead = true;

                goneduckPilot = PilotManager.GetInstance().CreatePilot(PilotType.DUCKDEAD, flyduckPilot.GetPosition());

                // new a bullet  
                bullet.SetTarget(this);

                return;
            }
            boudingTrigle.Clear();
            foreach (Vector2 triglepos in boundingTrigle2)
            {
                boudingTrigle.Add(new Vector2(triglepos.X * scale, triglepos.Y * scale));
            }
            if (CollectionDetect.PointInTriangle(bullet2DuckPos, boudingTrigle))
            {
                // shot
                Active = false;
                dead = true;
                goneduckPilot = PilotManager.GetInstance().CreatePilot(PilotType.DUCKDEAD, flyduckPilot.GetPosition());

                // new a bullet  
                bullet.SetTarget(this);
                return;
            }
            */


            //
            duckCenter.X += anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            duckCenter.Y += anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;

            Vector2 subpos = bulletCenter - duckCenter;
            if (subpos.Length() < 40 * scale)
            {
                Active = false;
                dead = true;
                goneduckPilot = PilotManager.GetInstance().CreatePilot(PilotType.DUCKDEAD, flyduckPilot.GetPosition());

                // new a bullet  
                bullet.SetTarget(this);
            }

        }

    }



    class DogModel : ModelObject
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



        public ModelType Type()
        {
            return ModelType.DOG;
        }


        public void Initialize(ModelObject parent1, Rectangle dogSpace, int seed)
        {
            dogspace = dogSpace;
        }

        public List<ResourceItem> GetResourceList()
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
            
            /*
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
            */
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.SOUND;
            resourceItm.path = "Sound\\dog_found";
            //resourceItm.path = "Sound\\laserFire";
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = RelativePosition;
            // adjust from lefttop conner to center
            abspos.X -= anationInfoList[GetCurrentAnimationIndex()].frameWidth / 2;
            abspos.Y -= anationInfoList[GetCurrentAnimationIndex()].frameHeight / 2;
            return abspos;
        }

        public Rectangle GetSpace()
        {
            Rectangle space = new Rectangle(0, 0
            , anationInfoList[GetCurrentAnimationIndex()].frameWidth
            , anationInfoList[GetCurrentAnimationIndex()].frameHeight);

            return space;
        }
        public float GetSacle()
        {
            return 1.0f;
        }


        public void Update(GameTime gameTime)
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


        public List<AnimationInfo> GetAnimationInfoList()
        {
            return anationInfoList;
        }

        public int GetCurrentAnimationIndex()
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

        public float GetAnimationDepth()
        {
            return depth;
        }

        public int GetSoundIndex()
        {
            if (dog_sound_index >= 0)
            {
                int returnindex = dog_sound_index;
                dog_sound_index = -2;
                return returnindex;
            }
            return -1;
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
            //dogspace = dogrunspace;
            seekPilot.Initialize(dogspace, 0);
        }

    }



    class BulletModel : ModelObject
    {
        ModelObject parent = null;
        Vector2 relativePositionInParent = Vector2.Zero;
        Vector2 targetposition = Vector2.Zero;
        List<DuckModel> shootduckList;

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

            shootduckList = new List<DuckModel>();
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

            shootduckList = new List<DuckModel>();
        }

        float deltax = 48f; //40f;
        float deltay = -18f; //-20f;



        public ModelType Type()
        {
            // sky 

            return ModelType.BULLET;
        }


        public void Initialize(ModelObject parent1, Rectangle space, int seed)
        {
            parent = null;

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


        public void Update(GameTime gameTime)
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
            resourceItm.path = "Sound\\shoot";
            //resourceItm.path = "Sound\\laserFire";
            resourceList.Add(resourceItm);

            return resourceList;
        }


        public Vector2 GetAbsolutePosition()
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
        public Rectangle GetSpace()
        {
            return bulletspace;
        }
        public float GetSacle()
        {
            return scale;
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
            return depth;
        }

        public int GetSoundIndex()
        {
            if (sound_index >= 0)
            {
                int returnindex = sound_index;
                sound_index = -1;
                return returnindex;
            }
            return -1;
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


        // specific functions
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
                //targetposition = relativePositionInParent;
                //targetposition.X += 10;
                //targetposition.Y -= 5;
                //relativePositionInParent.X = relativePositionInParent.X - 20 * 6;
                //relativePositionInParent.Y = relativePositionInParent.Y + 10 * 6;
            }
        }
        public void AdjustForFlyEffect()
        {
            if (shootduckList.Count > 0)
            {
                targetposition = relativePositionInParent;
                relativePositionInParent.X = relativePositionInParent.X - 20 * 6;
                relativePositionInParent.Y = relativePositionInParent.Y + 10 * 6;

            }
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


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            /*
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
             */


            return resourceList;
        }

        public ModelType Type()
        {
            return ModelType.DUCKICON;
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


        public Vector2 GetAbsolutePosition()
        {
            Vector2 absposition = relativePos;
            // adjust from lefttop conner to center
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


        public void Update(GameTime gameTime)
        {

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

        public int GetSoundIndex()
        {
            return -1;
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
        int hitcount = 0;

        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

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


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            /*
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\HitBoardBackground";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
             */
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\font";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }


        public ModelType Type()
        {
            return ModelType.HITBOARD;
        }


        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
        }


        public void Update(GameTime gameTime)
        {
            // no update for itself


            // update child object
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
            return 0.35f;
        }

        public int GetSoundIndex()
        {
            return -1;
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


        public int GetHitCount()
        {
            return hitcount;
        }
        public void AddHitCount(int hitCount)
        {
            hitcount += hitCount;
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
            animationInfo.frameWidth = 10;
            animationInfo.frameHeight = 19;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            space.Width = 10;
            space.Height = 19;
        }


        public ModelType Type()
        {
            return ModelType.BULLETICON;
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
            /*
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\bulletIcon";
            resourceList.Add(resourceItm);
            */
            return resourceList;
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


        public void Update(GameTime gameTime)
        {

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

        public int GetSoundIndex()
        {
            return -1;
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


        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            /*
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\BulletBoard";
            resourceList.Add(resourceItm);
             */

            return resourceList;
        }


        public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
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
            return 0;
        }

        public float GetAnimationDepth()
        {

            return 0.35f;
        }

        public int GetSoundIndex()
        {
            return -1;
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


        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
        }




        /// <summary>
        /// 
        /// </summary>
        public void RemoveFirstBullet()
        {
            if (bulletIcons.Count > 0)
            {
                bulletIcons.RemoveAt(0);
                this.viewObject = null;
            }
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



    }



    class ScroeBoardModel : ModelObject
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


        public ModelType Type()
        {
            return ModelType.SCOREBOARD;
        }

        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            /*
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\ScoreBoard";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
             */
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\font";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
        }


        public void Update(GameTime gameTime)
        {
            // no update for itself


            // update child object
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
            return 0.35f;
        }

        public int GetSoundIndex()
        {
            return -1;
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

        ViewObject viewObject;
        public ViewObject GetViewObject()
        {
            return viewObject;
        }
        public void SetViewObject(ViewObject viewObject1)
        {
            viewObject = viewObject1;
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
    }



    class ScroeListBoardModel : ModelObject
    {
        // include the background, duck icon/deadduck icon
        List<AnimationInfo> anationInfoList;
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        List<KeyValuePair<string, int>> scorelist;

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

            scorelist = new List<KeyValuePair<string, int>>();

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

            scorelist = new List<KeyValuePair<string, int>>();
        }


        public ModelType Type()
        {
            return ModelType.SCORELISTBOARD;
        }

        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            /*
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\scorelistboard";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
             */
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\font";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
        }


        public void Update(GameTime gameTime)
        {
            // no update for itself


            // update child object
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return null;
        }
        public int GetCurrentAnimationIndex()
        {
            return 0;
        }

        public float GetAnimationDepth()
        {
            return 0.35f;
        }

        public int GetSoundIndex()
        {
            return -1;
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


        public void AddScore(string name, int score)
        {
            scorelist.Add(new KeyValuePair<string, int>(name, score));
        }

        public List<KeyValuePair<string, int>> ScoreList
        {
            get
            {
                return scorelist;
            }
        }
    }


    class TimeBoardModel : ModelObject
    {
        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

        public TimeBoardModel()
        {

            space.Width = 300;
            space.Height = 63;

        }

        public TimeBoardModel(Vector2 position1)
        {
            // get least of duck icon

            space.Width = 300;
            space.Height = 63;
        }


        public ModelType Type()
        {
            return ModelType.TIMEBOARD;
        }

        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\font";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
        }


        public void Update(GameTime gameTime)
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

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return null;
        }
        public int GetCurrentAnimationIndex()
        {
            return -1;
        }

        public float GetAnimationDepth()
        {
            return 0.35f;
        }

        public int GetSoundIndex()
        {
            return -1;
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



    class LostDuckBoardModel : ModelObject
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


        public ModelType Type()
        {
            return ModelType.LOSTDUCKBOARD;
        }

        public void Initialize(ModelObject parent1, Rectangle rangespace, int seed)
        {
            space = rangespace;
            relativePosition.X = space.Left;
            relativePosition.Y = space.Top;
            space.Offset(-space.Left, -space.Top);
        }


        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\font";
#endif
            resourceList.Add(resourceItm);

            return resourceList;
        }

        public Vector2 GetAbsolutePosition()
        {
            Vector2 abspos = relativePosition;
            if (GetParentObject() != null)
            {
                abspos += GetParentObject().GetAbsolutePosition();
            }
            return abspos;
        }

        public Rectangle GetSpace()
        {
            return space;
        }
        public float GetSacle()
        {
            return 1;
        }


        double lasttime = 0;
        public void Update(GameTime gameTime)
        {
            // no update for itself
            // update child object
        }

        public List<AnimationInfo> GetAnimationInfoList()
        {
            return null;
        }
        public int GetCurrentAnimationIndex()
        {
            return -1;
        }

        public float GetAnimationDepth()
        {
            return 0.35f;
        }

        public int GetSoundIndex()
        {
            return -1;
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
