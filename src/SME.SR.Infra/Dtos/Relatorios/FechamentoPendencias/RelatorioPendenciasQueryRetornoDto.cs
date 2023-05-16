using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasQueryRetornoDto
    {
        private List<int> PendenciaSemDetalhe;
        public RelatorioPendenciasQueryRetornoDto()
        {
            Detalhes = new List<string>();
            PendenciaSemDetalhe = new List<int>() { (int)Infra.TipoPendencia.Frequencia, (int)Infra.TipoPendencia.PlanoAula, (int)Infra.TipoPendencia.DiarioBordo };
        }

        public string PendenciaId { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public IList<string> Detalhes { get; set; }
        public string Instrucao { get; set; }
        public int Situacao { get; set; }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public int AnoLetivo { get; set; }
        public int ModalidadeCodigo { get; set; }
        public string Semestre { get; set; }
        public string TurmaNome { get; set; }
        public string TurmaCodigo { get; set; }
        public long DisciplinaId { get; set; }
        public int Bimestre { get; set; }
        public string Criador { get; set; }
        public string CriadorRf { get; set; }
        public string Aprovador { get; set; }
        public string AprovadorRf { get; set; }
        public string TipoPendencia { get; set; }
        public bool OutrasPendencias { get; set; }
        public bool ExibirDetalhes { get { return !PendenciaSemDetalhe.Contains(Tipo); }}
        public int Tipo { get; set; }
        public void AdicionaDetalhe(string detalhe)
        {
            if (!string.IsNullOrEmpty(detalhe) && !Detalhes.Contains(detalhe))
                Detalhes.Add(detalhe);
        }
    }
}
