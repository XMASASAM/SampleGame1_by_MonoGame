using Microsoft.Xna.Framework.Content.Pipeline;

using TInput = System.String;
using TOutput = System.String;
using System.Collections.Generic;
namespace Read
{
    [ContentProcessor(DisplayName = "TextProcessor")]
    class Processor1 : ContentProcessor<TInput[], TOutput[]>
    {
        public override TOutput[] Process(TInput[] input, ContentProcessorContext context)
        {
            List<TInput> send = new List<TInput>();

            foreach (TInput i in input) if (!System.String.Equals(i, System.String.Empty))
                {
                    string[] j = i.Split(new char[] { ',',' ' });
                    foreach (string k in j)if(!string.Equals(string.Empty,k)) send.Add(k);
                    
                }
            

            return send.ToArray();
        }
    }
}
