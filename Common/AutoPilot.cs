using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace GameCommon
{
    public enum Direction { LEFT, BOTTOM, RIGHT, UP, RANDOM, IN, OUT };
    public enum PilotType
    {
        DUCKNORMAL, DUCKQUICK, DUCKFLOWER, DUCKDEAD, DUCKFLYAWAY,
        DUCKEIGHT, DUCKEIGHTDEPTH, DUCKCIRCLE, DUCKCIRCLEDEPTH,
        DUCKELLIPSE, DUCKELLIPSEDEPTH, DUCKSIN, DUCKSINDEPTH,
        DUCKLINE, DUCKREN, DUCKILOVEU_I, DUCKILOVEU_L, DUCKILOVEU_U,
        DOGSEEK, DOGJUMP, DOGSHOW,
        CLOUD,PARROT,
    };

    abstract class AiPilot
    {
        public abstract void Initialize(Rectangle boundary, int seed);
        public abstract void SetStartPos(Vector2 pos);
        public abstract void SetEndPos(Vector2 pos);
        public abstract void SetSpeedRatio(float speedRatio);
        public abstract void Update(GameTime gameTime);
        public abstract Direction GetHorizationDirection();
        public abstract Direction GetZDirection();
        public abstract float GetDepth();
        public abstract Vector2 GetPosition();
        public abstract PilotType GetType();
    }

    abstract class BasePilot : AiPilot
    {
        override public void SetStartPos(Vector2 pos)
        {
        }
        override public void SetEndPos(Vector2 pos)
        {
        }
        override public void SetSpeedRatio(float speedRatio)
        {
        }

    }

    
    struct pilotGroupInfo
    {
        public int idx;
        public Point endpoint;
        public float speed_ratio;
        public int[] dirs;
    }

    class PilotManager
    {
        Dictionary<string, pilotGroupInfo> duckEightPilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckCirclePilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckEllipsePilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckSinPilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckLinePilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckILoveU_IPilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckILoveU_LPilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckILoveU_UPilotGroup = new Dictionary<string, pilotGroupInfo>();

        static int count = 0;
        static PilotManager instance;
        public PilotManager()
        {

        }

        ~PilotManager()
        {
            duckEightPilotGroup = null;
            duckCirclePilotGroup = null;
            duckEllipsePilotGroup = null;
            duckSinPilotGroup = null;
            duckLinePilotGroup = null;
            duckILoveU_IPilotGroup = null;
            duckILoveU_LPilotGroup = null;
            duckILoveU_UPilotGroup = null;
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

        public AiPilot CreatePilot(PilotType type, string groupname)
        {
            Vector2 pos = Vector2.Zero;
            return CreatePilot(type, pos, groupname);
        }

        public AiPilot CreatePilot(PilotType type, Vector2 pos)
        {
            string name = "";
            return CreatePilot(type, pos, name);
        }

        private pilotGroupInfo GenPGI()
        {
            pilotGroupInfo pgi = new pilotGroupInfo();
            pgi.idx = 0;
            pgi.endpoint = new Point(0, 0);
            DateTime now = System.DateTime.Now;
            int s = now.Hour * 60 * 60 + now.Minute * 60 + now.Second + count++;
            Random rdm = new Random(s);
            pgi.endpoint.X = rdm.Next(-50, 50);
            pgi.endpoint.Y = rdm.Next(-50, 50);

            pgi.speed_ratio = (float)(rdm.Next(50, 200)) / 100;

            pgi.dirs = new int[10];
            for (int i = 0; i < 10; i++)
            {
                pgi.dirs[i] = rdm.Next(0,3);
            }
           
            return pgi;
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
                case PilotType.DUCKEIGHT:
                    {
                        if (duckEightPilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckEightPilotGroup[clustername];
                            pgi.idx++;
                            duckEightPilotGroup[clustername] = pgi;
                            pilot = new DuckEightPilot(pos, pgi);

                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckEightPilotGroup.Add(clustername, pgi);
                            pilot = new DuckEightPilot(pos, pgi);

                        }
                    }
                    break;
                case PilotType.DUCKEIGHTDEPTH:
                    {

                        if (duckEightPilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckEightPilotGroup[clustername];
                            pgi.idx++;
                            duckEightPilotGroup[clustername] = pgi;
                            pilot = new DuckEightPilotWithDepth(pos, pgi);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckEightPilotGroup.Add(clustername, pgi);
                            pilot = new DuckEightPilotWithDepth(pos, pgi);
                        }
                    }
                    break;
                case PilotType.DUCKCIRCLE:
                    {

                        if (duckCirclePilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckCirclePilotGroup[clustername];
                            pgi.idx++;
                            duckCirclePilotGroup[clustername] = pgi;
                            pilot = new DuckCirclePilot(pos, pgi);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckCirclePilotGroup.Add(clustername, pgi);
                            pilot = new DuckCirclePilot(pos, pgi);
                        }

                    }
                    break;
                case PilotType.DUCKCIRCLEDEPTH:
                    {
                        if (duckCirclePilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckCirclePilotGroup[clustername];
                            pgi.idx++;
                            duckCirclePilotGroup[clustername] = pgi;
                            pilot = new DuckCirclePilotWithDepth(pos, pgi);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckCirclePilotGroup.Add(clustername, pgi);
                            pilot = new DuckCirclePilotWithDepth(pos, pgi);
                        }
                    }
                    break;
                case PilotType.DUCKELLIPSE:
                    {

                        if (duckEllipsePilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckEllipsePilotGroup[clustername];
                            pgi.idx++;
                            duckEllipsePilotGroup[clustername] = pgi;
                            pilot = new DuckEllipsePilot(pos, pgi);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckEllipsePilotGroup.Add(clustername, pgi);
                            pilot = new DuckEllipsePilot(pos, pgi);
                        }
                      
                    }
                    break;
                case PilotType.DUCKELLIPSEDEPTH:
                    {

                        if (duckEllipsePilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckEllipsePilotGroup[clustername];
                            pgi.idx++;
                            duckEllipsePilotGroup[clustername] = pgi;
                            pilot = new DuckEllipsePilotWithDepth(pos, pgi);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckEllipsePilotGroup.Add(clustername, pgi);
                            pilot = new DuckEllipsePilotWithDepth(pos, pgi);
                        }
                    }
                    break;
                case PilotType.DUCKSIN:
                    {
                        if (duckSinPilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckSinPilotGroup[clustername];
                            pgi.idx++;
                            duckSinPilotGroup[clustername] = pgi;
                            pilot = new DuckSinPilot(pos, pgi);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckSinPilotGroup.Add(clustername, pgi);
                            pilot = new DuckSinPilot(pos, pgi);
                        }
                    }
                    break;
                case PilotType.DUCKSINDEPTH:
                    {

                        if (duckSinPilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckSinPilotGroup[clustername];
                            pgi.idx++;
                            duckSinPilotGroup[clustername] = pgi;
                            pilot = new DuckSinPilotWithDepth(pos, pgi);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckSinPilotGroup.Add(clustername, pgi);
                            pilot = new DuckSinPilotWithDepth(pos, pgi);
                        }
                    }
                    break;
                case PilotType.DUCKLINE:
                    {
                        if (duckLinePilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckLinePilotGroup[clustername];
                            pgi.idx++;
                            duckLinePilotGroup[clustername] = pgi;
                            pilot = new DuckLinePilot(pos, pgi);

                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckLinePilotGroup.Add(clustername, pgi);
                            pilot = new DuckLinePilot(pos, pgi);

                        }

                    }
                    break;
                case PilotType.DUCKREN:
                    {
                        //not done yet
                    }
                    break;
                case PilotType.DUCKILOVEU_I:
                    {
                        if (duckILoveU_IPilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckILoveU_IPilotGroup[clustername];
                            pgi.idx++;
                            duckILoveU_IPilotGroup[clustername] = pgi;
                            pilot = new DuckILoveU_IPilot(pos, pgi);

                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckILoveU_IPilotGroup.Add(clustername, pgi);
                            pilot = new DuckILoveU_IPilot(pos, pgi);

                        }
                    }
                    break;
                case PilotType.DUCKILOVEU_L:
                    {
                        if (duckILoveU_LPilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckILoveU_LPilotGroup[clustername];
                            pgi.idx++;
                            duckILoveU_LPilotGroup[clustername] = pgi;
                            pilot = new DuckILoveU_LPilot(pos, pgi);

                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckILoveU_LPilotGroup.Add(clustername, pgi);
                            pilot = new DuckILoveU_LPilot(pos, pgi);

                        }
                    }
                    break;
                case PilotType.DUCKILOVEU_U:
                    {
                        if (duckILoveU_UPilotGroup.ContainsKey(clustername))
                        {
                            pilotGroupInfo pgi = duckILoveU_UPilotGroup[clustername];
                            pgi.idx++;
                            duckILoveU_UPilotGroup[clustername] = pgi;
                            pilot = new DuckILoveU_UPilot(pos, pgi);

                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckILoveU_UPilotGroup.Add(clustername, pgi);
                            pilot = new DuckILoveU_UPilot(pos, pgi);

                        }
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
                case PilotType.PARROT:
                    {
                        pilot = new ParrotPilot();
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


    class ParrotPilot : BasePilot
    {

        // The boundary
        public Rectangle boundaryRect = new Rectangle();

        override public Direction GetHorizationDirection()
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
        override public Direction GetZDirection()
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
        override public void Initialize(Rectangle boundary, int seed)
        {
            radom = new Random(seed);
            boundaryRect = boundary;

            // radom a intial position
            Position.X = boundary.Width / 2;
            Position.Y = boundary.Height / 2;

            Rectangle startSpace = new Rectangle(0, 0 + (int)(0.1 * boundaryRect.Height),
                boundaryRect.Width, (int)(boundaryRect.Height * 0.8));

            LeadDirection(Direction.RANDOM, Direction.UP);
            RadomStartPos(startSpace);

        }

        override public Vector2 GetPosition()
        {
            return Position;
        }

        override public float GetDepth()
        {
            return depthpos;
        }
        override public PilotType GetType()
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
            if (deltax > 0)
            {
                // flying right
                Position.X = startSpace.Left;
            }
            else
            {
                // flying left
                Position.X = startSpace.Right;
            }
            Position.Y = startSpace.Y + radom.Next(startSpace.Height);
            prePos = Position;
            boundaryRect = startSpace;
        }

        override public void Update(GameTime gameTime)
        {
            // Update the elapsed time
            if (deltax < 0 && Position.X <= boundaryRect.X )
            {
            }
            if (deltax > 0 && Position.X >= boundaryRect.Right)
            {
            }

            if (deltay > 0 && Position.Y >= boundaryRect.Bottom - 10)
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

            if (deltay < 0 && Position.Y <= boundaryRect.Y + 10)
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

        }
    }


    class DuckNormalPilot : BasePilot
    {

        // The boundary
        public Rectangle boundaryRect = new Rectangle();

        override public Direction GetHorizationDirection()
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
        override public Direction GetZDirection()
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
        override public void Initialize(Rectangle boundary, int seed)
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

        override public Vector2 GetPosition()
        {
            return Position;
        }

        override public float GetDepth()
        {
            return depthpos;
        }
        override public PilotType GetType()
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

        override public void Update(GameTime gameTime)
        {
            // Update the elapsed time
            if (deltax < 0 && Position.X <= boundaryRect.X + 10)
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
            if (deltax > 0 && Position.X >= boundaryRect.Right - 10)
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

            if (deltay > 0 && Position.Y >= boundaryRect.Bottom - 10)
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

            if (deltay < 0 && Position.Y <= boundaryRect.Y + 10)
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

            /*
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
             */
        }
    }


    class DuckNormalDepthPilot : BasePilot
    {

        // The boundary
        public Rectangle boundaryRect = new Rectangle();

        override public Direction GetHorizationDirection()
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
        override public Direction GetZDirection()
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
        override public void Initialize(Rectangle boundary, int seed)
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

        override public Vector2 GetPosition()
        {
            return Position;
        }

        override public float GetDepth()
        {
            return depthpos;
        }
        override public PilotType GetType()
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

        override public void Update(GameTime gameTime)
        {
            // Update the elapsed time
            if (Position.X >= (boundaryRect.Right - 10) || Position.X <= boundaryRect.X + 10)
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
            if (Position.Y >= boundaryRect.Bottom - 10 || Position.Y <= boundaryRect.Y + 10)
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


    class DuckDeadPilot : BasePilot
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

        override public void Initialize(Rectangle space, int seed)
        {
            boundaryRect = space;
        }


        override public Vector2 GetPosition()
        {
            return Position;
        }


        override public float GetDepth()
        {
            return 0f;
        }

        override public PilotType GetType()
        {
            return PilotType.DUCKDEAD;
        }


        override public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        override public Direction GetZDirection()
        {
            return Direction.IN;
        }


        override public void Update(GameTime gameTime)
        {

            // Update the elapsed time
            //Position.X += deltax * factorx;
            if (stopcnt < 10)
            {
                stopcnt++;
                return;
            }
            Position.Y += deltay;
        }
    }

    class DuckFlyawayPilot : BasePilot
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

        override public void Initialize(Rectangle space, int seed)
        {
            boundaryRect = space;
        }


        override public Vector2 GetPosition()
        {
            return Position;
        }

        float depthpos = 0;
        override public float GetDepth()
        {
            return depthpos;
        }

        override public PilotType GetType()
        {
            return PilotType.DUCKFLYAWAY;
        }


        override public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        override public Direction GetZDirection()
        {
            return Direction.IN;
        }


        override public void Update(GameTime gameTime)
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

    static class Constants
    {
        public const double Pi = 3.14159;
        public const int Ratio = 2;
        public const int Groupfactor = 10;
        public const int MaxLineSteps = 160;
        public const int MaxCurveSteps = 400;
    }

    interface DepthCom
    {
        float update_depth(float cur_dp);
    }

    class DepthCosCom : DepthCom
    {
        double z = 0.0;
        double z_delta = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public void setStartZ(int seed)
        {
            Random rdm = new Random(seed);

            z = 0.0 + (double)(rdm.Next(0, Constants.MaxCurveSteps)) / (double)Constants.MaxCurveSteps * 2 * Constants.Pi;
        }
        public float update_depth(float cur_dp)
        {
            z += z_delta;
            if (z > 2 * Constants.Pi)
                z = 0.0;

            float depthpos = (float)(Math.Cos(z) * 100.0 + 100.0) / 2;
            if (depthpos < 0)
                depthpos = 0;
            else if (depthpos > 60)
                depthpos = 60;

            return depthpos;
        }
    }

    class DepthLinearCom : DepthCom
    {
        double z = 0.0;
        double z_delta = 100.0 / Constants.MaxCurveSteps;

        public float update_depth(float cur_dp)
        {
            z += z_delta;

            float depthpos = (float)(z);
            if (depthpos < 0)
                depthpos = 0;
            else if (depthpos > 60)
                depthpos = 60;

            return depthpos;
        }
    }


    class DuckPilot : BasePilot
    {
        protected Rectangle boundaryRect = new Rectangle();

        protected Vector2 start_pos; //random start position
        protected Vector2 end_pos; //different end position
        protected Vector2 center_pos; //
        protected int center_pos_idx = 0;
        protected Rectangle centerBoundaryRect = new Rectangle();

        protected int lineStep = 0;
        protected int max_lineSteps = Constants.MaxLineSteps;

        // current position
        protected Vector2 Position;
        protected float depthpos = 0;
        protected int depth_seed = 0;

        //group info
        protected pilotGroupInfo group_info;

        public DuckPilot(Vector2 pos, pilotGroupInfo pgi)
        {
            Position = pos;

            group_info = pgi;
        }

        override public void Initialize(Rectangle space, int seed)
        {
            boundaryRect = space;
            depth_seed = seed;

            Random rdm = new Random(seed);

            int r = Math.Min(boundaryRect.Height, boundaryRect.Width);
            r /= (Constants.Ratio * 2);

            end_pos.X = boundaryRect.Center.X + (group_info.endpoint.X) * r / 50;
            end_pos.Y = boundaryRect.Center.Y + (group_info.endpoint.Y) * r / 50;

            center_pos = end_pos;

            start_pos.X = rdm.Next(boundaryRect.Left, boundaryRect.Right);
            start_pos.Y = boundaryRect.Bottom;


            centerBoundaryRect.Width = 2 * r;
            centerBoundaryRect.Height = 2 * r;
            centerBoundaryRect.X = boundaryRect.Center.X - r;
            centerBoundaryRect.Y = boundaryRect.Center.Y - r;

            max_lineSteps = (int)(max_lineSteps / group_info.speed_ratio);

            Position = start_pos;
        }

        override public void SetStartPos(Vector2 pos)
        {
            start_pos = pos;
        }

        public override void SetEndPos(Vector2 pos)
        {
            end_pos = pos;
            center_pos = end_pos;
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            group_info.speed_ratio = speedRatio;

            max_lineSteps = (int)(max_lineSteps / group_info.speed_ratio);
        }

        override public Vector2 GetPosition()
        {
            return Position;
        }

        override public float GetDepth()
        {
            return depthpos;
        }

        override public PilotType GetType()
        {
            return PilotType.DUCKEIGHT;
        }

        override public Direction GetHorizationDirection()
        {
            return start_pos.X < end_pos.X ? Direction.RIGHT : Direction.LEFT;
        }

        override public Direction GetZDirection()
        {
            return Direction.IN;
        }

        override public void Update(GameTime gameTime)
        {
            if (lineStep <= 0)
            {
                lineStep++;
            }
            else if (lineStep <= max_lineSteps)
            {
                double k = (end_pos.Y - start_pos.Y) / (end_pos.X - start_pos.X);
                double dx = (double)((end_pos.X - start_pos.X) / max_lineSteps);
                Position.X = (float)(start_pos.X + dx * lineStep);
                Position.Y = (float)(start_pos.Y + k * dx * lineStep);
                lineStep++;
            }
        }

        public bool InCurve()
        {
            return lineStep > max_lineSteps ? true : false;
        }

        public void updateCenter()
        {
            int d = 20;
            float[] xs = { -d, 0, d, 0 };
            float[] ys = { 0, d, 0, -d };

            Vector2 tmp_pos = center_pos;
            tmp_pos.X += (float)(xs[group_info.dirs[center_pos_idx]]);
            tmp_pos.Y += (float)(ys[group_info.dirs[center_pos_idx]]);
         
            if (centerBoundaryRect.Contains((int)tmp_pos.X, (int)tmp_pos.Y))
            {
                center_pos = tmp_pos;
            }
            
            if (center_pos_idx >= 10)
            {
                center_pos_idx = 0;
            }
        }
    }

    class DuckEightPilot : DuckPilot
    {
        double cur_angle = 0.0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckEightPilot(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {

        }

        private void adjustEndPos()
        {
            cur_angle += delta_angle * group_info.idx * Constants.Groupfactor;
            if (cur_angle > 2 * Constants.Pi)
                cur_angle = 0.0;

            float a = Math.Min(boundaryRect.Width, boundaryRect.Height);
            a /= Constants.Ratio;

            end_pos.X = center_pos.X + (float)(a * Math.Sin(cur_angle));
            end_pos.Y = center_pos.Y + (float)(a * Math.Cos(cur_angle) * Math.Sin(cur_angle));
        }

        public override void Initialize(Rectangle space, int seed)
        {
            base.Initialize(space, seed);

            adjustEndPos();
        }

        public override void SetEndPos(Vector2 pos)
        {
            base.SetEndPos(pos);

            adjustEndPos();
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            base.SetSpeedRatio(speedRatio);

            delta_angle = delta_angle * group_info.speed_ratio;
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKEIGHT;
        }

        public override Direction GetHorizationDirection()
        {
            if (InCurve())
            {
                if (cur_angle >= 0 && cur_angle < Constants.Pi * 0.5)
                {
                    return Direction.RIGHT;
                }
                else if (cur_angle >= Constants.Pi * 0.5 && cur_angle < Constants.Pi * 1.5)
                {
                    return Direction.LEFT;
                }
                else
                {
                    return Direction.RIGHT;
                }
            }
            else
            {
                return base.GetHorizationDirection();
            }
        }

        public override Direction GetZDirection()
        {
            return base.GetZDirection();
        }

        public override void Update(GameTime gameTime)
        {
            if (InCurve())
            {
                cur_angle += delta_angle;
                if (cur_angle > 2 * Constants.Pi)
                {                   
                    updateCenter();
                    cur_angle = 0.0;
                }
               
                float a = Math.Min(boundaryRect.Width, boundaryRect.Height);
                a /= Constants.Ratio;

                Position.X = center_pos.X + (float)(a * Math.Sin(cur_angle));
                Position.Y = center_pos.Y + (float)(a * Math.Cos(cur_angle) * Math.Sin(cur_angle));
            }
            else
            {
                base.Update(gameTime);
            }
        }
    }

    class DuckEightPilotWithDepth : DuckEightPilot
    {
        DepthCosCom d = new DepthCosCom();
        int set_start_z = 0;

        public DuckEightPilotWithDepth(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKEIGHTDEPTH;
        }

        public override void Update(GameTime gameTime)
        {
            if (set_start_z == 0)
            {
                d.setStartZ(depth_seed);
                set_start_z = 1;
            }
            depthpos = d.update_depth(depthpos);
            base.Update(gameTime);
        }
    }

    class DuckCirclePilot : DuckPilot
    {
        double cur_angle = 0.0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckCirclePilot(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {

        }

        private void adjustEndPos()
        {
            cur_angle += delta_angle * group_info.idx * Constants.Groupfactor;
            if (cur_angle > 2 * Constants.Pi)
                cur_angle = 0.0;

            float r = Math.Min(boundaryRect.Width, boundaryRect.Height);
            r /= (3 * Constants.Ratio / 2);

            end_pos.X = center_pos.X + (float)(r * Math.Cos(cur_angle));
            end_pos.Y = center_pos.Y + (float)(r * Math.Sin(cur_angle));
        }

        public override void Initialize(Rectangle space, int seed)
        {
            base.Initialize(space, seed);

            adjustEndPos();
        }

        public override void SetEndPos(Vector2 pos)
        {
            base.SetEndPos(pos);

            adjustEndPos();
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            base.SetSpeedRatio(speedRatio);

            delta_angle = delta_angle * group_info.speed_ratio;
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKCIRCLE;
        }

        public override Direction GetHorizationDirection()
        {
            if (InCurve())
            {
                if (cur_angle >= 0 && cur_angle < Constants.Pi)
                {
                    return Direction.LEFT;
                }
                else if (cur_angle >= Constants.Pi && cur_angle < Constants.Pi * 2)
                {
                    return Direction.RIGHT;
                }
                else
                {
                    return Direction.RIGHT;
                }
            }
            else
            {
                return base.GetHorizationDirection();
            }
        }

        public override Direction GetZDirection()
        {
            return base.GetZDirection();
        }

        public override void Update(GameTime gameTime)
        {
            if (InCurve())
            {
                cur_angle += delta_angle;
                if (cur_angle > 2 * Constants.Pi)
                {
                    updateCenter();
                    cur_angle = 0.0;
                }
                float r = Math.Min(boundaryRect.Width, boundaryRect.Height);
                r /= (3 * Constants.Ratio / 2);

                Position.X = center_pos.X + (float)(r * Math.Cos(cur_angle));
                Position.Y = center_pos.Y + (float)(r * Math.Sin(cur_angle));

            }
            else
                base.Update(gameTime);
        }
    }

    class DuckCirclePilotWithDepth : DuckCirclePilot
    {
        DepthCosCom d = new DepthCosCom();
        int set_start_z = 0;

        public DuckCirclePilotWithDepth(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKCIRCLEDEPTH;
        }

        public override void Update(GameTime gameTime)
        {
            if (set_start_z == 0)
            {
                d.setStartZ(depth_seed);
                set_start_z = 1;
            }
            depthpos = d.update_depth(depthpos);
            base.Update(gameTime);
        }
    }

    class DuckEllipsePilot : DuckPilot
    {
        double cur_angle = 0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckEllipsePilot(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {

        }

        private void adjustEndPos()
        {
            cur_angle += delta_angle * group_info.idx * Constants.Groupfactor;
    
            if (cur_angle > 2 * Constants.Pi)
                cur_angle = 0;

            float a = boundaryRect.Width / (2 * Constants.Ratio);
            float b = boundaryRect.Height / (2 * Constants.Ratio);

            end_pos.X = center_pos.X + (float)(a * Math.Cos(cur_angle));
            end_pos.Y = center_pos.Y + (float)(b * Math.Sin(cur_angle));
        }

        public override void Initialize(Rectangle space, int seed)
        {
            base.Initialize(space, seed);

            adjustEndPos();
        }

        public override void SetEndPos(Vector2 pos)
        {
            base.SetEndPos(pos);

            adjustEndPos();
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            base.SetSpeedRatio(speedRatio);

            delta_angle = delta_angle * group_info.speed_ratio;
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKELLIPSE;
        }

        public override Direction GetHorizationDirection()
        {
            if (InCurve())
            {
                if (cur_angle >= 0 && cur_angle < Constants.Pi)
                {
                    return Direction.LEFT;
                }
                else if (cur_angle >= Constants.Pi && cur_angle < Constants.Pi * 2)
                {
                    return Direction.RIGHT;
                }
                else
                {
                    return Direction.RIGHT;
                }
            }
            else
            {
                return base.GetHorizationDirection();
            }
        }

        public override Direction GetZDirection()
        {
            return base.GetZDirection();
        }

        public override void Update(GameTime gameTime)
        {
            if (InCurve())
            {
                float a = boundaryRect.Width / (2 * Constants.Ratio);
                float b = boundaryRect.Height / (2 * Constants.Ratio);

                cur_angle += delta_angle;
                if (cur_angle > 2 * Constants.Pi)
                {
                    updateCenter();
                    cur_angle = 0.0;
                }
                Position.X = center_pos.X + (float)(a * Math.Cos(cur_angle));
                Position.Y = center_pos.Y + (float)(b * Math.Sin(cur_angle));
            }
            else
            {
                base.Update(gameTime);
            }
        }
    }

    class DuckEllipsePilotWithDepth : DuckEllipsePilot
    {
        DepthCosCom d = new DepthCosCom();
        int set_start_z = 0;

        public DuckEllipsePilotWithDepth(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKELLIPSEDEPTH;
        }

        public override void Update(GameTime gameTime)
        {
            if (set_start_z == 0)
            {
                d.setStartZ(depth_seed);
                set_start_z = 1;
            }
            depthpos = d.update_depth(depthpos);
            base.Update(gameTime);
        }
    }

    class DuckSinPilot : DuckPilot
    {
        int left2right = 1;
        int hor_steps = 0;
        int max_curveSteps = Constants.MaxCurveSteps;
        double cur_angle = 0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckSinPilot(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {

        }

        private void adjustEndPos()
        {
            cur_angle += delta_angle * group_info.idx * Constants.Groupfactor;
            if (left2right == 1)
            {
                hor_steps += group_info.idx * Constants.Groupfactor;
            }
            else
            {
                hor_steps -= group_info.idx * Constants.Groupfactor;
            }
            float a = boundaryRect.Width / Constants.Ratio;
            float b = boundaryRect.Height / (3 * Constants.Ratio / 2);

            end_pos.X = center_pos.X + (float)hor_steps * a / max_curveSteps;
            if (end_pos.X >= boundaryRect.Right)
            {
                end_pos.X = boundaryRect.Right;
                left2right = 0;
            }
            else if (end_pos.X <= boundaryRect.Left)
            {
                end_pos.X = boundaryRect.Left;
                left2right = 1;
            }
            end_pos.Y = center_pos.Y + (float)(b * Math.Sin(cur_angle));
        }

        public override void Initialize(Rectangle space, int seed)
        {
            base.Initialize(space, seed);
            
            max_curveSteps = (int)(max_curveSteps / group_info.speed_ratio);

            adjustEndPos();
        }

        public override void SetEndPos(Vector2 pos)
        {
            base.SetEndPos(pos);

            adjustEndPos();
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            base.SetSpeedRatio(speedRatio);

            delta_angle = delta_angle * group_info.speed_ratio;
            max_curveSteps = (int)(max_curveSteps / group_info.speed_ratio);
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKSIN;
        }

        public override Direction GetHorizationDirection()
        {
            if (InCurve())
            {
                if (left2right == 1)
                    return Direction.RIGHT;
                else
                    return Direction.LEFT;
            }
            else
                return base.GetHorizationDirection();
        }

        public override Direction GetZDirection()
        {
            return base.GetZDirection();
        }

        public override void Update(GameTime gameTime)
        {
            if (InCurve())
            {
                cur_angle += delta_angle;
                if (left2right == 1)
                    hor_steps++;
                else
                    hor_steps--;

                float a = boundaryRect.Width / Constants.Ratio;
                float b = boundaryRect.Height / (3 * Constants.Ratio / 2);

                Position.X = center_pos.X + (float)hor_steps * a / Constants.MaxCurveSteps;
                if (Position.X >= boundaryRect.Right)
                {
                    Position.X = boundaryRect.Right;
                    left2right = 0;
                }
                else if (Position.X <= boundaryRect.Left)
                {
                    Position.X = boundaryRect.Left;
                    left2right = 1;
                }
                Position.Y = center_pos.Y + (float)(b * Math.Sin(cur_angle));
            }
            else
            {
                base.Update(gameTime);
            }
        }
    }

    class DuckSinPilotWithDepth : DuckSinPilot
    {
        DepthCosCom d = new DepthCosCom();
        int set_start_z = 0;

        public DuckSinPilotWithDepth(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKSINDEPTH;
        }

        public override void Update(GameTime gameTime)
        {
            if (set_start_z == 0)
            {
                d.setStartZ(depth_seed);
                set_start_z = 1;
            }
            depthpos = d.update_depth(depthpos);
            base.Update(gameTime);
        }
    }

    class DuckLinePilot : DuckPilot
    {
        DepthLinearCom d = new DepthLinearCom();
        int max_curveSteps = Constants.MaxCurveSteps;
        int x_steps = 0;

        public DuckLinePilot(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {
        }

        private void adjustEndPos()
        {
            if ( center_pos.X > boundaryRect.Center.X )
                center_pos.X -= boundaryRect.Width / 2;

            x_steps += group_info.idx * Constants.Groupfactor*2;

            double dx = (double)(boundaryRect.Right - center_pos.X) / max_curveSteps;

            end_pos.X = (float)(center_pos.X + dx * x_steps);
            end_pos.Y = (float)(center_pos.Y + -0.35 * dx * x_steps);
        }

        public override void Initialize(Rectangle space, int seed)
        {
            base.Initialize(space, seed);

            max_curveSteps = (int)(max_curveSteps / group_info.speed_ratio);

            adjustEndPos();
        }

        public override void SetEndPos(Vector2 pos)
        {
            base.SetEndPos(pos);

            adjustEndPos();
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            base.SetSpeedRatio(speedRatio);

            max_curveSteps = (int)(max_curveSteps / group_info.speed_ratio);
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKLINE;
        }

        public override Direction GetHorizationDirection()
        {
            if (InCurve())
            {
                return Direction.RIGHT;
            }
            else
            {
                return base.GetHorizationDirection();
            }
        }

        public override Direction GetZDirection()
        {
            return base.GetZDirection();
        }

        public override void Update(GameTime gameTime)
        {
            if (InCurve())
            {
                depthpos = d.update_depth(depthpos);

                x_steps++;
                double dx = (double)(boundaryRect.Right - center_pos.X) / max_curveSteps;

                Position.X = (float)(center_pos.X + dx * x_steps);
                Position.Y = (float)(center_pos.Y + -0.35 * dx * x_steps);
            }
            else
            {
                base.Update(gameTime);
            }
        }
    }

    class DuckILoveU_IPilot : DuckPilot
    {
        public DuckILoveU_IPilot(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {

        }

        private void adjustEndPos()
        {
            float b = boundaryRect.Height / ( Constants.Ratio);
            float db = b / 4;

            float dy = 0;
            if (group_info.idx % 2 == 0)
            {
                dy = db * group_info.idx / 2;
            }
            else
            {
                dy = db * ( group_info.idx /2 + 1 )* -1;
            }

            end_pos.X = center_pos.X;
            end_pos.Y = center_pos.Y + dy;
        }

        public override void Initialize(Rectangle space, int seed)
        {
            base.Initialize(space, seed);

            adjustEndPos();
        }

        public override void SetEndPos(Vector2 pos)
        {
            base.SetEndPos(pos);

            adjustEndPos();
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            base.SetSpeedRatio(speedRatio);

           // delta_angle = delta_angle * group_info.speed_ratio;
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKILOVEU_I;
        }

        public override Direction GetHorizationDirection()
        {
            if (InCurve())
            {
               
                    return Direction.RIGHT;
             
            }
            else
            {
                return base.GetHorizationDirection();
            }
        }

        public override Direction GetZDirection()
        {
            return base.GetZDirection();
        }

        public override void Update(GameTime gameTime)
        {
            if (InCurve())
            {
                /*
                float b = boundaryRect.Height / (2 * Constants.Ratio);

                
                if (cur_angle > 2 * Constants.Pi)
                    cur_angle = 0;

                Position.X = center_pos.X + (float)(a * Math.Cos(cur_angle));
                Position.Y = center_pos.Y + (float)(b * Math.Sin(cur_angle));
                */
            }
            else
            {
                base.Update(gameTime);
            }
        }
    }

    class DuckILoveU_LPilot : DuckPilot
    {
        double cur_angle = 0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckILoveU_LPilot(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {

        }

        private void adjustEndPos()
        {
            cur_angle += delta_angle * group_info.idx * Constants.Groupfactor * 2;

            if (cur_angle > 2 * Constants.Pi)
                cur_angle = 0;

            float a = boundaryRect.Width / (3 * Constants.Ratio);
            float b = boundaryRect.Height / (2 * Constants.Ratio);

            double sin_v = Math.Sin(cur_angle);
            double cos_v = Math.Cos(cur_angle);
            double cos_2v = Math.Cos(2 * cur_angle);
            double cos_3v = Math.Cos(3 * cur_angle);
            double cos_4v = Math.Cos(4 * cur_angle);
            end_pos.X = center_pos.X + (float)(a * sin_v * sin_v * sin_v);
            end_pos.Y = center_pos.Y - (float)(b* (cos_v - 5 * cos_2v/13 - 2 * cos_3v/13 - cos_4v/13));
        }

        public override void Initialize(Rectangle space, int seed)
        {
            base.Initialize(space, seed);

            adjustEndPos();
        }

        public override void SetEndPos(Vector2 pos)
        {
            base.SetEndPos(pos);

            adjustEndPos();
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            base.SetSpeedRatio(speedRatio);

            delta_angle = delta_angle * group_info.speed_ratio;
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKILOVEU_L;
        }

        public override Direction GetHorizationDirection()
        {
            if (InCurve())
            {
                if (cur_angle >= 0.5 * Constants.Pi && cur_angle < 1.5 * Constants.Pi)
                {
                    return Direction.LEFT;
                }
                else
                {
                    return Direction.RIGHT;
                }
            }
            else
            {
                return base.GetHorizationDirection();
            }
        }

        public override Direction GetZDirection()
        {
            return base.GetZDirection();
        }

        public override void Update(GameTime gameTime)
        {
            if (InCurve())
            {
                cur_angle += delta_angle;
                if (cur_angle > 2 * Constants.Pi)
                    cur_angle = 0;

                float a = boundaryRect.Width / (3 * Constants.Ratio);
                float b = boundaryRect.Height / (2 * Constants.Ratio);

                double sin_v = Math.Sin(cur_angle);
                double cos_v = Math.Cos(cur_angle);
                double cos_2v = Math.Cos(2 * cur_angle);
                double cos_3v = Math.Cos(3 * cur_angle);
                double cos_4v = Math.Cos(4 * cur_angle);

                Position.X = center_pos.X + (float)(a * sin_v * sin_v * sin_v);
                Position.Y = center_pos.Y - (float)(b * (cos_v - 5 * cos_2v / 13 - 2 * cos_3v / 13 - cos_4v / 13));
                //Position.X = center_pos.X + (float)(16 * sin_v * sin_v * sin_v);
                //Position.Y = center_pos.Y + (float)(13 * cos_v - 5 * cos_2v - 2 * cos_3v - cos_4v);
            }
            else
            {
                base.Update(gameTime);
            }
        }
    }

    class DuckILoveU_UPilot : DuckPilot
    {
        int left2right = 1;
        double cur_angle = Constants.Pi;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckILoveU_UPilot(Vector2 pos, pilotGroupInfo pgi)
            : base(pos, pgi)
        {

        }

        private void adjustEndPos()
        {
            if (left2right == 1)
            {
                cur_angle += delta_angle * group_info.idx * Constants.Groupfactor * 2;
            }
            else
            {
                cur_angle -= delta_angle * group_info.idx * Constants.Groupfactor * 2;
            }

            if (cur_angle > 2 * Constants.Pi)
            {
                cur_angle = 2 * Constants.Pi;
                left2right = 0;
            }
            else if (cur_angle < Constants.Pi)
            {
                cur_angle = Constants.Pi;
                left2right = 1;
            }

            float a = boundaryRect.Width / (5 * Constants.Ratio);
            float b = boundaryRect.Height / (Constants.Ratio);

            end_pos.X = center_pos.X + (float)(a * Math.Cos(cur_angle));
            end_pos.Y = center_pos.Y - (float)(b * Math.Sin(cur_angle));
        }

        public override void Initialize(Rectangle space, int seed)
        {
            base.Initialize(space, seed);

            adjustEndPos();
        }

        public override void SetEndPos(Vector2 pos)
        {
            base.SetEndPos(pos);

            adjustEndPos();
        }

        public override void SetSpeedRatio(float speedRatio)
        {
            base.SetSpeedRatio(speedRatio);

            delta_angle = delta_angle * group_info.speed_ratio;
        }

        public override PilotType GetType()
        {
            return PilotType.DUCKILOVEU_U;
        }

        public override Direction GetHorizationDirection()
        {
            if (InCurve())
            {
                if (left2right == 0)
                {
                    return Direction.LEFT;
                }
                else
                {
                    return Direction.RIGHT;
                }
            }
            else
            {
                return base.GetHorizationDirection();
            }
        }

        public override Direction GetZDirection()
        {
            return base.GetZDirection();
        }

        public override void Update(GameTime gameTime)
        {
            if (InCurve())
            {
                /*
                if (left2right == 1)
                {
                    cur_angle += delta_angle;
                }
                else
                {
                    cur_angle -= delta_angle;
                }

                if (cur_angle > 2 * Constants.Pi)
                {
                    cur_angle = 2 * Constants.Pi;
                    left2right = 0;
                }
                else if (cur_angle < Constants.Pi)
                {
                    cur_angle = Constants.Pi;
                    left2right = 1;
                }

                float a = boundaryRect.Width / (5 * Constants.Ratio);
                float b = boundaryRect.Height / (Constants.Ratio);

                Position.X = center_pos.X + (float)(a * Math.Cos(cur_angle));
                Position.Y = center_pos.Y - (float)(b * Math.Sin(cur_angle));
                */
            }
            else
            {
                base.Update(gameTime);
            }
        }
    }


    class DogPilot : BasePilot
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

        override public void Initialize(Rectangle dogspace, int seed)
        {
            boundaryRect = dogspace;
            Position.X = dogspace.Left;
            Position.Y = dogspace.Bottom;
        }

        override public Vector2 GetPosition()
        {
            return Position;
        }


        float depth = 0;
        override public float GetDepth()
        {
            return depth;
        }

        override public PilotType GetType()
        {
            return PilotType.DOGSEEK;
        }

        override public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        override public Direction GetZDirection()
        {
            return Direction.IN;
        }

        override public void Update(GameTime gameTime)
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



    class DogJumpPilot : BasePilot
    {
        // The boundary
        public Rectangle boundaryRect;

        // current position
        Vector2 Position;
        int deltax = 1;
        int deltay = 6;

        int direction = 0; // 0, up, 1, down

        override public void Initialize(Rectangle jumpspace, int seed)
        {
            boundaryRect = jumpspace;
        }

        override public Vector2 GetPosition()
        {
            return Position;
        }

        override public PilotType GetType()
        {
            return PilotType.DOGJUMP;
        }
        public DogJumpPilot(Vector2 pos)
        {
            //boundaryRect = pilot.boundaryRect;
            Position = pos;
        }
        override public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        override public Direction GetZDirection()
        {
            return Direction.IN;
        }
        override public float GetDepth()
        {
            return 0f;
        }

        override public void Update(GameTime gameTime)
        {

            // Update the elapsed time
            Position.X += 2;
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


    class DogShowPilot : BasePilot
    {
        // The boundary
        public Rectangle boundaryRect;

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

        override public void Initialize(Rectangle space, int seed)
        {
            boundaryRect = space;
        }

        override public PilotType GetType()
        {
            return PilotType.DOGSHOW;
        }


        override public Vector2 GetPosition()
        {
            return Position;
        }

        override public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        override public Direction GetZDirection()
        {
            return Direction.IN;
        }

        override public float GetDepth()
        {
            return 0f;
        }

        override public void Update(GameTime gameTime)
        {

            // Update the elapsed time
            //Position.X += deltax * factorx;
            if (direction == 0)
            {
                Position.Y -= deltay / 2;
                if (Position.Y <= boundaryRect.Top + 56)
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

    class CloudPilot : BasePilot
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

        override public PilotType GetType()
        {
            return PilotType.CLOUD;
        }
        override public void Initialize(Rectangle boundary, int seed)
        {
            radom = new Random(seed);
            boundaryRect = boundary;

            // radom a intial position
            Position.X = boundary.Width / 2;
            Position.Y = 100;

        }
        override public Direction GetHorizationDirection()
        {
            return Direction.RIGHT;
        }
        override public Direction GetZDirection()
        {
            return Direction.IN;
        }

        override public Vector2 GetPosition()
        {
            return Position;
        }

        override public float GetDepth()
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
        override public void Update(GameTime gameTime)
        {
            // Update the elapsed time
            if (Position.X >= (boundaryRect.Right + 100))
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
