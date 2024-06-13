using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Sondagem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class SondagemAnaliticaRepository : ISondagemAnaliticaRepository
    {
        private const string OPCAO_TODAS = "-99";

        private readonly VariaveisAmbiente variaveisAmbiente;

        public SondagemAnaliticaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
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
                sql.AppendLine($"         count(pp.\"studentCodeEol\") filter (where pp.\"writing{filtroRelatorioAnaliticoSondagemDto.Periodo}B\" = 'SemPreenchimento') as  SemPreenchimento");
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
    }
}