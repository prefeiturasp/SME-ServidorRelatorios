using System.Collections.Generic;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application.Queries.RelatorioFaltasFrequencia
{
    public class ObterRelatorioFrequenciaPdfPorUeQuery : IRequest<RelatorioFrequenciaUeDto>
    {
        public ObterRelatorioFrequenciaPdfPorUeQuery(FiltroRelatorioFrequenciasDto filtro, List<RelatorioFrequenciaTurmaAnoDto> relatorioFrequenciaTurmaAnoDto, IEnumerable<PeriodoEscolar> periodosEscolares, IEnumerable<Data.ComponenteCurricular> componentes, IEnumerable<AlunoTurma> alunos, IEnumerable<Turma> turmas, bool deveAdicionarFinal, bool mostrarSomenteFinal)
        {
            Filtro = filtro;
            PeriodosEscolares = periodosEscolares;
            Componentes = componentes;
            Alunos = alunos;
            Turmas = turmas;
            DeveAdicionarFinal = deveAdicionarFinal;
            MostrarSomenteFinal = mostrarSomenteFinal;
            RelatorioFrequenciaTurmaAnoDto = relatorioFrequenciaTurmaAnoDto;
        }
        
        public FiltroRelatorioFrequenciasDto Filtro { get; set; }
        public IEnumerable<PeriodoEscolar> PeriodosEscolares { get; set; }
        public IEnumerable<Data.ComponenteCurricular> Componentes { get; set; }
        public IEnumerable<AlunoTurma> Alunos { get; set; }
        public IEnumerable<Turma> Turmas { get; set; }
        public bool DeveAdicionarFinal { get; set; }
        public bool MostrarSomenteFinal { get; set; }
        public List<RelatorioFrequenciaTurmaAnoDto> RelatorioFrequenciaTurmaAnoDto { get; set; }
    }
}
