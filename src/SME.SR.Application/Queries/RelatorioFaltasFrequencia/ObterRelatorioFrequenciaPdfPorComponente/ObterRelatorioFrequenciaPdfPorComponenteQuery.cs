using System.Collections.Generic;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorComponenteQuery : IRequest<string>
    {
        public ObterRelatorioFrequenciaPdfPorComponenteQuery(FiltroRelatorioFrequenciasDto filtro, IEnumerable<PeriodoEscolar> periodosEscolares, IEnumerable<AlunoTurma> alunos, bool deveAdicionarFinal, bool mostrarSomenteFinal)
        {
            Filtro = filtro;
            PeriodosEscolares = periodosEscolares;
            Alunos = alunos;
            DeveAdicionarFinal = deveAdicionarFinal;
            MostrarSomenteFinal = mostrarSomenteFinal;
        }
        
        public FiltroRelatorioFrequenciasDto Filtro { get; set; }
        public IEnumerable<PeriodoEscolar> PeriodosEscolares { get; set; }
        public IEnumerable<AlunoTurma> Alunos { get; set; }
        public bool DeveAdicionarFinal { get; set; }
        public bool MostrarSomenteFinal { get; set; }
    }
}
