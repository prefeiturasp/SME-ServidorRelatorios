using SME.SR.Infra.CDEP;
using System;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class FiltroRelatorioTitulosMaisPesquisados
    {
        public string Usuario { get; set; }
        public string UsuarioRF { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<TipoAcervo> TipoAcervos { get; set; }
    }
}
