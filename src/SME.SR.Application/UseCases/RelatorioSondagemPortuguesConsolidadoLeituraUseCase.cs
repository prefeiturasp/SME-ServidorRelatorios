using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    class RelatorioSondagemPortuguesConsolidadoLeituraUseCase : IRelatorioSondagemPortuguesConsolidadoLeituraUseCase
    {
        private readonly IMediator mediator;
        public RelatorioSondagemPortuguesConsolidadoLeituraUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto>();

            var semestre = (filtros.Bimestre <= 2) ? 1 : 2;

            var dataDoPeriodo = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(semestre, filtros.AnoLetivo));

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(Int32.Parse(filtros.TurmaCodigo), dataDoPeriodo));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto relatorio = new RelatorioSondagemPortuguesConsolidadoLeituraRelatorioDto()
            {
                Cabecalho = await ObterCabecalho(filtros, dataDoPeriodo),
                Planilhas = await ObterPlanilhas(filtros)
            };

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            var mensagemDaNotificacao = $"Este é o relatório de Sondagem de Português ({relatorio.Cabecalho.Proficiencia}) da turma {relatorio.Cabecalho.Turma} da {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre})";
            var mensagemTitulo = $"Relatório de Sondagem (Português) - {relatorio.Cabecalho.Ue} ({relatorio.Cabecalho.Dre}) - {relatorio.Cabecalho.Turma}";

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemPortuguesConsolidadoLeitura", relatorio, request.CodigoCorrelacao, mensagemDaNotificacao, mensagemTitulo));
        }

        private async Task<RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto> ObterCabecalho(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros, DateTime periodo)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorCodigoRfQuery() { UsuarioRf = filtros.UsuarioRF });

            return await Task.FromResult(new RelatorioSondagemPortuguesConsolidadoLeituraCabecalhoDto()
            {
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = filtros.DreCodigo,
                Periodo = periodo.ToString("dd/MM/yyyy"),
                Rf = filtros.UsuarioRF,
                Turma = filtros.TurmaCodigo.ToString(),
                Ue = filtros.UeCodigo,
                Usuario = usuario.Nome,
                AnoLetivo = filtros.AnoLetivo,
                AnoTurma = filtros.AnoTurma,
                ComponenteCurricular = ComponenteCurricularSondagemEnum.Portugues.Name(),
                Proficiencia = filtros.ProficienciaId.ToString()
            });
        }

        private async Task<List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>> ObterPlanilhas(RelatorioSondagemPortuguesConsolidadoLeituraFiltroDto filtros)
        {
            // TODO: Montar planilhas de dados
            var planilhas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto>();
            for (int i = 0; i <= 5; i++)
            {
                var linhas = new List<RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaLinhaDto>();
                #region Monta dados
                linhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaLinhaDto()
                {
                    Descricao = $"ORDEM { i } - COMPOSIÇÃO",
                    Ideia = "Ideia",
                    IdeiaPorcentagem = "%",
                    Resultado = "Resultado",
                    ResultadoPorcentagem = "%"
                });
                linhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaLinhaDto()
                {
                    Descricao = "Acertou",
                    Ideia = "60 alunos",
                    IdeiaPorcentagem = "60%",
                    Resultado = "60 alunos",
                    ResultadoPorcentagem = "60%"
                });
                linhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaLinhaDto()
                {
                    Descricao = "Errou",
                    Ideia = "30 alunos",
                    IdeiaPorcentagem = "30%",
                    Resultado = "30 alunos",
                    ResultadoPorcentagem = "30%"
                });
                linhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaLinhaDto()
                {
                    Descricao = "Não Resolveu",
                    Ideia = "10 alunos",
                    IdeiaPorcentagem = "10%",
                    Resultado = "10 alunos",
                    ResultadoPorcentagem = "10%"
                });
                linhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaLinhaDto()
                {
                    Descricao = "Total",
                    Ideia = "100 alunos",
                    IdeiaPorcentagem = "100%",
                    Resultado = "100 alunos",
                    ResultadoPorcentagem = "100%"
                });
                #endregion
                planilhas.Add(new RelatorioSondagemPortuguesConsolidadoLeituraPlanilhaDto()
                {
                    Linhas = linhas
                });
            }
            return await Task.FromResult(planilhas);
        }
    }
}
