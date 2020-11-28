using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ConselhoClasseRepository : IConselhoClasseRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ConselhoClasseRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<long> ObterConselhoPorFechamentoTurmaId(long fechamentoTurmaId)
        {
            var query = ConselhoClasseConsultas.ConselhoPorFechamentoId;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<long>(query, parametros);
            }
        }
        public async Task<IEnumerable<long>> ObterPareceresConclusivosPorTipoAprovacao(bool aprovado)
        {
            var query = @"select id from conselho_classe_parecer ccp 
                            where 
                            ccp.aprovado  = @aprovado";

            var parametros = new { aprovado };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<long>(query, parametros);

        }

        public async Task<IEnumerable<HistoricoAlteracaoNotasDto>> ObterHistoricoAlteracaoNotasConselhoClasse(long turmaId)
        {
            var query = @"select cca.aluno_codigo as codigoAluno,
	                               hn.nota_anterior as notaAnterior,
	                               hn.nota_nova as notaAtribuida,
	                               hn.conceito_anterior_id as conceitoAnteriorId,
	                               hn.conceito_novo_id as conceitoAtribuidoId, 
	                               hn.criado_por as usuarioAlteracao,
	                               hn.criado_rf as RfAlteracao,
	                               hn.criado_em as DataAlteracao
                              from historico_nota hn
                             inner join historico_nota_conselho_classe hncc on hn.id = hncc.historico_nota_id
                             inner join conselho_classe_nota ccn on hncc.conselho_classe_nota_id = ccn.id 
                             inner join conselho_classe_aluno cca on ccn.conselho_classe_aluno_id = cca.id 
                             inner join conselho_classe cc on cca.conselho_classe_id = cc.id 
                             inner join fechamento_turma ft on cc.fechamento_turma_id = ft.id 
                             where ft.turma_id = @turmaId";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryAsync<HistoricoAlteracaoNotasDto>(query, new { turmaId });
            }
        }
    }
}
