using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class DMascota
    {
        private readonly BDEFEntities _context;

        public DMascota(BDEFEntities context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Mascota> ListarPorCliente(int idCliente)
        {
            return _context.Mascota
                .Where(m => m.IDCliente == idCliente && !m.Eliminado)
                .ToList();
        }

        public string Registrar(Mascota m)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    m.FechaCreacion = DateTime.Now;
                    ctx.Mascota.Add(m);
                    ctx.SaveChanges();
                }
                return "Mascota registrada.";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Modificar(Mascota m)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    var mas = ctx.Mascota.Find(m.IDMascota);
                    if (mas == null) return "No existe.";
                    mas.Nombre = m.Nombre;
                    mas.Edad = m.Edad;
                    mas.IDEspecie = m.IDEspecie;
                    mas.Sexo = m.Sexo;
                    mas.FechaModificacion = DateTime.Now;
                    ctx.SaveChanges();
                }
                return "Mascota modificada.";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string EliminarLogico(int id)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    var mas = ctx.Mascota.Find(id);
                    if (mas == null) return "No existe.";
                    mas.Eliminado = true;
                    mas.FechaModificacion = DateTime.Now;
                    ctx.SaveChanges();
                }
                return "Mascota eliminada lógicamente.";
            }
            catch (Exception ex) { return ex.Message; }
        }
    }
}
