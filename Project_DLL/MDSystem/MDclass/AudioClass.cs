using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace MDclass.AudioClass
{
    /// <summary>
    /// 音楽(BGM,SE)のデータをロードするときに用いるクラス
    /// </summary>
    public class SoundLoad
    {

        Game game;
        /// <summary>
        /// SoundNameANDLoad.cs の SoundLoadDetail の DetailLoadで設定された音データを一気にロードする
        /// </summary>
        static public void Load(Game game1)
        {
            SoundLoadDetail sound = new SoundLoadDetail();
            sound.game = game1;
            sound.DetailLoad();
            
        }
        /// <summary>
        /// SoundNameANDLoad.cs の SoundLoadDetail の DetailLoadで使用される関数
        /// <para>BGMとして設定される</para>
        /// <para>例：SetBGM(音データのファイル名,BGMの大きさ,音データのファイル名,BGMの大きさ,...)</para>
        /// <para>string : 音データのファイル名</para>
        /// <para>double : BGMの大きさ(<![CDATA[0<=x<=2]]>)</para>
        /// BGMの大きさは省略可(デフォルトで1となる)
        /// <para>例：SetBGM(音データのファイル名,音データのファイル名,BGMの大きさ,...)</para>
        /// </summary>
        protected void SetBGM(params object[] data)
        {
            Set(data, out Sound.bgms, out Sound.adjustB);

        }

        /// <summary>
        /// SoundNameANDLoad.cs の SoundLoadDetail の DetailLoadで使用される関数
        /// <para>SEとして設定される</para>
        /// <para>例：SetSE(音データのファイル名,SEの大きさ,音データのファイル名,SEの大きさ,...)</para>
        /// <para>string : 音データのファイル名</para>
        /// <para>double : SEの大きさ(<![CDATA[0<=x<=2]]>)</para>
        /// SEの大きさは省略可(デフォルトで1となる)
        /// <para>例：SetSE(音データのファイル名,音データのファイル名,SEの大きさ,...)</para>
        /// </summary>
        protected void SetSE(params object[] a)
        {
            Set(a, out Sound.ses, out Sound.adjustS);
            
        }

        /// <summary>
        /// SetBGM,SetSEで用いられる関数
        ///　<para>Soundクラスの各データを設定する</para>
        /// </summary>
        void Set(object[] a, out SoundEffect[] sounds, out double[] adjA)
        {
            List<SoundEffect> musics = new List<SoundEffect>();
            List<double> adj = new List<double>();

            for (int i = 0; i < a.Length; i += 2)
            {
                //ここで音データをロード
                musics.Add(game.Content.Load<SoundEffect>((string)a[i]));

                //音の大きさを省略しているかの判定
                if (i + 1 >= a.Length || (a[i + 1] is string))
                {
                    adj.Add(1f);
                    i--;
                    continue;
                }
                else adj.Add((double)a[i + 1]);
            }

            //データを登録
            sounds = musics.ToArray();
            adjA = adj.ToArray();

        }


    }

    /// <summary>
    /// BGMやSEを再生する。
    /// また、そのデータを返す。
    /// </summary>
    static public class Sound
    {
        /// BGMのデータ：bgms, adjustB
        /// SE のデータ：ses , adjustS
        internal static SoundEffect[] bgms, ses;
        internal static double[] adjustB, adjustS;

        /// <summary>
        /// BGMを再生
        /// </summary>
        /// <param name="name">SoundNameANDLoad.csで定義したBGMName</param>
        static public void Play(BGMName name) { Get(name).Play(); }
        /// <summary>
        /// SEを再生
        /// </summary>
        /// <param name="name">SoundNameANDLoad.csで定義したSEName</param>
        static public void Play(SEName name) { Get(name).Play(); }


        /// <summary>
        /// BGMのデータを返すよ
        /// </summary>
        /// <param name="name">SoundNameANDLoad.csで定義したBGMName</param>
        /// <returns></returns>
        static public SoundEffectInstance Get(BGMName name)
        {
            SoundEffectInstance instance = bgms[(int)name].CreateInstance();
            instance.IsLooped = true;
            instance.Volume = grap(0.5f * adjustB[(int)name], 0, 1);
            return instance;
        }

        /// <summary>
        /// SEのデータを返すよ
        /// </summary>
        /// <param name="name">SoundNameANDLoad.csで定義したSEName</param>
        /// <returns></returns>
        static public SoundEffectInstance Get(SEName name)
        {
            SoundEffectInstance instance = ses[(int)name].CreateInstance();
            instance.Volume = grap(0.5f * adjustS[(int)name], 0, 1);
            return instance;
        }

        /// <summary>
        /// xの値をmin以上max以下にする
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static float grap(double x, float min = -1, float max = 1)
        {
            if (x < min) x = min;
            else if (max < x) x = max;
            return (float)x;
        }

    }

}
