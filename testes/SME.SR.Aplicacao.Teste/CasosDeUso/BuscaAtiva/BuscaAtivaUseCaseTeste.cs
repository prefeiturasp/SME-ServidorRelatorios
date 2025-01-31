using MediatR;
using Moq;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Infra;
using SME.SR.Application.Interfaces;

namespace SME.SR.Aplicacao.Teste.CasosDeUso.Relatorio
{
    public class RelatorioBuscasAtivasUseCaseTests
    {
        private readonly Mock<IMediator> mockMediator;
        private readonly IRelatorioBuscasAtivasUseCase useCase;

        private static readonly string filtroJsonDefault = JsonConvert.SerializeObject(new FiltroRelatorioBuscasAtivasDto
        {
            AnoLetivo = 2025,
            DreCodigo = "1",
            UeCodigo = "2",
            Modalidade = Modalidade.Fundamental,
            Semestre = 1,
            TurmasCodigo = new[] { "T1", "T2" },
            AlunoCodigo = "12345",
            CpfABAE = "123.456.789-00",
            DataInicioRegistroAcao = DateTime.Now.AddMonths(-1),
            DataFimRegistroAcao = DateTime.Now,
            OpcoesRespostaIdMotivoAusencia = new long[] { 1, 2 },
            UsuarioNome = "Usuário Teste",
            UsuarioRf = "RF123"
        });

        private static readonly List<BuscaAtivaSimplesDto> registrosAcaoBuscaAtivaDefault = new List<BuscaAtivaSimplesDto>
        {
            new BuscaAtivaSimplesDto
            {
                Id = 1,
                AlunoNome = "Aluno 1",
                AlunoCodigo = "12345",
                Modalidade = Modalidade.Fundamental,
                TurmaNome = "Turma 1",
                TipoEscola = TipoEscola.EMEF,
                DreCodigo = "1",
                UeCodigo = "2",
                DataRegistroAcao = DateTime.Now,
                ProcedimentoRealizado = "Visita Realizada",
                ConseguiuContatoResponsavel = "Sim",
                QuestoesObsDuranteVisita = "Observações",
                JustificativaMotivoFalta = "Falta justificada",
                JustificativaMotivoFaltaOpcaoOutros = "Motivo específico"
            }
        };

        public RelatorioBuscasAtivasUseCaseTests()
        {
            mockMediator = new Mock<IMediator>();
            useCase = new RelatorioBuscasAtivasUseCase(mockMediator.Object);
        }

        private FiltroRelatorioDto CriarFiltroRelatorioDto(string filtroJson)
        {
            return new FiltroRelatorioDto
            {
                Mensagem = filtroJson
            };
        }

        [Fact]
        public async Task Executar_DeveLancarExcecaoQuandoNaoHouverRegistros()
        {
            var filtroRelatorio = CriarFiltroRelatorioDto(filtroJsonDefault);

            mockMediator.Setup(m => m.Send(It.IsAny<ObterResumoBuscasAtivasQuery>(), default))
                        .ReturnsAsync(new List<BuscaAtivaSimplesDto>());

            var exception = await Assert.ThrowsAsync<NegocioException>(() => useCase.Executar(filtroRelatorio));
            Assert.Equal("Nenhuma informação para os filtros informados.", exception.Message);
        }

        [Fact]
        public async Task Executar_DeveGerarRelatorioQuandoExistiremRegistros()
        {
            var filtroRelatorio = CriarFiltroRelatorioDto(filtroJsonDefault);

            mockMediator.Setup(m => m.Send(It.IsAny<ObterResumoBuscasAtivasQuery>(), default)).ReturnsAsync(registrosAcaoBuscaAtivaDefault);
            mockMediator.Setup(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), default))
                        .ReturnsAsync("Relatório gerado com sucesso");

            await useCase.Executar(filtroRelatorio);

            mockMediator.Verify(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task Executar_DeveAgruparRegistrosCorretamente()
        {
            var filtroRelatorio = CriarFiltroRelatorioDto(filtroJsonDefault);

            mockMediator.Setup(m => m.Send(It.IsAny<ObterResumoBuscasAtivasQuery>(), default)).ReturnsAsync(registrosAcaoBuscaAtivaDefault);

            await useCase.Executar(filtroRelatorio);

            mockMediator.Verify(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task Executar_DeveLancarExcecaoQuandoFalharNaObtencaoDeRegistros()
        {
            var filtroRelatorio = CriarFiltroRelatorioDto(filtroJsonDefault);

            mockMediator.Setup(m => m.Send(It.IsAny<ObterResumoBuscasAtivasQuery>(), default))
                        .ThrowsAsync(new Exception("Erro ao obter dados"));

            var exception = await Assert.ThrowsAsync<Exception>(() => useCase.Executar(filtroRelatorio));
            Assert.Equal("Erro ao obter dados", exception.Message);
        }

        [Fact]
        public async Task Executar_DeveLancarExcecaoQuandoFalharNaGeracaoDoRelatorio()
        {
            var filtroRelatorio = CriarFiltroRelatorioDto(filtroJsonDefault);

            mockMediator.Setup(m => m.Send(It.IsAny<ObterResumoBuscasAtivasQuery>(), default)).ReturnsAsync(registrosAcaoBuscaAtivaDefault);
            mockMediator.Setup(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), default))
                        .ThrowsAsync(new Exception("Erro ao gerar o relatório"));

            var exception = await Assert.ThrowsAsync<Exception>(() => useCase.Executar(filtroRelatorio));
            Assert.Equal("Erro ao gerar o relatório", exception.Message);
        }

       
    }
}
