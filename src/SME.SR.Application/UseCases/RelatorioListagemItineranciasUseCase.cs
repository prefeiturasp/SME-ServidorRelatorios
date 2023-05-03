using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
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
            var parametros = request.ObterObjetoFiltro<FiltroRelatorioListagemItineranciasDto>();

            try
            {
                var relatorioDto = new RelatorioListagemRegistrosItineranciaDto();

                await ObterFiltrosRelatorio(relatorioDto, parametros);

                relatorioDto.Registros = MapearDTO((await mediator.Send(new ObterListagemItineranciasQuery(parametros))).ToList());
                
                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioListagemRegistrosItinerancia", relatorioDto, request.CodigoCorrelacao, diretorioComplementar: "itinerancia"));
            }
            catch
            {
                throw;
            }
        }

        private List<RegistroListagemItineranciaDto> MapearDTO(List<ListagemItineranciaDto> itinerancias)
        {
            var retorno = itinerancias.Select(itinerancia => 
                                            new RegistroListagemItineranciaDto(
                                                itinerancia.DreAbreviacao
                                                itinerancia.DreAbreviacao,
                                                $"{itinerancia.UeCodigo} - {itinerancia.TipoEscola.ShortName()} {itinerancia.UeNome}",
                                                itinerancia.DataVisita.ToString("dd/MM/yyyy"),
                                                string.Join("|", itinerancia.Objetivos.Select(objetivo => $"{objetivo.Nome}{(!string.IsNullOrEmpty(objetivo.Descricao) ? $": {objetivo.Descricao}" : string.Empty)}")),
                                                string.Join(";", itinerancia.Alunos.Select(aluno => $"{aluno.Nome} ({aluno.Codigo})")),
                                                itinerancia.Situacao.Name(),
                                                $"{itinerancia.ResponsavelPaaiNome} ({itinerancia.ResponsavelPaaiLoginRf})"
                ));

        }

        private async Task ObterFiltrosRelatorio(RelatorioListagemRegistrosItineranciaDto relatorioDto, FiltroRelatorioListagemItineranciasDto parametros)
        {
            relatorioDto.Usuario = parametros.UsuarioNome;
            relatorioDto.RF = parametros.UsuarioRf;
            relatorioDto.DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
