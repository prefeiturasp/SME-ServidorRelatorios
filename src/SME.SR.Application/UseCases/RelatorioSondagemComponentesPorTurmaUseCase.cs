using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemComponentesPorTurmaUseCase : IRelatorioSondagemComponentesPorTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemComponentesPorTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemComponentesPorTurmaFiltroDto>();

            //Obter a data do periodo\\
            var dataDoPeriodo = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(filtros.Semestre, filtros.AnoLetivo));


            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(filtros.TurmaCodigo, dataDoPeriodo));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");


            var relatorio = await ObterDadosRelatorio(filtros, alunosDaTurma);

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");
            

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemComponentesPorTurma", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }

        private async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> ObterDadosRelatorio(RelatorioSondagemComponentesPorTurmaFiltroDto filtros, IEnumerable<Aluno> alunos)
        {
            return await mediator.Send(
               new ObterRelatorioSondagemComponentesPorTurmaQuery()
               {
                    AnoLetivo = filtros.AnoLetivo,
                    ComponenteCurricular = filtros.ComponenteCurricularId,
                    DreCodigo = filtros.DreCodigo,
                    Proficiencia = filtros.ProficienciaId,
                    Semestre = filtros.Semestre,
                    TurmaCodigo = filtros.TurmaCodigo,
                    UeCodigo = filtros.UeCodigo,
                    UsuarioRF = filtros.UsuarioRF,
                    alunos = alunos,
                    Ano = filtros.Ano
               });
        }
    }
}
