using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MDclass.ScreenClass
{
    /// <summary>
    /// 画面を整えるクラス
    /// </summary>
    static public class Screen
    {
        //画面の倍率
        static public Matrix scale;
        //1を倍率で割ったもの
        static public Vector2 subscale;
        //描写するときの画面
        static Viewport viewPort;
        //動作している環境の画面情報
        static Viewport virtialViewPort;

        static GraphicsDeviceManager graphicsDevice;
        static SpriteBatch spriteBatch;

        //自分で設定した画面
        static Rectangle view;
        //機種ごとに設定されている初期値
        static Vector2 firstV;
        //画面の幅と高さ
        static int Width1,Height1;


        /// <summary>
        /// 画面を設定
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="sprite"></param>
        /// <param name="Width">画面の幅</param>
        /// <param name="Height">画面の高さ</param>
        static public  void Set(GraphicsDeviceManager graphics,SpriteBatch sprite,int Width,int Height)
        {
            //画面の情報を設定
            view = new Rectangle(0,0,Width1,Height1);
            Width1 = Width;
            Height1 = Height;
            graphicsDevice = graphics;
            spriteBatch = sprite;

            float sub1, sub2, subScale;
                
            int firstX, firstY,dW,dH;

            //機種ごとに設定されている初期値
            firstX = graphics.GraphicsDevice.Viewport.X;
            firstY = graphics.GraphicsDevice.Viewport.Y;
            
            //画面の大きさ
            dW = graphics.GraphicsDevice.Viewport.Width  +firstX*2;
            dH = graphics.GraphicsDevice.Viewport.Height +firstY*2;

            //動作している環境の画面情報
            virtialViewPort = new Viewport(0, 0,Width1, Height1);
            

            //倍率
            sub1 = (float)(dW) / (Width1);
            sub2 = (float)(dH) / (Height1);
            //低い方の倍率を基準に整形
            subScale = Math.Min(sub1, sub2);

            //マトリックスを生成
            scale = Matrix.CreateScale(subScale);

            //アスペクト比を保ちながら最大限まで画面を拡大する
            if (sub1 > sub2)
                //高さ基準
                viewPort = new Viewport(
                    (int)((dW - Width1 * subScale) / 2.0),
                    0,
                    (int)(Width1 * subScale),
                    dH
                    );
            
            else
                //幅基準
                viewPort = new Viewport(
                    0,
                    (int)((dH - Height1 * subScale) / 2.0),
                    dW,
                    (int)(Height1 * subScale)
                    );

            //画面の設定
            graphicsDevice.GraphicsDevice.Viewport = virtialViewPort;
            graphics.ApplyChanges();


            //1を倍率で割ったもの
            subscale = new Vector2(1/subScale);
            //調整用
            firstV = new Vector2(firstX - viewPort.X, firstY - viewPort.Y);

        }


        static public void Update()
        {
            //一応デフォルトの画面情報へ切り替える
            graphicsDevice.GraphicsDevice.Viewport = virtialViewPort;
        }

        /// <summary>
        /// 画面を適切に拡大してスプライトを表示する
        /// </summary>
        /// <param name="spriteSortMode">奥行のモード</param>
        static public void Begin(SpriteSortMode spriteSortMode)
        {
            //描写用の画面へ切り替える
            graphicsDevice.GraphicsDevice.Viewport = viewPort;
            //スプライトの描写開始
            spriteBatch.Begin(spriteSortMode,null,null,null,null,null, scale);
        }
        
        static public void End()
        {
            //スプライトの描写完了
            spriteBatch.End();
            //一応デフォルトの画面情報へ切り替える
            graphicsDevice.GraphicsDevice.Viewport = virtialViewPort;
        }


        static public Matrix GetMatrix { get { return scale; } }

        static public Vector2 Get_divscale { get { return subscale; } }

        static public Rectangle Get_view { get { return view; } }

        static public int Width { get { return Width1; } }
        static public int Height { get { return Height1; } }

        /// <summary>
        /// 動作している環境のスクリーン座標からこのゲームのスクリーン座標へ変換する
        /// </summary>
        static public Vector2 ExportVector(Vector2 point)
        {
            return (point + firstV) * subscale;
        }

    }
}
