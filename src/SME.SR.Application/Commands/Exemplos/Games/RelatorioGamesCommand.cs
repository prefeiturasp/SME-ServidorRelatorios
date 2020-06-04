using MediatR;

namespace SME.SR.Application
{
    public class RelatorioGamesCommand : IRequest<bool>
    {
        public RelatorioGamesCommand(string nome)
        {
            Nome = nome;
        }

        public string Nome { get; set; }
    }
}
