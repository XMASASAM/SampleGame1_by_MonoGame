using System;
using System.Collections.Generic;
using System.Text;
using MDclass.CollisionClass;
using Microsoft.Xna.Framework;
using MDclass.ImageAndSprite;
namespace ActionGame_1_share
{
    class Wall : Sprite
    {
        CollisionRectangle coRec;
        CollisionField field;
        public Wall(CollisionField field,Rectangle rec):base(ImageName.White)
        {
            this.field = field;
            Scale = new Vector2(rec.Width, rec.Height);
            Point = new Vector2(rec.X, rec.Y);
            Depth = 0.4f;
            

            coRec = new CollisionRectangle(CollisionName.BackGround, Vector2.Zero,Vector2.Zero, rec.Width, rec.Height);
            coRec.Point = Point;
            field.Set(coRec);

            var rn = new System.Random();
            var r = rn.Next(0,255);
            var g = rn.Next(0,255);
            var b = rn.Next(0,50);
            this.Color = new Color(r, g, b);

        }

        public override void Delete()
        {
            base.Delete();
            field.Delete(coRec);

        }




    }
}
