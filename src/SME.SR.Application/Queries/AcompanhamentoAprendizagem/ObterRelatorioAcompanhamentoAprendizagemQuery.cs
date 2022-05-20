using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQuery : IRequest<RelatorioAcompanhamentoAprendizagemDto>
    {
        public ObterRelatorioAcompanhamentoAprendizagemQuery(Turma turma, IEnumerable<AlunoRetornoDto> alunosEol, IEnumerable<ProfessorTitularComponenteCurricularDto> professores, IEnumerable<AcompanhamentoAprendizagemAlunoDto> acompanhamentoAlunos, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias, FiltroRelatorioAcompanhamentoAprendizagemDto filtro, IEnumerable<QuantidadeAulasDadasBimestreDto> quantidadeAulasDadas, long periodoId, bool relatorioEscolaAqui)
        {
            Turma = turma;
            AlunosEol = alunosEol;
            Professores = professores;
            AcompanhamentoTurma = acompanhamentoAlunos;
            FrequenciaAlunos = frequenciaAlunos;
            Ocorrencias = ocorrencias;
            Filtro = filtro;
            QuantidadeAulasDadas = quantidadeAulasDadas;
            PeriodoId = periodoId;
            RelatorioEscolaAqui = relatorioEscolaAqui;
        }

        public Turma Turma { get; set; }
        public IEnumerable<AlunoRetornoDto> AlunosEol { get; set; }
        public IEnumerable<ProfessorTitularComponenteCurricularDto> Professores { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemAlunoDto> AcompanhamentoTurma { get; set; }
        public IEnumerable<FrequenciaAluno> FrequenciaAlunos { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> Ocorrencias { get; set; }
        public FiltroRelatorioAcompanhamentoAprendizagemDto Filtro { get; set; }
        public IEnumerable<QuantidadeAulasDadasBimestreDto> QuantidadeAulasDadas { get; set; }
        public long PeriodoId { get; set; }
        public bool RelatorioEscolaAqui { get; set; }

    }
}
