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
        private readonly string DESCRICAO_SEMPREENCHIMENTO = "Sem Preenchimento";
        private readonly int ANO_ESCOLAR_2022 = 2022;


        public SondagemAnaliticaRepository(VariaveisAmbiente variaveisAmbiente, IDreRepository dreRepository, IUeRepository ueRepository
            , ISondagemRelatorioRepository sondagemRelatorioRepository, ITurmaRepository turmaRepository)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
            this.sondagemRelatorioRepository = sondagemRelatorioRepository ?? throw new ArgumentNullException(nameof(sondagemRelatorioRepository));
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

        public async Task<IEnumerable<TotalRespostasAnaliticoLeituraDto>> ObterRespostasRelatorioAnaliticoDeLeitura(FiltroRelatorioAnaliticoSondagemDto filtro, bool valorSemPreenchimento)
        {
            var sql = ConsultaLeituraLinguaPortuguesaPrimeiroAoTerceiroAno(filtro, valorSemPreenchimento);
            var parametros = new { dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conexao.QueryAsync<TotalRespostasAnaliticoLeituraDto>(sql, parametros);
            }
        }

        public async Task<IEnumerable<TotalRespostasAnaliticoEscritaDto>> ObterRespostaRelatorioAnaliticoDeEscrita(FiltroRelatorioAnaliticoSondagemDto filtro, bool valorSemPreenchimento)
        {
            var sql = ConsultaEscritaLinguaPortuguesaPrimeiroAoTerceiroAno(filtro, valorSemPreenchimento);
            var parametros = new { dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conexao.QueryAsync<TotalRespostasAnaliticoEscritaDto>(sql, parametros);
            }
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

        private string ConsultaEscritaLinguaPortuguesaPrimeiroAoTerceiroAno(FiltroRelatorioAnaliticoSondagemDto filtroRelatorioAnaliticoSondagemDto, bool valorSemPreenchimento)
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
            if (valorSemPreenchimento)
                sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"reading{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'SemPreenchimento') as  SemPreenchimento");
            else
                sql.AppendLine($"	      count(pp.\"studentCodeEol\") filter (where ((pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" is null) or (trim(pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\") = ''))) as SemPreenchimento");
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

        private int ObterTotalAlunosOuTotalRespostas_NumerosIAD(List<PerguntaRespostaOrdemDto> perguntasRespostasUe, int totalAlunos)
        {
            var totalRespostas = 0;
            if (perguntasRespostasUe != null && perguntasRespostasUe.Any())
                totalRespostas = perguntasRespostasUe.Sum(x => x.QtdRespostas);
            if (totalAlunos < totalRespostas)
                return totalRespostas;
            return totalAlunos;
        }
    }
}