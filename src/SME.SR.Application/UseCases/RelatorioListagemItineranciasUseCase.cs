using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Extensions;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioListagemItineranciasUseCase : IRelatorioListagemItineranciasUseCase
    {
        private readonly IMediator mediator;

        public RelatorioListagemItineranciasUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            //var parametros = request.ObterObjetoFiltro<FiltroRelatorioListagemItineranciasDto>();
            var parametros = new FiltroRelatorioListagemItineranciasDto()
            {
                DreCodigo = "108100",
                UeCodigo = "",
                AnoLetivo = 2021,
                UsuarioNome = "Jailson",
                UsuarioRf = "9999",
                CodigosPAAIResponsavel = new string[] {"8239614", "7940017", "8160376" },
                SituacaoIds = new int[] {2, 3, 4}
            };

            try
            {
                var itinerancias = (await mediator.Send(new ObterListagemItineranciasQuery(parametros))).ToList();
                if (itinerancias == null || !itinerancias.Any())
                    throw new NegocioException("Nenhuma informação para os filtros informados.");

                var relatorioDto = new RelatorioListagemRegistrosItineranciaDto();
                relatorioDto.Registros = MapearDTO(itinerancias);

                PreencherFiltrosRelatorio(relatorioDto, parametros);
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioListagemRegistrosItinerancia", relatorioDto, request.CodigoCorrelacao, diretorioComplementar: "itinerancia"));
            }
            catch
            {
                throw;
            }
        }

        private IEnumerable<RegistroListagemItineranciaDto> MapearDTO(List<ListagemItineranciaDto> itinerancias)
        {
            var retorno = itinerancias.OrderBy(itinerancia => itinerancia.DreCodigo)
                            .ThenBy(itinerancia => itinerancia.TipoEscola.ShortName())            
                            .ThenBy(itinerancia => itinerancia.UeNome)
                            .ThenBy(itinerancia => itinerancia.DataVisita)
                            .Select(itinerancia => 
                                    new RegistroListagemItineranciaDto(
                                        itinerancia.DreAbreviacao,
                                        $"{itinerancia.UeCodigo} - {itinerancia.TipoEscola.ShortName()} {itinerancia.UeNome}",
                                        itinerancia.DataVisita,
                                        string.Join("|", itinerancia.Objetivos.Select(objetivo => $"{objetivo.Nome}{(!string.IsNullOrEmpty(objetivo.Descricao) ? $": {objetivo.Descricao}" : string.Empty)}")),
                                        string.Join(";", itinerancia.Alunos.Select(aluno => $"{aluno.Nome} ({aluno.Codigo})")),
                                        itinerancia.Situacao.Name(),
                                        $"{itinerancia.ResponsavelPaaiNome} ({itinerancia.ResponsavelPaaiLoginRf})"
                ));
            return retorno;
        }

        private void PreencherFiltrosRelatorio(RelatorioListagemRegistrosItineranciaDto relatorioDto, FiltroRelatorioListagemItineranciasDto parametros)
        {
            relatorioDto.Usuario = parametros.UsuarioNome;
            relatorioDto.RF = parametros.UsuarioRf;
            relatorioDto.DataSolicitacao = DateTime.Now;
            relatorioDto.Dre = parametros.DreCodigo.EstaFiltrandoTodas() ? "TODAS" : relatorioDto.Registros.FirstOrDefault()?.Dre;
            relatorioDto.Ue = parametros.UeCodigo.EstaFiltrandoTodas() ? "TODAS" : relatorioDto.Registros.FirstOrDefault()?.Ue;
        }
    }
}
