using MediatR;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class MontarHistoricoEscolarQuery : IRequest<IEnumerable<HistoricoEscolarDTO>>
    {
        public MontarHistoricoEscolarQuery(Dre dre, Ue ue, IEnumerable<AreaDoConhecimento> areasConhecimento,
                                                 IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurricularesTurmas,
                                                 IEnumerable<AlunoTurmasHistoricoEscolarDto> alunosTurmas,
                                                 IEnumerable<MediaFrequencia> mediasFrequencia,
                                                 IEnumerable<IGrouping<string, NotasAlunoBimestre>> notas,
                                                 IEnumerable<IGrouping<string, FrequenciaAluno>> frequencias,
                                                 IDictionary<string, string> tiposNota,
                                                 string[] turmasCodigo, CabecalhoDto cabecalho, LegendaDto legenda)
        {
            Dre = dre;
            Ue = ue;
            AreasConhecimento = areasConhecimento;
            ComponentesCurricularesTurmas = componentesCurricularesTurmas;
            AlunosTurmas = alunosTurmas;
            TurmasCodigo = turmasCodigo;
            Cabecalho = cabecalho;
            Notas = notas;
            Frequencias = frequencias;
            MediasFrequencia = mediasFrequencia;
            TiposNota = tiposNota;
            Legenda = legenda;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<IGrouping<string, NotasAlunoBimestre>> Notas { get; set; }
        public IEnumerable<IGrouping<string, FrequenciaAluno>> Frequencias { get; set; }
        public IEnumerable<MediaFrequencia> MediasFrequencia { get; set; }
        public IEnumerable<AreaDoConhecimento> AreasConhecimento { get; set; }
        public IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ComponentesCurricularesTurmas { get; set; }
        public IEnumerable<AlunoTurmasHistoricoEscolarDto> AlunosTurmas { get; set; }
        public IDictionary<string, string> TiposNota { get; set; }
        public string[] TurmasCodigo { get; set; }
        public CabecalhoDto Cabecalho { get; set; }
        public LegendaDto Legenda { get; set; }
    }
}
