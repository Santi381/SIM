using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp5_Colas.Clases
{
    public interface IBloqueo
    {
         double tiempoBloqueo(double h, double t0, double A, double A0);
    }
}
