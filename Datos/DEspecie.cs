using System.Collections.Generic;
using System.Linq;

namespace Datos
{
    public class DEspecie
    {
        private readonly BDEFEntities _context;

        public DEspecie(BDEFEntities context)
        {
            _context = context;
        }

        public List<Especie> ObtenerEspeciesParaComboBox()
        {
            // Solo obtenemos los datos necesarios para el ComboBox
            return _context.Especie
                         .OrderBy(e => e.Nombre)
                         .ToList();
        }
    }
}