using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
namespace MDclass.ImageAndSprite
{

    /// <summary>
    /// 画像データをロードするときに使用するクラス
    /// </summary>
    public class ImageLoad
    {
        int set;
        Game game1;
        /// <summary>
        /// ImageNameANDLoad.cs の ImageLoadDetail の DetailLoadで設定された画像データを一気にロードする
        /// </summary>
        static public void Load(Game game)
        {
            var sub = new ImageLoadDetail() { game1 = game };
            
            Texture2D texture = new Texture2D(game.GraphicsDevice, 1, 1);
            //まずは白い点だけの画像を設定（初期画像として）
            Color[] colors = new Color[] { Color.White };
            texture.SetData(colors);
            sub.PageAdd(texture);
            sub.ImageAdd(sub.set, new Rectangle(0, 0, 1, 1));
            //ロード開始
            sub.DetailLoad();
        }

        /// <summary>
        /// 現在の画像データを出力
        /// </summary>
        protected Texture2D CurrentPage()
        {
            return Image.page[set];
        }

        /// <summary>
        /// 画像データを追加
        /// </summary>
        /// <param name="name">ファイル名</param>
        protected void PageAdd(string name)
        {
            PageAdd(game1.Content.Load<Texture2D>(name));
        }

        /// <summary>
        /// 画像データを追加
        /// </summary>
        /// <param name="name">テクスチャデータ</param>
        protected void PageAdd(Texture2D texture)
        {
            set = Image.page.Count;
            Image.page.Add(texture);
        }
        /// <summary>
        /// 今の画像データを変更
        /// </summary>
        /// <param name="set1">画像データの番号</param>
        protected void ChangeSet(int set1)
        {
            set = set1;
        }

        /// <summary>
        /// 画像データを長方形に切り取って切り取りデータとして設定する
        /// </summary>
        /// <param name="rectangles">切り取る範囲</param>
        protected void ImageAdd(Rectangle rectangles)
        {
            ImageAdd(set, rectangles);
        }

        /// <summary>
        /// 画像データの番号を指定して長方形に切り取ってI切り取りデータとして設定する
        /// </summary>
        /// <param name="pageI">画像データの番号</param>
        /// <param name="rectangles">切り取る範囲</param>
        protected void ImageAdd(int pageI, Rectangle rectangles)
        {
            Image.images.Add(new Image(pageI, rectangles));
        }

        /// <summary>
        ///  画像データを様々な長方形で切り取って一気にたくさんの切り取りデータを設定する
        /// </summary>
        /// <param name="rectangles">切り取る範囲たち</param>
        protected void ImageAdd(Rectangle[] rectangles)
        {
            foreach (Rectangle i in rectangles) ImageAdd(i);
        }
    }



    /// <summary>
    /// 画像の切り取りデータ
    /// </summary>
    public class Image
    {
        /// <summary>
        /// 切り取りデータのもととなる画像データ
        /// </summary>
        static internal List<Texture2D> page = new List<Texture2D>();
        /// <summary>
        /// どの画像からどこを切り取るのかを保存している配列
        /// </summary>
        static internal List<Image> images = new List<Image>();

        readonly Texture2D texture;
        readonly Rectangle rectangle;

        /// <summary>
        ///　切り取りデータを設定
        /// </summary>
        /// <param name="pageI">画像データが保存されている番号</param>
        /// <param name="rectangles">切り取る範囲</param>
        internal Image(int pageI,Rectangle rectangles)
        {
            texture = page[pageI];
            rectangle = rectangles;
        }

        /// <summary>
        /// 画像の切り取りデータを2次元のColorデータへ変換させる
        /// </summary>
        public Color[,] ToColors()
        {
            Color[] data =new Color[texture.Width*texture.Height];
            Color[,] send = new Color[rectangle.Height, rectangle.Width];

            texture.GetData<Color>(data);

            for (int y = rectangle.Y; y < rectangle.Bottom; y++)
                for (int x = rectangle.X; x < rectangle.Right; x++){
                    int sx = x - rectangle.X;
                    int sy = y - rectangle.Y;
                    send[sy, sx] = data[y * texture.Width + x];

                }

            return send;

        }


        public Texture2D ToTexture2D { get { return texture; } }
        public Rectangle ToRectangle { get { return rectangle; } }

        /// <summary>
        /// 画像の切り取りデータを出力
        /// </summary>
        /// <param name="a">画像の番号</param>
        static public Image Get(int a)
        {
            return images[a];
        }

        /// <summary>
        /// 画像切り取りデータを出力
        /// </summary>
        /// <param name="name">画像の名前</param>
        static public Image Get(ImageName name)
        {
            return Get((int)name);
        }

        /// <summary>
        /// 画像の名前と変位を指定して画像の切り取りデータを出力
        /// </summary>
        /// <param name="name">画像の名前</param>
        /// <param name="a">変位</param>
        /// <returns></returns>
        static public Image Get(ImageName name,int a)
        {
            return Get((int)name + a);
        }

    }



}
