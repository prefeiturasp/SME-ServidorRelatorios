using SME.SR.Infra.Dtos;
using System.Collections.Generic;

namespace SME.SR.HtmlPdf
{
    public interface IReportConverter
    {
        byte[] ConvertToPdf(List<string> paginas);
        void Converter(string html, string nomeArquivo, string tituloRelatorioRodape = "", bool gerarPaginacao = true);
        void ConvertToPdfPaginacaoSolo(List<PaginaParaRelatorioPaginacaoSoloDto> paginas, string caminhoBase, string nomeArquivo);
    }
}
