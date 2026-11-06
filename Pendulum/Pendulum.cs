using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pendulum
{
    public class Pendulum
    {

        private double theta;   //[rad]
        private double dth;

        private double x;
        private double y;
        private double r;
        private EquationSolver es;

        private double standx;
        private double standy;

        public Pendulum(double theta, double x, double y, double r, double standx, double standy)
        {
            setValue(theta, x, y, r, standx, standy);
            this.standy = standy;
        }

        public Pendulum(double theta, double r, double stnadx, double standy)
        {
            this.theta = theta;
            this.r = r;
            this.standx = stnadx;
            this.standy = standy;

            this.x = this.r * Math.Sin(theta);
            this.y = this.r * Math.Cos(theta);
        }

        public Pendulum(EquationSolver es, double theta, double r, double stnadx, double standy)
        {
            this.es = es;
            this.theta = theta;
            this.r = r;
            this.standx = stnadx;
            this.standy = standy;

            this.x = this.r * Math.Sin(theta);
            this.y = this.r * Math.Cos(theta);
        }

        public void setValue(double theta, double x, double y, double r, double standx, double standy)
        {
            this.theta = theta;
            this.x = x;
            this.y = y;
            this.r = r;
        }

        //public void setValue(double theta)
        //{
        //    this.theta = theta;
        //    this.x = this.standx + this.r * Math.Sin(theta);
        //    this.y = this.standy + this.r * Math.Cos(theta);
        //}
        
        public void setValue(double theta)
        {
            this.theta = theta;
            this.x = this.r * Math.Sin(theta);
            this.y = this.r * Math.Cos(theta);
        }

        public void setValue()
        {
            this.x = this.r * Math.Sin(theta);
            this.y = this.r * Math.Cos(theta);
        }

        public void calc(double t, int mode = 0)
        {
            es.Euler(t, theta, dth, mode);
            this.theta = es.getTheta();
            this.dth = es.getDtheta();
        }

        public double getTheta() { return theta; }

        public double getX() { return x; }
            
        public double getY() { return y; }
        public double getR() { return r; }

    }
}
