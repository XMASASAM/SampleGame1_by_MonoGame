using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MDclass.TouchClass;
using MDclass.ImageAndSprite;
namespace MDclass.LabelClass
{
    /// <summary>
    /// 文字表示版のスプライト
    /// </summary>
    public class Label:Sprite
    {
        //文字表示用のフォント
        static SpriteFont BaseFont;
        /// <summary>
        /// 初期設定
        /// </summary>
        /// <param name="font">基本のフォント</param>
        static public void Load(SpriteFont font)
        {
            BaseFont = font;
        }

        //文字表示時のフォント
        protected SpriteFont _font;
        //非表示
        protected bool _strIsHide = false;
        /// <summary>
        /// 文字列を表示。
        /// フォント無指定の場合初期設定時のフォントを使用する
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="point">座標</param>
        /// <param name="font">フォント</param>
        public Label(string text,Vector2 point,SpriteFont font=null) :base(ImageName.White)
        {
            _letter = text;
            _font = font ?? BaseFont;
             _ishide = true;
            
            Point = point;
        }


        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            //描写
            if (_letter ==null || _letter == "" || _strIsHide) return;
            batch.DrawString(_font,_letter,_point,_color,_rot,_origin,_scale,SpriteEffects.None,_depth) ;
        }


        public string Text { get { return _letter; }set { _letter = value; }}


        public SpriteFont SpriteFont { get { return _font; } set { _font = value; } }

        public override bool isHide { get { return _strIsHide; }set { _strIsHide = value; } }

    }

    /// <summary>
    /// 押しボタン用のクラス
    /// </summary>
    public class Button:Label
    {
        //ボタンの範囲
        protected readonly TouchBoard _board;
        //一つ一つのタッチを検知させるか
        protected readonly bool _individual;
        //ボタンが設定されているビュー
        protected TouchView _view;

        /// <summary>
        /// 押しボタンを設定
        /// </summary>
        /// <param name="view">ボタンが設定するビュー</param>
        /// <param name="board">ボタンの範囲</param>
        /// <param name="name">画像の名前</param>
        /// <param name="individual">個々のタッチは識別するのか</param>
        /// <param name="font">フォント</param>
        public Button(TouchView view,TouchBoard board,Image name,bool individual=false,SpriteFont font=null)
            :base("",board.Point,font)
        {
            _board = board;
            _individual = individual;
            _view = view;
            _view.Add(_board);
        }

        //public TouchBoard ToTouchBoard { get { return _board; } }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //初めて押した場合
            if (_board.FirstFlag) FirstPress();
            //中間
            else if (_board.MiddlesFlag) MiddlePress();
            //タッチが終了した場合
            else if (_board.FinalFlag) FinalPress();


            //個々を識別する場合
            if(_individual)
                foreach(TouchSatellite i in _board.TouchList)
                    if (i.FirstFlag) IndividualFirstPress(i);
                    else if (i.MiddleFlag) IndividualMiddlePress(i);
                    else if (i.FinalFlag) IndividualFinalPress(i);
        }

        /// <summary>
        /// 初め
        /// </summary>
        public virtual void FirstPress() { }

        /// <summary>
        /// 中間
        /// </summary>
        public virtual void MiddlePress() { }

        /// <summary>
        /// 終わり
        /// </summary>
        public virtual void FinalPress(){ }

        /// <summary>
        /// 初め
        /// </summary>
        /// <param name="touch">タッチ情報</param>
        public virtual void IndividualFirstPress(TouchSatellite touch){ }

        /// <summary>
        /// 中間
        /// </summary>
        /// <param name="touch">タッチ情報</param>
        public virtual void IndividualMiddlePress(TouchSatellite touch) { }

        /// <summary>
        /// 終わり
        /// </summary>
        /// <param name="touch">タッチ情報</param>
        public virtual void IndividualFinalPress(TouchSatellite touch) { }



    }
        
}
