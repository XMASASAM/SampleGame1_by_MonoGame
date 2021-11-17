using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace ActionGame_1_share
{
    static public class MagicNumber
    {
        public const int ScreenWidth = 400*2;//375;
        public const int ScreenHeight = 240*2;//667 ;
        

        public const float Sprite_Scale = 1/2f;

        static public readonly Vector2 ScreenFrame;
        static public readonly bool Mobile;
        static MagicNumber()
        {
#if __MOBILE__
            Mobile = true;
#else
            Mobile = false;
#endif

            ScreenFrame = new Vector2(ScreenWidth, ScreenHeight);
            
        }
        public static int sendInt;
    }
}
