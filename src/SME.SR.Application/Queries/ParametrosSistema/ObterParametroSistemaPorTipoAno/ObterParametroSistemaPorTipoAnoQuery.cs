using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterParametroSistemaPorTipoAnoQuery : IRequest<string>
    {
        public ObterParametroSistemaPorTipoAnoQuery(int ano, TipoParametroSistema tipo)
        {
            Ano = ano;
            Tipo = tipo;
        }

        public int Ano { get; set; }
        public TipoParametroSistema Tipo { get; set; }
    }
}
