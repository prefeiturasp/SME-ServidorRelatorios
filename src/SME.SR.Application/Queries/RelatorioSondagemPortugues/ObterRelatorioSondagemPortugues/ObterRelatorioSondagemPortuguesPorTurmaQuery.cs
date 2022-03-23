using System.Collections.Generic;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemPortuguesPorTurmaQuery : IRequest<IEnumerable<RelatorioSondagemPortuguesPorTurmaPlanilhaQueryDto>>
    {
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public int AnoLetivo { get; set; }
        public int AnoTurma { get; set; }
        public int Bimestre { get; set; }
        public ProficienciaSondagemEnum Proficiencia { get; set; }
        public GrupoSondagemEnum Grupo { get; set; }
        public int[] Modalidades { get; set; }
    }
}
