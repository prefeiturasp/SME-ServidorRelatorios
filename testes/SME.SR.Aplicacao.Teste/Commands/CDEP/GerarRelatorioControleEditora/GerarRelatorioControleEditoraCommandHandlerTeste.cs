using ClosedXML.Excel;
using MediatR;
using Moq;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleAcervo;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleEditora;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleEditora;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;

namespace SME.SR.Tests.Application.Commands.CDEP.GerarRelatorioControleEditora
{
    public class GerarRelatorioControleEditoraCommandHandlerTeste
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GerarRelatorioControleEditoraCommandHandler _handler;
        private readonly FiltroRelatorioControleEditora _filtros;
        private readonly GerarRelatorioControleEditoraCommand _command;

        public GerarRelatorioControleEditoraCommandHandlerTeste()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new GerarRelatorioControleEditoraCommandHandler(_mediatorMock.Object);

            _filtros = new FiltroRelatorioControleEditora
            {
                Usuario = "João Silva",
                UsuarioRF = "1234567",
                EditoraId = new List<int> { 1, 2, 3 }
            };

            _command = new GerarRelatorioControleEditoraCommand(_filtros);
        }

        [Fact]
        public async Task Handle_Com_Dados_Validos_Deve_Retornar_Memory_Stream_Com_Relatorio()
        {
            // Arrange
            var acervos = CriarListaAcervosTest();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            // Act
            var resultado = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<MemoryStream>(resultado);
            Assert.True(resultado.Length > 0);

            // Verificação do conteúdo do Excel
            using var workbook = new XLWorkbook(resultado);
            var sheet = workbook.Worksheet("Relatório");

            Assert.Equal("CDEP - CENTRO DE DOCUMENTAÇÃO DA EDUCAÇÃO PAULISTANA", sheet.Cell("B2").Value);
            Assert.Equal("Relatório de Controle de Editora", sheet.Cell("B3").Value);
            Assert.Equal("João Silva", sheet.Cell("B5").Value);
            Assert.Equal("RF: 1234567", sheet.Cell("C5").Value);
            Assert.Equal("Editora", sheet.Cell("A7").Value);
            Assert.Equal("Tombo/Código", sheet.Cell("B7").Value);
            Assert.Equal("Título", sheet.Cell("C7").Value);
            Assert.Equal("Situação do empréstimo", sheet.Cell("D7").Value);
            Assert.Equal("Editora A", sheet.Cell("A8").Value);
        }

        [Fact]
        public async Task Handle_Com_Lista_Vazia_Deve_Lancar_Negocio_Exception()
        {
            // Arrange
            var acervosVazio = new List<ControleEditoraDTO>();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervosVazio);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NegocioException>(
                () => _handler.Handle(_command, CancellationToken.None));

            Assert.Equal("Não possui informações.", exception.Message);
        }

        [Fact]
        public async Task Handle_Deve_Chamar_Mediator_Com_Filtros_Corretos()
        {
            // Arrange
            var acervos = CriarListaAcervosTest();
            ObterRelatorioCDEPControleEditoraQuery queryCapturada = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<ControleEditoraDTO>>, CancellationToken>((query, token) =>
                {
                    queryCapturada = (ObterRelatorioCDEPControleEditoraQuery)query;
                })
                .ReturnsAsync(acervos);

            // Act
            await _handler.Handle(_command, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(
                x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.NotNull(queryCapturada);
            Assert.Equal(_filtros.Usuario, queryCapturada.filtros.Usuario);
            Assert.Equal(_filtros.UsuarioRF, queryCapturada.filtros.UsuarioRF);
            Assert.Equal(_filtros.EditoraId, queryCapturada.filtros.EditoraId);
        }

        [Fact]
        public async Task Handle_Com_Diferentes_Situacoes_Emprestimo_Deve_Gerar_Relatorio_Com_Todas_Situacoes()
        {
            // Arrange
            var acervos = new List<ControleEditoraDTO>
            {
                new ControleEditoraDTO
                {
                    Editora = "Editora A",
                    Tombo = "12345",
                    Titulo = "Livro Emprestado",
                    SituacaoEmprestimo = Infra.CDEP.SituacaoEmprestimo.EMPRESTADO
                },
                new ControleEditoraDTO
                {
                    Editora = "Editora B",
                    Tombo = "67890",
                    Titulo = "Livro Devolvido",
                    SituacaoEmprestimo = Infra.CDEP.SituacaoEmprestimo.DEVOLVIDO
                }
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            // Act
            var resultado = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            using var workbook = new XLWorkbook(resultado);
            var sheet = workbook.Worksheet("Relatório");

            Assert.Equal("Livro Emprestado", sheet.Cell("C8").Value);
            Assert.Equal("Emprestado", sheet.Cell("D8").Value);

            Assert.Equal("Livro Devolvido", sheet.Cell("C9").Value);
            Assert.Equal("Devolvido", sheet.Cell("D9").Value);
        }

        [Fact]
        public void Construtor_Deve_Inicializar_Mediator_Corretamente()
        {
            // Arrange & Act
            var handler = new GerarRelatorioControleEditoraCommandHandler(_mediatorMock.Object);

            // Assert
            Assert.NotNull(handler);
        }

        [Fact]
        public async Task Handle_ComGrupoVazio_DeveLancarNegocioException()
        {
            // Arrange
            var acervosVazio = Enumerable.Empty<ControleEditoraDTO>();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervosVazio);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(
                () => _handler.Handle(_command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Com_Situacoes_Com_Descricao_Personalizada_Deve_Usar_Descricao_Correta()
        {
            // Arrange
            var acervos = new List<ControleEditoraDTO>
            {
                new ControleEditoraDTO
                {
                    Editora = "Editora A",
                    Tombo = "12345",
                    Titulo = "Livro Emprestado",
                    SituacaoEmprestimo = Infra.CDEP.SituacaoEmprestimo.EMPRESTADO_PRORROGACAO
                }
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            // Act
            var resultado = await _handler.Handle(_command, CancellationToken.None);

            // Assert
            using var workbook = new XLWorkbook(resultado);
            var sheet = workbook.Worksheet("Relatório");

            Assert.Equal("Emprestado - Prorrogação", sheet.Cell("D8").Value);
        }

        #region Métodos Auxiliares

        private static List<ControleEditoraDTO> CriarListaAcervosTest()
        {
            return new List<ControleEditoraDTO>
            {
                new ControleEditoraDTO
                {
                    Editora = "Editora A",
                    Tombo = "12345",
                    Titulo = "Livro Teste 1",
                    SituacaoEmprestimo = SituacaoEmprestimo.DEVOLVIDO
                },
                new ControleEditoraDTO
                {
                    Editora = "Editora B",
                    Tombo = "67890",
                    Titulo = "Livro Teste 2",
                    SituacaoEmprestimo = SituacaoEmprestimo.EMPRESTADO
                },
                new ControleEditoraDTO
                {
                    Editora = "Editora C",
                    Tombo = "11111",
                    Titulo = "Livro Teste 3",
                    SituacaoEmprestimo = SituacaoEmprestimo.EMPRESTADO_PRORROGACAO
                }
            };
        }

        #endregion
    }
}