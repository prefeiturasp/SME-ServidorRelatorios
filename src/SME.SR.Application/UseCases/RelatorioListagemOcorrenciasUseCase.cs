using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioListagemOcorrenciasUseCase : AbstractUseCase, IRelatorioListagemOcorrenciasUseCase
    {
        public RelatorioListagemOcorrenciasUseCase(IMediator mediator) : base(mediator)
        {
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioListagemOcorrenciasDto>();

            var relatorioDto = await mediator.Send(new ObterListagemOcorrenciasQuery(filtros.ExibirHistorico, filtros.AnoLetivo, filtros.CodigoDre, filtros.CodigoUe, filtros.Modalidade, filtros.Semestre, filtros.CodigosTurma, filtros.DataInicio, filtros.DataFim, filtros.OcorrenciaTipoIds, filtros.ImprimirDescricaoOcorrencia));

            if (relatorioDto.Registros == null || !relatorioDto.Registros.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            PreencherFiltrosRelatorio(relatorioDto, filtros);

            if (filtros.ImprimirDescricaoOcorrencia)
            {
                foreach (var registro in relatorioDto.Registros)
                {
                    registro.Descricao = string.IsNullOrEmpty(registro.Descricao) ? null : UtilHtml.FormatarHtmlParaTexto(registro.Descricao);
                }
            }

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioListagemOcorrencias", relatorioDto, request.CodigoCorrelacao));
        }

        private void PreencherFiltrosRelatorio(RelatorioListagemOcorrenciasDto relatorioDto, FiltroRelatorioListagemOcorrenciasDto filtros)
        {
            relatorioDto.Usuario = $"{filtros.NomeUsuario} ({filtros.CodigoRf})";
            relatorioDto.DataSolicitacao = DateTime.Now;
            relatorioDto.ImprimirDescricaoOcorrencia = filtros.ImprimirDescricaoOcorrencia;

            relatorioDto.Ues = relatorioDto.Registros
                .GroupBy(t =>
                new
                {
                    t.UeCodigo,
                    t.UeDescricao,
                    t.TipoEscola,
                    t.DreAbreviacao
                })
                .Select(t =>
                    new UeDto
                    {
                        CodigoUe = t.Key.UeCodigo,
                        Nome = t.Key.UeDescricao,
                        TipoEscola = t.Key.TipoEscola,
                        Dre = new DreDto
                        {
                            Abreviacao = t.Key.DreAbreviacao
                        }
                    })
                .OrderBy(t => t.Dre.Codigo)
                .ThenBy(t => t.Nome);
        }
    }
}
