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
        public double MediaDiasDataAulaRegistroFrequencia { get; set; }
    }

    public static class ConsolidacaoProdutividadeFrequenciaExtension
    {
        public static IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> AgruparPorUe(this IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> source)
        {
            var retorno = source.GroupBy(consol => new { consol.CodigoUe, consol.DescricaoUe, consol.CodigoDre, consol.DescricaoDre, consol.Bimestre, consol.AnoLetivo });
            return retorno.Select(consolAgrupado => 
                new ConsolidacaoProdutividadeFrequenciaDto 
                {
                    CodigoUe = consolAgrupado.Key.CodigoUe,
                    DescricaoUe = consolAgrupado.Key.DescricaoUe,
                    CodigoDre = consolAgrupado.Key.CodigoDre,
                    DescricaoDre = consolAgrupado.Key.DescricaoDre, 
                    Bimestre = consolAgrupado.Key.Bimestre,
                    AnoLetivo = consolAgrupado.Key.AnoLetivo,
                    MediaDiasDataAulaRegistroFrequencia = Math.Round((double) consolAgrupado.Sum(c => c.DiferenciaDiasDataAulaRegistroFrequencia)/consolAgrupado.Count(), 2)
                })
                .OrderBy(consol => consol.CodigoDre)
                .ThenBy(consol => consol.DescricaoUe)
                .ThenBy(consol => consol.Bimestre);
        }

        public static IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> AgruparPorProfessor(this IEnumerable<ConsolidacaoProdutividadeFrequenciaDto> source)
        {
            var retorno = source.GroupBy(consol => new { consol.CodigoUe, consol.DescricaoUe, consol.CodigoDre, consol.DescricaoDre, consol.Bimestre, consol.AnoLetivo, consol.RfProfessor, consol.NomeProfessor });
            return retorno.Select(consolAgrupado =>
                new ConsolidacaoProdutividadeFrequenciaDto
                {
                    CodigoUe = consolAgrupado.Key.CodigoUe,
                    DescricaoUe = consolAgrupado.Key.DescricaoUe,
                    CodigoDre = consolAgrupado.Key.CodigoDre,
                    DescricaoDre = consolAgrupado.Key.DescricaoDre,
                    Bimestre = consolAgrupado.Key.Bimestre,
                    RfProfessor = consolAgrupado.Key.RfProfessor,
                    NomeProfessor = consolAgrupado.Key.NomeProfessor,
                    AnoLetivo = consolAgrupado.Key.AnoLetivo,
                    MediaDiasDataAulaRegistroFrequencia = Math.Round((double)consolAgrupado.Sum(c => c.DiferenciaDiasDataAulaRegistroFrequencia) / consolAgrupado.Count(), 2)
                })
                .OrderBy(consol => consol.CodigoDre)
                .ThenBy(consol => consol.DescricaoUe)
                .ThenBy(consol => consol.NomeProfessor)
                .ThenBy(consol => consol.Bimestre);
        }
    }

}
