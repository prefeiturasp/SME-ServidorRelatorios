using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorUeAnosModalidadeSemestreQuery : IRequest<IEnumerable<TurmaFiltradaUeCicloAnoDto>>
    {
        public ObterTurmasPorUeAnosModalidadeSemestreQuery(string ueId, string[] anos, Modalidade? modalidade, int? semestre)
        {
            UeId = ueId;
            Anos = anos;
            Modalidade = modalidade;
            Semestre = semestre;
        }
        public string UeId { get; set; }
        public string[] Anos { get; set; }
        public Modalidade? Modalidade { get; set; }
        public int? Semestre { get; set; }
    }
}