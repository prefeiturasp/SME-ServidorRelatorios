using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class GraficoPAPDto
    {
        public string DreNome { get; set; }

        public string UeNome { get; set; }

        public int AnoLetivo { get; set; }

        public string Ciclo { get; set; }

        public string Ano { get; set; }

        public string Turma { get; set; }

        public string Periodo { get; set; }

        public string UsuarioNome { get; set; }

        public string UsuarioRF { get; set; }

        public string Data { get; set; }

        public bool EhEncaminhamento { get; set; }

        public List<ResumoPAPGraficoDto> GraficosDto { get; set; }
    }
}
