
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using TWrite = System.String;

namespace Read
{
    [ContentTypeWriter]
    public class Writer1 : ContentTypeWriter<TWrite[]>
    {
        protected override void Write(ContentWriter output, TWrite[] value)
        {
            output.Write(value.Length);
            foreach (System.String i in value) output.Write(i);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "TextRead.Reader1, TextRead";
        }
    }
}
