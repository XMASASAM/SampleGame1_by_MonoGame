using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MDclass.GeometricClass;
namespace MDclass.ImageAndSprite
{
    /// <summary>
    /// 表示したい画像の名前
    /// </summary>
    public enum ImageName 
    {
        White,
        Controller_Circle,
        Controller_Wave,
        People1_Down,
        People1_Right=People1_Down+3,
        People1_Up=People1_Right+3,
        People1_Left=People1_Up+3,
        BG_Wood_yellow = People1_Left+3,
        BG_Wood_Brown,
        BG_Wood_Ceiling
    }


    class ImageLoadDetail:ImageLoad
    {
        /// <summary>
        /// どんな画像データをロードするのかをここで設定
        /// </summary>
        public void DetailLoad()
        {
            //Controller_Circle
            Geometric paper = new Geometric(100, 100);
            paper.circle(new Vector2(50, 50), 50, Color.White);
            paper.circle(new Vector2(50, 50), 46, Color.Black, 8);
            SetTextureImage(paper.ToTexture());

            //Controller_Wave
            paper = new Geometric(100, 100);
            paper.circle(new Vector2(50, 50), 44,new Color(255,255,255), 5);
            for (int i = 0; i < 5; i++)
                paper.circle(new Vector2(50, 50), 47 - i, new Color(51 * i, 51 * i, 51 * i, 0), 1);
            SetTextureImage(paper.ToTexture());


            #region Common_People
            PageAdd("CommonPeople1_3");
            WidthHeight(48, 48, 3, 3);
            PageAdd( Geometric.ToTexture(
                    Geometric.InversionH(
                        Geometric.Cutout(
                            Geometric.GetColors(CurrentPage()),
                            new Rectangle(0,48,144,48)
                        )
                    )
                  ) 
             );
            WidthHeight(48, 48, 3, 1,true);

            #endregion

            PageAdd("BG");
            WidthHeight(48, 48, 3, 1);


        }

        void WidthHeight(int width,int height,int x,int y,bool re=false)
        {
            for (int j = 0; j < y; j++) for (int i = 0; i < x; i++)
                    if(re)
                        ImageAdd(new Rectangle((x-i-1) * width, j * height, width, height));
                    else
                        ImageAdd(new Rectangle(i * width, j * height, width, height));

        }

        void SetTextureImage(Texture2D page)
        {
            PageAdd(page);
            ImageAdd(new Rectangle(0, 0, page.Width, page.Height));
        }


    }

}
