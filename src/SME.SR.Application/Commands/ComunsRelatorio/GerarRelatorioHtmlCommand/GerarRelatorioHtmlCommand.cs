using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlCommand : IRequest<string>
    {
        public GerarRelatorioHtmlCommand(string nomeTemplate, object model, Guid codigoCorrelacao, string mensagemUsuario = "", string mensagemTitulo = "", bool envioPorRabbit = true, 
            string tituloRelatorioRodape = "", bool gerarPaginacao = true, string nomeFila = RotasRabbitSGP.RotaRelatoriosProntosSgp, string mensagemDados = null, Modalidade? modalidade = null)
        {
            if (!string.IsNullOrWhiteSpace(nomeTemplate))
                NomeTemplate = nomeTemplate.Replace(".cshtml", "");
            Model = model;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
            EnvioPorRabbit = envioPorRabbit;
            MensagemDados = mensagemDados;
            GerarPaginacao = gerarPaginacao;
            NomeFila = nomeFila;
            Modalidade = modalidade;
        }

        public Guid CodigoCorrelacao { get; set; }

        public string NomeTemplate { get; set; }
        public object Model { get; }
        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
        public bool EnvioPorRabbit { get; set; }
        public bool GerarPaginacao { get; set; }
        public string NomeFila { get; set; }
        public string MensagemDados { get; set; }
        public Modalidade? Modalidade { get; set; }
    }
}
