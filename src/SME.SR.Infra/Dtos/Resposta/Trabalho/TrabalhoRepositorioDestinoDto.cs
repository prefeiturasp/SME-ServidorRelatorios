using Newtonsoft.Json;
using Refit;

namespace SME.SR.Infra.Dtos
{
    public class TrabalhoRepositorioDestinoDto
    {
        [JsonProperty("id"), AliasAs("id")]
        public int Id { get; set; }

        [JsonProperty("version"), AliasAs("version")]
        public int Versao { get; set; }

        [JsonProperty("folderURI"), AliasAs("folderURI")]
        public string PastaDestinoURI { get; set; }

        [JsonProperty("sequentialFilenames"), AliasAs("sequentialFilenames")]
        public bool NomeArquivosSequenciais { get; set; }

        [JsonProperty("overwriteFiles"), AliasAs("overwriteFiles")]
        public bool SobrescreverArquivos { get; set; }

        [JsonProperty("outputDescription"), AliasAs("outputDescription")]
        public object DescricaoSaida { get; set; }

        [JsonProperty("timestampPattern"), AliasAs("timestampPattern")]
        public string PadraoTimeZone { get; set; }

        [JsonProperty("saveToRepository"), AliasAs("saveToRepository")]
        public bool SalvarParaRepositorio { get; set; }

        [JsonProperty("defaultReportOutputFolderURI"), AliasAs("defaultReportOutputFolderURI")]
        public string URIPastaSaidaRelatorioPadrao { get; set; }

        [JsonProperty("usingDefaultReportOutputFolderURI"), AliasAs("usingDefaultReportOutputFolderURI")]
        public bool UtilizandoURISaidaPadraoRelatorio { get; set; }

        [JsonProperty("outputLocalFolder"), AliasAs("outputLocalFolder")]
        public string PastaSaidaLocal { get; set; }

        [JsonProperty("outputFTPInfo"), AliasAs("outputFTPInfo")]
        public TrabalhoInformacaoFTPSaidaDto InformacaoFTPSaida { get; set; }
    }
}
