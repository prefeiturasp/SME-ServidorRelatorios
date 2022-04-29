using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciasAlunosConsolidadoQuery : IRequest<IEnumerable<FrequenciaAlunoConsolidadoRelatorioDto>>
    {
        public ObterFrequenciasAlunosConsolidadoQuery(string[] codigosturma, string componenteCurricularId, string bimestre)
        {
            Codigosturma = codigosturma;
            ComponenteCurricularId = componenteCurricularId;
            Bimestre = bimestre;
        }

        public string[] Codigosturma { get; set; }
        public string ComponenteCurricularId { get; set; }
        public string Bimestre { get; set; }
    }
}
