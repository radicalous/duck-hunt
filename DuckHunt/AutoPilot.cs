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
        DUCKLINE, DUCKREN, DUCKILOVEU,
        DOGSEEK, DOGJUMP, DOGSHOW,
        CLOUD,
    };

    abstract class AiPilot
    {
        public abstract void Initialize(Rectangle boundary, int seed);
        public abstract void SetStartPos(Vector2 pos);
        public abstract void SetEndPos(Vector2 pos);
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
    }


    struct pilotGroupInfo
    {
        public int idx;
        public Point endpoint;
    }

    class PilotManager
    {
        Dictionary<string, pilotGroupInfo> duckEightPilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckCirclePilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckEllipsePilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckSinPilotGroup = new Dictionary<string, pilotGroupInfo>();
        Dictionary<string, pilotGroupInfo> duckLinePilotGroup = new Dictionary<string, pilotGroupInfo>();

        static int count = 0;
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
                            pilot = new DuckEightPilot(pos, pgi.idx, pgi.endpoint);

                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckEightPilotGroup.Add(clustername, pgi);
                            pilot = new DuckEightPilot(pos, pgi.idx, pgi.endpoint);

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
                            pilot = new DuckEightPilotWithDepth(pos, pgi.idx, pgi.endpoint);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckEightPilotGroup.Add(clustername, pgi);
                            pilot = new DuckEightPilotWithDepth(pos, pgi.idx, pgi.endpoint);
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
                            pilot = new DuckCirclePilot(pos, pgi.idx, pgi.endpoint);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckCirclePilotGroup.Add(clustername, pgi);
                            pilot = new DuckCirclePilot(pos, pgi.idx, pgi.endpoint);
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
                            pilot = new DuckCirclePilotWithDepth(pos, pgi.idx, pgi.endpoint);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckCirclePilotGroup.Add(clustername, pgi);
                            pilot = new DuckCirclePilotWithDepth(pos, pgi.idx, pgi.endpoint);
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
                            pilot = new DuckEllipsePilot(pos, pgi.idx, pgi.endpoint);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckEllipsePilotGroup.Add(clustername, pgi);
                            pilot = new DuckEllipsePilot(pos, pgi.idx, pgi.endpoint);
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
                            pilot = new DuckEllipsePilotWithDepth(pos, pgi.idx, pgi.endpoint);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckEllipsePilotGroup.Add(clustername, pgi);
                            pilot = new DuckEllipsePilotWithDepth(pos, pgi.idx, pgi.endpoint);
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
                            pilot = new DuckSinPilot(pos, pgi.idx, pgi.endpoint);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckSinPilotGroup.Add(clustername, pgi);
                            pilot = new DuckSinPilot(pos, pgi.idx, pgi.endpoint);
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
                            pilot = new DuckSinPilotWithDepth(pos, pgi.idx, pgi.endpoint);
                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckSinPilotGroup.Add(clustername, pgi);
                            pilot = new DuckSinPilotWithDepth(pos, pgi.idx, pgi.endpoint);
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
                            pilot = new DuckLinePilot(pos, pgi.idx, pgi.endpoint);

                        }
                        else
                        {
                            pilotGroupInfo pgi = GenPGI();

                            duckLinePilotGroup.Add(clustername, pgi);
                            pilot = new DuckLinePilot(pos, pgi.idx, pgi.endpoint);

                        }

                    }
                    break;
                case PilotType.DUCKREN:
                    {
                        //not done yet
                    }
                    break;
                case PilotType.DUCKILOVEU:
                    {
                        //not done yet
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
        public const int MaxLineSteps = 80;
        public const int MaxCurveSteps = 400;
    }

    class DuckPilot : BasePilot
    {
        protected Rectangle boundaryRect = new Rectangle();

        protected Vector2 start_pos; //random start position
        protected Vector2 end_pos; //different end position
        protected Point endpoint; //record the percent of the end position according the boundary

        protected int lineStep;
        protected int max_lineSteps;

        // current position
        protected Vector2 Position;
        protected float depthpos = 0;
        protected int depth_seed = 0;

        public DuckPilot(Vector2 pos, int idx, Point ep)
        {
            Position = pos;

            endpoint = ep;

            lineStep = 0;
            max_lineSteps = Constants.MaxLineSteps + idx * 10;
        }

        override public void Initialize(Rectangle space, int seed)
        {
            boundaryRect = space;
            depth_seed = seed;

            Random rdm = new Random(seed);

            int r = Math.Min(boundaryRect.Height, boundaryRect.Width);
            r /= (Constants.Ratio * 2);

            end_pos.X = boundaryRect.Center.X + (endpoint.X) * r / 50;
            end_pos.Y = boundaryRect.Center.Y + (endpoint.Y) * r / 50;

            start_pos.X = rdm.Next(boundaryRect.Left, boundaryRect.Right);
            start_pos.Y = boundaryRect.Bottom;

            Position = start_pos;
        }

        override public void SetStartPos(Vector2 pos)
        {
            start_pos = pos;
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
            if (depthpos < 20)
                depthpos = 20;
            else if (depthpos > 80)
                depthpos = 80;

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
            else if (depthpos > 80)
                depthpos = 80;

            return depthpos;
        }
    }

    class DuckEightPilot : DuckPilot
    {
        double cur_angle = 0.0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckEightPilot(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
        {
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
                    cur_angle = 0.0;

                float a = Math.Min(boundaryRect.Width, boundaryRect.Height);
                a /= Constants.Ratio;

                Position.X = end_pos.X + (float)(a * Math.Sin(cur_angle));
                Position.Y = end_pos.Y + (float)(a * Math.Cos(cur_angle) * Math.Sin(cur_angle));
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

        public DuckEightPilotWithDepth(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
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
        static int max_hor_steps = 20;
        int hor_steps = 0;
        double cur_angle = 0.0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckCirclePilot(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
        {

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
            float r = Math.Min(boundaryRect.Width, boundaryRect.Height);
            r /= (3 * Constants.Ratio / 2);

            if (InCurve())
            {
                if (hor_steps <= max_hor_steps)
                {
                    Position.X = end_pos.X + hor_steps * r / max_hor_steps;

                    hor_steps++;
                }
                else
                {
                    cur_angle += delta_angle;
                    if (cur_angle > 2 * Constants.Pi)
                        cur_angle = 0.0;

                    Position.X = end_pos.X + (float)(r * Math.Cos(cur_angle));
                    Position.Y = end_pos.Y + (float)(r * Math.Sin(cur_angle));
                }
            }
            else
                base.Update(gameTime);
        }
    }

    class DuckCirclePilotWithDepth : DuckCirclePilot
    {
        DepthCosCom d = new DepthCosCom();
        int set_start_z = 0;

        public DuckCirclePilotWithDepth(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
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
        static int max_hor_steps = 20;
        int hor_steps = 0;
        double cur_angle = 0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckEllipsePilot(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
        {

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

                if (hor_steps <= max_hor_steps)
                {
                    Position.X = end_pos.X + hor_steps * a / max_hor_steps;
                    hor_steps++;
                }
                else
                {
                    cur_angle += delta_angle;
                    if (cur_angle > 2 * Constants.Pi)
                        cur_angle = 0;

                    Position.X = end_pos.X + (float)(a * Math.Cos(cur_angle));
                    Position.Y = end_pos.Y + (float)(b * Math.Sin(cur_angle));
                }
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

        public DuckEllipsePilotWithDepth(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
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
        double cur_angle = 0;
        double delta_angle = 2 * Constants.Pi / Constants.MaxCurveSteps;

        public DuckSinPilot(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
        {

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

                Position.X = end_pos.X + (float)hor_steps * a / Constants.MaxCurveSteps;
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
                Position.Y = end_pos.Y + (float)(b * Math.Sin(cur_angle));
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

        public DuckSinPilotWithDepth(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
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
        int x_steps = 0;

        public DuckLinePilot(Vector2 pos, int idx, Point ep)
            : base(pos, idx, ep)
        {
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
                double dx = (double)(boundaryRect.Right - end_pos.X) / Constants.MaxCurveSteps;

                Position.X = (float)(end_pos.X + dx * x_steps);
                Position.Y = (float)(end_pos.Y + -0.35 * dx * x_steps);
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
