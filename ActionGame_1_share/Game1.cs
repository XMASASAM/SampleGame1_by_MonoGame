using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MDclass.LoadClass;
using MDclass.Write;
using MDclass.TouchClass;
using MDclass.CollisionClass;
using MDclass.ImageAndSprite;
using TextRead;
using MDclass.GeometricClass;
namespace ActionGame_1_share
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
#if !__MOBILE__
            _graphics.PreferredBackBufferWidth = MagicNumber.ScreenWidth;
            _graphics.PreferredBackBufferHeight = MagicNumber.ScreenHeight;
            _graphics.ApplyChanges();
#endif
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpriteFont font = Content.Load<SpriteFont>("File2");
            LoadClass.Load(this, _graphics, _spriteBatch,font,MagicNumber.ScreenWidth,MagicNumber.ScreenHeight );
            TouchView touchView = new TouchView();
            TouchManager.SetView(touchView);
            controller = new Controller(touchView, 5, 100);

            
            Text out1 = Content.Load<Text>("map_room0");
            map = new Map(out1);
            man = new Hunam(map,map.GetCollisionField,map.GetStartPoint,controller);

        }
        Controller controller;
        Hunam man;

        Map map;
        protected override void Update(GameTime gameTime)
        {

            LoadClass.Update(gameTime, false);
            controller.Update(gameTime);
            map.Update(gameTime,man.Point);
            setFPS(gameTime);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            LoadClass.Draw(_spriteBatch, SpriteSortMode.BackToFront);


            base.Draw(gameTime);
        }


        void setFPS(GameTime gameTime)
        {
            float fps = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            WriteString.Write("FPS", "[FPS]:" + fps.ToString(), 0, 0);
        }
        
    }

   
}
