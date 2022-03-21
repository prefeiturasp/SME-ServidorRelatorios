using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlPDFBoletimDetalhadoCommand : IRequest<string>
    {
        public GerarRelatorioHtmlPDFBoletimDetalhadoCommand(object model, Guid codigoCorrelacao, Modalidade modalidade, string mensagemUsuario = "", string mensagemTitulo = "", bool envioPorRabbit = true, string mensagemDados = null)
        {
            Model = model;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
            EnvioPorRabbit = envioPorRabbit;
            Modalidade = modalidade;
            MensagemDados = mensagemDados;
        }

        public Guid CodigoCorrelacao { get; set; }                
        public object Model { get; }
        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
        public bool EnvioPorRabbit { get; set; }
        public Modalidade Modalidade { get; set; }
        public string MensagemDados { get; set; }

    }
}
