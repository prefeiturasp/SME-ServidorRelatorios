using Nest;

namespace SME.SR.Infra.Dtos.ElasticSearch
{
    public class DocumentoElastic 
    {
        [Text(Name = "Id")]
        public string Id { get; set; }

    }
}