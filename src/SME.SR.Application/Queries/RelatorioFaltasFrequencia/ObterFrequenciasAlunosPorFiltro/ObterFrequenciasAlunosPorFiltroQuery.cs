using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciasAlunosPorFiltroQuery : IRequest<IEnumerable<FrequenciaAlunoRetornoDto>>
    {
        public ObterFrequenciasAlunosPorFiltroQuery(string[] codigosturma, string componenteCurricularId, int bimestre)
        {
            Codigosturma = codigosturma;
            ComponenteCurricularId = componenteCurricularId;
            Bimestre = bimestre;
        }

        public ObterFrequenciasAlunosPorFiltroQuery(string codigoTurma, string componenteCurricularId, int bimestre)
        {
            Codigosturma = new string[] { codigoTurma };
            ComponenteCurricularId = componenteCurricularId;
            Bimestre = bimestre;
        }

        public string[] Codigosturma { get; set; }
        public string ComponenteCurricularId { get; set; }
        public int Bimestre { get; set; }
    }
}
