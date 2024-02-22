namespace SME.SR.Infra
{
    public class ConfiguracaoFilasRabbitOptions
    {
        public const string Secao = "FilasRabbit";
        public string[] Filas { get; set; } = new string[] { };
        public string[] FilasIgnoradas { get; set; } = new string[] { };
    }
}
