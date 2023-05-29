using System;

namespace SME.SR.Data
{
    public class ComponenteCurricularTerritorioSaber
    {
        public string CodigoTurma { get; set; }
        public long CodigoExperienciaPedagogica { get; set; }
        public long CodigoTerritorioSaber { get; set; }
        public string DescricaoTerritorioSaber { get; set; }
        public string DescricaoExperienciaPedagogica { get; set; }
        public DateTime DataInicio { get; set; }
        public long CodigoComponenteCurricular { get; set; }
        public long GrupoMatrizId { get; set; }
        public string Professor { get; set; }

        public long ObterCodigoComponenteCurricular(string codigoTurma)
        {
            return Convert.ToInt64($"{codigoTurma}{CodigoTerritorioSaber}{CodigoExperienciaPedagogica}{DataInicio:MMdd}");
        }

        public string ObterDescricaoComponenteCurricular()
        {
            return $"{DescricaoTerritorioSaber} - {DescricaoExperienciaPedagogica}";
        }
    }
}
