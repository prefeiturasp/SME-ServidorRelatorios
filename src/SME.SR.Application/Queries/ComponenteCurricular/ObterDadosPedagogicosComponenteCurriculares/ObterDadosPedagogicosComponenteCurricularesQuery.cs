using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDadosPedagogicosComponenteCurricularesQuery : IRequest<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>>
    {
        public ObterDadosPedagogicosComponenteCurricularesQuery() { }
        public ObterDadosPedagogicosComponenteCurricularesQuery(string dreCodigo, string ueCodigo, long[] componentesCurriculares, int anoLetivo, long[] codigoTurmas, string professorNome, string professorCodigo, List<int> bimestres)
        {
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
            ComponentesCurriculares = componentesCurriculares;
            TurmasId = codigoTurmas;
            AnoLetivo = anoLetivo;
            ProfessorNome = professorNome;
            ProfessorCodigo = professorCodigo;
            Bimestres = bimestres;
        }

        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long[] ComponentesCurriculares { get; set; }
        public int AnoLetivo { get; set; }
        public long[] TurmasId { get; set; }
        public string ProfessorNome { get; set; }
        public string ProfessorCodigo { get; set; }
        public List<int> Bimestres { get; set; }
    }
}
