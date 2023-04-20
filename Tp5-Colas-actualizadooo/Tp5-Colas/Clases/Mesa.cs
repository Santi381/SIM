using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp5_Colas
{
    public class Mesa
    {
        public int mesaId { get; set; }
        public bool estado { get; set; }
        public Cliente cliente { get; set; }

        public int contadorClientes { get; set; }  
    }
}
