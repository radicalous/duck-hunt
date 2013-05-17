// ParallaxingBackground.cs
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics; 

namespace DuckHuntCommon
{
    class ParallaxingBackground
    {
        // The image representing the parallaxing background

        Texture2D texture;

        // An array of positions of the parallaxing background

        Vector2[] positions;

        // The speed which the background is moving

        int speed;

        int bgWidth;
        int bgHeight;


        public void Initialize(ContentManager content, String texturePath, int screenWidth, int screenHeight, int speed)
        {

            bgHeight = screenHeight;

            bgWidth = screenWidth;



            // Load the background texture we will be using

            texture = content.Load<Texture2D>(texturePath);

            // Set the speed of the background

            this.speed = speed;

            // If we divide the screen with the texture width then we can determine the number of tiles need.

            // We add 1 to it so that we won't have a gap in the tiling

            positions = new Vector2[screenWidth / texture.Width + 1];

            // Set the initial positions of the parallaxing background

            for (int i = 0; i < positions.Length; i++)
            {

                // We need the tiles to be side by side to create a tiling effect

                positions[i] = new Vector2(i * texture.Width, 0);

            }

        }


        public void Update(GameTime gametime)
        {

            // Update the positions of the background
            if (positions == null)
            {
                return;
            }
            for (int i = 0; i < positions.Length; i++)
            {

                // Update the position of the screen by adding the speed

                positions[i].X += speed;

                // If the speed has the background moving to the left

                if (speed <= 0)
                {

                    // Check the texture is out of view then put that texture at the end of the screen

                    if (positions[i].X <= -texture.Width)
                    {

                        positions[i].X = texture.Width * (positions.Length - 1);

                    }

                }

                // If the speed has the background moving to the right

                else
                {

                    // Check if the texture is out of view then position it to the start of the screen

                    if (positions[i].X >= texture.Width * (positions.Length - 1))
                    {

                        positions[i].X = -texture.Width;

                    }

                }

            }

        }


        public void Draw(SpriteBatch spriteBatch)
        {

            for (int i = 0; i < positions.Length; i++)
            {

                Rectangle rectBg = new Rectangle((int)positions[i].X, (int)positions[i].Y, bgWidth, bgHeight);

                spriteBatch.Draw(texture, rectBg, Color.White);

            }

        }


    }

    class StaticBackground
    {
        // The image representing the parallaxing background

        Texture2D texture;

        // An array of positions of the parallaxing background

        Vector2[] positions;

        // The speed which the background is moving

        int speed;

        int bgWidth;
        int bgHeight;

        int _screenWidth = 0;
        int _screenHeight = 0;
        Vector2 _orgpos;

        float scale = 1.0f;

        public void Initialize(Texture2D texture1, Vector2 orgpos, int screenWidth, int screenHeight, int speed)
        {
            _orgpos = orgpos;

            _screenWidth = screenWidth;
            _screenHeight = screenHeight;

            // Load the background texture we will be using

            texture = texture1;

            // Set the speed of the background

            this.speed = speed;


            bgHeight = texture.Height;

            bgWidth = texture.Width;


            // If we divide the screen with the texture width then we can determine the number of tiles need.
            scale = _screenHeight * 1.0f / texture.Height;

            // We add 1 to it so that we won't have a gap in the tiling

            positions = new Vector2[screenWidth / (int)(texture.Width*scale) + 1];

            // Set the initial positions of the parallaxing background


            if (scale * texture.Width < _screenWidth)
            {

                for (int i = 0; i < positions.Length; i++)
                {

                    // We need the tiles to be side by side to create a tiling effect

                    positions[i] = new Vector2(i * texture.Width * scale, 0);
                    positions[i].Y = 0;
                    positions[i] += orgpos;

                }
            }
            else
            {
                positions[0].X = -((scale * texture.Width - _screenWidth) / 2);
                positions[0].Y = 0;
                positions[0] += orgpos;

            }

        }


        public void Update(GameTime gametime)
        {

            // Update the positions of the background
            return;

        }


        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            //Rectangle rectBg = new Rectangle((int)0, (int)0, _screenWidth, _screenHeight);
            Vector2 pos = new Vector2(0, 0);



            for (int i = 0; i < positions.Length; i++)
            {

                int width = bgWidth;
                int height = bgHeight;
 
                Rectangle textrc = new Rectangle(0, 0, width, height);
                Rectangle dstrc = new Rectangle();

                if (positions[i].X < _orgpos.X)
                {

                    textrc.X = (int)((_orgpos.X - positions[i].X) * 1.0 / ((_orgpos.X - positions[i].X) * 2 + _screenWidth) * textrc.Width);
                    textrc.Width = bgWidth - textrc.X * 2;

                    //
                    dstrc.X = (int)_orgpos.X;
                    dstrc.Width = (int)(textrc.Width * scale);
                    dstrc.Height = _screenHeight;
                }
                else
                {
                    if ((int)positions[i].X + bgWidth * scale > _screenWidth + _orgpos.X)
                    {
                        width = (int)((_screenWidth + _orgpos.X - positions[i].X) / scale);
                    }
                    textrc.Width = width;

                    dstrc.X = (int)positions[i].X;
                    dstrc.Width = (int)(textrc.Width * scale);
                    dstrc.Height = _screenHeight;
                }


                //spriteBatch.Draw(texture, positions[i], textrc, Color.White,
                //    0, pos, scale, SpriteEffects.None, depth);// depth = 1, back
                //spriteBatch.Draw(texture, dstrc, textrc, Color.White);
                Vector2 orgion = new Vector2(0, 0);
                spriteBatch.Draw(texture, dstrc, textrc, Color.White, 0, orgion, SpriteEffects.None, depth); // 0 f


                //spriteBatch.Draw(texture, rectBg, Color.White);

            }
            return;
        }


    }

}
