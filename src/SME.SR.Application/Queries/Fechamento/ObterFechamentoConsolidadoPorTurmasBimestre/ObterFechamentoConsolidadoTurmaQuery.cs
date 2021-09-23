using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFechamentoConsolidadoTurmaQuery : IRequest<IEnumerable<FechamentoConsolidadoTurmaDto>>
    {
        public string DreCodigo { get; internal set; }
        public int ModalidadeCodigo { get; internal set; }
        public int AnoLetivo { get; internal set; }
        public int[] Bimestres { get; internal set; }
        public SituacaoFechamento? Situacao { get; internal set; }
        public int Semestre { get; internal set; }

        public ObterFechamentoConsolidadoTurmaQuery(string dreCodigo, int modalidadeCodigo, int anoLetivo, int[] bimestres, SituacaoFechamento? situacao, int semestre = 0)
        {
            DreCodigo = dreCodigo;
            ModalidadeCodigo = modalidadeCodigo;
            AnoLetivo = anoLetivo;
            Bimestres = bimestres;
            Situacao = situacao;
            Semestre = semestre;
        }
    }
}
