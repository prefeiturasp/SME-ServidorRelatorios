using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.EncaminhamentoNaapa;
using SME.SR.Infra.Utilitarios;

namespace SME.SR.Application
{
    public class RelatorioEncaminhamentosNaapaDetalhadoUseCase : IRelatorioEncaminhamentosNaapaDetalhadoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioEncaminhamentosNaapaDetalhadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroRelatorioEncaminhamentoNaapaDetalhadoDto>();
            var encaminhamentosNaapa = await mediator.Send(new ObterEncaminhamentosNAAPAQuery(filtroRelatorio));

            if (encaminhamentosNaapa == null || !encaminhamentosNaapa.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var relatorios = new List<RelatorioEncaminhamentoNaapaDetalhadoDto>();

            foreach (var encaminhamentoNaapa in encaminhamentosNaapa)
            {
                var relatorio = new RelatorioEncaminhamentoNaapaDetalhadoDto();

                ObterCabecalho(encaminhamentoNaapa, relatorio);
                //await ObterQuestoesSecoes(encaminhamentoNaapa, relatorio);

                relatorios.Add(relatorio);
            }
        }
        private void ObterCabecalho(EncaminhamentoNaapaDto encaminhamentoNaapa, RelatorioEncaminhamentoNaapaDetalhadoDto relatorio)
        {
            relatorio.Cabecalho.DreNome = encaminhamentoNaapa.DreAbreviacao;
            relatorio.Cabecalho.UeNome = $"{encaminhamentoNaapa.UeCodigo} - {encaminhamentoNaapa.TipoEscola.ShortName()} {encaminhamentoNaapa.UeNome}";
            relatorio.Cabecalho.AnoLetivo = encaminhamentoNaapa.AnoLetivo;
            relatorio.Cabecalho.Aluno = $"{encaminhamentoNaapa.AlunoNome} ({encaminhamentoNaapa.AlunoCodigo})";
            relatorio.Cabecalho.SituacaoEncaminhamento = ((SituacaoEncaminhamentoAEE)encaminhamentoNaapa.Situacao).Name();
            relatorio.Cabecalho.TurmaNome = $"{encaminhamentoNaapa.Modalidade.ShortName()} - {encaminhamentoNaapa.TurmaNome}";
            relatorio.Cabecalho.DataCriacao = encaminhamentoNaapa.CriadoEm;
        }
    }
}