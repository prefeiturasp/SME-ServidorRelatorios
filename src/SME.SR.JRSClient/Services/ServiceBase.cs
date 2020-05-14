using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.JRSClient.Services
{
    public abstract class ServiceBase
    {
        protected readonly Configuracoes configuracoes;
        public ServiceBase(Configuracoes configuracoes)
        {
            this.configuracoes = configuracoes ?? throw new ArgumentNullException(nameof(configuracoes));
        }
        public string ObterCabecalhoAutenticacaoBasica()
        {
            return "basic " + $"{configuracoes.JasperLogin}:{configuracoes.JasperPassword}".EncodeTo64();
        }
    }
}
