﻿using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class ParecerConclusivoRepository : IParecerConclusivoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ParecerConclusivoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<RelatorioParecerConclusivoRetornoDto>> ObterPareceresFinais(int anoLetivo, string dreCodigo, string ueCodigo, Modalidade? modalidade, int? semestre,
                                                                                                  long cicloId, string[] turmasCodigo, string[] anos, long parecerConclusivoId)
        {
            var query = new StringBuilder(@"select t.turma_id as TurmaId, 
	                                               cca.aluno_codigo AlunoCodigo, 
	                                               ccp.nome ParecerConclusivo, 
	                                               d.abreviacao as DreNome, 
                                                   d.dre_id as DreCodigo,
	                                               te.descricao || ' - ' || u.nome as UeNome,
                                                   u.ue_id as UeCodigo,
                                                   t.nome as TurmaNome,
	                                               t.ano,
                                                   t.ano_letivo as anoLetivo,
	                                               tc.descricao as Ciclo,
                                                   tc.Id as CicloId
	                                            from conselho_classe_aluno cca 
		                                            inner join conselho_classe_parecer ccp
			                                            on cca.conselho_classe_parecer_id = ccp.id 
		                                            inner join conselho_classe cc 
			                                            on cca.conselho_classe_id = cc.id
		                                            inner join fechamento_turma ft
			                                            on cc.fechamento_turma_id = ft.id
		                                            inner join turma t
			                                            on ft.turma_id  = t.id
		                                            inner join ue u 
			                                            on t.ue_id = u.id 
		                                            inner join dre d 
			                                            on u.dre_id  = d.id
		                                            inner join tipo_ciclo_ano tca
			                                            on tca.ano  = t.ano 
			                                            and tca.modalidade = t.modalidade_codigo 
		                                            inner join tipo_ciclo tc
			                                            on tca.tipo_ciclo_id = tc.id 
    	                                            inner join tipo_escola te
                                                        on te.id = u.tipo_escola where t.tipo_turma = @tipoTurmaRegular ");

            if (semestre.HasValue)
                query.AppendLine(" and t.semestre = @semestre ");

            if (cicloId > 0)
                query.AppendLine(" and tc.id = @cicloId ");

            if (anoLetivo > 0)
                query.AppendLine(" and t.ano_letivo = @anoLetivo ");

            if (anos != null && anos.Length > 0)
                query.AppendLine(" and t.ano = ANY(@anos) ");

            if (turmasCodigo != null && turmasCodigo.Length > 0)
                query.AppendLine(" and t.turma_id = ANY(@turmasCodigo) ");

            if (parecerConclusivoId > 0)
                query.AppendLine(" and ccp.id = @parecerConclusivoId ");

            else if (parecerConclusivoId < 0)
                query.AppendLine(" and ccp.id is null ");

            if (modalidade.HasValue)
                query.AppendLine(" and t.modalidade_codigo = @modalidadeId ");

            if (!string.IsNullOrEmpty(dreCodigo))
                query.AppendLine(" and d.dre_id = @dreCodigo ");

            if (!string.IsNullOrEmpty(ueCodigo))
                query.AppendLine(" and u.ue_id = @ueCodigo ");

            query.AppendLine("order by d.id, u.id, t.id");

            var parametros = new
            {
                tipoTurmaRegular = TipoTurma.Regular,
                anoLetivo,
                dreCodigo,
                ueCodigo,
                modalidadeId = modalidade.HasValue ? (int)modalidade : 0,
                semestre = semestre ?? 0,
                cicloId,
                parecerConclusivoId,
                anos = anos?.ToList(),
                turmasCodigo
            };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryAsync<RelatorioParecerConclusivoRetornoDto>(query.ToString(), parametros);
        }

        public async Task<string> ObterDescricaoParecerEmAprovacao(string codigoAluno, int ano)
        {
            var query = $@"select ccp.nome from wf_aprovacao_parecer_conclusivo wpc
                                  inner join conselho_classe_aluno cca on cca.id = wpc.conselho_classe_aluno_id
                                  inner join conselho_classe_parecer ccp on ccp.id = wpc.conselho_classe_parecer_id
                           where cca.aluno_codigo = @codigoAluno and Extract('Year' from cca.criado_em) = @ano";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp);

            return await conexao.QueryFirstOrDefaultAsync<string>(query.ToString(), new { codigoAluno, ano });
        }
    }
}
