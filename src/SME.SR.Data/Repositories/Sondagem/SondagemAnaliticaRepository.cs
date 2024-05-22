using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.ApplicationInsights.Metrics.Extensibility;
using Npgsql;
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
        private readonly int ANO_ESCOLAR_2023 = 2023;
        private readonly int PRIMEIRO_PERIODO = 1;

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

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoEscrita(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo, filtro.TipoSondagem);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> { 5, 13 };
            var quantidadeSemPreenchimentoAVerificar = new Dictionary<string, int>();

            var sql = ConsultaEscritaLinguaPortuguesaPrimeiroAoTerceiroAno(filtro);
            var parametros = new { dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma };
            IEnumerable<TotalRespostasAnaliticoEscritaDto> dtoConsulta = null;
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                dtoConsulta = (await conexao.QueryAsync<TotalRespostasAnaliticoEscritaDto>(sql, parametros)).ToList();
            }

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
                        var agrupamentoPorAnoTurma = itemUe.GroupBy(x => x.AnoTurma).ToList();
                        var quantidadeTotalAlunosPorAno = new List<TotalAlunosAnoTurmaDto>(); //(await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio.Date, periodoFixo.DataFim.Date, itemUe.Key, itemDre.Key)).ToList();
                        var totalTurmas = await ObterQuantidadeTurmaPorAno(itemUe.Key, filtro.AnoLetivo);

                        foreach (var anoTurma in agrupamentoPorAnoTurma)
                        {
                            var quantidadeTotalAlunosEol = quantidadeTotalAlunosPorAno.Where(x => x.AnoTurma == anoTurma.Key).Select(x => x.QuantidadeAluno).Sum();
                            int quantidadeTurmas = totalTurmas?.FirstOrDefault(t => t.Ano == anoTurma.Key).Quantidade ?? 0;

                            var totalSemPreenchimento = EhTerceiroAnoPrimeiroPeriodoAteDoisMilEVinteTres(filtro, anoTurma)
                                    ? TotalSemPreenchimentoTerceiroAnoEscritaSoNivel(anoTurma, quantidadeTotalAlunosEol)
                                    : TotalSemPreenchimentoPrimeiroSegundoAnoEscrita(anoTurma, quantidadeTotalAlunosEol, anoTurma.Key.Equals("3"));

                            var totalDeAlunosNaSondagem = EhTerceiroAnoPrimeiroPeriodoAteDoisMilEVinteTres(filtro, anoTurma)
                                                                ? TotalAlunosEscritaTerceiroAno(anoTurma, totalSemPreenchimento)
                                                                : TotalAlunosEscrita(anoTurma, totalSemPreenchimento, anoTurma.Key.Equals("3"));

                            int quantidadeSemPreenchimentoDoAnoTurma = quantidadeSemPreenchimentoAVerificar?.FirstOrDefault(q => q.Key == anoTurma.Key).Value ?? 0;

                            if (totalDeAlunosNaSondagem >= quantidadeTotalAlunosEol && quantidadeSemPreenchimentoDoAnoTurma == 0)
                            {
                                if (quantidadeSemPreenchimentoAVerificar.Any(q => q.Key == anoTurma.Key))
                                    quantidadeSemPreenchimentoAVerificar.Remove(anoTurma.Key);

                                quantidadeSemPreenchimentoAVerificar.Add(anoTurma.Key, totalDeAlunosNaSondagem - quantidadeTotalAlunosEol);

                                quantidadeSemPreenchimentoDoAnoTurma = totalDeAlunosNaSondagem - quantidadeTotalAlunosEol;
                            }

                            if (quantidadeSemPreenchimentoDoAnoTurma > 0 && totalSemPreenchimento > quantidadeSemPreenchimentoDoAnoTurma)
                            {
                                totalSemPreenchimento = totalSemPreenchimento - quantidadeSemPreenchimentoDoAnoTurma;
                                quantidadeSemPreenchimentoAVerificar.Remove(anoTurma.Key);
                                quantidadeSemPreenchimentoAVerificar.Add(anoTurma.Key, 0);

                                quantidadeSemPreenchimentoDoAnoTurma = 0;
                            }
                            else if (totalSemPreenchimento > 0 && totalSemPreenchimento < quantidadeSemPreenchimentoDoAnoTurma)
                            {
                                quantidadeSemPreenchimentoAVerificar.Remove(anoTurma.Key);
                                quantidadeSemPreenchimentoAVerificar.Add(anoTurma.Key, quantidadeSemPreenchimentoDoAnoTurma - totalSemPreenchimento);

                                totalSemPreenchimento = 0;
                            }

                            var Ue = ueLista.FirstOrDefault(x => x.Codigo == itemUe.Key);
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
                                SemPreenchimento = totalSemPreenchimento,
                                TotalDeAlunos = quantidadeTotalAlunosEol,
                                Ano = int.Parse(anoTurma.Key),
                                TotalDeTurma = quantidadeTurmas,
                                Ue = Ue.TituloTipoEscolaNome
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

        public async Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem(string termoPeriodo, int anoLetivo)
        {
            var sql = new StringBuilder();

            sql.AppendLine("select pfa.* ");
            sql.AppendLine("from \"Periodo\" p");
            sql.AppendLine("inner join \"PeriodoFixoAnual\" pfa on pfa.\"PeriodoId\" = p.\"Id\" ");
            sql.AppendLine("where p.\"Descricao\" = @termoPeriodo ");
            sql.AppendLine("and \"Ano\" = @anoLetivo ");

            var parametros = new { termoPeriodo, anoLetivo };
            using (var conn = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conn.QueryFirstOrDefaultAsync<PeriodoFixoSondagem>(sql.ToString(), parametros);
            }
        }

        private bool EhTerceiroAnoPrimeiroPeriodoAteDoisMilEVinteTres(FiltroRelatorioAnaliticoSondagemDto filtro, IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma)
        {
            return (anoTurma.Key == TURMA_TERCEIRO_ANO && (filtro.AnoLetivo <= ANO_ESCOLAR_2023 && filtro.Periodo == PRIMEIRO_PERIODO));
        }
        private static int TotalSemPreenchimentoPrimeiroSegundoAnoEscrita(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int quantidadeTotalAlunosEol, bool ehTerceiroAnoNovoModelo = false)
        {
            var totalRepostas = ((anoTurma.Select(x => x.PreSilabico).Sum() + anoTurma.Select(x => x.SilabicoSemValor).Sum()
                                                               + anoTurma.Select(x => x.SilabicoComValor).Sum()
                                                               + anoTurma.Select(x => x.SilabicoAlfabetico).Sum()
                                                               + anoTurma.Select(x => x.Alfabetico).Sum()));

            if (ehTerceiroAnoNovoModelo)
                totalRepostas += anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum()
                                 + anoTurma.Select(x => x.Nivel3).Sum() + anoTurma.Select(x => x.Nivel4).Sum();

            return quantidadeTotalAlunosEol <= totalRepostas ? 0 : quantidadeTotalAlunosEol - totalRepostas;
        }

        private static int TotalSemPreenchimentoTerceiroAnoEscritaSoNivel(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int quantidadeTotalAlunosEol)
        {
            var totalRepostas = (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum()
                                 + anoTurma.Select(x => x.Nivel4).Sum());

            return quantidadeTotalAlunosEol <= totalRepostas ? 0 : quantidadeTotalAlunosEol - totalRepostas;
        }

        private static int TotalSemPreenchimentoTerceiroAnoEscrita(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int quantidadeTotalAlunosEol)
        {
            var totalRepostas = (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum()
                                 + anoTurma.Select(x => x.Nivel3).Sum() + anoTurma.Select(x => x.Nivel4).Sum()
                                 + anoTurma.Select(x => x.PreSilabico).Sum() + anoTurma.Select(x => x.SilabicoSemValor).Sum()
                                 + anoTurma.Select(x => x.SilabicoComValor).Sum() + anoTurma.Select(x => x.SilabicoAlfabetico).Sum() + anoTurma.Select(x => x.Alfabetico).Sum());

            return quantidadeTotalAlunosEol <= totalRepostas ? 0 : quantidadeTotalAlunosEol - totalRepostas;
        }

        public async Task<IEnumerable<TotalRespostasAnaliticoLeituraDto>> ObterRepostasRelatorioAnaliticoDeLeitura(FiltroRelatorioAnaliticoSondagemDto filtro, bool valorSemPreenchimento)
        {
            var sql = ConsultaLeituraLinguaPortuguesaPrimeiroAoTerceiroAno(filtro, valorSemPreenchimento);
            var parametros = new { dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conexao.QueryAsync<TotalRespostasAnaliticoLeituraDto>(sql, parametros);
            }
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoLeituraDeVozAlta(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var modalidades = new List<int> { 5, 13 };
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo, filtro.TipoSondagem);
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
                        var totalDeAlunosPorAnoTurma = new List<TotalAlunosAnoTurmaDto>();//(await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio.Date, periodoFixo.DataFim.Date, itemUe.Key, itemDre.Key)).ToList();
                        var relatorioAgrupadoPorAnoTurma = itemUe.GroupBy(x => x.AnoTurma).ToList();
                        var totalTurmas = await ObterQuantidadeTurmaPorAno(itemUe.Key, filtro.AnoLetivo);
                        foreach (var anoTurmaItem in relatorioAgrupadoPorAnoTurma)
                        {
                            var alunoNaSondagem = anoTurmaItem.GroupBy(x => x.CodigoAluno);
                            int quantidadeTurmas = totalTurmas?.FirstOrDefault(t => t.Ano == anoTurmaItem.Key).Quantidade ?? 0;

                            var quantidadeAlunoNaSondagem = alunoNaSondagem.Select(x => x.Key).Count();

                            var totalDeAlunos = totalDeAlunosPorAnoTurma.Where(x => x.AnoTurma == anoTurmaItem.Key).Select(x => x.QuantidadeAluno).Sum();
                            var Ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key);
                            var respostas = new RespostaSondagemAnaliticoLeituraDeVozAltaDto
                            {
                                NaoConseguiuOuNaoQuisLer = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoConseguiuOuNaoQuisLer),
                                LeuComMuitaDificuldade = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComMuitaDificuldade),
                                LeuComAlgumaFluencia = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComAlgumaFluencia),
                                LeuComFluencia = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComFluencia),
                                SemPreenchimento = totalDeAlunos - quantidadeAlunoNaSondagem,
                                Ano = int.Parse(anoTurmaItem.Key),
                                TotalDeTurma = quantidadeTurmas,
                                TotalDeAlunos = totalDeAlunos,
                                Ue = Ue.TituloTipoEscolaNome
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
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo, filtro.TipoSondagem);
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
                        var totalDeAlunosPorAnoTurma = new List<TotalAlunosAnoTurmaDto>();//(await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, itemUe.Key, itemDre.Key)).ToList();
                        var relatorioAgrupadoPorAnoTurma = itemUe.GroupBy(x => x.AnoTurma).ToList();
                        var totalTurmas = await ObterQuantidadeTurmaPorAno(itemUe.Key, filtro.AnoLetivo);

                        foreach (var anoTurmaItem in relatorioAgrupadoPorAnoTurma)
                        {
                            int quantidadeTurmas = totalTurmas?.FirstOrDefault(t => t.Ano == anoTurmaItem.Key).Quantidade ?? 0;
                            var alunoNaSondagem = anoTurmaItem.GroupBy(x => x.CodigoAluno);
                            var quantidadeAlunoNaSondagem = alunoNaSondagem.Select(x => x.Key).Count();

                            var totalDeAlunos = totalDeAlunosPorAnoTurma.Where(x => x.AnoTurma == anoTurmaItem.Key).Select(x => x.QuantidadeAluno).Sum();

                            var Ue = listaUes.FirstOrDefault(x => x.Codigo == itemUe.Key);
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
                                TotalDeTurma = quantidadeTurmas,
                                TotalDeAlunos = totalDeAlunos,
                                Ue = Ue.TituloTipoEscolaNome
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

        private int TotalAlunosEscritaTerceiroAno(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int valorSemPreenchimento)
        {
            return (valorSemPreenchimento + anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum()
                                                            + anoTurma.Select(x => x.Nivel4).Sum());
        }
        private int TotalAlunosEscrita(IGrouping<string, TotalRespostasAnaliticoEscritaDto> anoTurma, int valorSemPreenchimento, bool ehTerceiroAnoNovoModelo)
        {
            int totalEscrita = (valorSemPreenchimento + (anoTurma.Select(x => x.PreSilabico).Sum() + anoTurma.Select(x => x.SilabicoSemValor).Sum()
                                                                                  + anoTurma.Select(x => x.SilabicoComValor).Sum()
                                                                                  + anoTurma.Select(x => x.Alfabetico).Sum()
                                                                                  + anoTurma.Select(x => x.SilabicoAlfabetico).Sum()));

            return ehTerceiroAnoNovoModelo ? totalEscrita + (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum()
                                                            + anoTurma.Select(x => x.Nivel4).Sum()) : totalEscrita;
        }

        private async Task<IEnumerable<TotalDeTurmasPorAnoDto>> ObterQuantidadeTurmaPorAno(string codigoUe, int anoLetivo, int modalidade = (int)Modalidade.Fundamental)
           => await turmaRepository.ObterTotalDeTurmasPorUeAnoLetivoEModalidade(0, modalidade, anoLetivo);

        private async Task<int> ObterTotalAlunosAtivosPorTurmaEPeriodo(List<string> turmasCodigo, DateTime dataInicio, DateTime dataFim)
        {
            var quantidade = 0;

            foreach (var turmaCodigo in turmasCodigo)
            {
                var consultaQuantidade = await alunoRepository
                    .ObterTotalAlunosAtivosPorTurmaEPeriodo(turmaCodigo, dataInicio, dataFim);

                quantidade += consultaQuantidade;
            }

            return quantidade;
        }

        private string ObterTituloSemestreBimestre(TipoSondagem tipoSondagem, int anoLetivo)
        {
            var ehAnoLetivoAnterior2022 = anoLetivo < 2022;
            var ehAnoLetivo2022 = anoLetivo == 2022;
            var ehAnoLetivoApos2022 = anoLetivo > 2022;
            var ehAnoLetivoAposIgual2024 = anoLetivo >= 2024;
            var ehTipoSondagemMatematica = tipoSondagem == TipoSondagem.MAT_IAD ||
                                           tipoSondagem == TipoSondagem.MAT_CampoMultiplicativo ||
                                           tipoSondagem == TipoSondagem.MAT_CampoAditivo ||
                                           tipoSondagem == TipoSondagem.MAT_Numeros;

            var ehTipoSondagemPortuguesIAD = tipoSondagem == TipoSondagem.LP_CapacidadeLeitura ||
                                          tipoSondagem == TipoSondagem.LP_LeituraVozAlta ||
                                          tipoSondagem == TipoSondagem.LP_ProducaoTexto;

            if (ehTipoSondagemMatematica)
            {
                if (ehAnoLetivoAnterior2022)
                    return "Semestre";
                else if (ehAnoLetivo2022)
                    return "Bimestre";
                else if (ehAnoLetivoApos2022 && tipoSondagem == TipoSondagem.MAT_IAD)
                    return "Semestre";
                else
                    return "Bimestre";
            }
            else if (ehTipoSondagemPortuguesIAD && ehAnoLetivoAposIgual2024)
                return "Semestre";
            else return "Bimestre";

        }

        private async Task<PeriodoSondagem> ObterPeriodoSondagem(int bimestre, int anoLetivo, TipoSondagem tipoSondagem)
        {
            var termo = @$"{bimestre}° {ObterTituloSemestreBimestre(tipoSondagem, anoLetivo)}";

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

        private string ConsultaLeituraLinguaPortuguesaPrimeiroAoTerceiroAno(FiltroRelatorioAnaliticoSondagemDto filtroRelatorioAnaliticoSondagemDto, bool valorSemPreenchimento)
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
            if (valorSemPreenchimento)
                sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"reading{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'SemPreenchimento') as  SemPreenchimento");
            else
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

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoNumero(FiltroRelatorioAnaliticoSondagemDto filtro) => await ObterRelatorioSondagemAnaliticoNumeroIAD(filtro, ProficienciaSondagemEnum.Numeros);
        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoIAD(FiltroRelatorioAnaliticoSondagemDto filtro) => await ObterRelatorioSondagemAnaliticoNumeroIAD(filtro, ProficienciaSondagemEnum.IAD);

        private async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoNumeroIAD(FiltroRelatorioAnaliticoSondagemDto filtro, ProficienciaSondagemEnum proficienciaSondagemEnum)
        {
            var retorno = new List<RelatorioSondagemAnaliticoNumeroIadDto>();
            var considerarBimestre = filtro.AnoLetivo >= ANO_ESCOLAR_2022;
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo, filtro.TipoSondagem);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> { 5, 13 };

            var consultarDados = await ConsultaMatematicaNumerosAutoral(filtro, periodo.Id, proficienciaSondagemEnum);
            var listaDres = await dreRepository.ObterPorCodigos(consultarDados.Where(x => x.CodigoDre != null).Select(x => x.CodigoDre).Distinct().ToArray());
            var perguntasPorAno = consultarDados.Where(x => int.Parse(x.AnoTurma) > 0).GroupBy(p => new { p.AnoTurma, p.OrdemPergunta, p.PerguntaDescricao }).ToList().OrderBy(x => x.Key.AnoTurma).ThenBy(t => t.Key.OrdemPergunta);
            var respostasPorAno = consultarDados.Where(x => int.Parse(x.AnoTurma) > 0).GroupBy(p => new { p.OrdemPergunta, p.PerguntaDescricao, p.OrdemResposta, p.RespostaDescricao }).OrderBy(x => x.Key.OrdemPergunta).ThenBy(t => t.Key.OrdemResposta).ToList();

            var cabecalho = respostasPorAno.GroupBy(x => new { x.Key.OrdemPergunta, x.Key.PerguntaDescricao }).Select(x =>
                new CabecalhoSondagemAnaliticaDto
                {
                    Descricao = x.Key.PerguntaDescricao,
                    Ordem = x.Key.OrdemPergunta,
                    SubCabecalhos = respostasPorAno
                    .Where(f => f.Key.OrdemPergunta == x.Key.OrdemPergunta && f.Key.PerguntaDescricao == x.Key.PerguntaDescricao)
                    .Select(y => new SubCabecalhoSondagemAnaliticaDto { Ordem = y.Key.OrdemResposta, IdPerguntaResposta = @$"{x.Key.OrdemPergunta}_{x.Key.PerguntaDescricao}_{y.Key.RespostaDescricao}", Descricao = y.Key.RespostaDescricao }).ToList()
                }
                ).ToList();

            foreach (var dre in listaDres)
            {
                var relatorioSondagemAnaliticoNumeroIad = new RelatorioSondagemAnaliticoNumeroIadDto(filtro.TipoSondagem)
                {
                    Dre = dre.Nome,
                    DreSigla = dre.Abreviacao,
                    AnoLetivo = filtro.AnoLetivo,
                    Periodo = filtro.Periodo
                };

                relatorioSondagemAnaliticoNumeroIad.ColunasDoCabecalho.AddRange(cabecalho);

                var listaUes = await ueRepository.ObterPorCodigos(consultarDados.Where(x => x.CodigoDre == dre.Codigo && x.CodigoUe != null).Select(x => x.CodigoUe).Distinct().ToArray());

                var turmasDaDre = await turmaRepository.ObterTurmasPorUeAnosModalidadesAnoLetivoESemestre(listaUes.Select(x => x.Codigo).ToArray(), perguntasPorAno?.Select(p=> p.Key.AnoTurma)?.Distinct().ToArray(), modalidades.ToArray(), filtro.AnoLetivo, null);

                foreach (var ue in listaUes.OrderBy(ue => ue.TituloTipoEscolaNome))
                {
                    var turmasPorUe = turmasDaDre?.Where(t => t.UeId == ue.Id).ToList();

                    var totalDeAlunosPorAno = new List<TotalAlunosAnoTurmaDto>();//(await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(),
                                                                                 //   periodoFixo.DataInicio, periodoFixo.DataFim,
                                                                                 // ue.Codigo, dre.Codigo)).ToList();

                    foreach (var anoTurmaItem in perguntasPorAno)
                    {
                        var quantidadeTurmasPorAnoUe = turmasPorUe?.Where(t => t.Ano == anoTurmaItem.Key.AnoTurma)?.Count() ?? 0;

                        var totalDeAlunos = totalDeAlunosPorAno.Where(x => x.AnoTurma == anoTurmaItem.Key.AnoTurma.ToString()).Select(x => x.QuantidadeAluno).Sum();
                        var perguntasRespostasUe = consultarDados.Where(x => x.CodigoUe == ue.Codigo &&
                                                                        x.AnoTurma == anoTurmaItem.Key.AnoTurma &&
                                                                        x.OrdemPergunta == anoTurmaItem.Key.OrdemPergunta &&
                                                                        x.RespostaDescricao != DESCRICAO_SEMPREENCHIMENTO).ToList();

                        totalDeAlunos = ObterTotalAlunosOuTotalRespostas_NumerosIAD(perguntasRespostasUe, totalDeAlunos);

                        var relatorioSondagemAnaliticoNumero = relatorioSondagemAnaliticoNumeroIad.Respostas.Where(x => x.Ano.ToString() == anoTurmaItem.Key.AnoTurma && x.Ue == ue.TituloTipoEscolaNome).FirstOrDefault();
                        if (relatorioSondagemAnaliticoNumero == null)
                        {
                            relatorioSondagemAnaliticoNumero = new RespostaSondagemAnaliticaNumeroIadDto()
                            {
                                TotalDeAlunos = totalDeAlunos,
                                Ano = int.Parse(anoTurmaItem.Key.AnoTurma),
                                TotalDeTurma = quantidadeTurmasPorAnoUe,
                                Ue = ue.TituloTipoEscolaNome
                            };
                            relatorioSondagemAnaliticoNumeroIad.Respostas.Add(relatorioSondagemAnaliticoNumero);
                        }


                        MapearRespostaNumeros(perguntasRespostasUe, anoTurmaItem.Key.OrdemPergunta, anoTurmaItem.Key.PerguntaDescricao,
                                              totalDeAlunos, relatorioSondagemAnaliticoNumero.Respostas
                                              , relatorioSondagemAnaliticoNumeroIad.ColunasDoCabecalho);

                    }
                }
                retorno.Add(relatorioSondagemAnaliticoNumeroIad);
            }
            return retorno;
        }

        private void MapearRespostaNumeros(List<PerguntaRespostaOrdemDto> perguntasRespostasUe, int ordermPergunta, string perguntaDescricao, int totalDeAlunos,
            List<RespostaSondagemAnaliticaDto> respostas, List<CabecalhoSondagemAnaliticaDto> colunasDoCabecalho)
        {
            var perguntas = perguntasRespostasUe;
            foreach (var cabec in colunasDoCabecalho)
            {
                var naoExiste = cabec.SubCabecalhos?.Count(x => x.Descricao == DESCRICAO_SEMPREENCHIMENTO) == 0;
                if (naoExiste)
                    cabec.SubCabecalhos.Add(new SubCabecalhoSondagemAnaliticaDto { Descricao = DESCRICAO_SEMPREENCHIMENTO, IdPerguntaResposta = $"{cabec.Ordem}_{cabec.Descricao}_{DESCRICAO_SEMPREENCHIMENTO}", Ordem = cabec.SubCabecalhos.Count() + 1 });
            }
            var cabecalhoRespostas = colunasDoCabecalho.Where(x => x.Ordem == ordermPergunta && x.Descricao == perguntaDescricao).FirstOrDefault().SubCabecalhos;


            foreach (var cabecalhoResposta in cabecalhoRespostas)
            {
                var resposta = perguntas.Where(x => x.OrdemResposta == cabecalhoResposta.Ordem);
                if (cabecalhoResposta.Descricao == DESCRICAO_SEMPREENCHIMENTO)
                {
                    var totalRespostas = perguntas?.Sum(c => c.QtdRespostas) ?? 0;
                    var semPrenechimento = (totalDeAlunos >= totalRespostas ? totalDeAlunos - totalRespostas : 0);
                    respostas.Add(new RespostaSondagemAnaliticaDto
                    {
                        IdPerguntaResposta = $"{ordermPergunta}_{perguntaDescricao}_{cabecalhoResposta.Descricao}",
                        Valor = semPrenechimento,
                    });

                }
                else
                {
                    respostas.Add(new RespostaSondagemAnaliticaDto
                    {
                        IdPerguntaResposta = $"{ordermPergunta}_{perguntaDescricao}_{cabecalhoResposta.Descricao}",
                        Valor = resposta?.Sum(x => x.QtdRespostas) ?? 0,
                    });
                }

            }
        }

        private async Task<IEnumerable<PerguntaRespostaOrdemDto>> ConsultaMatematicaNumerosAutoral(FiltroRelatorioAnaliticoSondagemDto filtro, string periodoId, ProficienciaSondagemEnum proficienciaSondagemEnum)
        {
            if (proficienciaSondagemEnum == ProficienciaSondagemEnum.Numeros)
            {
                return filtro.AnoLetivo >= ANO_ESCOLAR_2022 ? await sondagemRelatorioRepository.MatematicaIADNumeroBimestre(filtro.AnoLetivo, SondagemComponenteCurricular.MATEMATICA, filtro.Periodo, filtro.UeCodigo, filtro.DreCodigo, proficienciaSondagemEnum)
                                                     : await sondagemRelatorioRepository.MatematicaNumerosAntes2022(filtro.AnoLetivo, filtro.Periodo, filtro.UeCodigo, filtro.DreCodigo, periodoId);
            }
            else
            {
                return filtro.AnoLetivo >= ANO_ESCOLAR_2022 ? await sondagemRelatorioRepository.MatematicaIADNumeroBimestre(filtro.AnoLetivo, SondagemComponenteCurricular.MATEMATICA, filtro.Periodo, filtro.UeCodigo, filtro.DreCodigo, proficienciaSondagemEnum)
                                     : await sondagemRelatorioRepository.MatematicaIADAntes2022(filtro.AnoLetivo, SondagemComponenteCurricular.MATEMATICA, filtro.Periodo, filtro.UeCodigo, filtro.DreCodigo, periodoId);
            }
        }

        private async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoCampoAditivoMultiplicativo(FiltroRelatorioAnaliticoSondagemDto filtro, ProficienciaSondagemEnum proficiencia)
        {
            var retorno = new List<RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Periodo, filtro.AnoLetivo, filtro.TipoSondagem);
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


            var perguntasPorAno = dtoConsultaDados.Where(x => x.AnoTurma != null).GroupBy(p => new { p.AnoTurma, p.OrdemPergunta, p.PerguntaDescricao }).ToList().OrderBy(x => x.Key.AnoTurma).ThenBy(d => d.Key.OrdemPergunta);
            var listaDres = await dreRepository.ObterPorCodigos(dtoConsultaDados.Where(x => x.CodigoDre != null).Select(x => x.CodigoDre).Distinct().ToArray());
            foreach (var dre in listaDres.OrderBy(x => x.Abreviacao))
            {
                var perguntas = new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto();
                var relatorioSondagemAnaliticoCampoAditivoMultiplicativoDto = new RelatorioSondagemAnaliticoCampoAditivoMultiplicativoDto(filtro.TipoSondagem)
                {
                    Dre = dre.Nome,
                    DreSigla = dre.Abreviacao,
                    AnoLetivo = filtro.AnoLetivo,
                    Periodo = filtro.Periodo
                };

                var listaUes = await ueRepository.ObterPorCodigos(dtoConsultaDados.Where(x => x.CodigoDre == dre.Codigo && x.CodigoUe != null).Select(x => x.CodigoUe).Distinct().ToArray());
                foreach (var ue in listaUes.OrderBy(ue => ue.TituloTipoEscolaNome))
                {
                    var totalDeAlunosPorAno = new List<TotalAlunosAnoTurmaDto>();//(await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(),
                                                                                 //      periodoFixo.DataInicio, periodoFixo.DataFim,
                                                                                 //      ue.Codigo, dre.Codigo)).ToList();

                    var totalTurmas = await ObterQuantidadeTurmaPorAno(ue.Codigo, filtro.AnoLetivo);

                    foreach (var anoTurmaItem in perguntasPorAno)
                    {
                        int quantidadeTurmas = totalTurmas?.FirstOrDefault(t => t.Ano == anoTurmaItem.Key.AnoTurma).Quantidade ?? 0;
                        var descricaoPergunta = ObterDescricaoPergunta(proficiencia, anoTurmaItem.Key.PerguntaDescricao, anoTurmaItem.Key.OrdemPergunta, int.Parse(anoTurmaItem.Key.AnoTurma));
                        if (string.IsNullOrEmpty(descricaoPergunta)) continue;

                        var totalDeAlunos = totalDeAlunosPorAno.Where(x => x.AnoTurma == anoTurmaItem.Key.AnoTurma).Select(x => x.QuantidadeAluno).Sum();

                        var perguntasRespostasUe = dtoConsultaDados.Where(x => x.CodigoUe == ue.Codigo &&
                                                                        x.AnoTurma == anoTurmaItem.Key.AnoTurma &&
                                                                        x.OrdemPergunta == anoTurmaItem.Key.OrdemPergunta &&
                                                                        x.RespostaDescricao != DESCRICAO_SEMPREENCHIMENTO).ToList();

                        totalDeAlunos = ObterTotalAlunosOuTotalRespostas_AditivoMultiplicativo(perguntasRespostasUe, totalDeAlunos);

                        var respostaSondagemAnaliticoCampoAditivoMultiplicativoDto = relatorioSondagemAnaliticoCampoAditivoMultiplicativoDto.Respostas.Where(x => x.Ano == int.Parse(anoTurmaItem.Key.AnoTurma) && x.Ue == ue.TituloTipoEscolaNome).FirstOrDefault();
                        if (respostaSondagemAnaliticoCampoAditivoMultiplicativoDto == null)
                        {
                            respostaSondagemAnaliticoCampoAditivoMultiplicativoDto = new RespostaSondagemAnaliticoCampoAditivoMultiplicativoDto()
                            {
                                TotalDeAlunos = totalDeAlunos,
                                Ano = int.Parse(anoTurmaItem.Key.AnoTurma),
                                TotalDeTurma = quantidadeTurmas,
                                Ue = ue.TituloTipoEscolaNome
                            };
                            relatorioSondagemAnaliticoCampoAditivoMultiplicativoDto.Respostas.Add(respostaSondagemAnaliticoCampoAditivoMultiplicativoDto);
                        }


                        var respostaSondagemAnaliticoOrdem = ObterRespostaSondagemAnaliticoOrdemDto(perguntasRespostasUe, anoTurmaItem.Key.AnoTurma,
                                                                                                    anoTurmaItem.Key.OrdemPergunta, descricaoPergunta,
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

        private int ObterTotalAlunosOuTotalRespostas_AditivoMultiplicativo(List<PerguntaRespostaOrdemDto> perguntasRespostasUe, int totalAlunos)
        {
            var totalRespostas = 0;
            if (perguntasRespostasUe != null && perguntasRespostasUe.Any())
                totalRespostas = perguntasRespostasUe.GroupBy(pergunta => pergunta.SubPerguntaDescricao).Select(subpergunta => subpergunta.Sum(x => x.QtdRespostas)).Max();
            if (totalAlunos < totalRespostas)
                return totalRespostas;
            return totalAlunos;
        }

        private int ObterTotalAlunosOuTotalRespostas_NumerosIAD(List<PerguntaRespostaOrdemDto> perguntasRespostasUe, int totalAlunos)
        {
            var totalRespostas = 0;
            if (perguntasRespostasUe != null && perguntasRespostasUe.Any())
                totalRespostas = perguntasRespostasUe.Sum(x => x.QtdRespostas);
            if (totalAlunos < totalRespostas)
                return totalRespostas;
            return totalAlunos;
        }

        private RespostaOrdemMatematicaDto ObterRespostaSondagemAnaliticoOrdemDto(List<PerguntaRespostaOrdemDto> perguntasRepostasUe,
                                                                                  string anoTurma, int ordemPergunta, string descricaoPergunta, int totalDeAlunos)
        {
            var respostaSondagemAnaliticoIdeiaDto = new RespostaMatematicaDto();
            var respostaSondagemAnaliticoResultadoDto = new RespostaMatematicaDto();

            var perguntasRespostas = perguntasRepostasUe.Where(x => x.SubPerguntaDescricao == "Ideia").ToList();
            respostaSondagemAnaliticoIdeiaDto.Acertou = perguntasRespostas?.Where(x => x.RespostaDescricao == "Acertou").Select(x => x.QtdRespostas).Sum() ?? 0;
            respostaSondagemAnaliticoIdeiaDto.Errou = perguntasRespostas?.Where(x => x.RespostaDescricao == "Errou").Select(x => x.QtdRespostas).Sum() ?? 0;
            respostaSondagemAnaliticoIdeiaDto.NaoResolveu = perguntasRespostas?.Where(x => x.RespostaDescricao == "Não resolveu").Select(x => x.QtdRespostas).Sum() ?? 0;
            var totalRespostas = respostaSondagemAnaliticoIdeiaDto.Acertou +
                                    respostaSondagemAnaliticoIdeiaDto.Errou +
                                    respostaSondagemAnaliticoIdeiaDto.NaoResolveu;
            respostaSondagemAnaliticoIdeiaDto.SemPreenchimento = totalDeAlunos >= totalRespostas ? totalDeAlunos - totalRespostas : 0;

            perguntasRespostas = perguntasRepostasUe?.Where(x => x.SubPerguntaDescricao == "Resultado").ToList();
            respostaSondagemAnaliticoResultadoDto.Acertou = perguntasRespostas?.Where(x => x.RespostaDescricao == "Acertou").Select(x => x.QtdRespostas).Sum() ?? 0;
            respostaSondagemAnaliticoResultadoDto.Errou = perguntasRespostas?.Where(x => x.RespostaDescricao == "Errou").Select(x => x.QtdRespostas).Sum() ?? 0;
            respostaSondagemAnaliticoResultadoDto.NaoResolveu = perguntasRespostas?.Where(x => x.RespostaDescricao == "Não resolveu").Select(x => x.QtdRespostas).Sum() ?? 0;
            totalRespostas = respostaSondagemAnaliticoResultadoDto.Acertou +
                             respostaSondagemAnaliticoResultadoDto.Errou +
                             respostaSondagemAnaliticoResultadoDto.NaoResolveu;
            respostaSondagemAnaliticoResultadoDto.SemPreenchimento = totalDeAlunos >= totalRespostas ? totalDeAlunos - totalRespostas : 0;

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