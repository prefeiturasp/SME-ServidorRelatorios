using System.Collections.Generic;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemComponentesPorTurmaQuery : IRequest<RelatorioSondagemComponentesPorTurmaRelatorioDto>
    {

        public IEnumerable<Aluno> alunos;
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long TurmaCodigo { get; set; }
        public ComponenteCurricularSondagemEnum ComponenteCurricular { get; set; }
        public ProficienciaSondagemEnum Proficiencia { get; set; }
        public int Semestre { get; set; }
        public int Bimestre { get; set; }
        public string UsuarioRF { get; set; }
        public string Ano { get; set; }
    }
}
