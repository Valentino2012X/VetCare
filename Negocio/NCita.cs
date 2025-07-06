using Datos;
using System;
using System.Collections.Generic;

namespace Negocio
{
    public class NCita
    {
        private readonly DCita _dCita;

        public NCita()
        {
            _dCita = new DCita();
        }

        // Inyección de dependencias para testing
        public NCita(DCita dCita)
        {
            _dCita = dCita;
        }

        public string Registrar(Cita cita)
        {
            // Validaciones de negocio adicionales
            if (cita.FechaHora < DateTime.Now.AddMinutes(30))
                return "Las citas deben programarse con al menos 30 minutos de anticipación.";

            if (string.IsNullOrWhiteSpace(cita.Motivo))
                return "El motivo de la cita es obligatorio.";

            return _dCita.Registrar(cita);
        }

        public string Modificar(Cita cita)
        {
            // Validaciones de negocio
            if (cita.FechaHora < DateTime.Now)
                return "No se puede modificar una cita al pasado.";

            return _dCita.Modificar(cita);
        }

        public string Cancelar(int idCita, int idUsuario)
        {
            // Aquí podrías agregar lógica para verificar permisos del usuario
            return _dCita.Cancelar(idCita, idUsuario);
        }

        public string Completar(int idCita, string diagnostico)
        {
            // Validaciones adicionales de negocio
            if (string.IsNullOrWhiteSpace(diagnostico))
                return "El diagnóstico es obligatorio.";

            return _dCita.Completar(idCita, diagnostico);
        }

        public List<Cita> ListarPorVeterinario(int idVeterinario)
        {
            return _dCita.ListarPorVeterinario(idVeterinario);
        }

        public Cita ObtenerCita(int idCita)
        {
            return _dCita.ObtenerPorId(idCita);
        }

        public string EliminarLogico(int idCita)
        {
            // Validar que la cita no esté completada antes de eliminarla
            var cita = _dCita.ObtenerPorId(idCita);
            if (cita?.IDEstadoCita == 2) // Completada
                return "No se pueden eliminar citas completadas.";

            return _dCita.EliminarLogico(idCita);
        }
    }
}