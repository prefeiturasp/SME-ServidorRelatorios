using Moq;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPTitulosMaisPesquisados;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Aplicacao.Teste.Queries
{
    public class ObterRelatorioCDEPTitulosMaisPesquisadosQueryHandlerTestes
    {
        private readonly Mock<IRelatorioControleLivrosRepository> _relatorioControleLivrosRepositoryMock;
        private readonly ObterRelatorioCDEPTitulosMaisPesquisadosQueryHandler _handler;

        public ObterRelatorioCDEPTitulosMaisPesquisadosQueryHandlerTestes()
        {
            _relatorioControleLivrosRepositoryMock = new Mock<IRelatorioControleLivrosRepository>();
            _handler = new ObterRelatorioCDEPTitulosMaisPesquisadosQueryHandler(_relatorioControleLivrosRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Deve_Retornar_Dados_Quando_Houver_Registros()
        {
            // Arrange
            var query = new ObterRelatorioCDEPTitulosMaisPesquisadosQuery()
            {
                Filtros =
                new FiltroRelatorioTitulosMaisPesquisados
                {
                    DataInicio = DateTime.Now.AddDays(-30),
                    DataFim = DateTime.Now,
                    TipoAcervos = new List<TipoAcervo> { TipoAcervo.DocumentacaoTextual, TipoAcervo.Fotografico }
                }
            };
            var dadosEsperados = new List<RelatorioTitulosMaisPesquisadosDto>
            {
                new RelatorioTitulosMaisPesquisadosDto { TermoNormalizado = "Livro A", Quantidade = 10 },
                new RelatorioTitulosMaisPesquisadosDto { TermoNormalizado = "Livro B", Quantidade = 5 }
            };
            _relatorioControleLivrosRepositoryMock
                .Setup(r => r.ObterRelatorioTitulosMaisPesquisados(query.Filtros.DataInicio, query.Filtros.DataFim, query.Filtros.TipoAcervos))
                .ReturnsAsync(dadosEsperados);
            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);
            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Equal("Livro A", resultado.First().TermoNormalizado);
        }
    }
}