using MediatR;
using SME.SR.Infra.Dtos;
using System.Collections.Generic;

namespace SME.SR.Application
{
    class ObterAusenciaPorAlunoTurmaBimestreQuery : IRequest<IEnumerable<AusenciaBimestreDto>>
    {
        public ObterAusenciaPorAlunoTurmaBimestreQuery(string[] alunosCodigo, string turmaCodigo, string bimestre, string[] componentesCurricularesIds = null)
        {
            AlunosCodigo = alunosCodigo;
            TurmaCodigo = turmaCodigo;
            Bimestre = bimestre;
            ComponentesCurricularesIds = componentesCurricularesIds;
        }

        public string[] AlunosCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public string Bimestre { get; set; }
        public string[] ComponentesCurricularesIds { get; set; }
    }
}
