using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pendulum
{
    public class EquationSolver
    {
        public double h;    //刻み幅
        public double omg0;  //角振動数

        public double t;    //時間
        //public double dt;

        public double th;
        public double dth;

        public double th0;
        public double dth0;

        public double interval;

        public Value vl;
        public Stand st;

        public EquationSolver(double h, double interval, Value vl, Stand st)
        {
            this.h = h;
            this.omg0 = st.omg;
            this.interval = interval;
            this.vl = vl;
            this.st = st;

            this.t = 0.0;
        }

        public double ddfunc(double t, double th, double dth)
        {
            double temp1 = st.B * omg0 * omg0 * Math.Sin(omg0 * t) * Math.Cos(th) / ( vl.r);    //慣性力
            double temp2 = -vl.g * Math.Sin(th) / vl.r;                                         //重力
            double temp3 = -vl.k * dth / vl.m;                                                  //空気抵抗

            return temp1 + temp2 + temp3;
        }

        public double ddfunc(double t, double th, double dth, double ddx)
        {
            double temp1 = -ddx * Math.Cos(th) / (vl.r);    //慣性力
            double temp2 = -vl.g * Math.Sin(th) / vl.r;                                         //重力
            double temp3 = -vl.k * dth / vl.m;                                                  //空気抵抗

            return temp1 + temp2 + temp3;
        }

        public void Euler(double t, double th, double dth, int mode = 0)
        {
            this.th0 = th;
            this.dth0 = dth;

            this.th = th;
            this.dth = dth;

            this.t = t;

            for (int i = 1; i <= interval / h; i++)
            {
                this.th += h * this.dth;

                if (mode == 0) this.dth += h * ddfunc(this.t, this.th, this.dth);
                else this.dth += h * ddfunc(this.t, this.th, this.dth, this.st.getA(this.interval));

                this.t += h;
            }

        }

        public double getTheta()
        {
            return this.th;
        }

        public double getDtheta()
        {
            return this.dth;
        }


    }
}
