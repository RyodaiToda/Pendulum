using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pendulum
{
    public class Stand
    {
        private double[] x = new double[3];
        private double y;
        public double B;
        public double omg;

        private double x0;
        private double y0;

        public Stand(double x, double y, double B, double omg)
        {
            this.x0 = x;
            this.y0 = y;

            this.B = B;
            this.omg = omg;

            this.x[0] = 0.0;
            this.x[1] = 0.0;
            this.x[2] = x;
            this.y = this.y0;
        }

        public void setValue(double t)
        {
            this.x[0] = this.x[1];
            this.x[1] = this.x[2];
            this.x[2] = this.x0 + this.B * Math.Sin(this.omg * t);
        }

        public void setX(double x) { this.x[2] = x;}
        public void setY(double y) {  this.y = y;}
        public double getX() { return x[2]; }

        public double getY() { return y; }

        public double getA(double interval = 0.017)
        {
            return (x[2] - 2.0 * x[1] + x[0]) / (interval * interval * 1000);
        }

        public double getA2(double t)
        {
            return -this.omg * this.omg * this.B * Math.Sin(this.omg * t);
        }
    }
}
