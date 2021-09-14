using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasSituacaoConsolidacaoQuery : IRequest<IEnumerable<Turma>>
    {
        public ObterTurmasSituacaoConsolidacaoQuery(string[] codigos, SituacaoFechamento? situacaoFechamento, SituacaoConselhoClasse? situacaoConselhoClasse, int[] bimestres)
        {
            Codigos = codigos;
            SituacaoFechamento = situacaoFechamento;
            SituacaoConselhoClasse = situacaoConselhoClasse;
            Bimestres = bimestres;
        }


        public string[] Codigos { get; set; }
        public SituacaoFechamento? SituacaoFechamento { get; set; }
        public SituacaoConselhoClasse? SituacaoConselhoClasse { get; set; }
        public int[] Bimestres { get; set; }

    }
}

