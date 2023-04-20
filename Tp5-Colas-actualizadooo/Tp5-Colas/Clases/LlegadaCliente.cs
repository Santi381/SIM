using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp5_Colas.Clases
{
    public class LlegadaCliente: IEvento
    {
        public int id { get; set; } 
        public double rndLlegada {get; set; }
        public double tiempoLlegada { get; set; }
        public double proximaLlegada { get; set; }
        public Cliente cliente { get; set; }

        

        public double tiempoEvento()
        {
            double media;
            media = 3;
            double tLlegada = -media * (Math.Log(1 - rndLlegada));
            return  Math.Round(tLlegada, 3);
        }

        public double crearRandomLlegada()
        {
            Random rndLleg = new Random(DateTime.Now.Millisecond);
            double randomm = 0;
            while (randomm == 0 || randomm == 1)
            {
                randomm = Math.Round(rndLleg.NextDouble(), 3);
            }
            return Math.Round(randomm, 3);
        }

       
    }
}
