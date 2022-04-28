using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRecomendacoesPorAlunosTurmasQueryHandler : IRequestHandler<ObterRecomendacoesPorAlunosTurmasQuery, IEnumerable<RecomendacaoConselhoClasseAluno>>
    {
        private readonly IConselhoClasseAlunoRepository conselhoClasseAlunoRepository;
        private readonly IConselhoClasseRecomendacaoRepository conselhoClasseRecomendacaoRepository;

        public ObterRecomendacoesPorAlunosTurmasQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository, IConselhoClasseRecomendacaoRepository conselhoClasseRecomendacaoRepository)
        {
            this.conselhoClasseAlunoRepository = conselhoClasseAlunoRepository ?? throw new System.ArgumentNullException(nameof(conselhoClasseAlunoRepository));
            this.conselhoClasseRecomendacaoRepository = conselhoClasseRecomendacaoRepository ?? throw new System.ArgumentNullException(nameof(conselhoClasseRecomendacaoRepository));
        }

        public async Task<IEnumerable<RecomendacaoConselhoClasseAluno>> Handle(ObterRecomendacoesPorAlunosTurmasQuery request, CancellationToken cancellationToken)
        {
            var recomendacoes = await conselhoClasseAlunoRepository.ObterRecomendacoesPorAlunosTurmas(request.CodigosAluno, request.CodigosTurma, request.AnoLetivo, request.Modalidade, request.Semestre);

            if (recomendacoes == null && recomendacoes.Any())
            {
                var recomendacoesGeral = await conselhoClasseRecomendacaoRepository.ObterTodos();

                foreach (var recomendacao in recomendacoes)
                {
                    recomendacao.RecomendacoesAluno = recomendacao?.RecomendacoesAluno ?? MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Aluno).Select(b => b.Recomendacao));
                    recomendacao.RecomendacoesFamilia = recomendacao?.RecomendacoesFamilia ?? MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Familia).Select(b => b.Recomendacao));
                }
            }

           await FormatarRecomendacoes(recomendacoes);

            return recomendacoes;
        }

        private async Task FormatarRecomendacoes(IEnumerable<RecomendacaoConselhoClasseAluno> recomendacoesConselho)
        {
            foreach (var recomendacao in recomendacoesConselho)
            {
                var recomendacoesDoAluno = await conselhoClasseAlunoRepository.ObterRecomendacoesAlunoFamiliaPorAlunoETurma(recomendacao.AlunoCodigo, recomendacao.TurmaCodigo);

                var concatenaRecomendacaoAluno = new StringBuilder();
                foreach (var aluno in recomendacoesDoAluno.Where(r => r.Tipo == (int)ConselhoClasseRecomendacaoTipo.Aluno).ToList())
                    concatenaRecomendacaoAluno.AppendLine("- " + aluno.Recomendacao);

                concatenaRecomendacaoAluno.AppendLine("<br/>");
                concatenaRecomendacaoAluno.AppendLine(UtilHtml.FormatarHtmlParaTexto(recomendacao.RecomendacoesAluno));

                var concatenaRecomendacaoFamilia = new StringBuilder();
                foreach (var aluno in recomendacoesDoAluno.Where(r => r.Tipo == (int)ConselhoClasseRecomendacaoTipo.Familia).ToList())
                    concatenaRecomendacaoFamilia.AppendLine("- " + aluno.Recomendacao);

                concatenaRecomendacaoFamilia.AppendLine("<br/>");
                concatenaRecomendacaoFamilia.AppendLine(UtilHtml.FormatarHtmlParaTexto(recomendacao.RecomendacoesFamilia));


                recomendacao.AnotacoesPedagogicas = UtilHtml.FormatarHtmlParaTexto(recomendacao.AnotacoesPedagogicas);
                recomendacao.RecomendacoesAluno = concatenaRecomendacaoAluno.ToString();
                recomendacao.RecomendacoesFamilia = concatenaRecomendacaoFamilia.ToString();
            }

        } 

        private string MontaTextUlLis(IEnumerable<string> textos)
        {
            var str = new StringBuilder();

            foreach (var item in textos)
            {
                str.AppendFormat(item);
            }

            return str.ToString().Trim();
        }
    }
}
