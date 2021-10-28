using Dapper;
using Npgsql;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class AlunoFotoRepository : IAlunoFotoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public AlunoFotoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<AlunoFotoArquivoDto>> ObterFotosDoAlunoPorCodigos(string[] codigosAluno)
        {
            var query = new StringBuilder(@"SELECT af.aluno_codigo CodigoAluno, a.id, a.nome NomeOriginal, a.codigo, a.tipo, a.tipo_conteudo as TipoArquivo
                                            FROM aluno_foto af 
                                            INNER JOIN arquivo a on af.arquivo_id = a.id 
                                            WHERE af.aluno_codigo = ANY(@codigosAluno) ");

            var parametros = new { codigosAluno };

            var lstFotosAluno = new List<AlunoFotoArquivoDto>();

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgpConsultas))
            {
                await conexao.QueryAsync<AlunoFotoArquivoDto, ArquivoDto, AlunoFotoArquivoDto>(query.ToString(),
                 (alunoFoto,  arquivoDto) =>
                 {
                     alunoFoto.ArquivoDto = arquivoDto;
                     lstFotosAluno.Add(alunoFoto);

                     return alunoFoto;
                 }, param: parametros);

            }

            return lstFotosAluno;
        }
    }
}
