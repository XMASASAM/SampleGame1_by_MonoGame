using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MDclass.GeometricClass;
using MDclass.ImageAndSprite;
using MDclass.ScreenClass;
using MDclass.LabelClass;
using MDclass.VariationClass;
using MDclass.CollisionClass;
using MDclass.TouchClass;
using MDclass.Write;
namespace MDclass.LoadClass
{
    /// <summary>
    /// 様々なアセットを一気にロードし、
    /// このライブラリの初期設定も行うクラス
    /// </summary>
    static public class LoadClass
    {
        /// <summary>
        /// 様々なアセットを一気にロード&MDclassの初期設定も行う
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphics"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="font">フォント</param>
        /// <param name="ScreenW">画面の幅</param>
        /// <param name="ScreenH">画面の高さ</param>
        static public void Load(Game game,GraphicsDeviceManager graphics,SpriteBatch spriteBatch,SpriteFont font,int ScreenW,int ScreenH)
        {

            Geometric.Load(game.GraphicsDevice);
            ImageLoad.Load(game);
            SpriteFont font1 = font;
            Label.Load(font1);
            Screen.Set(graphics,spriteBatch, ScreenW, ScreenH);
            WriteString.Load(font1);
           

        }

        /// <summary>
        /// 一気に更新する
        /// </summary>
        static public void Update(GameTime gameTime,bool mobile)
        {
            Screen.Update();
            Variation.Update(gameTime);
            TouchManager.Update(gameTime,mobile);
            SpriteManager.UpDate(gameTime);
            CollisionField.Update();
            SpriteManager.CollisionUpdate();

        }
        /// <summary>
        /// 一気に描写する
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="spriteSortMode"></param>

        static public void Draw(SpriteBatch _spriteBatch,SpriteSortMode spriteSortMode)
        {
            Screen.Begin(spriteSortMode);
            SpriteManager.Draw(_spriteBatch);
            WriteString.Draw(_spriteBatch);
            Screen.End();
        }

    }
}
