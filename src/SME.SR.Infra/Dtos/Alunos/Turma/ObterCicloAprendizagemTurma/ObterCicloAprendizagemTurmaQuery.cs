using MediatR;

namespace SME.SR.Application
{
    public class ObterCicloAprendizagemTurmaQuery: IRequest<string>
    {
        public ObterCicloAprendizagemTurmaQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; set; }
    }
}
