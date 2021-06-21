using MediatR;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFAcompAprendizagemCommand : IRequest<string>
    {
        public GerarRelatorioHtmlPDFAcompAprendizagemCommand(object model, Guid codigoCorrelacao, string mensagemUsuario = "", string mensagemTitulo = "", bool envioPorRabbit = true)
        {
            Model = model;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
            EnvioPorRabbit = envioPorRabbit;            
        }

        public Guid CodigoCorrelacao { get; set; }                
        public object Model { get; }
        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
        public bool EnvioPorRabbit { get; set; }
    }
}
