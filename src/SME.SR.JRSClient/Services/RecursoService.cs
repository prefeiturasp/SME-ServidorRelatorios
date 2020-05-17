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

        public async Task<DetalhesRecursoDto> ObterDetalhesRecurso(string caminhoRelatorio, bool expanded)
        {
            return await restService.GetDetalhesRecurso(caminhoRelatorio, expanded);
        }

        public async Task<DetalhesRecursoDto> Post(string caminhoRelatorio, bool criarCaminho, bool? sobrescrever)
        {
            return await restService.PostRecurso(caminhoRelatorio, criarCaminho, sobrescrever,
                    new DetalhesRecursoDto()
                    {
                        DataAtualizacao = DateTime.Now,
                        DataCriacao = DateTime.Now,
                        Descricao = "Testando",
                        Diretorio = "/teste/teste",
                        MascaraPermissao = "1",
                        Titulo = "Testando",
                        Versao = "1"
                    });
        }
    }
}
