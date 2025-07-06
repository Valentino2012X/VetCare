using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class NPago
    {
        private DPago dPago = new DPago();

        public string Registrar(Pago pago)
        {
            return dPago.Registrar(pago);
        }

        public List<Pago> ListarPendientes()
        {
            return dPago.ListarPendientes();
        }

        public List<Pago> ListarPagados()
        {
            return dPago.ListarPagados();
        }
    }
}
