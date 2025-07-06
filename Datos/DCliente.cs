using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class DCliente
    {
        public List<Cliente> ListarTodo()
        {
            var lista = new List<Cliente>();
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    lista = ctx.Cliente
                               .Where(c => !c.Eliminado)
                               .ToList();
                }
            }
            catch { /* log o manejo */ }

            return lista;
        }

        public string Registrar(Cliente cliente)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    cliente.FechaCreacion = DateTime.Now;
                    ctx.Cliente.Add(cliente);
                    ctx.SaveChanges();
                }
                return "Cliente registrado.";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Modificar(Cliente cliente)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    var cli = ctx.Cliente.Find(cliente.IDCliente);
                    if (cli == null) return "No existe.";
                    cli.Nombre = cliente.Nombre;
                    cli.Apellido = cliente.Apellido;
                    cli.DNI = cliente.DNI;
                    cli.Telefono = cliente.Telefono;
                    cli.Direccion = cliente.Direccion;
                    cli.FechaModificacion = DateTime.Now;
                    ctx.SaveChanges();
                }
                return "Cliente modificado.";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string EliminarLogico(int id)
        {
            try
            {
                using (var ctx = new BDEFEntities())
                {
                    var cli = ctx.Cliente.Find(id);
                    if (cli == null) return "No existe.";
                    cli.Eliminado = true;
                    cli.FechaModificacion = DateTime.Now;
                    ctx.SaveChanges();
                }
                return "Cliente eliminado lógicamente.";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public Cliente Obtener(int id)
        {
            using (var ctx = new BDEFEntities())
            {
                return ctx.Cliente.Find(id);
            }
        }
    }
}
