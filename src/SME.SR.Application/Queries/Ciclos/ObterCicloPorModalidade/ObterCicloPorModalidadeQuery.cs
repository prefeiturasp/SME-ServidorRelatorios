using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterCicloPorModalidadeQuery : IRequest<CicloTurmaDto>
    {
        public ObterCicloPorModalidadeQuery(string ano, Modalidade modalidade)
        {
            Ano = ano;
            Modalidade = modalidade;
        }

        public string Ano { get; set; }
        public Modalidade Modalidade { get; set; }
    }
}
