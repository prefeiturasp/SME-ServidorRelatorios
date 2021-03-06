﻿using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterRelatorioAcompanhamentoAprendizagemQuery : IRequest<RelatorioAcompanhamentoAprendizagemDto>
    {
        public ObterRelatorioAcompanhamentoAprendizagemQuery(Turma turma, IEnumerable<AlunoRetornoDto> alunosEol, IEnumerable<ProfessorTitularComponenteCurricularDto> professores, IEnumerable<AcompanhamentoAprendizagemTurmaDto> acompanhamentoTurma, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> registrosIndividuais, IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> ocorrencias, FiltroRelatorioAcompanhamentoAprendizagemDto filtro, IEnumerable<QuantidadeAulasDadasBimestreDto> quantidadeAulasDadas)
        {
            Turma = turma;
            AlunosEol = alunosEol;
            Professores = professores;
            AcompanhamentoTurma = acompanhamentoTurma;
            FrequenciaAlunos = frequenciaAlunos;
            RegistrosIndividuais = registrosIndividuais;
            Ocorrencias = ocorrencias;
            Filtro = filtro;
            QuantidadeAulasDadas = quantidadeAulasDadas;
        }

        public Turma Turma { get; set; }
        public IEnumerable<AlunoRetornoDto> AlunosEol { get; set; }
        public IEnumerable<ProfessorTitularComponenteCurricularDto> Professores { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemTurmaDto> AcompanhamentoTurma { get; set; }
        public IEnumerable<FrequenciaAluno> FrequenciaAlunos { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemRegistroIndividualDto> RegistrosIndividuais { get; set; }
        public IEnumerable<AcompanhamentoAprendizagemOcorrenciaDto> Ocorrencias { get; set; }
        public FiltroRelatorioAcompanhamentoAprendizagemDto Filtro { get; set; }
        public IEnumerable<QuantidadeAulasDadasBimestreDto> QuantidadeAulasDadas { get; set; }

    }
}
