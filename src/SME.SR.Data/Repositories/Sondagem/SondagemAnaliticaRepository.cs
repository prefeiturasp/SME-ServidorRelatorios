using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using SME.SR.Data.Interfaces.Sondagem;
using SME.SR.Data.Models;
using SME.SR.Infra.Dtos.Sondagem;
using SME.SR.Infra.Utilitarios;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Collections;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Ocsp;

namespace SME.SR.Data
{
    public class SondagemAnaliticaRepository : ISondagemAnaliticaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        private readonly IAlunoRepository alunoRepository;
        private readonly IDreRepository dreRepository;
        private readonly IUeRepository ueRepository;
        private readonly ISondagemRelatorioRepository sondagemRelatorioRepository;
        private readonly ITurmaRepository turmaRepository;

        private readonly string TURMA_TERCEIRO_ANO = "3";
        private readonly string DESCRICAO_SEMPREENCHIMENTO = "Sem Preenchimento";
        private readonly int ANO_ESCOLAR_2022 = 2022;

        public SondagemAnaliticaRepository(VariaveisAmbiente variaveisAmbiente, IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository
            , ISondagemRelatorioRepository sondagemRelatorioRepository, ITurmaRepository turmaRepository)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
            this.sondagemRelatorioRepository = sondagemRelatorioRepository ?? throw new ArgumentNullException(nameof(sondagemRelatorioRepository));
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoCapacidadeDeLeitura(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();

            var modalidades = new List<int> { 5, 13 };

            var consultaDados = await sondagemRelatorioRepository.ConsolidadoCapacidadeLeitura(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.CAPACIDADE_DE_LEITURA,
                PeriodoId = periodo.Id
            });
            var agrupadoPorDre = consultaDados.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre).Distinct().ToList();
            if (agrupadoPorDre.Any())
            {
                var listaDres = await dreRepository.ObterPorCodigos(agrupadoPorDre.Select(x => x.Key).ToArray());
                foreach (var itemDre in agrupadoPorDre)
                {
                    var perguntas = new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto();
                    var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).Distinct().ToList();
                    var listaUes = await ueRepository.ObterPorCodigos(agrupadoPorUe.Select(x => x.Key).ToArray());
                    foreach (var itemUe in agrupadoPorUe)
                    {
                        var turmas = await ObterQuantidadeTurmaPorAno(filtro, itemUe.Key);
                        var relatorioAgrupadoPorAno = itemUe.Where(x => x.AnoTurma != null).GroupBy(p => p.AnoTurma).ToList();
                        var totalDeAlunosPorAno = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, itemUe.Key, itemDre.Key)).ToList();

                        foreach (var anoTurmaItem in relatorioAgrupadoPorAno)
                        {
                            var listaDtoOrdemNarrar = new List<RespostaCapacidadeDeLeituraDto>();
                            var listaDtoOrdemRelatar = new List<RespostaCapacidadeDeLeituraDto>();
                            var listaDtoOrdemArgumentar = new List<RespostaCapacidadeDeLeituraDto>();
                            var respostaSondagemAnaliticoCapacidadeDeLeituraDtoLista = new List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>();

                            var agrupamentoPorOrderm = anoTurmaItem.GroupBy(x => x.Ordem);


                            var ordemNarrarLista = agrupamentoPorOrderm.Where(x => x.Key == OrdemSondagem.ORDEM_DO_NARRAR).ToList();
                            var ordemRelatarLista = agrupamentoPorOrderm.Where(x => x.Key == OrdemSondagem.ORDEM_DO_RELATAR).ToList();
                            var ordemArgumentarLista = agrupamentoPorOrderm.Where(x => x.Key == OrdemSondagem.ORDEM_DO_ARGUMENTAR).ToList();

                            var totalDeAlunos = totalDeAlunosPorAno.Where(x => x.AnoTurma == anoTurmaItem.Key).Select(x => x.QuantidadeAluno).Sum();



                            if (ordemNarrarLista.Count() == 0)
                            {
                                var ordemNarrar = new RespostaCapacidadeDeLeituraDto();
                                ordemNarrar.Localizacao.SemPreenchimento = totalDeAlunos;
                                ordemNarrar.Reflexao.SemPreenchimento = totalDeAlunos;
                                ordemNarrar.Inferencia.SemPreenchimento = totalDeAlunos;
                                listaDtoOrdemNarrar.Add(ordemNarrar);
                            }
                            foreach (var ordemNarrarItem in ordemNarrarLista)
                            {
                                var ordemNarrar = new RespostaCapacidadeDeLeituraDto
                                {
                                    Localizacao = MapearOrdemDoNarrarLocalizacao(ordemNarrarItem, totalDeAlunos, ordemNarrarLista),
                                    Inferencia = MapearOrdemDoNarrarInferencia(ordemNarrarItem, totalDeAlunos, ordemNarrarLista),
                                    Reflexao = MapearOrdemDoNarrarReflexao(ordemNarrarItem, totalDeAlunos, ordemNarrarLista),
                                };
                                listaDtoOrdemNarrar.Add(ordemNarrar);
                            }

                            if (ordemRelatarLista.Count() == 0)
                            {
                                var ordemRelatarItem = new RespostaCapacidadeDeLeituraDto();
                                ordemRelatarItem.Localizacao.SemPreenchimento = totalDeAlunos;
                                ordemRelatarItem.Reflexao.SemPreenchimento = totalDeAlunos;
                                ordemRelatarItem.Inferencia.SemPreenchimento = totalDeAlunos;
                                listaDtoOrdemRelatar.Add(ordemRelatarItem);
                            }

                            foreach (var ordemRelatarItem in ordemRelatarLista)
                            {
                                var ordemDoRelatar = new RespostaCapacidadeDeLeituraDto
                                {
                                    Localizacao = MapearOrdemDoNarrarLocalizacao(ordemRelatarItem, totalDeAlunos, ordemRelatarLista),
                                    Inferencia = MapearOrdemDoNarrarInferencia(ordemRelatarItem, totalDeAlunos, ordemRelatarLista),
                                    Reflexao = MapearOrdemDoNarrarReflexao(ordemRelatarItem, totalDeAlunos, ordemRelatarLista),
                                };
                                listaDtoOrdemRelatar.Add(ordemDoRelatar);
                            }

                            if (ordemArgumentarLista.Count() == 0)
                            {
                                var ordemArgumentarItem = new RespostaCapacidadeDeLeituraDto();
                                ordemArgumentarItem.Localizacao.SemPreenchimento = totalDeAlunos;
                                ordemArgumentarItem.Reflexao.SemPreenchimento = totalDeAlunos;
                                ordemArgumentarItem.Inferencia.SemPreenchimento = totalDeAlunos;
                                listaDtoOrdemArgumentar.Add(ordemArgumentarItem);
                            }

                            foreach (var ordemArgumentarItem in ordemArgumentarLista)
                            {
                                var ordemDoArgumentar = new RespostaCapacidadeDeLeituraDto
                                {
                                    Localizacao = MapearOrdemDoNarrarLocalizacao(ordemArgumentarItem, totalDeAlunos, ordemArgumentarLista),
                                    Inferencia = MapearOrdemDoNarrarInferencia(ordemArgumentarItem, totalDeAlunos, ordemArgumentarLista),
                                    Reflexao = MapearOrdemDoNarrarReflexao(ordemArgumentarItem, totalDeAlunos, ordemArgumentarLista),
                                };
                                listaDtoOrdemArgumentar.Add(ordemDoArgumentar);
                            }

                            foreach (var listaDtoOrdemNarraritem in listaDtoOrdemNarrar)
                            {
                                var resp = new RespostaSondagemAnaliticoCapacidadeDeLeituraDto();
                                resp.OrdemDoNarrar = listaDtoOrdemNarraritem;

                                foreach (var listaDtoOrdemRelatarItem in listaDtoOrdemRelatar)
                                {
                                    resp.OrdemDoRelatar = listaDtoOrdemRelatarItem;
                                    foreach (var listaDtoOrdemArgumentaritem in listaDtoOrdemArgumentar)
                                    {
                                        resp.OrdemDoArgumentar = listaDtoOrdemArgumentaritem;
                                    }
                                }

                                resp.Ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key).Nome;
                                resp.Ano = int.Parse(anoTurmaItem.Key);
                                resp.TotalDeTurma = turmas.Count(x => x.AnoTurma == anoTurmaItem.Key);
                                resp.TotalDeAlunos = totalDeAlunos;
                                respostaSondagemAnaliticoCapacidadeDeLeituraDtoLista.Add(resp);
                            }

                            foreach (var respost in respostaSondagemAnaliticoCapacidadeDeLeituraDtoLista)
                            {
                                perguntas.Respostas.Add(respost);
                            }
                        }
                    }

                    var dre = listaDres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    perguntas.Dre = dre.Nome;
                    perguntas.DreSigla = dre.Abreviacao;
                    perguntas.AnoLetivo = filtro.AnoLetivo;
                    perguntas.Periodo = filtro.Periodo;
                    retorno.Add(perguntas);
                }
            }

            return retorno;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoEscrita(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> { 5, 13 };

            var sql = ConsultaEscritaLinguaPortuguesaPrimeiroAoTerceiroAno(filtro);
            var parametros = new { dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma };
            IEnumerable<TotalRespostasAnaliticoEscritaDto> dtoConsulta = null;
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                dtoConsulta = (await conexao.QueryAsync<TotalRespostasAnaliticoEscritaDto>(sql, parametros)).ToList();
            }
            var anoComValorSemPreenchimento = dtoConsulta.Select(s => new { Ano = s.AnoTurma, Valor = s.SemPreenchimento }).ToList();
            var agrupamentoPorDre = dtoConsulta.Where(x => x.DreCodigo != null).GroupBy(x => x.DreCodigo).Distinct().ToList();
            if (agrupamentoPorDre.Any())
            {
                var dreLista = (await dreRepository.ObterPorCodigos(agrupamentoPorDre.Select(x => x.Key).ToArray())).ToList();

                foreach (var itemDre in agrupamentoPorDre)
                {
                    var relatorioSondagemAnaliticoEscritaDto = new RelatorioSondagemAnaliticoEscritaDto();
                    var agrupamentoPorUe = itemDre.GroupBy(x => x.UeCodigo).Distinct().ToList();
                    var ueLista = (await ueRepository.ObterPorCodigos(agrupamentoPorUe.Select(x => x.Key).ToArray())).ToList();
                    foreach (var itemUe in agrupamentoPorUe)
                    {
                        var turmas = await ObterQuantidadeTurmaPorAno(filtro, itemUe.Key);
                        var agrupamentoPorAnoTurma = itemUe.GroupBy(x => x.AnoTurma).ToList();
                        var quantidadeTotalAlunosPorAno = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, itemUe.Key, itemDre.Key)).ToList();

                        foreach (var anoTurma in agrupamentoPorAnoTurma)
                        {

                            var quantidadeTotalAlunosEol = 0;
                            var turmasComSondagem = anoTurma.Select(x => x.TurmaCodigo).ToList();

                            if (turmasComSondagem.Any())
                                quantidadeTotalAlunosEol = await ObterTotalAlunosAtivosPorTurmaEPeriodo(turmasComSondagem, periodoFixo.DataFim);
                            else
                                quantidadeTotalAlunosEol = quantidadeTotalAlunosPorAno.Where(x => x.AnoTurma == anoTurma.Key).Select(x => x.QuantidadeAluno).Sum();


                            var totalSemPreenchimento = anoTurma.Key == TURMA_TERCEIRO_ANO
                                    ? TotalSemPreenchimentoTerceiroAnoEscrita(anoTurma, quantidadeTotalAlunosEol)
                                    : TotalSemPreenchimentoPrimeiroSegundoAnoEscrita(anoTurma, quantidadeTotalAlunosEol);



                            int valorSemPreenchimento = anoComValorSemPreenchimento.Where(x => x.Ano == anoTurma.Key).Select(x => x.Valor).Sum();

                            var semPreenchimentoRelatorio = valorSemPreenchimento > 0 ? (totalSemPreenchimento >= 0 ? totalSemPreenchimento : anoTurma.Select(x => x.SemPreenchimento).Sum()) : 0;

                            var totalDeAlunosNaSondagem = anoTurma.Key == TURMA_TERCEIRO_ANO ? TotalAlunosEscritaTerceiroAno(anoTurma, totalSemPreenchimento, semPreenchimentoRelatorio)
                                                                                                 : TotalAlunosEscritaPrimeiroSegundoAno(anoTurma, totalSemPreenchimento, anoTurma.Key, semPreenchimentoRelatorio);


                            var respostaSondagemAnaliticoEscritaDto = new RespostaSondagemAnaliticoEscritaDto
                            {
                                PreSilabico = anoTurma.Select(x => x.PreSilabico).Sum(),
                                SilabicoSemValor = anoTurma.Select(x => x.SilabicoSemValor).Sum(),
                                SilabicoComValor = anoTurma.Select(x => x.SilabicoComValor).Sum(),
                                SilabicoAlfabetico = anoTurma.Select(x => x.SilabicoAlfabetico).Sum(),
                                Nivel1 = anoTurma.Select(x => x.Nivel1).Sum(),
                                Nivel2 = anoTurma.Select(x => x.Nivel2).Sum(),
                                Nivel3 = anoTurma.Select(x => x.Nivel3).Sum(),
                                Nivel4 = anoTurma.Select(x => x.Nivel4).Sum(),
                                Alfabetico = anoTurma.Select(x => x.Alfabetico).Sum(),
                                SemPreenchimento = semPreenchimentoRelatorio,
                                TotalDeAlunos = totalDeAlunosNaSondagem,
                                Ano = int.Parse(anoTurma.Key),
                                TotalDeTurma = turmas.Count(x => x.AnoTurma == anoTurma.Key),
                                Ue = ueLista.FirstOrDefault(x => x.Codigo == itemUe.Key).Nome
                            };
                            relatorioSondagemAnaliticoEscritaDto.Respostas.Add(respostaSondagemAnaliticoEscritaDto);
                        }
                    }

                    var dre = dreLista.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    relatorioSondagemAnaliticoEscritaDto.Dre = dre.Nome;
                    relatorioSondagemAnaliticoEscritaDto.DreSigla = dre.Abreviacao;
                    relatorioSondagemAnaliticoEscritaDto.AnoLetivo = filtro.AnoLetivo;
                    relatorioSondagemAnaliticoEscritaDto.Periodo = filtro.Periodo;
                    retorno.Add(relatorioSondagemAnaliticoEscritaDto);
                }
            }

            return retorno;
        }

        private static int TotalSemPreenchimentoPrimeiroSegundoAnoEscrita(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int quantidadeTotalAlunosEol)
        {
            return (quantidadeTotalAlunosEol - (anoTurma.Select(x => x.PreSilabico).Sum() + anoTurma.Select(x => x.SilabicoSemValor).Sum()
                                                                                                                  + anoTurma.Select(x => x.SilabicoComValor).Sum()
                                                                                                                  + anoTurma.Select(x => x.SilabicoAlfabetico).Sum()
                                                                                                                  + anoTurma.Select(x => x.Alfabetico).Sum()));
        }

        private static int TotalSemPreenchimentoTerceiroAnoEscrita(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int quantidadeTotalAlunosEol)
        {
            return (quantidadeTotalAlunosEol - (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum()
                                                                        + anoTurma.Select(x => x.Nivel4).Sum()));
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoLeitura(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> { 5, 13 };
            var sql = ConsultaLeituraLinguaPortuguesaPrimeiroAoTerceiroAno(filtro);
            var parametros = new { dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma };
            IEnumerable<TotalRespostasAnaliticoLeituraDto> dtoConsultaDados = null;
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                dtoConsultaDados = (await conexao.QueryAsync<TotalRespostasAnaliticoLeituraDto>(sql, parametros)).ToList();
            }

            var anoComValorSemPreenchimento = dtoConsultaDados.Select(s => new { Ano = s.AnoTurma, Valor = s.SemPreenchimento }).ToList();
            var agrupadoPorDre = dtoConsultaDados.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre).Distinct().ToList();
            if (agrupadoPorDre.Any())
            {
                var listaDres = await dreRepository.ObterPorCodigos(agrupadoPorDre.Select(x => x.Key).ToArray());
                foreach (var itemDre in agrupadoPorDre)
                {
                    var relatorioSondagemAnaliticoLeituraDto = new RelatorioSondagemAnaliticoLeituraDto();
                    var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).Distinct().ToList();
                    var listaUes = await ueRepository.ObterPorCodigos(agrupadoPorUe.Select(x => x.Key).ToArray());
                    foreach (var itemUe in agrupadoPorUe)
                    {
                        var agrupamentoPorAnoTurma = itemUe.GroupBy(x => x.AnoTurma).ToList();
                        var quantidadeTotalAlunosPorAno = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, itemUe.Key, itemDre.Key)).ToList();
                        var turmas = await ObterQuantidadeTurmaPorAno(filtro, itemUe.Key);
                        foreach (var anoTurma in agrupamentoPorAnoTurma)
                        {
                            var quantidadeTotalAlunosEol = 0;
                            var turmasComSondagem = anoTurma.Select(x => x.TurmaCodigo).ToList();

                            if (turmasComSondagem.Any())
                                quantidadeTotalAlunosEol = await ObterTotalAlunosAtivosPorTurmaEPeriodo(turmasComSondagem, periodoFixo.DataFim);
                            else
                                quantidadeTotalAlunosEol = quantidadeTotalAlunosPorAno.Where(x => x.AnoTurma == anoTurma.Key).Select(x => x.QuantidadeAluno).Sum();

                            var semPreenchimento = (quantidadeTotalAlunosEol - (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum()
                                                                                                                  + anoTurma.Select(x => x.Nivel3).Sum() + anoTurma.Select(x => x.Nivel4).Sum()));




                            int valorSemTotalPreenchimento = anoComValorSemPreenchimento.Where(x => x.Ano == anoTurma.Key).Select(x => x.Valor).Sum();

                            var totalDeAlunosNaSondagem = TotaldeAlunosPorAnoLeitura(semPreenchimento, anoTurma, valorSemTotalPreenchimento);

                            var respostaSondagemAnaliticoLeituraDto = new RespostaSondagemAnaliticoLeituraDto
                            {
                                Nivel1 = anoTurma.Select(x => x.Nivel1).Sum(),
                                Nivel2 = anoTurma.Select(x => x.Nivel2).Sum(),
                                Nivel3 = anoTurma.Select(x => x.Nivel3).Sum(),
                                Nivel4 = anoTurma.Select(x => x.Nivel4).Sum(),
                                SemPreenchimento = valorSemTotalPreenchimento > 0 ? (semPreenchimento >= 0 ? semPreenchimento : anoTurma.Select(x => x.SemPreenchimento).Sum()) : 0,
                                TotalDeAlunos = totalDeAlunosNaSondagem,
                                Ano = int.Parse(anoTurma.Key),
                                TotalDeTurma = turmas.Count(x => x.AnoTurma == anoTurma.Key),
                                Ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key).Nome
                            };
                            relatorioSondagemAnaliticoLeituraDto.Respostas.Add(respostaSondagemAnaliticoLeituraDto);
                        }
                    }

                    var dre = listaDres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    relatorioSondagemAnaliticoLeituraDto.Dre = dre.Nome;
                    relatorioSondagemAnaliticoLeituraDto.DreSigla = dre.Abreviacao;
                    relatorioSondagemAnaliticoLeituraDto.AnoLetivo = filtro.AnoLetivo;
                    relatorioSondagemAnaliticoLeituraDto.Periodo = filtro.Periodo;
                    retorno.Add(relatorioSondagemAnaliticoLeituraDto);
                }
            }

            return retorno;
        }

        private int TotaldeAlunosPorAnoLeitura(int semPreenchimento, IGrouping<string, TotalRespostasAnaliticoLeituraDto> anoTurma, int valorSemPreenchimento)
        {

            if (valorSemPreenchimento == 0)
                return (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum() + anoTurma.Select(x => x.Nivel4).Sum());


            return (semPreenchimento + (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum() + anoTurma.Select(x => x.Nivel4).Sum()));

        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoLeituraDeVozAlta(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var modalidades = new List<int> { 5, 13 };
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);


            var consultaDados = (await sondagemRelatorioRepository.ObterDadosProducaoTexto(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.LEITURA_EM_VOZ_ALTA,
                PeriodoId = periodo.Id
            })).ToList();

            var agrupadoPorDre = consultaDados.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre).Distinct().ToList();
            if (agrupadoPorDre.Any())
            {
                var listaDre = await dreRepository.ObterPorCodigos(agrupadoPorDre.Select(x => x.Key).ToArray());

                foreach (var itemDre in agrupadoPorDre)
                {
                    var perguntas = new RelatorioSondagemAnaliticoLeituraDeVozAltaDto();
                    var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).Distinct().ToList();
                    var listaUes = await ueRepository.ObterPorCodigos(agrupadoPorUe.Select(x => x.Key).ToArray());
                    foreach (var itemUe in agrupadoPorUe)
                    {
                        var totalDeAlunosPorAnoTurma = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, itemUe.Key, itemDre.Key)).ToList();
                        var relatorioAgrupadoPorAnoTurma = itemUe.GroupBy(x => x.AnoTurma).ToList();
                        var turmas = await ObterQuantidadeTurmaPorAno(filtro, itemUe.Key);
                        foreach (var anoTurmaItem in relatorioAgrupadoPorAnoTurma)
                        {
                            var alunoNaSondagem = anoTurmaItem.GroupBy(x => x.CodigoAluno);

                            var quantidadeAlunoNaSondagem = alunoNaSondagem.Select(x => x.Key).Count();

                            var totalDeAlunos = totalDeAlunosPorAnoTurma.Where(x => x.AnoTurma == anoTurmaItem.Key).Select(x => x.QuantidadeAluno).Sum();
                            var respostas = new RespostaSondagemAnaliticoLeituraDeVozAltaDto
                            {
                                NaoConseguiuOuNaoQuisLer = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoConseguiuOuNaoQuisLer),
                                LeuComMuitaDificuldade = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComMuitaDificuldade),
                                LeuComAlgumaFluencia = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComAlgumaFluencia),
                                LeuComFluencia = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComFluencia),
                                SemPreenchimento = totalDeAlunos - quantidadeAlunoNaSondagem,
                                Ano = int.Parse(anoTurmaItem.Key),
                                TotalDeTurma = turmas.Count(x => x.AnoTurma == anoTurmaItem.Key),
                                TotalDeAlunos = totalDeAlunos,
                                Ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key).Nome
                            };

                            perguntas.Respostas.Add(respostas);
                        }
                    }

                    var dre = listaDre.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    perguntas.DreSigla = dre.Abreviacao;
                    perguntas.Dre = dre.Nome;
                    perguntas.AnoLetivo = filtro.AnoLetivo;
                    perguntas.Periodo = filtro.Periodo;
                    retorno.Add(perguntas);
                }
            }

            return retorno;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoProducaoDeTexto(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var modalidades = new List<int> { 5, 13 };
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);

            var consultaDados = (await sondagemRelatorioRepository.ObterDadosProducaoTexto(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.PRODUCAO_DE_TEXTO,
                PeriodoId = periodo.Id
            })).ToList();


            var agrupadoPorDre = consultaDados.Where(x => x.CodigoDre != null).GroupBy(x => x.CodigoDre).Distinct().ToList();
            if (agrupadoPorDre.Any())
            {
                var listaDres = await dreRepository.ObterPorCodigos(agrupadoPorDre.Select(x => x.Key).ToArray());
                foreach (var itemDre in agrupadoPorDre)
                {
                    var perguntas = new RelatorioSondagemAnaliticoProducaoDeTextoDto();
                    var agrupadoPorUe = itemDre.GroupBy(x => x.CodigoUe).Distinct().ToList();
                    var listaUes = await ueRepository.ObterPorCodigos(agrupadoPorUe.Select(x => x.Key).ToArray());
                    foreach (var itemUe in agrupadoPorUe)
                    {
                        var totalDeAlunosPorAnoTurma = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, itemUe.Key, itemDre.Key)).ToList();
                        var relatorioAgrupadoPorAnoTurma = itemUe.GroupBy(x => x.AnoTurma).ToList();
                        var turmas = await ObterQuantidadeTurmaPorAno(filtro, itemUe.Key);
                        foreach (var anoTurmaItem in relatorioAgrupadoPorAnoTurma)
                        {
                            var alunoNaSondagem = anoTurmaItem.GroupBy(x => x.CodigoAluno);

                            var quantidadeAlunoNaSondagem = alunoNaSondagem.Select(x => x.Key).Count();

                            var totalDeAlunos = totalDeAlunosPorAnoTurma.Where(x => x.AnoTurma == anoTurmaItem.Key).Select(x => x.QuantidadeAluno).Sum();

                            var respostas = new RespostaSondagemAnaliticoProducaoDeTextoDto()
                            {
                                NaoProduziuEntregouEmBranco = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoProduziuEntregouEmBranco),
                                NaoApresentouDificuldades = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoApresentouDificuldades),
                                EscritaNaoAlfabetica = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.EscritaNaoAlfabetica),
                                DificuldadesComAspectosSemanticos = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosSemanticos),
                                DificuldadesComAspectosTextuais = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosTextuais),
                                DificuldadesComAspectosOrtograficosNotacionais = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosOrtograficosNotacionais),
                                SemPreenchimento = totalDeAlunos - quantidadeAlunoNaSondagem,
                                Ano = int.Parse(anoTurmaItem.Key),
                                TotalDeTurma = turmas.Count(x => x.AnoTurma == anoTurmaItem.Key),
                                TotalDeAlunos = totalDeAlunos,
                                Ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key).Nome
                            };

                            perguntas.Respostas.Add(respostas);
                        }
                    }

                    var dre = listaDres.FirstOrDefault(x => x.Codigo == itemDre.Key);
                    perguntas.DreSigla = dre.Abreviacao;
                    perguntas.Dre = dre.Nome;
                    perguntas.AnoLetivo = filtro.AnoLetivo;
                    perguntas.Periodo = filtro.Periodo;
                    retorno.Add(perguntas);
                }
            }

            return retorno;
        }

        private int TotalAlunosEscritaTerceiroAno(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int totalSemPreenchimento, int valorSemPreenchimento)
        {

            if (valorSemPreenchimento == 0)
            {
                return (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum()
                                               + anoTurma.Select(x => x.Nivel4).Sum());
            }

            return (totalSemPreenchimento + anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum()
                                                            + anoTurma.Select(x => x.Nivel4).Sum());
        }
        private int TotalAlunosEscritaPrimeiroSegundoAno(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int totalSemPreenchimento, string ano, int valorSemPreenchimento)
        {
            if (valorSemPreenchimento == 0)
            {
                return (anoTurma.Select(x => x.PreSilabico).Sum() + anoTurma.Select(x => x.SilabicoSemValor).Sum()
                                                                                                  + anoTurma.Select(x => x.SilabicoComValor).Sum()
                                                                                                  + anoTurma.Select(x => x.SilabicoAlfabetico).Sum()
                                                                                                  + anoTurma.Select(x => x.Alfabetico).Sum());
            }

            return (totalSemPreenchimento + (anoTurma.Select(x => x.PreSilabico).Sum() + anoTurma.Select(x => x.SilabicoSemValor).Sum()
                                                                                  + anoTurma.Select(x => x.SilabicoComValor).Sum()
                                                                                  + anoTurma.Select(x => x.Alfabetico).Sum()
                                                                                  + anoTurma.Select(x => x.SilabicoAlfabetico).Sum()));
        }

        private async Task<List<QuantidadeTurmaPorAnoDto>> ObterQuantidadeTurmaPorAno(FiltroRelatorioAnaliticoSondagemDto filtro, string codigoUe, int modalidade = (int)Modalidade.Fundamental)
        {
            var historico = filtro.AnoLetivo < DateTime.Now.Year;
            var turmas = (await turmaRepository.ObterTotalDeTurmasPorAno(filtro.LoginUsuarioLogado, codigoUe, filtro.PerfilUsuarioLogado, historico, modalidade, filtro.AnoLetivo)).ToList();
            return turmas;
        }

        private async Task<int> ObterTotalAlunosAtivosPorTurmaEPeriodo(List<string> turmasCodigo, DateTime dataFim)
        {
            var quantidade = 0;
            foreach (var turmaCodigo in turmasCodigo)
            {
                var consultaQuantidade = await alunoRepository.ObterTotalAlunosAtivosPorTurmaEPeriodo(turmaCodigo, dataFim);
                quantidade += consultaQuantidade;
            }

            return quantidade;
        }

        private ItemRespostaCapacidadeDeLeituraDto MapearOrdemDoNarrarLocalizacao(IGrouping<string, OrdemPerguntaRespostaDto> ordemItem, int totalDeAlunos, List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPergunta)
        {
            var localizacao = new ItemRespostaCapacidadeDeLeituraDto();
            var perguntaItemLocalizacao = relatorioAgrupadoPergunta.FirstOrDefault()?.Where(x => x.PerguntaDescricao == PerguntaDescricaoSondagem.Localizacao).ToList();
            if (perguntaItemLocalizacao != null)
            {
                var totalRespostas = perguntaItemLocalizacao.Select(s => s.QtdRespostas).ToList().Sum();
                totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

                localizacao.Adequada = perguntaItemLocalizacao.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Adequada).Select(x => x.QtdRespostas).Sum();
                localizacao.Inadequada = perguntaItemLocalizacao.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Inadequada).Select(x => x.QtdRespostas).Sum();
                localizacao.NaoResolveu = perguntaItemLocalizacao.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.NaoResolveu).Select(x => x.QtdRespostas).Sum();
                localizacao.SemPreenchimento = totalDeAlunos - totalRespostas;
            }

            return localizacao;
        }

        private ItemRespostaCapacidadeDeLeituraDto MapearOrdemDoNarrarInferencia(IGrouping<string, OrdemPerguntaRespostaDto> ordemItem, int totalDeAlunos, List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPergunta)
        {
            var inferencia = new ItemRespostaCapacidadeDeLeituraDto();
            var perguntaIteInferencia = relatorioAgrupadoPergunta.FirstOrDefault()?.Where(x => x.PerguntaDescricao == PerguntaDescricaoSondagem.Inferencia).ToList();
            if (perguntaIteInferencia != null)
            {
                var totalRespostas = perguntaIteInferencia.Select(s => s.QtdRespostas).ToList().Sum();
                totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

                inferencia.Adequada = perguntaIteInferencia.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Adequada).Select(x => x.QtdRespostas).Sum();
                inferencia.Inadequada = perguntaIteInferencia.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Inadequada).Select(x => x.QtdRespostas).Sum();
                inferencia.NaoResolveu = perguntaIteInferencia.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.NaoResolveu).Select(x => x.QtdRespostas).Sum();
                inferencia.SemPreenchimento = totalDeAlunos - totalRespostas;
            }

            return inferencia;
        }

        private ItemRespostaCapacidadeDeLeituraDto MapearOrdemDoNarrarReflexao(IGrouping<string, OrdemPerguntaRespostaDto> ordemItem, int totalDeAlunos, List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPergunta)
        {
            var reflexao = new ItemRespostaCapacidadeDeLeituraDto();
            var perguntaItemReflexao = relatorioAgrupadoPergunta.FirstOrDefault()?.Where(x => x.PerguntaDescricao == PerguntaDescricaoSondagem.Reflexao).ToList();
            if (perguntaItemReflexao != null)
            {
                var totalRespostas = perguntaItemReflexao.Select(s => s.QtdRespostas).ToList().Sum();
                totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

                reflexao.Adequada = perguntaItemReflexao.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Adequada).Select(x => x.QtdRespostas).Sum();
                reflexao.Inadequada = perguntaItemReflexao.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Inadequada).Select(x => x.QtdRespostas).Sum();
                reflexao.NaoResolveu = perguntaItemReflexao.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.NaoResolveu).Select(x => x.QtdRespostas).Sum();
                reflexao.SemPreenchimento = totalDeAlunos - totalRespostas;
            }

            return reflexao;
        }

        private async Task<PeriodoSondagem> ObterPeriodoSondagem(int bimestre, int anoLetivo)
        {
            var termo = @$"{bimestre}° {(anoLetivo < 2022 ? "Semestre" : "Bimestre")}";

            var sql = " select * from \"Periodo\" p where p.\"Descricao\" = @termo";

            using (var conn = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conn.QueryFirstOrDefaultAsync<PeriodoSondagem>(sql, new { termo });
            }
        }

        private async Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem(int anoLetivo, string periodoId)
        {
            var sql = " select * from \"PeriodoFixoAnual\" pfa where pfa.\"PeriodoId\" = @periodoId and \"Ano\" = @anoLetivo ";
            var parametros = new { periodoId, anoLetivo };
            using (var conn = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conn.QueryFirstOrDefaultAsync<PeriodoFixoSondagem>(sql, parametros);
            }
        }

        private string ConsultaLeituraLinguaPortuguesaPrimeiroAoTerceiroAno(FiltroRelatorioAnaliticoSondagemDto filtroRelatorioAnaliticoSondagemDto)
        {
            var sql = new StringBuilder();
            sql.AppendLine("         select ");
            sql.AppendLine("                pp.\"yearClassroom\" as AnoTurma ,");
            sql.AppendLine("                pp.\"classroomCodeEol\" as TurmaCodigo,  ");
            sql.AppendLine("                pp.\"dreCodeEol\" as CodigoDre,  ");
            sql.AppendLine("                pp.\"schoolCodeEol\" as CodigoUe,  ");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"reading{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'Nivel1') as  Nivel1,");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"reading{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'Nivel2') as  Nivel2,");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"reading{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'Nivel3') as  Nivel3,");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"reading{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'Nivel4') as  Nivel4,");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where ((pp.\"reading{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" is null) or (trim(pp.\"reading{filtroRelatorioAnaliticoSondagemDto.Periodo}B\") = ''))) as SemPreenchimento");
            sql.AppendLine("         from \"PortuguesePolls\" pp ");
            sql.AppendLine("         where   pp.\"schoolYear\" = @anoLetivo     ");

            if (filtroRelatorioAnaliticoSondagemDto.DreCodigo != "-99")
                sql.AppendLine("				and pp.\"dreCodeEol\" = @dreCodeEol ");

            if (filtroRelatorioAnaliticoSondagemDto.UeCodigo != "-99")
                sql.AppendLine("                and pp.\"schoolCodeEol\" = @ueCodigo ");

            sql.AppendLine("			    group by pp.\"dreCodeEol\", pp.\"schoolCodeEol\",pp.\"classroomCodeEol\" ,pp.\"yearClassroom\" ");
            sql.AppendLine("                order by pp.\"dreCodeEol\", pp.\"schoolCodeEol\", pp.\"yearClassroom\" ;");

            return sql.ToString();
        }

        private string ConsultaEscritaLinguaPortuguesaPrimeiroAoTerceiroAno(FiltroRelatorioAnaliticoSondagemDto filtroRelatorioAnaliticoSondagemDto)
        {
            var sql = new StringBuilder();
            sql.AppendLine("               select  ");
            sql.AppendLine("                pp.\"yearClassroom\" as AnoTurma ,");
            sql.AppendLine("                pp.\"classroomCodeEol\" as TurmaCodigo,  ");
            sql.AppendLine("                pp.\"dreCodeEol\" as DreCodigo,  ");
            sql.AppendLine("                pp.\"schoolCodeEol\" as UeCodigo,  ");
            sql.AppendLine($"		        count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'PS') as  PreSilabico,");
            sql.AppendLine($"		        count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'SSV') as  SilabicoSemValor,");
            sql.AppendLine($"		        count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'SCV') as  SilabicoComValor,");
            sql.AppendLine($"		        count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'SA') as  SilabicoAlfabetico,");
            sql.AppendLine($"                count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'A') as  Alfabetico,");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'Nivel1') as  Nivel1,");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'Nivel2') as  Nivel2,");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'Nivel3') as  Nivel3,");
            sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'Nivel4') as  Nivel4,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where ((pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" is null) or (trim(pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\") = ''))) as SemPreenchimento");
            sql.AppendLine("         from \"PortuguesePolls\" pp ");
            sql.AppendLine("         where  pp.\"schoolYear\" = @anoLetivo ");

            if (filtroRelatorioAnaliticoSondagemDto.DreCodigo != "-99")
                sql.AppendLine("                and pp.\"dreCodeEol\" = @dreCodeEol");

            if (filtroRelatorioAnaliticoSondagemDto.UeCodigo != "-99")
                sql.AppendLine("                and pp.\"schoolCodeEol\" = @ueCodigo");

            sql.AppendLine("			    group by pp.\"dreCodeEol\", pp.\"schoolCodeEol\" ,pp.\"yearClassroom\" ,pp.\"classroomCodeEol\" ,pp.\"yearClassroom\"");
            sql.AppendLine("                order by pp.\"dreCodeEol\", pp.\"schoolCodeEol\", pp.\"yearClassroom\" ");

            return sql.ToString();
        }
        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoCampoAditivo(FiltroRelatorioAnaliticoSondagemDto filtro) => await ObterRelatorioSondagemAnaliticoCampoAditivoMultiplicativo(filtro, ProficienciaSondagemEnum.CampoAditivo);
        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoCampoMultiplicativo(FiltroRelatorioAnaliticoSondagemDto filtro) => await ObterRelatorioSondagemAnaliticoCampoAditivoMultiplicativo(filtro, ProficienciaSondagemEnum.CampoMultiplicativo);

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoNumero(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoNumeroIadDto>();
            var considerarBimestre = filtro.AnoLetivo >= ANO_ESCOLAR_2022;
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> { 5, 13 };

            var consultarDados = await ConsultaMatematicaNumerosAutoral(filtro, periodo.Id);
            var listaDres = await dreRepository.ObterPorCodigos(consultarDados.Where(x => x.CodigoDre != null).Select(x => x.CodigoDre).Distinct().ToArray());
            var perguntasPorAno = consultarDados.Where(x => x.AnoTurma > 0).GroupBy(p => new { p.AnoTurma, p.OrdermPergunta, p.PerguntaDescricao }).ToList();
            var respostasPorAno = consultarDados.Where(x => x.AnoTurma > 0).GroupBy(p => new { p.OrdermPergunta, p.PerguntaDescricao, p.OrdemReposta, p.RespostaDescricao }).ToList();

            var cabecalho = respostasPorAno.GroupBy(x => new { x.Key.OrdermPergunta, x.Key.PerguntaDescricao }).Select(x =>
                new CabecalhoSondagemAnaliticaDto
                {
                    Descricao = x.Key.PerguntaDescricao,
                    Ordem = x.Key.OrdermPergunta,
                    SubCabecalhos = respostasPorAno
                    .Where(f => f.Key.OrdermPergunta == x.Key.OrdermPergunta)
                    .Select(y => new SubCabecalhoSondagemAnaliticaDto { Ordem = y.Key.OrdemReposta, IdPerguntaResposta = @$"{x.Key.OrdermPergunta}_{y.Key.OrdemReposta}", Descricao = y.Key.RespostaDescricao }).ToList()
                }
                ).ToList();

            foreach (var dre in listaDres)
            {
                var relatorioSondagemAnaliticoNumeroIad = new RelatorioSondagemAnaliticoNumeroIadDto(TipoSondagem.MAT_Numeros)
                {
                    Dre = dre.Nome,
                    DreSigla = dre.Abreviacao,
                    AnoLetivo = filtro.AnoLetivo,
                    Periodo = filtro.Periodo
                };

                relatorioSondagemAnaliticoNumeroIad.ColunasDoCabecalho.AddRange(cabecalho);

                var listaUes = await ueRepository.ObterPorCodigos(consultarDados.Where(x => x.CodigoDre == dre.Codigo && x.CodigoUe != null).Select(x => x.CodigoUe).Distinct().ToArray());
                foreach (var ue in listaUes)
                {
                    var turmas = await ObterQuantidadeTurmaPorAno(filtro, ue.Codigo);

                    var totalDeAlunosPorAno = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(),
                                                                                                                periodoFixo.DataInicio, periodoFixo.DataFim,
                                                                                                                ue.Codigo, dre.Codigo)).ToList();
                    foreach (var anoTurmaItem in perguntasPorAno)
                    {
                        var totalDeAlunos = totalDeAlunosPorAno.Where(x => x.AnoTurma == anoTurmaItem.Key.AnoTurma.ToString()).Select(x => x.QuantidadeAluno).Sum();

                        var relatorioSondagemAnaliticoNumero = relatorioSondagemAnaliticoNumeroIad.Respostas.Where(x => x.Ano == anoTurmaItem.Key.AnoTurma && x.Ue == ue.Nome).FirstOrDefault();
                        if (relatorioSondagemAnaliticoNumero == null)
                        {
                            relatorioSondagemAnaliticoNumero = new RespostaSondagemAnaliticaNumeroIadDto()
                            {
                                TotalDeAlunos = totalDeAlunos,
                                Ano = anoTurmaItem.Key.AnoTurma,
                                TotalDeTurma = turmas.Count(x => x.AnoTurma == anoTurmaItem.Key.AnoTurma.ToString()),
                                Ue = ue.Nome
                            };
                            relatorioSondagemAnaliticoNumeroIad.Respostas.Add(relatorioSondagemAnaliticoNumero);
                        }

                        var perguntasRespostasUe = consultarDados.Where(x => x.CodigoUe == ue.Codigo).ToList();
                        MapearRespostaNumeros(perguntasRespostasUe, anoTurmaItem.Key.AnoTurma, anoTurmaItem.Key.OrdermPergunta, anoTurmaItem.Key.PerguntaDescricao,
                                                                                                    totalDeAlunos, relatorioSondagemAnaliticoNumero.Respostas
                                                                                                    , relatorioSondagemAnaliticoNumeroIad.ColunasDoCabecalho);

                        var semPrenchimento = TotalSemRespostasNumerosIAD(relatorioSondagemAnaliticoNumero.Respostas, totalDeAlunos);
                    }
                }
                relatorioSondagemAnaliticoNumeroIad.Respostas.Select(x => x.Respostas.Count == 0);
                retorno.Add(relatorioSondagemAnaliticoNumeroIad);
            }
            return retorno;
        }

        private int TotalSemRespostasNumerosIAD(List<RespostaSondagemAnaliticaDto> respostas, int totalDeAlunos)
        {
            var totalRespostas = respostas?.Sum(c => c.Valor) ?? 0;
            return totalDeAlunos - totalRespostas;
        }

        private void MapearRespostaNumeros(List<PerguntaRelatorioMatematicaNumerosDto> perguntasRespostasUe, int anoTurma, int ordermPergunta, string perguntaDescricao, int totalDeAlunos,
            List<RespostaSondagemAnaliticaDto> respostas, List<CabecalhoSondagemAnaliticaDto> colunasDoCabecalho)
        {
            var perguntas = perguntasRespostasUe.Where(x => x.AnoTurma == anoTurma && x.OrdermPergunta == ordermPergunta);

            foreach (var cabec in colunasDoCabecalho)
            {
                var naoExiste = cabec.SubCabecalhos?.Count(x => x.Descricao == DESCRICAO_SEMPREENCHIMENTO) == 0;
                if (naoExiste)
                    cabec.SubCabecalhos.Add(new SubCabecalhoSondagemAnaliticaDto { Descricao = DESCRICAO_SEMPREENCHIMENTO, IdPerguntaResposta = $"{cabec.Ordem}_{cabec.SubCabecalhos.Count() + 1}", Ordem = cabec.SubCabecalhos.Count() + 1 });
            }
            var cabecalhoRespostas = colunasDoCabecalho.Where(x => x.Ordem == ordermPergunta).FirstOrDefault().SubCabecalhos;


            foreach (var cabecalhoResposta in cabecalhoRespostas)
            {
                var resposta = perguntas.Where(x => x.OrdemReposta == cabecalhoResposta.Ordem);
                if (cabecalhoResposta.Descricao == DESCRICAO_SEMPREENCHIMENTO)
                {
                    var totalRespostas = respostas?.Sum(c => c.Valor) ?? 0;
                    var semPrenechimento = totalDeAlunos - totalRespostas;
                    respostas.Add(new RespostaSondagemAnaliticaDto
                    {
                        IdPerguntaResposta = $"{ordermPergunta}_{cabecalhoResposta.Ordem}",
                        Valor = semPrenechimento,
                    });

                }
                else
                {
                    respostas.Add(new RespostaSondagemAnaliticaDto
                    {
                        IdPerguntaResposta = $"{ordermPergunta}_{cabecalhoResposta.Ordem}",
                        Valor = resposta?.Sum(x => x.QtdRespostas) ?? 0,
                    });
                }
                
            }
        }

        private async Task<IEnumerable<PerguntaRelatorioMatematicaNumerosDto>> ConsultaMatematicaNumerosAutoral(FiltroRelatorioAnaliticoSondagemDto filtro, string periodoId)
        {
            return filtro.AnoLetivo >= ANO_ESCOLAR_2022 ? await sondagemRelatorioRepository.MatematicaIADNumeroBimestre(filtro.AnoLetivo, SondagemComponenteCurricular.MATEMATICA, filtro.Periodo, filtro.UeCodigo, filtro.DreCodigo, ProficienciaSondagemEnum.Numeros)
                                                 : await sondagemRelatorioRepository.MatematicaNumerosAntes2022(filtro.AnoLetivo, filtro.Periodo, filtro.UeCodigo, filtro.DreCodigo, periodoId);
        }
        public Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoIAD(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoCampoAditivoMultiplicativo(FiltroRelatorioAnaliticoSondagemDto filtro, ProficienciaSondagemEnum proficiencia)
        {
            var retorno = new List<RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> { 5, 13 };

            IEnumerable<PerguntaRespostaOrdemDto> dtoConsultaDados = Enumerable.Empty<PerguntaRespostaOrdemDto>(); ;
            if (filtro.AnoLetivo >= 2022)
                dtoConsultaDados = await sondagemRelatorioRepository.ConsolidacaoCampoAditivoMultiplicativo(new RelatorioMatematicaFiltroDto
                {
                    AnoLetivo = filtro.AnoLetivo,
                    CodigoDre = filtro.DreCodigo,
                    CodigoUe = filtro.UeCodigo,
                    ComponenteCurricularId = SondagemComponenteCurricular.MATEMATICA,
                    Proficiencia = proficiencia,
                    Bimestre = filtro.Periodo
                });
            else dtoConsultaDados = await sondagemRelatorioRepository.ConsolidacaoCampoAditivoMultiplicativoAntes2022(new RelatorioMatematicaFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                Proficiencia = proficiencia,
                Bimestre = filtro.Periodo
            });


            var perguntasPorAno = dtoConsultaDados.Where(x => x.AnoTurma != null).GroupBy(p => new { p.AnoTurma, p.OrdemPergunta, p.PerguntaDescricao }).ToList();
            var listaDres = await dreRepository.ObterPorCodigos(dtoConsultaDados.Where(x => x.CodigoDre != null).Select(x => x.CodigoDre).Distinct().ToArray());
            foreach (var dre in listaDres.OrderBy(x => x.Abreviacao))
            {
                var perguntas = new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto();
                var relatorioSondagemAnaliticoCampoAditivoMultiplicativoDto = new RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto(TipoSondagem.MAT_CampoAditivo)
                {
                    Dre = dre.Nome,
                    DreSigla = dre.Abreviacao,
                    AnoLetivo = filtro.AnoLetivo,
                    Periodo = filtro.Periodo
                };

                var listaUes = await ueRepository.ObterPorCodigos(dtoConsultaDados.Where(x => x.CodigoDre == dre.Codigo && x.CodigoUe != null).Select(x => x.CodigoUe).Distinct().ToArray());
                foreach (var ue in listaUes.OrderBy(x => x.Nome))
                {
                    var turmas = await ObterQuantidadeTurmaPorAno(filtro, ue.Codigo);
                    if (!turmas.Any())
                    {
                        turmas.AddRange(
                            dtoConsultaDados.Where(x => x.CodigoUe == ue.Codigo).GroupBy(x => new { x.CodigoTurma, x.AnoTurma }).Select(x => new QuantidadeTurmaPorAnoDto { Codigo = x.Key.CodigoTurma, AnoTurma = x.Key.AnoTurma }));
                    }

                    var totalDeAlunosPorAno = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(),
                                                                                                                periodoFixo.DataInicio, periodoFixo.DataFim,
                                                                                                                ue.Codigo, dre.Codigo)).ToList();
                    foreach (var anoTurmaItem in perguntasPorAno)
                    {
                        var descricaoPergunta = ObterDescricaoPergunta(proficiencia, anoTurmaItem.Key.PerguntaDescricao, anoTurmaItem.Key.OrdemPergunta, int.Parse(anoTurmaItem.Key.AnoTurma));
                        if (string.IsNullOrEmpty(descricaoPergunta)) continue;

                        var totalDeAlunos = totalDeAlunosPorAno.Where(x => x.AnoTurma == anoTurmaItem.Key.AnoTurma).Select(x => x.QuantidadeAluno).Sum();

                        var respostaSondagemAnaliticoCampoAditivoMultiplicativoDto = relatorioSondagemAnaliticoCampoAditivoMultiplicativoDto.Respostas.Where(x => x.Ano == int.Parse(anoTurmaItem.Key.AnoTurma) && x.Ue == ue.Nome).FirstOrDefault();
                        if (respostaSondagemAnaliticoCampoAditivoMultiplicativoDto == null)
                        {
                            respostaSondagemAnaliticoCampoAditivoMultiplicativoDto = new RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto()
                            {
                                TotalDeAlunos = totalDeAlunos,
                                Ano = int.Parse(anoTurmaItem.Key.AnoTurma),
                                TotalDeTurma = turmas.Count(x => x.AnoTurma == anoTurmaItem.Key.AnoTurma),
                                Ue = ue.Nome
                            };
                            relatorioSondagemAnaliticoCampoAditivoMultiplicativoDto.Respostas.Add(respostaSondagemAnaliticoCampoAditivoMultiplicativoDto);
                        }

                        var perguntasRespostasUe = dtoConsultaDados.Where(x => x.CodigoUe == ue.Codigo).ToList();
                        var respostaSondagemAnaliticoOrdem = ObterRespostaSondagemAnaliticoOrdemDto(perguntasRespostasUe, anoTurmaItem.Key.AnoTurma,
                                                                                                    anoTurmaItem.Key.OrdemPergunta, anoTurmaItem.Key.PerguntaDescricao,
                                                                                                    totalDeAlunos);

                        respostaSondagemAnaliticoCampoAditivoMultiplicativoDto.Ordens.Add(respostaSondagemAnaliticoOrdem);

                    }
                }
                retorno.Add(relatorioSondagemAnaliticoCampoAditivoMultiplicativoDto);
            }

            return retorno;
        }

        private string ObterDescricaoPergunta(ProficienciaSondagemEnum proficiencia, string perguntaDescricao, int ordem, int anoTurma)
        {
            if (!string.IsNullOrEmpty(perguntaDescricao))
                return perguntaDescricao;

            string orderTitle = string.Empty;

            switch (anoTurma)
            {
                case 1:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            orderTitle = "COMPOSIÇÃO";
                            break;

                        default:
                            orderTitle = string.Empty;
                            break;
                    }
                    break;

                case 2:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    orderTitle = "COMPOSIÇÃO";
                                    break;

                                case 2:
                                    orderTitle = "TRANSFORMAÇÃO";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;

                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 3:
                                    orderTitle = "PROPORCIONALIDADE";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;

                case 3:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    orderTitle = "COMPOSIÇÃO";
                                    break;

                                case 2:
                                    orderTitle = "TRANSFORMAÇÃO";
                                    break;

                                case 3:
                                    orderTitle = "COMPARAÇÃO";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;

                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 4:
                                    orderTitle = "CONFIGURAÇÃO RETANGULAR";
                                    break;

                                case 5:
                                    orderTitle = "PROPORCIONALIDADE";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;

                case 4:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    orderTitle = "COMPOSIÇÃO";
                                    break;

                                case 2:
                                    orderTitle = "TRANSFORMAÇÃO";
                                    break;

                                case 3:
                                    orderTitle = "COMPOSIÇÃO DE TRANSF.";
                                    break;

                                case 4:
                                    orderTitle = "COMPARAÇÃO";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;

                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 5:
                                    orderTitle = "CONFIGURAÇÃO RETANGULAR";
                                    break;

                                case 6:
                                    orderTitle = "PROPORCIONALIDADE";
                                    break;

                                case 7:
                                    orderTitle = "COMBINATÓRIA";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;

                case 5:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    orderTitle = "COMPOSIÇÃO";
                                    break;

                                case 2:
                                    orderTitle = "TRANSFORMAÇÃO";
                                    break;

                                case 3:
                                    orderTitle = "COMPOSIÇÃO DE TRANSF.";
                                    break;

                                case 4:
                                    orderTitle = "COMPARAÇÃO";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;

                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 5:
                                    orderTitle = "COMBINATÓRIA";
                                    break;

                                case 6:
                                    orderTitle = "CONFIGURAÇÃO RETANGULAR";
                                    break;

                                case 7:
                                    orderTitle = "PROPORCIONALIDADE";
                                    break;

                                case 8:
                                    orderTitle = "MULTIPLICAÇÃO COMPARATIVA";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;

                case 6:
                    switch (proficiencia)
                    {
                        case ProficienciaSondagemEnum.CampoAditivo:
                            switch (ordem)
                            {
                                case 1:
                                    orderTitle = "COMPOSIÇÃO";
                                    break;

                                case 2:
                                    orderTitle = "TRANSFORMAÇÃO";
                                    break;

                                case 3:
                                    orderTitle = "COMPOSIÇÃO DE TRANSF.";
                                    break;

                                case 4:
                                    orderTitle = "COMPARAÇÃO";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;

                        case ProficienciaSondagemEnum.CampoMultiplicativo:
                            switch (ordem)
                            {
                                case 5:
                                    orderTitle = "COMBINATÓRIA";
                                    break;

                                case 6:
                                    orderTitle = "CONFIGURAÇÃO RETANGULAR";
                                    break;

                                case 7:
                                    orderTitle = "PROPORCIONALIDADE";
                                    break;

                                case 8:
                                    orderTitle = "MULTIPLICAÇÃO COMPARATIVA";
                                    break;

                                default:
                                    orderTitle = string.Empty;
                                    break;
                            }
                            break;
                    }
                    break;

                default:
                    break;
            }

            return orderTitle;

        }
        private RespostaOrdemMatematicaDto ObterRespostaSondagemAnaliticoOrdemDto(List<PerguntaRespostaOrdemDto> perguntasRepostasUe,
                                                                                  string anoTurma, int ordemPergunta, string descricaoPergunta, int totalDeAlunos)
        {
            var respostaSondagemAnaliticoIdeiaDto = new RespostaMatematicaDto();
            var respostaSondagemAnaliticoResultadoDto = new RespostaMatematicaDto();

            var perguntasRespostas = perguntasRepostasUe.Where(x => x.AnoTurma == anoTurma && x.OrdemPergunta == ordemPergunta && x.SubPerguntaDescricao == "Ideia").ToList();
            respostaSondagemAnaliticoIdeiaDto.Acertou = perguntasRespostas?.Where(x => x.RespostaDescricao == "Acertou").Select(x => x.QtdRespostas).Sum() ?? 0;
            respostaSondagemAnaliticoIdeiaDto.Errou = perguntasRespostas?.Where(x => x.RespostaDescricao == "Errou").Select(x => x.QtdRespostas).Sum() ?? 0;
            respostaSondagemAnaliticoIdeiaDto.NaoResolveu = perguntasRespostas?.Where(x => x.RespostaDescricao == "Não resolveu").Select(x => x.QtdRespostas).Sum() ?? 0;
            var totalRespostas = respostaSondagemAnaliticoIdeiaDto.Acertou +
                                    respostaSondagemAnaliticoIdeiaDto.Errou +
                                    respostaSondagemAnaliticoIdeiaDto.NaoResolveu;
            respostaSondagemAnaliticoIdeiaDto.SemPreenchimento = totalDeAlunos - totalRespostas;


            perguntasRespostas = perguntasRepostasUe?.Where(x => x.AnoTurma == anoTurma && x.OrdemPergunta == ordemPergunta && x.SubPerguntaDescricao == "Resultado").ToList();
            respostaSondagemAnaliticoResultadoDto.Acertou = perguntasRespostas?.Where(x => x.RespostaDescricao == "Acertou").Select(x => x.QtdRespostas).Sum() ?? 0;
            respostaSondagemAnaliticoResultadoDto.Errou = perguntasRespostas?.Where(x => x.RespostaDescricao == "Errou").Select(x => x.QtdRespostas).Sum() ?? 0;
            respostaSondagemAnaliticoResultadoDto.NaoResolveu = perguntasRespostas?.Where(x => x.RespostaDescricao == "Não resolveu").Select(x => x.QtdRespostas).Sum() ?? 0;
            totalRespostas = respostaSondagemAnaliticoResultadoDto.Acertou +
                             respostaSondagemAnaliticoResultadoDto.Errou +
                             respostaSondagemAnaliticoResultadoDto.NaoResolveu;
            respostaSondagemAnaliticoResultadoDto.SemPreenchimento = totalDeAlunos - totalRespostas;

            return new RespostaOrdemMatematicaDto
            {
                Ordem = ordemPergunta,
                Descricao = descricaoPergunta,
                Resultado = respostaSondagemAnaliticoResultadoDto,
                Ideia = respostaSondagemAnaliticoIdeiaDto
            };
        }
    }
}