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
            query.AppendLine(ObterQueryLocalUmaTurmaUmLocal());

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
                    proposta.LocalEncontro = await queryMultiple.ReadFirstOrDefaultAsync<PropostaLocal>();
                }

                return proposta;
            }
        }

        public async Task<PropostaCompleta> ObterPropostaCompleta(long propostaId)
        {
            var parametro = new { propostaId, situacaoPublicada = SITUACAO_PUBLICADA };
            var query = new StringBuilder();

            query.AppendLine(ObterQueryPropostaCompleta());
            query.AppendLine(ObterQueryPublicoAlvo() + ";");
            query.AppendLine(ObterQueryFuncaoEspecifica() + ";");
            query.AppendLine(ObterQueryCriterioDeValidacao());
            query.AppendLine(ObterQueryCriteriosCertificacao());
            query.AppendLine(ObterQueryRegentes());
            query.AppendLine(ObterQueryEncontros());
            query.AppendLine(ObterQueryVagasRemanecentes());
            query.AppendLine(ObterQueryTelefonesAreaPromotora());

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringConecta))
            {
                var queryMultiple = await conexao.QueryMultipleAsync(query.ToString(), parametro);
                var proposta = await queryMultiple.ReadFirstOrDefaultAsync<PropostaCompleta>();

                if (!(proposta is null) && proposta.Id > 0)
                {
                    proposta.PublicosAlvo = await queryMultiple.ReadAsync<PropostaPublicoAlvo>();
                    proposta.FuncaoEspecifica = await queryMultiple.ReadAsync<PropostaPublicoAlvo>();
                    proposta.CriteriosValidacao = await queryMultiple.ReadAsync<PropostaCriterioDeValidacao>();
                    proposta.CriteriosCertificacao = await queryMultiple.ReadAsync<PropostaCriterioCertificacao>();
                    proposta.Regentes = await queryMultiple.ReadAsync<PropostaRegente>();
                    proposta.Encontros = await queryMultiple.ReadAsync<PropostaLocal>();
                    proposta.VagasRemanecentes = await queryMultiple.ReadAsync<PropostaPublicoAlvo>();
                    proposta.TelefonesAreaPromotora = await queryMultiple.ReadAsync<AreaPromotoraTelefone>();
                }

                return proposta;
            }
        }

        private string ObterQueryPropostaCompleta()
        {
            var query = new StringBuilder();

            query.AppendLine(@"SELECT p.id, ap.nome as NomeAreaPromotora, ap.tipo as TipoAreaPromotora, 
                                     p.tipo_formacao as TipoFormacao, p.formato as Modalidade,
                                     p.justificativa, p.objetivos, p.conteudo_programatico as ConteudoProgramatico,
                                     p.procedimento_metodologico as Procedimentos, p.referencia as Referencias,
                                     p.descricao_atividade as DescricaoAtividade,
                                     p.nome_formacao as NomeFormacao, p.carga_horaria_presencial as CargaHorariaPresencial, 
                                     p.carga_horaria_sincrona as CargaHorariaSincrona, p.carga_horaria_distancia as CargaHorariaDistancia,
                                     p.data_realizacao_inicio as DataRealizacaoInicio, p.data_realizacao_fim as DataRealizacaoFim,
                                     p.quantidade_turmas as QuantidadeTurmas, p.quantidade_vagas_turma as QuantidadeVagasTurmas, 
                                     p.data_inscricao_inicio as DataInscricaoInicio, p.data_inscricao_fim as DataInscricaoFim,
                                     p.numero_homologacao as NumeroHomologacao, p.link_inscricoes_externa as LinkInscricaoExterna ");
            query.AppendLine("FROM proposta p ");
            query.AppendLine("INNER JOIN area_promotora ap ON ap.id = p.area_promotora_id ");
            query.AppendLine("WHERE p.id = @propostaId ");
            query.AppendLine(" AND NOT p.excluido; ");
         //   query.AppendLine(" AND p.situacao = @situacaoPublicada;");

            return query.ToString();
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
                                     p.numero_homologacao as NumeroHomologacao, p.link_inscricoes_externa as LinkInscricaoExterna ");
            query.AppendLine("FROM proposta p ");
            query.AppendLine("INNER JOIN area_promotora ap ON ap.id = p.area_promotora_id ");
            query.AppendLine("WHERE p.id = @propostaId ");
            query.AppendLine(" AND NOT p.excluido ");
            query.AppendLine(" AND p.situacao = @situacaoPublicada");
            query.AppendLine(" AND p.formacao_homologada = @homologada;");

            return query.ToString();
        }

        private string ObterQueryPublicoAlvoFuncao()
        {
            return @$"{ObterQueryPublicoAlvo()} UNION {ObterQueryFuncaoEspecifica()};";
        }

        private string ObterQueryPublicoAlvo()
        {
            return @"SELECT proposta_id as PropostaId, cf.nome as Nome 
                    FROM proposta_publico_alvo ppa
                    INNER JOIN cargo_funcao cf on cf.id = ppa.cargo_funcao_id
                    WHERE NOT ppa.excluido 
                      AND NOT cf.excluido
                      AND proposta_id = @propostaId";
        }

        private string ObterQueryFuncaoEspecifica()
        {
            return @"SELECT proposta_id as PropostaId, cf.nome as Nome 
                    FROM proposta_funcao_especifica pfe
                    INNER JOIN cargo_funcao cf on cf.id = pfe.cargo_funcao_id 
                    WHERE NOT pfe.excluido 
                      AND NOT cf.excluido
                      AND proposta_id = @propostaId";
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
            return @"SELECT proposta_id as PropostaId, nome_regente as Nome, 
                            registro_funcional as Rf, mini_biografia as MiniBio   
                     FROM proposta_regente
                     WHERE NOT excluido 
                       AND proposta_id = @propostaId;";
        }

        private string ObterQueryLocalUmaTurmaUmLocal()
        {
            return @"SELECT COUNT(pet.proposta_encontro_id) TotalTurmas, proposta_id, 
                        pe.local as Local, ped.data_inicio as DataInicio, ped.data_fim as DataFim, 
                        pe.hora_inicio as HoraInicio, pe.hora_fim as HoraFim 
                    FROM proposta_encontro pe
                    INNER JOIN proposta_encontro_turma pet ON pet.proposta_encontro_id = pe.id 
                    INNER JOIN proposta_encontro_data ped ON ped.proposta_encontro_id = pe.id
                    WHERE NOT pe.excluido 
                       AND NOT pet.excluido
                       AND NOT ped.excluido
                       AND pe.proposta_id = @propostaId
                    GROUP BY proposta_id, pe.local, ped.data_inicio, ped.data_fim, pe.hora_inicio, pe.hora_fim  
                    HAVING COUNT(pet.proposta_encontro_id) >= 1 AND COUNT(ped.proposta_encontro_id) = 1;";
        }

        private string ObterQueryEncontros()
        {
            return @"SELECT pt.nome, pe.proposta_id, pe.tipo, 
                        pe.local, ped.data_inicio, ped.data_fim, pe.hora_inicio, pe.hora_fim  
                    FROM proposta_encontro pe
                    INNER JOIN proposta_encontro_turma pet ON pet.proposta_encontro_id = pe.id 
                    INNER JOIN proposta_encontro_data ped ON ped.proposta_encontro_id = pe.id
                    INNER JOIN proposta_turma pt on pt.id = pet.turma_id 
                    WHERE NOT pe.excluido 
                       AND NOT pet.excluido
                       AND NOT ped.excluido
                       AND pe.proposta_id = @propostaId;";
        }

        private string ObterQueryVagasRemanecentes()
        {
            return @"SELECT proposta_id as PropostaId, cf.nome as Nome  
                   FROM proposta_vaga_remanecente pvr
                   INNER JOIN public.cargo_funcao cf ON cf.id = pvr.cargo_funcao_id 
                   WHERE NOT pvr.excluido 
                     AND NOT cf.excluido
                     AND proposta_id = @propostaId;";
        }

        private string ObterQueryTelefonesAreaPromotora()
        {
            return @"SELECT telefone 
                     FROM proposta p
                     INNER JOIN area_promotora ap on ap.id = p.area_promotora_id
                     INNER JOIN area_promotora_telefone apt on apt.area_promotora_id = ap.id
                     WHERE NOT ap.excluido
                       AND NOT apt.excluido
                       AND NOT p.excluido
                       AND p.id = @propostaId; ";
        }
    }
}
