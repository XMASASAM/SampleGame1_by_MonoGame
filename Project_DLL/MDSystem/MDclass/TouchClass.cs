using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Linq;
using MDclass.CollisionClass;
using MDclass.ScreenClass;
namespace MDclass.TouchClass
{

    /// <summary>
    /// タッチ情報を管理する
    /// </summary>
    static public class TouchManager
    {
        //タッチ回数
        static int touchnum = 0;
        
        //タッチ情報
        static Dictionary<int, TouchInfo> Tdict = new Dictionary<int, TouchInfo>();
        //ただいま設定されているタッチ画面
        static TouchView panel;
        //前回タッチされたか
        static bool preFlag = false;

        /// <summary>
        /// タッチ画面を設定する
        /// </summary>
        /// <param name="view">タッチ画面</param>
        static public void SetView(TouchView view)
        {
            
            if (view == null) return;
            //既に設定されていた場合
            if (panel != null) panel.active = false;

            //設定
            panel = view;
            panel.active = true;
        }

        static public void Update(GameTime gameTime,bool mobile)
        {
            //TouchList用
            touchs = null;

            //実行環境がモバイルの場合
            if (mobile)
            {
                //タッチ情報を取得
                TouchCollection touches = TouchPanel.GetState();
                //初期化
                Dictionary<int, TouchInfo> temp = new Dictionary<int, TouchInfo>();

                foreach (TouchLocation i in touches)
                {
                    //新しくタッチした場合
                    if (!Tdict.ContainsKey(i.Id))
                        //タッチ情報を追加する
                        Tdict.Add(i.Id, SetT(new TouchInfo(i.Position, i.Id)));

                    //IDから情報を取得
                    TouchInfo current = Tdict[i.Id];
                    //タッチ情報を更新
                    current.Update(i.Position, gameTime);
                    //存在が確認されたタッチ情報を追加
                    temp.Add(i.Id, current);
                }

                //存在が確認されなかったタッチ情報を削除
                foreach (TouchInfo i in Tdict.Values.Except(temp.Values))
                    i.Delete();

                Tdict = temp;
            }
            else
            {
                //実行環境がパソコンの場合

                //マウスの情報
                MouseState mouseState;
                mouseState = Mouse.GetState();

                //左クリックしたか
                bool touchFlag = mouseState.LeftButton == ButtonState.Pressed;

                //クリックした場合
                if (touchFlag)
                {
                    //前回クリックしなかった場合
                    if (!preFlag)
                    {

                        touchnum++;
                        Tdict.Add(touchnum, SetT(new TouchInfo(mouseState.Position.ToVector2(), touchnum)));

                        preFlag = true;
                    }
                }
                else
                {
                    //クリックしなかった場合
                    if (Tdict.Count > 0)
                    {
                        //タッチ情報を削除
                        Tdict[touchnum].Delete();
                        Tdict.Remove(touchnum);
                        preFlag = false;
                    }
                }

                //タッチ情報が存在している場合更新する
                if (Tdict.Count > 0) Tdict[touchnum].Update(mouseState.Position.ToVector2(), gameTime);


            }
            //タッチする画面を更新
            if(panel!=null) panel.Update();
        }



        //タッチ情報を精査
        static TouchInfo SetT(TouchInfo info)
        {
            if (panel == null) return info;
            //各タッチ範囲
            foreach(TouchBoard i in panel.Content)
            {
                //タッチ情報がタッチ範囲内か
                if (info.Within(i.range))
                {
                    //情報を追加
                    i.Add(info);
                    //透過フラグがfalseの場合
                    if (!i.Transparent) break;
                }
            }
            
            return info;
        }

        //タッチ情報
        static TouchInfo[] touchs;
        static public TouchInfo[] TouchList
        {
            get
            {
                if (touchs == null) touchs = Tdict.Values.ToArray<TouchInfo>();
                return touchs;
            }
        }

        /// <summary>
        /// 現在のタッチ画面
        /// </summary>
        static public TouchView CurrentView { get { return panel; } }


    }

    /// <summary>
    /// タッチの画面(レイアウト)
    /// </summary>
    public class TouchView
    {
        //現在使用中か(TouchManagerで設定されているか)
        internal bool active = false;
        //z軸
        internal int BaseDepth = 0;
        //各当たり判定の範囲
        internal readonly List<TouchBoard> Content;

        /// <summary>
        /// タッチ画面を設定
        /// </summary>
        /// <param name="board">タッチ範囲たち</param>
        public TouchView(params TouchBoard[] board)
        {
            if (board != null) Content = new List<TouchBoard>(board); 
            else Content = new List<TouchBoard>();

        }
        



        /// <summary>
        /// タッチ範囲を追加する
        /// </summary>
        /// <param name="board">タッチ範囲</param>
        /// <param name="depth">z軸</param>
        public void Add(TouchBoard board,int depth=-1)
        {
            if (board == null) return;
            Content.Add(board);
            board.pre = this;
            board.Depth = depth < 0 ? BaseDepth++ : depth;
            Content.Sort((a, b) => a.Depth - b.Depth);
        }

        /// <summary>
        /// タッチ範囲を切り離す
        /// </summary>
        /// <param name="board">タッチ範囲</param>
        public void Remove(TouchBoard board)
        {
            Content.Remove(board);
            board.pre = null;
        }

        
        internal void Update()
        {
            //全てのタッチ範囲の情報を更新する
            foreach (TouchBoard i in Content) i.Update();
        }

        public bool isActive { get { return active; } }
        public List<TouchBoard> BoardList { get { return Content; } }


    }

    /// <summary>
    /// タッチ範囲を設定するクラス
    /// </summary>
    public class TouchBoard
    {
        //所属しているタッチ画面
        internal TouchView pre = null;
        //範囲内でタッチされたら追加する配列
        internal List<TouchSatellite> touches = new List<TouchSatellite>();
        //座標
        internal Vector2 point;
        //タッチ範囲
        internal readonly CollisionShape[] range;
        //透過するか
        public readonly bool Transparent;
        //z軸
        public int Depth;
        //タッチの状態を表現するフラグ
        bool onPress = false, press = false, first = false, middle = false, final = false;

        /// <summary>
        /// タッチ範囲を設定する
        /// 透過をtrueにすると、もしタッチ範囲が重なっている場合、後ろに隠れているタッチ範囲も
        /// 認識される。falseにすると認識されない。
        /// </summary>
        /// <param name="point">座標</param>
        /// <param name="range">タッチ範囲</param>
        /// <param name="transparent">透過</param>
        public TouchBoard(Vector2 point,CollisionShape range,bool transparent=false)
        {
            this.range = new CollisionShape[] { range };
            this.Transparent = transparent;
            this.point = point;
            this.range[0].Point = point;

        }
        /// <summary>
        /// タッチ範囲を設定する
        /// 透過をtrueにすると、もしタッチ範囲が重なっている場合、後ろに隠れているタッチ範囲も
        /// 認識される。falseにすると認識されない。
        /// </summary>
        /// <param name="point">座標</param>
        /// <param name="range">複数のタッチ範囲</param>
        /// <param name="transparent">透過</param>
        public TouchBoard(Vector2 point,CollisionShape[] range, bool transparent = false)
        {
            this.range = range;
            this.point = point;
            this.Transparent = transparent;

            foreach (CollisionShape i in this.range)
            {
                i.Point = this.point;
                i.point = this.point;
            }
        }

        internal void Update()
        {
            //前回タッチされていなかったかのフラグ
            bool oldnull = !press;
            //範囲内でタッチされているというフラグの初期化
            onPress = false;
            //ループのため値を複製
            List<TouchSatellite> temp = new List<TouchSatellite>(touches);


            foreach(TouchSatellite i in temp)
            {
                //範囲内でタッチしたタッチの情報を更新
                i.Update();
                //範囲内でタッチされているというフラグ
                onPress |= i.OnPressFlag;
                //タッチが離された場合
                if (i.FinalFlag)
                {
                    //情報を切り離す
                    touches.Remove(i);
                }
            }

            

            //タッチ情報が1つ以上あるとこの範囲はタッチされている
            press = touches.Count > 0;

            //タッチし始めた
            first =  press && oldnull;
            //中間
            middle = !first && press;
            //タッチが離された
            final =  !(press ||oldnull);

        }
        //タッチ情報を追加する
        internal void Add(TouchInfo info)
        {
            touches.Add(new TouchSatellite(info, this));
            
        }

        /// <summary>
        /// 範囲内でタッチされたのタッチの情報
        /// </summary>
        public List<TouchSatellite> TouchList { get { return touches; } }
        /// <summary>
        /// タッチされているか
        /// </summary>
        public bool PressFlag { get { return press; } }
        /// <summary>
        /// 範囲内でタッチされているか
        /// </summary>
        public bool OnPressFlag { get { return onPress; } }
        /// <summary>
        /// タッチし始めた
        /// </summary>
        public bool FirstFlag { get { return first; } }
        /// <summary>
        /// タッチしている最中
        /// </summary>
        public bool MiddlesFlag { get { return middle; } }
        /// <summary>
        /// タッチが離された
        /// </summary>
        public bool FinalFlag { get { return final; } }
        /// <summary>
        /// このタッチ範囲は現在使用中か
        /// </summary>
        public bool isActive
        {
            get
            {
                if (pre == null) return false;
                return pre.isActive;
            }
        }
        /// <summary>
        /// 画面上の座標
        /// </summary>
        public Vector2 Point
        {
            get { return point; }
            set
            {
                point.X = value.X;
                point.Y = value.Y;
                if (range.Length == 1)range[0].Point = point;
                else foreach (CollisionShape i in range)i.Point = point;

            }
        }
        /// <summary>
        /// 所属しているタッチ画面(レイアウト)
        /// </summary>
        public TouchView Parent { get { return pre; } }
       
        
    }


    /// <summary>
    ///　範囲内でタッチした情報
    /// </summary>
    public class TouchSatellite
    {
        //タッチ情報
        readonly TouchInfo touch;
        //該当範囲
        readonly TouchBoard pre;
        //現在は範囲内でタッチしているか　, ひとつ前の状態
        bool onRange,oldOnRange;

        /// <summary>
        /// 範囲内でタッチした情報
        /// </summary>
        /// <param name="touch">該当のタッチ情報</param>
        /// <param name="pre">該当のタッチ範囲</param>
        internal TouchSatellite(TouchInfo touch,TouchBoard pre)
        {
            this.touch = touch;
            this.pre = pre;
        }


        internal void Update()
        {
            //1つ前の状態を保持
            oldOnRange = onRange;
            //現在範囲内にいるのか
            onRange = touch.Within(pre.range);
        }

        /// <summary>
        /// タッチ情報を取得する
        /// </summary>
        public TouchInfo ToTouchInfo { get { return touch; } }
        /// <summary>
        /// タッチし始めたか
        /// </summary>
        public bool FirstFlag { get { return touch.FirstFlag; } }
        /// <summary>
        /// タッチしている最中か
        /// </summary>
        public bool MiddleFlag { get { return touch.MiddleFlag; } }
        /// <summary>
        /// タッチが終了したか
        /// </summary>
        public bool FinalFlag { get { return touch.FinalFlag; } }
        /// <summary>
        /// 現在範囲内でタッチしているか
        /// </summary>
        public bool OnPressFlag { get { return onRange; } }
        /// <summary>
        /// 1つ前では範囲内でタッチしていたか
        /// </summary>
        public bool OldOnPressFlag { get { return oldOnRange; } }
    }
        

    /// <summary>
    /// タッチ情報
    /// </summary>
    public class TouchInfo
    {
        //識別番号 , 生存時間
        int id, lifetime=0;
        //タッチを開始した座標 , 現在の座標 ,　変位 
        Vector2 center, point, delta =Vector2.Zero;
        //タッチの当たり判定
        internal CollisionDot dot;
        //タッチの状態を表現するフラグ
        bool first=true, final=false,middle=false;

        /// <summary>
        /// タッチ情報を設定
        /// </summary>
        /// <param name="point">タッチした座標</param>
        /// <param name="id">識別番号</param>
        internal TouchInfo(Vector2 point,int id)
        {
            
            this.id = id;
            dot = new CollisionDot(CollisionName.TouchDot, Vector2.Zero);
            Vector2 point2 = Screen.ExportVector(point);
            center = point2;
            this.point = point2;

            dot.Point = point2;
        }


        internal void Update(Vector2 point,GameTime gameTime)
        {
            //生存時間が0の場合タッチし始めた
            first = lifetime == 0;
            //そうでなければタッチしている最中
            middle = !first;
            //タッチされた座標をこのゲームの座標へ変換する
            Vector2 point2 = Screen.ExportVector(point);

            float sX = point2.X;
            float sY = point2.Y;
            //生存時間を更新する。最大値までいったら変化しなくなる
            lifetime = Math.Min(1 << 30, lifetime + gameTime.ElapsedGameTime.Milliseconds);

            //タッチ座標の変位を求める
            delta.X = sX - this.point.X;
            delta.Y = sY - this.point.Y;

            //現在のタッチ座標として設定
            this.point.X = sX;
            this.point.Y = sY;
            //当たり判定の座標も設定
            dot.Point = this.point;
           

        }
        /// <summary>
        /// タッチ情報を削除
        /// </summary>
        internal void Delete()
        {
            //タッチは離された
            final = true;
            //タッチしている最中ではなくなった
            middle = false;
        }

        /// <summary>
        /// タッチは指定した当たり判定内か
        /// </summary>
        /// <param name="range">当たり判定</param>
        /// <returns></returns>
        internal bool Within(CollisionShape[] range)
        {
            //1つでも当たっていたらtrueを返す
            foreach (CollisionShape i in range) if (Collision.Check(dot, i)) return true;
            return false;
        }

        /// <summary>
        /// 識別番号
        /// </summary>
        public int ID { get { return id; } }
        /// <summary>
        /// 生存時間
        /// </summary>
        public int LifeTime { get { return lifetime; } }
        /// <summary>
        /// タッチ開始時の座標
        /// </summary>
        public Vector2 Center { get { return center; } }
        /// <summary>
        /// 現在の座標
        /// </summary>
        public Vector2 Point { get { return point; } }
        /// <summary>
        /// タッチ開始時からの変位
        /// </summary>
        public Vector2 Delta { get { return delta; } }
        /// <summary>
        /// タッチ開始時か
        /// </summary>
        public bool FirstFlag { get { return first; } }
        /// <summary>
        /// タッチしている最中か
        /// </summary>
        public bool MiddleFlag { get { return middle; } }
        /// <summary>
        /// タッチは離されたか
        /// </summary>
        public bool FinalFlag { get { return final; } }

    }

}
