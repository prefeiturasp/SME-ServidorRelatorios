using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using SME.SR.Infra.Dtos.Relatorios.MapeamentoEstudante;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class ConsolidacaoProdutividadeFrequenciaDto
    {
        public ConsolidacaoProdutividadeFrequenciaDto()
        {}
        public long Id { get; set; }
        public string CodigoTurma { get; set; }
        public string DescricaoTurma { get; set; }
        public string CodigoUe { get; set; }
        public string DescricaoUe { get; set; }
        public string CodigoDre { get; set; }
        public string DescricaoDre { get; set; }
        public string NomeProfessor { get; set; }
        public string RfProfessor { get; set; }
        public int Bimestre { get; set; }
        public Modalidade Modalidade { get; set; }
        public DateTime DataAula { get; set; }
        public DateTime DataRegistroFrequencia { get; set; }
        public int DiferenciaDiasDataAulaRegistroFrequencia { get; set; }
        public int AnoLetivo { get; set; }
        public string CodigoComponenteCurricular { get; set; }
        public string NomeComponenteCurricular { get; set; }
    }

}
