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
        private const int TERCEIRO_BIMESTRE = 3;
        private const int QUARTO_BIMESTRE = 4;
        private const int SEGUNDO_SEMESTRE = 2;
        private const int ANO_LETIVO_2024 = 2024;
        private const int ANO_LETIVO_2025 = 2025;

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

        protected Task<IEnumerable<TotalDeTurmasPorAnoDto>> ObterQuantidadeTurmaPorAno(long dreId, int anoLetivo, int modalidade = (int)Modalidade.Fundamental)
        {
            return turmaRepository.ObterTotalDeTurmasPorUeAnoLetivoEModalidade(dreId, modalidade, anoLetivo);
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

        private string ObterDescricaoSemestreBimestre(bool ehSemestre)
        {
            var descricao = ehSemestre ? "Semestre" : "Bimestre";

            return @$"{filtro.Periodo}° {descricao}";
        }

        protected Task<IEnumerable<TotalAlunosAnoTurmaDto>> ObterTotalDeAlunosAnoTurma(string codigoDre)
        {
            var modalidades = new List<int> { 5, 13 };
            if (EhTodosPreenchidos())
                return Task.FromResult(Enumerable.Empty<TotalAlunosAnoTurmaDto>());

            return alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(
                                                    filtro.AnoLetivo,
                                                    modalidades.ToArray(),
                                                    this.periodoFixoSondagem.DataInicio.Date,
                                                    this.periodoFixoSondagem.DataFim.Date,
                                                    codigoDre);
        }

        protected bool ComPreenchimentoDeTodosEstudantesIAD()
        {
            return filtro.AnoLetivo == ANO_LETIVO_2024 && filtro.Periodo == SEGUNDO_SEMESTRE || filtro.AnoLetivo >= ANO_LETIVO_2025;
        }

        protected bool ComPreenchimentoDeTodosEstudantes()
        {
            var bimestres = new int[] { TERCEIRO_BIMESTRE, QUARTO_BIMESTRE };

            return filtro.AnoLetivo == ANO_LETIVO_2024 && bimestres.Contains(filtro.Periodo) || filtro.AnoLetivo >= ANO_LETIVO_2025;
        }

        protected IEnumerable<TotalAlunosAnoTurmaDto> ObterTotalDeAlunosPorUe(IEnumerable<TotalAlunosAnoTurmaDto> totalDeAlunos, string codigoUe)
        {
            if (!(totalDeAlunos is null) || totalDeAlunos.Any())
                return totalDeAlunos.Where(x => x.CodigoUe == codigoUe);

            return Enumerable.Empty<TotalAlunosAnoTurmaDto>();
        }

        protected int ObterTotalDeAluno(IEnumerable<TotalAlunosAnoTurmaDto> totalDeAlunos, string anoTurma)
        {
            if (!(totalDeAlunos is null) || totalDeAlunos.Any())
                return totalDeAlunos.Where(x => x.AnoTurma == anoTurma).Select(x => x.QuantidadeAluno).Sum();

            return 0;
        }
    }
}
