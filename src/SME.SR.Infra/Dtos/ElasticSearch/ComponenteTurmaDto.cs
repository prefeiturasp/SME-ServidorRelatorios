using Nest;
using System;

namespace SME.SR.Infra.Dtos.ElasticSearch
{
    [ElasticsearchType(RelationName = "ComponenteTurma")]
    public class ComponenteTurmaDto
    {
        [Text(Name = "NomeComponenteCurricular")]
        public string NomeComponenteCurricular { get; set; }
        
        [Number(Name = "ComponenteCurricularCodigo")]
        public long ComponenteCurricularCodigo { get; set; }
        
        [Text(Name = "RegistroFuncional")]
        public string RegistroFuncional { get; set; }
        
        [Date(Name = "DataDisponibizacao", Format = "MMddyyyy")]
        public DateTime? DataDisponibizacao { get; set; }
    }
}
