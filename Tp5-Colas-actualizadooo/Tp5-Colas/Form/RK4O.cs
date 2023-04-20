using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tp5_Colas
{
    public partial class RK4O : Form
    {

        public double[] V1 = new double[] { 0, 0, 0, 0,0,0,0,0,0,0,0,0,0 };
      
        public RK4O()
        {
            InitializeComponent();
        }

        private void RK4O_Load(object sender, EventArgs e)
        {

        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            DgvRK4.Rows.Clear();
            double h =  Convert.ToDouble( Txth.Text);
            double t0 =  Convert.ToDouble( Txtt0.Text);
            double L =  Convert.ToDouble( TxtL.Text);
            double L0 =  Convert.ToDouble(TxtLo.Text);

            V1[0] = t0;
            V1[1] = L0;

            bool ban = false;
         

            while (1 < Math.Abs(V1[1] - V1[12]))
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

                double k1 = -((V1[1] / 0.8) * Math.Pow(V1[0], 2)) - V1[1];
                V1[2] = k1;
                double xmh2 = V1[0] + (h / 2);
                double ymh2K1 = V1[1] + (h / 2) * k1;
                double k2 = -((ymh2K1 / 0.8) * Math.Pow(xmh2, 2)) - ymh2K1;
                V1[5] = k2;
                double ymh2K2 = V1[1] + (h / 2) * k2;
                double k3 = -((ymh2K2 / 0.8) * Math.Pow(xmh2, 2)) - ymh2K2;
                V1[8] = k3;
                double xmh = V1[0] + h;
                double ymhK3 = V1[1] + h * k3;
                double k4 = -((ymhK3 / 0.8) * Math.Pow(xmh, 2)) - ymhK3;
                V1[11] = k4;


                V1[12] = Math.Round(V1[1] + (h / 6) * ((V1[2] + 2 * V1[5] + 2 * V1[8] + V1[11])), 6);


            var filas = new string[8];
                filas[0] = V1[0].ToString();
                filas[1] = V1[1].ToString();
                filas[2] = V1[2].ToString();
                filas[3] = V1[5].ToString();
                filas[4] = V1[8].ToString();
                filas[5] = V1[11].ToString();
                filas[6] = xmh.ToString();
                filas[7] = V1[12].ToString();

                DgvRK4.Rows.Add(filas);
            }

            Txttt.Text = V1[0] .ToString() ;
        }

        private void BtnCalcular2_Click(object sender, EventArgs e)
        {
            DgvRK4to2.Rows.Clear();
            double h = Convert.ToDouble(TxtH2.Text);
            double t0 = Convert.ToDouble(Txtt02.Text);
            double S = Convert.ToDouble(TxtS.Text);
            double S0 = Convert.ToDouble(TxtS0 .Text);

            V1[0] = t0;
            V1[1] = S0;


            bool ban = false;


            while (V1[1] <= S0 *1.35)
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





                var filas1 = new string[8];
                filas1[0] = V1[0].ToString();
                filas1[1] = V1[1].ToString();
                filas1[2] = V1[2].ToString();
                filas1[3] = V1[5].ToString();
                filas1[4] = V1[8].ToString();
                filas1[5] = V1[11].ToString();
                filas1[6] = xmh.ToString();
                filas1[7] = V1[12].ToString();

                DgvRK4to2.Rows.Add(filas1);
            }

            TxtSS.Text = V1[0].ToString();
        }

        private void BtnCalcular3_Click(object sender, EventArgs e)
        {
            DgvRK4O3.Rows.Clear();
            double h = Convert.ToDouble(TxtH3.Text);
            double t0 = Convert.ToDouble(Txtt03.Text);
            double A = Convert.ToDouble(TxtA.Text);
            double A0 = Convert.ToDouble(TxtA0.Text);

            V1[0] = t0;
            V1[1] = A0;

            Random random = new Random(DateTime.Now.Millisecond);
            double rndB = crearRandom(random);

            bool ban = false;


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

                double k1 = V1[1] * rndB;
                V1[2] = k1;
                double xmh2 = V1[0] + (h / 2);
                double ymh2K1 = V1[1] + (h / 2) * k1;
                double k2 = ymh2K1 * rndB;
                V1[5] = k2;
                double ymh2K2 = V1[1] + (h / 2) * k2;
                double k3 =ymh2K2 * rndB;
                V1[8] = k3;
                double xmh = V1[0] + h;
                double ymhK3 = V1[1] + h * k3;
                double k4 = ymhK3 * rndB;
                V1[11] = k4;


                V1[12] = Math.Round(V1[1] + (h / 6) * ((V1[2] + 2 * V1[5] + 2 * V1[8] + V1[11])), 6);





                var filas2 = new string[8];
                filas2[0] = V1[0].ToString();
                filas2[1] = V1[1].ToString();
                filas2[2] = V1[2].ToString();
                filas2[3] = V1[5].ToString();
                filas2[4] = V1[8].ToString();
                filas2[5] = V1[11].ToString();
                filas2[6] = xmh.ToString();
                filas2[7] = V1[12].ToString();

                DgvRK4O3.Rows.Add(filas2);
            }

            TxtAA.Text = V1[0].ToString();
        }
        double crearRandom(Random rndCant)
        {
            double randomm = 0;
            while (randomm == 0 || randomm == 1)
            {
                randomm = Math.Round(rndCant.NextDouble(), 3);
            }
            return Math.Round(randomm, 3);
        }

        private void TxtAA_TextChanged(object sender, EventArgs e)
        {

        }

        private void DgvRK4O3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TxtSS_TextChanged(object sender, EventArgs e)
        {

        }

        private void DgvRK4to2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Txttt_TextChanged(object sender, EventArgs e)
        {

        }

        private void DgvRK4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
