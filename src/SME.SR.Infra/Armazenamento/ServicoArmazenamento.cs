using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace SME.SR.Infra
{
    public class ServicoArmazenamento : IServicoArmazenamento
    {
        private ConfiguracaoArmazenamentoOptions configuracaoArmazenamentoOptions;
        private readonly IConfiguration configuration;

        public ServicoArmazenamento(IOptions<ConfiguracaoArmazenamentoOptions> configuracaoArmazenamentoOptions, IConfiguration configuration)
        {
            this.configuracaoArmazenamentoOptions = configuracaoArmazenamentoOptions?.Value ?? throw new ArgumentNullException(nameof(configuracaoArmazenamentoOptions));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string Obter(string nomeArquivo)
        {
            var hostAplicacao = configuration["UrlFrontEnd"];
            return $"{hostAplicacao}{configuracaoArmazenamentoOptions.BucketArquivos}/{nomeArquivo}";
        }
    }
}
