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

namespace SME.SR.Data
{
    public class SondagemAnaliticaRepository : ISondagemAnaliticaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        private readonly IAlunoRepository alunoRepository;
        private readonly IDreRepository dreRepository;
        private readonly IUeRepository ueRepository;
        private readonly ISondagemRelatorioRepository sondagemRelatorioRepository;
        private readonly string TURMA_TERCEIRO_ANO = "3";

        public SondagemAnaliticaRepository(VariaveisAmbiente variaveisAmbiente, IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository
            , ISondagemRelatorioRepository sondagemRelatorioRepository)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
            this.sondagemRelatorioRepository = sondagemRelatorioRepository ?? throw new ArgumentNullException(nameof(sondagemRelatorioRepository));
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoCapacidadeDeLeitura(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var periodo = await ObterPeriodoSondagem(filtro.Periodo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var perguntas = new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto();
            var modalidades = new List<int> {5, 13};
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);

            var realizarConsulta = await sondagemRelatorioRepository.ConsolidadoCapacidadeLeitura(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.CAPACIDADE_DE_LEITURA,
                PeriodoId = periodo.Id
            });

            var relatorioAgrupadoPorAno = realizarConsulta.Where(x => x.AnoTurma != null).GroupBy(p => p.AnoTurma).ToList();
            var totalDeAlunosPorAno = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo)).ToList();

            foreach (var anoTurmaItem in relatorioAgrupadoPorAno)
            {
                var totalDeTurma = anoTurmaItem.Select(x => x.CodigoTurma)!.Distinct()!.Count();
                var listaDtoOrdemNarrar = new List<RespostaCapacidadeDeLeituraDto>();
                var listaDtoOrdemRelatar = new List<RespostaCapacidadeDeLeituraDto>();
                var listaDtoOrdemArgumentar = new List<RespostaCapacidadeDeLeituraDto>();
                var respostaSondagemAnaliticoCapacidadeDeLeituraDtoLista = new List<RespostaSondagemAnaliticoCapacidadeDeLeituraDto>();

                var agrupamentoPorOrderm = anoTurmaItem.GroupBy(x => x.Ordem);


                var ordemNarrarLista = agrupamentoPorOrderm.Where(x => x.Key == OrdemSondagem.ORDEM_DO_NARRAR).ToList();
                var ordemRelatarLista = agrupamentoPorOrderm.Where(x => x.Key == OrdemSondagem.ORDEM_DO_RELATAR).ToList();
                var ordemArgumentarLista = agrupamentoPorOrderm.Where(x => x.Key == OrdemSondagem.ORDEM_DO_ARGUMENTAR).ToList();

                var totalDeAlunos = totalDeAlunosPorAno.Where(x => x.AnoTurma == anoTurmaItem.Key).Select(x => x.QuantidadeAluno).Sum();

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

                    resp.Ue = ue.Nome;
                    resp.Ano = int.Parse(anoTurmaItem.Key);
                    resp.TotalDeTurma = totalDeTurma;
                    resp.TotalDeAlunos = totalDeAlunos;
                    respostaSondagemAnaliticoCapacidadeDeLeituraDtoLista.Add(resp);
                }

                foreach (var respost in respostaSondagemAnaliticoCapacidadeDeLeituraDtoLista)
                {
                    perguntas.Respostas.Add(respost);
                }
            }


            perguntas.Dre = dre.Nome;
            perguntas.DreSigla = dre.Abreviacao;
            retorno.Add(perguntas);

            return retorno;
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

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoEscrita(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Periodo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> {5, 13};
            var relatorioSondagemAnaliticoEscritaDto = new RelatorioSondagemAnaliticoEscritaDto();
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);
            var sql = ConsultaEscritaLinguaPortuguesaPrimeiroAoTerceiroAno(filtro);
            var parametros = new {dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma};
            IEnumerable<TotalRespostasAnaliticoEscritaDto> dtoConsulta = null;
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                dtoConsulta = (await conexao.QueryAsync<TotalRespostasAnaliticoEscritaDto>(sql, parametros)).ToList();
            }

            var agrupamentoPorAnoTurma = dtoConsulta.GroupBy(x => x.AnoTurma).ToList();
            var quantidadeTotalAlunosPorAno = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo)).ToList();

            foreach (var anoTurma in agrupamentoPorAnoTurma)
            {
                var quantidadeTotalAlunos = 0;
                var turmasComSondagem = anoTurma.Select(x => x.TurmaCodigo).ToList();
                if (turmasComSondagem.Any())
                    quantidadeTotalAlunos = await alunoRepository.ObterTotalAlunosAtivosPorTurmaEPeriodo(turmasComSondagem.ToArray(), periodoFixo.DataFim);
                else
                    quantidadeTotalAlunos = quantidadeTotalAlunosPorAno.Where(x => x.AnoTurma == anoTurma.Key).Select(x => x.QuantidadeAluno).Sum();

                var calculoComAlunos = anoTurma.Key == TURMA_TERCEIRO_ANO
                    ? (quantidadeTotalAlunos - (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum()
                                                + anoTurma.Select(x => x.Nivel4).Sum()))
                    : (quantidadeTotalAlunos - (anoTurma.Select(x => x.PreSilabico).Sum() + anoTurma.Select(x => x.SilabicoSemValor).Sum()
                                                                                          + anoTurma.Select(x => x.SilabicoComValor).Sum()
                                                                                          + anoTurma.Select(x => x.SilabicoAlfabetico).Sum()
                                                                                          + anoTurma.Select(x => x.Alfabetico).Sum()));
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
                    SemPreenchimento = calculoComAlunos >= 0 ? calculoComAlunos : anoTurma.Select(x => x.SemPreenchimento).Sum(),
                    TotalDeAlunos = quantidadeTotalAlunos,
                    Ano = int.Parse(anoTurma.Key),
                    TotalDeTurma = turmasComSondagem.Count(),
                    Ue = ue.Nome
                };
                relatorioSondagemAnaliticoEscritaDto.Respostas.Add(respostaSondagemAnaliticoEscritaDto);
            }

            relatorioSondagemAnaliticoEscritaDto.Dre = dre.Nome;
            relatorioSondagemAnaliticoEscritaDto.DreSigla = dre.Abreviacao;
            retorno.Add(relatorioSondagemAnaliticoEscritaDto);
            return retorno;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoLeitura(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Periodo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> {5, 13};
            var relatorioSondagemAnaliticoLeituraDto = new RelatorioSondagemAnaliticoLeituraDto();
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);
            var sql = ConsultaLeituraLinguaPortuguesaPrimeiroAoTerceiroAno(filtro);
            var parametros = new {dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma};
            IEnumerable<TotalRespostasAnaliticoLeituraDto> dtoConsulta = null;
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                dtoConsulta = (await conexao.QueryAsync<TotalRespostasAnaliticoLeituraDto>(sql, parametros)).ToList();
            }

            var agrupamentoPorAnoTurma = dtoConsulta.GroupBy(x => x.AnoTurma).ToList();
            var quantidadeTotalAlunosPorAno = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo)).ToList();
            foreach (var anoTurma in agrupamentoPorAnoTurma)
            {
                var quantidadeTotalAlunos = 0;
                var turmasComSondagem = anoTurma.Select(x => x.TurmaCodigo).ToList();
                if (turmasComSondagem.Any())
                    quantidadeTotalAlunos = await alunoRepository.ObterTotalAlunosAtivosPorTurmaEPeriodo(turmasComSondagem.ToArray(), periodoFixo.DataFim);
                else
                    quantidadeTotalAlunos = quantidadeTotalAlunosPorAno.Where(x => x.AnoTurma == anoTurma.Key).Select(x => x.QuantidadeAluno).Sum();

                var calculoComAlunos = (quantidadeTotalAlunos - (anoTurma.Select(x => x.Nivel1).Sum() + anoTurma.Select(x => x.Nivel2).Sum() + anoTurma.Select(x => x.Nivel3).Sum() + anoTurma.Select(x => x.Nivel4).Sum()));

                var respostaSondagemAnaliticoLeituraDto = new RespostaSondagemAnaliticoLeituraDto
                {
                    Nivel1 = anoTurma.Select(x => x.Nivel1).Sum(),
                    Nivel2 = anoTurma.Select(x => x.Nivel2).Sum(),
                    Nivel3 = anoTurma.Select(x => x.Nivel3).Sum(),
                    Nivel4 = anoTurma.Select(x => x.Nivel4).Sum(),
                    SemPreenchimento = calculoComAlunos >= 0 ? calculoComAlunos : anoTurma.Select(x => x.SemPreenchimento).Sum(),
                    TotalDeAlunos = quantidadeTotalAlunos,
                    Ano = int.Parse(anoTurma.Key),
                    TotalDeTurma = turmasComSondagem.Count(),
                    Ue = ue.Nome
                };
                relatorioSondagemAnaliticoLeituraDto.Respostas.Add(respostaSondagemAnaliticoLeituraDto);
            }

            relatorioSondagemAnaliticoLeituraDto.Dre = dre.Nome;
            relatorioSondagemAnaliticoLeituraDto.DreSigla = dre.Abreviacao;
            retorno.Add(relatorioSondagemAnaliticoLeituraDto);
            return retorno;
        }

        private async Task<PeriodoSondagem> ObterPeriodoSondagem(int bimestre)
        {
            var termo = @$"{bimestre}° Bimestre";

            var sql = " select * from \"Periodo\" p where p.\"Descricao\" = @termo";

            using (var conn = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conn.QueryFirstOrDefaultAsync<PeriodoSondagem>(sql, new {termo});
            }
        }

        private async Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem(int anoLetivo, string periodoId)
        {
            var sql = " select * from \"PeriodoFixoAnual\" pfa where pfa.\"PeriodoId\" = @periodoId and \"Ano\" = @anoLetivo ";
            var parametros = new {periodoId, anoLetivo};
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

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoLeituraDeVozAlta(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var modalidades = new List<int> {5, 13};
            var periodo = await ObterPeriodoSondagem(filtro.Periodo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);
            var totalDeAlunosPorAnoTurma = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo)).ToList();
            var perguntas = new RelatorioSondagemAnaliticoLeituraDeVozAltaDto();

            var consulta = (await sondagemRelatorioRepository.ObterDadosProducaoTexto(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.LEITURA_EM_VOZ_ALTA,
                PeriodoId = periodo.Id
            })).ToList();

            var relatorioAgrupadoPorAnoTurma = consulta.GroupBy(x => x.AnoTurma).ToList();

            foreach (var anoTurmaItem in relatorioAgrupadoPorAnoTurma)
            {
                var alunoNaSondagem = anoTurmaItem.GroupBy(x => x.CodigoAluno);
                var tumasNaSondagem = anoTurmaItem.GroupBy(x => x.CodigoTurma);
                var quantidadeAlunoNaSondagem = alunoNaSondagem.Select(x => x.Key).Count();
                var totalDeTurma = tumasNaSondagem.Select(x => x.Key).Count();
                var totalDeAlunos = totalDeAlunosPorAnoTurma.Where(x => x.AnoTurma == anoTurmaItem.Key).Select(x => x.QuantidadeAluno).Sum();
                var respostas = new RespostaSondagemAnaliticoLeituraDeVozAltaDto
                {
                    NaoConseguiuOuNaoQuisLer = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoConseguiuOuNaoQuisLer),
                    LeuComMuitaDificuldade = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComMuitaDificuldade),
                    LeuComAlgumaFluencia = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComAlgumaFluencia),
                    LeuComFluencia = anoTurmaItem.Count(p => p.Pergunta == PerguntaDescricaoSondagem.LeuComFluencia),
                    SemPreenchimento = totalDeAlunos - quantidadeAlunoNaSondagem,
                    Ano = int.Parse(anoTurmaItem.Key),
                    TotalDeTurma = totalDeTurma,
                    TotalDeAlunos = totalDeAlunos,
                    Ue = ue.Nome
                };

                perguntas.Respostas.Add(respostas);
            }

            perguntas.DreSigla = dre.Abreviacao;
            perguntas.Dre = dre.Nome;
            retorno.Add(perguntas);

            return retorno;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoProducaoDeTexto(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var modalidades = new List<int> {5, 13};
            var periodo = await ObterPeriodoSondagem(filtro.Periodo);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);
            var totalDeAlunosPorAnoTurma = (await alunoRepository.ObterTotalAlunosAtivosPorPeriodoEAnoTurma(filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo)).ToList();
            var perguntas = new RelatorioSondagemAnaliticoProducaoDeTextoDto();


            var consulta = (await sondagemRelatorioRepository.ObterDadosProducaoTexto(new RelatorioPortuguesFiltroDto
            {
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.PRODUCAO_DE_TEXTO,
                PeriodoId = periodo.Id
            })).ToList();

            var relatorioAgrupadoPorAnoTurma = consulta.GroupBy(x => x.AnoTurma).ToList();
            foreach (var anoTurmaItem in relatorioAgrupadoPorAnoTurma)
            {
                var alunoNaSondagem = anoTurmaItem.GroupBy(x => x.CodigoAluno);
                var tumasNaSondagem = anoTurmaItem.GroupBy(x => x.CodigoTurma);
                var quantidadeAlunoNaSondagem = alunoNaSondagem.Select(x => x.Key).Count();
                var totalDeTurma = tumasNaSondagem.Select(x => x.Key).Count();
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
                    TotalDeTurma = totalDeTurma,
                    TotalDeAlunos = totalDeAlunos,
                    Ue = ue.Nome
                };

                perguntas.Respostas.Add(respostas);
            }

            perguntas.DreSigla = dre.Abreviacao;
            perguntas.Dre = dre.Nome;
            retorno.Add(perguntas);
            return retorno;
        }
    }
}