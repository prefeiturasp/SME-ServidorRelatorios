using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class BoletimEscolarCabecalhoDto
    {
        public string NomeDre { get; set; }

        public string NomeUe { get; set; }

        public string NomeTurma { get; set; }

        public string Aluno { get; set; }
        public string NomeAlunoOrdenacao { get; set; }

        public string CodigoEol { get; set; }

        public string Data { get; set; }

        public string FrequenciaGlobal { get; set; }
        public string AnoLetivo { get; set; }

        public string DataImpressao { get { return DateTime.Today.ToString("dd/MM/yyyy"); } }

        public string NomeNumeroTurma { get; set; }
    }
}
