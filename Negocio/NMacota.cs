using Datos;
using System.Collections.Generic;
using System.Linq;

namespace Negocio
{
    public class NMascota
    {
        private readonly DMascota _dMascota;

        // Constructor que recibe el contexto de BD
        public NMascota(BDEFEntities context)
        {
            _dMascota = new DMascota(context);
        }

        public List<Mascota> ListarPorCliente(int idCliente)
        {
            return _dMascota.ListarPorCliente(idCliente);
        }

        public string Registrar(Mascota mascota)
        {
            return _dMascota.Registrar(mascota);
        }

        public string Modificar(Mascota mascota)
        {
            return _dMascota.Modificar(mascota);
        }

        public string EliminarLogico(int id)
        {
            return _dMascota.EliminarLogico(id);
        }
    }
}