using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<RelatorioRecuperacaoParalelaAlunoDto>> Obter(int idRecuperacaoParalela, long turmaId, int semestre)
        {

            //AlunoNome = alunoNome;
            //DataNascimento = dataNascimento;
            //CodigoEol = codigoEol;
            //Situacao = situacao;

            //Turma = turma;
            //TurmaRegular = turmaRegular;

            //Historico = historico;
            //Dificuldades = dificuldades;
            //Encaminhamentos = encaminhamentos;
            //Avancos = avancos;
            //Outros = outros;

            var query = @" select	 
                            srs.id, 
	                        srs.nome, 
	                        srs.descricao, 
	                        srs.obrigatorio,
	                        srs.ordem,
	                        rsas.valor
                           from secao_relatorio_semestral_pap srs     
                            left join relatorio_semestral_pap_aluno_secao rsas on rsas.secao_relatorio_semestral_pap_id = srs.id  
                            and rsas.relatorio_semestral_pap_aluno_id = @idRecuperacaoParalela
                            and rs.semestre = @semestre";

            await using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);
            var parametros = new { idRecuperacaoParalela, turmaId, semestre };
            return await conexao.QueryAsync<RelatorioRecuperacaoParalelaAlunoDto>(query, parametros);
        }
    }
}
