using MediatR;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlParaPdfCommand : IRequest<string>
    {
        public GerarRelatorioHtmlParaPdfCommand(string nomeTemplate, object model, Guid codigoCorrelacao, string mensagemUsuario = "", string mensagemTitulo = "", bool envioPorRabbit = true, string tituloRelatorioRodape = "", bool gerarPaginacao = true)
        {
            if (!string.IsNullOrWhiteSpace(nomeTemplate))
                NomeTemplate = nomeTemplate.Replace(".cshtml", "");
            Model = model;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
            EnvioPorRabbit = envioPorRabbit;
            TituloRelatorioRodape = tituloRelatorioRodape;
            GerarPaginacao = gerarPaginacao;
        }

        public Guid CodigoCorrelacao { get; set; }

        public string NomeTemplate { get; set; }
        public string TituloRelatorioRodape { get; set; }
        public object Model { get; }
        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
        public bool EnvioPorRabbit { get; set; }
        public bool GerarPaginacao { get; set; }
    }
}
