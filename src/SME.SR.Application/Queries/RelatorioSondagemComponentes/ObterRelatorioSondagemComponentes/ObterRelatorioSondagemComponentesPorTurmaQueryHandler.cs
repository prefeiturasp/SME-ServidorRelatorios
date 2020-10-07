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
        private readonly IUsuarioRepository usuarioRepository;

        public ObterRelatorioSondagemComponentesPorTurmaQueryHandler(
            IRelatorioSondagemComponentePorTurmaRepository relatorioSondagemComponentePorTurmaRepository,
            IAlunoRepository alunoRepository,
            IComponenteCurricularRepository componenteCurricularRepository,
            IDreRepository dreRepository,
            IUeRepository ueRepository,
            IUsuarioRepository usuarioRepository)
        {
            this.relatorioSondagemComponentePorTurmaRepository = relatorioSondagemComponentePorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemComponentePorTurmaRepository));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.componenteCurricularRepository = componenteCurricularRepository ?? throw new ArgumentNullException(nameof(componenteCurricularRepository));
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> Handle(ObterRelatorioSondagemComponentesPorTurmaQuery request, CancellationToken cancellationToken)
        {
            RelatorioSondagemComponentesPorTurmaCabecalhoDto cabecalho = await ObterCabecalho(request);
            RelatorioSondagemComponentesPorTurmaPlanilhaDto planilha = await ObterPlanilha(request);

            return new RelatorioSondagemComponentesPorTurmaRelatorioDto() { 
                Cabecalho = cabecalho,
                Planilha = planilha
            };
        }

        private async Task<RelatorioSondagemComponentesPorTurmaCabecalhoDto> ObterCabecalho(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var componenteCurricular = await ObterComponenteCurricular(request.ComponenteCurricularId);
            var ordens = await ObterOrdens();
            var ue = await ObterUe(request.UeCodigo);
            var usuario = await this.usuarioRepository.ObterDados(request.UsuarioRF);
            var perguntas = await ObterPerguntas();

            return new RelatorioSondagemComponentesPorTurmaCabecalhoDto()
            {
                Ano = request.AnoLetivo,
                AnoLetivo = request.AnoLetivo,
                ComponenteCurricular = componenteCurricular.FirstOrDefault().Descricao,
                DataSolicitacao = DateTime.Now,
                Dre = this.dreRepository.ObterPorCodigo(request.DreCodigo).Result.Abreviacao,
                Periodo = request.Semestre.ToString(),
                Proficiencia = request.ProficienciaId.ToString(),
                Turma = request.TurmaCodigo,
                Ue = ue.NomeRelatorio,
                Rf = request.UsuarioRF,
                Usuario = usuario.Login,
                Ordens = ordens.ToList(),
                Perguntas = perguntas
            };
        }

        private async Task<IEnumerable<RelatorioSondagemComponentesPorTurmaOrdemDto>> ObterOrdens()
        {
            return await this.relatorioSondagemComponentePorTurmaRepository.ObterOrdensAsync();

        }

        private async Task<IEnumerable<ComponenteCurricularSondagem>> ObterComponenteCurricular(string componenteCurricularId)
        {
            return await this.componenteCurricularRepository.ObterComponenteCurricularDeSondagemPorId(componenteCurricularId);

        }

        private async Task<Ue> ObterUe(string ueCodigo)
        {
            return await this.ueRepository.ObterPorCodigo(ueCodigo);
        }

        public async Task<List<RelatorioSondagemComponentesPorTurmaPerguntaDto>> ObterPerguntas()
        {
            return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaPerguntaDto>()
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
                });
        }

        public async Task<RelatorioSondagemComponentesPorTurmaPlanilhaDto> ObterPlanilha(ObterRelatorioSondagemComponentesPorTurmaQuery request)
        {
            var planilhaLinhas = await this.relatorioSondagemComponentePorTurmaRepository.ObterPlanilhaLinhas(request.DreCodigo, request.TurmaCodigo, request.AnoLetivo, request.Semestre);

            List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> linhasPlanilhaQueryDto = new List<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>();
            foreach (var linha in planilhaLinhas.ToList())
            {
                var aluno = await ObterAluno(linha.TurmaEolCode, linha.AlunoEolCode, linha.AlunoNome);
                var respostas = await ObterOrdemRespostas(linha);

                linhasPlanilhaQueryDto.Add(new RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto()
                {
                    Aluno = aluno,
                    OrdensRespostas = respostas
                });
            }
            return new RelatorioSondagemComponentesPorTurmaPlanilhaDto() { Linhas = linhasPlanilhaQueryDto };
        }

        private async Task<RelatorioSondagemComponentesPorTurmaAlunoDto> ObterAluno(string turmaEolCode, string alunoEolCode, string alunoNome)
        {
            Aluno aluno = await alunoRepository.ObterDados(turmaEolCode, alunoEolCode);

            return new RelatorioSondagemComponentesPorTurmaAlunoDto()
            {
                Codigo = alunoEolCode,
                Nome = alunoNome,
                SituacaoMatricula = aluno.CodigoSituacaoMatricula,
                DataSituacao = aluno.DataSituacao
            };
        }

        private async Task<List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>> ObterOrdemRespostas(RelatorioSondagemComponentesPorTurmaPlanilhaQueryDto linha)
        {
            return await Task.FromResult(new List<RelatorioSondagemComponentesPorTurmaOrdemRespostasDto>()
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
                });
        }
    }
}
