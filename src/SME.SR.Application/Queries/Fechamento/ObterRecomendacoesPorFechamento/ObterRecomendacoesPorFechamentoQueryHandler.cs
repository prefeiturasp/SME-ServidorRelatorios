using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRecomendacoesPorFechamentoQueryHandler : IRequestHandler<ObterRecomendacoesPorFechamentoQuery, RecomendacaoConselhoClasseAluno>
    {
        private IConselhoClasseAlunoRepository _conselhoClasseAlunoRepository;
        private IConselhoClasseRecomendacaoRepository _conselhoClasseRecomendacaoRepository;

        public ObterRecomendacoesPorFechamentoQueryHandler(IConselhoClasseAlunoRepository conselhoClasseAlunoRepository,
                                                           IConselhoClasseRecomendacaoRepository conselhoClasseRecomendacaoRepository)
        {
            this._conselhoClasseAlunoRepository = conselhoClasseAlunoRepository;
            this._conselhoClasseRecomendacaoRepository = conselhoClasseRecomendacaoRepository;
        }

        public async Task<RecomendacaoConselhoClasseAluno> Handle(ObterRecomendacoesPorFechamentoQuery request, CancellationToken cancellationToken)
        {
            var recomendacoes = await _conselhoClasseAlunoRepository.ObterRecomendacoesPorFechamento(request.FechamentoTurmaId, request.CodigoAluno);

            if (recomendacoes == null || string.IsNullOrEmpty(recomendacoes.RecomendacoesAluno) || string.IsNullOrEmpty(recomendacoes.RecomendacoesFamilia))
            {
                var recomendacoesGeral = await _conselhoClasseRecomendacaoRepository.ObterTodos();

                return new RecomendacaoConselhoClasseAluno
                {
                    RecomendacoesAluno = recomendacoes?.RecomendacoesAluno ?? MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Aluno).Select(b => b.Recomendacao)),
                    RecomendacoesFamilia = recomendacoes?.RecomendacoesFamilia ?? MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Familia).Select(b => b.Recomendacao)),
                };
            }

            RemoverTags(recomendacoes);

            return recomendacoes;
        }

        private void RemoverTags(RecomendacaoConselhoClasseAluno recomendacaoConselho)
        {
            if (!string.IsNullOrEmpty(recomendacaoConselho.AnotacoesPedagogicas))
                recomendacaoConselho.AnotacoesPedagogicas =
                    Regex.Replace(recomendacaoConselho.AnotacoesPedagogicas, "<.*?>", String.Empty);

            if (!string.IsNullOrEmpty(recomendacaoConselho.RecomendacoesAluno))
                recomendacaoConselho.RecomendacoesAluno =
                    Regex.Replace(recomendacaoConselho.RecomendacoesAluno, "<.*?>", String.Empty);

            if (!string.IsNullOrEmpty(recomendacaoConselho.RecomendacoesFamilia))
                recomendacaoConselho.RecomendacoesFamilia =
                    Regex.Replace(recomendacaoConselho.RecomendacoesFamilia, "<.*?>", String.Empty);
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
