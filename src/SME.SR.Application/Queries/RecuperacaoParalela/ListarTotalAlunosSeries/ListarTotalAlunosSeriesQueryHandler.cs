using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.RecuperacaoParalela.ListarTotalAlunosSeries
{
    public class ListarTotalAlunosSeriesQueryHandler : IRequestHandler<ListarTotalAlunosSeriesQuery, ResumoPAPTotalEstudantesDto>
    {
        private readonly IRecuperacaoParalelaRepository recuperacaoParalelaRepository;

        public ListarTotalAlunosSeriesQueryHandler(IRecuperacaoParalelaRepository recuperacaoParalelaRepository)
        {
            this.recuperacaoParalelaRepository = recuperacaoParalelaRepository ?? throw new ArgumentNullException(nameof(recuperacaoParalelaRepository));
        }

        public async Task<ResumoPAPTotalEstudantesDto> Handle(ListarTotalAlunosSeriesQuery request, CancellationToken cancellationToken)
        {
            var totalAlunos = await recuperacaoParalelaRepository.ListarTotalAlunosSeries(request.Periodo, request.DreId, request.UeId, request.CicloId,
                request.TurmaId, request.Ano, request.AnoLetivo);

            if (!totalAlunos.Any()) return null;
            var total = totalAlunos.Sum(s => s.Total);

            bool deveExibirCiclo = false;

            if (string.IsNullOrEmpty(request.Ano))
                deveExibirCiclo = true;

            return MapearParaDtoTotalEstudantes(total, totalAlunos.OrderBy(a => a.Ano).ToList(), deveExibirCiclo);
        }

        private ResumoPAPTotalEstudantesDto MapearParaDtoTotalEstudantes(int total, IEnumerable<RetornoResumoPAPTotalAlunosAnoDto> totalAlunosPorSeries, bool deveExibirCiclo)
        {

            if (deveExibirCiclo)
            {
                var listaAgrupada = totalAlunosPorSeries.GroupBy(a => a.Ciclo).ToList();

                var retorno = new ResumoPAPTotalEstudantesDto
                {
                    QuantidadeTotal = total,
                    PorcentagemTotal = 100,

                };

                List<ResumoPAPTotalAnoDto> listaAnos = new List<ResumoPAPTotalAnoDto>();

                foreach (var ano in listaAgrupada)
                {
                    var objetos = totalAlunosPorSeries.Where(a => a.Ciclo == ano.Key);

                    if (objetos.Any())
                    {
                        var quantidadeTotal = objetos.Sum(a => a.Total);

                        var anoParaAdicionar = new ResumoPAPTotalAnoDto()
                        {
                           AnoDescricao = objetos.FirstOrDefault().Ciclo,
                            Quantidade = quantidadeTotal,                            
                            Porcentagem = ((double)(quantidadeTotal * 100)) / total
                        };

                        listaAnos.Add(anoParaAdicionar);
                    }
                }

                retorno.Anos = listaAnos;

                return retorno;

            }
            else
            {
                return new ResumoPAPTotalEstudantesDto
                {
                    QuantidadeTotal = total,
                    PorcentagemTotal = 100,
                    Anos = totalAlunosPorSeries.Select(x => new ResumoPAPTotalAnoDto
                    {
                        AnoDescricao = (deveExibirCiclo == true ? x.Ciclo : x.Ano.ToString() + "°"),
                        Quantidade = x.Total,
                        Porcentagem = ((double)(x.Total * 100)) / total
                    })
                };
            }



        }

    }
}

