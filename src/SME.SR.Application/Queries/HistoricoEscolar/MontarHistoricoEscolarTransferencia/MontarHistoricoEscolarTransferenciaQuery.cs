﻿using MediatR;
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
                                            IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao,
                                            IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurricularesTurmas,
                                            IEnumerable<AlunoTurmasHistoricoEscolarDto> alunosTurmas,
                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                            IEnumerable<IGrouping<string, NotasAlunoBimestre>> notas,
                                            IEnumerable<IGrouping<string, FrequenciaAluno>> frequencias,
                                            IEnumerable<TipoNotaCicloAno> tiposNota,
                                            string[] turmasCodigo, LegendaDto legenda, IEnumerable<RegistroFrequenciaAlunoDto> registroFrequenciasAlunos, int bimestreAtual)
        {
            AreasConhecimento = areasConhecimento;
            ComponentesCurricularesTurmas = componentesCurricularesTurmas;
            GrupoAreaOrdenacao = grupoAreaOrdenacao;
            AlunosTurmas = alunosTurmas;
            TurmasCodigo = turmasCodigo;
            TiposNota = tiposNota;
            Notas = notas;
            Frequencias = frequencias;
            MediasFrequencia = mediasFrequencia;
            Legenda = legenda;
            RegistroFrequenciasAlunos = registroFrequenciasAlunos;
            BimestreAtual = bimestreAtual;
        }

        public IEnumerable<IGrouping<string, NotasAlunoBimestre>> Notas { get; set; }
        public IEnumerable<RegistroFrequenciaAlunoDto> RegistroFrequenciasAlunos { get; set; }
        public IEnumerable<IGrouping<string, FrequenciaAluno>> Frequencias { get; set; }
        public IEnumerable<MediaFrequencia> MediasFrequencia { get; set; }
        public IEnumerable<AreaDoConhecimento> AreasConhecimento { get; set; }
        public IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> GrupoAreaOrdenacao { get; set; }
        public IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ComponentesCurricularesTurmas { get; set; }
        public IEnumerable<AlunoTurmasHistoricoEscolarDto> AlunosTurmas { get; set; }
        public IEnumerable<TipoNotaCicloAno> TiposNota { get; set; }
        public string[] TurmasCodigo { get; set; }
        public LegendaDto Legenda { get; set; }
        public int BimestreAtual { get; set; }
    }
}
