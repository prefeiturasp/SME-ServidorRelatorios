using Nest;

namespace SME.SR.Infra.Dtos.ElasticSearch
{
    [ElasticsearchType(RelationName = "ComponenteCurricular")]
    public class ComponenteCurricularDto
    {
        [Number(Name = "ComponenteCurricularCodigo")]
        public long Codigo { get; set; }

        [Text(Name = "NomeComponenteCurricular")]
        public string Descricao { get; set; }

        [Number(Name = "TipoEscola")]
        public long TipoEscola { get; set; }

        [Text(Name = "AnoTurma")]
        public string AnoTurma { get; set; }

        [Text(Name = "Turno")]
        public string TurnoTurma { get; set; }

        [Number(Name = "codigoturma")]
        public long CodigoTurma { get; set; }
    }
}