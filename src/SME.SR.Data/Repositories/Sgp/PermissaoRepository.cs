using Dapper;
using Npgsql;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class PermissaoRepository : IPermissaoRepository
    {
        private readonly VariaveisAmbiente variaveisAmbiente;

        public PermissaoRepository(VariaveisAmbiente variaveisAmbiente)
        {
            this.variaveisAmbiente = variaveisAmbiente ?? throw new ArgumentNullException(nameof(variaveisAmbiente));
        }

        public async Task<IEnumerable<GrupoAbrangenciaApiEol>> ObterTodosGrupos()
        {
            using (var conexao = new NpgsqlConnection(variaveisAmbiente.ConnectionStringApiEol))
            {
                var grupos = new Dictionary<Guid, GrupoAbrangenciaApiEol>();

                var query = @"SELECT g.id as Grupo, guidperfil as GrupoID, 
                            cdtipofuncaoatividade as TipoFuncaoAtividade, 
                            idabrangencia as Abrangencia, 
                            ehperfilmanual, 
                            cargo
                    FROM public.grupos g
                    LEFT JOIN public.grupocargos gc on g.id = gc.idgrupo ";

                var count = 0;
                await conexao.QueryAsync<GrupoAbrangenciaApiEol, int?, GrupoAbrangenciaApiEol>(query, (grupo, cargoId) =>
                 {
                     count = count + 1;
                     GrupoAbrangenciaApiEol grupoPerfilApiEol;
                     if (!grupos.TryGetValue(grupo.GrupoID, out grupoPerfilApiEol))
                     {
                         grupoPerfilApiEol = grupo;
                         grupos.Add(grupo.GrupoID, grupoPerfilApiEol);
                     }
                     grupoPerfilApiEol.AdicionarCargo(cargoId);
                     return grupoPerfilApiEol;
                 }, splitOn: "cargo");
                return grupos.Select(c => c.Value);
            }
        }
    }
}
