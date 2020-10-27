using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery, RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto>
    {
        private readonly IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository;
        private readonly IPerguntasAutoralRepository perguntasAutoralRepository;
        private readonly IMediator mediator;

        public ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQueryHandler(
            IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository,
            IMediator mediator, IPerguntasAutoralRepository perguntasAutoralRepository)
        {
            this.relatorioSondagemPortuguesPorTurmaRepository = relatorioSondagemPortuguesPorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemPortuguesPorTurmaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.perguntasAutoralRepository = perguntasAutoralRepository ?? throw new ArgumentNullException(nameof(perguntasAutoralRepository));
        }

        public async Task<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto> Handle(ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto();

            var periodo = await mediator.Send(new ObterPeriodoPorTipoQuery(request.Bimestre, TipoPeriodoSondagem.Bimestre));

            var semestre = (periodo.Periodo <= 2) ? 1 : 2;

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaAno.ToString(), request.AnoLetivo, periodo.Periodo, request.Usuario.CodigoRf, request.Usuario.Nome);

            var dataDoPeriodo = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(semestre, request.AnoLetivo));

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(request.TurmaCodigo, dataDoPeriodo));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            var perguntas = await perguntasAutoralRepository.ObterPerguntasPorGrupo(GrupoSondagemEnum.CapacidadeLeitura, ComponenteCurricularSondagemEnum.Portugues);

            ObterPerguntas(relatorio, perguntas);

            var dados = await relatorioSondagemPortuguesPorTurmaRepository.ObterPorFiltros(GrupoSondagemEnum.CapacidadeLeitura.Name(), ComponenteCurricularSondagemEnum.Portugues.Name(), 0, request.AnoLetivo, request.TurmaCodigo);

            ObterLinhas(relatorio, dados, alunosDaTurma, perguntas);

            return relatorio;
        }

        private void ObterLinhas(RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto relatorio, IEnumerable<SondagemAutoralPorAlunoDto> dados, IEnumerable<Aluno> alunosDaTurma, IEnumerable<PerguntasOrdemGrupoAutoralDto> perguntas)
        {
            var alunosAgrupados = dados.GroupBy(x => x.CodigoAluno);

            relatorio.Planilha = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaDto();

            relatorio.Planilha.Linhas.AddRange(alunosDaTurma.Select(alunoRetorno =>
            {
                var itemRelatorio = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPlanilhaLinhasDto();
                var aluno = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaAlunoDto()
                {
                    Codigo = alunoRetorno.CodigoAluno,
                    DataSituacao = alunoRetorno.DataSituacao.ToString("dd/MM/yyyy"),
                    Nome = alunoRetorno.NomeAluno,
                    SituacaoMatricula = alunoRetorno.SituacaoMatricula
                };

                itemRelatorio.Aluno = aluno;

                itemRelatorio.OrdensRespostas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto>();
                var alunoRespostas = alunosAgrupados.FirstOrDefault(x => x.Key == aluno.Codigo).ToList();

                itemRelatorio.OrdensRespostas.AddRange(alunoRespostas.Select(aluno =>
                {
                    var itemResposta = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemRespostasDto()
                    {
                        OrdemId = aluno.OrdemId,
                        PerguntaId = aluno.PerguntaId,
                        Resposta = aluno.RespostaDescricao
                    };

                    return itemResposta;
                }));

                return itemRelatorio;
            }));
        }

        private void ObterPerguntas(RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto relatorio, IEnumerable<PerguntasOrdemGrupoAutoralDto> perguntas)
        {
            if (perguntas != null && perguntas.Any())
            {
                relatorio.Cabecalho.Perguntas = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaPerguntaDto>();
                relatorio.Cabecalho.Ordens = new List<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto>();
            }

            var ordensAgrupado = perguntas.GroupBy(p => new { p.OrdemId, p.Ordem });
            var perguntasAgrupado = perguntas.GroupBy(p => new { p.PerguntaId, p.Pergunta });

            relatorio.Cabecalho.Ordens.AddRange(ordensAgrupado.Select(o => new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
            {
                Id = o.Key.OrdemId,
                Descricao = o.Key.Ordem,
            }));

            relatorio.Cabecalho.Ordens.AddRange(perguntasAgrupado.Select(o => new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaOrdemDto()
            {
                Id = o.Key.PerguntaId,
                Descricao = o.Key.Pergunta,
            }));
        }

        private void MontarCabecalho(RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto relatorio, Dre dre, Ue ue, string anoTurma, int anoLetivo, int bimestre, string codigoRf, string usuario)
        {
            relatorio.Cabecalho = new RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaCabecalhoDto()
            {
                Ano = anoTurma,
                AnoLetivo = anoLetivo,
                ComponenteCurricular = "Português",
                DataSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                Dre = dre != null ? dre.Abreviacao : "Todas",
                Periodo = $"{bimestre}º Bimestre",
                Proficiencia = "Capacidade de Leitura",
                Rf = codigoRf,
                Turma = "Todas",
                Ue = ue != null ? ue.NomeComTipoEscola : "Todas",
                Usuario = usuario,
            };
        }
    }
}
