using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp5_Colas.Clases
{
    public class FinDeAtencion: IEvento
    {
       public int idFinEvento { get; set; }
       public double tiempoFinAtencion { get; set; }
       public Cliente cliente { get; set; }
       public Mesa mesa { get; set; }
       
       public double tiempoEvento()
       {
            switch (cliente.CantidadCliente)
            {
                case 1:
                    return  20;
                case 2:
                    return 30;
                case 3:
                    return 40;
                case 4:
                    return 40;
                default:
                    return 0;
            }
       }
    }
}
