using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
namespace MDclass.GeometricClass
{
    /// <summary>
    /// 絵を描くクラス
    /// </summary>
    public class Geometric
    {
        static GraphicsDevice graphics;
        //描いたデータ
        Color[,] pixcel;
        //描くキャンバスの幅と高さ
        int width, height;

        /// <summary>
        /// 初期設定
        /// </summary>
        static public void Load(GraphicsDevice device)
        {
            graphics = device;
        }

        /// <summary>
        /// キャンバスをつくる
        /// </summary>
        /// <param name="x">幅</param>
        /// <param name="y">高さ</param>
        public Geometric(int x,int y)
        {
            //値の初期設定
            pixcel = new Color[y, x];
            width = x;
            height = y;
            
        }

        /// <summary>
        /// 色データを長方形で切り取る
        /// </summary>
        /// <param name="pixcel">色データ</param>
        /// <param name="rec">切り取る範囲</param>
        /// <returns></returns>
        static public Color[,] Cutout(Color[,] pixcel,Rectangle rec)
        {
            //返す値の準備
            Color[,] send = new Color[rec.Height,rec.Width];
            //長方形の範囲で切り取る
            for (int y = rec.Y; y < rec.Y + rec.Height; y++)
                for (int x = rec.X; x < rec.X + rec.Width; x++)
                    if (inIndex(pixcel, x, y)) send[y - rec.Y, x - rec.X] = pixcel[y, x];

            return send;

        }

        /// <summary>
        /// 自身の色データを長方形で切り取る
        /// </summary>
        /// <param name="rec">切り取る範囲</param>
        /// <returns></returns>
        public Color[,] Cutout(Rectangle rec)
        {
            return Cutout(pixcel, rec);
        }

        /// <summary>
        /// 画像のデータを横反転する
        /// </summary>
        /// <param name="pixcel">画像データ</param>
        static public Color[,] InversionH (Color[,] pixcel)
        {
            //幅と高さを取得
            int w = pixcel.GetLength(1);
            int h = pixcel.GetLength(0);

            //返すデータの準備
            Color[,] temp = new Color[h,w];

            //データを反転
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    temp[y, w - x-1] = pixcel[y, x];

            return temp;
        }

        /// <summary>
        /// 画像のデータを横反転する
        /// </summary>
        public Color[,] InversionH()
        {
            return InversionH(pixcel);
        }

        /// <summary>
        /// 画像のデータを縦反転する
        /// </summary>
        static public Color[,] InversionV(Color[,] pixcel)
        {
            int w = pixcel.GetLength(1);
            int h = pixcel.GetLength(0);

            Color[,] temp = new Color[h, w];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    temp[h- y-1,x] = pixcel[y, x];
            return temp;
        }


        /// <summary>
        /// 画像のデータを縦反転する
        /// </summary>
        public Color[,] InversionV()
        {
            return InversionV(pixcel);
        }

        /// <summary>
        /// Texture2DからColorの2次元配列を生成する
        /// </summary>
        static public Color[,] GetColors(Texture2D texture)
        {
            //画像の幅と高さ
            int w = texture.Bounds.Width;
            int h = texture.Bounds.Height;
            //Texture2Dからのデータを格納する準備
            Color[] data = new Color[h*w];
            //2次元配列にする用の配列
            Color[,] data2 = new Color[h,w];
            //データをセットする
            texture.GetData<Color>(data);
            //2次元配列へ格納する
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    data2[y, x] = data[y * w + x];

            return data2;
        }

        /// <summary>
        /// Texture2Dの画像データを横に反転する
        /// </summary>
        static public Texture2D InversionH(Texture2D texture)
        {
            return ToTexture(InversionH(GetColors(texture)));
        }
        /// <summary>
        /// Texture2Dの画像データを縦に反転する
        /// </summary>
        static public Texture2D InversionV(Texture2D texture)
        {
            return ToTexture(InversionV(GetColors(texture)));
        }

        /// <summary>
        /// 長方形を描写
        /// </summary>
        /// <param name="pixcel">描写する画像データ</param>
        /// <param name="rec">長方形のデータ</param>
        /// <param name="color">色データ</param>
        static public void rectangle(Color[,] pixcel,Rectangle rec, Color color)
        {
            //高さ
            for (int i = 0; i < rec.Height; i++)
                //幅
                for (int j = 0; j < rec.Width; j++)
                {
                    //描写するデータの座標
                    int x = rec.X + j;
                    int y = rec.Y + i;
                    //座標が画像の横幅立幅にはみ出ていないかチェックする
                    if (inIndex(pixcel,x,y))
                        //色データを入力
                        pixcel[y,x] = color;
                }
        }

        /// <summary>
        /// 長方形を描写
        /// </summary>
        /// <param name="rec">長方形のデータ</param>
        /// <param name="color">色データ</param>
        public void rectangle(Rectangle rec, Color color)
        {
            rectangle(pixcel, rec, color);
        }

        /// <summary>
        /// 長方形の線を描写する
        /// </summary>
        /// <param name="rec">長方形のデータ</param>
        /// <param name="color">色データ</param>
        /// <param name="thick">線の太さ</param>
        public void rectangle(Rectangle rec, Color color, int thick)
        {
            //長方形の幅高さ
            int w = rec.Width;
            int h = rec.Height;
            //長方形用の画像データを準備
            Color[,] pix = new Color[h+thick, w+thick];
            //一色で埋めつくす
            rectangle(pix,new Rectangle(0, 0,w,h), color);
            //太さ分小さくした透明の長方形を上から描写する
            rectangle(pix,new Rectangle(thick, thick, w - thick*2, h - thick*2), Color.Transparent);

            //自身のデータと合体させる
            attach(pix, new Vector2(rec.X, rec.Y));

        }


        /// <summary>
        /// 円を描写する
        /// </summary>
        /// <param name="pixcel">描写する画像データ</param>
        /// <param name="vec">中点の座標</param>
        /// <param name="r">半径</param>
        /// <param name="color">色データ</param>
        /// <param name="d1">始点の角度</param>
        /// <param name="d2">終点の角度</param>
        static public void circle(Color[,] pixcel,Vector2 vec,double r, Color color,float d1=0,float d2=360)
        {
            //微調整
            double rr = r - 1 / 2.0;
            //直径
            int R = (int)(r * 2);
            //ラジアン
            double dd1 = d1 * (Math.PI / 180) -Math.PI;//始点
            double dd2 = d2 * (Math.PI / 180) -Math.PI;//終点

            for (int i = 0; i < R; i++)
                for (int j = 0; j < R; j++)
                {
                    //中点からの距離の2乗
                    double sub = ((i - rr) * (i - rr) + (j - rr) * (j - rr));
                    //上の値が円の半径以下の場合かつ座標が画像データの範囲内の場合
                    if (sub <= r * r && inIndex(pixcel, i + (int)vec.X, j + (int)vec.Y))
                    {
                        //始点と終点が初期値の場合
                        if (d1 == 0 && d2 == 360) pixcel[j + (int)vec.Y, i + (int)vec.X] = color;
                        else
                        {
                            //中点からの座標をもとにラジアンを求める
                            double st = Math.Atan2(i - rr, -j + rr);
                            //求めたラジアンが始点と終点の場合
                            if (dd1 <= st && st <= dd2)
                                pixcel[j + (int)vec.Y, i + (int)vec.X] = color;
                        }
                    }
                }
        }


        /// <summary>
        /// 円を描写する
        /// </summary>
        /// <param name="pixcel">描写する画像データ</param>
        /// <param name="vec">中点の座標</param>
        /// <param name="r">半径</param>
        /// <param name="color">色データ</param>
        /// <param name="d1">始点の角度</param>
        /// <param name="d2">終点の角度</param>
        public void circle(Vector2 vec, float r, Color color, float d1 = 0, float d2 = 360)
        {
            //Console.WriteLine(vec);
            circle(pixcel,vec - new Vector2(r,r),r, color,d1,d2);
        }

        /// <summary>
        /// 円を描写する
        /// </summary>
        /// <param name="pixcel">描写する画像データ</param>
        /// <param name="vec">中点の座標</param>
        /// <param name="r">半径</param>
        /// <param name="color">色データ</param>
        /// <param name="thick">線の太さ</param>
        /// <param name="d1">始点の角度</param>
        /// <param name="d2">終点の角度</param>
        public void circle(Vector2 vec, float r, Color color,int thick, float d1 = 0, float d2 = 360)
        {
            //直径を求める
            int R = (int)(r * 2);
            //円を描写する準備をする
            Color[,] pix = new Color[R + thick, R + thick];
            //大きい円を描く
            circle(pix,Vector2.Zero, r+thick*0.5f, color,d1,d2);
            //小さい円を描く
            circle(pix,new Vector2(thick, thick), r - thick*0.5f, Color.Transparent,d1,d2);
            //自身の画像データと合体させる
            attach(pix, vec - new Vector2(thick / 2f + r, thick / 2f + r));
        }

        /// <summary>
        /// 三角形を描写
        /// </summary>
        /// <param name="pixcel">描写する画像データ</param>
        /// <param name="a">三角形の点の座標</param>
        /// <param name="b">三角形の点の座標</param>
        /// <param name="c">三角形の点の座標</param>
        /// <param name="color">色データ</param>
        static public void triangle(Color[,] pixcel, Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            //3点の座標
            int[] vx0 = new int[] { (int)a.X, (int)b.X, (int)c.X };
            int[] vy0 = new int[] { (int)a.Y, (int)b.Y, (int)c.Y };
            //X軸をもとにソート
            Array.Sort(vx0, vy0);
            //最小最大のX座標を求める
            int maxX = vx0[2];
            int minX = vx0[0];
            //Y軸をもとにソート
            Array.Sort(vy0, vx0);
           
            
            for (int i = vy0[0]; i <= vy0[2]; i++)
                for (int j = minX; j <= maxX; j++)
                {
                    //座標が範囲外の場合
                    if (!inIndex(pixcel, j, i)) continue;
                    //三角形の線分を作る3点を求める
                    int[] vx = new int[] { vx0[0] - j, vx0[1] - j, vx0[2] - j };
                    int[] vy = new int[] { vy0[0] - i, vy0[1] - i, vy0[2] - i };
                    //外積を求める
                    bool[] o = new bool[]{
                        (vx[0] * vy[1] < vx[1] * vy[0]),
                        (vx[1] * vy[2] < vx[2] * vy[1]),
                        (vx[2] * vy[0] < vx[0] * vy[2]) };
                    //点が三角形以内にある場合
                    if (o[0] == o[1] && o[1] == o[2]) pixcel[i, j] = color;
                }

        }


        /// <summary>
        /// 三角形を描写
        /// </summary>
        /// <param name="pixcel">描写する画像データ</param>
        /// <param name="a">三角形の点の座標</param>
        /// <param name="b">三角形の点の座標</param>
        /// <param name="c">三角形の点の座標</param>
        /// <param name="color">色データ</param>
        public void triangle(Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            triangle(pixcel, a, b, c, color);
        }
        /// <summary>
        /// 三角形の線を描写
        /// </summary>
        /// <param name="a">三角形の点の座標</param>
        /// <param name="b">三角形の点の座標</param>
        /// <param name="c">三角形の点の座標</param>
        /// <param name="color">色データ</param>
        /// <param name="thick">線の太さ</param>
        public void triangle(Vector2 a, Vector2 b, Vector2 c, Color color,int thick)
        {
            //3本の線を描く
            straight(pixcel, a, b, color, thick);
            straight(pixcel, a, c, color, thick);
            straight(pixcel, b, c, color, thick);
        }

        /// <summary>
        /// 線分を描く
        /// </summary>
        /// <param name="a">線分の点</param>
        /// <param name="b">線分の点</param>
        /// <param name="color">色データ</param>
        /// <param name="thick">線の太さ</param>
        public void straight(Vector2 a, Vector2 b, Color color, int thick = 1)
        {
            straight(pixcel, a, b, color, thick);
        }

        /// <summary>
        /// 線分を描く
        /// </summary>
        /// <param name="pixcel">描写する画像データ</param>
        /// <param name="a">線分の点</param>
        /// <param name="b">線分の点</param>
        /// <param name="color">色データ</param>
        /// <param name="thick">線分の太さ</param>
        static public void straight(Color[,] pixcel, Vector2 a, Vector2 b, Color color, int thick = 1)
        {
            //線分の幅と高さ
            int width = (int)Math.Abs(b.X - a.X);
            int height = (int)Math.Abs(b.Y - a.Y);

            //線分の太さ分の円を準備する
            Color[,] pix = new Color[thick<<1, thick<<1];
            circle(pix, Vector2.Zero, thick, color);

            //横幅の方が大きい場合
            if (width >= height)
            {
                //Xが微小に変化するときの値
                float delX = (b.X - a.X) / width;
                float delY = (b.Y - a.Y) / width;

                for (int i = 0; i <= width; i++)
                {
                    //X軸をもとに線分の座標を求める
                    int x = (int)(a.X + i * delX);
                    int y = (int)(a.Y + i * delY);
                    
                    //求めた座標に円を描く
                    attach(pixcel, pix, new Vector2(x - thick, y - thick ));
                }
            }
            else
            {//縦幅の方が大きい場合

                //Yが微小に変化するときの値
                float delX = (b.X - a.X) / height;
                float delY = (b.Y - a.Y) / height;

                for (int i = 0; i <= height; i++)
                {
                    //Y軸をもとに線分の座標を求める
                    int x = (int)(a.X + i * delX);
                    int y = (int)(a.Y + i * delY);

                    //求めた座標に円を描く
                    attach(pixcel, pix, new Vector2(x - thick , y - thick ));
                }
            }

        }




        /// <summary>
        /// 画像を合体させる
        /// </summary>
        /// <param name="a">合体される画像データ</param>
        /// <param name="b">合体する画像データ</param>
        /// <param name="vec">座標</param>
        /// <param name="trans">透過を有効にするか</param>
        static public void attach(Color[,] a,Color[,] b, Vector2 vec,bool trans=true)
        {
            //各座標に対応した色データを入力する
            for (int i = 0; i < b.GetLength(0); i++)
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    //座標を求める
                    int x = (int)vec.X + j;
                    int y = (int)vec.Y + i;
                    //座標がaの範囲内か
                    if (inIndex(a, x, y))
                        //透過フラグがオンかつ色が透明な場合
                        if (trans && b[i, j] == Color.Transparent) continue;
                    　　//色データを入力
                        else a[y, x] = b[i, j];
                }
        }

        /// <summary>
        /// 画像を合体させる
        /// </summary>
        /// <param name="b">合体する画像データ</param>
        /// <param name="vec">座標</param>
        public void attach(Color[,] b, Vector2 vec)
        {
            attach(pixcel, b, vec);
        }



        /// <summary>
        /// 座標が画像の範囲内か
        /// </summary>
        /// <param name="x">座標</param>
        /// <param name="y">座標</param>
        bool inIndex (int x,int y)
        {
            //範囲内か返す
            return (0 <= x && 0 <= y && x < width && y < height);


        }
        /// <summary>
        /// 座標が画像の範囲内か
        /// </summary>
        /// <param name="a">画像データ</param>
        /// <param name="x">座標</param>
        /// <param name="y">座標</param>
        static bool inIndex(Color[,] a,int x, int y)
        {
            //画像の幅と高さを求める
            int width = a.GetLength(1);
            int height = a.GetLength(0);

            //範囲内か返す
            return (0 <= x && 0 <= y && x < width && y < height);


        }

        /// <summary>
        /// Colorの2次元配列からTexture2Dへ変換する
        /// </summary>
        public Texture2D ToTexture()
        {
            return ToTexture(pixcel);
        }

        /// <summary>
        /// Colorの2次元配列からTexture2Dへ変換する
        /// </summary>
        /// <param name="pixcel">画像のデータ</param>
        /// <returns></returns>
        static public Texture2D ToTexture(Color[,] pixcel) 
        {
            //画像の幅と高さを求める
            int width = pixcel.GetLength(1);
            int height = pixcel.GetLength(0);
            //1次元配列へ変換させる準備
            Color[] b = new Color[width * height];

            //1次元配列へ変換させる
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    b[i * width + j] = pixcel[i, j];
                }

            //Texture2Dを初期化
            Texture2D te = new Texture2D(graphics, width, height);
            //データをセット
            te.SetData(b);


            return te;
        }


    }
}
