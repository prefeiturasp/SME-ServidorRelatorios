using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;

namespace SME.SR.Application
{
    public class RelatorioPlanoAeeUseCase : IRelatorioPlanoAeeUseCase
    {
        private readonly IMediator mediator;

        public RelatorioPlanoAeeUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
//            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioPlanoAeeDto>();

            var filtroRelatorio = new FiltroRelatorioPlanoAeeDto()
            {
                //VersoesPlanosIds = new long[] { 38532, 132, 160, 38533, 39189 }
                VersoesPlanosIds = new long[] { 39189 }
            }; 

            var relatoriosPlanoAee = new List<RelatorioPlanoAeeDto>();
            
            foreach (var versaoPlanoId in filtroRelatorio.VersoesPlanosIds)
            {
                var planoAee = await mediator.Send(new ObterPlanoAEEPorVersaoPlanoIdQuery(versaoPlanoId));

                if (planoAee == null)
                    throw new NegocioException("Plano AEE não localizado para a impressão.");

                var relatorioPlanoAee = new RelatorioPlanoAeeDto();

                ObterCabecalho(planoAee, relatorioPlanoAee);
                await ObterCadastro(planoAee, relatorioPlanoAee);
                ObterParecer(planoAee, relatorioPlanoAee); 
                
                relatoriosPlanoAee.Add(relatorioPlanoAee);
            }

            await mediator.Send(new GerarRelatorioHtmlPDFPlanoAeeCommand(relatoriosPlanoAee, request.CodigoCorrelacao));
        }

        private static void ObterCabecalho(PlanoAeeDto planoAee, RelatorioPlanoAeeDto relatorioPlanoAee)
        {
            relatorioPlanoAee.Cabecalho.AlunoCodigo = planoAee.AlunoCodigo;
            relatorioPlanoAee.Cabecalho.AlunoNome = planoAee.AlunoNome;
            relatorioPlanoAee.Cabecalho.AnoLetivo = planoAee.AnoLetivo;
            relatorioPlanoAee.Cabecalho.DreNome = planoAee.DreAbreviacao;
            relatorioPlanoAee.Cabecalho.SituacaoPlano = planoAee.SituacaoPlano.Name();
            relatorioPlanoAee.Cabecalho.TurmaNome = $"{planoAee.Modalidade.ShortName()} {planoAee.TurmaNome}";
            relatorioPlanoAee.Cabecalho.UeNome = $"{planoAee.TipoEscola.ShortName()} {planoAee.UeNome}";
            relatorioPlanoAee.Cabecalho.VersaoPlano = $"v{planoAee.VersaoPlano} - {planoAee.DataVersaoPlano:dd/MM/yyyy}";
        }

        private async Task ObterCadastro(PlanoAeeDto planoAee, RelatorioPlanoAeeDto relatorioPlanoAee)
        {
            relatorioPlanoAee.Cadastro.Responsavel = $"{planoAee.ResponsavelNome} ({planoAee.ResponsavelLoginRf})";

            var questoes = await mediator.Send(new ObterQuestoesPlanoAEEPorVersaoPlanoIdQuery(planoAee.VersaoPlano));

            var questoesRelatorio = new List<QuestaoPlanoAeeDto>();

            foreach (var questao in questoes)
            {
                var questaoRelatorio = new QuestaoPlanoAeeDto
                {
                    Questao = questao.Nome,
                    Ordem = questao.Ordem,
                    QuestaoId = questao.Id,
                    TipoQuestao = questao.Tipo
                };

                var respostaQuestao = questao.Respostas.FirstOrDefault(c => c.QuestaoId == questao.Id);

                if (respostaQuestao == null) 
                    continue;

                questaoRelatorio.RespostaId = respostaQuestao.OpcaoRespostaId;
                questaoRelatorio.Resposta = respostaQuestao.Texto;
                
                var opcaoRespostaQuestao = questao.OpcaoResposta.FirstOrDefault(c => c.QuestaoId == questao.Id &&
                    c.Id == respostaQuestao.OpcaoRespostaId);
                
                questaoRelatorio.Resposta = questao.Tipo switch
                {
                    TipoQuestao.Radio => opcaoRespostaQuestao?.Nome,
                    TipoQuestao.PeriodoEscolar => await ObterRespostaQuestaoPeriodoEscolar(respostaQuestao, relatorioPlanoAee),
                    _ => respostaQuestao.Texto
                };

                questaoRelatorio.Justificativa = ObterJustificativaQuestao(opcaoRespostaQuestao);
                questaoRelatorio.FrequenciaAluno = ObterRespostaFrequenciaAluno(questao.Tipo, respostaQuestao);
                questoesRelatorio.Add(questaoRelatorio);
            }

            relatorioPlanoAee.Cadastro.Questoes = questoesRelatorio;
        }

        private static void ObterParecer(PlanoAeeDto planoAee, RelatorioPlanoAeeDto relatorioPlanoAee)
        {
            relatorioPlanoAee.Parecer.Coordenacao = planoAee.ParecerCoordenacao;
            relatorioPlanoAee.Parecer.Cefai = planoAee.ParecerPaai;
            relatorioPlanoAee.Parecer.PaaiResponsavel = $"{planoAee.ResponsavelPaaiNome} ({planoAee.ResponsavelPaaiLoginRf})";
        }

        private async Task<string> ObterRespostaQuestaoPeriodoEscolar(RespostaQuestaoDto respostaQuestao, RelatorioPlanoAeeDto relatorioPlanoAee)
        {
            var idPeriodoEscolar = long.Parse(respostaQuestao.Texto);
            var periodoEscolar = await mediator.Send(new ObterPeriodoEscolarPorIdQuery(idPeriodoEscolar));
            return $"{periodoEscolar.Bimestre}º BIMESTRE - {relatorioPlanoAee.Cabecalho.AnoLetivo}";            
        }

        private static string ObterJustificativaQuestao(OpcaoRespostaDto opcaoResposta)
        {
            if (opcaoResposta?.QuestoesComplementares == null)
                return string.Empty;
            
            var questaoComplementar = opcaoResposta.QuestoesComplementares.FirstOrDefault();

            var respostaQuestaoComplementar =
                questaoComplementar?.Respostas.FirstOrDefault(c => c.QuestaoId == questaoComplementar.Id);

            return respostaQuestaoComplementar?.Texto;            
        }

        private static IEnumerable<FrequenciaAlunoPlanoAeeDto> ObterRespostaFrequenciaAluno(TipoQuestao tipoQuestao, RespostaQuestaoDto respostaQuestao)
        {
            if (tipoQuestao != TipoQuestao.FrequenciaEstudanteAEE)
                return null;
            
            var respostasFrequencias = JsonConvert.DeserializeObject<IEnumerable<RespostaFrequenciaAlunoPlanoAeeDto>>(respostaQuestao.Texto);

            return respostasFrequencias?.Select(respostaFrequencia =>
                new FrequenciaAlunoPlanoAeeDto
                {
                    DiaDaSemana = respostaFrequencia.DiaSemana,
                    Inicio = respostaFrequencia.HorarioInicio.ToString("HH:mm"),
                    Termino = respostaFrequencia.HorarioTermino.ToString("HH:mm")
                }).ToList();
        }
    }
}