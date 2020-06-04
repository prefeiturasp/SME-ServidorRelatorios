using MediatR;

namespace SME.SR.Application
{
    public class GamesQuery : IRequest<string>
    {
        public GamesQuery(int ano)
        {
            Ano = ano;
        }

        public int Ano { get; set; }
    }
}
