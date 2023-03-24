using System.Collections.Generic;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorTurmaQuery : IRequest<string>
    {
        public ObterRelatorioFrequenciaPdfPorTurmaQuery(FiltroRelatorioFrequenciasDto filtro, IEnumerable<PeriodoEscolar> periodosEscolares, IEnumerable<Data.ComponenteCurricular> componentes, IEnumerable<AlunoTurma> alunos, bool deveAdicionarFinal, bool mostrarSomenteFinal)
        {
            Filtro = filtro;
            PeriodosEscolares = periodosEscolares;
            Componentes = componentes;
            Alunos = alunos;
            DeveAdicionarFinal = deveAdicionarFinal;
            MostrarSomenteFinal = mostrarSomenteFinal;
        }
        
        public FiltroRelatorioFrequenciasDto Filtro { get; set; }
        public IEnumerable<PeriodoEscolar> PeriodosEscolares { get; set; }
        public IEnumerable<Data.ComponenteCurricular> Componentes { get; set; }
        public IEnumerable<AlunoTurma> Alunos { get; set; }
        public bool DeveAdicionarFinal { get; set; }
        public bool MostrarSomenteFinal { get; set; }
    }
}
