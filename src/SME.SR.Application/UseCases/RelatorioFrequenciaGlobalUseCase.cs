using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioFrequenciaGlobalUseCase : AbstractUseCase, IRelatorioFrequenciaGlobalUseCase
    {
        public RelatorioFrequenciaGlobalUseCase(IMediator mediator) : base(mediator)
        {
        }

        public delegate Task OpcaoRelatorio(List<FrequenciaGlobalDto> listaDeFrequencia, Guid codigoCorrelacao);        

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroFrequenciaGlobalDto>();
            var listaDeFrenquenciaGlobal = await mediator.Send(new ObterRelatorioDeFrequenciaGlobalQuery(filtroRelatorio));
            if (listaDeFrenquenciaGlobal?.Any() != true)                
                throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
            else
            {
                switch (filtroRelatorio.TipoFormatoRelatorio)
                {
                    case TipoFormatoRelatorio.Pdf:
                        await GerarRelatorioPdf(listaDeFrenquenciaGlobal, request, filtroRelatorio);
                        break;
                    case TipoFormatoRelatorio.Xlsx:
                        await ExecuteExcel(listaDeFrenquenciaGlobal, request.CodigoCorrelacao);
                        break;
                    default:
                        throw new NegocioException($"Não foi possível exportar este relátorio para o formato {filtroRelatorio.TipoFormatoRelatorio}");
                }
            }
        }

        private async Task GerarRelatorioPdf(List<FrequenciaGlobalDto> listaDeFrequencia, FiltroRelatorioDto request, FiltroFrequenciaGlobalDto filtroRelatorio)
        {
            var dto = new List<FrequenciaMensalDto>();
            var cabecalho = new FrequenciaMensalCabecalhoDto();
            await MapearCabecalho(cabecalho, filtroRelatorio);
            MaperarDto(dto, listaDeFrequencia);
            await ObtenhaRelatorioPaginado(dto, cabecalho, request.CodigoCorrelacao);

        }
        private void MaperarDto(List<FrequenciaMensalDto> dto, List<FrequenciaGlobalDto> listaDeFrequencia)
        {
            foreach (var item in listaDeFrequencia)
            {
                var frequencia = new FrequenciaMensalDto
                {
                    CodigoDre = item.DreCodigo,
                    NomeDre = item.SiglaDre,
                    CodigoUe = item.UeCodigo,
                    NomeUe = item.UeNome,
                    NomeTurma = item.Turma,
                    CodigoTurma = item.TurmaCodigo,
                    NomeMes  = ObterNomeMesReferencia(item.Mes),
                    ValorMes = item.Mes,
                    CodigoAluno = item.CodigoEOL,
                    NumeroAluno = item.NumeroChamadda,
                    NomeAluno = item.Estudante,
                    ProcentagemFrequencia = item.PercentualFrequencia
                };
                dto.Add(frequencia);
            }
        }
        private async Task<string> ObtenhaRelatorioPaginado(List<FrequenciaMensalDto> dto, FrequenciaMensalCabecalhoDto cabecalho, Guid codigoCorrelacao)
        {
            var preparo = new PreparadorDeRelatorioPaginadoFrequenciaGlobalMensal(dto, cabecalho);
            var dtoPaginado = preparo.ObtenhaRelatorioPaginadoDto();

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioPaginado/Index", dtoPaginado, codigoCorrelacao));
        }
        private async Task MapearCabecalho(FrequenciaMensalCabecalhoDto cabecalhoDto, FiltroFrequenciaGlobalDto filtroRelatorio)
        {
            cabecalhoDto.NomeTurma = await ObterNomeTurma(filtroRelatorio);

            await ObterNomeDreUe(filtroRelatorio.CodigoDre, filtroRelatorio.CodigoUe,cabecalhoDto);
            cabecalhoDto.AnoLetivo = filtroRelatorio.AnoLetivo;
            cabecalhoDto.NomeModalidade = filtroRelatorio.Modalidade.Name();
            cabecalhoDto.RfUsuarioSolicitante = filtroRelatorio.UsuarioRf;
            cabecalhoDto.UsuarioSolicitante = filtroRelatorio.UsuarioNome;

            if (filtroRelatorio.MesesReferencias.Count() == 1 && !filtroRelatorio.MesesReferencias.Contains("-99"))
                cabecalhoDto.MesReferencia = ObterNomeMesReferencia(int.Parse(filtroRelatorio.MesesReferencias.FirstOrDefault()));
            else if (filtroRelatorio.MesesReferencias.Count() == 1 && filtroRelatorio.MesesReferencias.Contains("-99"))
                cabecalhoDto.MesReferencia = "Todos";
        }
        private async Task<string> ObterNomeTurma(FiltroFrequenciaGlobalDto filtroRelatorio)
        {
            var nomeTurma = String.Empty;
            if (filtroRelatorio.CodigosTurmas.Count() == 1 && !filtroRelatorio.CodigosTurmas.Contains("-99"))
            {
                var turma = await mediator.Send(new ObterTurmaPorCodigoQuery(filtroRelatorio.CodigosTurmas.FirstOrDefault()));
                nomeTurma = turma.NomePorFiltroModalidade(filtroRelatorio.Modalidade);
            }
            else if (filtroRelatorio.CodigosTurmas.Contains("-99"))
                nomeTurma = "Todas";

            return nomeTurma;
        }
        private async Task ObterNomeDreUe(string dreCodigo, string ueCodigo, FrequenciaMensalCabecalhoDto cabecalhoDto)
        {
            if (!dreCodigo.Contains("-99") && !ueCodigo.Contains("-99"))
            {
                var dadosDreUe = await mediator.Send(new ObterDreUePorDreCodigoQuery(dreCodigo, ueCodigo));
                cabecalhoDto.NomeUe = dadosDreUe.UeNome;
                cabecalhoDto.NomeDre = dadosDreUe.DreNome;
            }
            else if (!dreCodigo.Contains("-99") && ueCodigo.Contains("-99"))
            {
                var dadosDre = await mediator.Send(new ObterDrePorCodigoQuery(dreCodigo));
                cabecalhoDto.NomeDre = dadosDre.Nome;
                cabecalhoDto.NomeUe = "Todas";
            }
            else if (dreCodigo.Contains("-99") && ueCodigo.Contains("-99"))
            {
                cabecalhoDto.NomeUe = "Todas";
                cabecalhoDto.NomeDre = "Todas";
            }
        }
        private async Task ExecuteExcel(List<FrequenciaGlobalDto> listaDeFrequencia, Guid codigoCorrelacao)
        {
            await mediator.Send(new GerarExcelGenericoCommand(listaDeFrequencia.Cast<object>().ToList(), "Frequência Global", codigoCorrelacao, relatorioFrequenciaGlobal: true));
        }
        private string ObterNomeMesReferencia(int mes)
            => Enum.GetValues(typeof(Mes)).Cast<Mes>().Where(x => (int)x == mes).FirstOrDefault().ToString();
    }
}
