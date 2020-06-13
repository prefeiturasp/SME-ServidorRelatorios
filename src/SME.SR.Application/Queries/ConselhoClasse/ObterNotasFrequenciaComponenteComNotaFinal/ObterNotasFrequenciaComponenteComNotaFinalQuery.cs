using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterNotasFrequenciaComponenteComNotaFinalQuery : IRequest<ComponenteComNotaFinal>
    {
        public ComponenteCurricularPorTurma ComponenteCurricular { get; set; }

        public FrequenciaAluno FrequenciaAluno { get; set; }

        public PeriodoEscolar PeriodoEscolar { get; set; }

        public Turma Turma { get; set; }

        public IEnumerable<NotaConceitoBimestreComponente> NotasConselhoClasseAluno { get; set; }

        public IEnumerable<NotaConceitoBimestreComponente> NotasFechamentoAluno { get; set; }
    }
}
