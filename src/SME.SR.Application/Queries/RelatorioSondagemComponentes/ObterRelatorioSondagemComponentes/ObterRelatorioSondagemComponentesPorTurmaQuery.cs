using System.Collections.Generic;
using MediatR;
using SME.SR.Infra;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemComponentesPorTurmaQuery : IRequest<RelatorioSondagemComponentesPorTurmaRelatorioDto>
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public string ComponenteCurricularId { get; set; }
        public int ProficienciaId { get; set; }
        public int Semestre { get; set; }
        public string UsuarioRF { get; set; }
    }
}
