using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application.Services
{
    public abstract class ServicoAnaliticoSondagemAbstract 
    {
        protected readonly IAlunoRepository alunoRepository;
        protected readonly IDreRepository dreRepository;
        protected readonly IUeRepository ueRepository;
        protected readonly ISondagemRelatorioRepository sondagemRelatorioRepository;
        protected readonly ITurmaRepository turmaRepository;
        protected readonly ISondagemAnaliticaRepository sondagemAnaliticaRepository;
        protected FiltroRelatorioAnaliticoSondagemDto filtro;
        protected PeriodoFixoSondagem periodoFixoSondagem;

        protected const int ANO_ESCOLAR_2022 = 2022;
        protected const int ANO_ESCOLAR_2023 = 2023;
        protected const int PRIMEIRO_BIMESTRE = 1;
        private const int TERCEIRO_BIMESTRE = 3;
        private const int QUARTO_BIMESTRE = 4;
        private const int SEGUNDO_SEMESTRE = 2;
        private const int ANO_LETIVO_2024 = 2024;
        private const int ANO_LETIVO_2025 = 2025;
        protected const string TURMA_TERCEIRO_ANO = "3";
        private const string TODOS = "-99";

        protected ServicoAnaliticoSondagemAbstract(IAlunoRepository alunoRepository, 
                                                   IDreRepository dreRepository, 
                                                   IUeRepository ueRepository, 
                                                   ISondagemRelatorioRepository sondagemRelatorioRepository,
                                                   ISondagemAnaliticaRepository sondagemAnaliticaRepository,
                                                   ITurmaRepository turmaRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new System.ArgumentNullException(nameof(alunoRepository));
            this.dreRepository = dreRepository ?? throw new System.ArgumentNullException(nameof(dreRepository));
            this.ueRepository = ueRepository ?? throw new System.ArgumentNullException(nameof(ueRepository));
            this.sondagemAnaliticaRepository = sondagemAnaliticaRepository ?? throw new System.ArgumentNullException(nameof(sondagemAnaliticaRepository));
            this.sondagemRelatorioRepository = sondagemRelatorioRepository ?? throw new System.ArgumentNullException(nameof(sondagemRelatorioRepository));
            this.turmaRepository = turmaRepository ?? throw new System.ArgumentNullException(nameof(turmaRepository));
        }

        protected abstract bool EhTodosPreenchidos();

        protected Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem(string termo)
        {
            return sondagemAnaliticaRepository.ObterPeriodoFixoSondagem(termo, filtro.AnoLetivo);
        }

        protected async Task<IEnumerable<TotalDeTurmasPorAnoDto>> ObterQuantidadeTurmaPorAnoDre(long dreId)
        {
            if (filtro.UeCodigo != TODOS)
                return Enumerable.Empty<TotalDeTurmasPorAnoDto>();

            return await ObterQuantidadeTurmaPorAnoUe(dreId, string.Empty, null);
        }

        protected async Task<IEnumerable<TotalDeTurmasPorAnoDto>> ObterQuantidadeTurmaPorAnoUe(long dreId, string codigoUe, IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas)
        {
            if (!(totalDeTurmas is null) && totalDeTurmas.Any())
                return totalDeTurmas.Where(x => x.CodigoUe == codigoUe);

            return await turmaRepository.ObterTotalDeTurmasPorUeAnoLetivoEModalidade(dreId, codigoUe, (int)Modalidade.Fundamental, filtro.AnoLetivo);
        }

        protected string ObterTituloSemestreBimestrePortugues(bool ehIAD)
        {
            return ObterDescricaoSemestreBimestre(ehIAD && filtro.AnoLetivo >= 2024);
        }

        protected string ObterTituloSemestreBimestreMatematica(bool ehIAD)
        {
            return ObterDescricaoSemestreBimestre(filtro.AnoLetivo < 2022 || filtro.AnoLetivo > 2022 && ehIAD);
        }

        protected async Task<IEnumerable<Dre>> ObterDres(string[] codigos)
        {
            return await dreRepository.ObterPorCodigos(codigos);
        }

        protected async Task<IEnumerable<Ue>> ObterUe(string[] codigos)
        {
            return await ueRepository.ObterPorCodigos(codigos);
        }

        protected async Task<IEnumerable<TotalAlunosAnoTurmaDto>> ObterTotalDeAlunosPorDre(string codigoDre)
        {
            if (EhTodosPreenchidos() || filtro.UeCodigo != TODOS)
                return Enumerable.Empty<TotalAlunosAnoTurmaDto>();

            return await ObterTotalDeAlunosPorUe(codigoDre, string.Empty, null);
        }

        protected async Task<IEnumerable<TotalAlunosAnoTurmaDto>> ObterTotalDeAlunosPorUe(
                                                                string codigoDre, 
                                                                string codigoUe,
                                                                IEnumerable<TotalAlunosAnoTurmaDto> totalDeAlunos)
        {
            if (EhTodosPreenchidos())
                return Enumerable.Empty<TotalAlunosAnoTurmaDto>();

            if (!(totalDeAlunos is null) && totalDeAlunos.Any())
                return totalDeAlunos.Where(x => x.CodigoUe == codigoUe);

            return await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(
                                                    filtro.AnoLetivo,
                                                    new int[] { 5, 13 },
                                                    this.periodoFixoSondagem.DataInicio.Date,
                                                    this.periodoFixoSondagem.DataFim.Date,
                                                    codigoUe,
                                                    codigoDre);
        }

        protected bool EhPreenchimentoDeTodosEstudantesIAD()
        {
            return filtro.AnoLetivo == ANO_LETIVO_2024 && filtro.Periodo == SEGUNDO_SEMESTRE || filtro.AnoLetivo >= ANO_LETIVO_2025;
        }

        protected bool EhPreenchimentoDeTodosEstudantes()
        {
            var bimestres = new int[] { TERCEIRO_BIMESTRE, QUARTO_BIMESTRE };

            return filtro.AnoLetivo == ANO_LETIVO_2024 && bimestres.Contains(filtro.Periodo) || filtro.AnoLetivo >= ANO_LETIVO_2025;
        }

        protected int ObterTotalDeAluno(IEnumerable<TotalAlunosAnoTurmaDto> totalDeAlunos, string anoTurma)
        {
            if (!(totalDeAlunos is null) && totalDeAlunos.Any())
                return totalDeAlunos.Where(x => x.AnoTurma == anoTurma).Select(x => x.QuantidadeAluno).Sum();

            return 0;
        }

        protected int ObterTotalDeTurmas(IEnumerable<TotalDeTurmasPorAnoDto> totalDeTurmas, string anoTurma)
        {
            if (totalDeTurmas is null || !totalDeTurmas.Any())
                return 0;

            return totalDeTurmas?.FirstOrDefault(t => t.Ano == anoTurma)?.Quantidade ?? 0;
        }

        protected Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagemPortugues(bool ehIAD)
        {
            return ObterPeriodoFixoSondagem(ObterTituloSemestreBimestrePortugues(ehIAD));
        }

        protected Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagemMatematica(bool ehIAD)
        {
            return ObterPeriodoFixoSondagem(ObterTituloSemestreBimestreMatematica(ehIAD));
        }

        protected int ObterTotalDeAlunos(
            IEnumerable<PerguntaRespostaOrdemDto> respostasOrdem,
            string anoTurma,
            IEnumerable<TotalAlunosAnoTurmaDto> totalDeAlunosUe)
        {
            if (EhTodosPreenchidos())
                return ObterTotalDeAlunos(respostasOrdem);

            return ObterTotalAlunosOuTotalRespostas(
                                    respostasOrdem,
                                    ObterTotalDeAluno(totalDeAlunosUe, anoTurma));
        }

        private int ObterTotalAlunosOuTotalRespostas(
                                IEnumerable<PerguntaRespostaOrdemDto> perguntasRespostasUe,
                                int totalAlunos)
        {
            var totalRespostas = ObterTotalDeAlunos(perguntasRespostasUe);

            if (totalAlunos < totalRespostas)
                return totalRespostas;

            return totalAlunos;
        }

        private int ObterTotalDeAlunos(IEnumerable<PerguntaRespostaOrdemDto> respostasOrdem)
        {
            return respostasOrdem.GroupBy(pergunta => new { pergunta.OrdemPergunta, pergunta.SubPerguntaDescricao })
                                 .Select(subpergunta => subpergunta.Sum(x => x.QtdRespostas))
                                 .Max();
        }

        private string ObterDescricaoSemestreBimestre(bool ehSemestre)
        {
            var descricao = ehSemestre ? "Semestre" : "Bimestre";

            return @$"{filtro.Periodo}° {descricao}";
        }
    }
}
