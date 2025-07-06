using System.Collections.Generic;
using Datos;

namespace Negocio
{
    public class NEspecie
    {
        private readonly DEspecie _dEspecie;

        public NEspecie(BDEFEntities context)
        {
            _dEspecie = new DEspecie(context);
        }

        public List<Especie> ObtenerEspeciesParaComboBox()
        {
            return _dEspecie.ObtenerEspeciesParaComboBox();
        }
    }
}
