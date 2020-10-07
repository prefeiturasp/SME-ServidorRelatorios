using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Data.Interfaces;
using SME.SR.Data;
using System.Linq;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemComponentesPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemComponentesPorTurmaQuery, RelatorioSondagemComponentesPorTurmaRelatorioDto>
    { 
        private readonly IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository;
        private readonly IAlunoRepository alunoRepository;
        private readonly IComponenteCurricularRepository componenteCurricularRepository;
        private readonly IDreRepository dreRepository;
        private readonly IUeRepository ueRepository;

        public ObterRelatorioSondagemComponentesPorTurmaQueryHandler(
            IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository,
            IAlunoRepository alunoRepository,
            IComponenteCurricularRepository componenteCurricularRepository,
            IDreRepository dreRepository,
            IUeRepository ueRepository)
        {
            this.relatorioSondagemComponentePorTurmaRepository = relatorioSondagemComponentePorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemComponentePorTurmaRepository));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
        }

        public Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> Handle(ObterRelatorioSondagemComponentesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RelatorioSondagemComponentesPorTurmaRelatorioDto() { 
                Cabecalho = ObterCabecalho(request),
                Planilha = ObterPlanilha(request)
            });
        }

        private RelatorioSondagemComponentesPorTurmaCabecalhoDto ObterCabecalho(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            return new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
            {
                Ano = request.AnoLetivo,
                AnoLetivo = request.AnoLetivo,
                ComponenteCurricular = ObterComponenteCurricular(request.ComponenteCurricularId).Descricao,
                DataSolicitacao = DateTime.Now,
                Dre = this.dreRepository.ObterPorCodigo(request.DreCodigo).Result.Abreviacao,
                Periodo = request.Semestre.ToString(),
                Proficiencia = "Campo Aditivo",
                Turma = request.TurmaCodigo,
                Ue = ObterUe(request.UeCodigo).NomeRelatorio,
                Rf = request.UsuarioRF,
                Usuario = request.UsuarioRF,
                Ordens = this.relatorioSondagemComponentePorTurmaRepository.ObterOrdens(),
                Perguntas = ObterPerguntas()
            };
        }

        private ComponenteCurricularSondagem ObterComponenteCurricular(string componenteCurricularId)
        {
            return this.componenteCurricularRepository.ObterComponenteCurricularDeSondagemPorId(componenteCurricularId).Result.FirstOrDefault();

        }

        private Ue ObterUe(string ueCodigo)
        {
            return this.ueRepository.ObterPorCodigo(ueCodigo).Result;
        }

        public List<RelatorioSondagemComponentesPorTurmaPerguntaDto> ObterPerguntas()
        {
            return new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 1,
                        Nome = "Ideia"
                    },
                    new RelatorioSondagemComponentesPorTurmaPerguntaDto()
                    {
                        Id = 2,
                        Nome = "Resultado"
                    }
                };
        }

        public RelatorioSondagemComponentesPorTurmaPlanilhaDto ObterPlanilha(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();

            foreach (var linha in this.relatorioSondagemComponentePorTurmaRepository.ObterPlanilhaLinhas(request.DreCodigo, request.TurmaCodigo, request.AnoLetivo, request.Semestre))
            {
                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = ObterAluno(linha.TurmaEolCode, linha.AlunoEolCode, linha.AlunoNome),
                    OrdensRespostas = ObterOrdemRespostas(linha)
                });
            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private RelatorioSondagemComponentesPorTurmaAlunoDto ObterAluno(string turmaEolCode, string alunoEolCode, string alunoNome)
        {
            Aluno aluno = alunoRepository.ObterDados(turmaEolCode, alunoEolCode).Result;

            return new RelatorioSondagemComponentesPorTurmaAlunoDto()
            {
                Codigo = alunoEolCode,
                Nome = alunoNome,
                SituacaoMatricula = aluno.CodigoSituacaoMatricula,
                DataSituacao = aluno.DataSituacao
            };
        }

        private List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto> ObterOrdemRespostas(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha)
        {
            return new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
                {
                    new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                        OrdemId = 1,
                        PerguntaId = linha.Ordem1Ideia,
                        Resposta = linha.Ordem1Resultado,
                    },
                    new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                        OrdemId = 2,
                        PerguntaId = linha.Ordem2Ideia,
                        Resposta = linha.Ordem2Resultado,
                    },
                    new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                        OrdemId = 3,
                        PerguntaId = linha.Ordem3Ideia,
                        Resposta = linha.Ordem3Resultado,
                    },
                    new RelatorioSondagemComponentesPorTurmaOrdemRespostasDto() {
                        OrdemId = 4,
                        PerguntaId = linha.Ordem4Ideia,
                        Resposta = linha.Ordem4Resultado,
                    },
                };
        }
    }
}
