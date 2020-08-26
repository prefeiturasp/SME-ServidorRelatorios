using MediatR;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
  public  class MontarHistoricoEscolarTransferenciaQuery : IRequest<IEnumerable<TransferenciaDto>>
    {
        public MontarHistoricoEscolarTransferenciaQuery(IEnumerable<AreaDoConhecimento> areasConhecimento,
                                            IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurricularesTurmas,
                                            IEnumerable<AlunoTurmasHistoricoEscolarDto> alunosTurmas,
                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                            IEnumerable<IGrouping<string, NotasAlunoBimestre>> notas,
                                            IEnumerable<IGrouping<string, FrequenciaAluno>> frequencias,
                                            string[] turmasCodigo)
        {
            AreasConhecimento = areasConhecimento;
            ComponentesCurricularesTurmas = componentesCurricularesTurmas;
            AlunosTurmas = alunosTurmas;
            TurmasCodigo = turmasCodigo;
            Notas = notas;
            Frequencias = frequencias;
            MediasFrequencia = mediasFrequencia;
        }

        public IEnumerable<IGrouping<string, NotasAlunoBimestre>> Notas { get; set; }
        public IEnumerable<IGrouping<string, FrequenciaAluno>> Frequencias { get; set; }
        public IEnumerable<MediaFrequencia> MediasFrequencia { get; set; }
        public IEnumerable<AreaDoConhecimento> AreasConhecimento { get; set; }
        public IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ComponentesCurricularesTurmas { get; set; }
        public IEnumerable<AlunoTurmasHistoricoEscolarDto> AlunosTurmas { get; set; }
        public string[] TurmasCodigo { get; set; }
    }
}
