using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces.Conecta;
using SME.SR.Infra;
using SME.SR.Infra.Dtos.Relatorios.Conecta;
using System.Threading.Tasks;

namespace SME.SR.Data.Repositories.Conecta
{
    public class RelatorioCodafRepository : IRelatorioCodafRepository
    {
        private readonly VariaveisAmbiente _variaveisAmbiente;
        private readonly string _conectaStringConnection;

        public RelatorioCodafRepository(VariaveisAmbiente variaveisAmbiente)
        {
            _variaveisAmbiente = variaveisAmbiente;
            _conectaStringConnection = variaveisAmbiente.ConnectionStringConecta;
        }

        public async Task<DadosPrincipaisRelatorioCodafDto> ObterDadosRelatorioAsync(long codafId)
        {
            var sql = @"
            -- Dados Principais Das Turmas
            SELECT DISTINCT
                   CLP.ID AS codafId,
                   PT.ID AS turmaId,
                   PT.NOME AS nomeTurma,
                   p.QUANTIDADE_VAGAS_TURMA AS quantidadeVagasTurma,
                   AP.NOME AS nomeAreaPromotora,
                   P.TIPO_FORMACAO AS tipoFormacao, -- 1-Curso; 2-Evento
                   P.NOME_FORMACAO AS nomeFormacao,
                   P.QUANTIDADE_TURMAS AS quantidadeTurmas,
                   COALESCE(PGP.DATA_INICIO, P.DATA_REALIZACAO_INICIO)
                        AS periodoRealizacaoInicio,
                   COALESCE(PGP.DATA_FIM, P.DATA_REALIZACAO_FIM) 
                        AS periodoRealizacaoFim,
                   P.CURSO_COM_CERTIFICADO AS cursoComCertificado,
                   p.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                   p.CODIGO_EVENTO_SIGPEC AS codigoEventoSigpec,
                   p.HORAS_TOTAIS AS cargaHorariaTotal,
                   p.CARGA_HORARIA_DISTANCIA AS cargaHorariaDistancia,
                   p.CARGA_HORARIA_SINCRONA AS cargaHorariaPresencial,
                   p.CARGA_HORARIA_PRESENCIAL AS cargaHorariaSincrona,
                   P.FORMATO AS tipoFormato, -- 1-Presencial;2-A Distância;3-Híbrido
                   CLP.NUMERO_COMUNICADO AS numeroComunicado,
                   CLP.DATA_PUBLICACAO AS dataPublicacao,
                   CLP.DATA_PUBLICACAO_DOM AS dataPublicacaoDom,
                   clp.PAGINA_COMUNICADO_DOM AS paginaComunicadoDom,
                   CASE
       		            WHEN D.DRE_ID IS NULL THEN ''
       		            ELSE D.NOME 
                   END AS nomeDre,
                    CLP.OBSERVACAO
            FROM   PUBLIC.PROPOSTA AS P
                   INNER JOIN PUBLIC.PROPOSTA_TURMA AS PT ON PT.PROPOSTA_ID = P.ID
                   INNER JOIN PUBLIC.AREA_PROMOTORA AS AP ON AP.ID = P.AREA_PROMOTORA_ID
	               INNER JOIN PUBLIC.CODAF_LISTA_PRESENCA AS CLP ON CLP.PROPOSTA_TURMA_ID = PT.ID
                   INNER JOIN PUBLIC.PROPOSTA_DRE AS PD ON PD.PROPOSTA_ID = P.ID 
	               INNER JOIN PUBLIC.DRE AS D ON D.ID = PD.DRE_ID 
                   LEFT JOIN PUBLIC.PROPOSTA_GRUPO_PERIODO_TURMA PGPT ON PGPT.PROPOSTA_TURMA_ID = PT.ID AND NOT PGPT.EXCLUIDO
                   LEFT JOIN PUBLIC.PROPOSTA_GRUPO_PERIODO PGP ON PGP.ID = PGPT.GRUPO_PERIODO_ID AND NOT PGP.EXCLUIDO

            WHERE  CLP.ID = @codafId;

            -- Data das Aulas
            SELECT PED.DATA_INICIO AS dataInicio,
                   PED.DATA_FIM AS dataFim
            FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                   INNER JOIN PUBLIC.PROPOSTA_ENCONTRO_TURMA AS PET ON PET.TURMA_ID = CLP.PROPOSTA_TURMA_ID
                   INNER JOIN PUBLIC.PROPOSTA_ENCONTRO AS PE ON PE.ID = PET.PROPOSTA_ENCONTRO_ID 
                   INNER JOIN PUBLIC.PROPOSTA_ENCONTRO_DATA AS PED ON PED.PROPOSTA_ENCONTRO_ID = PE.ID 
            WHERE  CLP.ID = @codafId
              AND  PE.TIPO IN (@presencial, @sincrono)
              AND NOT PE.EXCLUIDO 
              AND NOT PET.EXCLUIDO 
              AND NOT PED.EXCLUIDO;

            -- Dados dos Regentes
            SELECT coalesce(U.NOME, PR.NOME_REGENTE) AS nome,
                   PR.REGISTRO_FUNCIONAL AS registroFuncional,
                   U.ID AS numeroRegistro
            FROM   PUBLIC.CODAF_LISTA_PRESENCA AS CLP
                   INNER JOIN PUBLIC.PROPOSTA_REGENTE_TURMA AS PRT ON PRT.TURMA_ID = CLP.PROPOSTA_TURMA_ID
                   INNER JOIN PUBLIC.PROPOSTA_REGENTE AS PR ON PR.ID = PRT.PROPOSTA_REGENTE_ID 
                   LEFT JOIN PUBLIC.USUARIO AS U ON U.LOGIN = PR.REGISTRO_FUNCIONAL 
            WHERE  CLP.ID = @codafId;


            -- Dados dos Participantes
            SELECT U.LOGIN AS documento,
                   (U.LOGIN <> U.CPF) AS temRf,
                   U.NOME,
                   CILP.APROVADO,
                   CILP.ATIVIDADE_OBRIGATORIO AS atividadeObrigatoria,
                   CILP.CONCEITO_FINAL AS conceitoFinal,
                   CILP.PERCENTUAL_FREQUENCIA AS percentualFrequencia,
                   CC.CODIGO_CERTIFICADO AS codigoCertificado
            FROM   PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CILP
                   INNER JOIN PUBLIC.INSCRICAO AS I ON I.ID = CILP.INSCRICAO_ID
                   INNER JOIN PUBLIC.USUARIO AS U ON U.ID = I.USUARIO_ID
                   LEFT JOIN PUBLIC.CODAF_CERTIFICADOS AS CC ON CC.CODAF_INSCRICAO_LISTA_PRESENCA_ID = CILP.ID 
            WHERE  CILP.CODAF_LISTA_PRESENCA_ID  = @codafId;

            SELECT CRLP.DATA_RETIFICACAO AS DATA, CRLP.PAGINA_RETIFICACAO_DOM AS pagina
            FROM PUBLIC.CODAF_RETIFICACAO_LISTA_PRESENCA AS CRLP
            WHERE CRLP.CODAF_LISTA_PRESENCA_ID = @codafId;";

            var parametros = new
            {
                codafId,
                presencial = 0,
                sincrono = 1
            };

            using (var conn = new NpgsqlConnection(_conectaStringConnection))
            {
                using (var multi = await conn.QueryMultipleAsync(sql, parametros))
                {
                    var dadosRelatorio = await multi.ReadFirstOrDefaultAsync<DadosPrincipaisRelatorioCodafDto>();

                    if (dadosRelatorio == null) return null;

                    dadosRelatorio.DataAulas = await multi.ReadAsync<DataAulaTurmaRelatorioCodafDto>();

                    dadosRelatorio.RegentesTurma = await multi.ReadAsync<DadosRegenteTurmaRelatorioCodafDto>();

                    dadosRelatorio.Participantes = await multi.ReadAsync<DadosParticipanteRelatorioCodafDto>();

                    dadosRelatorio.Retificacoes = await multi.ReadAsync<DadosRetificacaoRelatorioCodafDto>();

                    return dadosRelatorio;
                }
            }
        }
    }
}
