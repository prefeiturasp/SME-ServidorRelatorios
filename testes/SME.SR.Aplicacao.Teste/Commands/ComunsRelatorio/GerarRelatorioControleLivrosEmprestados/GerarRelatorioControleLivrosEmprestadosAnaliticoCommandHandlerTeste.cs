using MediatR;
using Moq;
using SME.SR.Application.Commands.CDEP.GerarRelatorioControleLivrosEmprestadosAnalitico;
using SME.SR.Application.Queries.CDEP.ObterRelatorioCDEPControleLivrosEmprestadoSintetico;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;

namespace SME.SR.Aplicacao.Teste.Commands.ComunsRelatorio.GerarRelatorioControleLivrosEmprestados
{
    public class GerarRelatorioControleLivrosEmprestadosAnaliticoCommandHandlerTests
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly GerarRelatorioControleLivrosEmprestadosAnaliticoCommandHandler handler;

        public GerarRelatorioControleLivrosEmprestadosAnaliticoCommandHandlerTests()
        {
            mediatorMock = new Mock<IMediator>();
            handler = new GerarRelatorioControleLivrosEmprestadosAnaliticoCommandHandler(mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoNaoPossuirLivros()
        {
            // Arrange
            var request = new GerarRelatorioControleLivrosEmprestadosAnaliticoCommand(new FiltroRelatorioControleLivro());
            mediatorMock.Setup(m => m.Send(It.IsAny<ObterRelatorioCDEPControleLivrosEmprestadoQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Enumerable.Empty<AcervoSolicitacaoDto>());

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeveGerarRelatorioEretornarCodigoCorrelacao()
        {
            // Arrange
            var filtro = new FiltroRelatorioControleLivro { Usuario = "Teste", UsuarioRF = "12345" };
            var request = new GerarRelatorioControleLivrosEmprestadosAnaliticoCommand(filtro);
            var livros = new List<AcervoSolicitacaoDto>
        {
            new AcervoSolicitacaoDto
            {
                Tombo = "123",
                Titulo = "Livro Teste 1",
                Solicitante = "João",
                SituacaoEmprestimo = SituacaoEmprestimo.EMPRESTADO,
                DataEmprestimo = DateTime.Now.AddDays(-10),
                DataDevolucao = DateTime.Now.AddDays(-1)
            },
            new AcervoSolicitacaoDto
            {
                Tombo = "123",
                Titulo = "Livro Teste 1",
                Solicitante = "Maria",
                SituacaoEmprestimo = SituacaoEmprestimo.EMPRESTADO,
                DataEmprestimo = DateTime.Now.AddDays(-20),
                DataDevolucao = DateTime.Now.AddDays(-10)
            },
            new AcervoSolicitacaoDto
            {
                Tombo = "456",
                Titulo = "Livro Teste 2",
                Solicitante = "José",
                SituacaoEmprestimo = SituacaoEmprestimo.DEVOLVIDO,
                DataEmprestimo = DateTime.Now.AddDays(-50),
                DataDevolucao = DateTime.Now.AddDays(-30)
            }
        };

            mediatorMock.Setup(m => m.Send(It.IsAny<ObterRelatorioCDEPControleLivrosEmprestadoQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(livros);

            // Act
            var resultado = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(Guid.TryParse(resultado, out _));
        }

        [Fact]
        public async Task GerarRelatorio_DeveGerarArquivoExcel()
        {
            // Arrange
            var livros = new List<AcervoSolicitacaoDto>
            {
                new AcervoSolicitacaoDto
                {
                    Tombo = "123",
                    Titulo = "Livro Teste",
                    Solicitante = "Teste",
                    SituacaoEmprestimo = SituacaoEmprestimo.EMPRESTADO,
                    DataEmprestimo = DateTime.Now,
                    DataDevolucao = DateTime.Now
                }
            };

            var codigoCorrelacao = Guid.NewGuid();
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var caminhoDiretorio = Path.Combine(caminhoBase, "relatorios");
            var caminhoRelatorio = Path.Combine(caminhoDiretorio, $"{codigoCorrelacao}.xls");

            // Cria o diretório se ele não existir
            if (!Directory.Exists(caminhoDiretorio))
            {
                Directory.CreateDirectory(caminhoDiretorio);
            }

            if (File.Exists(caminhoRelatorio))
            {
                File.Delete(caminhoRelatorio);
            }

            // Act
            await handler.GerarRelatorio(livros, codigoCorrelacao, "Teste", "12345");

            // Assert
            Assert.True(File.Exists(caminhoRelatorio));
            File.Delete(caminhoRelatorio);
        }
    }
}
