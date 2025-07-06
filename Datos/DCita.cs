using System;
using System.Collections.Generic;
using System.Linq;

namespace Datos
{
    public class DCita
    {
        public string Registrar(Cita cita)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    // Validación adicional
                    if (ExisteCitaEnMismoHorario(cita, ctx))
                        return "Ya existe una cita programada en ese horario para el veterinario.";

                    cita.FechaCreacion = DateTime.Now;
                    cita.Eliminado = false;
                    ctx.Cita.Add(cita);
                    ctx.SaveChanges();
                }
                return "Cita registrada exitosamente.";
            }
            catch (Exception ex)
            {
                // Loggear el error (implementar logging)
                return $"Error al registrar cita: {ex.Message}";
            }
        }
        public string EliminarLogico(int id)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    var cli = ctx.Cita.Find(id);
                    if (cli == null) return "No existe.";
                    cli.Eliminado = true;
                    cli.FechaModificacion = DateTime.Now;
                    ctx.SaveChanges();
                }
                return "Cita eliminado lógicamente.";
            }
            catch (Exception ex) { return ex.Message; }
        }
        public string Modificar(Cita cita)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    var citaExistente = ctx.Cita.Find(cita.IDCita);
                    if (citaExistente == null || citaExistente.IDEstadoCita != 1) // 1 = Programada
                        return "La cita no existe o no está en estado Programada.";

                    // Validar que no se modifique a un horario ocupado
                    if (ExisteOtraCitaEnMismoHorario(cita, ctx))
                        return "El veterinario ya tiene otra cita programada en ese horario.";

                    // Actualizar solo campos permitidos
                    citaExistente.FechaHora = cita.FechaHora;
                    citaExistente.Motivo = cita.Motivo;
                    citaExistente.IDTipoCita = cita.IDTipoCita;
                    citaExistente.IDMascota = cita.IDMascota;
                    citaExistente.IDVeterinario = cita.IDVeterinario;
                    citaExistente.FechaModificacion = DateTime.Now;

                    ctx.SaveChanges();
                }
                return "Cita modificada exitosamente.";
            }
            catch (Exception ex)
            {
                return $"Error al modificar cita: {ex.Message}";
            }
        }

        public string Cancelar(int idCita, int idUsuario)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    var cita = ctx.Cita.Find(idCita);
                    if (cita == null) return "Cita no encontrada.";
                    if (cita.IDEstadoCita != 1) return "Solo se pueden cancelar citas Programadas.";

                    cita.IDEstadoCita = 3; // Cancelada
                    cita.FechaModificacion = DateTime.Now;
                    ctx.SaveChanges();
                }
                return "Cita cancelada exitosamente.";
            }
            catch (Exception ex)
            {
                return $"Error al cancelar cita: {ex.Message}";
            }
        }

        public string Completar(int idCita, string diagnostico)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    var cita = ctx.Cita.Find(idCita);
                    if (cita == null) return "Cita no encontrada.";
                    if (cita.IDEstadoCita != 1) return "Solo se pueden completar citas Programadas.";
                    if (DateTime.Now < cita.FechaHora.AddMinutes(-30)) // Permite completar 30 mins antes
                        return "No se puede completar la cita antes de su horario programado.";
                    if (string.IsNullOrWhiteSpace(diagnostico))
                        return "El diagnóstico es obligatorio.";

                    cita.Diagnostico = diagnostico;
                    cita.IDEstadoCita = 2; // Completada
                    cita.FechaModificacion = DateTime.Now;
                    ctx.SaveChanges();
                }
                return "Cita completada exitosamente.";
            }
            catch (Exception ex)
            {
                return $"Error al completar cita: {ex.Message}";
            }
        }

        public List<Cita> ListarPorVeterinario(int idVeterinario)
        {
            using (var ctx = new BDEFEntities())
            {
                return ctx.Cita
                    .Where(c => c.IDVeterinario == idVeterinario &&
                               c.IDEstadoCita == 1 && // Programada
                               !c.Eliminado &&
                               c.FechaHora >= DateTime.Now)
                    .OrderBy(c => c.FechaHora)
                    .ToList();
            }
        }

        public Cita ObtenerPorId(int idCita)
        {
            using (var ctx = new BDEFEntities())
            {
                return ctx.Cita.Find(idCita);
            }
        }

        // Métodos auxiliares privados
        private bool ExisteCitaEnMismoHorario(Cita nuevaCita, BDEFEntities ctx)
        {
            return ctx.Cita.Any(c =>
                c.IDVeterinario == nuevaCita.IDVeterinario &&
                c.FechaHora == nuevaCita.FechaHora &&
                !c.Eliminado &&
                c.IDEstadoCita == 1); // Programada
        }

        private bool ExisteOtraCitaEnMismoHorario(Cita citaModificada, BDEFEntities ctx)
        {
            return ctx.Cita.Any(c =>
                c.IDVeterinario == citaModificada.IDVeterinario &&
                c.FechaHora == citaModificada.FechaHora &&
                c.IDCita != citaModificada.IDCita && // Excluir la cita actual
                !c.Eliminado &&
                c.IDEstadoCita == 1); // Programada
        }
    }
}