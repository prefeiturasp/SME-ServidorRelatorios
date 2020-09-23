using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ListarTotalAlunosPorFrequenciaQueryHandler : IRequestHandler<ListarTotalAlunosPorFrequenciaQuery, ResumoPAPTotalEstudantePorFrequenciaDto>
    {
        private readonly IRecuperacaoParalelaRepository recuperacaoParalelaRepository;

        public ListarTotalAlunosPorFrequenciaQueryHandler(IRecuperacaoParalelaRepository recuperacaoParalelaRepository)
        {
            this.recuperacaoParalelaRepository = recuperacaoParalelaRepository ?? throw new ArgumentNullException(nameof(recuperacaoParalelaRepository));
        }

        public async Task<ResumoPAPTotalEstudantePorFrequenciaDto> Handle(ListarTotalAlunosPorFrequenciaQuery request, CancellationToken cancellationToken)
        {
            var totalAlunosPorSeries = await recuperacaoParalelaRepository.ListarTotalEstudantesPorFrequencia(request.Periodo, request.DreId, request.UeId, request.CicloId,
                request.TurmaId, request.Ano, request.AnoLetivo);

            if (!totalAlunosPorSeries.Any()) return null;
            var total = totalAlunosPorSeries.Sum(s => s.Total);

            return MapearParaDtoTotalEstudantesPorFrequencia(total, totalAlunosPorSeries);
        }

        private ResumoPAPTotalEstudantePorFrequenciaDto MapearParaDtoTotalEstudantesPorFrequencia(int total, IEnumerable<RetornoResumoPAPTotalAlunosAnoFrequenciaDto> items)
        {
            var retorno = new ResumoPAPTotalEstudantePorFrequenciaDto
            {
                Frequencia = items.GroupBy(fg => new { fg.RespostaId, fg.Frequencia }).Select(freq => new ResumoPAPTotalEstudanteFrequenciaDto
                {
                    FrequenciaDescricao = freq.Key.Frequencia,
                    PorcentagemTotalFrequencia = (double)(freq.Sum(x => x.Total) * 100) / total,
                    QuantidadeTotalFrequencia = freq.Sum(x => x.Total),
                    Linhas = items.Where(wlinha => wlinha.RespostaId == freq.Key.RespostaId).GroupBy(glinha => new { glinha.RespostaId }).Select(lin => new ResumoPAPFrequenciaDto
                    {
                        Anos = items.Where(wano => wano.RespostaId == lin.Key.RespostaId).GroupBy(gano => new { gano.Ano }).Select(ano => new ResumoPAPTotalFrequenciaAnoDto
                        {
                            CodigoAno = ano.Key.Ano,
                            DescricaoAno = ano.Key.Ano.ToString(),
                            Descricao = $"{ano.Key.Ano}º",
                            Quantidade = ano.Sum(c => c.Total),
                            Porcentagem = ((double)ano.Sum(c => c.Total) * 100) / total,
                            TotalQuantidade = items.Where(t => t.RespostaId == lin.Key.RespostaId).Sum(s => s.Total),
                            TotalPorcentagem = ((double)items.Where(t => t.RespostaId == lin.Key.RespostaId).Sum(s => s.Total) * 100) / total
                        })
                    }).ToList()
                }).ToList()
            };

            var linhaTotal = new List<ResumoPAPFrequenciaDto>();
            linhaTotal.Add(new ResumoPAPFrequenciaDto
            {
                Anos = items.GroupBy(w => w.Ano).Select(s => new ResumoPAPTotalFrequenciaAnoDto
                {
                    Quantidade = s.Sum(x => x.Total),
                    TotalQuantidade = total,
                    Porcentagem = ((double)s.Sum(x => x.Total) * 100) / total,
                    TotalPorcentagem = 100,
                    DescricaoAno = s.Key.ToString(),
                    CodigoAno = s.Key,
                })
            });

            retorno.Frequencia.Add(new ResumoPAPTotalEstudanteFrequenciaDto
            {
                FrequenciaDescricao = "Total",
                QuantidadeTotalFrequencia = total,
                PorcentagemTotalFrequencia = 100,
                Linhas = linhaTotal
            });

            return retorno;
        }
    }
}
