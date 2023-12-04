using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Data
{
    public class InformacaoPedagogicaComponenteCurricularSGPDTO : ICloneable
    {
        public InformacaoPedagogicaComponenteCurricularSGPDTO()
        {}
        public long Codigo { get; set; }
        public long? CodComponenteCurricularPai { get; set; }
        public string Descricao { get; set; }
        public string DescricaoInfantil { get; set; }
        public bool EhTerritorioSaber { get; set; }
        public bool EhRegencia { get; set; }
        public long GrupoMatrizId { get; set; }
        public string GrupoMatrizNome { get; set; }
        public bool Compartilhada { get; set; }
        public bool LancaNota { get; set; }
        public bool RegistraFrequencia { get; set; }
        public bool BaseNacional { get; set; }
        public long? AreaConhecimentoId { get; set; }
        public string AreaConhecimentoNome { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public static class InformacaoPedagogicaComponenteCurricularSGPDTOExtension
    {
        public static ComponenteCurricular ToComponenteCurricular(this InformacaoPedagogicaComponenteCurricularSGPDTO value)
        {
            return new ComponenteCurricular()
            {
                Codigo = value.Codigo,
                Descricao = value.Descricao,
                BaseNacional = value.BaseNacional,
                Compartilhada = value.Compartilhada,
                DescricaoInfantil = value.DescricaoInfantil,
                Frequencia = value.RegistraFrequencia,
                GrupoMatrizId = value.GrupoMatrizId,
                TerritorioSaber = value.EhTerritorioSaber,
                ComponentePlanejamentoRegencia = value.EhRegencia,
                CodComponentePai = value.CodComponenteCurricularPai,
                LancaNota = value.LancaNota
            };
        }

        public static IEnumerable<ComponenteCurricular> ToComponentesCurriculares(this IEnumerable<InformacaoPedagogicaComponenteCurricularSGPDTO> values)
        {
            return values.Select(inf => inf.ToComponenteCurricular());
        }
    }
 }
