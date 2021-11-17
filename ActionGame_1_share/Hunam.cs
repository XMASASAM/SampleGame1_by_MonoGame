using System;
using System.Collections.Generic;
using System.Text;
using MDclass.ImageAndSprite;
using Microsoft.Xna.Framework;
using MDclass.Write;
using MDclass.CollisionClass;
namespace ActionGame_1_share
{
    class Hunam : MapSprite
    {
        readonly ImageName[] oriImage = new ImageName[] {ImageName.People1_Up,ImageName.People1_Right,ImageName.People1_Down,ImageName.People1_Left};
        readonly Controller controller;
        Vector2 deltaVector=Vector2.Zero;
        Vector2 oldVector,oldMax,oldMin;
        readonly CollisionRectangle coRec;
        Vector2 maxRange;
        public Hunam(Map map,CollisionField field,Vector2 point,Controller controller) : base(ImageName.People1_Down,map)
        {
            //   _point = point;
            maxRange = new Vector2(map.Width, map.Height);
            _depth = 0.3f;
            oldOri = Orientation.South;
            Origin = new Vector2(Width >> 1, Height >> 1);
            this.controller = controller;
            

            Rectangle rec = this.Image.ToRectangle;
            coRec = new CollisionRectangle(CollisionName.None,Origin,new Vector2(3,rec.Height>>1),rec.Width-6,rec.Height>>1);
         //   coRec.Point = Point;
            field.Set(coRec,new List<CollisionShape>());

            Point = point;
            System.Diagnostics.Debug.WriteLine("kkkk:" + Point+point);
            oldVector = Point;

            

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            self_move(gameTime);
        }


        public override void CollisionUpdate()
        {
            base.CollisionUpdate();


            foreach (var i in coRec.HitList)
            {

                if (i.GetLabel() == CollisionName.BackGround)
                    self_collision(i);
            }
            


        }

        Orientation oldOri;
        void self_move(GameTime gameTime)
        {
            //if (isHitting) return;
            deltaVector = 3 * controller.DeltaDistance * (float)gameTime.ElapsedGameTime.TotalSeconds;
            oldVector = Point;
            oldMax = coRec.GetMax;
            oldMin = coRec.GetMin;
            Point += deltaVector;
            
            Orientation nowOri = controller.GetOritentation();

            if (nowOri == Orientation.None)
            {
                if (oldOri != Orientation.None)
                {
                    AnimDelate(AnimName.Image);
                    this.Image = Image.Get(oriImage[(int)oldOri]);
                }

            }
            else if (nowOri != oldOri)
            {
                var sub = oriImage[(int)nowOri];
                var subTime = 100;
                AnimDelate(AnimName.Image);
                Anim(AnimName.Image, 0, subTime, Image.Get(sub, 1), subTime, Image.Get(sub, 0), subTime, Image.Get(sub, 2), subTime, Image.Get(sub));

            }

            oldOri = nowOri;
        }


        void self_collision(CollisionShape A )
        {
            if (!Collision.Check(coRec, A)) return;

            Vector2 sub = deltaVector;
               if (A is CollisionRectangle){

                   if (oldMax.Y <= A.GetMin.Y) sub.Y = sub.Y - (coRec.GetMax.Y - A.GetMin.Y) - 1;
                   else if (oldMin.Y >= A.GetMax.Y) sub.Y = sub.Y + (A.GetMax.Y - coRec.GetMin.Y) + 1;
                   else if (oldMax.X <= A.GetMin.X) sub.X = sub.X - (coRec.GetMax.X - A.GetMin.X) - 1;
                   else sub.X = sub.X + (A.GetMax.X - coRec.GetMin.X) + 1;
                   Point = oldVector + sub;
               }
            
            Point = oldVector + sub;

            deltaVector = Point - oldVector;
            
        }

        public override Vector2 Point
        {
            get { return mapPoint; }
            set
            {
                mapPoint = Vector2.Clamp(value, Vector2.Zero, maxRange);

                coRec.Point = mapPoint;
            }
        }

    }
}
