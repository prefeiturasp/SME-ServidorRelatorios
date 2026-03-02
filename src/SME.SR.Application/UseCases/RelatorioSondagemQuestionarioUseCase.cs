using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Commands.NovoSondagem.GerarRelatorioSondagemQuestionarioHtmlParaPdf;
using SME.SR.Application.Commands.Sondagem.EscritaTurma;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.NovoSondagem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SME.SR.Application.UseCases
{
    public class RelatorioSondagemQuestionarioUseCase : IRelatorioSondagemQuestionarioUseCase
    {
        private readonly IMediator mediator;
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioSondagemQuestionarioUseCase(
            IMediator mediator,
            VariaveisAmbiente variaveisAmbiente)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
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
                var tarefaApi = ObterDadosQuestionarioExterno(filtro);
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

            //await FinalizarSolicitacaoRelatorio(filtro.SolicitacaoRelatorioId);
        }

        private async Task FinalizarSolicitacaoRelatorio(int solicitacaoRelatorioId)
        {
            var url = $"{variaveisAmbiente.UrlApiNovoSgp}v1/solicitacao-relatorio/finalizar-solicitacao";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-sgp-api-key", variaveisAmbiente.ChaveIntegracaoApiSgp);

            var payload = JsonConvert.SerializeObject(new { SolicitacaoRelatorioId = solicitacaoRelatorioId });
            var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao finalizar solicitação de relatório. Status: {response.StatusCode}. SolicitacaoRelatorioId: {solicitacaoRelatorioId}");
        }

        private async Task<RetornoApiSondagemQuestionarioDto> ObterDadosQuestionarioExterno(
            FiltroRelatorioSondagemQuestionarioDto filtro)
        {
            var url = $"{variaveisAmbiente.UrlApiNovaSondagem}/relatorio-integracao/sondagem-por-turma" +
                      $"?TurmaId={filtro.TurmaId}" +
                      $"&ProficienciaId={filtro.ProficienciaId}" +
                      (filtro.ComponenteCurricularId > 0 ? $"&ComponenteCurricularId={filtro.ComponenteCurricularId}" : string.Empty) +
                      $"&Modalidade={filtro.Modalidade}" +
                      $"&Ano={filtro.Ano}" +
                      $"&AnoLetivo={filtro.AnoLetivo}" +
                      (filtro.SemestreId > 0 ? $"&SemestreId={filtro.SemestreId}" : string.Empty) +
                      (!string.IsNullOrWhiteSpace(filtro.UeCodigo) ? $"&UeCodigo={filtro.UeCodigo}" : string.Empty) +
                      (filtro.BimestreId.HasValue && filtro.BimestreId > 0 ? $"&BimestreId={filtro.BimestreId}" : string.Empty);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-api-sondagem-key", variaveisAmbiente.ChaveIntegracaoApiSondagem);

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro ao consultar API de sondagem. Status: {response.StatusCode}. Url: {url}");

            var json = await response.Content.ReadAsStringAsync();
            var retorno = JsonConvert.DeserializeObject<RetornoApiSondagemQuestionarioDto>(json);

            if (retorno == null)
                throw new Exception("Retorno da API de sondagem veio nulo ou inválido.");

            return retorno;
        }
    }
}