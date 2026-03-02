using MediatR;
using SME.SR.Application.Commands.NovoSondagem.GerarRelatorioSondagemQuestionarioHtmlParaPdf;
using SME.SR.Application.Commands.Sondagem.EscritaTurma;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.NovoSondagem;
using SME.SR.Infra.Interfaces.Integracoes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioSondagemQuestionarioUseCase : IRelatorioSondagemQuestionarioUseCase
    {
        private readonly IMediator mediator;
        private readonly ISondagemApiClient sondagemApiClient;
        private readonly ISolicitacaoRelatorioApiClient solicitacaoRelatorioApiClient;

        public RelatorioSondagemQuestionarioUseCase(
            IMediator mediator,
            ISondagemApiClient sondagemApiClient,
            ISolicitacaoRelatorioApiClient solicitacaoRelatorioApiClient)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.sondagemApiClient = sondagemApiClient ?? throw new ArgumentNullException(nameof(sondagemApiClient));
            this.solicitacaoRelatorioApiClient = solicitacaoRelatorioApiClient ?? throw new ArgumentNullException(nameof(solicitacaoRelatorioApiClient));
        }

        public async Task Executar(FiltroRelatorioDto filtroRelatorioDto)
        {
            var mensagem = filtroRelatorioDto.ObterObjetoFiltro<MensagemSondagemQuestionarioDto>();

            var filtro = new FiltroRelatorioSondagemQuestionarioDto
            {
                TurmaId = mensagem.FiltrosUsados.TurmaId,
                ProficienciaId = mensagem.FiltrosUsados.ProficienciaId,
                ComponenteCurricularId = mensagem.FiltrosUsados.ComponenteCurricularId,
                Modalidade = mensagem.FiltrosUsados.Modalidade,
                Ano = mensagem.FiltrosUsados.Ano,
                AnoLetivo = mensagem.FiltrosUsados.AnoLetivo,
                SemestreId = mensagem.FiltrosUsados.Semestre,
                UeCodigo = mensagem.FiltrosUsados.UeCodigo,
                BimestreId = mensagem.FiltrosUsados.BimestreId,
                ExtensaoRelatorio = mensagem.FiltrosUsados.ExtensaoRelatorio,
                SolicitacaoRelatorioId = mensagem.SolicitacaoRelatorioId,
                TipoRelatorio = mensagem.TipoRelatorio,
                StatusSolicitacao = mensagem.StatusSolicitacao,
                UsuarioQueSolicitou = mensagem.UsuarioQueSolicitou
            };

            if (filtro.TipoRelatorio == (int)TipoFormatoRelatorio.Xlsx)
            {
                await mediator.Send(new GerarRelatorioSondagemPorTurmaEscritaCommand(
                    filtroRelatorioDto.CodigoCorrelacao,
                    filtro.TurmaId,
                    filtro.ProficienciaId,
                    filtro.ComponenteCurricularId,
                    (Modalidade)filtro.Modalidade,
                    filtro.Ano,
                    filtro.AnoLetivo,
                    filtro.SemestreId,
                    filtroRelatorioDto.UsuarioLogadoRF,
                    filtro.UeCodigo,
                    filtro.BimestreId));
            }
            else if (filtro.TipoRelatorio == (int)TipoFormatoRelatorio.Pdf)
            {
                var tarefaApi = sondagemApiClient.ObterDadosQuestionarioAsync(filtro);
                var tarefaUsuario = mediator.Send(new ObterNomeUsuarioPorLoginQuery(filtroRelatorioDto.UsuarioLogadoRF));
                var tarefaDreUe = mediator.Send(new ObterDreUePorTurmaQuery(filtro.TurmaId.ToString()));
                var tarefaTurma = mediator.Send(new ObterTurmaQuery(filtro.TurmaId.ToString()));

                await Task.WhenAll(tarefaApi, tarefaUsuario, tarefaDreUe, tarefaTurma);

                var dadosApi = tarefaApi.Result;
                var nomeUsuario = tarefaUsuario.Result;
                var dreUe = tarefaDreUe.Result;
                var turma = tarefaTurma.Result;

                var modalidadeEnum = (Modalidade)filtro.Modalidade;
                var modalidadeNome = modalidadeEnum
                    .GetType()
                    .GetField(modalidadeEnum.ToString())
                    ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                    .Cast<DisplayAttribute>()
                    .FirstOrDefault()
                    ?.Name ?? modalidadeEnum.ToString();

                var pagina = new QuestionarioSondagemRelatorioDto
                {
                    AnoLetivo = filtro.AnoLetivo,
                    Dre = dreUe.DreNome,
                    Semestre = dadosApi.Semestre,
                    Turma = turma.NomeRelatorio,
                    UnidadeEducacional = $"{dreUe.UeCodigo} - {dreUe.UeNome}",
                    Modalidade = modalidadeNome,
                    Proficiencia = dadosApi.TituloTabelaRespostas,
                    DataImpressao = DateTime.Now,
                    Usuario = string.IsNullOrWhiteSpace(nomeUsuario) ? "SISTEMA" : nomeUsuario,
                    TituloTabelaRespostas = dadosApi.TituloTabelaRespostas,
                    Estudantes = dadosApi.Estudantes?.Select(e => new EstudanteQuestionarioDto
                    {
                        NumeroAlunoChamada = e.NumeroAlunoChamada,
                        LinguaPortuguesaSegundaLingua = e.LinguaPortuguesaSegundaLingua,
                        Codigo = e.Codigo,
                        Raca = e.Raca,
                        Genero = e.Genero,
                        Nome = e.NomeRelatorio,
                        Pap = e.Pap,
                        Aee = e.Aee,
                        PossuiDeficiencia = e.PossuiDeficiencia,
                        Coluna = e.Coluna
                    })
                };

                await mediator.Send(new GerarRelatorioSondagemQuestionarioHtmlParaPdfCommand(
                    nomeTemplate: "RelatorioSondagemQuestionario",
                    paginas: new List<QuestionarioSondagemRelatorioDto> { pagina },
                    codigoCorrelacao: filtroRelatorioDto.CodigoCorrelacao,
                    mensagemUsuario: filtroRelatorioDto.Mensagem?.ToString() ?? string.Empty));
            }

            await solicitacaoRelatorioApiClient.FinalizarSolicitacaoAsync(filtro.SolicitacaoRelatorioId);
        }
    }
}