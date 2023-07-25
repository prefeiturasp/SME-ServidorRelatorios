using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterFrequenciaAlunoDiariaQuery : IRequest<IEnumerable<RelatorioFrequenciaIndividualDiariaAlunoDto>>
    {
        public ObterFrequenciaAlunoDiariaQuery(string bimestre, string[] codigosAlunos, string turmaCodigo, string[] componentesCurricularesIds, string professorTitularRf = null)
        {
            Bimestre = bimestre;
            CodigosAlunos = codigosAlunos;
            TurmaCodigo = turmaCodigo;
            ComponentesCurricularesIds = componentesCurricularesIds;
            ProfessorTitularRf = professorTitularRf;
        }

        public string Bimestre { get; set; }
        public string[] CodigosAlunos { get; set; }
        public string TurmaCodigo { get; set; }
        public string[] ComponentesCurricularesIds { get; set; }
        public string ProfessorTitularRf { get; set; }
    }
}
