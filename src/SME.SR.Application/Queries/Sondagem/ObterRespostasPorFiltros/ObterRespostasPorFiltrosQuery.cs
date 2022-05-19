using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRespostasPorFiltrosQuery : IRequest<IEnumerable<SondagemAutoralDto>>
    {
        public string DreCodigo { get; internal set; }
        public string UeCodigo { get; internal set; }
        public string GrupoId { get; internal set; }
        public string PeriodoId { get; internal set; }
        public int Bimestre { get; set; }
        public int? TurmaAno { get; internal set; }
        public int? AnoLetivo { get; internal set; }
        public ComponenteCurricularSondagemEnum? ComponenteCurricular { get; internal set; }
    }
}
