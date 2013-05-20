using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics; 


namespace GameCommon
{
    class AutoPilot
    {
        public enum Direction { LEFT, BOTTOM, RIGHT, UP, RANDOM, IN, OUT };

        // The boundary
        public Rectangle boundaryRect = new Rectangle();

        public Direction HorizationDirection
        {
            get
            {
                if (deltax > 0)
                {
                    return Direction.RIGHT;
                }
                else
                {
                    return Direction.LEFT;
                }
            }
        }
        public Direction ZDirection
        {
            get
            {
                if (detalz > 0)
                {
                    return Direction.IN;
                }
                else
                {
                    return Direction.OUT;
                }
            }
        }

        // current position
        Vector2 prePos;
        public Vector2 Position;
        //public float scale = 1.0f;

        public float depthpos = 0;

        int deltax = 1;
        int deltay = 1;
        int factorx = 1;
        int factory = 1;
        float detalz = 1;

        Random radom;
        int maxRatio = 8;
        public void Initialize(Rectangle boundary, int seed)
        {
            radom = new Random(seed);
            boundaryRect = boundary;

            // radom a intial position
            Position.X = boundary.Width / 2;
            Position.Y = boundary.Height / 2;

        }

        public void LeadDirection(Direction hor, Direction ver)
        {
            if (hor == Direction.RANDOM)
            {
                if (radom.Next(10) > 5)
                {
                    deltax = -deltax;
                }
            }
            else if (hor == Direction.LEFT)
            {
                if (deltax < 0)
                {
                    deltax = -deltax;
                }
            }
            else
            {
                if (deltax > 0)
                {
                    deltax = -deltax;
                }
            }

            if (ver == Direction.RANDOM)
            {
                if (radom.Next(10) > 5)
                {
                    deltay = -deltay;
                }
            }
            else if (ver == Direction.UP)
            {
                if (deltay > 0)
                {
                    deltay = -deltay;
                }
            }
            else
            {
                if (deltay < 0)
                {
                    deltay = -deltay;
                }
            }
        }

        public void RadomStartPos(Rectangle startSpace)
        {
            //
            Position.X = radom.Next(startSpace.Width);
            Position.Y = startSpace.Y + radom.Next(startSpace.Height);
            prePos = Position;
        }

        public void Update(GameTime gameTime)
        {
            // Update the elapsed time
            if (Position.X >= (boundaryRect.Right-10) || Position.X <= boundaryRect.X+10)
            {
                deltax = -deltax;
                factorx = radom.Next(maxRatio);
                if (factorx < 1)
                {
                    factorx = 1;
                }
                factory = radom.Next(maxRatio);
                if (factory < 1)
                {
                    factory = 1;
                }
            }
            if (Position.Y >= boundaryRect.Bottom-10 || Position.Y <= boundaryRect.Y+10)
            {
                deltay = -deltay;
                factorx = radom.Next(maxRatio);
                if (factorx < 1)
                {
                    factorx = 1;
                }
                factory = radom.Next(maxRatio);
                if (factory < 1)
                {
                    factory = 1;
                }
            }
            prePos = Position;
            Position.X += ((float)deltax) * factorx;
            Position.Y += ((float)deltay) * factory;

            depthpos += detalz;
            if (depthpos < 0 || depthpos > 50)
            {
                detalz = -detalz;
            }
            if (depthpos < 0)
            {
                depthpos = 0;
            }
            if (depthpos > 50)
            {
                depthpos = 50;
            }
        }
    }


    class DeadPilot
    {
        // The boundary
        Rectangle boundaryRect = new Rectangle();

        // current position
        public Vector2 Position;
        int deltay = 15;

        int stopcnt = 0;

        public DeadPilot(AutoPilot pilot, bool up)
        {
            Position = pilot.Position;
            if (up)
            {
                deltay = -10;
            }

            //boundaryRect = pilot.boundaryRect;
        }

        public void Initialize(Rectangle boundary)
        {
            boundaryRect = boundary;
        }


        public void Update(GameTime gameTime)
        {

            // Update the elapsed time
            //Position.X += deltax * factorx;
            if (stopcnt < 10)
            {
                stopcnt++;
                return;
            }
            Position.Y += deltay ;
        }
    }


    class DogPilot
    {
        // The boundary
        public Rectangle boundaryRect = new Rectangle();

        // current position
        public Vector2 Position;
        int deltax = 1;
        int deltay = 5;

        public enum DOGSTATE { FindingDuck, Jumpup, Jumpdown, Showup, Showdown };

        DOGSTATE state = DOGSTATE.FindingDuck;

        public DOGSTATE DogState
        {
            get { return state; }
        }
        public void NextState()
        {
            if (state == DOGSTATE.FindingDuck)
            {
                state = DOGSTATE.Jumpup;
            }
            else if (state == DOGSTATE.Jumpup)
            {
                state = DOGSTATE.Jumpdown;
            }
            else if (state == DOGSTATE.Jumpdown)
            {
                state = DOGSTATE.Showup;
            }
            else if (state == DOGSTATE.Showup)
            {
                state = DOGSTATE.Showdown;
            }
        }

        public DogPilot()
        {

        }

        public void Initialize(Rectangle dogspace)
        {
            boundaryRect = dogspace;
            Position.X = dogspace.Left;
            Position.Y = dogspace.Bottom;
        }


        public void Update(GameTime gameTime)
        {

            // Update the elapsed time
            //Position.X += deltax * factorx;
            switch (state)
            {
                case DOGSTATE.FindingDuck:
                    {
                        Position.X += deltax;
                    }
                    break;
                case DOGSTATE.Jumpup:
                    {
                        Position.Y -= deltay;
                    }
                    break;
                case DOGSTATE.Jumpdown:
                    {
                        Position.Y += deltay;
                    }
                    break;
            }

        }
    }



    class DogJumpPilot
    {
        // The boundary
        public Rectangle boundaryRect ;

        // current position
        public Vector2 Position;
        int deltax = 1;
        int deltay = 5;

        int direction = 0; // 0, up, 1, down

        public DogJumpPilot(DogPilot pilot)
        {
            boundaryRect = pilot.boundaryRect;
            Position = pilot.Position;
        }


        public void Update(GameTime gameTime)
        {

            // Update the elapsed time
            //Position.X += deltax * factorx;
            if (direction == 0)
            {
                Position.Y -= deltay;
                if ((Position.Y) <= boundaryRect.Top)
                {
                    direction = 1;
                }
            }
            else
            {
                Position.Y += deltay;
            }
        }
    }


    class DogShowPilot
    {
       // The boundary
        public Rectangle boundaryRect ;

        // current position
        public Vector2 Position;
        int deltax = 1;
        int deltay = 2;

        int direction = 0; // 0, up, 1, down

        public DogShowPilot(DogPilot pilot)
        {
            boundaryRect = pilot.boundaryRect;
            Position = pilot.Position;
        }


        public void Update(GameTime gameTime)
        {

            // Update the elapsed time
            //Position.X += deltax * factorx;
            if (direction == 0)
            {
                Position.Y -= deltay/2;
                if (Position.Y <= boundaryRect.Top+56)
                {
                    direction = 1;
                }
            }
            else
            {
                Position.Y += deltay;
            }
        }
    }

    class CloudPilot
    {
        // The boundary
        public Rectangle boundaryRect = new Rectangle();


        // current position
        Vector2 prePos;
        public Vector2 Position;
        //public float scale = 1.0f;

        public float depthpos = 0;

        int deltax = 1;
        int deltay = 1;
        int factorx = 1;
        int factory = 1;

        Random radom;
        int maxRatio = 8;
        public void Initialize(Rectangle boundary, int seed)
        {
            radom = new Random(seed);
            boundaryRect = boundary;

            // radom a intial position
            Position.X = boundary.Width / 2;
            Position.Y =  100;

        }


        public void RadomStartPos(Rectangle startSpace)
        {
            //
            Position.X = radom.Next(startSpace.Width);
            Position.Y = startSpace.Y + 20;
            prePos = Position;
        }

        int elapsedTime = 0;
        public void Update(GameTime gameTime)
        {
            // Update the elapsed time
            if (Position.X >= (boundaryRect.Right + 100) )
            {
                Position.X = boundaryRect.X - 100;
            }

            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime > 50)
            {
                prePos = Position;
                Position.X += ((float)deltax) * factorx;
                elapsedTime = 0;
            }

        }
    }

}
