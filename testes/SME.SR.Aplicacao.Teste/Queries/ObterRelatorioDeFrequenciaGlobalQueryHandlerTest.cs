using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using SME.SR.Application;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Aplicacao.Teste.Queries
{
    public class ObterRelatorioDeFrequenciaGlobalQueryHandlerTest
    {
        private readonly Mock<IFrequenciaAlunoRepository> _frequenciaRepositorioMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly VariaveisAmbiente _variaveisAmbiente;
        private readonly ObterRelatorioDeFrequenciaGlobalQueryHandler _handler;

        private const int ANO_LETIVO = 2025;
        private const int MES_REFERENCIA = 3;

        public ObterRelatorioDeFrequenciaGlobalQueryHandlerTest()
        {
            _frequenciaRepositorioMock = new Mock<IFrequenciaAlunoRepository>();
            _mediatorMock = new Mock<IMediator>();
            var configuracaoEmMemoria = new Dictionary<string, string> {
                { "ProcessamentoMaximoUes", "1" }
            };
            var configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(configuracaoEmMemoria)
                                .Build();
            _variaveisAmbiente = new VariaveisAmbiente(configuration);

            // Desabilita paralelismo para testes

            _handler = new ObterRelatorioDeFrequenciaGlobalQueryHandler(
                _frequenciaRepositorioMock.Object,
                _mediatorMock.Object,
                _variaveisAmbiente);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioRetornaNuloOuVazio_DeveRetornarListaVazia()
        {
            // Arrange
            var filtro = new FiltroFrequenciaGlobalDto
            {
                CodigosTurmas = new List<string>(),
                MesesReferencias = new List<string>()
            };
            var query = new ObterRelatorioDeFrequenciaGlobalQuery(filtro);
            _frequenciaRepositorioMock.Setup(r => r.ObterFrequenciaAlunoMensal(It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Modalidade>(),
                                                                               It.IsAny<int>(), It.IsAny<string[]>(), It.IsAny<int[]>(), It.IsAny<int>()))
                                      .ReturnsAsync(Enumerable.Empty<FrequenciaAlunoMensalConsolidadoDto>());

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<object>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoFiltroDreEhEspecifica_DeveMapearCorretamente()
        {
            // Arrange
            var filtro = CriarFiltro(codigoDre: "DRE-01");
            var query = new ObterRelatorioDeFrequenciaGlobalQuery(filtro);

            ConfigurarMocks(filtro, "Aluno Ativo", SituacaoMatriculaAluno.Ativo, new DateTime(ANO_LETIVO, MES_REFERENCIA, 10));

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(resultado);
            Assert.Equal("Aluno Ativo", resultado.First().Estudante);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterDadosAlunosEscolaQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoAlunoInativoNoMes_DeveConcatenarSituacaoNoNome()
        {
            // Arrange
            var dataSituacao = new DateTime(ANO_LETIVO, MES_REFERENCIA, 15);
            var filtro = CriarFiltro(codigoDre: "DRE-01");
            var query = new ObterRelatorioDeFrequenciaGlobalQuery(filtro);

            ConfigurarMocks(filtro, "Aluno Inativo", SituacaoMatriculaAluno.Transferido, dataSituacao);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(resultado);
            Assert.Contains(" - Transferido 15/03/2025", resultado.First().Estudante);
        }

        [Fact]
        public async Task Handle_QuandoAlunoInativoAntesDoMes_NaoDeveIncluirNoRelatorio()
        {
            // Arrange
            var dataSituacao = new DateTime(ANO_LETIVO, MES_REFERENCIA - 1, 15); // Inativo no mês anterior
            var filtro = CriarFiltro(codigoDre: "DRE-01");
            var query = new ObterRelatorioDeFrequenciaGlobalQuery(filtro);

            ConfigurarMocks(filtro, "Aluno Inativo Antigo", SituacaoMatriculaAluno.Transferido, dataSituacao);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioRetornaDuplicidade_DeveIncluirAlunoUmaUnicaVez()
        {
            // Arrange
            var filtro = CriarFiltro(codigoDre: "DRE-01");
            var query = new ObterRelatorioDeFrequenciaGlobalQuery(filtro);
            var frequencias = CriarFrequenciaConsolidada("Aluno Duplicado", filtro.CodigoDre, "UE-01");
            var frequenciaDuplicada = CriarFrequenciaConsolidada("Aluno Duplicado", filtro.CodigoDre, "UE-01").Single();
            frequenciaDuplicada.Percentual = 80;
            frequenciaDuplicada.QuantidadeAusencias = 20;
            frequencias.Add(frequenciaDuplicada);

            _frequenciaRepositorioMock.Setup(r => r.ObterFrequenciaAlunoMensal(It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Modalidade>(),
                                                                               It.IsAny<int>(), It.IsAny<string[]>(), It.IsAny<int[]>(), It.IsAny<int>()))
                                      .ReturnsAsync(frequencias);

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterDadosAlunosEscolaQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(CriarDadosAluno("Aluno Duplicado", SituacaoMatriculaAluno.Ativo, new DateTime(ANO_LETIVO, MES_REFERENCIA, 10)));

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(resultado);
            Assert.Equal("12345", resultado.Single().CodigoEOL);
        }

        #region Metodos Privados Auxiliares

        private void ConfigurarMocks(FiltroFrequenciaGlobalDto filtro, string nomeAluno, SituacaoMatriculaAluno situacao, DateTime dataSituacao)
        {
            _frequenciaRepositorioMock.Setup(r => r.ObterFrequenciaAlunoMensal(It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Modalidade>(),
                                                                               It.IsAny<int>(), It.IsAny<string[]>(), It.IsAny<int[]>(), It.IsAny<int>()))
                                      .ReturnsAsync(CriarFrequenciaConsolidada(nomeAluno, filtro.CodigoDre, "UE-01"));

            _mediatorMock.Setup(m => m.Send(It.Is<ObterDrePorCodigoQuery>(q => q.DreCodigo == filtro.CodigoDre), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Dre { Codigo = filtro.CodigoDre, Nome = "DRE TESTE" });

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterDadosAlunosEscolaQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(CriarDadosAluno(nomeAluno, situacao, dataSituacao));
        }

        private FiltroFrequenciaGlobalDto CriarFiltro(string codigoDre)
        {
            return new FiltroFrequenciaGlobalDto
            {
                AnoLetivo = ANO_LETIVO,
                CodigoDre = codigoDre,
                CodigoUe = "-99",
                Modalidade = Modalidade.EJA,
                CodigosTurmas = new List<string> { "-99" },
                MesesReferencias = new List<string> { MES_REFERENCIA.ToString() }
            };
        }

        private List<FrequenciaAlunoMensalConsolidadoDto> CriarFrequenciaConsolidada(string nomeAluno, string codigoDre, string codigoUe)
        {
            return new List<FrequenciaAlunoMensalConsolidadoDto>
            {
                new FrequenciaAlunoMensalConsolidadoDto
                {
                    DreSigla = "DRE-SIGLA",
                    DreCodigo = codigoDre,
                    UeNome = "ESCOLA TESTE",
                    UeCodigo = codigoUe,
                    DescricaoTipoEscola = "EMEF",
                    Mes = MES_REFERENCIA,
                    ModalidadeCodigo = (int)Modalidade.EJA,
                    TurmaNome = "TURMA 01",
                    TurmaCodigo = "T-01",
                    CodigoEol = "12345",
                    Percentual = 90,
                    QuantidadeAulas = 100,
                    QuantidadeAusencias = 10,
                    QuantidadeCompensacoes = 0
                }
            };
        }

        private IEnumerable<DadosMatriculaAlunoDto> CriarDadosAluno(string nomeAluno, SituacaoMatriculaAluno situacao, DateTime dataSituacao)
        {
            return new List<DadosMatriculaAlunoDto>
            {
                new DadosMatriculaAlunoDto
                {
                    CodigoAluno = 12345,
                    NomeAluno = nomeAluno,
                    NomeSocialAluno = "",
                    CodigoSituacaoMatricula = (int)situacao,
                    SituacaoMatricula = situacao.ToString(),
                    DataSituacao = dataSituacao,
                    DataMatricula = new DateTime(ANO_LETIVO, 1, 20),
                    NumeroAlunoChamada = "10",
                    CodigoTurma = "T-01",
                    AnoLetivo = ANO_LETIVO
                }
            };
        }

        #endregion
    }
}
