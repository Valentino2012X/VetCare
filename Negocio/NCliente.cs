using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class NCliente
    {
        private DCliente datos = new DCliente();

        public List<Cliente> ListarTodo()
        {
            return datos.ListarTodo();
        }

        public string Registrar(Cliente cliente)
        {
            return datos.Registrar(cliente);
        }

        public string Modificar(Cliente cliente)
        {
            return datos.Modificar(cliente);
        }

        public string EliminarLogico(int id)
        {
            return datos.EliminarLogico(id);
        }

        public Cliente Obtener(int id)
        {
            return datos.Obtener(id);
        }
    }
}
