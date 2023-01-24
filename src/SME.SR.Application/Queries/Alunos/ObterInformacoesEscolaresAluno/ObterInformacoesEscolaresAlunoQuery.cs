using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterInformacoesEscolaresAlunoQuery : IRequest<InformacaoEscolarAlunoDto>
    {

        public ObterInformacoesEscolaresAlunoQuery(string codigoAluno, string turmaId)
        {
            CodigoAluno = codigoAluno;
            TurmaCodigo = turmaId;
        }
        public string CodigoAluno { get; set; }
        public string TurmaCodigo { get; set; }
    }
}