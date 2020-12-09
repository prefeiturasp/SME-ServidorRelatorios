using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class DadosRelatorioUsuariosDto
    {
        public IEnumerable<PerfilUsuarioDto> PerfisSme { get; set; }

        public IEnumerable<DreUsuarioDto> Dres { get; set; }
    }
}
