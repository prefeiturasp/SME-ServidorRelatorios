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

        public ObterRelatorioSondagemComponentesPorTurmaQueryHandler(IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository)
        {
            this.relatorioSondagemComponentePorTurmaRepository = relatorioSondagemComponentePorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemComponentePorTurmaRepository));
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
            // TODO: Verificar como montar o restante de dados do cabeçalho
            return new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
            {
                Ano = request.Ano,
                AnoLetivo = request.Ano,
                ComponenteCurricular = "Matemática",
                DataSolicitacao = DateTime.Now,
                Dre = "",
                Periodo = request.Semestre.ToString(),
                Proficiencia = "Campo Aditivo",
                Turma = "Todas",
                Ue = "CEU EMEF BUTANTA",
                Rf = "987987",
                Usuario = "master",
                Ordens = this.relatorioSondagemComponentePorTurmaRepository.ObterOrdens(),
                Perguntas = ObterPerguntas()
            };
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

            foreach (var linha in this.relatorioSondagemComponentePorTurmaRepository.ObterPlanilhaLinhas(request.DreId, request.TurmaId, request.Ano, request.Semestre))
            {
                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = ObterAluno(linha.AlunoEolCode, linha.AlunoNome),
                    OrdensRespostas = ObterOrdemRespostas(linha)
                });
            }

            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private RelatorioSondagemComponentesPorTurmaAlunoDto ObterAluno(string alunoEolCode, string alunoNome)
        {
            return new RelatorioSondagemComponentesPorTurmaAlunoDto()
            {
                Codigo = alunoEolCode,
                Nome = alunoNome,
                SituacaoMatricula = ObterSituacaoMatriculaAluno(alunoEolCode)
            };
        }

        private SituacaoMatriculaAluno ObterSituacaoMatriculaAluno(string alunoEolCode)
        {
            // TODO: Criar lógica
            return SituacaoMatriculaAluno.Ativo;
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
