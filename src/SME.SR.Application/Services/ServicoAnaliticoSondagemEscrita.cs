using Npgsql;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Data.Models;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Sondagem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Application.Services
{
    public class ServicoAnaliticoSondagemEscrita : ServicoAnaliticoSondagemAbstract, IServicoAnaliticoSondagemEscrita
    {
        public ServicoAnaliticoSondagemEscrita(IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository, ISondagemRelatorioRepository sondagemRelatorioRepository, ISondagemAnaliticaRepository sondagemAnaliticaRepository, ITurmaRepository turmaRepository) : base(alunoRepository, dreRepository, ueRepository, sondagemRelatorioRepository, sondagemAnaliticaRepository, turmaRepository)
        {
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorio(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;
            periodoFixoSondagem = await ObterPeriodoFixoSondagemPortugues(false);

            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var respostasEscrita = await sondagemAnaliticaRepository.ObterRespostaRelatorioAnaliticoDeEscrita(filtro, EhTodosPreenchidos());
            var agrupamentoPorDre = respostasEscrita.Where(x => x.DreCodigo != null).GroupBy(x => x.DreCodigo).Distinct().ToList();
            
            if (agrupamentoPorDre.Any())
            {
                var dres = await ObterDres(agrupamentoPorDre.Select(x => x.Key).ToArray());

                foreach (var itemDre in agrupamentoPorDre)
                {
                    var relatorioSondagemAnaliticoEscritaDto = new RelatorioSondagemAnaliticoEscritaDto();
                    var agrupamentoPorUe = itemDre.GroupBy(x => x.UeCodigo).Distinct().ToList();
                    var dre = dres.FirstOrDefault(x => x.Codigo == itemDre.Key);

                    relatorioSondagemAnaliticoEscritaDto.Respostas.AddRange(await ObterRespostas(agrupamentoPorUe, dre));

                    relatorioSondagemAnaliticoEscritaDto.Dre = dre.Nome;
                    relatorioSondagemAnaliticoEscritaDto.DreSigla = dre.Abreviacao;
                    relatorioSondagemAnaliticoEscritaDto.AnoLetivo = filtro.AnoLetivo;
                    relatorioSondagemAnaliticoEscritaDto.Periodo = filtro.Periodo;
                    retorno.Add(relatorioSondagemAnaliticoEscritaDto);
                }
            }

            return retorno;
        }

        protected override bool EhTodosPreenchidos()
        {
            return EhPreenchimentoDeTodosEstudantes();
        }

        private async Task<List<RespostaSondagemAnaliticoEscritaDto>> ObterRespostas(
                                                                    List<IGrouping<string, TotalRespostasAnaliticoEscritaDto>> agrupadoRespostasPorUe,
                                                                    Dre dre)
        {
            var ues = await ObterUe(agrupadoRespostasPorUe.Select(x => x.Key).ToArray());
            var totalDeTurmas = await ObterQuantidadeTurmaPorAnoDre(dre.Id);
            var respostas = new List<RespostaSondagemAnaliticoEscritaDto>();
            var totalDeAlunosPorAno = await ObterTotalDeAlunosPorDre(dre.Codigo);

            foreach (var itemUe in agrupadoRespostasPorUe)
            {
                var agrupamentoPorAnoTurma = itemUe.OrderBy(x => x.AnoTurma).GroupBy(x => x.AnoTurma);
                var totalDeAlunosUe = await ObterTotalDeAlunosPorUe(dre.Codigo, itemUe.Key, totalDeAlunosPorAno);
                var totalTurmasUe = await ObterQuantidadeTurmaPorAnoUe(dre.Id, itemUe.Key, totalDeTurmas);

                foreach (var anoTurma in agrupamentoPorAnoTurma)
                {
                    var ue = ues.FirstOrDefault(x => x.Codigo == itemUe.Key);

                    respostas.Add(ObterRespostaEscrita(anoTurma, ue, totalTurmasUe, ObterTotalDeAluno(totalDeAlunosUe, anoTurma.Key)));
                }
            }

            return respostas;
        }

        private RespostaSondagemAnaliticoEscritaDto ObterRespostaEscrita(
                                                        IGrouping<string, TotalRespostasAnaliticoEscritaDto> respostaAnoTurma,
                                                        Ue ue,
                                                        IEnumerable<TotalDeTurmasPorAnoDto> totalTurmaUe,
                                                        int totalDeAlunos)
        {
            return new RespostaSondagemAnaliticoEscritaDto
            {
                PreSilabico = respostaAnoTurma.Select(x => x.PreSilabico).Sum(),
                SilabicoSemValor = respostaAnoTurma.Select(x => x.SilabicoSemValor).Sum(),
                SilabicoComValor = respostaAnoTurma.Select(x => x.SilabicoComValor).Sum(),
                SilabicoAlfabetico = respostaAnoTurma.Select(x => x.SilabicoAlfabetico).Sum(),
                Nivel1 = respostaAnoTurma.Select(x => x.Nivel1).Sum(),
                Nivel2 = respostaAnoTurma.Select(x => x.Nivel2).Sum(),
                Nivel3 = respostaAnoTurma.Select(x => x.Nivel3).Sum(),
                Nivel4 = respostaAnoTurma.Select(x => x.Nivel4).Sum(),
                Alfabetico = respostaAnoTurma.Select(x => x.Alfabetico).Sum(),
                SemPreenchimento = ObterTotalSemPreenchimento(respostaAnoTurma, totalDeAlunos),
                TotalDeAlunos = ObterTotalDeAlunos(respostaAnoTurma, totalDeAlunos),
                Ano = int.Parse(respostaAnoTurma.Key),
                TotalDeTurma = ObterTotalDeTurmas(totalTurmaUe, respostaAnoTurma.Key),
                Ue = ue.TituloTipoEscolaNome
            };
        }

        private int ObterTotalDeAlunos(
                            IGrouping<string, TotalRespostasAnaliticoEscritaDto> respostaAnoTurma,
                            int totalDeAlunos)
        {
            if (totalDeAlunos == 0)
                return ObterTotalDeAlunos(respostaAnoTurma);

            return totalDeAlunos;
        }

        private int ObterTotalDeAlunos(IGrouping<string, TotalRespostasAnaliticoEscritaDto> respostaAnoTurma)
        {
            return ObterTotalDeRespostaAtual(respostaAnoTurma) + ObterTotalSemPreenchimento(respostaAnoTurma);
        }

        private int ObterTotalSemPreenchimento(IGrouping<string, TotalRespostasAnaliticoEscritaDto> respostaAnoTurma)
        {
            return respostaAnoTurma.Select(x => x.SemPreenchimento).Sum();
        }

        private int ObterTotalSemPreenchimento(
                            IGrouping<string, TotalRespostasAnaliticoEscritaDto> respostaAnoTurma,
                            int totalDeAlunos)
        {
            if (EhTodosPreenchidos())
                return ObterTotalSemPreenchimento(respostaAnoTurma);

            return ObterTotalSemPreenchimentoCalculado(respostaAnoTurma, totalDeAlunos);
        }

        private int ObterTotalSemPreenchimentoCalculado(
                            IGrouping<string, TotalRespostasAnaliticoEscritaDto> respostaAnoTurma,
                            int totalDeAlunos)
        {
            var totalRepostas = EhTerceiroAnoPrimeiroPeriodoAteDoisMilEVinteTres(respostaAnoTurma) ?
                            ObterTotalRespostaSoNivel(respostaAnoTurma) :
                            ObterTotalDeRespostaAtual(respostaAnoTurma);

            return totalDeAlunos <= totalRepostas ? 0 : totalDeAlunos - totalRepostas;
        }

        private bool EhTerceiroAnoPrimeiroPeriodoAteDoisMilEVinteTres(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma)
        {
            return (anoTurma.Key == TURMA_TERCEIRO_ANO && (filtro.AnoLetivo <= ANO_ESCOLAR_2023 && filtro.Periodo == PRIMEIRO_BIMESTRE));
        }

        private int ObterTotalDeRespostaAtual(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma)
        {
            var totalRepostas = anoTurma.Select(x => x.PreSilabico).Sum() + 
                anoTurma.Select(x => x.SilabicoSemValor).Sum() +
                anoTurma.Select(x => x.SilabicoComValor).Sum() +
                anoTurma.Select(x => x.SilabicoAlfabetico).Sum() +
                anoTurma.Select(x => x.Alfabetico).Sum();

            if (anoTurma.Key.Equals(TURMA_TERCEIRO_ANO))
                totalRepostas += ObterTotalRespostaSoNivel(anoTurma);

            return totalRepostas;
        }

        private int ObterTotalRespostaSoNivel(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma)
        {
            return anoTurma.Select(x => x.Nivel1).Sum() + 
                anoTurma.Select(x => x.Nivel2).Sum() + 
                anoTurma.Select(x => x.Nivel3).Sum() +
                anoTurma.Select(x => x.Nivel4).Sum();
        }
    }
}
