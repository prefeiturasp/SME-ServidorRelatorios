using SME.SR.Infra.Dtos;
using System.Collections.Generic;
using SME.SR.Infra;

namespace SME.SR.HtmlPdf
{
    public interface IReportConverter
    {
        byte[] ConvertToPdf(List<string> paginas);
        void Converter(string html, string nomeArquivo, string tituloRelatorioRodape = "", EnumTipoDePaginas tipoDePaginas = EnumTipoDePaginas.PaginaComTotalPaginas, string templateHeader = "");
        void ConvertToPdfPaginacaoSolo(List<PaginaParaRelatorioPaginacaoSoloDto> paginas, string caminhoBase, string nomeArquivo, string tituloRelatorioRodape = "");
    }
}
