using MediatR;
using Moq;
using Newtonsoft.Json;
using SME.SR.Application;
using SME.SR.Application.Queries.RelatorioFaltasFrequencia;
using SME.SR.Infra;

namespace SME.SR.Aplicacao.Teste.CasosDeUso.Relatorio { 

    public class RelatorioFrequenciasUseCaseTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RelatorioFrequenciasUseCase _useCase;

        public RelatorioFrequenciasUseCaseTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _useCase = new RelatorioFrequenciasUseCase(_mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_Deve_GerarRelatorioEmPdf_QuandoFormatoForPdf()
        {
            var filtroFrequenciaDto = new FiltroRelatorioFrequenciasDto
            {
                TipoFormatoRelatorio = TipoFormatoRelatorio.Pdf
            };

            var filtroRelatorioDto = new FiltroRelatorioDto
            {
                CodigoCorrelacao = Guid.NewGuid(),
                Mensagem = JsonConvert.SerializeObject(filtroFrequenciaDto)
            };

            _mediatorMock
         .Setup(m => m.Send(It.IsAny<ObterRelatorioFrequenciaPdfQuery>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(new RelatorioFrequenciaDto()); 


            await _useCase.Executar(filtroRelatorioDto);

            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarRelatorioHtmlParaPdfCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_Deve_LancarExcecao_QuandoFormatoNaoSuportado()
        {
            var filtroFrequenciaDto = new FiltroRelatorioFrequenciasDto
            {
                TipoFormatoRelatorio = TipoFormatoRelatorio.Csv
            };

            var filtroRelatorioDto = new FiltroRelatorioDto
            {
                CodigoCorrelacao = Guid.NewGuid(),
                Mensagem = JsonConvert.SerializeObject(filtroFrequenciaDto)
            };

            await Assert.ThrowsAsync<NegocioException>(() => _useCase.Executar(filtroRelatorioDto));
        }
        [Fact]
        public async Task Executar_Deve_LancarExcecao_QuandoErroAoEnviarComando()
        {
            var filtroFrequenciaDto = new FiltroRelatorioFrequenciasDto
            {
                TipoFormatoRelatorio = TipoFormatoRelatorio.Pdf
            };

            var filtroRelatorioDto = new FiltroRelatorioDto
            {
                CodigoCorrelacao = Guid.NewGuid(),
                Mensagem = JsonConvert.SerializeObject(filtroFrequenciaDto)
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterRelatorioFrequenciaPdfQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.Executar(filtroRelatorioDto));
        }
        [Fact]
        public async Task Executar_Deve_LancarExcecao_QuandoFormatoForInvalido()
        {
            var filtroFrequenciaDto = new FiltroRelatorioFrequenciasDto
            {
                TipoFormatoRelatorio = (TipoFormatoRelatorio)999
            };

            var filtroRelatorioDto = new FiltroRelatorioDto
            {
                CodigoCorrelacao = Guid.NewGuid(),
                Mensagem = JsonConvert.SerializeObject(filtroFrequenciaDto)
            };

            await Assert.ThrowsAsync<NegocioException>(() => _useCase.Executar(filtroRelatorioDto));
        }



    }
}