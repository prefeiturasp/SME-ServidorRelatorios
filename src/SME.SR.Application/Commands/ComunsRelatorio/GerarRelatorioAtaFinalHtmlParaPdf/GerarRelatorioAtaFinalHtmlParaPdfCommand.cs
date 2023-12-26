using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class GerarRelatorioAtaFinalHtmlParaPdfCommand : IRequest<bool>
    {
        public GerarRelatorioAtaFinalHtmlParaPdfCommand(string nomeTemplate, List<ConselhoClasseAtaFinalPaginaDto> paginas, Guid codigoCorrelacao, string mensagemUsuario)
        {
            NomeTemplate = nomeTemplate;
            Paginas = paginas;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
        }

        public Guid CodigoCorrelacao { get; set; }

        public string NomeTemplate { get; set; }

        public List<ConselhoClasseAtaFinalPaginaDto> Paginas { get; set; }

        public string MensagemUsuario { get; set; }
    }
}
