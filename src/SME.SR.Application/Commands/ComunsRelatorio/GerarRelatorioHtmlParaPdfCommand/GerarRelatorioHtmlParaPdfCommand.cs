using MediatR;
using System;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlParaPdfCommand : IRequest<string>
    {
        public GerarRelatorioHtmlParaPdfCommand(string nomeTemplate, object model, Guid codigoCorrelacao, string mensagemUsuario = "", string mensagemTitulo = "", bool envioPorRabbit = true, string tituloRelatorioRodape = "", EnumTipoDePaginas tipoDePaginas = EnumTipoDePaginas.PaginaComTotalPaginas, bool relatorioSincrono = false, string diretorioComplementar = null)
        {
            if (!string.IsNullOrWhiteSpace(nomeTemplate))
                NomeTemplate = nomeTemplate.Replace(".cshtml", "");
            Model = model;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
            EnvioPorRabbit = envioPorRabbit;
            TituloRelatorioRodape = tituloRelatorioRodape;
            TipoDePaginas = tipoDePaginas;
            RelatorioSincrono = relatorioSincrono;
            DiretorioComplementar = diretorioComplementar;
        }

        public Guid CodigoCorrelacao { get; set; }
        public string NomeTemplate { get; set; }
        public string TituloRelatorioRodape { get; set; }
        public object Model { get; }
        public string MensagemUsuario { get; set; }
        public string MensagemTitulo { get; set; }
        public bool EnvioPorRabbit { get; set; }
        public EnumTipoDePaginas TipoDePaginas { get; set; }
        public bool RelatorioSincrono { get; set; }
        public string DiretorioComplementar { get; set; }
    }
}
