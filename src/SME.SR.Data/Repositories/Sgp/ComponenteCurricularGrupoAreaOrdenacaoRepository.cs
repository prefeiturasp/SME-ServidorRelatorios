﻿using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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
            var query = @"SELECT grupo_matriz_id GrupoMatrizId, area_conhecimento_id AreaConhecimentoId, ordem
                                FROM public.componente_curricular_grupo_area_ordenacao
                            where grupo_matriz_id = ANY(@grupoMatrizIds) and area_conhecimento_id = ANY(@areaConhecimentoIds)";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            return await conexao.QueryAsync<ComponenteCurricularGrupoAreaOrdenacaoDto>(query.ToString(), new { grupoMatrizIds, areaConhecimentoIds });
        }
    }
}
