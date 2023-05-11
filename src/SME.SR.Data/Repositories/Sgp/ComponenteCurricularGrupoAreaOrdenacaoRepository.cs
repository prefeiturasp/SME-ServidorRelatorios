using Dapper;
using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ComponenteCurricularGrupoAreaOrdenacaoRepository : IComponenteCurricularGrupoAreaOrdenacaoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ComponenteCurricularGrupoAreaOrdenacaoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto>> ObterOrdenacaoPorGruposAreas(long[] grupoMatrizIds, long[] areaConhecimentoIds)
        {
            var query = @$"SELECT grupo_matriz_id GrupoMatrizId, area_conhecimento_id AreaConhecimentoId, ordem
                                FROM public.componente_curricular_grupo_area_ordenacao
                            where grupo_matriz_id = ANY(@grupoMatrizIds) {(areaConhecimentoIds.Any(a=> a > 0) ? " and area_conhecimento_id = ANY(@areaConhecimentoIds)" : string.Empty)}";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);
            return await conexao.QueryAsync<ComponenteCurricularGrupoAreaOrdenacaoDto>(query.ToString(), new { grupoMatrizIds, areaConhecimentoIds });
        }
    }
}
