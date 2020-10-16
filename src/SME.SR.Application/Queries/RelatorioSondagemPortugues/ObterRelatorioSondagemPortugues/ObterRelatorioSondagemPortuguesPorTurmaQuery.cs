using System.Collections.Generic;
using MediatR;
using SME.SR.Data;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemPortuguesPorTurmaQuery : IRequest<RelatorioSondagemPortuguesPorTurmaRelatorioDto>
    {
        public IEnumerable<Aluno> Alunos { get; set; }
        public RelatorioSondagemPortuguesPorTurmaCabecalhoDto Cabecalho { get; set; }
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public long TurmaCodigo { get; set; }
        public ComponenteCurricularSondagemEnum ComponenteCurricular { get; set; }
        public ProficienciaSondagemEnum Proficiencia { get; set; }
        public int Semestre { get; set; }
        public string UsuarioRF { get; set; }
        public int Ano { get; set; }
    }
}
