using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Services;
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
    public class RecursoServiceTest
    {
        // TODO Evitar instancia compartilhada na execução dos caso de testes
        static JRSClient.Configuracoes Settings = new JRSClient.Configuracoes
        {
            JasperLogin = "user",
            JasperPassword = "bitnami",
            UrlBase = "http://localhost:8080"
        };

        [Fact]
        public void DeveCriarUmaUnidadeRelatorio()
        {
            RecursoService recursoService = new RecursoService(Settings);
            UnidadeRelatorioRecursoDto relatorio = recursoService.CriarRelatorio(
                "/testes/jrsclient/abstract_book_cover.jrxml",
                true,
                true,
                new UnidadeRelatorioRecursoDto
                {
                    Titulo = "abstract_book_cover.jrxml",
                    JRXML = new JRXMLRecursoDto
                    {
                        Arquivo = new ArquivoJRXMLRecursoDto
                        {
                            Tipo = "jrxml",
                            Titulo = "abstract_book_cover.jrxml",
                            ConteudoBase64 = RecursoServiceStaticMock.AbstractBookCoverReportContentBase64
                        }
                    }
                }
            ).Result;
            Assert.NotNull(relatorio);
        }

        [Fact]
        public void DeveObterDetalhesDoRelatorio() {
            RecursoService service = new RecursoService(Settings);
            var detalhes = service.ObterDetalhesRecurso(
                "/testes/jrsclient/abstract_book_cover.jrxml", 
                true
            ).Result;
            Assert.NotNull(detalhes);
        }
    }
}
