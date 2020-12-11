using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ComunicadosRepository : IComunicadosRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ComunicadosRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<LeituraComunicadoDto>> ObterComunicadosPorFiltro(FiltroRelatorioLeituraComunicadosDto filtro)
        {
            var query = @"select titulo as Comunicado, data_envio as DataEnvio, data_expiracao as DataExpiracao
                          from comunicado
                         where ano_letivo = @AnoLetivo
                           and modalidade = @ModalidadeTurma";

            if (!string.IsNullOrEmpty(filtro.CodigoDre))
                query += " and codigo_dre = @CodigoDre ";
            if (!string.IsNullOrEmpty(filtro.CodigoUe))
                query += " and codigo_ue = @CodigoUe ";
            if (filtro.Semestre > 0)
                query += " and semestre = @Semestre ";
            if (filtro.DataInicio > DateTime.MinValue)
                query += " and data_envio between @DataInicio and @DataFim ";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<LeituraComunicadoDto>(query.ToString(), new 
            { 
                filtro.AnoLetivo,
                filtro.ModalidadeTurma,
                filtro.CodigoDre,
                filtro.CodigoUe,
                filtro.Semestre,
                filtro.DataInicio,
                filtro.DataFim
            });
        }
    }
}
