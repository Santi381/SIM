using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp5_Colas.Clases
{
    public  class finBloqueoBarra : IBloqueo
    {
        public int id { get; set; }
        public double tiempoBloqueoParcial { get; set; }
        public double tiempoFinBloqueoMesa { get; set; }


        public double[] V1 = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public double tiempoBloqueo(double h, double t0, double s, double S0)
        {
            V1[0] = t0;
            V1[1] = S0;


            bool ban = false;


            while (V1[1] <= S0 * 1.35)
            {
                if (ban)
                {
                    V1[0] += h;
                    V1[1] = Math.Round(V1[12], 6);
                }
                else
                {
                    V1[0] = 0;
                }
                ban = true;

                double k1 = (0.2 * V1[1]) + 3 - V1[0];
                V1[2] = k1;
                double xmh2 = V1[0] + (h / 2);
                double ymh2K1 = V1[1] + (h / 2) * k1;
                double k2 = (0.2 * ymh2K1) + 3 - xmh2;
                V1[5] = k2;
                double ymh2K2 = V1[1] + (h / 2) * k2;
                double k3 = (0.2 * ymh2K2) + 3 - xmh2;
                V1[8] = k3;
                double xmh = V1[0] + h;
                double ymhK3 = V1[1] + h * k3;
                double k4 = (0.2 * ymhK3) + 3 - xmh;
                V1[11] = k4;


                V1[12] = Math.Round(V1[1] + (h / 6) * ((V1[2] + 2 * V1[5] + 2 * V1[8] + V1[11])), 6);

            }
            return Math.Round(V1[0]*2, 3);

        }
    }
}
