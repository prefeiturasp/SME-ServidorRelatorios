using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Commands.NovoSondagem.GerarRelatorioSondagemQuestionarioHtmlParaPdf;
using SME.SR.Application.Commands.Sondagem.EscritaTurma;
using SME.SR.Application.Interfaces.UseCases;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.NovoSondagem;
using System;
using System.Collections.Generic;
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
            var filtro = filtroRelatorioDto.ObterObjetoFiltro<FiltroRelatorioSondagemQuestionarioDto>();

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
                                        filtro.Semestre,
                                        filtroRelatorioDto.UsuarioLogadoRF,
                                        filtro.UeCodigo,
                                        filtro.BimestreId
                                        ));
                return;
            }
            else if (filtro.TipoRelatorio == (int)TipoFormatoRelatorio.Pdf)
            {

                var dadosApi = await ObterDadosQuestionarioExterno(filtro);

                var pagina = new QuestionarioSondagemRelatorioDto
                {
                    AnoLetivo = filtro.AnoLetivo,
                    Dre = filtro.DreNome,
                    Semestre = $"{filtro.Semestre}º semestre",
                    Turma = filtro.TurmaNome,
                    UnidadeEducacional = $"{filtro.UeCodigo} - {filtro.UeNome}",
                    Modalidade = filtro.ModalidadeNome,
                    Proficiencia = dadosApi.TituloTabelaRespostas,
                    DataImpressao = DateTime.Now,
                    Usuario = filtro.UsuarioLogadoNome,
                    TituloTabelaRespostas = dadosApi.TituloTabelaRespostas,
                    Estudantes = dadosApi.Estudantes
                };

                await mediator.Send(new GerarRelatorioSondagemQuestionarioHtmlParaPdfCommand(
                    nomeTemplate: "RelatorioSondagemQuestionario",
                    paginas: new List<QuestionarioSondagemRelatorioDto> { pagina },
                    codigoCorrelacao: filtroRelatorioDto.CodigoCorrelacao,
                    mensagemUsuario: filtroRelatorioDto.Mensagem?.ToString() ?? string.Empty));
            }
        }

        private async Task<RetornoApiSondagemQuestionarioDto> ObterDadosQuestionarioExterno(
            FiltroRelatorioSondagemQuestionarioDto filtro)
        {
            var url = $"{variaveisAmbiente.UrlApiNovaSondagem}/api/relatorio-integracao/sondagem-por-turma" +
                      $"?turmaId={filtro.TurmaId}" +
                      $"&proficienciaId={filtro.ProficienciaId}" +
                      $"&componenteCurricularId={filtro.ComponenteCurricularId}" +
                      $"&modalidade={filtro.Modalidade}" +
                      $"&ano={filtro.Ano}" +
                      $"&anoLetivo={filtro.AnoLetivo}" +
                      $"&semestre={filtro.Semestre}" +
                      $"&ueCodigo={filtro.UeCodigo}" +
                      (filtro.BimestreId.HasValue ? $"&bimestreId={filtro.BimestreId}" : string.Empty);

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