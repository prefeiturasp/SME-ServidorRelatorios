using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Services;
using SME.SR.JRSClientTest.Mock;
using System;
using Refit;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace SME.SR.JRSClientTest
{
    public class InformacaoServidorServiceTest
    {
        static JRSClient.Configuracoes Settings = new JRSClient.Configuracoes
        {
            JasperLogin = "user",
            JasperPassword = "bitnami",
            UrlBase = "http://localhost:8080"
        };

        [Fact]
        public void DeveRetornarAsInformacoesDoServidor()
        {
            InformacaoServidorService service = new InformacaoServidorService(Settings);
            InformacaoServidorRespostaDto dto = service.Obter().Result;
            Assert.NotNull(dto);
        }
    }
}
