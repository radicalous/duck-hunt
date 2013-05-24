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
        DUCKICON, BULLETBOARD, BULLETICON, SCOREBOARD,SCORELISTBOARD, MENUITEM};
    
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
                    ViewObject viewobj = ViewObjectFactory.CreateViewObject(screenRc, obj, _orgpointinscreen, _defscaleinscreen);
                    childViewObjectList.Add(viewobj);
                }
            }
        }

        public void Init(Vector2 orgpointinscreen, float defscaleinscreen, ModelObject model1, 
            Dictionary<ModelType, ObjectTexturesItem> objTextureLst, Rectangle spaceInLogic)
        {
            _orgpointinscreen = orgpointinscreen;
            _defscaleinscreen = defscaleinscreen;

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

            scoreposition = model.GetAbsolutePosition()*_defscale + _orgpoint;
            scoreposition.X += 20*_defscale;
            scoreposition.Y += 25*_defscale;
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
            //color.A = 10;
            /*
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(251, 251, 251);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(248, 248, 248);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(243, 243, 243);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(239, 239, 239);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(232, 232, 232);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(226, 226, 226);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);

            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(218, 218, 218);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);

            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(209, 209, 209);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(192, 192, 192);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(192, 192, 192);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(169, 169, 169);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(140, 140, 140);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);
            */
            /*
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(140, 140, 140);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, Color.Black);
            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            color = new Color(237, 237, 237);
            //color.A = 10;
            DrawRectangle(spriteBatch, rc, color);

            rc.X += 1;
            rc.Width -= 2;
            rc.Y += 1;
            rc.Height -= 2;
            */
            /*
            color = new Color(167, 167, 167);
            color = Color.Blue;
            color.A = 10;

            DrawRectangle2(spriteBatch, rc, color);
             */
            Color color1 = Color.Blue;
            color1.A = 10;
            DrawRectangle2(spriteBatch, rc, color1);
            DrawRectangle(spriteBatch, rc, Color.Blue);


            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10*_defscale;
            pos1.X += 10 * _defscale;
            string value = this.model.TotalScore.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(fontList[0], "SCORE: " + value, pos1, Color.LightGray, 0, Vector2.Zero, 1, 
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

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 20 * _defscale;
            scoreposition.Y += 25 * _defscale;
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
            /*
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];
            if (animationList[model.GetCurrentAnimationIndex()].animation)
            {
                viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            }
            else
            {
                viewItm.staticBackground.Draw(spriteBatch, model.GetAnimationDepth());
            }
             */
            Rectangle rc = model.GetSpace();
            rc.Width = (int) (rc.Width*_defscale);
            rc.Height = (int)(rc.Height*_defscale);
            rc.X += (int)scoreposition.X;
            rc.Y += (int)scoreposition.Y;

            Color color = Color.Blue;
            color.A = 10;
            DrawRectangle2(spriteBatch, rc, color);
            DrawRectangle(spriteBatch, rc, Color.Blue);

            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * _defscale;
            pos1.X += 10 * _defscale;
            string value = "Hit Count: 153";
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(fontList[0], value, pos1, Color.LightGray, 0, Vector2.Zero, 1,
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

            menuContentPos = model.GetAbsolutePosition() * _defscale + _orgpoint;
            menuContentPos.X += 20 * _defscale;
            menuContentPos.Y += 25 * _defscale;
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
            pos1.Y += (space.Height/2 - 40) * _defscale;
            pos1.X += 150 * _defscale;
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

            scoreposition = model.GetAbsolutePosition() * _defscale + _orgpoint;
            scoreposition.X += 210 * _defscale;
            scoreposition.Y += 210 * _defscale;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ViewItem viewItm = viewItmList[model.GetCurrentAnimationIndex()];

            viewItm.animation.Draw(spriteBatch, model.GetAnimationDepth());
            // draw score
            Vector2 pos1 = scoreposition;
            pos1.Y += 10 * _defscale;
            pos1.X += 10 * _defscale;
            //string value = this.model.TotalScore.ToString();
            //spriteBatch.DrawString(fontList[0], value, pos1, Color.White, 0, Vector2.Zero, 1,
            //    SpriteEffects.None,  model.GetAnimationDepth() - 0.02f);
            spriteBatch.DrawString(fontList[0], "Penner: " + "1000", pos1, Color.LightGray, 0, Vector2.Zero, 1,
                SpriteEffects.None, model.GetAnimationDepth() - 0.02f);
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


    class MenuItemModel : ModelObject
    {
        ModelObject parent = null;

        // Animation representing the player
        List<AnimationInfo> anationInfoList;
        int elapsedTime = 0;

        List<Vector2> boundingTrigle1;
        List<Vector2> boundingTrigle2;

        Rectangle _itemspace = new Rectangle(0, 0, 482, 114);

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
            animationInfo.texturesPath = "Graphics\\MenuItemBg_selected";
            animationInfo.frameWidth = 482;
            animationInfo.frameHeight = 114;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            //1. hover duck
            animationInfo = new AnimationInfo();
            animationInfo.texturesPath = "Graphics\\MenuItemBg_unselected";
            animationInfo.frameWidth = 482;
            animationInfo.frameHeight = 114;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 3000;
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
            pos1.X = GetAbsolutePosition().X + 12;
            pos1.Y = GetAbsolutePosition().Y + itemSpace.Height - 30;
            boundingTrigle1.Add(pos1);
            pos1.X = GetAbsolutePosition().X + 160;
            pos1.Y = GetAbsolutePosition().Y + 12;
            boundingTrigle1.Add(pos1);
            pos1.X = GetAbsolutePosition().X + itemSpace.Width - 80;
            pos1.Y = GetAbsolutePosition().Y + itemSpace.Height - 30;
            boundingTrigle1.Add(pos1);

            pos1.X = GetAbsolutePosition().X + 160;
            pos1.Y = GetAbsolutePosition().Y + 12;
            boundingTrigle2.Add(pos1);
            pos1.X = GetAbsolutePosition().X + itemSpace.Width - 80;
            pos1.Y = GetAbsolutePosition().Y + itemSpace.Height - 30;
            boundingTrigle2.Add(pos1);
            pos1.X = GetAbsolutePosition().X + itemSpace.Width - 10;
            pos1.Y = GetAbsolutePosition().Y + 55;
            boundingTrigle2.Add(pos1);
        }

        public List<ResourceItem> GetResourceList()
        {
            //
            List<ResourceItem> resourceList = new List<ResourceItem>();
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\MenuItemBg_selected";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\MenuItemBg_unselected";
            resourceList.Add(resourceItm);


            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\gameFont";
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
            absPos.X -= 105 / 2;
            absPos.Y -= 102 / 2;

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
            /*
            Vector2 menumItemCenter = GetAbsolutePosition();
            //
            menumItemCenter.X += anationInfoList[AnimationIndex].frameWidth / 2;
            menumItemCenter.Y += anationInfoList[AnimationIndex].frameHeight / 2;

            Vector2 subpos = menumItemCenter - absposition;
            if (subpos.Length() < 30 * scale)
            {
                return true;
            }

            return false;
             */
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

            relativePositionInParent = position1;
            relativePositionInParent.X -= animationInfo.frameWidth / 2;
            relativePositionInParent.Y -= animationInfo.frameHeight / 2;
            bulletspace.X = 0;
            bulletspace.Y = 0;
            bulletspace.Width = animationInfo.frameWidth;
            bulletspace.Height = animationInfo.frameHeight;

            shootduckList = new List<DuckModel>();
        }

        float deltax = 40f; //40f;
        float deltay = -20f; //-20f;



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
            resourceItm.path = "Sound\\laserFire";
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

        Rectangle space; //indicate the object view range
        Vector2 relativePosition = Vector2.Zero; // no use

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
            animationInfo.texturesPath = "Graphics\\HitBoardBackground";
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
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\HitBoardBackground";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\gameFont";
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
            ResourceItem resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\BulletBoard";
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
            animationInfo.texturesPath = "Graphics\\ScoreBoard";
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
            animationInfo.texturesPath = "Graphics\\ScoreBoard";
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
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\ScoreBoard";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\gameFont";
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
            animationInfo.frameWidth = 652;
            animationInfo.frameHeight = 644;
            animationInfo.frameCount = 1;
            animationInfo.frameTime = 300;
            anationInfoList.Add(animationInfo);

            // get least of duck icon

            space.Width = 652;
            space.Height = 644;

            scorelist = new List<KeyValuePair<string, int>>();
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

            space.Width = 652;
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
            resourceItm.type = ResourceType.TEXTURE;
            resourceItm.path = "Graphics\\scorelistboard";
            resourceList.Add(resourceItm);

            resourceItm = new ResourceItem();
            resourceItm.type = ResourceType.FONT;
#if  WINDOWS_PHONE
            resourceItm.path = "Graphics\\gameFont_10";
#else
            resourceItm.path = "Graphics\\gameFont";
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
        }

        public List<KeyValuePair<string, int>> ScoreList
        {
            get
            {
                return scorelist;
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
