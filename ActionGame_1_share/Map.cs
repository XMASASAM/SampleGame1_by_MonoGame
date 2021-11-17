using System;
using System.Collections.Generic;
using System.Text;
using TextRead;
using Microsoft.Xna.Framework.Graphics;
using MDclass.ImageAndSprite;
using MDclass.GeometricClass;
using Microsoft.Xna.Framework;
using MDclass.CollisionClass;
namespace ActionGame_1_share
{

    class MapSprite : Sprite
    {
        protected Vector2 mapPoint;
        public MapSprite(ImageName name,Map map) : base(name)
        {
            map.AddSprite(this);
        }

        public void SetLocate(Vector2 camera)
        {
            _point = Point +camera;
        }
    }
    class Map
    {

        Texture2D mapTexture;
        Sprite background;
        Vector2 camera = Vector2.Zero;
        Vector2 mapVec = Vector2.Zero;
        Vector2 halfFrame;
        Vector2 startPoint;
        bool widthIsWide = false;
        bool heightIsWide = false;
        List<MapSprite> sprites = new List<MapSprite>();
        List<CollisionShape> collisions = new List<CollisionShape>();
        Vector2 scale = new Vector2(48, 48);
        public int Width { get { return background.Width; } }
        public int Height { get { return background.Height; } }

        CollisionField collisionField;

        class MapChip
        {
            public ImageName image;
            public char key;
            public string value1;

            public bool isHit { get { return value1 == "hit"; } }
        }

        public Map(Text text)
        {
            string[] data = text.GetStrings();
            Dictionary<char, MapChip> putData = new Dictionary<char, MapChip>();

            int now = 0;
            int step = 3;
            for(now=0;data[now] != "-1";now+=step)
            {
                MapChip chip = new MapChip()
                {
                    image = (ImageName)Enum.Parse(typeof(ImageName), data[now + 1]),
                    value1 = data[now + 2],
                    key = data[now][0]
                };
                putData.Add(chip.key,chip);

            }

            List<string> mapdata = new List<string>();

            for (now++; data[now] != "-1"; now++)
                mapdata.Add(data[now]);

            int width = mapdata[0].Length;
            int height = mapdata.Count;

            Geometric map = new Geometric(width * 48, height * 48);
            int[,] kasann = new int[height,width];
            bool[,] finishhit = new bool[height, width];
            MapChip[,] mapchips = new MapChip[height, width];
            for (int y = 0; y < height; y++)
            {
                var sub = putData[ mapdata[y][0]].isHit;
                for (int x = 0; x < width; x++)
                {
                    MapChip current = putData[mapdata[y][x]];
                    var sub2 = current.isHit;
                    if (sub.Equals(sub2)&&x>=1) kasann[y, x] = kasann[y, x - 1];
                    else if(x>=1)kasann[y,x]= kasann[y, x-1]+1;
                    finishhit[y, x] = false;
                    sub = sub2;

                    if (current.value1 == "start") startPoint = new Vector2(x, y)*scale;

                    map.attach(Image.Get(current.image).ToColors(), new Vector2(x * 48, y * 48));
                    mapchips[y, x] = current;
                }
            }


            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    MapChip current = mapchips[y,x];
                    


                    if (current.isHit&&!finishhit[y, x])
                    {
                        int yy;
                        int xx;
                        for(xx = x + 1; xx<width&&mapchips[y, xx].isHit; xx++)
                        {
                            finishhit[y,xx] = true;
                        }
                        xx--;

                        for (yy = y + 1; 
                            yy < height && mapchips[yy, x].isHit && 
                            kasann[yy, x] == kasann[yy, xx]; 
                            yy++)
                        {
                            for (int xxx = x; xxx <= xx; xxx++) finishhit[yy, xxx] = true;
                        }
                        yy--;

                        collisions.Add(new CollisionRectangle(CollisionName.BackGround, Vector2.Zero,Vector2.Zero, (int)((xx - x + 1) * scale.X), (int)((yy - y + 1) * scale.Y))
                        {
                            Point = new Vector2(x, y) * scale
                        });
                    }
                    finishhit[y, x] = true;
                }
            }




            mapTexture = map.ToTexture();

            background = new Sprite(mapTexture);
            background.Depth = 0.5f;

            mapVec.X = Math.Max( (MagicNumber.ScreenWidth - mapTexture.Width)*0.5f,0);
            mapVec.Y = Math.Max( (MagicNumber.ScreenHeight - mapTexture.Height)*0.5f,0);
            widthIsWide = mapVec.X <= 0.0;
            heightIsWide = mapVec.Y <= 0.0;

            halfFrame = MagicNumber.ScreenFrame * 0.5f;

            collisionField = new CollisionField(Width, Height);
            foreach (CollisionShape i in collisions)
                collisionField.Set(i);

        }

        public Texture2D GetTexture2D { get { return mapTexture; } }

        public void AddSprite(MapSprite sprite)
        {
            sprites.Add(sprite);
        }


        public void Update(GameTime gameTime,Vector2 point)
        {
            Vector2 sub=Vector2.Zero;

            if (widthIsWide)
            {
                if (background.Width - halfFrame.X < point.X)
                {
                    sub.X = (MagicNumber.ScreenWidth) - background.Width;
                }
                else
                if (point.X > halfFrame.X)
                {
                    sub.X = halfFrame.X - point.X;
                }
            }


            if (heightIsWide)
            {
                if (background.Height - halfFrame.Y < point.Y)
                {
                    sub.Y =  (MagicNumber.ScreenHeight) - background.Height;
                }
                else
                if (point.Y > halfFrame.Y)
                {
                    sub.Y = halfFrame.Y - point.Y;
                }
            }



            background.Point = sub+mapVec;
            foreach (var i in sprites) i.SetLocate(background.Point);
        }

        public void AddCollision(CollisionShape collision)
        {
            collisionField.Set(collision);
        }

        public CollisionField GetCollisionField { get { return collisionField; } }
        public Vector2 GetStartPoint { get { return startPoint; } }
    }
}
