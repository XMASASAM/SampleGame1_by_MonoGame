using System;

namespace MDclass.AudioClass
{
    /// <summary>
    /// BGMの名前
    /// </summary>
    public enum BGMName
    {
        isekai=0
    }

    /// <summary>
    /// SEの名前
    /// </summary>
    public enum SEName
    {
        water2=0
    }


    class SoundLoadDetail:SoundLoad
    {
        /// <summary>
        /// どんな音データをロードするのかをここで設定
        /// </summary>
        public void DetailLoad()
        {
            
       //   SetBGM("isekai",0.2);
       //   SetSE("water2");
        }
    }


}
