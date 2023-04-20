using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp5_Colas
{
    public class Cliente
    {
        public int clienteId { get; set; }
        public int CantidadCliente { get; set; }
        public double HorarioLlegada { get; set; }
        public bool estado { get; set; }
        public double rndCantClientes { get; set; }



        public int calcularCantidadClientes()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            double rndCantClientes = crearRandomCantidad(random);
            this.rndCantClientes = rndCantClientes;
            double[] V1 = new double[] { 0, 0.1, 0.6, 0.8 };
            double[] V2 = new double[] { 0.099, 0.599, 0.799, 0.999 };
            int[] Cantidad = new int[] { 1, 2, 3, 4 };
            int cantidad = 0;
            for (int i = 0; i < Cantidad.Length; i++)
            {
                if (V1[i] <= rndCantClientes && rndCantClientes <= V2[i])
                {
                    return Cantidad[i];
                }
            }
            return 0;
        }
        public double crearRandomCantidad(Random rndCant)
        {
            double randomm = 0;
            while (randomm == 0 || randomm == 1)
            {
                randomm = Math.Round(rndCant.NextDouble(), 3);
            }
            return Math.Round(randomm, 3);
        }

    }

    
}