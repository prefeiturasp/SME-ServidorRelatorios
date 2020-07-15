using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioFaltasFrequenciasQuery : IRequest<FiltroRelatorioFaltasFrequenciasDto>
    {
        public int AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public Modalidade Modalidade { get; set; }
        public int? Semestre { get; set; }
        public IEnumerable<string> AnosEscolares { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
        public IEnumerable<int> Bimestres { get; set; }
        public TipoRelatorioFaltasFrequencia TipoRelatorio { get; set; }
        public CondicoesRelatorioFaltasFrequencia Condicao { get; set; }
        public int ValorCondicao { get; set; }
        public TipoFormatoRelatorio TipoFormatoRelatorio { get; set; }
    }
}
