using MediatR;
using Moq;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleAcervo;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleEditora;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleEditora;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Text;

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
            var acervos = CriarListaAcervosTest();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            var resultado = await _handler.Handle(_command, CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.IsType<MemoryStream>(resultado);
            Assert.True(resultado.Length > 0);

            resultado.Position = 0;
            var htmlContent = await LerConteudoMemoryStream(resultado);

            Assert.Contains("Relatório de Controle de Editora", htmlContent);
            Assert.Contains("João Silva", htmlContent);
            Assert.Contains("1234567", htmlContent);
            Assert.Contains("<table border='1'", htmlContent);
            Assert.Contains("Editora A", htmlContent);
            Assert.Contains("Livro Teste 1", htmlContent);
            Assert.Contains("12345", htmlContent);
        }

        [Fact]
        public async Task Handle_Com_Lista_Vazia_Deve_Lancar_Negocio_Exception()
        {
            var acervosVazio = new List<ControleEditoraDTO>();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervosVazio);

            var exception = await Assert.ThrowsAsync<NegocioException>(
                () => _handler.Handle(_command, CancellationToken.None));

            Assert.Equal("Não possui informações.", exception.Message);
        }

        [Fact]
        public async Task Handle_Com_Acervos_Agrupados_Deve_Gerar_Relatorio_Correto()
        {
            var acervos = new List<ControleEditoraDTO>
            {
                new ControleEditoraDTO
                {
                    Editora = "Editora A",
                    Tombo = "12345",
                    Titulo = "Livro Repetido",
                    SituacaoEmprestimo = Infra.CDEP.SituacaoEmprestimo.EMPRESTADO
                },
                new ControleEditoraDTO
                {
                    Editora = "Editora A",
                    Tombo = "67890",
                    Titulo = "Livro Repetido",
                    SituacaoEmprestimo = Infra.CDEP.SituacaoEmprestimo.EMPRESTADO
                }
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            var resultado = await _handler.Handle(_command, CancellationToken.None);

            resultado.Position = 0;
            var htmlContent = await LerConteudoMemoryStream(resultado);

            var occurrences = ContarOcorrencias(htmlContent, "Livro Repetido");
            Assert.Equal(1, occurrences);
        }

        [Fact]
        public async Task Handle_Deve_Chamar_Mediator_Com_Filtros_Corretos()
        {
            var acervos = CriarListaAcervosTest();
            ObterRelatorioCDEPControleEditoraQuery queryCapturada = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<ControleEditoraDTO>>, CancellationToken>((query, token) =>
                {
                    queryCapturada = (ObterRelatorioCDEPControleEditoraQuery)query;
                })
                .ReturnsAsync(acervos);

            await _handler.Handle(_command, CancellationToken.None);

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
                    Titulo = "Livro Disponível",
                    SituacaoEmprestimo = Infra.CDEP.SituacaoEmprestimo.DEVOLVIDO
                }
            };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            var resultado = await _handler.Handle(_command, CancellationToken.None);

            resultado.Position = 0;
            var htmlContent = await LerConteudoMemoryStream(resultado);

            Assert.Contains("Livro Emprestado", htmlContent);
            Assert.Contains("Livro Disponível", htmlContent);
        }

        [Fact]
        public async Task Gerar_Arquivo_Para_Excel_Deve_Gerar_HTML_Com_Estilos_Corretos()
        {
            var acervos = CriarListaAcervosTest();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            var resultado = await _handler.Handle(_command, CancellationToken.None);

            resultado.Position = 0;
            var htmlContent = await LerConteudoMemoryStream(resultado);

            Assert.Contains("<html><head>", htmlContent);
            Assert.Contains("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">", htmlContent);
            Assert.Contains("<style>", htmlContent);
            Assert.Contains("th { font-weight: bold; }", htmlContent);
            Assert.Contains(".numero { text-align: center; }", htmlContent);
            Assert.Contains(".data { text-align: right; }", htmlContent);
            Assert.Contains("</style>", htmlContent);
            Assert.Contains("</head><body>", htmlContent);
            Assert.Contains("</table></body></html>", htmlContent);
        }

        [Fact]
        public async Task Gerar_Arquivo_Para_Excel_Deve_Gerar_Cabecalhos_Tabela_Corretos()
        {
            var acervos = CriarListaAcervosTest();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            var resultado = await _handler.Handle(_command, CancellationToken.None);

            resultado.Position = 0;
            var htmlContent = await LerConteudoMemoryStream(resultado);

            Assert.Contains("<th>Editora</th>", htmlContent);
            Assert.Contains("<th>Tombo/Código</th>", htmlContent);
            Assert.Contains("<th>Título</th>", htmlContent);
            Assert.Contains("<th>Situação do empréstimo</th>", htmlContent);
        }

        [Fact]
        public async Task Gerar_Arquivo_Para_Excel_Deve_Aplicar_Classe_CSS_No_Tombo()
        {
            var acervos = CriarListaAcervosTest();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervos);

            var resultado = await _handler.Handle(_command, CancellationToken.None);

            resultado.Position = 0;
            var htmlContent = await LerConteudoMemoryStream(resultado);

            Assert.Contains("class=\"numero\"", htmlContent);
        }

        [Fact]
        public void Construtor_Deve_Inicializar_Mediator_Corretamente()
        {
            var handler = new GerarRelatorioControleEditoraCommandHandler(_mediatorMock.Object);

            Assert.NotNull(handler);
        }

        [Fact]
        public async Task Handle_ComGrupoVazio_DeveLancarNegocioException()
        {
            var acervosVazio = Enumerable.Empty<ControleEditoraDTO>();

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterRelatorioCDEPControleEditoraQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(acervosVazio);

            await Assert.ThrowsAsync<NegocioException>(
                () => _handler.Handle(_command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Com_Situacoes_Com_Descricao_Personalizada_Deve_Usar_Descricao_Correta()
        {
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

            var resultado = await _handler.Handle(_command, CancellationToken.None);

            resultado.Position = 0;
            var htmlContent = await LerConteudoMemoryStream(resultado);

            Assert.Contains("Emprestado", htmlContent);
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

        private static async Task<string> LerConteudoMemoryStream(MemoryStream memoryStream)
        {
            using var reader = new StreamReader(memoryStream, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        private static int ContarOcorrencias(string texto, string busca)
        {
            return (texto.Length - texto.Replace(busca, "").Length) / busca.Length;
        }

        #endregion
    }   

}