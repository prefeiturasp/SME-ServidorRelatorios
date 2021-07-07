using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasRelatorioAcompanhamentoFechamentoQuery : IRequest<IEnumerable<Turma>>
    {
        public string[] CodigosTurma { get; set; }

        public string CodigoUe { get; set; }

        public Modalidade Modalidade { get; set; }

        public int AnoLetivo { get; set; }

        public int Semestre { get; set; }

        public Usuario Usuario { get; set; }
        public bool ConsideraHistorico { get; set; }

        public SituacaoFechamento? SituacaoFechamento { get; set; }
        public SituacaoConselhoClasse? SituacaoConselhoClasse { get; set; }

        public int[] Bimestres { get; set; }
    }
}
