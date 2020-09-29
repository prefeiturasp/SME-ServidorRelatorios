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

            return await MapearResultadoParaDto(totalResultados);
        }

        private async Task<IEnumerable<ResumoPAPTotalResultadoDto>> MapearResultadoParaDto(IEnumerable<RetornoResumoPAPTotalResultadoDto> items)
        {
            var idsObjetivos = items.Select(a => a.ObjetivoId).Distinct().ToArray();

           var respostasDosObjetivos = await recuperacaoParalelaRepository.ObterRespostasPorObjetivosIdsAsync(idsObjetivos);

            return items
                .GroupBy(g => new { g.EixoId, g.Eixo })
                .Select(eixo => new ResumoPAPTotalResultadoDto
                {
                    EixoDescricao = eixo.Key.Eixo,
                    Objetivos = ObterObjetivos(items, eixo.Key.EixoId, respostasDosObjetivos)
                });
        }

        private IEnumerable<ResumoPAPResultadoObjetivoDto> ObterObjetivos(IEnumerable<RetornoResumoPAPTotalResultadoDto> items, int eixoId, IEnumerable<RetornoResumoPAPRespostasPorObjetivosIds> respostasDosObjetivos)
        {
            var itens = items.Where(obj => obj.EixoId == eixoId)
                .GroupBy(objetivo => new { objetivo.ObjetivoId, objetivo.Objetivo });


            var listaRetorno = new List<ResumoPAPResultadoObjetivoDto>();

            foreach (var item in itens)
            {
                var objetivoId = item.Key.ObjetivoId;

                listaRetorno.Add(new ResumoPAPResultadoObjetivoDto()
                {
                    Anos = ObterAnos(items, objetivoId, items.Where(x => x.ObjetivoId == item.Key.ObjetivoId).Sum(s => s.Total), respostasDosObjetivos.Where(a => a.ObjetivoId == objetivoId)),
                    ObjetivoDescricao = item.Key.Objetivo,
                    Total = ObterTotalPorObjetivo(items, objetivoId, items.Where(x => x.ObjetivoId == objetivoId).Sum(s => s.Total))
                });
            }

            return listaRetorno;

        }

        private IEnumerable<ResumoPAPResultadoRespostaDto> ObterTotalPorObjetivo(IEnumerable<RetornoResumoPAPTotalResultadoDto> items, int objetivoId, int total)
        {
            var itens = items.Where(tot => tot.ObjetivoId == objetivoId)
                .GroupBy(gt => gt.ObjetivoId);


            var listaRetorno = new List<ResumoPAPResultadoRespostaDto>();

            foreach (var item in itens)
            {
                listaRetorno.Add(new ResumoPAPResultadoRespostaDto
                {
                    TotalQuantidade = item.Sum(x => x.Total),
                    TotalPorcentagem = item.Sum(x => x.Total) * 100 / total
                });
            }


            return listaRetorno;
        }

        private IEnumerable<ResumoPAPResultadoAnoDto> ObterAnos(IEnumerable<RetornoResumoPAPTotalResultadoDto> items, int objetivoId, int total, IEnumerable<RetornoResumoPAPRespostasPorObjetivosIds> respostasDosObjetivos)
        {
            var itens = items.Where(ano => ano.ObjetivoId == objetivoId)
                .GroupBy(h => new { h.Ano, h.ObjetivoId });


            var listaRetorno = new List<ResumoPAPResultadoAnoDto>();

            foreach (var item in itens)
            {
                listaRetorno.Add(new ResumoPAPResultadoAnoDto
                {
                    AnoDescricao = item.Key.Ano,
                    Respostas = ObterRespostas(items, objetivoId, ehAno: true, item.Key.Ano, total, respostasDosObjetivos)
                });
            }

            return listaRetorno;
        }

        private IEnumerable<ResumoPAPResultadoRespostaDto> ObterRespostas(IEnumerable<RetornoResumoPAPTotalResultadoDto> items, int objetivoId, bool ehAno, int anoCiclo, int total, IEnumerable<RetornoResumoPAPRespostasPorObjetivosIds> respostasDosObjetivos)
        {
            var itens = items.Where(res => res.ObjetivoId == objetivoId && (ehAno ? res.Ano == anoCiclo : res.CicloId == anoCiclo))
                .GroupBy(gre => (gre.Resposta, gre.RespostaId));

            var listaRetorno = new List<ResumoPAPResultadoRespostaDto>();

            foreach (var resposta in respostasDosObjetivos)
            {
                listaRetorno.Add(new ResumoPAPResultadoRespostaDto
                {
                    RespostaDescricao = resposta.RespostaDescricao,
                    RespostaId = resposta.RespostaId
                });
            }


            foreach (var item in itens)
            {
                var itemParaAlterar = listaRetorno.Find(a => a.RespostaId == item.Key.RespostaId);
                itemParaAlterar.Quantidade = item.Sum(q => q.Total);
                itemParaAlterar.Porcentagem = (double)item.Sum(q => q.Total) * 100 / total;
            }

            return listaRetorno;
        }
    }
}
