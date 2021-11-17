using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
namespace MDclass.CollisionClass
{

    #region Shapes
    
    /// <summary>
    /// 全ての当たり判定の元となるクラス
    /// </summary>
    public class CollisionShape
    {
        /// <summary>
        /// 画面上の座標
        /// </summary>
        internal protected readonly Vector2[] vertex; 
        /// <summary>
        /// 中心座標からの座標
        /// </summary>
        internal protected readonly Vector2[] coordinate;
        /// <summary>
        /// 辺(内積・外積用)
        /// </summary>
        internal protected readonly Vector2[] line;
        /// <summary>
        /// 何の当たり判定なのかを判定するためのラベル
        /// </summary>
        internal protected readonly CollisionName label;
        /// <summary>
        /// 4分木空間分割用
        /// </summary>
        internal CollisionForm Form = null;
        //internal List<CollisionShape> hit;
        internal protected Vector2 point;
        /// <summary>
        /// 最大の座標
        /// </summary>
        internal protected Vector2 Max;
        /// <summary>
        /// 最小の座標
        /// </summary>
        internal protected Vector2 Min ;
        /// <summary>
        /// 中点からの最大座標
        /// </summary>
        internal protected Vector2 MaxO = new Vector2(float.MinValue, float.MinValue);//中心座標からの最小座標
        /// <summary>
        /// 中点からの最小座標
        /// </summary>
        internal protected Vector2 MinO = new Vector2(float.MaxValue,float.MaxValue);//中心座標からの最大座標
        internal int name=1;

        /// <summary>
        /// 原点
        /// </summary>
        internal protected Vector2 origin;




        /// <param name="label">何の当たり判定なのか</param>
        /// <param name="origin">原点</param>
        /// <param name="vectors">(0,0)を原点としたときの座標</param>
        internal CollisionShape(CollisionName label,Vector2 origin, Vector2[] vectors)
        {
            //引数からのデータを代入
            this.origin = origin;
            this.point = Vector2.Zero;
            this.label = label;

            //代入のための準備
            vertex = new Vector2[vectors.Length];
            coordinate = new Vector2[vectors.Length];
            line = new Vector2[vectors.Length];

            

            for (int i = 0; i < vectors.Length; i++)
            {
                //originを原点としたときの各座標を取得
                coordinate[i] = vectors[i] - origin;

                //辺を取得
                line[i] = i != coordinate.Length - 1 ? vectors[i+1] - vectors[i] : vectors[0] - vectors[i];

                //座標(0,0)にいるときの最大最小の座標を取得
                MaxO.X = Math.Max(MaxO.X, coordinate[i].X);
                MaxO.Y = Math.Max(MaxO.Y, coordinate[i].Y);
                MinO.X = Math.Min(MinO.X, coordinate[i].X);
                MinO.Y = Math.Min(MinO.Y, coordinate[i].Y);

                //座標がpointにいるときの最大最小の座標を取得
                Max = MaxO + point;
                Min = MinO + point;


            }

        }

        /// <summary>
        /// 座標
        /// </summary>
        public virtual Vector2 Point
        {
            get
            {
                return point;
            }

            set
            {
                //4分木空間分割の設定をしている場合
                if (Form != null) Form.Remove();//.field.Remove(Form);

                //座標を代入
                point.X = value.X;
                point.Y = value.Y;
                
                //最小最大の座標を取得
                Vector2.Add(ref MaxO ,ref point,out Max);
                Vector2.Add(ref MinO, ref point, out Min);
            }
        }


        /// <summary>
        /// 4分木空間分割による当たり判定の結果。当たっていたらこのリストに挿入される。
        /// </summary>
        public List<CollisionShape> HitList { 
            get {
                if (Form == null) return null;
                return Form.HitList; 
            } 
        }

        /// <summary>
        /// 何の当たり判定かを返す
        /// </summary>
        /// <returns></returns>
        public CollisionName GetLabel()
        {
            return label;
        }

        /// <summary>
        /// 最小の座標(左上の座標)を返す
        /// </summary>
        public Vector2 GetMin
        {
            get { return Min; }
        }

        /// <summary>
        /// 最大の座標(右下の座標)を返す
        /// </summary>
        public Vector2 GetMax
        {
            get { return Max; }
        }



    }

    /// <summary>
    /// 多角形の当たり判定
    /// </summary>
    public class CollisionPolygon : CollisionShape
    {

        /// <summary>
        /// 多角形の当たり判定
        /// </summary>
        /// <param name="label">何の当たり判定か</param>
        /// <param name="origin">原点</param>
        /// <param name="vectors">(0,0)を原点とした時の座標</param>
        public CollisionPolygon(CollisionName label, Vector2 origin, params Vector2[] vectors) :
            base(label,origin, vectors)
        { }

        /// <summary>
        /// 座標
        /// </summary>
        public override Vector2 Point
        {
            get { return base.Point; } set
            {
                base.Point = value;
                //各頂点を移動させる
                for(int i=0;i<vertex.Length;i++)vertex[i] = Vector2.Add(coordinate[i], value);
                //4分木空間分割の設定をしている場合
                if (Form!=null) Form.field.Add(Form);
            }
        }

    }

    /// <summary>
    /// 長方形の当たり判定
    /// </summary>
    public class CollisionRectangle:CollisionShape
    {
        public int width, height;

        /// <summary>
        /// 長方形の当たり判定
        /// </summary>
        /// <param name="label">何の当たり判定か</param>
        /// <param name="origin">原点</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        public CollisionRectangle(CollisionName label, Vector2 origin,Vector2 point, int width,int height) :
            base(label,origin,
                new Vector2[] {point,new Vector2(width,0) + point,new Vector2(width,height) + point,new Vector2(0,height) + point }
                )
        {
            this.width = width;
            this.height = height;
        }
        
        public override Vector2 Point
        {
            get { return base.Point; }
            set
            {
                base.Point = value;
                //各頂点を移動させる
                vertex[0] = Vector2.Add(coordinate[0], value);
                vertex[1] = Vector2.Add(coordinate[1], value);
                vertex[2] = Vector2.Add(coordinate[2], value);
                vertex[3] = Vector2.Add(coordinate[3], value);

                //4分木空間分割の設定をしている場合
                if (Form != null) Form.field.Add(Form);
                
            }
        }

        /// <summary>
        /// 当たり判定の大きさを変更する
        /// </summary>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="origin">原点</param>
        public void Resize(int width,int height,Vector2 origin)
        {
            //幅、高さを代入
            this.width = width;
            this.height = height;

            //(0,0)を原点とした座標を設定
            coordinate[0].X = -origin.X;
            coordinate[0].Y = -origin.Y;

            coordinate[1].X = width - origin.X;
            coordinate[1].Y = -origin.Y;

            coordinate[2].X = width - origin.X;
            coordinate[2].Y = height - origin.Y;

            coordinate[3].X = -origin.X;
            coordinate[3].Y = height - origin.Y;

            //最小最大の座標
            MaxO = coordinate[2];
            MinO = coordinate[0];

            //4分木空間分割のため
            Point = point;
        }
        
    }
    /// <summary>
    /// 円の当たり判定
    /// </summary>
    public class CollisionCircle:CollisionShape
    {
        //半径
        public float radius;

        /// <summary>
        /// 円の当たり判定
        /// </summary>
        /// <param name="label">何の当たり判定か</param>
        /// <param name="origin">原点</param>
        /// <param name="radius">半径</param>
        public CollisionCircle(CollisionName label, Vector2 origin,float radius) :
            base(label,origin,new Vector2[] { Vector2.Zero})
        {
            this.radius = radius;

            //最小最大座標を設定
            MinO = new Vector2(-radius, -radius);
            MaxO = new Vector2(radius, radius);
            Min = MinO;
            Max = MaxO;
        }

        /// <summary>
        /// 円の中心の座標
        /// </summary>
        public Vector2 Midpoint { get { return vertex[0]; } }

        
        /// <summary>
        /// 座標
        /// </summary>
        public override Vector2 Point
        {
            get { return base.Point; }
            set
            {
                base.Point = value;
                //頂点を移動
                vertex[0] = Vector2.Add(coordinate[0], value);

                //4分木空間分割のため
                if (Form != null)Form.field.Add(Form);
                
            }
        }
        

    }

    /// <summary>
    /// 点の当たり判定
    /// </summary>
    public class CollisionDot:CollisionShape
    {

        /// <param name="label">何の当たり判定か</param>
        /// <param name="origin">原点</param>
        public CollisionDot( CollisionName label, Vector2 origin) :
            base(label,origin,new Vector2[] {Vector2.Zero})
        { }

        /// <summary>
        /// 点の座標
        /// </summary>
        public Vector2 Apex { get { return vertex[0]; } }

        
        /// <summary>
        /// 座標
        /// </summary>
        public override Vector2 Point
        {
            get { return base.Point; }
            set
            {
                base.Point = value;
                //点の移動
                vertex[0] = Vector2.Add(coordinate[0], value);

                //4分木空間分割のため
                if (Form != null) Form.field.Add(Form);

            }
        }
        

    }
    #endregion


    /// <summary>
    /// 実際に判定するクラス
    /// </summary>
    static public class Collision
    {

        static public bool Try(CollisionPolygon a, CollisionPolygon b)
        {
            //各辺の法線ベクトルを格納する準備
            Vector2[] hou = new Vector2[a.vertex.Length + b.vertex.Length];
            
            //法線ベクトルを格納
            for (int i = 0; i < a.vertex.Length; i++)
                hou[i] = new Vector2(a.line[i].Y, -1 * a.line[i].X);
            
            for (int i = 0; i < b.vertex.Length; i++)
                hou[i + a.line.Length] = new Vector2(b.line[i].Y, -1 * b.line[i].X);

            //以降の処理はTokumeiで行う
            return Tokumei(a, b, hou);

        }


        static public bool Try(CollisionPolygon a,CollisionRectangle b)
        {
            //aが完全に四角形の外側にあるとfalseを返す
            if (!((a.Min.X <= b.Min.X && b.Min.X <= a.Max.X) || (b.Min.X <= a.Min.X && a.Min.X <= b.Max.X))||
                !((a.Min.Y <= b.Min.Y && b.Min.Y <= a.Max.Y) || (b.Min.Y <= a.Min.Y && a.Min.Y <= b.Max.Y)))
                return false;

            //以降の処理はTokumeiで行う
            return Tokumei(a, b,Tokumei2(a));
        }


        //クラスが逆でも対応
        static public bool Try(CollisionRectangle a, CollisionPolygon b)
        {
            return Try(b, a);
        }



        static public bool Try(CollisionPolygon a, CollisionDot b)
        {
            //以降の処理はTokumeiで行う
            return Tokumei(a, b, Tokumei2(a));
        }

        //クラスが逆でも対応
        static public bool Try(CollisionDot a,CollisionPolygon b)
        {
            return Try(b, a);
        }


        /// <param name="a">判定する形</param>
        /// <param name="b">判定する形</param>
        /// <param name="hou">法線ベクトル</param>
        /// <returns></returns>
        static bool Tokumei(CollisionShape a,CollisionShape b,Vector2[] hou)
        {
            foreach (Vector2 i in hou)
            {
                //最大の内積を求める準備
                float max1 = i.X * a.vertex[0].X + i.Y * a.vertex[0].Y;
                float max2 = i.X * b.vertex[0].X + i.Y * b.vertex[0].Y;

                //最小の内積を求める準備
                float min1 = max1;
                float min2 = max2;

                for (int j = 1; j < a.vertex.Length; j++)
                {
                    //内積を求める
                    float l = i.X * a.vertex[j].X + i.Y * a.vertex[j].Y;
                    //最大最小を求める
                    max1 = Math.Max(max1, l);
                    min1 = Math.Min(min1, l);
                }

                for (int j = 1; j < b.vertex.Length; j++)
                {
                    //内積を求める
                    float l = i.X * b.vertex[j].X + i.Y * b.vertex[j].Y;
                    //最大最小を求める
                    max2 = Math.Max(max2, l);
                    min2 = Math.Min(min2, l);
                }

                //重なっていなかったら当たっていない
                if (!((min1 <= min2 && min2 <= max1) || (min2 <= min1 && min1 <= max2)))
                    return false;
            }
            //全て重なっていると当たっている
            return true;
        }


        /// <summary>
        /// 法線ベクトルを求める
        /// </summary>
        static Vector2[] Tokumei2(CollisionShape a)
        {
            Vector2[] hou = new Vector2[a.vertex.Length];
            for (int i = 0; i < a.vertex.Length; i++)
                hou[i] = new Vector2(a.line[i].Y, -1 * a.line[i].X);
            return hou;
        }



        static public bool Try(CollisionPolygon a, CollisionCircle b)
        {
            //aの辺を時計まわりしてｂの中点が左側にあるのかの判定用
            bool flg = false;
            for (int i = 0; i < a.vertex.Length; i++)
            {
                //aの頂点を原点としたときのｂの中点
                Vector2 v1 = new Vector2(b.Midpoint.X - a.vertex[i].X, b.Midpoint.Y - a.vertex[i].Y);
                //aの隣の頂点を原点としたときのbの中点
                Vector2 v2 = new Vector2(b.Midpoint.X - a.vertex[(i + 1) % a.vertex.Length].X, b.Midpoint.Y - a.vertex[(i + 1) % a.vertex.Length].Y);

                //外積
                float l = v1.X * v2.Y - v2.X * v1.Y;
                //aの辺の長さの２乗を求める
                float c = a.line[i].X * a.line[i].X + a.line[i].Y * a.line[i].Y;

                //内積
                float n1 = v1.X * a.line[i].X + v1.Y * a.line[i].Y;
                //内積
                float n2 = v2.X * a.line[i].X + v2.Y * a.line[i].Y;

                //bの半径の２乗
                float sub = b.radius * b.radius;

                //bの中点がaの辺(線分)内にあるとき、bの中点とaの辺(線分)の距離がbの半径以下だった場合当たっている
                if (n1 >= 0 && n2 <= 0 && l * l <= sub * c) return true;
                //bの中点がaの辺(線分)外にあるとき、bの中点とaの頂点がbの半径以下だった場合当たっている
                else if (v1.X * v1.X + v1.Y * v1.Y <= sub
                   　 || v2.X * v2.X + v2.Y * v2.Y <= sub) return true;

                //aの辺を時計まわりしてｂの中点が左側にあるのか
                flg |= l < 0;
            }

            //aの辺を時計まわりしてｂの中点が１度でも左側にあると当たっていない
            return !flg;


        }
        //クラスが逆でも対応
        static public bool Try(CollisionCircle a, CollisionPolygon b)
        { return Try(b, a); }



        static public bool Try(CollisionRectangle a, CollisionCircle b)
        {
            //aの長方形の範囲の中で円bの中点に最も近い座標を求める
            Vector2 locate = Vector2.Clamp(b.Midpoint, a.GetMin, a.GetMax);

            //上で求めた座標と円bの中点の距離が円bの半径以下の場合当たっている
            if (Vector2.DistanceSquared(locate, b.Midpoint) < b.radius * b.radius) return true;

            return false;

        }


        //クラスが逆でも対応
        static public bool Try(CollisionCircle a, CollisionRectangle b)
        { return Try(b, a); }


        static public bool Try(CollisionRectangle a, CollisionRectangle b)
        {
            //aとbの距離の差を求める
            int dis1 = (int)(b.Min.X - a.Min.X);
            int dis2 = (int)(b.Min.Y - a.Min.Y);


            if (//X軸の距離の差が長方形の幅より大きい場合当たっていない
             (dis1 > 0 && a.width <= dis1) || (dis1 < 0 && b.width <= -dis1)
             ||//Y軸の距離の差が長方形の高さより大きい場合当たっていない
             (dis2 > 0 && a.height <= dis2) || (dis2 < 0 && b.height <= -dis2)
             ) return false;


            else return true;

        }

        static public bool Try(CollisionRectangle a,CollisionDot b)
        {
            //点bが長方形aの内側にある場合当たっている
            return a.Min.X <= b.Apex.X && b.Apex.X <= a.Max.X && a.Min.Y <= b.Apex.Y && b.Apex.Y <= a.Max.Y;
        }

        //クラスが逆でも対応
        static public bool Try(CollisionDot a,CollisionRectangle b)
        {
            return Try(b, a);
        }


        static public bool Try(CollisionCircle a, CollisionCircle b)
        {
            //円aと円bの距離の差を求める
            float s1 = a.Midpoint.X - b.Midpoint.X;
            float s2 = a.Midpoint.Y - b.Midpoint.Y;

            //円aと円bの半径を足し合わせる
            float s3 = a.radius + b.radius;

            //円aと円bの距離の差が足し合わせた半径以下の場合当たっている
            return s1 * s1 + s2 * s2 <= s3 * s3;
        }

        static public bool Try(CollisionCircle a,CollisionDot b)
        {
            //円aと点bの距離の差を求める
            float s1 = a.Midpoint.X - b.Apex.X;
            float s2 = a.Midpoint.Y - b.Apex.Y;
            //円aと点bの距離の差が円aの半径以下の場合当たっている
            return s1 * s1 + s2 * s2 <= a.radius*a.radius;
        }

        //クラスが逆でも対応
        static public bool Try(CollisionDot a,CollisionCircle b)
        {
            return Try(b, a);
        }

        static public bool Try(CollisionDot a,CollisionDot b)
        {
            //点aと点bが同じ座標の場合当たっている
            return a.Apex.X == b.Apex.X && a.Apex.Y == b.Apex.Y;
        }

        /// <summary>
        /// 全ての形の当たり判定に対応
        /// </summary>
        static public bool Check(CollisionShape a,CollisionShape b)
        {
            bool t;
            if(a is CollisionPolygon)
                if (b is CollisionPolygon) t=Try(a as CollisionPolygon, b as CollisionPolygon);
                else if (b is CollisionRectangle)t= Try(a as CollisionPolygon, b as CollisionRectangle);
                else if (b is CollisionCircle)t= Try(a as CollisionPolygon, b as CollisionCircle);
                else t=Try(a as CollisionPolygon, b as CollisionDot);
            else if (a is CollisionRectangle)
                if (b is CollisionPolygon)t= Try(a as CollisionRectangle, b as CollisionPolygon);
                else if (b is CollisionRectangle)t= Try(a as CollisionRectangle, b as CollisionRectangle);
                else if (b is CollisionCircle)t= Try(a as CollisionRectangle, b as CollisionCircle);
                else t=Try(a as CollisionRectangle, b as CollisionDot);
            else if(a is CollisionCircle)
                if (b is CollisionPolygon)t= Try(a as CollisionCircle, b as CollisionPolygon);
                else if (b is CollisionRectangle)t= Try(a as CollisionCircle, b as CollisionRectangle);
                else if (b is CollisionCircle)t= Try(a as CollisionCircle, b as CollisionCircle);
                else t=Try(a as CollisionCircle, b as CollisionDot);
            else
                if (b is CollisionPolygon)t= Try(a as CollisionDot, b as CollisionPolygon);
                else if (b is CollisionRectangle)t= Try(a as CollisionDot, b as CollisionRectangle);
                else if (b is CollisionCircle)t= Try(a as CollisionDot, b as CollisionCircle);
                else t=Try(a as CollisionDot, b as CollisionDot);
            return t;
        }

    }


    /// <summary>
    /// ４分木空間分割用のリンクリストの要素
    /// </summary>
    internal class CollisionForm
    {
        //当たり判定の形状
        internal readonly CollisionShape shape;
        //４分木空間分割を行う空間
        internal readonly CollisionField field;
        //当たった物の情報を格納する
        internal readonly List<CollisionShape> hit;

        //自分より前の要素
        internal CollisionForm pre = null;
        //自分より次の要素
        internal CollisionForm nex = null;

        //空間番号
        internal int num;
        //分割レベル
        internal int lev;
        //最終的な配列上の番号
        internal int sum;

        /// <summary>
        /// ４分木空間分割用のリンクリストの要素
        /// </summary>
        /// <param name="a">当たり判定の情報</param>
        /// <param name="b">４分木空間分割を行う空間</param>
        /// <param name="hitlist">当たった物のセット先の配列</param>
        internal CollisionForm(CollisionShape a,CollisionField b,List<CollisionShape>hitlist)
        {
            shape = a;
            field = b;
            hit = hitlist;
            
        }

        //当たった情報を返す
       　internal List<CollisionShape> HitList { get { return hit; } }

        /// <summary>
        /// 指定した要素の前へ要素を挿入する
        /// </summary>
        /// <param name="input">挿入する要素</param>
        /// <param name="next">後ろに移動する要素</param>
        internal void Add(CollisionForm input)
        {
            //リンクリストへ要素を追加する
            input.pre = this.pre;
            input.nex = this;
            this.pre.nex = input;
            this.pre = input;
        }

        internal void Remove()
        {
            pre.nex = nex;
            if (nex != null) nex.pre = pre;
            pre = null;
            nex = null;
        }

    }


    /// <summary>
    /// ４分木空間分割を行う空間
    /// </summary>
    public class CollisionField
    {

        //分割空間のレベル
        static int[] lev = new int[] { 0, 1, 5, 21 };
        //設定した空間を管理する
        static List<CollisionField> fields = new List<CollisionField>();
        //各区間のリンクリストを一元管理している配列
        CollisionForm[] table = new CollisionForm[85];
        //空間の幅、高さ
        int width, height;
        //幅、高さで８を割ったもの
        float width8, height8;
        //設定された当たり判定を管理
        List<CollisionForm> forms = new List<CollisionForm>();

        /// <summary>
        /// 当たり判定を行う空間を設定する。設定された空間は４分木空間分割を
        /// 用いて判定が行われる。
        /// </summary>
        /// <param name="width">空間の幅</param>
        /// <param name="height">空間の高さ</param>
        public CollisionField(int width,int height)
        {
            //初期設定
            this.width = width;
            this.height = height;
            width8 = 8f/(width+1);
            height8 = 8f/(height+1);
            fields.Add(this);
            //各区間のリンクリストの初期値を代入
            for (int i = 0; i < table.Length; i++)
            {
                table[i] = new CollisionForm(null,this,null);
                table[i].pre = table[i];
                table[i].nex = table[i];
            }
        }

        public int GetWidth { get { return width; } }
        public int GetHeight { get { return height; } }

        /// <summary>
        /// 当たり判定を空間へ追加する
        /// </summary>
        /// <param name="shape">当たり判定</param>
        /// <param name="hitlist">当たり判定の結果を保存する配列</param>
        public void Set(CollisionShape shape,List<CollisionShape> hitlist=null)
        {
            //当たり判定とリンクリストの要素をつなぐ
            shape.Form = new CollisionForm(shape,this,hitlist);

            //リンクリストの要素を追加
            forms.Add(shape.Form);
            //空間へ要素を追加
            Add(shape.Form);
        }

        /// <summary>
        ///区間別に分けたリンクリストへ要素を追加する
        /// </summary>
        internal void Add(CollisionForm a)
        {
            
            //空間番号と分割空間のレベルを求める
            NumLev(a);
            //該当箇所のリンクリストが格納してある要素番号を取得
            int s = a.sum;
            //リンクリストへ要素を追加する
            table[s].Add(a);

            
        }

        
        /// <summary>
        /// 空間から当たり判定を削除する
        /// </summary>
        public void Delete(CollisionShape a)
        {
            //リンクリストから離す
            a.Form.Remove();
            //空間から当たり判定を離す
            forms.Remove(a.Form);
            //当たり判定とリンクリストの関係を断つ
            a.Form = null;
        }


        /// <summary>
        /// 空間の当たり判定を検出する
        /// vertion1
        /// </summary>
        void Up_v1()
        {
            //判定の処理が完了した区間をメモ
            bool[] ggflg = new bool[85];

            for (int i = 0; i < forms.Count; i++)
            {
                //物体が所属している区間の要素番号を取得
                int su = forms[i].sum;

                //判定の処理が既に完了している場合以下の処理をスキップする
                if (ggflg[su]) continue;


                //リンクリストの要素のスタックを用意
                Stack<CollisionForm> sta = new Stack<CollisionForm>();

                //空間番号と空間レベルを取得
                int num = forms[i].num;
                int k = forms[i].lev;

                //親空間に所属している物体の情報をスタックへ追加する
                while (k != 0)
                {
                    //親の空間番号を取得
                    num >>= 2;
                    //配列上の要素番号を取得
                    int ss = num + lev[--k];
                    //区間のリンクリストを取得
                    CollisionForm lj = table[ss].nex;

                    //リンクリストの要素が初期値の場合終了
                    while (lj != table[ss])
                    {
                        //スタックへ要素を追加
                        sta.Push(lj);
                        //次の要素へ
                        lj = lj.nex;
                    }
                }

                //最初の要素を取得
                CollisionForm li = table[su].nex;


                //当たり判定を行う
                while (li != table[su])
                {
                    //次の要素を取得
                    CollisionForm lj = li.nex;
                    //次の要素が初期値の場合終了
                    while (lj != table[su])
                    {
                        //当たり判定処理
                        IfHit(li, lj);
                        //次の要素へ
                        lj = lj.nex;
                    }
                    //スタックにある物体とも当たり判定を行う
                    foreach (CollisionForm kk in sta) IfHit(kk, li);

                    //次の要素へ
                    li = li.nex;
                }

                //当たり判定の処理が完了
                ggflg[su] = true;
            }

        }
        /// <summary>
        /// version2
        /// </summary>
        void Up_v2()
        {
            //各要素を総当たりする
            for (int i = 0; i < forms.Count - 1; i++)
                for (int j = i + 1; j < forms.Count; j++)
                    IfHit(forms[i], forms[j]);
        }

        /// <summary>
        /// もしかしたら当たっているかもしれない
        /// </summary>
        void IfHit(CollisionForm a,CollisionForm b)
        {
            //当たっているかの判定
            if (a.hit!=b.hit&&Collision.Check(a.shape, b.shape))
            {
                //当たっていたら両方のヒットリストへ情報を追加する
                if(a.hit!=null) a.hit.Add(b.shape);
                if(b.hit!=null)b.hit.Add(a.shape);
            }
        }

        /// <summary>
        /// 空間番号と空間レベルを求める
        /// </summary>
        void NumLev(CollisionForm a)
        {
            //物体の左上の空間番号
            int min = get_tablenumber((int)(a.shape.Min.X * (width8)), (int)(a.shape.Min.Y * (height8)));
            //物体の右下の空間番号
            int max = get_tablenumber((int)(a.shape.Max.X * (width8)), (int)(a.shape.Max.Y * (height8)));

            //どの空間レベルまで一致しているのかを調べる
            int xor = min ^ max;

            //空間レベルを調べる
            if (xor == 0)
            {
                a.num = min;
                a.lev = 3;
            }
            else if (xor >> 2 == 0)
            {
                a.num = min >> 2;
                a.lev = 2;
            }
            else if (xor >> 4 == 0)
            {
                a.num = min >> 4;
                a.lev = 1;
            }
            else
            {
                a.num = 0;
                a.lev = 0;
            }

            //各区間のリンクリストを一元管理している配列上での要素番号を求める
            a.sum = a.num + lev[a.lev];
        }

        static public void Update()
        {
            //各空間を更新
            foreach (CollisionField i in fields)
            { 
                //空間がもっている全ての当たり判定を初期化する
                foreach (CollisionForm j in i.forms)if (j.hit != null) j.hit.Clear();
                
                //当たり判定の検出を行う
                i.Up_v1();
            }
        }

        /// <summary>
        /// 設定していた空間を管理リストから切り離す
        /// </summary>
        static public void Remove(CollisionField field)
        {
            //foreach (CollisionForm i in field.forms) field.Delete(i.shape);

            fields.Remove(field);
        }
        /// <summary>
        /// 各ビット間に0を挿入する
        /// <para><![CDATA[例：1111->1010101]]></para>
        /// </summary>
        /// <param name="n"></param>
        static int Ct(int n)
        {
            n = (n | (n << 8)) & 0x00ff00ff;
            n = (n | (n << 4)) & 0x0f0f0f0f;
            n = (n | (n << 2)) & 0x33333333;
            return (n | (n << 1)) & 0x55555555;
        }


        /// <summary>
        /// 空間番号を取得する
        /// </summary>
        /// <param name="x">空間上のX座標</param>
        /// <param name="y">空間上のY座標</param>
        /// <returns></returns>
        static int get_tablenumber(int x, int y)
        {
            return Ct(x) | (Ct(y) << 1);
        }
    }






}
