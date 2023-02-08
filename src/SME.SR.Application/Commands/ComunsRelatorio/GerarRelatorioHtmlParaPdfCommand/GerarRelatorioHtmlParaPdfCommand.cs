using MediatR;
using System;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class GerarRelatorioHtmlParaPdfCommand : IRequest<string>
    {
        public GerarRelatorioHtmlParaPdfCommand(string nomeTemplate, object model, Guid codigoCorrelacao, string mensagemUsuario = "", string mensagemTitulo = "", bool envioPorRabbit = true, string tituloRelatorioRodape = "", EnumTipoDePaginacao tipoDePaginacao = EnumTipoDePaginacao.PaginaComTotalPaginas, bool relatorioSincrono = false, string diretorioComplementar = null)
        {
            if (!string.IsNullOrWhiteSpace(nomeTemplate))
                NomeTemplate = nomeTemplate.Replace(".cshtml", "");
            Model = model;
            CodigoCorrelacao = codigoCorrelacao;
            MensagemUsuario = mensagemUsuario;
            MensagemTitulo = mensagemTitulo;
            EnvioPorRabbit = envioPorRabbit;
            TituloRelatorioRodape = tituloRelatorioRodape;
            TipoDePaginacao = tipoDePaginacao;
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
        public EnumTipoDePaginacao TipoDePaginacao { get; set; }
        public bool RelatorioSincrono { get; set; }
        public string DiretorioComplementar { get; set; }
    }
}
