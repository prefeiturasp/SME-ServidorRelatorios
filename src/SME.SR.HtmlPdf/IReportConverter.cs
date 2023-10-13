using SME.SR.Infra.Dtos;
using System.Collections.Generic;
using SME.SR.Infra;
using DinkToPdf;

namespace SME.SR.HtmlPdf
{
    public interface IReportConverter
    {
        byte[] ConvertToPdf(List<string> paginas);
        void Converter(string html, string nomeArquivo, string tituloRelatorioRodape = "", EnumTipoDePaginacao tipoDePaginacao = EnumTipoDePaginacao.PaginaComTotalPaginas, string templateHeader = "");
        void ConvertToPdfPaginacaoSolo(List<PaginaParaRelatorioPaginacaoSoloDto> paginas, string caminhoBase, string nomeArquivo, string tituloRelatorioRodape = "", Orientation orientacaoRelatorio = Orientation.Portrait);
    }
}
