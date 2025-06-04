using Moq;
using SME.SR.Application;
using SME.SR.Infra;
using MediatR;
using Newtonsoft.Json;
using SME.SR.Infra.Dtos.Relatorios.MapeamentoEstudante;

namespace SME.SR.Aplicacao.Teste.Commands.ComunsRelatorio.GerarRelatoricoMapeamentosEstudantesExcel
{
    public class GerarRelatorioMapeamentosEstudantesExcelCommandHandlerTeste
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IServicoFila> _servicoFilaMock;        

        public GerarRelatorioMapeamentosEstudantesExcelCommandHandlerTeste()
        {
            _mediatorMock = new Mock<IMediator>();
            _servicoFilaMock = new Mock<IServicoFila>();            
        }


        private MapeamentoEstudanteUltimoBimestreDto CriarEstudanteSimulado(int id = 1)
        {
            return new MapeamentoEstudanteUltimoBimestreDto
            {
                Id = id,
                DreCodigo = 10,
                DreAbreviacao = "DRE NORTE",
                UeCodigo = "123456",
                UeNome = "EMEF Teste",
                TipoEscola = TipoEscola.EMEI,
                Modalidade = Modalidade.Infantil,
                TurmaId = 20,
                TurmaCodigo = "T001",
                TurmaNome = "Pré A",
                AnoLetivo = 2025,
                Semestre = 1,
                AlunoCodigo = "A001",
                AlunoNome = $"Aluno {id}",
                ParecerConclusivoAnoAnterior = JsonConvert.SerializeObject(new ItemGenericoJSON { index = "1", value = "Aprovado" }),
                ParticipaPAP = JsonConvert.SerializeObject(new List<ItemGenericoJSON> {
                new ItemGenericoJSON { index = "1", value = "PAP Matemática" },
                new ItemGenericoJSON { index = "2", value = "PAP Leitura" }
            }),
                ParticipaProjetosMaisEducacao = JsonConvert.SerializeObject(new List<ItemGenericoJSON> {
                new ItemGenericoJSON { index = "1", value = "Mais Educação 1" }
            }),
                ProjetosFortalecimentoAprendizagem = JsonConvert.SerializeObject(new List<ItemGenericoJSON> {
                new ItemGenericoJSON { index = "1", value = "Reforço escolar" }
            }),
                AvaliacoesExternasProvaSP = JsonConvert.SerializeObject(new List<AvaliacaoExternaProvaSPDto>
            {
                new AvaliacaoExternaProvaSPDto {
                    AreaConhecimento = "Matemática",
                    Proficiencia = 250.0,
                    Nivel = "Básico"
                }
            }),
            };
        }
        
        
        private IEnumerable<MapeamentoEstudanteUltimoBimestreDto> CriarMapeamentosEstudantesFake(int quantidade = 1)
        {
            var estudantes = new List<MapeamentoEstudanteUltimoBimestreDto>();

            for (int i = 0; i < quantidade; i++)
            {
                var estudante = CriarEstudanteSimulado(i + 1);

                estudante.AdicionarRespostaBimestral(new RespostaBimestralMapeamentoEstudanteDto
                {
                    Bimestre = 1,
                    AnotacoesPedagogicasBimestreAnterior_Bimestre = "Anotações",
                    AcoesRedeApoio_Bimestre = "Apoio",
                    AcoesRecuperacaoContinua_Bimestre = "Recuperação",
                    HipoteseEscrita_Bimestre = "Silábico",
                    ObsAvaliacaoProcessual_Bimestre = "Observações",
                    Frequencia_Bimestre = "98%",
                    QdadeRegistrosBuscasAtivas_Bimestre = "2"
                });

                estudantes.Add(estudante);
            }

            return estudantes;
        }


        [Fact(DisplayName = "Deve gerar o Excel e publicar na fila com o caminho correto")]
        public async Task Handle_DeveGerarRelatorio_ComCaminhoEsperado()
        {
            // Arrange
            var codigoCorrelacao = Guid.NewGuid();
            var command = new GerarRelatorioMapeamentosEstudantesExcelCommand(
                CriarMapeamentosEstudantesFake(),
                codigoCorrelacao
            );

            PublicaFilaDto filaDtoRecebido = null;

            _servicoFilaMock
                .Setup(s => s.PublicaFila(It.IsAny<PublicaFilaDto>()))
                .Callback<PublicaFilaDto>(dto =>
                {
                    filaDtoRecebido = dto;
                })
                .Returns(Task.CompletedTask);
                        
            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var caminhoEsperado = Path.Combine(caminhoBase, "relatorios", $"{codigoCorrelacao}.xlsx");

            // Act
            var handler = new GerarRelatorioMapeamentosEstudantesExcelCommandHandlerFake(
                _mediatorMock.Object,
                _servicoFilaMock.Object
            );
            await handler.ExecutarHandle(command, CancellationToken.None);

            // Assert - verificar se o arquivo foi salvo
            Assert.True(File.Exists(caminhoEsperado), "O Excel deve ser salvo no caminho esperado");
                        
            // Assert - verificar se a fila foi chamada corretamente
            Assert.NotNull(filaDtoRecebido);
            Assert.Equal(codigoCorrelacao, filaDtoRecebido.CodigoCorrelacao);

            // Cleanup
            if (File.Exists(caminhoEsperado))
                File.Delete(caminhoEsperado);
        }


        [Fact(DisplayName = "Deve gerar o Excel e publicar na fila com o caminho correto para vários estudantes")]
        public async Task Handle_DeveGerarRelatorio_ComCaminhoEsperado_ComVariosEstudantes()
        {
            // Arrange
            var quantidadeEstudantes = 5;
            var codigoCorrelacao = Guid.NewGuid();

            var estudantes = CriarMapeamentosEstudantesFake(quantidadeEstudantes);
            var command = new GerarRelatorioMapeamentosEstudantesExcelCommand(estudantes, codigoCorrelacao);

            PublicaFilaDto filaDtoRecebido = null;

            _servicoFilaMock
                .Setup(s => s.PublicaFila(It.IsAny<PublicaFilaDto>()))
                .Callback<PublicaFilaDto>(dto =>
                {
                    filaDtoRecebido = dto;
                })
                .Returns(Task.CompletedTask);

            var caminhoBase = AppDomain.CurrentDomain.BaseDirectory;
            var caminhoEsperado = Path.Combine(caminhoBase, "relatorios", $"{codigoCorrelacao}.xlsx");

            var handler = new GerarRelatorioMapeamentosEstudantesExcelCommandHandlerFake(
                _mediatorMock.Object,
                _servicoFilaMock.Object
            );

            // Act
            await handler.ExecutarHandle(command, CancellationToken.None);

            // Assert - verificar se o arquivo foi salvo
            Assert.True(File.Exists(caminhoEsperado), "O Excel deve ser salvo no caminho esperado");

            // Assert - verificar se a fila foi chamada corretamente
            Assert.NotNull(filaDtoRecebido);
            Assert.Equal(codigoCorrelacao, filaDtoRecebido.CodigoCorrelacao);

            // Cleanup
            if (File.Exists(caminhoEsperado))
                File.Delete(caminhoEsperado);
        }
              

    }
}
