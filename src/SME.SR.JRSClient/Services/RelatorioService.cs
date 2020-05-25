using Refit;
using SME.SR.Infra.Dtos;
using SME.SR.JRSClient.Grupos;
using SME.SR.JRSClient.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SME.SR.JRSClient.Services
{
    public class RelatorioService : ServiceBase<IReports>, IRelatorioService
    {
        public RelatorioService(Configuracoes configuracoes) : base(configuracoes)
        {
        }
        
        public async Task<Stream> GetRelatorioSincrono(RelatorioSincronoDto Dto)
        {
            if (string.IsNullOrWhiteSpace(Dto.CaminhoRelatorio) || (int)Dto.Formato == 0)
                throw new Exception("O Caminho do relatorio e o formato devem ser informados");

            if(Dto.CaminhoRelatorio.StartsWith("/"))
                Dto.CaminhoRelatorio = Dto.CaminhoRelatorio.Substring(1);
            
            return await restService.GetRelatorioSincrono(Dto.CaminhoCompleto, Dto);
        }
    }
}
