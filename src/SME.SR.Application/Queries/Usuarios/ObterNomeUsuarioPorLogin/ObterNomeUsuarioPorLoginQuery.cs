using MediatR;

namespace SME.SR.Application
{
    public class ObterNomeUsuarioPorLoginQuery : IRequest<string>
    {
        public ObterNomeUsuarioPorLoginQuery(string login)
        {
            Login = login;
        }

        public string Login { get; set; }
    }
}
