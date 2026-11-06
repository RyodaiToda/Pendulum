using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pendulum
{
    public partial class Form2 : Form
    {

        public Value vl;
        public TextBox[] tbs = new TextBox[6];
        public Label[] lbs = new Label[6];
        public Button bt = new Button();

        public Form1 f = new Form1();

        public Form2(Form1 f, ref Value v, int w = 640, int h = 450)
        {
            InitializeComponent();
            this.Width = w;
            this.Height = h;
            this.vl = v;
            this.f = f;

            for(int i = 0; i < tbs.Length; i++)
            {
                this.tbs[i] = new TextBox();
                this.tbs[i].Size = new Size(100, 40);
                this.tbs[i].Left = 200;
                this.tbs[i].Top = i * (this.tbs[i].Height * 3) + 30;
                this.tbs[i].TextChanged += Form2_TextChanged;

                this.lbs[i] = new Label();
                this.lbs[i].Size = new Size(150, 40);
                this.lbs[i].Left = 50;
                this.lbs[i].Top = i * (this.tbs[i].Height * 3) + 30;

            }

            tbs[0].Text = this.vl.m.ToString();
            tbs[1].Text = (this.vl.g / 1000.0).ToString();
            tbs[2].Text = this.vl.k.ToString();
            tbs[3].Text = this.vl.r.ToString();
            tbs[4].Text = this.vl.th0.ToString();
            tbs[5].Text = this.vl.dth0.ToString();

            lbs[0].Text = "質量[kg]";
            lbs[1].Text = "重力加速度[m/s^2]";
            lbs[2].Text = "粘性係数[kg/s]";
            lbs[3].Text = "半径[mm]";
            lbs[4].Text = "初期角度[rad]";
            lbs[5].Text = "初期角速度[rad/s]";

            Controls.AddRange(this.tbs);
            Controls.AddRange(this.lbs);

            bt.Location = new Point(400, 300);
            bt.Text = "送信";
            bt.Size = new Size(100, 30);
            bt.Click += Bt_Click;
            Controls.Add(bt);


        }

        private void Form2_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //this.vl = new Value(
            //    double.Parse(tbs[0].Text),
            //    double.Parse(tbs[1].Text),
            //    double.Parse(tbs[2].Text),
            //    double.Parse(tbs[3].Text),
            //    double.Parse(tbs[4].Text),
            //    double.Parse(tbs[5].Text)
            //    );

            ////Console.WriteLine(tbs[0].Text.ToString());

            //f.vl = this.vl;
        }

        private void Bt_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();


            this.vl = new Value(
                double.Parse(tbs[0].Text),
                double.Parse(tbs[1].Text),
                double.Parse(tbs[2].Text),
                double.Parse(tbs[3].Text),
                double.Parse(tbs[4].Text),
                double.Parse(tbs[5].Text)
                );

            //Console.WriteLine(tbs[0].Text.ToString());

            f.vl = this.vl;

        }

    }
}
