using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
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

        public SondagemAnaliticaRepository(VariaveisAmbiente variaveisAmbiente, IAlunoRepository alunoRepository, IDreRepository dreRepository, IUeRepository ueRepository)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
            this.dreRepository = dreRepository ?? throw new ArgumentNullException(nameof(dreRepository));
            this.ueRepository = ueRepository ?? throw new ArgumentNullException(nameof(ueRepository));
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoCapacidadeDeLeitura(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();

            retorno.Add(new RelatorioSondagemAnaliticoCapacidadeDeLeituraDto());

            return retorno;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoEscrita(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Bimestre);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo, periodo.Id);
            var modalidades = new List<int> {5, 13};
            var relatorioSondagemAnaliticoLeituraDto = new RelatorioSondagemAnaliticoLeituraDto();
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
                    SilabicoSemValor=dtoConsulta.Select(x => x.SilabicoSemValor).Sum() ,
                    SilabicoComValor = dtoConsulta.Select(x => x.SilabicoComValor).Sum(),
                    SilabicoAlfabetico = dtoConsulta.Select(x => x.SilabicoAlfabetico).Sum() ,
                    Alfabetico = dtoConsulta.Select(x => x.Alfabetico).Sum(),
                    SemPreenchimento = calculoComAlunos >= 0 ? calculoComAlunos : dtoConsulta.Select(x => x.SemPreenchimento).Sum(),
                    TotalDeAlunos = quantidadeTotalAlunos,
                    Ano = int.Parse(filtro.AnoTurma),
                    TotalDeTurma = turmasComSondagem.Count(),
                    Ue = ue.Nome
                };
            }

            relatorioSondagemAnaliticoLeituraDto.Dre = dre.Nome;
            relatorioSondagemAnaliticoLeituraDto.DreSigla = dre.Abreviacao;
            retorno.Add(relatorioSondagemAnaliticoLeituraDto);
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
            sql.AppendLine($"		        count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'PS') as Nivel1,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'SSV') as Nivel2,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"{campoTabelaFiltroBimestre}\" = 'SCV') as Nivel3,");
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

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoLeituraDeVozAlta(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();

            retorno.Add(new RelatorioSondagemAnaliticoLeituraDeVozAltaDto());

            return retorno;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoProducaoDeTexto(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();

            retorno.Add(new RelatorioSondagemAnaliticoProducaoDeTextoDto());

            return retorno;
        }
    }
}