using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp5_Colas.Clases
{
    public class VectorEstado
    {
        public int nroIteracion { get; set; }
        public string nombreEvento { get; set; }
        public double reloj { get; set; }
        public LlegadaCliente llegadaCliente {get; set; }
        public List<FinDeAtencion> finDeAtenciones { get; set; }
        public List<Mesa> mesas { get; set; }
        public List<Cliente> clientes { get; set; }
        public ProximoBloqueo proximoBloqueo { get; set; }

        public int maximaCantClientes()
        {
            List<int> cantidades = new List<int>();
            int mayor = 0;
            int mesaID = 0;
            foreach (Mesa mesa in this.mesas)
            {
                if (mesa.contadorClientes > mayor )
                {
                    mayor = mesa.contadorClientes;
                    mesaID = mesa.mesaId;
                }
            }

            return mesaID;
        }

    }


}
