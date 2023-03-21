using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using SME.SR.Data.Models;

namespace SME.SR.Data
{
    public class SondagemAnaliticaRepository : ISondagemAnaliticaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        private readonly IAlunoRepository alunoRepository;

        public SondagemAnaliticaRepository(VariaveisAmbiente variaveisAmbiente,IAlunoRepository alunoRepository)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
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

            retorno.Add(new RelatorioSondagemAnaliticoEscritaDto());

            return retorno;
        }

        public async Task<IEnumerable<RelatorioSondagemAnaliticoPorDreDto>> ObterRelatorioSondagemAnaliticoLeitura(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            var sql = new StringBuilder();
            var retorno = new List<RelatorioSondagemAnaliticoPorDreDto>();
            var periodo = await ObterPeriodoSondagem(filtro.Bimestre);
            var periodoFixo = await ObterPeriodoFixoSondagem(filtro.AnoLetivo,periodo.Id);
            var modalidades = new List<int>{1,15};
            var quantidadeTotalAlunos = await alunoRepository.ObterTotalAlunosAtivosPorPeriodo(filtro.AnoTurma,filtro.AnoLetivo,modalidades.ToArray(),periodoFixo.DataInicio,periodoFixo.DataFim,filtro.UeCodigo,filtro.DreCodigo);
            var relatorioSondagemAnaliticoLeituraDto = new RelatorioSondagemAnaliticoLeituraDto();

            var campoTabelaFiltroBimestre = @$"reading${filtro.Bimestre}B";
            
            sql.AppendLine("          select");
            sql.AppendLine($"		        count(pp.\"studentCodeEol\") filter (where pp.\"${campoTabelaFiltroBimestre}\" = 'Nivel1') as Nivel1,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"${campoTabelaFiltroBimestre}\" = 'Nivel2') as Nivel2,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"${campoTabelaFiltroBimestre}\" = 'Nivel3') as Nivel3,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where pp.\"${campoTabelaFiltroBimestre}\" = 'Nivel4') as Nivel4,");
            sql.AppendLine($"	            count(pp.\"studentCodeEol\") filter (where ((pp.\"${campoTabelaFiltroBimestre}\" is null) or (trim(pp.\"${campoTabelaFiltroBimestre}\") = ''))) as SemPreenchimento");
            sql.AppendLine("         from \"PortuguesePolls\" pp ");
            sql.AppendLine("         where  pp.\"dreCodeEol\" = @dreCodeEol ");
            sql.AppendLine("                and pp.\"schoolCodeEol\" = @ueCodigo ");
            sql.AppendLine("				and pp.\"schoolYear\" = @anoLetivo ");
            sql.AppendLine("                and pp.\"yearClassroom\" = @anoTurma ");
            sql.AppendLine("			    group by pp.\"dreCodeEol\", pp.\"schoolCodeEol\" ,pp.\"yearClassroom\" ");
            sql.AppendLine("                order by pp.\"dreCodeEol\", pp.\"schoolCodeEol\", pp.\"yearClassroom\" ;");

            var parametros = new { dreCodeEol = filtro.DreCodigo, ueCodigo = filtro.UeCodigo, anoLetivo = filtro.AnoLetivo,anoTurma = filtro.AnoTurma };
            RespostaSondagemAnaliticoLeituraDto dtoConsulta = null;
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                dtoConsulta = await conexao.QueryFirstOrDefaultAsync<RespostaSondagemAnaliticoLeituraDto>(sql.ToString(), parametros);
            }

            if (dtoConsulta != null)
            {
                var calculo = (quantidadeTotalAlunos - (dtoConsulta.Nivel1 + dtoConsulta.Nivel2 + dtoConsulta.Nivel3 + dtoConsulta.Nivel4));
                dtoConsulta.SemPreenchimento = calculo >=0 ? calculo : dtoConsulta.SemPreenchimento;
                
                relatorioSondagemAnaliticoLeituraDto.Respostas.Add(dtoConsulta);
            }
            retorno.Add(relatorioSondagemAnaliticoLeituraDto);
            return retorno;
        }

        private async Task<PeriodoSondagem> ObterPeriodoSondagem(int bimestre)
        {
            var termo = @$"{bimestre}° Bimestre";

            var sql = " select * from \"Periodo\" p where p.\"Descricao\" = @termo";
            
            using (var conn = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conn.QueryFirstOrDefaultAsync<PeriodoSondagem>(sql,new{termo});
            }
        }
        private async Task<PeriodoFixoSondagem> ObterPeriodoFixoSondagem(int anoLetivo,string periodoId)
        {
            var sql = " select * from \"PeriodoFixoAnual\" pfa where pfa.\"PeriodoId\" = @periodoId and \"Ano\" = @anoLetivo ";
            var parametros = new {periodoId,anoLetivo };
            using (var conn = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSondagem))
            {
                return await conn.QueryFirstOrDefaultAsync<PeriodoFixoSondagem>(sql,parametros);
            }
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
