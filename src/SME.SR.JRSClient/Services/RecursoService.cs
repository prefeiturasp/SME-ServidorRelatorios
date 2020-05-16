using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class RecursoService : ServiceBase<IReports>, IRecursoService
    {
        public RecursoService(Configuracoes configuracoes) : base(configuracoes)
        {
        }

        public async Task<BuscaRepositorioRespostaDto> BuscarRepositorio(BuscaRepositorioRequisicaoDto requisicaoDto)
        {
            return await restService.GetRecursos(requisicaoDto);
        }
    }
}
