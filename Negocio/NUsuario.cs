using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class NUsuario
    {
        private readonly DUsuario dUsuario = new DUsuario();

        public Usuario ValidarCredenciales(string nombreUsuario, string contrasenaHash)
        {
            return dUsuario.ObtenerUsuario(nombreUsuario, contrasenaHash);
        }
    }
}
