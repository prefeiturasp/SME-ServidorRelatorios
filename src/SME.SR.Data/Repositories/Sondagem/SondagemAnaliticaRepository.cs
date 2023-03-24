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
            var periodo = await ObterPeriodoSondagem(filtro.Bimestre);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var perguntas = new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto();
            var modalidades = new List<int> {5, 13};
            var totalDeAlunos = await alunoRepository.ObterTotalAlunosAtivosPorPeriodo(filtro.AnoTurma, filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo);
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);

            var realizarConsulta = await sondagemRelatorioRepository.ConsolidadoCapacidadeLeitura(new RelatorioPortuguesFiltroDto
            {
                AnoEscolar = int.Parse(filtro.AnoTurma),
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.CAPACIDADE_DE_LEITURA,
                PeriodoId = periodo.Id
            });

            var relatorioAgrupado = realizarConsulta.GroupBy(p => p.Ordem).ToList();
            var ordemNarrar = new RespostaCapacidadeDeLeituraDto();
            var ordemDoRelatar = new RespostaCapacidadeDeLeituraDto();
            var ordemDoArgumentar = new RespostaCapacidadeDeLeituraDto();
            var totalDeTurma = realizarConsulta?.ToList() != null ? realizarConsulta.Select(x =>x.CodigoTurma)!.Distinct()!.Count() : 0;
            foreach (var ordemItem in relatorioAgrupado)
            {
                var relatorioAgrupadoPergunta = ordemItem.GroupBy(x => x.PerguntaDescricao).ToList();
                if (ordemItem.Key == OrdemSondagem.ORDEM_DO_NARRAR)
                {
                    ordemNarrar.Localizacao = MapearOrdemDoNarrarLocalizacao(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                    ordemNarrar.Inferencia = MapearOrdemDoNarrarInferencia(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                    ordemNarrar.Reflexao = MapearOrdemDoNarrarReflexao(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                }

                if (ordemItem.Key == OrdemSondagem.ORDEM_DO_RELATAR)
                {
                    ordemDoRelatar.Localizacao = MapearOrdemDoNarrarLocalizacao(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                    ordemDoRelatar.Inferencia = MapearOrdemDoNarrarInferencia(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                    ordemDoRelatar.Reflexao = MapearOrdemDoNarrarReflexao(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                }

                if (ordemItem.Key == OrdemSondagem.ORDEM_DO_ARGUMENTAR)
                {
                    ordemDoArgumentar.Localizacao = MapearOrdemDoNarrarLocalizacao(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                    ordemDoArgumentar.Inferencia = MapearOrdemDoNarrarInferencia(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                    ordemDoArgumentar.Reflexao = MapearOrdemDoNarrarReflexao(ordemItem, totalDeAlunos, relatorioAgrupadoPergunta);
                }
            }

            perguntas.Respostas.Add(new RespostaSondagemAnaliticoCapacidadeDeLeituraDto()
            {
                OrdemDoNarrar = ordemNarrar,
                OrdemDoRelatar = ordemDoRelatar,
                OrdemDoArgumentar = ordemDoArgumentar,
                Ue = ue.Nome,
                TotalDeAlunos = totalDeAlunos,
                TotalDeTurma = totalDeTurma,
                Ano = int.Parse(filtro.AnoTurma)
            });
            perguntas.Dre = dre.Nome;
            perguntas.DreSigla = dre.Abreviacao;
            retorno.Add(perguntas);

            return retorno;
        }

        private ItemRespostaCapacidadeDeLeituraDto MapearOrdemDoNarrarLocalizacao(IGrouping<string, OrdemPerguntaRespostaDto> ordemItem, int totalDeAlunos, List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPergunta)
        {
            var localizacao = new ItemRespostaCapacidadeDeLeituraDto();
            var perguntaItemLocalizacao = relatorioAgrupadoPergunta?.Where(x => x.Key == PerguntaDescricaoSondagem.Localizacao).ToList();
            var totalRespostas = perguntaItemLocalizacao !=null ? perguntaItemLocalizacao.Select(s => s.Sum(a => a.QtdRespostas)).ToList().Sum():0;
            totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

            localizacao.Adequada = perguntaItemLocalizacao?.FirstOrDefault() != null ? perguntaItemLocalizacao!.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Adequada).Select(x => x.QtdRespostas).Sum() : 0;
            localizacao.Inadequada = perguntaItemLocalizacao?.FirstOrDefault() != null ? perguntaItemLocalizacao!.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Inadequada).Select(x => x.QtdRespostas).Sum() : 0;
            localizacao.NaoResolveu = perguntaItemLocalizacao?.FirstOrDefault() != null ? perguntaItemLocalizacao!.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.NaoResolveu).Select(x => x.QtdRespostas).Sum() : 0;
            localizacao.SemPreenchimento = totalDeAlunos - totalRespostas;

            return localizacao;
        }

        private ItemRespostaCapacidadeDeLeituraDto MapearOrdemDoNarrarInferencia(IGrouping<string, OrdemPerguntaRespostaDto> ordemItem, int totalDeAlunos, List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPergunta)
        {
            var inferencia = new ItemRespostaCapacidadeDeLeituraDto();
            var perguntaIteInferencia = relatorioAgrupadoPergunta?.Where(x => x.Key == PerguntaDescricaoSondagem.Inferencia).ToList();
            var totalRespostas = perguntaIteInferencia != null ? perguntaIteInferencia.Select(s => s.Sum(a => a.QtdRespostas)).ToList().Sum(): 0;
            totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

            inferencia.Adequada = perguntaIteInferencia?.FirstOrDefault() != null ? perguntaIteInferencia.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Adequada).Select(x => x.QtdRespostas).Sum() : 0;
            inferencia.Inadequada = perguntaIteInferencia?.FirstOrDefault() != null ? perguntaIteInferencia.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Inadequada).Select(x => x.QtdRespostas).Sum() : 0;
            inferencia.NaoResolveu = perguntaIteInferencia?.FirstOrDefault() != null ? perguntaIteInferencia.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.NaoResolveu).Select(x => x.QtdRespostas).Sum() : 0;
            inferencia.SemPreenchimento = totalDeAlunos - totalRespostas;

            return inferencia;
        }

        private ItemRespostaCapacidadeDeLeituraDto MapearOrdemDoNarrarReflexao(IGrouping<string, OrdemPerguntaRespostaDto> ordemItem, int totalDeAlunos, List<IGrouping<string, OrdemPerguntaRespostaDto>> relatorioAgrupadoPergunta)
        {
            var reflexao = new ItemRespostaCapacidadeDeLeituraDto();
            var perguntaItemReflexao = relatorioAgrupadoPergunta?.Where(x => x.Key == PerguntaDescricaoSondagem.Reflexao).ToList();
            var totalRespostas = perguntaItemReflexao != null ? perguntaItemReflexao.Select(s => s.Sum(a => a.QtdRespostas)).ToList().Sum() : 0;
            totalDeAlunos = totalDeAlunos >= totalRespostas ? totalDeAlunos : totalRespostas;

            reflexao.Adequada = perguntaItemReflexao?.FirstOrDefault() !=null ? perguntaItemReflexao.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Adequada).Select(x => x.QtdRespostas).Sum() : 0;
            reflexao.Inadequada = perguntaItemReflexao?.FirstOrDefault() !=null ? perguntaItemReflexao.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.Inadequada).Select(x => x.QtdRespostas).Sum() : 0;
            reflexao.NaoResolveu = perguntaItemReflexao?.FirstOrDefault() !=null ? perguntaItemReflexao.FirstOrDefault()!.Where(f => f.RespostaDescricao == RespostaDescricaoSondagem.NaoResolveu).Select(x => x.QtdRespostas).Sum() : 0;
            reflexao.SemPreenchimento = totalDeAlunos - totalRespostas;

            return reflexao;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoEscrita(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Bimestre);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> {5, 13};
            var relatorioSondagemAnaliticoEscritaDto = new RelatorioSondagemAnaliticoEscritaDto();
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);
            var campoTabelaFiltroBimestre = @$"writing{filtro.Bimestre}B";
            var sql = ConsultaEscritaLinguaPortuguesaPrimeiroAoTerceiroAno(campoTabelaFiltroBimestre);
            var parametros = new {dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma};
            IEnumerable<TotalRespostasAnaliticoEscritaDto> dtoConsulta = null;
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                dtoConsulta = (await conexao.QueryAsync<TotalRespostasAnaliticoEscritaDto>(sql, parametros)).ToList();
            }

            if (dtoConsulta.Any())
            {
                var quantidadeTotalAlunos = 0;
                var turmasComSondagem = dtoConsulta.Select(x => x.TurmaCodigo).ToList();

                if (turmasComSondagem.Any())
                    quantidadeTotalAlunos = await alunoRepository.ObterTotalAlunosAtivosPorTurmaEPeriodo(turmasComSondagem.ToArray(), periodoFixo.DataFim);
                else
                    quantidadeTotalAlunos = await alunoRepository.ObterTotalAlunosAtivosPorPeriodo(filtro.AnoTurma, filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo);

                var calculoComAlunos = (quantidadeTotalAlunos - (dtoConsulta.Select(x => x.PreSilabico).Sum() + dtoConsulta.Select(x => x.SilabicoSemValor).Sum()
                                                                                                              + dtoConsulta.Select(x => x.SilabicoComValor).Sum()
                                                                                                              + dtoConsulta.Select(x => x.SilabicoAlfabetico).Sum()
                                                                                                              + dtoConsulta.Select(x => x.Alfabetico).Sum()));

                var respostaSondagemAnaliticoEscritaDto = new RespostaSondagemAnaliticoEscritaDto
                {
                    PreSilabico = dtoConsulta.Select(x => x.PreSilabico).Sum(),
                    SilabicoSemValor = dtoConsulta.Select(x => x.SilabicoSemValor).Sum(),
                    SilabicoComValor = dtoConsulta.Select(x => x.SilabicoComValor).Sum(),
                    SilabicoAlfabetico = dtoConsulta.Select(x => x.SilabicoAlfabetico).Sum(),
                    Alfabetico = dtoConsulta.Select(x => x.Alfabetico).Sum(),
                    SemPreenchimento = calculoComAlunos >= 0 ? calculoComAlunos : dtoConsulta.Select(x => x.SemPreenchimento).Sum(),
                    TotalDeAlunos = quantidadeTotalAlunos,
                    Ano = int.Parse(filtro.AnoTurma),
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
            var periodo = await ObterPeriodoSondagem(filtro.Bimestre);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> {5, 13};
            var relatorioSondagemAnaliticoLeituraDto = new RelatorioSondagemAnaliticoLeituraDto();
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);
            var campoTabelaFiltroBimestre = @$"reading{filtro.Bimestre}B";
            var sql = ConsultaLeituraLinguaPortuguesaPrimeiroAoTerceiroAno(campoTabelaFiltroBimestre);
            var parametros = new {dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo.ToString(), anoTurma = filtro.AnoTurma};
            IEnumerable<TotalRespostasAnaliticoLeituraDto> dtoConsulta = null;
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                dtoConsulta = (await conexao.QueryAsync<TotalRespostasAnaliticoLeituraDto>(sql, parametros)).ToList();
            }

            if (dtoConsulta.Any())
            {
                var quantidadeTotalAlunos = 0;
                var turmasComSondagem = dtoConsulta.Select(x => x.TurmaCodigo).ToList();

                if (turmasComSondagem.Any())
                    quantidadeTotalAlunos = await alunoRepository.ObterTotalAlunosAtivosPorTurmaEPeriodo(turmasComSondagem.ToArray(), periodoFixo.DataFim);
                else
                    quantidadeTotalAlunos = await alunoRepository.ObterTotalAlunosAtivosPorPeriodo(filtro.AnoTurma, filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo);


                var calculoComAlunos = (quantidadeTotalAlunos - (dtoConsulta.Select(x => x.Nivel1).Sum() + dtoConsulta.Select(x => x.Nivel2).Sum() + dtoConsulta.Select(x => x.Nivel3).Sum() + dtoConsulta.Select(x => x.Nivel4).Sum()));

                var respostaSondagemAnaliticoLeituraDto = new RespostaSondagemAnaliticoLeituraDto
                {
                    Nivel1 = dtoConsulta.Select(x => x.Nivel1).Sum(),
                    Nivel2 = dtoConsulta.Select(x => x.Nivel2).Sum(),
                    Nivel3 = dtoConsulta.Select(x => x.Nivel3).Sum(),
                    Nivel4 = dtoConsulta.Select(x => x.Nivel4).Sum(),
                    SemPreenchimento = calculoComAlunos >= 0 ? calculoComAlunos : dtoConsulta.Select(x => x.SemPreenchimento).Sum(),
                    TotalDeAlunos = quantidadeTotalAlunos,
                    Ano = int.Parse(filtro.AnoTurma),
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

        private string ConsultaLeituraLinguaPortuguesaPrimeiroAoTerceiroAno(string campoTabelaFiltroBimestre)
        {
            var sql = new StringBuilder();
            sql.AppendLine("          select  pp.\"classroomCodeEol\" as TurmaCodigo,  ");
            sql.AppendLine($"		        count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'Nivel1') as Nivel1,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'Nivel2') as Nivel2,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'Nivel3') as Nivel3,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'Nivel4') as Nivel4,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where ((pp.\"{campoTabelaFiltroBimestre}\" is null) or (trim(pp.\"{campoTabelaFiltroBimestre}\") = ''))) as SemPreenchimento");
            sql.AppendLine("         from \"PortuguesePolls\" pp ");
            sql.AppendLine("         where  pp.\"dreCodeEol\" = @dreCodeEol ");
            sql.AppendLine("                and pp.\"schoolCodeEol\" = @ueCodigo ");
            sql.AppendLine("				and pp.\"schoolYear\" = @anoLetivo ");
            sql.AppendLine("                and pp.\"yearClassroom\" = @anoTurma ");
            sql.AppendLine("			    group by pp.\"dreCodeEol\", pp.\"schoolCodeEol\" ,pp.\"yearClassroom\" ,pp.\"classroomCodeEol\" ");
            sql.AppendLine("                order by pp.\"dreCodeEol\", pp.\"schoolCodeEol\", pp.\"yearClassroom\" ;");

            return sql.ToString();
        }

        private string ConsultaEscritaLinguaPortuguesaPrimeiroAoTerceiroAno(string campoTabelaFiltroBimestre)
        {
            var sql = new StringBuilder();
            sql.AppendLine("          select  pp.\"classroomCodeEol\" as TurmaCodigo,  ");
            sql.AppendLine($"		        count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'PS') as PreSilabico,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'SSV') as SilabicoSemValor,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'SCV') as SilabicoComValor,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'SA') as SilabicoAlfabetico,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'A') as Alfabetico,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where ((pp.\"{campoTabelaFiltroBimestre}\" is null) or (trim(pp.\"{campoTabelaFiltroBimestre}\") = ''))) as SemPreenchimento");
            sql.AppendLine("         from \"PortuguesePolls\" pp ");
            sql.AppendLine("         where  pp.\"dreCodeEol\" = @dreCodeEol ");
            sql.AppendLine("                and pp.\"schoolCodeEol\" = @ueCodigo ");
            sql.AppendLine("				and pp.\"schoolYear\" = @anoLetivo ");
            sql.AppendLine("                and pp.\"yearClassroom\" = @anoTurma ");
            sql.AppendLine("			    group by pp.\"dreCodeEol\", pp.\"schoolCodeEol\" ,pp.\"yearClassroom\" ,pp.\"classroomCodeEol\" ");
            sql.AppendLine("                order by pp.\"dreCodeEol\", pp.\"schoolCodeEol\", pp.\"yearClassroom\" ;");

            return sql.ToString();
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoLeituraDeVozAlta(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();

            retorno.Add(new RelatorioSondagemAnaliticoLeituraDeVozAltaDto());

            return retorno;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoProducaoDeTexto(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var modalidades = new List<int> {5, 13};
            var periodo = await ObterPeriodoSondagem(filtro.Bimestre);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var dre = await dreRepository.ObterPorCodigo(filtro.DreCodigo);
            var ue = await ueRepository.ObterPorCodigo(filtro.UeCodigo);
            var totalDeAlunos = await alunoRepository.ObterTotalAlunosAtivosPorPeriodo(filtro.AnoTurma, filtro.AnoLetivo, modalidades.ToArray(), periodoFixo.DataInicio, periodoFixo.DataFim, filtro.UeCodigo, filtro.DreCodigo);
            var perguntas = new RelatorioSondagemAnaliticoProducaoDeTextoDto();


            var realizarConsulta =( await sondagemRelatorioRepository.ObterDadosProducaoTexto(new RelatorioPortuguesFiltroDto
            {
                AnoEscolar = int.Parse(filtro.AnoTurma),
                AnoLetivo = filtro.AnoLetivo,
                CodigoDre = filtro.DreCodigo,
                CodigoUe = filtro.UeCodigo,
                ComponenteCurricularId = SondagemComponenteCurricular.LINGUA_PORTUGUESA,
                GrupoId = GrupoSondagem.PRODUCAO_DE_TEXTO,
                PeriodoId = periodo.Id
            })).ToList();

            var alunoNaSondagem = realizarConsulta.GroupBy(x => x.CodigoAluno);
            var tumasNaSondagem = realizarConsulta.GroupBy(x => x.CodigoTurma);
            var quantidadeAlunoNaSondagem = alunoNaSondagem.Select(x => x.Key).Count();
            var totalDeTurma = tumasNaSondagem.Select(x => x.Key).Count();
            
            var respostas = new RespostaSondagemAnaliticoProducaoDeTextoDto()
            {
                NaoProduziuEntregouEmBranco = realizarConsulta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoProduziuEntregouEmBranco),
                NaoApresentouDificuldades = realizarConsulta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.NaoApresentouDificuldades),
                EscritaNaoAlfabetica = realizarConsulta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.EscritaNaoAlfabetica),
                DificuldadesComAspectosSemanticos = realizarConsulta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosSemanticos),
                DificuldadesComAspectosTextuais = realizarConsulta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosTextuais),
                DificuldadesComAspectosOrtograficosNotacionais = realizarConsulta.Count(p => p.Pergunta == PerguntaDescricaoSondagem.DificuldadesComAspectosOrtograficosNotacionais),
                SemPreenchimento = totalDeAlunos - quantidadeAlunoNaSondagem,
                Ano = int.Parse(filtro.AnoTurma),
                TotalDeTurma = totalDeTurma,
                TotalDeAlunos = totalDeAlunos,
                Ue = ue.Nome
            };
            
            perguntas.Respostas.Add(respostas);
            perguntas.DreSigla = dre.Abreviacao;
            perguntas.Dre = dre.Nome;
            retorno.Add(perguntas);
            return retorno;
        }
    }
}