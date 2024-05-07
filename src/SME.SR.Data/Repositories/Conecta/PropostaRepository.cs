using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models.Conecta;
using SME.SR.Infra;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PropostaRepository : IPropostaRepository
    {
        private const int SITUACAO_PUBLICADA = 1;
        private const int HOMOLOGADA = 1;

        private readonly VariaveisAmbiente variaveisAmbiente;

        public PropostaRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<Proposta> ObterProposta(long propostaId)
        {
            var parametro = new { propostaId, situacaoPublicada = SITUACAO_PUBLICADA, homologada = HOMOLOGADA };
            var query = new StringBuilder();

            query.AppendLine(ObterQueryProposta());
            query.AppendLine(ObterQueryPublicoAlvoFuncao());
            query.AppendLine(ObterQueryCriterioDeValidacao());
            query.AppendLine(ObterQueryCriteriosCertificacao());
            query.AppendLine(ObterQueryRegentes());

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringConecta))
            {
                var queryMultiple = await conexao.QueryMultipleAsync(query.ToString(), parametro);
                var proposta = await queryMultiple.ReadFirstOrDefaultAsync<Proposta>();

                if (!(proposta is null) && proposta.Id > 0)
                {
                    proposta.PublicosAlvo = await queryMultiple.ReadAsync<PropostaPublicoAlvo>();
                    proposta.CriteriosValidacao = await queryMultiple.ReadAsync<PropostaCriterioDeValidacao>();
                    proposta.CriteriosCertificacao = await queryMultiple.ReadAsync<PropostaCriterioCertificacao>();
                    proposta.Regentes = await queryMultiple.ReadAsync<PropostaRegente>();
                }

                return proposta;
            }
        }

        private string ObterQueryProposta()
        {
            var query = new StringBuilder();

            query.AppendLine(@"SELECT p.id,  ap.nome as NomeAreaPromotora, ap.tipo as TipoAreaPromotora, 
                                     p.nome_formacao as NomeFormacao, p.carga_horaria_presencial as CargaHorariaPresencial, 
                                     p.carga_horaria_sincrona as CargaHorariaSincrona, p.carga_horaria_distancia as CargaHorariaDistancia,
                                     p.data_realizacao_inicio as DataRealizacaoInicio, p.data_realizacao_fim as DataRealizacaoFim,
                                     p.quantidade_turmas as QuantidadeTurmas, p.quantidade_vagas_turma as QuantidadeVagasTurmas, 
                                     p.data_inscricao_inicio as DataInscricaoInicio, p.data_inscricao_fim as DataInscricaoFim,
                                     p.numero_homologacao as NumeroHomologacao, p.link_inscricoes_externa as LinkInscricaoExterna");
            query.AppendLine("FROM proposta p ");
            query.AppendLine("INNER JOIN area_promotora ap ON ap.id = p.area_promotora_id ");
            query.AppendLine("WHERE p.id = @propostaId ");
            query.AppendLine(" AND p.situacao = @situacaoPublicada");
            query.AppendLine(" AND p.formacao_homologada = @homologada");

            return query.ToString();
        }

        private string ObterQueryPublicoAlvoFuncao()
        {
            return @"SELECT proposta_id as PropostaId, cf.nome as Nome
                    FROM proposta_publico_alvo ppa
                    INNER JOIN cargo_funcao cf on cf.id = ppa.cargo_funcao_id
                    WHERE NOT ppa.excluido 
                      AND NOT cf.excluido
                      AND proposta_id = @propostaId
                    UNION
                    SELECT proposta_id as PropostaId, cf.nome as Nome 
                    FROM proposta_funcao_especifica pfe
                    INNER JOIN cargo_funcao cf on cf.id = pfe.cargo_funcao_id 
                    WHERE NOT pfe.excluido 
                      AND NOT cf.excluido
                      AND proposta_id = @propostaId;";
        }

        private string ObterQueryCriterioDeValidacao()
        {
            return @"SELECT proposta_id as PropostaId, cvi.nome as Nome
                    FROM proposta_criterio_validacao_inscricao pcv
                    INNER JOIN criterio_validacao_inscricao cvi on cvi.id = pcv.criterio_validacao_inscricao_id
                    WHERE NOT pcv.excluido 
                     AND NOT cvi.excluido
                     AND proposta_id = @propostaId;";
        }

        private string ObterQueryCriteriosCertificacao()
        {
            return @"SELECT proposta_id as PropostaId, descricao as Nome 
                    FROM proposta_criterio_certificacao pcc
                    INNER JOIN criterio_certificacao cc on cc.id = pcc.criterio_certificacao_id 
                    WHERE NOT pcc.excluido 
                      AND NOT cc.excluido 
                      AND proposta_id = @propostaId;";
        }

        private string ObterQueryRegentes()
        {
            return @"SELECT proposta_id as PropostaId, nome_regente as Nome   
                     FROM proposta_regente
                     WHERE NOT excluido 
                       AND proposta_id = @propostaId;";
        }
    }
}
