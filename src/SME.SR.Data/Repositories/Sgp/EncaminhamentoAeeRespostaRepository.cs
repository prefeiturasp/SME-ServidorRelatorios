using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Data.Repositories.Sgp
{
    public class EncaminhamentoAeeRespostaRepository : IEncaminhamentoAeeRespostaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public EncaminhamentoAeeRespostaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RespostaQuestaoDto>> ObterRespostasPorEncaminhamentoId(long encaminhamentoAeeId, string nomeComponenteSecao)
        {
            string query = @"select ear.id,
                                    ear.questao_encaminhamento_id  as encaminhamentoaeequestaoid,
                                    ear.resposta_id as respostaid,
                                    ear.texto,
                                    eaq.id,
                                    eaq.questao_id as questaoid
                                from questao_encaminhamento_aee eaq
                                    inner join resposta_encaminhamento_aee ear on ear.questao_encaminhamento_id  = eaq.id
                                    inner join encaminhamento_aee_secao eas on eas.id = eaq.encaminhamento_aee_secao_id 
                                    inner join secao_encaminhamento_aee sea on sea.id = eas.secao_encaminhamento_id 
                                    where eas.encaminhamento_aee_id  = @encaminhamentoAeeId";

            if (!string.IsNullOrEmpty(nomeComponenteSecao))
                query = query + " and sea.nome_componente = @nomeComponenteSecao ";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<EncaminhamentoAeeResposta, EncaminhamentoAeeQuestao, RespostaQuestaoDto>(query,
                (encaminhamentoAeeResposta, encaminhamentoAeeQuestao) => new RespostaQuestaoDto
                {
                    Id = encaminhamentoAeeResposta.Id,
                    QuestaoId = encaminhamentoAeeQuestao.QuestaoId,
                    OpcaoRespostaId = encaminhamentoAeeResposta.RespostaId,
                    Texto = encaminhamentoAeeResposta.Texto
                }, new { encaminhamentoAeeId, nomeComponenteSecao });
        }

        
    }
}