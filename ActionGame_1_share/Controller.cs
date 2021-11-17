using System;
using System.Collections.Generic;
using System.Text;
using MDclass.TouchClass;
using Microsoft.Xna.Framework;
using MDclass.CollisionClass;
using MDclass.ImageAndSprite;
using MDclass.VariationClass;
namespace ActionGame_1_share
{
    public class Controller
    {
        TouchBoard board;

        readonly float scope; //= 5*5;
        readonly float maxdis; //= 25 * 25;
        int time = 0;
        bool active = false;
        TouchInfo currentTouch = null;
        byte direction = 0;
        Vector2 delta,deltaDis;
        ControllerSprite controllerSprite;
        /// <summary>
        /// 画面いっぱいの操作用のタッチボードを作成する
        /// </summary>
        /// <param name="view">コントローラーを有効にしたいView</param>
        /// <param name="mindis">コントローラーが認識し始める変位</param>
        /// <param name="maxdis">コントローラーが対応する最大の変位</param>
        /// <param name="depth">タッチボードの深さ</param>
        public Controller(TouchView view,int mindis,int maxdis,int depth=-1)
        {
            board = new TouchBoard(Vector2.Zero, 
                new CollisionRectangle(CollisionName.None, Vector2.Zero,Vector2.Zero, MagicNumber.ScreenWidth, MagicNumber.ScreenHeight));
            //TouchManager.SetView(new TouchView(board));
            view.Add(board,depth);
            //TouchManager.NowView.Add(board);
            controllerSprite = new ControllerSprite(this);

            scope = mindis * mindis;
            this.maxdis = maxdis * maxdis;

        }

        public void ChangeView(TouchView view,int depth=-1)
        {
            var oldView = board.Parent;
            if (oldView != null)view.Remove(board);
            else if (oldView == view) return;
            view.Add(board, depth);
        }


        int oldTime=0,oldOldTime=1000;
        bool doubleTouch=false;
        public void Update(GameTime gameTime)
        {
            doubleTouch = false;
            foreach(TouchSatellite sa in board.TouchList)
                if (sa.FirstFlag)new ContWave(sa.ToTouchInfo.Point);
                


            if (currentTouch != null && currentTouch.FinalFlag)
            {
                active = false;
                currentTouch = null;
                controllerSprite.close();

                if (oldOldTime<200&&oldTime < 200) doubleTouch = true;

                oldTime = 0;
            }

            if (active) time += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
            {
                oldTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                foreach (TouchSatellite satellite in board.TouchList)
                    if (satellite.FirstFlag)
                    {
                        currentTouch = satellite.ToTouchInfo;
                        active = true;
                        oldOldTime = time;
                        time = 0;
                        break;
                    }
            }

            direction = 0;
            delta = Vector2.Zero;
            deltaDis = Vector2.Zero;
            

            if (active)
            {
                Vector2 sub = currentTouch.Point - currentTouch.Center;


                float dis = sub.X * sub.X + sub.Y * sub.Y;


                if (dis > scope)
                {

                    if (controllerSprite.isHide) controllerSprite.open();

                    if (sub.Y < 0) direction |= 0b0001;
                    if (sub.Y > 0) direction |= 0b0010;
                    if (sub.X < 0) direction |= 0b0100;
                    if (sub.X > 0) direction |= 0b1000;

                    delta = sub;
                    deltaDis = sub;
                    if (dis > maxdis)
                    {
                        float div = (float)Math.Sqrt(maxdis / dis);

                        deltaDis.X = delta.X * div;
                        deltaDis.Y = delta.Y * div;

                    }
                }

            }
            
        }
        /// <summary>
        /// 4方向でどこを向いているのかを方向で出力する
        /// </summary>
        public Orientation GetOritentation()
        {
            if (delta == Vector2.Zero) return Orientation.None;

            Orientation ori;

            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                if (delta.X > 0) ori = Orientation.East;
                else ori = Orientation.West;
            }
            else
            {
                if (delta.Y > 0) ori = Orientation.South;
                else ori = Orientation.North;
            }

            return ori;
        }

        /// <summary>
        /// 4方向でどこを向いているのかをbitで出力する。\n
        /// bit:
        /// (0,北)
        /// (1,南)
        /// (2,西)
        /// (3,東)
        /// </summary>
        public byte Direction { get { return direction; } }
        /// <summary>
        /// タッチボードを返す
        /// </summary>
        public TouchBoard Boad { get { return board; } }

        /// <summary>
        /// タッチされた中心からの変位座標
        /// mindis以上の時に返す
        /// </summary>
        public Vector2 Delta { get { return delta; } }

        /// <summary>
        /// タッチされた中心からの変位座標
        ///  mindis以上deltaがmaxdis以下になるように整えた値を返す
        /// </summary>
        public Vector2 DeltaDistance { get { return deltaDis; } }

        /// <summary>
        /// true:コントローラーを表示
        /// false:非表示
        /// </summary>
        public bool isActive { get { return active; } }


        public int GetTime { get { return time; } }

        public bool GetDoubleTouch { get { return doubleTouch; } }





        /// <summary>
        /// 以下タッチ時のアニメーション
        /// </summary>

        class ContWave:Sprite
        {
            public ContWave(Vector2 point) : base(ImageName.Controller_Wave)
            {
                Scale = Vector2.Zero;
                Point = point;
                Anim(AnimName.Scale, 1, -400, new Vector2(1f));
                Anim(AnimName.Color, 1, 200, Color.White, -200, Color.Transparent);
                Origin = new Vector2(50, 50);
                Depth = 0f;

                //var sub = new Sprite(ImageName.Controller_Wave);
                //sub.Link(this);
                //  sub.Scale = Vector2.Zero;
                // sub.Point = point;
                //  sub.Anim(AnimName.Scale, 1, -400, new Vector2(1f+0.2f));
                // sub.Anim(AnimName.Color, 1, 200, Color.Black, -200, Color.Transparent);
                //sub.Color = new Color(1,1,100);//Color.Black;
                //sub.Origin = new Vector2(50, 50);
                //sub.Depth = 0.01f;

            }

            public override void Update(GameTime gameTime)
            {
                if (GetAnim(AnimName.Scale).isCompletion)this.Delete();
                
                
                base.Update(gameTime);
            }
        }


        class ControllerSprite:Sprite
        {

            readonly int quantity = 10;
            readonly float quantDiv;
            readonly Sprite aniS;
            readonly Controller controller;
            public ControllerSprite(Controller controller) : base(ImageName.Controller_Circle)
            {
                this.controller = controller;
                quantDiv = 1 /(float) quantity;
                this.Origin = new Vector2(50);
                this.Color = new Color(200, 200, 200,100);

                for(int i = 0; i < quantity; i++)
                {
                    Sprite sub = new Sprite(ImageName.Controller_Circle);
                    sub.Link(this);
                    sub.Origin = new Vector2(50);
                    sub.isHide = true;
                    sub.Scale = new Vector2(0.1f - 0.005f*i);
                    sub.Color = new Color(100,100,100,100);
                    
                    //chiled[i] = sub;
                }


                _children[quantity - 1].Color = new Color(200, 200, 200, 100);
                _children[quantity - 1].Scale = new Vector2(0.16f);


                aniS = new Sprite(ImageName.Controller_Wave);
                aniS.isHide = true;
                aniS.Origin = new Vector2(50);
                aniS.Depth = 0f;
                this.Scale = new Vector2(0.2f);
                this.isHide = true;
                Depth = 0f;
            }
            public void open()
            {
                isHide = false;
                setRow();
                aniS.Scale = Vector2.Zero;

                aniS.Anim(AnimName.Scale, 0, -1500, new Vector2(1f), 0, Vector2.Zero);
                aniS.Anim(AnimName.Color, 0, 750, Color.White, -750, Color.Transparent,0,Color.White);
            }

            public void close()
            {
                isHide = true;
                aniS.AnimDelate(AnimName.ALL);
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);

                if (!isHide)
                {

                    setRow();
                    
                }

            }


            public void setRow()
            {
                if ( controller. currentTouch == null) return;

                this.Point = controller. currentTouch.Center;

                Vector2 sub = controller.Delta;

                float wDiv = sub.X *quantDiv;
                float hDiv = sub.Y *quantDiv;

                for (int i = 1; i <= _children.Count; i++) _children[i-1].Point = new Vector2(wDiv * i, hDiv * i);
            
                aniS.Point = controller.currentTouch.Point;
            }






            public override bool isHide 
            {
                get
                {
                    return _ishide;
                }
                set
                {
                    if (_ishide == value) return;

                    _ishide = value;

                    foreach(Sprite i in _children) i.isHide = value;

                    aniS.isHide = value;
                }
            }




        }



    }
}
