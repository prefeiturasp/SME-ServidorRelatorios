using MediatR;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlParaPdfCommand : IRequest<bool>
    {
        public GerarRelatorioHtmlParaPdfCommand(string nomeTemplate, object model, Guid codigoCorrelacao, string mensagemUsuario = "")
        {
            if (!string.IsNullOrWhiteSpace(nomeTemplate))
                NomeTemplate = nomeTemplate.Replace(".cshtml", "");
            Model = model;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
        }

        public Guid CodigoCorrelacao { get; set; }

        public string NomeTemplate { get; set; }
        public object Model { get; }
        public string MensagemUsuario { get; set; }
    }
}
