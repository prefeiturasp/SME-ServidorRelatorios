using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
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

        private bool ContemRecomendacoesAluno(RecomendacoesConselhoClasse recomendacoes) =>
                (!string.IsNullOrEmpty(recomendacoes.RecomendacoesAluno) || recomendacoes.RecomendacoesPreDefinidasAluno.Any());
        private bool ContemRecomendacoesFamilia(RecomendacoesConselhoClasse recomendacoes) =>
            (!string.IsNullOrEmpty(recomendacoes.RecomendacoesFamilia) || recomendacoes.RecomendacoesPreDefinidasFamilia.Any());

        public async Task<RecomendacaoConselhoClasseAluno> Handle(ObterRecomendacoesPorFechamentoQuery request, CancellationToken cancellationToken)
        {
            var recomendacoes = await conselhoClasseAlunoRepository.ObterRecomendacoesPorFechamento(request.FechamentoTurmaId, request.CodigoAluno);

            if (recomendacoes == null || 
                (!ContemRecomendacoesAluno(recomendacoes) &&
                 !ContemRecomendacoesFamilia(recomendacoes))
               )
            {
                var recomendacoesAlunoFamilia = await conselhoClasseAlunoRepository.ObterRecomendacoesAlunoFamiliaPorAlunoEFechamentoTurma(request.FechamentoTurmaId, request.CodigoAluno);
                if (recomendacoesAlunoFamilia != null && recomendacoesAlunoFamilia.Any())
                {
                    var recomendacaoAluno = string.IsNullOrEmpty(recomendacoes?.RecomendacoesAluno ?? "") ? MontaTextUlLis(recomendacoesAlunoFamilia.Where(r => r.Tipo == (int)ConselhoClasseRecomendacaoTipo.Aluno).Select(recomendacao => recomendacao.Recomendacao)) : recomendacoes?.RecomendacoesAluno;
                    var recomendacaoFamilia = string.IsNullOrEmpty(recomendacoes?.RecomendacoesFamilia ?? "") ? MontaTextUlLis(recomendacoesAlunoFamilia.Where(r => r.Tipo == (int)ConselhoClasseRecomendacaoTipo.Familia).Select(recomendacao => recomendacao.Recomendacao)): recomendacoes?.RecomendacoesFamilia;

                    return new RecomendacaoConselhoClasseAluno
                    {
                        RecomendacoesAluno = recomendacaoAluno,
                        RecomendacoesFamilia = recomendacaoFamilia,
                        AnotacoesPedagogicas = recomendacoes?.AnotacoesPedagogicas ?? ""
                    };
                } 

                var recomendacoesGeral = await conselhoClasseRecomendacaoRepository.ObterTodos();

                return new RecomendacaoConselhoClasseAluno
                {
                    RecomendacoesAluno = MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Aluno).Select(b => b.Recomendacao)),
                    RecomendacoesFamilia = MontaTextUlLis(recomendacoesGeral.Where(a => a.Tipo == ConselhoClasseRecomendacaoTipo.Familia).Select(b => b.Recomendacao)),
                };
            }

            return FormatarRecomendacoes(recomendacoes);
        }

        private RecomendacaoConselhoClasseAluno FormatarRecomendacoes(RecomendacoesConselhoClasse recomendacaoConselho)
        {
            var RecomendacoesAlunoLst = recomendacaoConselho.RecomendacoesPreDefinidasAluno.Select(recomendacao => Formatar(recomendacao.Recomendacao)).ToList();
            var RecomendacoesFamiliaLst = recomendacaoConselho.RecomendacoesPreDefinidasFamilia.Select(recomendacao => Formatar(recomendacao.Recomendacao)).ToList();

            if (!string.IsNullOrEmpty(recomendacaoConselho.RecomendacoesAluno))
                RecomendacoesAlunoLst.Insert(0, Formatar(recomendacaoConselho.RecomendacoesAluno));

            if (!string.IsNullOrEmpty(recomendacaoConselho.RecomendacoesFamilia))
                RecomendacoesFamiliaLst.Insert(0, Formatar(recomendacaoConselho.RecomendacoesFamilia));

            var RecomendacoesAluno = string.Join("\r\n", RecomendacoesAlunoLst);
            var RecomendacoesFamilia = string.Join("\r\n", RecomendacoesFamiliaLst);
            var AnotacoesPedagogicas = Formatar(recomendacaoConselho.AnotacoesPedagogicas);

            return new RecomendacaoConselhoClasseAluno()
            {
                RecomendacoesAluno = RecomendacoesAluno,
                RecomendacoesFamilia = RecomendacoesFamilia,
                AnotacoesPedagogicas = AnotacoesPedagogicas
            };
        }

        private string Formatar(string recomendacao)
        {
            if (!string.IsNullOrEmpty(recomendacao))
            {
                string semTags = UtilRegex.RemoverTagsHtmlMidia(recomendacao);
                semTags = UtilRegex.RemoverTagsHtml(semTags);
                semTags = UtilRegex.AdicionarEspacos(semTags);

                return semTags;
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
