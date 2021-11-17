using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using TRead = TextRead.Text;

namespace TextRead
{
    public class Reader1 : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            int len = input.ReadInt32();
            string[] send = new string[len];

            for (int i = 0; i < len; i++)
                send[i] = input.ReadString();

            return new TRead(send);
        }
    }
}
