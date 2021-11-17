using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace MDclass.Write
{
    /// <summary>
    /// 画面に文字を表示させる
    /// </summary>
    public static class WriteString
    {
        //使用するフォント
        static SpriteFont _font;
        //表示する文字列の情報
        static Dictionary<string, WString> strR = new Dictionary<string, WString>();

        /// <summary>
        /// デフォルトのフォントをロード
        /// </summary>
        /// <param name="Font">フォント</param>
        static internal void Load(SpriteFont Font)
        {
            _font =Font;
        }

        /// <summary>
        /// 文字列を設定
        /// </summary>
        /// <param name="tag">識別子</param>
        /// <param name="text">文字列</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="font">フォント</param>
        static public void Write(string tag,string text,int x,int y,SpriteFont font = null)
        {
            Write(tag, text, x, y, Color.White,font);
        }

        /// <summary>
        /// 文字列を設定
        /// </summary>
        /// <param name="tag">識別子</param>
        /// <param name="text">文字列</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="color">色</param>
        /// <param name="font">フォント</param>
        static public void Write(string tag,string text,int x,int y,Color color,SpriteFont font = null)
        {
            //初めての識別子の場合データを新たに作る
            if (!strR.ContainsKey(tag)) strR[tag] = new WString(text, x, y,color,font);
            else {
                WString sub = strR[tag];
                sub.text = text;
                sub.point.X = x;
                sub.point.Y = y;
                sub.font = font ?? _font;
            }
        }

        /// <summary>
        /// 文字列を描写する
        /// </summary>
        static internal void Draw(SpriteBatch spriteBatch)
        {
            foreach (WString i in strR.Values) {
                spriteBatch.DrawString(i.font, i.text, i.point, i.color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// 文字列を削除する
        /// </summary>
        /// <param name="tag">識別子</param>
        static public void Delete(string tag)
        {
            if (strR.ContainsKey(tag))
                strR.Remove(tag);
        }

        /// <summary>
        /// 文字列の情報
        /// </summary>
        class WString
        {
            //座標
            public Vector2 point;
            //文字列
            public string text;
            //色
            public Color color;
            //フォント
            public SpriteFont font;
            public WString(string str,int x,int y,Color color,SpriteFont font)
            {
                text = str;
                point = new Vector2(x, y);
                this.color = color;
                this.font = font ?? _font;
            }
        }

    }
}
