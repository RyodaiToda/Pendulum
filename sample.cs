using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;

namespace Pendulum
{

    public struct Value
    {

        public double m;
        public double g;
        public double k;
        public double r;
        public double z;
        public double omen;
        public double th0;
        public double dth0;
        public double temp; //sqrt(1-z^2)
        public bool flag;   //1-z^2が正か負か

        public Value(double m, double g, double k, double r, double th0, double dth0)
        {
            this.m = m;
            this.g = g * 1000;
            this.k = k;
            this.r = r;
            this.z = (2.0 * this.m / this.k) * (Math.Sqrt(this.g / this.r));
            this.omen = k / (2.0 * m);
            this.th0 = th0;
            this.dth0 = dth0;
            this.temp = Math.Sqrt(Math.Abs(1.0 - z * z));
            if (1 - z * z > 0.0) this.flag = true;
            else this.flag = false;
        }

    }

    public struct Std
    {
        public double x;
        public double y;
        public double B;
        public double omg;


        public Std(double x, double y, double B, double omg)
        {
            this.x = x;
            this.y = y;
            this.B = B;
            this.omg = omg;
        }


    }


    public partial class Form1 : Form
    {
        public PictureBox pb;   //描画
        public Pendulum pd;     //振子の数値をまとめた構造体
        public Pendulum pd2;
        public Stand st;        //台の数値をまとめた構造体

        public Value vl;        //系の数値をまとめた構造体

        public EquationSolver es;   //微分方程式解くマン

        public Button bt1;  //スタート
        public Button bt2;  //ストップ
        public Button bt3;  //再開

        private double t;   //時間

        private double theta;   //振子の角度
        private double dth;     //振子の各速度
        private double omg;

        private bool flag_stop = false;

        //Std std = new Std(200.0, 150, 50, 6.2832);

        public Form1()
        {
            InitializeComponent();

            Size = new Size(640, 450);
            vl = new Value(0.01, 9.8, 0.0001, 150.0, 150.0 * Math.PI / 180.0, 0.0);

            pb = new PictureBox();
            pb.Width = 500;
            pb.Height = 400;
            pb.Top = 5;
            pb.Left = 5;
            pb.BackColor = Color.White;
            Controls.Add(pb);

            bt1 = new Button();
            bt1.Width = 100;
            bt1.Height = 40;
            bt1.Location = new Point(pb.Width - 5 + (this.Width - pb.Width - bt1.Width) / 2, 100);
            bt1.Text = "Start";
            bt1.Font = new Font("MS UI Gothic", 12);
            bt1.Click += Bt1_Click;
            Controls.Add(bt1);

            bt2 = new Button();
            bt2.Width = 100;
            bt2.Height = 40;
            bt2.Location = new Point(pb.Width - 5 + (this.Width - pb.Width - bt1.Width) / 2, 160);
            bt2.Text = "Stop";
            bt2.Font = new Font("MS UI Gothic", 12);
            bt2.Click += Bt2_Click;
            Controls.Add(bt2);

            bt3 = new Button();
            bt3.Width = 100;
            bt3.Height = 40;
            bt3.Location = new Point(pb.Width - 5 + (this.Width - pb.Width - bt1.Width) / 2, 280);
            bt3.Text = "Option";
            bt3.Font = new Font("MS UI Gothic", 12);
            bt3.Click += Bt3_Click;
            Controls.Add(bt3);

            Initialize();

            //if (vl.flag)
            //    Console.WriteLine("過制動");
            //else
            //    Console.WriteLine("減衰振動");

            DrawSystem(pb);

        }

        private void Bt3_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //timer1.Start();

            //timer1.Stop();
            //flag_stop = true;

            Form2 form2 = new Form2(this, ref vl, 640, 450);
            form2.Show();

        }
        private void Bt2_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            if (!flag_stop)
            {
                timer1.Stop();
                flag_stop = true;
                bt2.Text = "Resume";
            }
            else
            {
                timer1.Start();
                flag_stop = false;
                bt2.Text = "Stop";
            }
        }

        private void Bt1_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Initialize();

            flag_stop = false;
            bt2.Text = "Stop";
        }

        private void Initialize()
        {
            //vl = new Value(0.01, 9.8, 0.0001, 150.0, 150.0 * Math.PI / 180.0, 0.0);

            st = new Stand(250.0, 200.0, 100.0, 8.08);
            pd = new Pendulum(vl.th0, vl.r, st.getX(), st.getY());
            pd2 = new Pendulum(0.0 * Math.PI / 180.0, vl.r, st.getX(), st.getY());
            //pd = new Pendulum(vl.th0, vl.r, std.x, std.y);
            omg = Math.Sqrt(vl.g / (vl.r / 1000));

            timer1.Interval = 8;

            es = new EquationSolver(timer1.Interval / (1000.0 * 100.0), timer1.Interval / 1000.0, vl, st);

            timer1.Start();
            t = 0.0;
            theta = vl.th0;
            dth = vl.dth0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Console.WriteLine("Hello");b 
            t += timer1.Interval / 1000.0;

            //Console.WriteLine(t.ToString() + "," + theta.ToString());

            //theta = (vl.th0 ) * Math.Cos(omg * t);    //空気抵抗なし
            //if (vl.flag)
            //{
            //    theta = Math.Exp(vl.temp * vl.omen * t) * ((-1.0 - vl.temp) * vl.th0);
            //    theta += Math.Exp(-vl.temp * vl.omen * t) * ((1.0 - vl.temp) * vl.th0);
            //    theta *= -0.5 * Math.Exp(-vl.omen * t) / vl.temp;
            //}
            //else
            //{
            //    theta = vl.th0 * Math.Exp(-vl.omen * t);
            //    theta *= Math.Cos(vl.temp * vl.omen * t) + Math.Sin(vl.temp * vl.omen * t) / vl.z;
            //}

            Console.WriteLine(t.ToString() + "," + theta.ToString());
            //Console.WriteLine(t.ToString() + "," + st.getX().ToString() + "," + st.getA(timer1.Interval / 1000.0).ToString() + "," + st.getA2(t).ToString());

            st.setValue(t);

            pd.setValue(theta);
            pd2.setValue(theta);

            DrawSystem(pb);


            es.Euler(t, this.theta, this.dth);

            this.theta = es.getTheta();
            this.dth = es.getDtheta();
        }

        private void DrawSystem(PictureBox pb)
        {
            Bitmap canvas = new Bitmap(pb.Width, pb.Height);
            Graphics g = Graphics.FromImage(canvas);

            Pen pen = new Pen(Color.Black, 1);
            Pen Rpen = new Pen(Color.Red, 1);

            DrawStand(g, pen);

            DrawPendulum(g, pen, pd);
            DrawLine(g, pen, pd, st);

            DrawPendulum(g, Rpen, pd2);
            DrawLine(g, Rpen, pd2, st);

            //g.DrawLine(pen, (float)std.x, (float)std.y, (float)pd.getX(), (float)pd.getY());
            //g.DrawLine(pen, (float)st.getX(), (float)st.getY(), (float)pd.getX(), (float)pd.getY());
            //g.DrawLine(pen, (float)(pd.getX() + st.getX()), (float)(pd.getY() + st.getY()), (float)st.getX(), (float)st.getY());

            //DrawAxis(g, pen);

            //g.DrawEllipse(pen, (float)(st.getX()), (float)(st.getY()), (float)pd.getR(), (float)pd.getR());

            //g.DrawLine(new Pen(Color.Black, 1), (float)st.getX(),(float)st.getY() - 220, (float)st.getX(), (float)st.getY() + 220);


            pen.Dispose();
            pb.Image = canvas;

        }

        private void DrawPendulum(Graphics g, Pen pen, Pendulum pd, int radius = 10)
        {
            g.DrawEllipse(pen, (float)(pd.getX() - radius + st.getX()), (float)(pd.getY() - radius + st.getY()), radius * 2, radius * 2);
        }

        private void DrawLine(Graphics g, Pen pen, Pendulum pd, Stand st)
        {
            g.DrawLine(pen, (float)(pd.getX() + st.getX()), (float)(pd.getY() + st.getY()), (float)st.getX(), (float)st.getY());

        }

        private void DrawAxis(Graphics g, Pen pen)
        {
            int w = 50;
            for (int i = 0; i <= pb.Height / w; i++)
            {
                g.DrawLine(pen, 0, i * w, pb.Width, i * w);
            }

            for (int i = 0; i <= pb.Width / w; i++)
            {
                g.DrawLine(pen, i * w, 0, i * w, pb.Width);
            }

        }

        private void DrawStand(Graphics g, Pen pen)
        {
            float w = 200;
            float h = 10;

            //g.DrawRectangle(pen, (float)std.x - w / 2, (float)std.y - h / 2, w, h);
            g.DrawRectangle(pen, (float)st.getX() - w / 2, (float)st.getY() - h / 2, w, h);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
