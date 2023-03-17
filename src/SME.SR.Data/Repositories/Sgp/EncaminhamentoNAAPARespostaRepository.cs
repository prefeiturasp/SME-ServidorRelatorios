using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class EncaminhamentoNAAPARespostaRepository : IEncaminhamentoNAAPARespostaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public EncaminhamentoNAAPARespostaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RespostaQuestaoDto>> ObterRespostasPorEncaminhamentoIdAsync(long encaminhamentoNaapaId, string nomeComponenteSecao)
        {
            string query = @"select enr.id,
                                    ens.id as SecaoId,
	                                enq.questao_id as QuestaoId,
	                                enr.resposta_id as OpcaoRespostaId,
	                                enr.texto
	                         from encaminhamento_naapa_secao ens 
	                         inner join secao_encaminhamento_naapa sen on sen.id = ens.secao_encaminhamento_id 
	                         inner join encaminhamento_naapa_questao enq on enq.encaminhamento_naapa_secao_id = ens.id 
	                         inner join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id 
	                         where ens.encaminhamento_naapa_id = @encaminhamentoNaapaId";

            if (!string.IsNullOrEmpty(nomeComponenteSecao))
                query += " and sen.nome_componente = @nomeComponenteSecao ";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<RespostaQuestaoDto>(query, new { encaminhamentoNaapaId, nomeComponenteSecao });
        }
    }
}
