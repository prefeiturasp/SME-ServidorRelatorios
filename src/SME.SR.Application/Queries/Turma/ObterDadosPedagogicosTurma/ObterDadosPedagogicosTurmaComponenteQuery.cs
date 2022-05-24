using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterDadosPedagogicosTurmaComponenteQuery : IRequest<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreInfantilComponenteDto>>
    {
        public ObterDadosPedagogicosTurmaComponenteQuery() { }
        public ObterDadosPedagogicosTurmaComponenteQuery(string dreCodigo, string ueCodigo, int anoLetivo, string professorNome, string professorCodigo,
            List<int> bimestres = null, List<string> codigoTurmas = null, List<long> componentesCurricularesIds = null)
        {
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
            TurmasId = codigoTurmas;
            AnoLetivo = anoLetivo;
            ProfessorNome = professorNome;
            ProfessorCodigo = professorCodigo;
            Bimestres = bimestres;
            ComponentesCurricularesIds = componentesCurricularesIds;
        }

        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public int AnoLetivo { get; set; }
        public List<string> TurmasId { get; set; }
        public string ProfessorNome { get; set; }
        public string ProfessorCodigo { get; set; }
        public List<int> Bimestres { get; set; }
        public List<long> ComponentesCurricularesIds { get; set; }
    }
}
