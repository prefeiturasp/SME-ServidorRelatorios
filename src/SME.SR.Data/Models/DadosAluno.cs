using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Data
{
    public class DadosAluno
    {
		public int CodigoAluno { get; set; }
		public string NomeAluno { get; set; }
		public string NomeSocialAluno { get; set; }
		public DateTime DataNascimento { get; set; }
		public int CodigoSituacaoMatricula { get; set; }
		public string SituacaoMatricula { get; set; }
		public DateTime DataSituacao { get; set; }
		public string NumeroAlunoChamada { get; set; }
		public bool PossuiDeficiencia { get; set; }
	}
}
