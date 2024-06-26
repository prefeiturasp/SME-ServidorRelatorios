﻿using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.RelatorioPaginado
{
    public abstract class RelatorioPaginadoHistoricoEscolarFundamentalMedio : RelatorioPaginadoHistoricoEscolar<HistoricoEscolarFundamentalDto>
    {
        protected RelatorioPaginadoHistoricoEscolarFundamentalMedio(IEnumerable<HistoricoEscolarFundamentalDto> historicoEscolarDTOs) : base(historicoEscolarDTOs)
        {
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
