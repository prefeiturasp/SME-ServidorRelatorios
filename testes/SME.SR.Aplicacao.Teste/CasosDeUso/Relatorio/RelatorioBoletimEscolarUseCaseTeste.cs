using MediatR;
using Moq;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Workers.SGP;

namespace SME.SR.Aplicacao.Teste.CasosDeUso.Relatorio
{
    public class RelatorioBoletimEscolarUseCaseTeste
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly RelatorioBoletimEscolarUseCase useCase;

        public RelatorioBoletimEscolarUseCaseTeste()
        {
            mediatorMock = new Mock<IMediator>();
            useCase = new RelatorioBoletimEscolarUseCase(mediatorMock.Object);
        }

        [Fact]
        public void Construtor_Com_Mediator_Nulo_Deve_Lancar_Argument_Null_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new RelatorioBoletimEscolarUseCase(null));
        }

        [Fact]
        public async Task Executar_Deve_Enviar_Query_E_Retornar_Resultado()
        {
            var filtro = new FiltroRelatorioDto
            {
                CodigoCorrelacao = Guid.NewGuid(),
                Mensagem = JsonConvert.SerializeObject(new ObterRelatorioBoletimEscolarQuery
                {
                    DreCodigo = "123456",
                    UeCodigo = "888888",
                    Semestre = 1,
                    TurmaCodigo = "T001",
                    AnoLetivo = 2025,
                    Modalidade = Modalidade.Fundamental,
                    AlunosCodigo = new[] { "A001", "A002" },
                    Usuario = new Usuario { Nome = "Teste" },
                    ConsideraHistorico = true,
                    ConsideraInativo = false
                })
            };

            var relatorioMock = new List<RelatorioBoletimSimplesEscolarDto>
            {
                new RelatorioBoletimSimplesEscolarDto
                {
                    EhRegencia = false,
                    ModalidadeTurma = Modalidade.Fundamental,
                    Cabecalho = new BoletimEscolarCabecalhoDto(),
                    ComponentesCurriculares = new List<ComponenteCurricularDto>(),
                    ComponenteCurricularRegencia = new ComponenteCurricularRegenciaDto(),
                    ParecerConclusivo = "Aprovado"
                }
            };

            mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRelatorioBoletimEscolarQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(relatorioMock);

            mediatorMock
                .Setup(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("ok");

            await useCase.Executar(filtro);

            mediatorMock.Verify(m => m.Send(It.IsAny<ObterRelatorioBoletimEscolarQuery>(), It.IsAny<CancellationToken>()), Moq.Times.Once);
            mediatorMock.Verify(m => m.Send(It.Is<GerarRelatorioHtmlParaPdfCommand>(c =>
                c.NomeTemplate == "RelatorioBoletimEscolarSimples" &&
                c.Model == relatorioMock &&
                c.CodigoCorrelacao == filtro.CodigoCorrelacao
            ), It.IsAny<CancellationToken>()), Moq.Times.Once);

            Assert.Equal(RotasRabbitSGP.RotaRelatoriosComErroBoletim, filtro.RotaErro);
        }
    }
}
