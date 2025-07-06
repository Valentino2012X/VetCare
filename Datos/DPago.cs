using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class DPago
    {
        public string Registrar(Pago p)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    // Valida que la cita esté completada y no cancelada
                    var cita = ctx.Cita.Find(p.IDCita);
                    if (cita == null || cita.IDEstadoCita != 2) // 2 = Completada
                        return "Solo se paga citas completadas.";

                    // Evita doble pago
                    bool yaPagado = ctx.Pago.Any(pg => pg.IDCita == p.IDCita);
                    if (yaPagado) return "La cita ya fue pagada.";

                    p.FechaPago = DateTime.Now;
                    p.FechaCreacion = DateTime.Now;
                    ctx.Pago.Add(p);
                    ctx.SaveChanges();
                }
                return "Pago registrado.";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public List<Pago> ListarPendientes()
        {
            using (var ctx = new BDEFEntities())
            {
                // Citas completadas SIN pago
                var pagadas = ctx.Pago.Select(pg => pg.IDCita);
                return ctx.Cita
                          .Where(c => c.IDEstadoCita == 2 && !pagadas.Contains(c.IDCita))
                          .Select(c => new Pago { IDCita = c.IDCita })
                          .ToList();
            }
        }

        public List<Pago> ListarPagados()
        {
            using (var ctx = new BDEFEntities())
            {
                return ctx.Pago.Include("Cita").ToList();
            }
        }
    }
}
