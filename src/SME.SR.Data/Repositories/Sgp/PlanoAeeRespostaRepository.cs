using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Data.Repositories.Sgp
{
    public class PlanoAeeRespostaRepository : IPlanoAeeRespostaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PlanoAeeRespostaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RespostaQuestaoDto>> ObterRespostasPorVersaoPlanoId(long versaoPlanoId)
        {
            const string query = @"select par.id,
                                        par.plano_questao_id as planoaeequestaoid,
                                        par.resposta_id as respostaid,
                                        par.texto,
                                        paq.id,
                                        paq.plano_aee_versao_id as planoaeeversaoid,
                                        paq.questao_id as questaoid
                                    from plano_aee_questao paq
                                        inner join plano_aee_resposta par on par.plano_questao_id = paq.id
                                    where paq.plano_aee_versao_id = @versaoPlanoId";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<PlanoAeeResposta, PlanoAeeQuestao, RespostaQuestaoDto>(query,
                (planoAeeResposta, planoAeeQuestao) => new RespostaQuestaoDto
                {
                    Id = planoAeeResposta.Id,
                    QuestaoId = planoAeeQuestao.QuestaoId,
                    OpcaoRespostaId = planoAeeResposta.RespostaId,
                    Texto = planoAeeResposta.Texto
                }, new { versaoPlanoId });
        }
    }
}