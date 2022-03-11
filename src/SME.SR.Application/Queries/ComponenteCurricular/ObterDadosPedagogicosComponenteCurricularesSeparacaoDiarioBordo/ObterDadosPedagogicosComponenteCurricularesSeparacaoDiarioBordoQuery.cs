using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterDadosPedagogicosComponenteCurricularesSeparacaoDiarioBordoQuery : IRequest<List<RelatorioAcompanhamentoRegistrosPedagogicosBimestreDto>>
    {
        public ObterDadosPedagogicosComponenteCurricularesSeparacaoDiarioBordoQuery() { }
        public ObterDadosPedagogicosComponenteCurricularesSeparacaoDiarioBordoQuery(string dreCodigo, string ueCodigo, long[] componentesCurriculares,
            int anoLetivo, List<string> codigoTurmas, string professorCodigo, List<int> bimestres, Modalidade modalidade, int semestre)
        {
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
            ComponentesCurriculares = componentesCurriculares;
            TurmasCodigo = codigoTurmas;
            AnoLetivo = anoLetivo;
            ProfessorCodigo = professorCodigo;
            Bimestres = bimestres;
            Modalidade = modalidade;
            Semestre = semestre; 
        }

        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long[] ComponentesCurriculares { get; set; }
        public int AnoLetivo { get; set; }
        public List<string> TurmasCodigo { get; set; }
        public string ProfessorNome { get; set; }
        public string ProfessorCodigo { get; set; }
        public List<int> Bimestres { get; set; }
        public Modalidade Modalidade { get; set; }
        public int Semestre { get; set; }
    }
}
