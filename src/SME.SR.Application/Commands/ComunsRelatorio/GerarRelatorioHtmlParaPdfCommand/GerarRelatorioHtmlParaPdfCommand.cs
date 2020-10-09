using MediatR;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlParaPdfCommand : IRequest<bool>
    {
        public GerarRelatorioHtmlParaPdfCommand(string nomeTemplate, object model, Guid codigoCorrelacao, string mensagemUsuario = "", string mensagemTitulo = "")
        {
            if (!string.IsNullOrWhiteSpace(nomeTemplate))
                NomeTemplate = nomeTemplate.Replace(".cshtml", "");
            Model = model;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
        }

        public Guid CodigoCorrelacao { get; set; }

        public string NomeTemplate { get; set; }
        public object Model { get; }
        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
    }
}
