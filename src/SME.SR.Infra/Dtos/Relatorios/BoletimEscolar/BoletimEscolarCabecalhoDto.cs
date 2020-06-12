using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.Relatorios.BoletimEscolar
{
    public class BoletimEscolarCabecalhoDto
    {
        public string NomeDre { get; set; }

        public string NomeUe { get; set; }

        public string NomeTurma { get; set; }

        public string Aluno { get; set; }

        public DateTime Data { get; set; }
    }
}
