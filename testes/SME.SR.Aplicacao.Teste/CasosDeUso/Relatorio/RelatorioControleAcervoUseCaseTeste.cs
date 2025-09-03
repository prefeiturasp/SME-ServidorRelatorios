using MediatR;
using Moq;
using SME.SR.Application.UseCases;
using SME.SR.Infra;
using SME.SR.Infra.CDEP;
using SME.SR.Infra.Dtos.Relatorios.CDEP;
using System.Text;

namespace SME.SR.Aplicacao.Teste.CasosDeUso.Relatorio
{
    public class RelatorioControleAcervoUseCaseTeste
    {
        [Fact(DisplayName = "Executar - Deve gerar relatório de controle de acervo com sucesso")]
        public async Task Executar_Deve_Gerar_Relatorio_Com_Sucesso()
        {
            var filtro = new FiltroRelatorioControleAcervo
            {
                Usuario = "João",
                UsuarioRF = "12345",
                SituacaoAcervo = SituacaoAcervo.ATIVO,
                TipoAcervo = TipoAcervo.Bibliografico,
                TiposAcervosPermitidos = new long[] { 1, 2 }
            };

            var filtroDto = new FiltroRelatorioSincronoDto
            {
                Mensagem = Newtonsoft.Json.JsonConvert.SerializeObject(filtro)
            };

            var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes("relatorio fake"));

            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<IRequest<MemoryStream>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStream);

            var useCase = new RelatorioControleAcervoUseCase(mediatorMock.Object);

            var result = await useCase.Executar(filtroDto);

            Assert.NotNull(result);
            Assert.Equal(expectedStream, result);
            mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<MemoryStream>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve relançar exceção ao falhar")]
        public async Task Executar_Deve_Relancar_Excecao()
        {
            var filtro = new FiltroRelatorioControleAcervo
            {
                Usuario = "Maria",
                UsuarioRF = "67890",
                SituacaoAcervo = SituacaoAcervo.INATIVO,
                TipoAcervo = TipoAcervo.Fotografico,
                TiposAcervosPermitidos = new long[] { 3, 4 }
            };

            var filtroDto = new FiltroRelatorioSincronoDto
            {
                Mensagem = Newtonsoft.Json.JsonConvert.SerializeObject(filtro)
            };

            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<IRequest<MemoryStream>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Erro no Mediator"));

            var useCase = new RelatorioControleAcervoUseCase(mediatorMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.Executar(filtroDto));
            Assert.Equal("Erro no Mediator", ex.Message);
        }
    }
}
