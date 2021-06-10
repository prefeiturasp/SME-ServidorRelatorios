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
    public class ArquivoRepository : IArquivoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public ArquivoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<ArquivoDto> ObterPorCodigo(Guid codigo)
        {
            var query = @" select arq.codigo, arq.tipo as Tipo, arq.nome as NomeOriginal, arq.tipo_conteudo as TipoConteudo from arquivo arq where arq.codigo = @codigo";

            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringSgp))
            {
                return await conexao.QueryFirstOrDefaultAsync<ArquivoDto>(query.ToString(), new { codigo });
            }
        }
    }
}
