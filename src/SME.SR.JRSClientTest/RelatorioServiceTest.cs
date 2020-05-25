using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Services;
using SME.SR.Infra.Enumeradores;
using SME.SR.JRSClientTest.Mock;
using System;
using System.IO;
using System.Collections.Generic;
using Refit;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
            var relatorio = relatorioService.GetRelatorioSincrono(new RelatorioSincronoDto {
                CaminhoRelatorio = "/testes/jrsclient/abstract_book_cover.jrxml/abstract_book_cover.jrxml",
                Formato = Enumeradores.FormatoEnum.Pdf
            }).Result;
            Assert.NotNull(relatorio);
        }
    }
}
