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
    public class ListarTotalResultadoQueryHandler : IRequestHandler<ListarTotalResultadoQuery, IEnumerable<ResumoPAPTotalResultadoDto>>
    {
        private readonly IRecuperacaoParalelaRepository recuperacaoParalelaRepository;

        public ListarTotalResultadoQueryHandler(IRecuperacaoParalelaRepository recuperacaoParalelaRepository)
        {
            this.recuperacaoParalelaRepository = recuperacaoParalelaRepository ?? throw new ArgumentNullException(nameof(recuperacaoParalelaRepository));
        }

        public async Task<IEnumerable<ResumoPAPTotalResultadoDto>> Handle(ListarTotalResultadoQuery request, CancellationToken cancellationToken)
        {
            var totalResultados = await recuperacaoParalelaRepository.ListarTotalResultado(request.Periodo, request.DreId, request.UeId, request.CicloId,
                request.TurmaId, request.Ano, request.AnoLetivo, null);

            if (!totalResultados.Any()) return null;

            return MapearResultadoParaDto(totalResultados);
        }

        private IEnumerable<ResumoPAPTotalResultadoDto> MapearResultadoParaDto(IEnumerable<RetornoResumoPAPTotalResultadoDto> items)
        {
            return items
                .GroupBy(g => new { g.EixoId, g.Eixo })
                .Select(eixo => new ResumoPAPTotalResultadoDto
                {
                    EixoDescricao = eixo.Key.Eixo,
                    Objetivos = ObterObjetivos(items, eixo.Key.EixoId)
                });
        }

        private IEnumerable<ResumoPAPResultadoObjetivoDto> ObterObjetivos(IEnumerable<RetornoResumoPAPTotalResultadoDto> items, int eixoId)
        {
            return items.Where(obj => obj.EixoId == eixoId)
                .GroupBy(objetivo => new { objetivo.ObjetivoId, objetivo.Objetivo })
                .Select(objetivo => new ResumoPAPResultadoObjetivoDto
                {
                    Anos = ObterAnos(items, objetivo.Key.ObjetivoId, items.Where(x => x.ObjetivoId == objetivo.Key.ObjetivoId).Sum(s => s.Total)),
                    ObjetivoDescricao = objetivo.Key.Objetivo,
                    Total = ObterTotalPorObjetivo(items, objetivo.Key.ObjetivoId, items.Where(x => x.ObjetivoId == objetivo.Key.ObjetivoId).Sum(s => s.Total))
                });
        }

        private IEnumerable<ResumoPAPResultadoRespostaDto> ObterTotalPorObjetivo(IEnumerable<RetornoResumoPAPTotalResultadoDto> items, int objetivoId, int total)
        {
            return items.Where(tot => tot.ObjetivoId == objetivoId)
                .GroupBy(gt => gt.ObjetivoId)
                .Select(objetivoTotal => new ResumoPAPResultadoRespostaDto
                {
                    TotalQuantidade = objetivoTotal.Sum(x => x.Total),
                    TotalPorcentagem = (objetivoTotal.Sum(x => x.Total) * 100) / total
                });
        }

        private IEnumerable<ResumoPAPResultadoAnoDto> ObterAnos(IEnumerable<RetornoResumoPAPTotalResultadoDto> items, int objetivoId, int total)
        {
            return items.Where(ano => ano.ObjetivoId == objetivoId)
                .GroupBy(h => new { h.Ano, h.ObjetivoId })
                .Select(ano => new ResumoPAPResultadoAnoDto
                {
                    AnoDescricao = ano.Key.Ano,
                    Respostas = ObterRespostas(items, objetivoId, ehAno: true, ano.Key.Ano, total)
                });
        }

        private IEnumerable<ResumoPAPResultadoRespostaDto> ObterRespostas(IEnumerable<RetornoResumoPAPTotalResultadoDto> items, int objetivoId, bool ehAno, int anoCiclo, int total)
        {
            return items.Where(res => res.ObjetivoId == objetivoId && (ehAno ? res.Ano == anoCiclo : res.CicloId == anoCiclo))
                .GroupBy(gre => (gre.Resposta, gre.RespostaId))
                .Select(resposta => new ResumoPAPResultadoRespostaDto
                {
                    RespostaDescricao = resposta.Key.Resposta,
                    Quantidade = resposta.Sum(q => q.Total),
                    Porcentagem = ((double)resposta.Sum(q => q.Total) * 100) / total
                });
        }
    }
}
