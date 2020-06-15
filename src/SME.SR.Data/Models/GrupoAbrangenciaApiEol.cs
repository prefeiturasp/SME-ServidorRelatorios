using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Data
{
    public class GrupoAbrangenciaApiEol
    {
        public GrupoAbrangenciaApiEol()
        {
            CargosId = new HashSet<int>();
        }
        public Guid GrupoID { get; set; }
        public ICollection<int> CargosId { get; set; }
        public GruposSGP Grupo { get; set; }
        public int TipoFuncaoAtividade { get; set; }
        public TipoAbrangencia Abrangencia { get; set; }
        public bool EhPerfilManual { get; set; }

        public void AdicionarCargo(int? cargoId)
        {
            if (cargoId.HasValue)
                CargosId.Add(cargoId.Value);
        }
    }
}
