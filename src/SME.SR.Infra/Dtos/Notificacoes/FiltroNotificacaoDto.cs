using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class FiltroNotificacaoDto
    {
        public DateTime Data => DateTime.Now;
        public string UsuarioNome { get; set; }
        public string UsuarioRf { get; set; }
        public long AnoLetivo { get; set; }
        public long DRE { get; set; }
        public long UE { get; set; }
        public Modalidade ModalidadeTurma { get; set; }
        public int Semestre { get; set; }
        public long Turma { get; set; }
        public string UsuarioBuscaNome { get; set; }
        public string UsuarioBuscaRf { get; set; }
        public IEnumerable<long> Categoria { get; set; }
        public IEnumerable<long> Tipo { get; set; }
        public IEnumerable<long> Situacao { get; set; }
        public bool ExibirDescricao { get; set; }
        public bool ExibirNotificacoesExcluidas { get; set; }
    }
}
