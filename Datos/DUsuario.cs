using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{

    public class DUsuario
    {
        public Usuario ObtenerUsuario(string nombreUsuario, string contrasenaHash)
        {
            using (var contexto = new BDEFEntities())
            {
                return contexto.Usuario
                    .Include("Rol") // incluye datos del rol
                    .FirstOrDefault(u =>
                        u.NombreUsuario == nombreUsuario &&
                        u.ContrasenaHash == contrasenaHash &&
                        u.Activo == true);
            }
        }
    }
}
