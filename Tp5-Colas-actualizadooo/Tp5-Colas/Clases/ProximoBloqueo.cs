using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp5_Colas.Clases
{
    public class ProximoBloqueo : IBloqueo
    {
        public int idProximoBloque { get; set; }
        public double TBloqueo { get; set; }
        public double TiempoProximBloqueo { get; set; }
        public string TipoBloqueo { get; set; }
        public finBloqueoLlegadaCliente finBloqueoLlegadaCliente { get; set; }
        public finBloqueoBarra finBloqueoBarra { get; set; }
        public double RndTipo { get; set; }


        public double[] V1 = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public double tiempoBloqueo(double h, double t0, double A, double A0)
        {

            V1[0] = t0;
            V1[1] = A0;

            bool ban = false;
            double b = crearRandom();


            while (V1[1] <= A0 * 2)
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

                double k1 = V1[1] * b;
                V1[2] = k1;
                double xmh2 = V1[0] + (h / 2);
                double ymh2K1 = V1[1] + (h / 2) * k1;
                double k2 = ymh2K1 * b;
                V1[5] = k2;
                double ymh2K2 = V1[1] + (h / 2) * k2;
                double k3 = ymh2K2 * b;
                V1[8] = k3;
                double xmh = V1[0] + h;
                double ymhK3 = V1[1] + h * k3;
                double k4 = ymhK3 * b;
                V1[11] = k4;


                V1[12] = Math.Round(V1[1] + (h / 6) * ((V1[2] + 2 * V1[5] + 2 * V1[8] + V1[11])), 6);

            }

            return Math.Round(V1[0]*9, 3);

        }

        public double crearRandom()
        {
            Random rndLleg = new Random(DateTime.Now.Millisecond);
            double randomm = 0;
            while (randomm == 0 || randomm == 1)
            {
                randomm = Math.Round(rndLleg.NextDouble(), 2);
            }
            return Math.Round(randomm, 2);
        }

        public string determinarTipoBloqueo()
        {
            //Random random = new Random(DateTime.Now.Millisecond);
            double rndTipo = crearRandom();
            this.RndTipo = rndTipo;
            double[] V1 = new double[] { 0, 0.70};
            double[] V2 = new double[] { 0.69, 0.99};
            string[] Tipo = new string[] { "Llegada", "Servidor"};
            int cantidad = 0;
            var res = "";
            for (int i = 0; i < Tipo.Length; i++)
            {
                if (V1[i] <= rndTipo && rndTipo <= V2[i])
                {
                    res = Tipo[i];
                }
                
            }
            return res;
        }



    }
}
