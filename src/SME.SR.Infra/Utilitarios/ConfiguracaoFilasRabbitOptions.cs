using System.Linq;

namespace SME.SR.Infra
{
    public class ConfiguracaoFilasRabbitOptions
    {
        public const string Secao = "FilasRabbit";
        public string Filas { get; set; }
        public string FilasIgnoradas { get; set; }
        public string[] GetFilas => ToStringArray(Filas);
        public string[] GetFilasIgnoradas => ToStringArray(FilasIgnoradas);

        private static string[] ToStringArray(string valor)
           => !string.IsNullOrEmpty(valor) 
              ? valor.Split(",").Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray()
              : Enumerable.Empty<string>().ToArray();
    }
}
