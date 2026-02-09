using System;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.Codaf
{
    public class CabecalhoRelatorioCodafDto
    {
        public string AreaPromotora { get; set; }
        public string NomeFormacao { get; set; }
        public int NumeroHomologacao { get; set; }
        public int CodigoEventoSigpec { get; set; }
        public int NumeroComunicado { get; set; }
        public DateTime DataComunicado { get; set; }
        public DateTime DataPublicacaoDom { get; set; }
        public int PaginaDom { get; set; }
        public DateTime DataPeriodoRealizacaoInicio { get; set; }
        public DateTime DataPeriodoRealizacaoFim { get; set; }
        public List<DateTime> DataDasAulasSincronas { get; set; }
        public int CargaHorariaTotal { get; set; }
        public int CargaHorariaDistancia { get; set; }
        public int CargaHorariaPresencial { get; set; }
        public string NomeDre { get; set; }
        public string NomeTurma { get; set; }
        public int QuantidadeTurmas { get; set; }
        public int NumeroVagas { get; set; }
        public List<RetificacaoRelatorioCodafDto> Retificacoes { get; set; }
        public PreviaInscritosRelatorioCodafDto PreviaInscritosSme { get; set; }
        public PreviaInscritosRelatorioCodafDto PreviaInscritosSemRf { get; set; }
        public string Observacao { get; set; }
        public TipoFormacaoRelatorioCodaf TipoFormacao { get; set; }
        public ModalidadeRelatorioCodaf Modalidade { get; set; }
        public TipoCertificacaoRelatorioCodaf TipoCertificacao { get; set; }
    }
}