using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AtribuicaoEsporadicaDto
    {
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public string NomeUsuario { get; set; }
        public string Cargo { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string DataAtribuicao { get; set; }
        public string AtribuidoPor { get; set; }
    }
}
