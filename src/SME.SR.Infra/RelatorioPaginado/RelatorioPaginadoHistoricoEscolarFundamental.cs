using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class RelatorioPaginadoHistoricoEscolarFundamental : RelatorioPaginadoHistoricoEscolar<HistoricoEscolarFundamentalDto>
    {
        public RelatorioPaginadoHistoricoEscolarFundamental(IEnumerable<HistoricoEscolarFundamentalDto> historicoEscolarDTOs) : base(historicoEscolarDTOs)
        {
            TabelaHistoricoTodosAnos = SecaoViewHistoricoEscolar.TabelaHistoricoTodosAnosFundamental;
            TabelaAnoAtual = SecaoViewHistoricoEscolar.TabelaAnoAtualFundamental;
        }

        protected override int ObterQuantidadeLinhaDadosHistorico(HistoricoEscolarFundamentalDto historicoEscolar)
        {
            var linhas = 0;
            var dadosHistoricoDto = historicoEscolar.DadosHistorico;

            if (dadosHistoricoDto.BaseNacionalComum != null)
                foreach (var area in dadosHistoricoDto.BaseNacionalComum.AreasDeConhecimento)
                    linhas += area.ComponentesCurriculares.Count();

            linhas += dadosHistoricoDto.EnriquecimentoCurricular != null ? dadosHistoricoDto.EnriquecimentoCurricular.Count() : 0;

            linhas += dadosHistoricoDto.ProjetosAtividadesComplementares != null ? dadosHistoricoDto.ProjetosAtividadesComplementares.Count() : 0;

            linhas += dadosHistoricoDto.GruposComponentesCurriculares != null ? dadosHistoricoDto.GruposComponentesCurriculares.Count() : 0;

            linhas += dadosHistoricoDto.FrequenciaGlobal != null ? 1 : 0;
            
            linhas += dadosHistoricoDto.ParecerConclusivo != null ? 1 : 0;

            return linhas;
        }

    }
}
