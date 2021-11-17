using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MDclass.VariationClass;
namespace MDclass.ImageAndSprite
{
    /// <summary>
    /// 全てのスプライトの親
    /// </summary>
    class SpriteOrigin : Sprite
    {
        public SpriteOrigin():base(ImageName.White)
        {
            _parent = this;
            _ishide = true;
        }
    }

    /// <summary>
    /// スプライトを管理するクラス
    /// </summary>
    public static class SpriteManager
    {
        //管理するスプライト
        internal static List<Sprite> sprites = new List<Sprite>();
        //削除するスプライト
        internal static List<Sprite> deleteS = new List<Sprite>();
        //親スプライト
        internal static Sprite origin = new SpriteOrigin();

        /// <summary>
        /// スプライトを追加
        /// </summary>
        internal static void Add(Sprite sprite)
        {
            sprites.Add(sprite);
        }
        //全てのスプライトを一括で更新する
        static public void UpDate(GameTime gameTime)
        {
            //更新
            foreach (Sprite i in sprites) i.Update(gameTime);
            //削除予定のスプライトを削除
            foreach (Sprite i in deleteS) i.Delete2();
            //削除リストをクリア
            deleteS.Clear();
        }

        //当たり判定の処理
        static public void CollisionUpdate()
        {
            foreach (Sprite i in sprites) i.CollisionUpdate();
        }

        //描写の処理
        static public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            foreach (Sprite i in sprites) i.Draw(batch);
        }

    }

    /// <summary>
    /// スプライト、
    /// アニメーションなどに対応
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// 親スプライトを基準にしたときの座標
        /// </summary>
        protected Vector2 _point=Vector2.Zero;
        /// <summary>
        /// 文字スプライト用
        /// </summary>
        protected string _letter = null;
        /// <summary>
        /// z軸
        /// </summary>
        protected float _depth=0;
        /// <summary>
        /// 画面上の座標
        /// </summary>
        protected Vector3 _dispoint = Vector3.Zero;
        /// <summary>
        /// 倍率
        /// </summary>
        protected Vector2 _scale=Vector2.One;
        /// <summary>
        /// 色
        /// </summary>
        protected Color _color = Color.White;
        /// <summary>
        /// 角度
        /// </summary>
        protected float _rot=0f;
        /// <summary>
        /// 親スプライト
        /// </summary>
        protected Sprite _parent= SpriteManager.origin;
        /// <summary>
        /// 子スプライト
        /// </summary>
        protected List<Sprite> _children = new List<Sprite>();
        /// <summary>
        /// 画像の切り取りデータ
        /// </summary>
        protected Image _image;
        /// <summary>
        /// 画像データ
        /// </summary>
        protected Texture2D _texture;
        /// <summary>
        /// 切り取りデータ
        /// </summary>
        protected Rectangle _rectangle;
        /// <summary>
        /// 基準の座標
        /// </summary>
        protected Vector2 _origin = Vector2.Zero;
        /// <summary>
        /// 非表示
        /// </summary>
        protected bool _ishide = false;
        /// <summary>
        /// アニメーションのデータ
        /// </summary>
        internal Variation[] anim = new Variation[6] {null,null,null,null,null,null};

        /// <summary>
        /// スプライトを設定
        /// </summary>
        /// <param name="imageNumber">画像の番号</param>
        public Sprite(int imageNumber){MakeSprite(imageNumber);}
        /// <summary>
        /// スプライトを設定
        /// </summary>
        /// <param name="imageNumber">画像の名前</param>
        public Sprite(ImageName imageNumber){MakeSprite((int)imageNumber);}
        /// <summary>
        /// スプライトを設定
        /// </summary>
        /// <param name="imageNumber">画像の名前</param>
        /// <param name="a">変位</param>
        public Sprite(ImageName imageNumber,int a){MakeSprite((int)imageNumber+a);}
        /// <summary>
        /// スプライトを設定
        /// </summary>
        /// <param name="image">画像の切り取りデータ</param>
        public Sprite(Image image)
        {
            if (image == null) image = Image.Get(ImageName.White);
            _image = image;
            _texture = image.ToTexture2D;
            _rectangle = image.ToRectangle;
            SpriteManager.Add(this);
        }
        /// <summary>
        /// スプライトを設定
        /// </summary>
        /// <param name="texture">画像データ</param>
        /// <param name="rec">切り取り範囲</param>
        public Sprite(Texture2D texture,Rectangle rec)
        {
            this._texture = texture;
            this._rectangle = rec;
            SpriteManager.Add(this);
        }
        /// <summary>
        /// スプライトを設定
        /// </summary>
        /// <param name="texture">画像データ</param>
        public Sprite(Texture2D texture)
        {
            this._texture = texture;
            this._rectangle = texture.Bounds;
            SpriteManager.Add(this);
        }

        //スプライト管理へ追加する
        void MakeSprite(int a)
        {
            _image = Image.Get(a);
            _texture = _image.ToTexture2D;
            _rectangle = _image.ToRectangle;
            SpriteManager.Add(this);
        }


        /// <summary>
        /// アニメーションデータを取得
        /// </summary>
        /// <param name="name">アニメーションの種類</param>
        public Variation GetAnim(AnimName name)
        {
            if (name == AnimName.ALL) return null;
            return anim[(int)name];
        }

        /// <summary>
        /// アニメーションを削除
        /// </summary>
        /// <param name="animName">アニメーションの種類</param>
        public void AnimDelate(AnimName animName)
        {

            if (animName == AnimName.ALL) 
                for (int i = 0; i < anim.Length; i++)
                {
                    Variation sub = anim[i];
                    if (sub != null) sub.Delete();
                    anim[i] = null;
                }
            else
            {
                Variation sub = anim[(int)animName];
                if (sub != null) sub.Delete();
                anim[(int)animName] = null;
            }

        }



        /// <summary>
        /// アニメーションが実行されているか
        /// </summary>
        /// <param name="animName">アニメーションの種類</param>
        /// <returns></returns>
        public bool AnimFlag(AnimName animName)
        {
            if (animName == AnimName.ALL)
            {
                for (int i = 0; i < anim.Length; i++) if (anim[i] != null) return true;
                return false;
            }
            return anim[(int)animName] != null;
        }
        


        public virtual Vector2 Point { get {return _point; } set {_point = value; } }
        public virtual float Depth { get { return _depth; } set { _depth = value; } }
        public virtual Vector2 Scale { get { return _scale; } set { _scale=value; } }
        public virtual Color Color { get { return _color; } set { _color = value; } }
        public virtual float Rot { get { return _rot; } set { _rot = value; } }


        public virtual Image Image { get { return _image; } set {
                _image = value;
                _texture = value.ToTexture2D;
                _rectangle = value.ToRectangle;
            } }

        public virtual Texture2D Texture2D { get { return _texture; } set
            {
                _texture = value;
                _image = null;
            } }

        public virtual Rectangle Rectangle { get { return _rectangle; } set { _rectangle = value; } }

        public virtual Vector2 Origin { get { return _origin; } set { _origin = value; } }
        public virtual bool isHide { get { return _ishide; } set { _ishide = value; } }

        

        public Vector2 DisplayPoint { get { return new Vector2(_dispoint.X, _dispoint.Y); } }

        public int Width { get {
                if (_image == null) return _texture.Width;
                return _image.ToRectangle.Width; 
            } }
        public int Height { get {
                if (_image == null) return _texture.Height;
                return _image.ToRectangle.Height; 
            } }
        public Vector2 Frame { get { return new Vector2(Width, Height); } }



        public virtual void Update(GameTime gameTime)
        {
            //アニメーションが実行されている場合各パラメータを更新する

            if (anim[0] != null)
            {
                _image = (anim[0] as VariationImage).Target;
                _texture = _image.ToTexture2D;
                _rectangle = _image.ToRectangle;
                sub1(0);
            }
            if (anim[1] != null)
            {
                _point = (anim[1] as VariationVector2).Target;
                sub1(1);
            }
            if (anim[2] != null)
            {
                _color = (anim[2] as VariationColor).Target;
                sub1(2);
            }
            if (anim[3] != null)
            {
                _scale = (anim[3] as VariationVector2).Target;
                sub1(3);
            }
            if (anim[4] != null)
            {
                _rot = (anim[4] as VariationFloat).Target;
                sub1(4);
            }

            if (anim[5] != null)
            {
                _letter = (anim[5] as VariationString).Target;
                sub1(5);
            }


        }

        public virtual void CollisionUpdate() { }


        //Updateのサブ関数
        void sub1(int i)
        {
            if (anim[i].isCompletion)
                if (anim[i].Next != null) anim[i] = anim[i].Next;
                else anim[i] = null;
        }


        /// <summary>
        /// アニメーションを実行
        /// <para>例：Anim(アニメーションの種類,ループ回数,待機時間1,パラメータ1,待機時間2,パラメータ2,...)</para>
        /// ループ回数が0の場合無限ループ
        /// <para>待機時間が負の場合パラメータはスムーズに変化する</para>
        /// <para>注意：アニメーションが実行途中かつ種類が同じ場合、今から設定するアニメーションはすぐに実行されません。
        /// 実行途中のアニメーションが完了してから実行されます。アニメーションをすぐに実行させたい場合は
        /// 直前にAnimDelateを実行させてください</para>
        /// <para>パラメータの型　画像:Image</para>
        /// <para>　　　　　　　　座標:Vector2</para>
        /// <para>　　　　　　　　色　:Color</para>
        /// <para>　　　　　　　　倍率:Vector2</para>
        /// <para>　　　　　　　　角度:float</para>
        /// <para>　　　　　　　　文字:string　(Labelの場合使用可能)</para>
        /// </summary>
        /// <param name="name">アニメーションの種類</param>
        /// <param name="loop">ループ回数</param>
        /// <param name="Time_Instance">待機時間 , パラメータ</param>
        public void Anim(AnimName name, int loop, params object[] Time_Instance)
        {
            object[] sub = new object[Time_Instance.Length + 2];
            sub[0] = name;
            sub[1] = loop;
            Array.Copy(Time_Instance, 0, sub, 2, Time_Instance.Length);
            Anim(sub);
        }
        /// <summary>
        /// アニメーションを実行
        /// </summary>
        /// <param name="Name_Loop_Time_Instance">アニメーションデータ</param>
        public void Anim(object[] Name_Loop_Time_Instance)
        {
            //第一引数
            AnimName name = (AnimName)Name_Loop_Time_Instance[0];
            //第二引数
            int loop = (int)Name_Loop_Time_Instance[1];
            //アニメーションデータ
            object[] Time_Instance = new object[Name_Loop_Time_Instance.Length - 2];
            Array.Copy(Name_Loop_Time_Instance, 2, Time_Instance, 0, Time_Instance.Length);

            //使用出来ない名前
            if (name == AnimName.ALL) return;

            //代入する用
            Variation sub;
            //各アニメーションの種類
            if (name == AnimName.Image) sub = new VariationImage(_image, loop, Time_Instance);
            else if (name == AnimName.Move) sub = new VariationVector2(_point, loop, Time_Instance);
            else if (name == AnimName.Color) sub = new VariationColor(_color, loop, Time_Instance);
            else if (name == AnimName.Scale) sub = new VariationVector2(_scale, loop, Time_Instance);
            else if (name == AnimName.Rot) sub = new VariationFloat(_rot, loop, Time_Instance);
            else sub = new VariationString(_letter, loop, Time_Instance);

            //スプライトが持っているアニメーションデータ
            Variation sub2 = anim[(int)name];
            //空でない場合
            if (sub2 != null)
            {
                //いま実行しているアニメーションが終了するとこのアニメーションを実行する
                while (sub2.Next != null) sub2 = sub2.Next;
                //いま実行しているアニメーションが無限ループをしている場合このアニメーションを設定しない
                if (!sub2.isInfinite) sub2.Next = sub;
            }
            //空の場合そのまま代入
            else anim[(int)name] = sub;


        }



        //描写
        public virtual void Draw(SpriteBatch batch)
        {
            //親スプライトの座標を参照して画面表示
            _dispoint.X = _point.X + _parent._point.X;
            _dispoint.Y = _point.Y + _parent._point.Y;
            _dispoint.Z = _depth + _parent._depth; 
            //隠している場合次へ
            if (_ishide) return;

            //描写開始
            batch.Draw(_texture, DisplayPoint, _rectangle, _color, _rot,_origin, _scale,
                SpriteEffects.None, _dispoint.Z);
        }

        /// <summary>
        /// 親スプライトを設定する
        /// <para>注意：親スプライトを先に生成する必要があります</para>
        /// <para>注意を守らない場合スプライトの表示が若干おかしくなります</para>
        /// </summary>
        /// <param name="parent">親スプライト</param>
        public void Link(Sprite parent)
        {
            //nullの場合回避用のスプライトを親にする
            if (parent == null) parent = SpriteManager.origin;

            //エラー表示
            try
            {
                if (SpriteManager.sprites.IndexOf(this) <= SpriteManager.sprites.IndexOf(parent))
                    throw new Exception("自分より管理番号が若いSpriteを親にしてください");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            //親スプライトを設定
            this._parent = parent;
            this._parent._children.Add(this);
        }


        //本当に消す
        internal void Delete2()
        {
            SpriteManager.sprites.Remove(this);
            AnimDelate(AnimName.ALL);
            _parent = null;
            foreach (Sprite i in _children) i.Delete2();
            _children = null;
        }

        /// <summary>
        /// スプライトの管理リストから離す
        /// 子スプライトも離される
        /// </summary>
        public virtual void Delete()
        {
            //削除リストへ追加
            SpriteManager.deleteS.Add(this);
        }

       

    }

    /// <summary>
    /// アニメーションの種類
    /// </summary>
    public enum AnimName : int
    {

        ALL=-1,
        /// <summary>
        /// 画像
        /// </summary>
        Image, 
        /// <summary>
        /// 座標
        /// </summary>
        Move, 
        /// <summary>
        /// 色
        /// </summary>
        Color, 
        /// <summary>
        /// 倍率
        /// </summary>
        Scale, 
        /// <summary>
        /// 角度
        /// </summary>
        Rot,
        /// <summary>
        /// 文字列
        /// </summary>
        String

    }







}
