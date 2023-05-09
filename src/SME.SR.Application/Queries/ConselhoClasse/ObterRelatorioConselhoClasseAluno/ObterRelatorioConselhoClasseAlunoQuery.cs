using MediatR;
using SME.SR.Data;

namespace SME.SR.Application
{
    public class ObterRelatorioConselhoClasseAlunoQuery : IRequest<RelatorioConselhoClasseArray>
    {
        public long FechamentoTurmaId { get; set; }
        public long ConselhoClasseId { get; set; }
        public string CodigoAluno { get; set; }
        public Usuario Usuario { get; set; }
        public string FrequenciaGlobal { get; set; }
    }
}


