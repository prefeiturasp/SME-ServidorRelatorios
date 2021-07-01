using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoFechamentoQuery : IRequest<RelatorioAcompanhamentoFechamentoPorUeDto>
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public Modalidade Modalidade { get; set; }
        public int Semestre { get; set; }
        public List<string> TurmasCodigo { get; set; }
        public List<int> Bimestres { get; set; }
        public SituacaoFechamento? SituacaoFechamento { get; set; }
        public SituacaoConselhoClasse? SituacaoConselhoClasse { get; set; }
        public bool ListarPendencias { get; set; }
        public Usuario Usuario { get; set; }
    }
}
