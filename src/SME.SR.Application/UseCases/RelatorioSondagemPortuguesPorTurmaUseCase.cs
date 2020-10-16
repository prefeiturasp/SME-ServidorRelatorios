using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemPortuguesPorTurmaUseCase : IRelatorioSondagemPortuguesPorTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemPortuguesPorTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
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

            var mensagemDaNotificacao = $"Este é o relatório de Sondagem de Português ({relatorio.Cabecalho.Proficiencia}) da turma {relatorio.Cabecalho.Turma} da {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre})";
            var mensagemTitulo = $"Relatório de Sondagem (Português) - {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre}) - {relatorio.Cabecalho.Turma}";

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesPorTurma", relatorio, request.CodigoCorrelacao, mensagemDaNotificacao, mensagemTitulo));
        }

        private async Task<RelatorioSondagemPortuguesPorTurmaRelatorioDto> ObterDadosRelatorio(RelatorioSondagemComponentesPorTurmaFiltroDto filtros, IEnumerable<Aluno> alunos)
        {
            return await mediator.Send(
               new ObterRelatorioSondagemPortuguesPorTurmaQuery()
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
