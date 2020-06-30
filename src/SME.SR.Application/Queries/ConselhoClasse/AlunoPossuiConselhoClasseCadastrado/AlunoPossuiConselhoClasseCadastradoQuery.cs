using MediatR;

namespace SME.SR.Application
{
    public class AlunoPossuiConselhoClasseCadastradoQuery : IRequest<bool>
    {
        public long ConselhoClasseId { get; set; }
        public string CodigoAluno { get; set; }
    }
}
