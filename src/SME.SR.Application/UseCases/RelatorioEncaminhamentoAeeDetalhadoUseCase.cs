using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentoAeeDetalhadoUseCase : IRelatorioEncaminhamentoAeeDetalhadoUseCase
    {
        private readonly IMediator mediator;
        private const string SECAO_INFORMACOES_ESCOLARES = "INFORMACOES_ESCOLARES";

        public RelatorioEncaminhamentoAeeDetalhadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(long []ids)//FiltroRelatorioEncaminhamentoAeeDetalhadoDto request)
        {
            //var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentoAeeDetalhadoDto>();
            //var encaminhamentosAee = await mediator.Send(new ObterEncaminhamentosAEEPorIdQuery(filtroRelatorio));
            var encaminhamentosAee = await mediator.Send(new ObterEncaminhamentosAEEPorIdQuery(new ids));

            if (encaminhamentosAee == null || !encaminhamentosAee.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var relatorios = new List<RelatorioEncaminhamentoAeeDetalhadoDto>();

            foreach (var encaminhamentoAee in encaminhamentosAee)
            {
                var relatorio = new RelatorioEncaminhamentoAeeDetalhadoDto();

                ObterCabecalho(encaminhamentoAee, relatorio);
                await ObterCadastro(encaminhamentoAee, relatorio);
                //ObterParecer(planoAee, relatorioPlanoAee);

                relatorios.Add(relatorio);
            }

            /* var encaminhamentosAgrupados = encaminhamentosAee.GroupBy(g => new
             {
                 DreId = g.DreId,
                 DreNome = g.DreAbreviacao,
                 UeNome = $"{g.UeCodigo} - {g.TipoEscola.ShortName()} {g.UeNome}",
             }, (key, group) =>
             new AgrupamentoEncaminhamentoAeeDreUeDto()
             {
                 DreId = key.DreId,
                 DreNome = key.DreNome,
                 UeNome = key.UeNome,
                 Detalhes = group.Select(s =>
                 new DetalheEncaminhamentoAeeDto()
                 {
                     Aluno = $"{s.AlunoNome} ({s.AlunoCodigo})",
                     Turma = $"{s.Modalidade.ShortName()} - {s.TurmaNome}",
                     Situacao = ((SituacaoEncaminhamentoAEE)s.Situacao).Name(),
                     ResponsavelPAAI = !string.IsNullOrEmpty(s.ResponsavelPaaiNome) ? $"{s.ResponsavelPaaiNome} ({s.ResponsavelPaaiLoginRf})" : string.Empty,
                 }).OrderBy(oAluno => oAluno.Aluno).ToList()
             }).OrderBy(oDre => oDre.DreId).ThenBy(oUe => oUe.UeNome).ToList();

             var cabecalho = new CabecalhoEncaminhamentoAeeDto()
             {
                 DreNome = filtroRelatorio.DreCodigo.Equals("-99") ? "TODAS" : encaminhamentosAgrupados.FirstOrDefault().DreNome,
                 UeNome = filtroRelatorio.UeCodigo.Equals("-99") ? "TODAS" : encaminhamentosAgrupados.FirstOrDefault().UeNome,
                 UsuarioNome = $"{filtroRelatorio.UsuarioNome} ({filtroRelatorio.UsuarioRf})",
             };

             await mediator.Send(new GerarRelatorioHtmlPDFEncaminhamentoAeeCommand(cabecalho, encaminhamentosAgrupados, request.CodigoCorrelacao));*/
        }

        private static void ObterCabecalho(EncaminhamentoAeeDto encaminhamentoAee, RelatorioEncaminhamentoAeeDetalhadoDto relatorioEncaminhamentoAeeDetalhado)
        {
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.DreNome = encaminhamentoAee.DreAbreviacao;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.UeNome = $"{encaminhamentoAee.UeCodigo} - {encaminhamentoAee.TipoEscola.ShortName()} {encaminhamentoAee.UeNome}";
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.AnoLetivo = encaminhamentoAee.AnoLetivo;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.AlunoNome = encaminhamentoAee.AlunoNome;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.SituacaoEncaminhamento = ((SituacaoEncaminhamentoAEE) encaminhamentoAee.Situacao).Name();
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.TurmaNome = $"{encaminhamentoAee.Modalidade.ShortName()} - {encaminhamentoAee.TurmaNome}";
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.AlunoCodigo = encaminhamentoAee.AlunoCodigo;
            relatorioEncaminhamentoAeeDetalhado.Cabecalho.DataCriacao = encaminhamentoAee.CriadoEm;
        }

        private async Task ObterCadastro(EncaminhamentoAeeDto encaminhamentoAee, RelatorioEncaminhamentoAeeDetalhadoDto relatorioEncaminhamentoAeeDetalhado)
        {
            var questoes = await mediator.Send(new ObterQuestoesEncaminhamentoAEEPorIdENomeComponenteSecaoQuery(encaminhamentoAee.Id, SECAO_INFORMACOES_ESCOLARES));

            var questoesRelatorio = new List<QuestaoEncaminhamentoAeeDto>();

            foreach (var questao in questoes)
            {
                var questaoRelatorio = new QuestaoEncaminhamentoAeeDto
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
                    /*TipoQuestao.PeriodoEscolar => await ObterRespostaQuestaoPeriodoEscolar(respostaQuestao, relatorioPlanoAee),
                    _ => UtilRegex.RemoverTagsHtml(respostaQuestao.Texto)*/
                };
                /*
                questaoRelatorio.Justificativa = ObterJustificativaQuestao(opcaoRespostaQuestao);

                var respostaFrequenciaAluno = ObterRespostaFrequenciaAluno(questao.Tipo, respostaQuestao);

                if (respostaFrequenciaAluno != null)
                    questaoRelatorio.FrequenciaAluno = ObterRespostaFrequenciaAluno(questao.Tipo, respostaQuestao);


                var respostaSrm = ObterRespostaInformacoesSrm(questao.Tipo, respostaQuestao);

                if (respostaSrm != null)
                    questaoRelatorio.InformacoesSrm = respostaSrm;*/


                questoesRelatorio.Add(questaoRelatorio);
            }

            relatorioEncaminhamentoAeeDetalhado.DetalheQuestoes.Questoes = questoesRelatorio;
        }
    }
}
