using Dapper;
using Npgsql;
using SME.SR.Data.Models;
using SME.SR.Data.Queries;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PeriodoFechamentoRepository : IPeriodoFechamentoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;
        

        public PeriodoFechamentoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<int> ObterBimestrePeriodoFechamentoAtual(int anoLetivo)
        {
            var query = @"select pe.bimestre 
                          from periodo_fechamento pf 
                         inner join periodo_fechamento_bimestre pfb on pfb.periodo_fechamento_id = pf.id
                         inner join periodo_escolar pe on pe.id = pfb.periodo_escolar_id
                         inner join tipo_calendario tc on pe.tipo_calendario_id = tc.id 
                         where tc.ano_letivo  = @anoLetivo
                           and now()::date >= pfb.inicio_fechamento::date
                           order by pfb.inicio_fechamento desc 
                           limit 1 ";

            var parametros = new
            {
                AnoLetivo = anoLetivo
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<int>(query, parametros);
            };
        }

        public async Task<PeriodoFechamentoBimestre> ObterPeriodoFechamentoTurmaAsync(long ueId, long dreId, int anoLetivo, int bimestre, long? periodoEscolarId)
        {
            var query = PeriodoFechamentoConsultas.ObterPorTurma(bimestre, periodoEscolarId);
            var parametros = new
            {
                UeId = ueId,
                DreId = dreId,
                AnoLetivo = anoLetivo,
                Bimestre = bimestre,
                PeriodoEscolarId =  periodoEscolarId
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<PeriodoFechamentoBimestre>(query, parametros);
            };
        }

        public async Task<IEnumerable<PeriodoFechamentoBimestre>> ObterPeriodosFechamento(long ueId, long dreId, int anoLetivo)
        {
            var query = @"select pfb.id, pfb.periodo_escolar_id PeriodoEscolarId, 
	                         pfb.periodo_fechamento_id PeriodoFechamentoId,
	                         pfb.inicio_fechamento InicioDoFechamento,
	                         pfb.final_fechamento FinalDoFechamento
                          from periodo_fechamento pf 
                         inner join periodo_fechamento_bimestre pfb on pfb.periodo_fechamento_id = pf.id
                         inner join periodo_escolar pe on pe.id = pfb.periodo_escolar_id
                         inner join tipo_calendario tc on pe.tipo_calendario_id = tc.id 
                         where pf.ue_id = @ueId
                           and pf.dre_id = @dreId 
                           and tc.ano_letivo  = @anoLetivo";

            var parametros = new
            {
                UeId = ueId,
                DreId = dreId,
                AnoLetivo = anoLetivo
            };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryAsync<PeriodoFechamentoBimestre>(query, parametros);
            };
        }

        public async Task<PeriodoFechamentoVigenteDto> TurmaEmPeriodoDeFechamentoVigente(Turma turma, TipoCalendario tipoCalendario, DateTime dataReferencia, int bimestre = 0)
        {
            var periodoFechamentoBimestre = await UeEmFechamentoVigente(dataReferencia, tipoCalendario.Id, turma.EhTurmaInfantil, bimestre);

            if (periodoFechamentoBimestre != null)
                return new PeriodoFechamentoVigenteDto() { PeriodoFechamentoInicio = periodoFechamentoBimestre.InicioDoFechamento, PeriodoFechamentoFim = periodoFechamentoBimestre.FinalDoFechamento };

            var periodoReabertura = await ObterReaberturaFechamentoBimestrePorDataReferencia(bimestre, dataReferencia, tipoCalendario.Id, turma.Ue.Dre.Codigo, turma.Ue.Codigo);

            if (periodoReabertura != null)
                return new PeriodoFechamentoVigenteDto() { PeriodoFechamentoInicio = periodoReabertura.Inicio, PeriodoFechamentoFim = periodoReabertura.Fim };

            return null;
        }
        public async Task<FechamentoReabertura> ObterReaberturaFechamentoBimestrePorDataReferencia(int bimestre, DateTime dataReferencia, long tipoCalendarioId, string dreCodigo, string ueCodigo)
        {
            var bimetreQuery = "(select pe.bimestre from periodo_escolar pe inner join tipo_calendario tc on tc.id  = pe.tipo_calendario_id and tc.id = fr.tipo_calendario_id order by pe.bimestre  desc limit 1)";
            var bimestreWhere = $"and frb.bimestre = {(bimestre > 0 ? " @bimestre" : bimetreQuery)}";

            var query = $@"select fr.* 
                          from fechamento_reabertura_bimestre frb
                         inner join fechamento_reabertura fr on fr.id = frb.fechamento_reabertura_id
                         left join dre on dre.id = fr.dre_id
                         left join ue on ue.id = fr.ue_id
                         where not fr.excluido 
                          {bimestreWhere}
                           and TO_DATE(fr.inicio::TEXT, 'yyyy/mm/dd') <= TO_DATE(@dataReferencia, 'yyyy/mm/dd')
                           and TO_DATE(fr.fim::TEXT, 'yyyy/mm/dd') >= TO_DATE(@dataReferencia, 'yyyy/mm/dd')
                           and fr.tipo_calendario_id = @tipoCalendarioId
                           and ((dre.dre_id = @dreCodigo and ue.ue_id = @ueCodigo)
                           or (dre.dre_id = @dreCodigo and ue.ue_id is null)
                           or (dre.dre_id is null and ue.ue_id is null))
                           and fr.status = 1 ";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<FechamentoReabertura>(query, new
                {
                    bimestre,
                    dataReferencia = dataReferencia.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo),
                    tipoCalendarioId,
                    dreCodigo,
                    ueCodigo
                });
            }
        }
        private async Task<PeriodoFechamentoBimestre> UeEmFechamentoVigente(DateTime dataReferencia, long tipoCalendarioId, bool ehModalidadeInfantil, int bimestre)
        {
            var query = new StringBuilder();

            var consultaObterBimestreFinal = "(select pe2.bimestre from periodo_escolar pe2 where @tipoCalendarioId = pe2.tipo_calendario_id order by pe2.bimestre desc limit 1)";

            query.AppendLine(@"select pfb.* from periodo_fechamento pf 
				inner join periodo_fechamento_bimestre pfb on pf.id = pfb.periodo_fechamento_id 
				inner join periodo_escolar pe on pe.id = pfb.periodo_escolar_id
				where pe.tipo_calendario_id = @tipoCalendarioId
				and pf.ue_id is null
				and pf.dre_id is null
				and TO_DATE(pfb.inicio_fechamento::TEXT, 'yyyy/mm/dd') <= TO_DATE(@dataReferencia, 'yyyy/mm/dd')
				and TO_DATE(pfb.final_fechamento::TEXT, 'yyyy/mm/dd') >= TO_DATE(@dataReferencia, 'yyyy/mm/dd')");

            if (bimestre > 0)
                query.AppendLine($"and pe.bimestre {BimestreConstants.ObterCondicaoBimestre(bimestre, ehModalidadeInfantil)}");

            else
                query.AppendLine($"and pe.bimestre =  {consultaObterBimestreFinal}");


            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                return await conexao.QueryFirstOrDefaultAsync<PeriodoFechamentoBimestre>(query.ToString(), new
                {
                    dataReferencia = dataReferencia.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo),
                    bimestre,
                    tipoCalendarioId
                });
            }
        }
    }
}
