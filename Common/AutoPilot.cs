using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics; 


namespace GameCommon
{
    public enum Direction { LEFT, BOTTOM, RIGHT, UP, RANDOM, IN, OUT };
    public enum PilotType { DUCKNORMAL, DUCKQUICK, DUCKFLOWER, DUCKDEAD,DUCKFLYAWAY, 
        DOGSEEK, DOGJUMP, DOGSHOW, 
        CLOUD,
    };

    interface AiPilot
    {
        void Initialize(Rectangle boundary, int seed);
        void Update(GameTime gameTime);
        Direction GetHorizationDirection();
        Direction GetZDirection();
        float GetDepth();
        Vector2 GetPosition();
        PilotType GetType();
    }


    class PilotManager
    {
        static PilotManager instance;
        public PilotManager()
        {
        }

        public static PilotManager GetInstance()
        {
            if (instance == null)
            {
                instance = new PilotManager();
            }
            return instance;
        }

        public AiPilot CreatePilot(PilotType type)
        {
            Vector2 pos = Vector2.Zero;
            return CreatePilot(type, pos);
        }

        public AiPilot CreatePilot(PilotType type, Vector2 pos)
        {
            string name = "";
            return CreatePilot(type, pos, name);
        }
        public AiPilot CreatePilot(PilotType type, Vector2 pos, string clustername)
        {
            AiPilot pilot = null;

            switch (type)
            {
                case PilotType.DUCKNORMAL:
                case PilotType.DUCKQUICK:
                    {
                        pilot = new DuckNormalPilot();
                    }
                    break;
                case PilotType.DUCKFLYAWAY:
                    {
                        pilot = new DuckFlyawayPilot(pos);
                    }
                    break;
                case PilotType.DUCKDEAD:
                    {
                        pilot = new DuckDeadPilot(pos);
                    }
                    break;
                case PilotType.DOGSEEK:
                    {
                        pilot = new DogPilot();
                    }
                    break;
                case PilotType.DOGJUMP:
                    {
                        pilot = new DogJumpPilot(pos);
                    }
                    break;
                case PilotType.DOGSHOW:
                    {
                        pilot = new DogShowPilot(pos);
                    }
                    break;
                case PilotType.CLOUD:
                    {
                        pilot = new CloudPilot();
                    }
                    break;
                default:
                    break;
            }


            return pilot;
        }

        public void ReturnPilot(AiPilot pilot)
        {
        }
    }


    class DuckNormalPilot: AiPilot
    {

        // The boundary
        public Rectangle boundaryRect = new Rectangle();

        public Direction GetHorizationDirection()
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
        public Direction GetZDirection()
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

        // current position
        Vector2 prePos;
        Vector2 Position;
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

            Rectangle startSpace = new Rectangle(0, 0 + (int)(0.9 * boundaryRect.Height),
                boundaryRect.Width, (int)(boundaryRect.Height * 0.1));

            LeadDirection(Direction.RANDOM, Direction.UP);
            RadomStartPos(startSpace);

        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        public float GetDepth()
        {
            return depthpos;
        }
        public PilotType GetType()
        {
            return PilotType.DUCKNORMAL;
        }

        void LeadDirection(Direction hor, Direction ver)
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

        void RadomStartPos(Rectangle startSpace)
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


    class DuckDeadPilot: AiPilot
    {
        // The boundary
        Rectangle boundaryRect = new Rectangle();

        // current position
        Vector2 Position;
        int deltay = 15;

        int stopcnt = 0;

        public DuckDeadPilot(Vector2 pos)
        {
            Position = pos;
        }

        public void Initialize(Rectangle space, int seed)
        {
            boundaryRect = space;
        }


        public Vector2 GetPosition()
        {
            return Position;
        }


        public float GetDepth()
        {
            return 0f;
        }

        public PilotType GetType()
        {
            return PilotType.DUCKDEAD;
        }
        public void Initialize(Rectangle boundary)
        {
            boundaryRect = boundary;
        }

        public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        public Direction GetZDirection()
        {
            return Direction.IN;
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

    class DuckFlyawayPilot : AiPilot
    {
        // The boundary
        Rectangle boundaryRect = new Rectangle();

        // current position
        Vector2 Position;
        int deltay = -15;

        int stopcnt = 0;

        public DuckFlyawayPilot(Vector2 pos)
        {
            Position = pos;
        }

        public void Initialize(Rectangle space, int seed)
        {
            boundaryRect = space;
        }


        public Vector2 GetPosition()
        {
            return Position;
        }

        float depthpos = 0;
        public float GetDepth()
        {
            return depthpos;
        }

        public PilotType GetType()
        {
            return PilotType.DUCKFLYAWAY;
        }

        public void Initialize(Rectangle boundary)
        {
            boundaryRect = boundary;
        }

        public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        public Direction GetZDirection()
        {
            return Direction.IN;
        }


        public void Update(GameTime gameTime)
        {
            // Update the elapsed time
            if (stopcnt < 10)
            {
                stopcnt++;
                return;
            }
            Position.Y += deltay;
        }
    }

    class DogPilot: AiPilot
    {
        // The boundary
        public Rectangle boundaryRect = new Rectangle();

        // current position
        Vector2 Position;
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

        public void Initialize(Rectangle dogspace, int seed)
        {
            boundaryRect = dogspace;
            Position.X = dogspace.Left;
            Position.Y = dogspace.Bottom;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }


        float depth = 0;
        public float GetDepth()
        {
            return depth;
        }

        public PilotType GetType()
        {
            return PilotType.DOGSEEK;
        }

        public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        public Direction GetZDirection()
        {
            return Direction.IN;
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



    class DogJumpPilot: AiPilot
    {
        // The boundary
        public Rectangle boundaryRect ;

        // current position
        Vector2 Position;
        int deltax = 1;
        int deltay = 6;

        int direction = 0; // 0, up, 1, down

        public void Initialize(Rectangle jumpspace, int seed)
        {
            boundaryRect = jumpspace;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        public PilotType GetType()
        {
            return PilotType.DOGJUMP;
        }
        public DogJumpPilot(Vector2 pos)
        {
            //boundaryRect = pilot.boundaryRect;
            Position = pos;
        }
        public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        public Direction GetZDirection()
        {
            return Direction.IN;
        }
        public float GetDepth()
        {
            return 0f;
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


    class DogShowPilot: AiPilot
    {
       // The boundary
        public Rectangle boundaryRect ;

        // current position
        Vector2 Position;
        int deltax = 1;
        int deltay = 6;

        int direction = 0; // 0, up, 1, down


        public DogShowPilot(Vector2 pos)
        {
            // boundaryRect = pilot.boundaryRect;
            Position = pos;
        }
        
        public void Initialize(Rectangle space, int seed)
        {
            boundaryRect = space;
        }

        public PilotType GetType()
        {
            return PilotType.DOGSHOW;
        }


        public Vector2 GetPosition()
        {
            return Position;
        }

        public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        public Direction GetZDirection()
        {
            return Direction.IN;
        }

        public float GetDepth()
        {
            return 0f;
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

    class CloudPilot: AiPilot
    {
        // The boundary
        public Rectangle boundaryRect = new Rectangle();


        // current position
        Vector2 prePos;
        Vector2 Position;
        //public float scale = 1.0f;

        public float depthpos = 0;

        int deltax = 1;
        int deltay = 1;
        int factorx = 1;
        int factory = 1;

        Random radom;
        int maxRatio = 8;

        public PilotType GetType()
        {
            return PilotType.CLOUD;
        }
        public void Initialize(Rectangle boundary, int seed)
        {
            radom = new Random(seed);
            boundaryRect = boundary;

            // radom a intial position
            Position.X = boundary.Width / 2;
            Position.Y =  100;

        }
        public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        public Direction GetZDirection()
        {
            return Direction.IN;
        }

        public Vector2 GetPosition()
        {
            return Position;
        }

        public float GetDepth()
        {
            return depthpos;
        }

        void RadomStartPos(Rectangle startSpace)
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
