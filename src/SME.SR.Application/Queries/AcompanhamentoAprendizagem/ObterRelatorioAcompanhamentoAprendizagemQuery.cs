using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQuery : IRequest<RelatorioAcompanhamentoAprendizagemDto>
    {
        public ObterRelatorioAcompanhamentoAprendizagemQuery(Turma turma, IEnumerable<AlunoRetornoDto> alunosEol, IEnumerable<ProfessorTitularComponenteCurricularDto> professores, IEnumerable<AcompanhamentoAprendizagemAlunoRetornoDto> acompanhamentosAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> registrosIndividuais, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias, FiltroRelatorioAcompanhamentoAprendizagemDto filtro)
        {
            Turma = turma;
            AlunosEol = alunosEol;
            Professores = professores;
            AcompanhamentosAlunos = acompanhamentosAlunos;
            FrequenciaAlunos = frequenciaAlunos;
            RegistrosIndividuais = registrosIndividuais;
            Ocorrencias = ocorrencias;
            Filtro = filtro;
        }

        public Turma Turma { get; set; }
        public IEnumerable<AlunoRetornoDto> AlunosEol { get; set; }
        public IEnumerable<ProfessorTitularComponenteCurricularDto> Professores { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemAlunoRetornoDto> AcompanhamentosAlunos { get; set; }
        public IEnumerable<FrequenciaAluno> FrequenciaAlunos { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> RegistrosIndividuais { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> Ocorrencias { get; set; }
        public FiltroRelatorioAcompanhamentoAprendizagemDto Filtro { get; set; }

    }
}
