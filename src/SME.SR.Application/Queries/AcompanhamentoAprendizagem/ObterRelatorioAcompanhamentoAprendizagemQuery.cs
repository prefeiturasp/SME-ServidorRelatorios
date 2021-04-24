using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQuery : IRequest<RelatorioAcompanhamentoAprendizagemDto>
    {
        public ObterRelatorioAcompanhamentoAprendizagemQuery(IEnumerable<AlunoRetornoDto> alunosEol, AcompanhamentoAprendizagemAlunoRetornoDto acompanhamentoAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> registrosIndividuais, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias)
        {
            AlunosEol = alunosEol;
            AcompanhamentoAlunos = acompanhamentoAlunos;
            FrequenciaAlunos = frequenciaAlunos;
            RegistrosIndividuais = registrosIndividuais;
            Ocorrencias = ocorrencias;
        }

        public IEnumerable<AlunoRetornoDto> AlunosEol { get; set; }
        public AcompanhamentoAprendizagemAlunoRetornoDto AcompanhamentoAlunos { get; set; }
        public IEnumerable<FrequenciaAluno> FrequenciaAlunos { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> RegistrosIndividuais { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> Ocorrencias { get; set; }

    }
}
