using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRecomendacoesPorFechamentoQueryHandler : IRequestHandler<ObterRecomendacoesPorFechamentoQuery, RecomendacaoConselhoClasseAluno>
    {
        private IConselhoClasseAlunoRepository conselhoClasseAlunoRepository;
        private IConselhoClasseRecomendacaoRepository conselhoClasseRecomendacaoRepository;

        public ObterRecomendacoesPorFechamentoQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository,
                                                           IConselhoClasseRecomendacaoRepository conselhoClasseRecomendacaoRepository)
        {
            this.conselhoClasseAlunoRepository = conselhoClasseAlunoRepository ?? throw new ArgumentNullException(nameof(conselhoClasseAlunoRepository));
            this.conselhoClasseRecomendacaoRepository = conselhoClasseRecomendacaoRepository ?? throw new ArgumentNullException(nameof(conselhoClasseRecomendacaoRepository));
        }

        public async Task<RecomendacaoConselhoClasseAluno> Handle(ObterRecomendacoesPorFechamentoQuery request, CancellationToken cancellationToken)
        {
            var recomendacoes = await conselhoClasseAlunoRepository.ObterRecomendacoesPorFechamento(request.FechamentoTurmaId, request.CodigoAluno);

            if (recomendacoes == null || string.IsNullOrEmpty(recomendacoes.RecomendacoesAluno) || string.IsNullOrEmpty(recomendacoes.RecomendacoesFamilia))
            {
                var recomendacoesGeral = await conselhoClasseRecomendacaoRepository.ObterTodos();

                return new RecomendacaoConselhoClasseAluno
                {
                    RecomendacoesAluno = recomendacoes?.RecomendacoesAluno ?? MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Aluno).Select(b => b.Recomendacao)),
                    RecomendacoesFamilia = recomendacoes?.RecomendacoesFamilia ?? MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Familia).Select(b => b.Recomendacao)),
                };
            }

            FormatarRecomendacoes(recomendacoes);

            return recomendacoes;
        }

        private void FormatarRecomendacoes(RecomendacaoConselhoClasseAluno recomendacaoConselho)
        {
            recomendacaoConselho.AnotacoesPedagogicas =
               Formatar(recomendacaoConselho.AnotacoesPedagogicas);

            recomendacaoConselho.RecomendacoesAluno =
                Formatar(recomendacaoConselho.RecomendacoesAluno);

            recomendacaoConselho.RecomendacoesFamilia =
                Formatar(recomendacaoConselho.RecomendacoesFamilia);
        }

        private string Formatar(string recomendacao)
        {
            if (!string.IsNullOrEmpty(recomendacao))
            {
                string semTags = Regex.Replace(recomendacao, "<.*?>", String.Empty);
                string adicionarEspaco = Regex.Replace(semTags, @"\.(?! |$)", ". ");

                return adicionarEspaco;
            }
            else
            {
                return recomendacao;
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
