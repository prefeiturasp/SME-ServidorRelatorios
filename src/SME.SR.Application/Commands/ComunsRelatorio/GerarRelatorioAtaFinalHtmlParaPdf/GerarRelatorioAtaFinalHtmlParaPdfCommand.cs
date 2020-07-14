using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application.Commands.ComunsRelatorio.GerarRelatorioHtmlParaPdf
{
    public class GerarRelatorioAtaFinalHtmlParaPdfCommand : IRequest<bool>
    {
        public GerarRelatorioAtaFinalHtmlParaPdfCommand(string nomeTemplate, List<ConselhoClasseAtaFinalPaginaDto> paginas, Guid codigoCorrelacao)
        {
            NomeTemplate = nomeTemplate;
            Paginas = paginas;
            CodigoCorrelacao = codigoCorrelacao;
        }

        public Guid CodigoCorrelacao { get; set; }

        public string NomeTemplate { get; set; }

        public List<ConselhoClasseAtaFinalPaginaDto> Paginas { get; set; }
    }
}
