using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioNotasEConceitosFinaisUseCase : IRelatorioNotasEConceitosFinaisUseCase
    {
        private readonly IMediator mediator;

        public RelatorioNotasEConceitosFinaisUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroNotasConceitosFinais;
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioNotasEConceitosFinaisDto>();
            var relatorioNotasEConceitosFinaisDto = await mediator.Send(new ObterRelatorioNotasEConceitosFinaisPdfQuery(filtros));
            var msgTituloRelatorio = $"Relatório de Notas e Conceitos Finais - {relatorioNotasEConceitosFinaisDto.DreNome}";
            switch (filtros.TipoFormatoRelatorio)
            {
                case TipoFormatoRelatorio.Xlsx:
                    var relatorioDto = await mediator.Send(new ObterRelatorioNotasEConceitosFinaisExcelQuery() { RelatorioNotasEConceitosFinais = relatorioNotasEConceitosFinaisDto });
                    if (relatorioDto == null)
                        throw new NegocioException($"Não foi possível transformar os dados obtidos em dados excel - {relatorioNotasEConceitosFinaisDto.DreNome}");

                    var possuiNotaFechamento = relatorioDto.Any(r => r.NotaConceito.Contains("*"));

                    await mediator.Send(new GerarExcelGenericoCommand(relatorioDto.ToList<object>(), "RelatorioNotasEConceitosFinais", request.CodigoCorrelacao, 
                                                                      possuiNotaFechamento, "* Estudante sem conselho de classe registrado , ** Aguardando aprovação",
                                                                      mensagemTitulo: msgTituloRelatorio));

                    break;
                case TipoFormatoRelatorio.Pdf:
                    await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioNotasEConceitosFinais", relatorioNotasEConceitosFinaisDto, request.CodigoCorrelacao, mensagemTitulo: msgTituloRelatorio));
                    break;
                case TipoFormatoRelatorio.Rtf:
                case TipoFormatoRelatorio.Html:
                case TipoFormatoRelatorio.Xls:
                case TipoFormatoRelatorio.Csv:
                case TipoFormatoRelatorio.Xml:
                case TipoFormatoRelatorio.Docx:
                case TipoFormatoRelatorio.Odt:
                case TipoFormatoRelatorio.Ods:
                case TipoFormatoRelatorio.Jrprint:
                default:
                    throw new NegocioException($"Não foi possível exportar este relátorio para o formato {filtros.TipoFormatoRelatorio}");
            }
        }
    }
}