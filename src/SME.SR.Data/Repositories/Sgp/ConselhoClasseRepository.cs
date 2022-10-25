using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
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

        public async Task<IEnumerable<int>> ObterBimestresPorAlunoCodigo(string codigoAluno, int anoLetivo, Modalidade modalidade, int semestre)
        {
            var query = @"select coalesce(pe.bimestre , 0) as Bimestre
                          from conselho_classe_aluno cca 
                         inner join conselho_classe cc on cc.id = cca.conselho_classe_id 
                         inner join fechamento_turma ft on ft.id = cc.fechamento_turma_id 
                          left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
                         inner join turma t on t.id = ft.turma_id
                         where cca.aluno_codigo  = @codigoAluno
                           and t.ano_letivo  = @anoLetivo
                           and t.modalidade_codigo  = @modalidade
                           and t.semestre = @semestre";

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<int>(query, new { codigoAluno, anoLetivo, modalidade, semestre });
        }

        public async Task<long> ObterConselhoPorFechamentoTurmaId(long fechamentoTurmaId)
        {
            var query = ConselhoClasseConsultas.ConselhoPorFechamentoId;
            var parametros = new { FechamentoTurmaId = fechamentoTurmaId };

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
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

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<long>(query, parametros);
        }
        public async Task<IEnumerable<TotalAulasTurmaDisciplinaDto>> ObterTotalAulasSemFrequenciaPorTurmaBismetre(string[] discplinasId, string[] codigoTurma, int[] bimestres)
        {
            var query = @" select a.disciplina_id as ComponenteCurricularId,
                                  a.turma_id as TurmaCodigo,
                                  COALESCE(SUM(quantidade), 0) as totalAulas 
                            from aula a
                            join  componente_curricular cc on cc.id = a.disciplina_id::int8 
                            join  periodo_escolar pe on pe.tipo_calendario_id = a.tipo_calendario_id::int8  
                            where cc.permite_registro_frequencia  = false 
                            and a.turma_id = any(@codigoTurma)
                            and not a.excluido 
                            and a.disciplina_id = any(@discplinasId) 
                            and pe.bimestre = any(@bimestres) 
                            and pe.periodo_inicio <= a.data_aula and pe.periodo_fim >= a.data_aula
                        group by a.disciplina_id,a.turma_id,a.quantidade"
            ;

            var parametros = new {  codigoTurma, discplinasId, bimestres };

            using var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas);

            return await conexao.QueryAsync<TotalAulasTurmaDisciplinaDto>(query, parametros);
        }
    }
}
