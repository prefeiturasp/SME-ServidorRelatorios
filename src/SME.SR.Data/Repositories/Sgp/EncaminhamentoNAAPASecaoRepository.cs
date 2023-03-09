using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Sgp
{
    public class EncaminhamentoNAAPASecaoRepository : IEncaminhamentoNAAPASecaoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public EncaminhamentoNAAPASecaoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<SecaoEncaminhamentoNAAPADto>> ObterSecoesPorEncaminhamentoIdAsync(long encaminhamentoNaapaId, string nomeComponenteSecao)
        {
            string query = @"with vw_resposta_data as (
                             select ens.id encaminhamento_naapa_secao_id, 
                                     to_date(enr.texto,'yyyy-mm-dd') DataAtendimento    
                             from encaminhamento_naapa_secao ens   
                             join encaminhamento_naapa_questao enq on ens.id = enq.encaminhamento_naapa_secao_id  
                             join questao q on enq.questao_id = q.id 
                             join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id 
                             where q.nome_componente  = 'DATA_DO_ATENDIMENTO' 
                             ),
                             vw_resposta_tipo_atendimento as (
                             select ens.id encaminhamento_naapa_secao_id,
                                     opr.nome as TipoAtendimento,
                                     enr.resposta_id  as TipoAtendimentoId
                             from encaminhamento_naapa_secao ens   
                             join encaminhamento_naapa_questao enq on ens.id = enq.encaminhamento_naapa_secao_id  
                             join questao q on enq.questao_id = q.id 
                             join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id 
                             join opcao_resposta opr on opr.id = enr.resposta_id
                             where q.nome_componente = 'TIPO_DO_ATENDIMENTO' 
                             )
                             select ens.id as SecaoId,
                                    questaoDataAtendimento.DataAtendimento,
                                    questaoTipoAtendimento.TipoAtendimento,
                                    ens.Criado_Por as CriadoPor
                             from encaminhamento_naapa en
                             inner join encaminhamento_naapa_secao ens on ens.encaminhamento_naapa_id = en.id
                             inner join secao_encaminhamento_naapa secao on secao.id = ens.secao_encaminhamento_id 
                             inner join vw_resposta_data questaoDataAtendimento on questaoDataAtendimento.encaminhamento_naapa_secao_id = ens.id
                             inner join vw_resposta_tipo_atendimento questaoTipoAtendimento on questaoTipoAtendimento.encaminhamento_naapa_secao_id = ens.id
                             where en.id = @encaminhamentoNaapaId and secao.nome_componente = @nomeComponenteSecao and not ens.excluido";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<SecaoEncaminhamentoNAAPADto>(query, new { encaminhamentoNaapaId, nomeComponenteSecao });
        }
    }
}
