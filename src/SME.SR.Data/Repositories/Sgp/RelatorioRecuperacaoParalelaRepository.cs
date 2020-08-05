using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class RelatorioRecuperacaoParalelaRepository : IRelatorioRecuperacaoParalelaRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public RelatorioRecuperacaoParalelaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RelatorioRecuperacaoParalelaRetornoQueryDto>> ObterDadosDeSecao(string turmaCodigo, string alunoCodigo, int semestre)
        {
            try
            {
                var query = new StringBuilder(@" select 
	                            rspa.aluno_codigo as alunoCodigo,
	                            t.turma_id as turmaCodigo, 
	                            t.nome as turmaNome,
	                            t.ano_letivo as anoLetivo,
	                            srsp.nome as secaoNome,
	                            rspas.valor as secaoValor	     
                            from relatorio_semestral_pap_aluno rspa 
                            inner join relatorio_semestral_turma_pap rstp
	                            on rspa.relatorio_semestral_turma_pap_id = rstp.id
                            inner join turma t 
	                            on rstp.turma_id = t.id 
                            inner join relatorio_semestral_pap_aluno_secao rspas 
	                            on rspa.id = rspas.relatorio_semestral_pap_aluno_id
	                        inner join secao_relatorio_semestral_pap srsp
	                            on rspas.secao_relatorio_semestral_pap_id  = srsp.id
                            where t.turma_id  = @turmaCodigo and 
                                  rstp.semestre = @semestre ");

                if (!string.IsNullOrEmpty(alunoCodigo))
                {
                    query.AppendLine("and aluno_codigo = @alunoCodigo ");
                }

                //TODO: adicionar vigencia

                query.AppendLine("order by ordem ");

                await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
                var parametros = new { turmaCodigo, alunoCodigo, semestre };
                return await conexao.QueryAsync<RelatorioRecuperacaoParalelaRetornoQueryDto>(query.ToString(), parametros);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
