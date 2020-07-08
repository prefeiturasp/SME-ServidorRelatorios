using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Services;
using System.IO;
using Xunit;

namespace SME.SR.JRSClientTest
{
    public class RelatorioServiceTest
    {
        static JRSClient.Configuracoes Settings = new JRSClient.Configuracoes
        {
            JasperLogin = "user",
            JasperPassword = "bitnami",
            UrlBase = "http://localhost:8080"
        };

        [Fact]
        public void DeveExecutarUmRelatorioSincrono()
        {
            RelatorioService relatorioService = new RelatorioService(Settings);
            Stream relatorio = relatorioService.GetRelatorioSincrono(new RelatorioSincronoDto
            {
                CaminhoRelatorio = "/testes/jrsclient/abstract_book_cover.jrxml/abstract_book_cover.jrxml",
                Formato = TipoFormatoRelatorio.Pdf
            }).Result;
            Assert.NotNull(relatorio);
        }
    }
}
