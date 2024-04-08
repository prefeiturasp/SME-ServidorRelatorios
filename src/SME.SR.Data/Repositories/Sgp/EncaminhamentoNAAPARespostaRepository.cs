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
                             inner join secao_encaminhamento_naapa sen on sen.id = ens.secao_encaminhamento_id and not sen.excluido
                             inner join encaminhamento_naapa_questao enq on enq.encaminhamento_naapa_secao_id = ens.id and not enq.excluido
                             inner join encaminhamento_naapa_resposta enr on enr.questao_encaminhamento_id = enq.id and not enr.excluido 
	                         where not ens.excluido and ens.encaminhamento_naapa_id = @encaminhamentoNaapaId";

            if (!string.IsNullOrEmpty(nomeComponenteSecao))
                query += " and sen.nome_componente = @nomeComponenteSecao ";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<RespostaQuestaoDto>(query, new { encaminhamentoNaapaId, nomeComponenteSecao });
        }

        public async Task<IEnumerable<ArquivoDto>> ObterRepostasArquivosPdfPorEncaminhamentoIdAsync(long encaminhamentoNaapaId, ImprimirAnexosNAAPA imprimirAnexosNAAPA)
        {
            var dicionario = new Dictionary<ImprimirAnexosNAAPA, string[]>()
            {
                { ImprimirAnexosNAAPA.ApenasEncaminhamento, new string[] { "INFORMACOES_ESTUDANTE" } },
                { ImprimirAnexosNAAPA.ApenasAtendimentos, new string[] { "QUESTOES_ITINERANCIA" } },
                { ImprimirAnexosNAAPA.EncaminhamentoAtendimentos, new string[] { "INFORMACOES_ESTUDANTE", "QUESTOES_ITINERANCIA" } }
            };

            var query = @"select a.id, 
                                 a.tipo_conteudo as TipoArquivo, 
                                 a.codigo, 
                                 a.nome as NomeOriginal, 
                                 a.Tipo
                        from encaminhamento_naapa ea
                        inner join encaminhamento_naapa_secao eas on ea.id = eas.encaminhamento_naapa_id
                        inner join secao_encaminhamento_naapa sen on sen.id = eas.secao_encaminhamento_id 
                        inner join encaminhamento_naapa_questao qea on eas.id = qea.encaminhamento_naapa_secao_id
                        inner join encaminhamento_naapa_resposta rea on qea.id = rea.questao_encaminhamento_id
                        inner join arquivo a on rea.arquivo_id = a.id
                        where ea.id = @encaminhamentoNaapaId 
                          and not rea.excluido
                          and a.tipo_conteudo like '%pdf'
                          and nome_componente = ANY(@componentes)";

            var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<ArquivoDto>(query, new { encaminhamentoNaapaId, componentes = dicionario[imprimirAnexosNAAPA] });
        }
    }
}
