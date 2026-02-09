using System;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Relatorios.Conecta
{
    public class DadosPrincipaisRelatorioCodafDto
    {
        public long CodafId { get; set; }
        public long TurmaId { get; set; }
        public string NomeTurma { get; set; }
        public int QuantidadeVagasTurma { get; set; }
        public string NomeAreaPromotora { get; set; }
        public TipoFormacaoConecta TipoFormacao { get; set; }
        public string NomeFormacao { get; set; }
        public int QuantidadeTurmas { get; set; }
        public DateTime PeriodoRealizacoInicio { get; set; }
        public DateTime PeriodoRealizacoFim { get; set; }
        public bool CursoComCertificado { get; set; }
        public int NumeroHomologacao { get; set; }
        public int CodigoEventoSigpec { get; set; }
        public int CargaHorariaTotal { get; set; }
        public string CargaHorariaDistancia { get; set; }
        public string CargaHorariaPresencial { get; set; }
        public string CargaHorariaSincrona { get; set; }
        public TipoFormatoConecta TipoFormato { get; set; }
        public short NumeroComunicado { get; set; }
        public DateTime DataPublicacao { get; set; }
        public DateTime DataPublicacaoDom { get; set; }
        public short PaginaComunicadoDom { get; set; }
        public string NomeDre { get; set; }
        public string Observacao { get; set; }

        public IEnumerable<DataAulaTurmaRelatorioCodafDto> DataAulas { get; set; }
        public IEnumerable<DadosRegenteTurmaRelatorioCodafDto> RegentesTurma { get; set; }
        public IEnumerable<DadosParticipanteRelatorioCodafDto> Participantes { get; set; }
        public IEnumerable<DadosRetificacaoRelatorioCodafDto> Retificacoes { get; set; }
    }
}
