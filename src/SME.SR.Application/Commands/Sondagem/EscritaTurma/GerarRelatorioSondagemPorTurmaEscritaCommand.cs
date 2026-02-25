using MediatR;
using SME.SR.Infra;
using System;

namespace SME.SR.Application.Commands.Sondagem.EscritaTurma
{
    public class GerarRelatorioSondagemPorTurmaEscritaCommand : IRequest
    {
        public GerarRelatorioSondagemPorTurmaEscritaCommand(Guid codigoCorrelacao, int turmaId, int proficienciaId, int componenteCurricularId, 
            Modalidade modalidade, int ano, int anoLetivo, int semestre, string nomeUsuarioSolicitacao,string ueCodigo = null, int? bimestreId = null)
        {
            CodigoCorrelacao = codigoCorrelacao;
            TurmaId = turmaId;
            ProficienciaId = proficienciaId;
            ComponenteCurricularId = componenteCurricularId;
            Modalidade = modalidade;
            Ano = ano;
            AnoLetivo = anoLetivo;
            Semestre = semestre;
            UeCodigo = ueCodigo;
            BimestreId = bimestreId;
            NomeUsuarioSolicitacao = nomeUsuarioSolicitacao;
        }

        public Guid CodigoCorrelacao { get; set; }
        public int TurmaId { get; set; }
        public int ProficienciaId { get; set; }
        public int ComponenteCurricularId { get; set; }
        public Modalidade Modalidade { get; set; }
        public int Ano { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public string UeCodigo { get; set; } = string.Empty;
        public int? BimestreId { get; set; }
        public string NomeUsuarioSolicitacao { get; set; }
    }
}
