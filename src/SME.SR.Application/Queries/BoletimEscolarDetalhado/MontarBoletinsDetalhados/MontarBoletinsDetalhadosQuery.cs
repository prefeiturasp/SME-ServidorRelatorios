using MediatR;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
   public class MontarBoletinsDetalhadosQuery : IRequest<BoletimEscolarDetalhadoDto>
    {
        public Dre Dre { get; set; }

        public Ue Ue { get; set; }

        public int UltimoBimestrePeriodoFechamento { get; set; }

        public IEnumerable<TipoCiclo> TiposCiclo { get; set; }

        public IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ComponentesCurriculares { get; set; }

        public IEnumerable<Turma> Turmas { get; set; }

        public IEnumerable<MediaFrequencia> MediasFrequencia { get; set; }

        public IEnumerable<RelatorioParecerConclusivoRetornoDto> PareceresConclusivos { get; set; }

        public IEnumerable<AlunoFotoArquivoDto> AlunosFoto { get; set; }

        public IEnumerable<RecomendacaoConselhoClasseAluno> RecomendacoesAlunos { get; set; }

        public IDictionary<string, string> TiposNota { get; set; }

        public IEnumerable<IGrouping<string, Aluno>> AlunosPorTuma { get; set; }

        public IEnumerable<IGrouping<string, NotasAlunoBimestre>> Notas { get; set; }

        public IEnumerable<IGrouping<string, FrequenciaAluno>> Frequencias { get; set; }

        public IEnumerable<IGrouping<string, FrequenciaAluno>> FrequenciasGlobal { get; set; }
        public int AnoLetivo { get; set; }
        public bool ExibirRecomendacao { get; set; }
        public Modalidade Modalidade { get; set; }
    }
}
