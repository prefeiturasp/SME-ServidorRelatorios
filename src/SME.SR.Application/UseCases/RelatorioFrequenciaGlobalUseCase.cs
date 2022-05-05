using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioFrequenciaGlobalUseCase : IRelatorioFrequenciaGlobalUseCase
    {
        public delegate Task OpcaoRelatorio(List<FrequenciaGlobalDto> listaDeFrequencia, Guid codigoCorrelacao);

        public readonly IMediator mediator;

        public RelatorioFrequenciaGlobalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroFrequenciaGlobalDto>();
            var listaDeFrenquenciaGlobal = await mediator.Send(new ObterRelatorioDeFrequenciaGlobalQuery(filtroRelatorio));
            if (listaDeFrenquenciaGlobal != null && listaDeFrenquenciaGlobal.Any())
            {
                switch (filtroRelatorio.TipoFormatoRelatorio)
                {
                    case TipoFormatoRelatorio.Pdf:
                        await GerarRelatorioPdf(listaDeFrenquenciaGlobal, request, filtroRelatorio);
                        break;
                    case TipoFormatoRelatorio.Xlsx:
                        await ExecuteExcel(listaDeFrenquenciaGlobal, request.CodigoCorrelacao);
                        break;
                    default:
                        throw new NegocioException($"Não foi possível exportar este relátorio para o formato {filtroRelatorio.TipoFormatoRelatorio}");
                }
            }
            else throw new NegocioException("Não foi possível localizar informações com os filtros selecionados");
        }

        private async Task GerarRelatorioPdf(List<FrequenciaGlobalDto> listaDeFrequencia, FiltroRelatorioDto request, FiltroFrequenciaGlobalDto filtroRelatorio)
        {
            var dto = new FrequenciaMensalDto();
            await MapearCabecalho(dto, filtroRelatorio);
            MapearFrequenciaMensalDre(listaDeFrequencia.GroupBy(x => x.DreCodigo).Distinct().ToList(), dto);
            MapearFrequenciaMensalUe(listaDeFrequencia.GroupBy(x => x.UeCodigo).Distinct().ToList(), dto.Dres);
            MapearFrequenciaMensalUeTurma(listaDeFrequencia.GroupBy(x => x.TurmaCodigo).Distinct().ToList(), dto.Dres);
            MapearFrequenciaMensalUeTurmaMes(listaDeFrequencia.GroupBy(x => x.TurmaCodigo).Distinct().ToList(),dto.Dres);
            MapearFrequenciaUesTurmaMesEstudande(listaDeFrequencia.GroupBy(x => x.CodigoEOL).Distinct().ToList(), dto.Dres);
            
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioFrequenciaGlobal", dto, request.CodigoCorrelacao));

        }

        private void MapearFrequenciaUesTurmaMesEstudande(List<IGrouping<string, FrequenciaGlobalDto>> listaDeFrequenciaUesTurmaMesEstudande, List<FrequenciaMensalDreDto> frequenciaMensalDre)
        {
            var listaEstudantes = new List<FrequenciaMensalAlunoDto>();

            foreach (var itemEstudante in listaDeFrequenciaUesTurmaMesEstudande)
            {
                var estudante = new FrequenciaMensalAlunoDto 
                {
                    CodigoAluno = itemEstudante.Key,
                    NumeroAluno = itemEstudante.FirstOrDefault().NumeroChamadda,
                    NomeAluno = itemEstudante.FirstOrDefault().Estudante,
                    ProcentagemFrequencia = itemEstudante.FirstOrDefault().PercentualFrequencia,
                    ValorMes = itemEstudante.FirstOrDefault().Mes,
                    CodigoUe = itemEstudante.FirstOrDefault().UeCodigo,
                    TurmaCodigo = itemEstudante.FirstOrDefault().TurmaCodigo
                };
                listaEstudantes.Add(estudante);
            }
            foreach (var itemDre in frequenciaMensalDre)
            {
                foreach (var itemUe in itemDre.Ues)
                {
                    foreach (var itemTurma in itemUe.Turmas)
                    {
                        foreach (var itemMes in itemTurma.Meses)
                        {
                            itemMes.Alunos.AddRange(listaEstudantes.Where(x => x.TurmaCodigo == itemMes.TurmaCodigo &&
                                                                                         x.ValorMes == itemMes.ValorMes && x.CodigoUe == itemTurma.CodigoUe).OrderBy(x => x.NomeAluno));
                        }
                    }
                }
            }
        }

        private void MapearFrequenciaMensalUeTurmaMes(List<IGrouping<string, FrequenciaGlobalDto>> listaDeFrequenciaUesTurmaMes, List<FrequenciaMensalDreDto> frequenciaMensalDre)
        {
            var listaMes = new List<FrequenciaMensalMesDto>();
            foreach (var itemMes in listaDeFrequenciaUesTurmaMes)
            {
                var freqMes = new FrequenciaMensalMesDto
                {
                    NomeMes = ObterNomeMesReferencia(itemMes.FirstOrDefault().Mes),
                    ValorMes = itemMes.FirstOrDefault().Mes,
                    TurmaCodigo = itemMes.FirstOrDefault().TurmaCodigo
                };
                listaMes.Add(freqMes);
            }

            foreach (var itemDre in frequenciaMensalDre)
            {
                foreach (var itemUe in itemDre.Ues)
                {
                    foreach (var itemTurma in itemUe.Turmas)
                    {
                        itemTurma.Meses.AddRange(listaMes.Where(x => x.TurmaCodigo == itemTurma.CodigoTurma).OrderBy(x => x.ValorMes));
                    }
                }
            }
        }

        private void MapearFrequenciaMensalUeTurma(List<IGrouping<string, FrequenciaGlobalDto>> listaDeFrequenciaUesTurma, List<FrequenciaMensalDreDto> frequenciaMensalDre)
        {
            var listaTurmas = new List<FrequenciaMensalTurmaDto>();
            foreach (var turma in listaDeFrequenciaUesTurma)
            {
                var freqTurma = new FrequenciaMensalTurmaDto
                {
                    NomeTurma = turma.FirstOrDefault().Turma,
                    CodigoTurma = turma.Key,
                    CodigoUe = turma.FirstOrDefault().UeCodigo,
                };
                listaTurmas.Add(freqTurma);
            }
            foreach (var itemDre in frequenciaMensalDre)
            {
                foreach (var itemUe in itemDre.Ues)
                {
                    itemUe.Turmas.AddRange(listaTurmas.Where(x => x.CodigoUe == itemUe.CodigoUe).OrderBy(x => x.CodigoTurma));
                }
            }
        }

        private async Task MapearCabecalho(FrequenciaMensalDto dto, FiltroFrequenciaGlobalDto filtroRelatorio)
        {
            dto.Cabecalho.NomeTurma = await ObterNomeTurma(filtroRelatorio);

            await ObterNomeDreUe(filtroRelatorio.CodigoDre, filtroRelatorio.CodigoUe, dto);
            dto.Cabecalho.AnoLetivo = filtroRelatorio.AnoLetivo;
            dto.Cabecalho.NomeModalidade = filtroRelatorio.Modalidade.Name();
            dto.Cabecalho.RfUsuarioSolicitante = filtroRelatorio.UsuarioRf;
            dto.Cabecalho.UsuarioSolicitante = filtroRelatorio.UsuarioNome;

            if (filtroRelatorio.MesesReferencias.Count() == 1 && !filtroRelatorio.MesesReferencias.Contains("-99"))
                dto.Cabecalho.MesReferencia = ObterNomeMesReferencia(int.Parse(filtroRelatorio.MesesReferencias.FirstOrDefault()));
            else if (filtroRelatorio.MesesReferencias.Count() == 1 && filtroRelatorio.MesesReferencias.Contains("-99"))
                dto.Cabecalho.MesReferencia = "Todos";
        }
        private void MapearFrequenciaMensalDre(List<IGrouping<string, FrequenciaGlobalDto>> listadres, FrequenciaMensalDto dto)
        {
            foreach (var itemDre in listadres.OrderBy(x => x.Key))
            {
                var freqDre = new FrequenciaMensalDreDto
                {
                    CodigoDre = itemDre.Key,
                    NomeDre = itemDre.FirstOrDefault().SiglaDre,
                };

                dto.Dres.Add(freqDre);
            }
        }

        private void MapearFrequenciaMensalUe(List<IGrouping<string, FrequenciaGlobalDto>> listaDeFrequenciaUes, List<FrequenciaMensalDreDto> frequenciaMensalDre)
        {
            var listaUes = new List<FrequenciaMensalUeDto>();
            foreach (var item in listaDeFrequenciaUes)
            {
                var freqUe = new FrequenciaMensalUeDto 
                {
                    CodigoUe = item.Key,
                    CodigoDre = item.FirstOrDefault().DreCodigo,
                    NomeUe = item.FirstOrDefault().UeNome
                };
                listaUes.Add(freqUe);
            }
            foreach (var item in frequenciaMensalDre)
                item.Ues.AddRange(listaUes.Where(x => x.CodigoDre == item.CodigoDre).OrderBy(x => x.NomeUe));
        }

        private async Task<string> ObterNomeTurma(FiltroFrequenciaGlobalDto filtroRelatorio)
        {
            var nomeTurma = String.Empty;
            if (filtroRelatorio.CodigosTurmas.Count() == 1 && !filtroRelatorio.CodigosTurmas.Contains("-99"))
            {
                var turma = await mediator.Send(new ObterTurmaPorCodigoQuery(filtroRelatorio.CodigosTurmas.FirstOrDefault()));
                nomeTurma = turma.NomePorFiltroModalidade(filtroRelatorio.Modalidade);
            }
            else if (filtroRelatorio.CodigosTurmas.Contains("-99"))
                nomeTurma = "Todas";

            return nomeTurma;
        }
        private async Task ObterNomeDreUe(string dreCodigo, string ueCodigo, FrequenciaMensalDto dto)
        {
            if (!dreCodigo.Contains("-99") && !ueCodigo.Contains("-99"))
            {
                var dadosDreUe = await mediator.Send(new ObterDreUePorDreCodigoQuery(dreCodigo, ueCodigo));
                dto.Cabecalho.NomeUe = dadosDreUe.UeNome;
                dto.Cabecalho.NomeDre = dadosDreUe.DreNome;
            }
            else if (!dreCodigo.Contains("-99") && ueCodigo.Contains("-99"))
            {
                var dadosDre = await mediator.Send(new ObterDrePorCodigoQuery(dreCodigo));
                dto.Cabecalho.NomeDre = dadosDre.Nome;
                dto.Cabecalho.NomeUe = "Todas";
            }
            else if (dreCodigo.Contains("-99") && ueCodigo.Contains("-99"))
            {
                dto.Cabecalho.NomeUe = "Todas";
                dto.Cabecalho.NomeDre = "Todas";
            }
        }
        private async Task ExecuteExcel(List<FrequenciaGlobalDto> listaDeFrequencia, Guid codigoCorrelacao)
        {
            await mediator.Send(new GerarExcelGenericoCommand(listaDeFrequencia.Cast<object>().ToList(), "Frequência Global", codigoCorrelacao));
        }
        private string ObterNomeMesReferencia(int mes)
            => Enum.GetValues(typeof(Mes)).Cast<Mes>().Where(x => (int)x == mes).FirstOrDefault().ToString();
    }
}
