using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Linq;
using MDclass.CollisionClass;
using MDclass.ScreenClass;
namespace MDclass.TouchClass
{

    /// <summary>
    /// �^�b�`�����Ǘ�����
    /// </summary>
    static public class TouchManager
    {
        //�^�b�`��
        static int touchnum = 0;
        
        //�^�b�`���
        static Dictionary<int, TouchInfo> Tdict = new Dictionary<int, TouchInfo>();
        //�������ܐݒ肳��Ă���^�b�`���
        static TouchView panel;
        //�O��^�b�`���ꂽ��
        static bool preFlag = false;

        /// <summary>
        /// �^�b�`��ʂ�ݒ肷��
        /// </summary>
        /// <param name="view">�^�b�`���</param>
        static public void SetView(TouchView view)
        {
            
            if (view == null) return;
            //���ɐݒ肳��Ă����ꍇ
            if (panel != null) panel.active = false;

            //�ݒ�
            panel = view;
            panel.active = true;
        }

        static public void Update(GameTime gameTime,bool mobile)
        {
            //TouchList�p
            touchs = null;

            //���s�������o�C���̏ꍇ
            if (mobile)
            {
                //�^�b�`�����擾
                TouchCollection touches = TouchPanel.GetState();
                //������
                Dictionary<int, TouchInfo> temp = new Dictionary<int, TouchInfo>();

                foreach (TouchLocation i in touches)
                {
                    //�V�����^�b�`�����ꍇ
                    if (!Tdict.ContainsKey(i.Id))
                        //�^�b�`����ǉ�����
                        Tdict.Add(i.Id, SetT(new TouchInfo(i.Position, i.Id)));

                    //ID��������擾
                    TouchInfo current = Tdict[i.Id];
                    //�^�b�`�����X�V
                    current.Update(i.Position, gameTime);
                    //���݂��m�F���ꂽ�^�b�`����ǉ�
                    temp.Add(i.Id, current);
                }

                //���݂��m�F����Ȃ������^�b�`�����폜
                foreach (TouchInfo i in Tdict.Values.Except(temp.Values))
                    i.Delete();

                Tdict = temp;
            }
            else
            {
                //���s�����p�\�R���̏ꍇ

                //�}�E�X�̏��
                MouseState mouseState;
                mouseState = Mouse.GetState();

                //���N���b�N������
                bool touchFlag = mouseState.LeftButton == ButtonState.Pressed;

                //�N���b�N�����ꍇ
                if (touchFlag)
                {
                    //�O��N���b�N���Ȃ������ꍇ
                    if (!preFlag)
                    {

                        touchnum++;
                        Tdict.Add(touchnum, SetT(new TouchInfo(mouseState.Position.ToVector2(), touchnum)));

                        preFlag = true;
                    }
                }
                else
                {
                    //�N���b�N���Ȃ������ꍇ
                    if (Tdict.Count > 0)
                    {
                        //�^�b�`�����폜
                        Tdict[touchnum].Delete();
                        Tdict.Remove(touchnum);
                        preFlag = false;
                    }
                }

                //�^�b�`��񂪑��݂��Ă���ꍇ�X�V����
                if (Tdict.Count > 0) Tdict[touchnum].Update(mouseState.Position.ToVector2(), gameTime);


            }
            //�^�b�`�����ʂ��X�V
            if(panel!=null) panel.Update();
        }



        //�^�b�`���𐸍�
        static TouchInfo SetT(TouchInfo info)
        {
            if (panel == null) return info;
            //�e�^�b�`�͈�
            foreach(TouchBoard i in panel.Content)
            {
                //�^�b�`��񂪃^�b�`�͈͓���
                if (info.Within(i.range))
                {
                    //����ǉ�
                    i.Add(info);
                    //���߃t���O��false�̏ꍇ
                    if (!i.Transparent) break;
                }
            }
            
            return info;
        }

        //�^�b�`���
        static TouchInfo[] touchs;
        static public TouchInfo[] TouchList
        {
            get
            {
                if (touchs == null) touchs = Tdict.Values.ToArray<TouchInfo>();
                return touchs;
            }
        }

        /// <summary>
        /// ���݂̃^�b�`���
        /// </summary>
        static public TouchView CurrentView { get { return panel; } }


    }

    /// <summary>
    /// �^�b�`�̉��(���C�A�E�g)
    /// </summary>
    public class TouchView
    {
        //���ݎg�p����(TouchManager�Őݒ肳��Ă��邩)
        internal bool active = false;
        //z��
        internal int BaseDepth = 0;
        //�e�����蔻��͈̔�
        internal readonly List<TouchBoard> Content;

        /// <summary>
        /// �^�b�`��ʂ�ݒ�
        /// </summary>
        /// <param name="board">�^�b�`�͈͂���</param>
        public TouchView(params TouchBoard[] board)
        {
            if (board != null) Content = new List<TouchBoard>(board); 
            else Content = new List<TouchBoard>();

        }
        



        /// <summary>
        /// �^�b�`�͈͂�ǉ�����
        /// </summary>
        /// <param name="board">�^�b�`�͈�</param>
        /// <param name="depth">z��</param>
        public void Add(TouchBoard board,int depth=-1)
        {
            if (board == null) return;
            Content.Add(board);
            board.pre = this;
            board.Depth = depth < 0 ? BaseDepth++ : depth;
            Content.Sort((a, b) => a.Depth - b.Depth);
        }

        /// <summary>
        /// �^�b�`�͈͂�؂藣��
        /// </summary>
        /// <param name="board">�^�b�`�͈�</param>
        public void Remove(TouchBoard board)
        {
            Content.Remove(board);
            board.pre = null;
        }

        
        internal void Update()
        {
            //�S�Ẵ^�b�`�͈͂̏����X�V����
            foreach (TouchBoard i in Content) i.Update();
        }

        public bool isActive { get { return active; } }
        public List<TouchBoard> BoardList { get { return Content; } }


    }

    /// <summary>
    /// �^�b�`�͈͂�ݒ肷��N���X
    /// </summary>
    public class TouchBoard
    {
        //�������Ă���^�b�`���
        internal TouchView pre = null;
        //�͈͓��Ń^�b�`���ꂽ��ǉ�����z��
        internal List<TouchSatellite> touches = new List<TouchSatellite>();
        //���W
        internal Vector2 point;
        //�^�b�`�͈�
        internal readonly CollisionShape[] range;
        //���߂��邩
        public readonly bool Transparent;
        //z��
        public int Depth;
        //�^�b�`�̏�Ԃ�\������t���O
        bool onPress = false, press = false, first = false, middle = false, final = false;

        /// <summary>
        /// �^�b�`�͈͂�ݒ肷��
        /// ���߂�true�ɂ���ƁA�����^�b�`�͈͂��d�Ȃ��Ă���ꍇ�A���ɉB��Ă���^�b�`�͈͂�
        /// �F�������Bfalse�ɂ���ƔF������Ȃ��B
        /// </summary>
        /// <param name="point">���W</param>
        /// <param name="range">�^�b�`�͈�</param>
        /// <param name="transparent">����</param>
        public TouchBoard(Vector2 point,CollisionShape range,bool transparent=false)
        {
            this.range = new CollisionShape[] { range };
            this.Transparent = transparent;
            this.point = point;
            this.range[0].Point = point;

        }
        /// <summary>
        /// �^�b�`�͈͂�ݒ肷��
        /// ���߂�true�ɂ���ƁA�����^�b�`�͈͂��d�Ȃ��Ă���ꍇ�A���ɉB��Ă���^�b�`�͈͂�
        /// �F�������Bfalse�ɂ���ƔF������Ȃ��B
        /// </summary>
        /// <param name="point">���W</param>
        /// <param name="range">�����̃^�b�`�͈�</param>
        /// <param name="transparent">����</param>
        public TouchBoard(Vector2 point,CollisionShape[] range, bool transparent = false)
        {
            this.range = range;
            this.point = point;
            this.Transparent = transparent;

            foreach (CollisionShape i in this.range)
            {
                i.Point = this.point;
                i.point = this.point;
            }
        }

        internal void Update()
        {
            //�O��^�b�`����Ă��Ȃ��������̃t���O
            bool oldnull = !press;
            //�͈͓��Ń^�b�`����Ă���Ƃ����t���O�̏�����
            onPress = false;
            //���[�v�̂��ߒl�𕡐�
            List<TouchSatellite> temp = new List<TouchSatellite>(touches);


            foreach(TouchSatellite i in temp)
            {
                //�͈͓��Ń^�b�`�����^�b�`�̏����X�V
                i.Update();
                //�͈͓��Ń^�b�`����Ă���Ƃ����t���O
                onPress |= i.OnPressFlag;
                //�^�b�`�������ꂽ�ꍇ
                if (i.FinalFlag)
                {
                    //����؂藣��
                    touches.Remove(i);
                }
            }

            

            //�^�b�`���1�ȏ゠��Ƃ��͈̔͂̓^�b�`����Ă���
            press = touches.Count > 0;

            //�^�b�`���n�߂�
            first =  press && oldnull;
            //����
            middle = !first && press;
            //�^�b�`�������ꂽ
            final =  !(press ||oldnull);

        }
        //�^�b�`����ǉ�����
        internal void Add(TouchInfo info)
        {
            touches.Add(new TouchSatellite(info, this));
            
        }

        /// <summary>
        /// �͈͓��Ń^�b�`���ꂽ�̃^�b�`�̏��
        /// </summary>
        public List<TouchSatellite> TouchList { get { return touches; } }
        /// <summary>
        /// �^�b�`����Ă��邩
        /// </summary>
        public bool PressFlag { get { return press; } }
        /// <summary>
        /// �͈͓��Ń^�b�`����Ă��邩
        /// </summary>
        public bool OnPressFlag { get { return onPress; } }
        /// <summary>
        /// �^�b�`���n�߂�
        /// </summary>
        public bool FirstFlag { get { return first; } }
        /// <summary>
        /// �^�b�`���Ă���Œ�
        /// </summary>
        public bool MiddlesFlag { get { return middle; } }
        /// <summary>
        /// �^�b�`�������ꂽ
        /// </summary>
        public bool FinalFlag { get { return final; } }
        /// <summary>
        /// ���̃^�b�`�͈͂͌��ݎg�p����
        /// </summary>
        public bool isActive
        {
            get
            {
                if (pre == null) return false;
                return pre.isActive;
            }
        }
        /// <summary>
        /// ��ʏ�̍��W
        /// </summary>
        public Vector2 Point
        {
            get { return point; }
            set
            {
                point.X = value.X;
                point.Y = value.Y;
                if (range.Length == 1)range[0].Point = point;
                else foreach (CollisionShape i in range)i.Point = point;

            }
        }
        /// <summary>
        /// �������Ă���^�b�`���(���C�A�E�g)
        /// </summary>
        public TouchView Parent { get { return pre; } }
       
        
    }


    /// <summary>
    ///�@�͈͓��Ń^�b�`�������
    /// </summary>
    public class TouchSatellite
    {
        //�^�b�`���
        readonly TouchInfo touch;
        //�Y���͈�
        readonly TouchBoard pre;
        //���݂͔͈͓��Ń^�b�`���Ă��邩�@, �ЂƂO�̏��
        bool onRange,oldOnRange;

        /// <summary>
        /// �͈͓��Ń^�b�`�������
        /// </summary>
        /// <param name="touch">�Y���̃^�b�`���</param>
        /// <param name="pre">�Y���̃^�b�`�͈�</param>
        internal TouchSatellite(TouchInfo touch,TouchBoard pre)
        {
            this.touch = touch;
            this.pre = pre;
        }


        internal void Update()
        {
            //1�O�̏�Ԃ�ێ�
            oldOnRange = onRange;
            //���ݔ͈͓��ɂ���̂�
            onRange = touch.Within(pre.range);
        }

        /// <summary>
        /// �^�b�`�����擾����
        /// </summary>
        public TouchInfo ToTouchInfo { get { return touch; } }
        /// <summary>
        /// �^�b�`���n�߂���
        /// </summary>
        public bool FirstFlag { get { return touch.FirstFlag; } }
        /// <summary>
        /// �^�b�`���Ă���Œ���
        /// </summary>
        public bool MiddleFlag { get { return touch.MiddleFlag; } }
        /// <summary>
        /// �^�b�`���I��������
        /// </summary>
        public bool FinalFlag { get { return touch.FinalFlag; } }
        /// <summary>
        /// ���ݔ͈͓��Ń^�b�`���Ă��邩
        /// </summary>
        public bool OnPressFlag { get { return onRange; } }
        /// <summary>
        /// 1�O�ł͔͈͓��Ń^�b�`���Ă�����
        /// </summary>
        public bool OldOnPressFlag { get { return oldOnRange; } }
    }
        

    /// <summary>
    /// �^�b�`���
    /// </summary>
    public class TouchInfo
    {
        //���ʔԍ� , ��������
        int id, lifetime=0;
        //�^�b�`���J�n�������W , ���݂̍��W ,�@�ψ� 
        Vector2 center, point, delta =Vector2.Zero;
        //�^�b�`�̓����蔻��
        internal CollisionDot dot;
        //�^�b�`�̏�Ԃ�\������t���O
        bool first=true, final=false,middle=false;

        /// <summary>
        /// �^�b�`����ݒ�
        /// </summary>
        /// <param name="point">�^�b�`�������W</param>
        /// <param name="id">���ʔԍ�</param>
        internal TouchInfo(Vector2 point,int id)
        {
            
            this.id = id;
            dot = new CollisionDot(CollisionName.TouchDot, Vector2.Zero);
            Vector2 point2 = Screen.ExportVector(point);
            center = point2;
            this.point = point2;

            dot.Point = point2;
        }


        internal void Update(Vector2 point,GameTime gameTime)
        {
            //�������Ԃ�0�̏ꍇ�^�b�`���n�߂�
            first = lifetime == 0;
            //�����łȂ���΃^�b�`���Ă���Œ�
            middle = !first;
            //�^�b�`���ꂽ���W�����̃Q�[���̍��W�֕ϊ�����
            Vector2 point2 = Screen.ExportVector(point);

            float sX = point2.X;
            float sY = point2.Y;
            //�������Ԃ��X�V����B�ő�l�܂ł�������ω����Ȃ��Ȃ�
            lifetime = Math.Min(1 << 30, lifetime + gameTime.ElapsedGameTime.Milliseconds);

            //�^�b�`���W�̕ψʂ����߂�
            delta.X = sX - this.point.X;
            delta.Y = sY - this.point.Y;

            //���݂̃^�b�`���W�Ƃ��Đݒ�
            this.point.X = sX;
            this.point.Y = sY;
            //�����蔻��̍��W���ݒ�
            dot.Point = this.point;
           

        }
        /// <summary>
        /// �^�b�`�����폜
        /// </summary>
        internal void Delete()
        {
            //�^�b�`�͗����ꂽ
            final = true;
            //�^�b�`���Ă���Œ��ł͂Ȃ��Ȃ���
            middle = false;
        }

        /// <summary>
        /// �^�b�`�͎w�肵�������蔻�����
        /// </summary>
        /// <param name="range">�����蔻��</param>
        /// <returns></returns>
        internal bool Within(CollisionShape[] range)
        {
            //1�ł��������Ă�����true��Ԃ�
            foreach (CollisionShape i in range) if (Collision.Check(dot, i)) return true;
            return false;
        }

        /// <summary>
        /// ���ʔԍ�
        /// </summary>
        public int ID { get { return id; } }
        /// <summary>
        /// ��������
        /// </summary>
        public int LifeTime { get { return lifetime; } }
        /// <summary>
        /// �^�b�`�J�n���̍��W
        /// </summary>
        public Vector2 Center { get { return center; } }
        /// <summary>
        /// ���݂̍��W
        /// </summary>
        public Vector2 Point { get { return point; } }
        /// <summary>
        /// �^�b�`�J�n������̕ψ�
        /// </summary>
        public Vector2 Delta { get { return delta; } }
        /// <summary>
        /// �^�b�`�J�n����
        /// </summary>
        public bool FirstFlag { get { return first; } }
        /// <summary>
        /// �^�b�`���Ă���Œ���
        /// </summary>
        public bool MiddleFlag { get { return middle; } }
        /// <summary>
        /// �^�b�`�͗����ꂽ��
        /// </summary>
        public bool FinalFlag { get { return final; } }

    }

}
