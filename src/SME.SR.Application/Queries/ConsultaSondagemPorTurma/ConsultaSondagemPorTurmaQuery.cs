using MediatR;
using SME.SR.Infra.Dtos.SondagemTurmaEscritaEF;

namespace SME.SR.Application.Queries.ConsultaSondagemPorTurma
{
    public class ConsultaSondagemPorTurmaQuery : IRequest<ConsultaSondagemPorTurmaDto>
    {
        public ConsultaSondagemPorTurmaQuery(int turmaId, int proficienciaId, int componenteCurricularId, int modalidadeId, int ano, int anoLetivo, int semestre, string ueCodigo = null, int? bimestreId = null)
        {
            TurmaId = turmaId;
            ProficienciaId = proficienciaId;
            ComponenteCurricularId = componenteCurricularId;
            ModalidadeId = modalidadeId;
            Ano = ano;
            AnoLetivo = anoLetivo;
            Semestre = semestre;
            UeCodigo = ueCodigo;
            BimestreId = bimestreId;
        }

        public int TurmaId { get; set; }
        public int ProficienciaId { get; set; }
        public int ComponenteCurricularId { get; set; }
        public int ModalidadeId { get; set; }
        public int Ano { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public string UeCodigo { get; set; } = string.Empty;
        public int? BimestreId { get; set; }
    }
}
