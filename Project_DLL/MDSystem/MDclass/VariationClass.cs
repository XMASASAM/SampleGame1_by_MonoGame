using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MDclass.ImageAndSprite;
namespace MDclass.VariationClass
{
    /// <summary>
    /// 値を指定時間後に変化させる
    /// </summary>
    public class Variation
    {
        /// <summary>
        /// 管理する配列
        /// </summary>
        static List<Variation> Variations = new List<Variation>();

        static GameTime GameTime;

        //目標の値
        protected readonly object[] _obj;
        //待機時間
        protected readonly int[] _times;
        //残りのループ回数 , 設定されたループ回数
        protected int _loop, Mloop;
        //初期の値
        protected object Mfirst;
        //次のステップへの閾値
        protected int _threshold;
        //停止中
        public bool isStop = false;
        //無限ループ
        public readonly bool isInfinite;
        //現在のパラーメータの要素番号
        protected int _current = 0;
        //1を閾値で割ったもの
        protected float _div;
        //変化が完了したか 
        bool completion = false;//, active = false;
        //次の変化へ
        Variation next = null;

        /// <summary>
        /// 値を徐々に変化させる。
        /// ループ回数が0の場合無限ループする。
        /// 待機時間が負数の場合値は徐々に変化する。
        /// インスタンスを作った時点から変化が始まる。
        /// <para>例：Variation(初期値,ループ回数,待機時間1,目標値1,待機時間2,目標値2,...)</para>
        /// </summary>
        /// <param name="first">初期値</param>
        /// <param name="loop">ループ回数</param>
        /// <param name="Time_Instance">待機時間/目標値</param>
        protected Variation(object first, int loop, params object[] Time_Instance)
        {
            _loop = loop;
            Mloop = loop;
            Mfirst = first;

            //ループ回数が0の場合無限ループ
            if (_loop == 0) isInfinite = true;

            //待機時間と目標値の準備
            _times = new int[Time_Instance.Length >> 1];
            _obj = new object[Time_Instance.Length >> 1];

            //各値をセット
            for (int i = 0; i < Time_Instance.Length; i += 2)
            {
                _times[i >>1] = (int)Time_Instance[i];
                _obj[i >>1] = Time_Instance[i + 1];
            }
            Start();
        }



        public static void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            //要素が削除される可能性があるため、コピーする
            List<Variation> temp = new List<Variation>(Variations);
            //各要素をアップデート
            foreach (Variation i in temp) i.Update();
        }



        void Update()
        {
            //停止している場合終了
            if (isStop) return;

            //閾値が0になるまで引く
            _threshold -= GameTime.ElapsedGameTime.Milliseconds;

            //閾値が0以下の場合次のステップへ
            if (_threshold <= 0)
            {
                //次要素番号
                int nextcurrent = (_current + 1) % _times.Length;

                //一周した場合 かつ 無限ループでない場合
                if (nextcurrent == 0 && (!isInfinite) && ((--_loop) <= 0))
                {
                    //処理完了
                    completion = true;
                    //管理リストから切り離す
                    Delete();
                    //次の変化へ
                    if (next != null) next.Start();// next.isStop = false;

                    //return;
                }

                //変化を設定
                Set(_obj[_current], _obj[nextcurrent]);
                //閾値を設定
                _threshold = Math.Abs(_times[nextcurrent]);
                //割り算時の値を保持
                _div = 1f / _threshold;

                //次の要素番号
                _current = nextcurrent;

            }
            else 
            //待機時間が負数の場合
            if (_times[_current] < 0)
            {
                //徐々に変化させる
                Change(1f - _threshold * _div);
            }




        }

        /// <summary>
        /// 値の変化の準備をさせる
        /// </summary>
        /// <param name="a">今の値</param>
        /// <param name="b">次の値</param>
        protected virtual void Set(object a, object b) { }

        /// <summary>
        /// 目標値までa%となるように変化させる
        /// </summary>
        /// <param name="a">百分率</param>
        protected virtual void Change(float a) { }


        /// <summary>
        /// 値を最初の状態から変化させる。
        /// 実行途中でも最初からになる。
        /// </summary>
        public void Start()
        {
            isStop = false;
            completion = false;
            _loop = Mloop;
            //型に応じた初期セット
            Set(Mfirst, _obj[0]);
            //最初の閾値を設定
            _threshold = Math.Abs(_times[0]);
            //処理を少し早くするため閾値の割り算を保持
            _div = 1f / _threshold;
            //管理リストへ追加
            if(!isActive)Variations.Add(this);
        }


        /// <summary>
        /// 管理リストから切り離す
        /// </summary>
        public void Delete()
        {
            Variations.Remove(this);
        }
        /// <summary>
        /// 管理リストに存在しているか
        /// </summary>
        public bool isActive { get { return Variations.Contains(this); } }
        /// <summary>
        /// 処理が完了しているか
        /// </summary>
        public bool isCompletion { get { return completion; } }


        /// <summary>
        /// 処理が完了したときに続いて実行されるもの
        /// </summary>
        public Variation Next
        {
            get { return next; }
            set
            {
                next = value;
                next.isStop = true;
            }
        }

    }

    /// <summary>
    /// 画像を指定時間後変化させる
    /// </summary>
    public class VariationImage : Variation
    {
        //目的の値
        Image target;
        /// <summary>
        /// 画像を指定時間後変化させる
        /// </summary>
        /// <param name="first">初期の画像</param>
        /// <param name="loop">ループ回数</param>
        /// <param name="Time_Instance">待機時間 , Image</param>
        public VariationImage(Image first, int loop, params object[] Time_Instance) : base(first, loop, Time_Instance) { }

        /// <summary>
        ///　Imageを設定
        /// </summary>
        /// <param name="current">現在のデータ</param>
        /// <param name="next">次のデータ</param>
        protected override void Set(object current, object next)
        {
            target = next as Image;
        }

        /// <summary>
        /// 現在の値を出力する
        /// </summary>
        public Image Target { get { return target; } }

    }

    /// <summary>
    /// Vector2を変化させる
    /// </summary>
    public class VariationVector2 : Variation
    {
        //目的の値 , ひとつ前の値 , 値の幅
        Vector2 target, old, dis;

        /// <summary>
        /// \vector2を指定時間後変化させる
        /// </summary>
        /// <param name="first">初期のVector2</param>
        /// <param name="loop">ループ回数</param>
        /// <param name="Time_Instance">待機時間 , Vector2</param>
        public VariationVector2(Vector2 first, int loop, params object[] Time_Instance) : base(first, loop, Time_Instance) { }


        /// <summary>
        /// 値の変化の準備をさせる
        /// </summary>
        /// <param name="a">今の値</param>
        /// <param name="b">次の値</param>
        protected override void Set(object current, object next)
        {
            target = (Vector2)current;
            old = (Vector2)current;
            dis = Vector2.Add((Vector2)next, -old);
        }


        /// <summary>
        /// 目標値までa%となるように変化させる
        /// </summary>
        /// <param name="a">百分率</param>
        protected override void Change(float a)
        {
            target = Vector2.Add(old, Vector2.Multiply(dis, a));
        }

        /// <summary>
        /// 現在の値を出力する
        /// </summary>
        public Vector2 Target { get { return target; } }

    }


    public class VariationColor : Variation
    {
        //目的の値
        Color target;
        //ひとつ前の値 , 値の幅
        Vector4 old, dis;

        public VariationColor(Color first, int loop, params object[] Time_Instance) : base(first, loop, Time_Instance) { }

        /// <summary>
        /// 値の変化の準備をさせる
        /// </summary>
        /// <param name="a">今の値</param>
        /// <param name="b">次の値</param>
        protected override void Set(object current, object next)
        {
            old = ((Color)current).ToVector4();
            target = Color.FromNonPremultiplied(old);
            dis = Vector4.Add(((Color)next).ToVector4(), -old);
        }

        /// <summary>
        /// 目標値までa%となるように変化させる
        /// </summary>
        /// <param name="a">百分率</param>
        protected override void Change(float a)
        {
            target = Color.FromNonPremultiplied(Vector4.Add(old, Vector4.Multiply(dis, a)));

        }

        /// <summary>
        /// 現在の値を出力する
        /// </summary>
        public Color Target { get { return target; } }

    }


    public class VariationFloat : Variation
    {
        //目的の値 , ひとつ前の値 , 値の幅
        float target, old, dis;

        public VariationFloat(float first, int loop, params object[] Time_Instance) : base(first, loop, Time_Instance) { }


        /// <summary>
        /// 値の変化の準備をさせる
        /// </summary>
        /// <param name="a">今の値</param>
        /// <param name="b">次の値</param>
        protected override void Set(object current, object next)
        {
            old = (float)current;
            target = old;
            dis = (float)next - old;
        }

        /// <summary>
        /// 目標値までa%となるように変化させる
        /// </summary>
        /// <param name="a">百分率</param>
        protected override void Change(float a)
        {
            target = old + dis * a;
        }


        /// <summary>
        /// 現在の値を出力する
        /// </summary>
        public float Target { get { return target; } }

    }

    public class VariationString : Variation
    {
        //目的の値
        string target;
        public VariationString(string first, int loop, params object[] Time_Instance) : base(first, loop, Time_Instance) { }

        /// <summary>
        /// 値の変化の準備をさせる
        /// </summary>
        /// <param name="a">今の値</param>
        /// <param name="b">次の値</param>
        protected override void Set(object current, object next)
        {
            target = next as string;
        }

        /// <summary>
        /// 現在の値を出力する
        /// </summary>
        public string Target { get { return target; } }

    }


}
