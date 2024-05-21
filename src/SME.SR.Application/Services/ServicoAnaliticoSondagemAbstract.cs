using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
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

        protected Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem(string termo)
        {
            return sondagemAnaliticaRepository.ObterPeriodoFixoSondagem(termo, filtro.AnoLetivo);
        }

        protected Task<IEnumerable<TotalDeTurmasPorAnoDto>> ObterQuantidadeTurmaPorAno(string codigoUe, int anoLetivo, int modalidade = (int)Modalidade.Fundamental)
        {
            return turmaRepository.ObterTotalDeTurmasPorUeAnoLetivoEModalidade(codigoUe, modalidade, anoLetivo);
        }

        protected string ObterTituloSemestreBimestrePortugues(bool ehIAD)
        {
            return ObterDescricaoSemestreBimestre(ehIAD && filtro.AnoLetivo >= 2024);
        }

        protected string ObterTituloSemestreBimestreMatematica(bool ehIAD)
        {
            return ObterDescricaoSemestreBimestre(filtro.AnoLetivo < 2022 || filtro.AnoLetivo > 2022 && ehIAD);
        }

        private string ObterDescricaoSemestreBimestre(bool ehSemestre)
        {
            var descricao = ehSemestre ? "Semestre" : "Bimestre";

            return @$"{filtro.Periodo}° {descricao}";
        }

        protected Task<IEnumerable<TotalAlunosAnoTurmaDto>> ObterTotalDeAlunosAnoTurma(string codigoDre, string codigoUe)
        {
            var modalidades = new List<int> { 5, 13 };

            return alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(
                                                    filtro.AnoLetivo,
                                                    modalidades.ToArray(),
                                                    this.periodoFixoSondagem.DataInicio.Date,
                                                    this.periodoFixoSondagem.DataFim.Date,
                                                    codigoUe,
                                                    codigoDre);
        }
    }
}
