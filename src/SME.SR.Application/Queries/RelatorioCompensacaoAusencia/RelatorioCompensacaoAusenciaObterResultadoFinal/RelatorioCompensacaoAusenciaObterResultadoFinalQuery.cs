using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class RelatorioCompensacaoAusenciaObterResultadoFinalQuery : IRequest<RelatorioCompensacaoAusenciaDto>
    {
        public RelatorioCompensacaoAusenciaObterResultadoFinalQuery(FiltroRelatorioCompensacaoAusenciaDto filtros, Ue ue, Dre dre, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta> compensacoes, IEnumerable<AlunoHistoricoEscolar> alunos, IEnumerable<FrequenciaAluno> frequencias)
        {
            Filtros = filtros;
            Ue = ue;
            Dre = dre;
            ComponentesCurriculares = componentesCurriculares;
            Compensacoes = compensacoes;
            Alunos = alunos;
            Frequencias = frequencias;
        }

        public FiltroRelatorioCompensacaoAusenciaDto Filtros { get; set; }
        public Ue Ue { get; set; }
        public Dre Dre { get; set; }

        public IEnumerable<ComponenteCurricularPorTurma> ComponentesCurriculares { get; set; }

        public IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta> Compensacoes { get; set; }
        public IEnumerable<AlunoHistoricoEscolar> Alunos { get; set; }

        public IEnumerable<FrequenciaAluno> Frequencias { get; set; }


    }
}
