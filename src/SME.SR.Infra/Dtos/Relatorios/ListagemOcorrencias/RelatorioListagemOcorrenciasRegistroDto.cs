using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioListagemOcorrenciasRegistroDto
    {
        public int OcorrenciaId { get; set; }
        public string DreAbreviacao { get; set; }

        public string UeCodigo { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }

        public string UeDescricao
        {
            get
            {
                return $"{UeCodigo} - {TipoEscola.ShortName()} {UeNome}";
            }
        }

        public Modalidade Modalidade { get; set; }
        public string TurmaNome { get; set; }
        public TipoTurnoEOL TipoTurno { get; set; }
        public string TurmaDescricao {
            get
            {
                return $"{Modalidade.ShortName()}-{TurmaNome}-{TipoTurno.Name()}";
            }
        }

        public DateTime DataOcorrencia { get; set; }
        public string OcorrenciaTipo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }

        public IEnumerable<RelatorioListagemOcorrenciasRegistroAlunoDto> Alunos { get; set; }
        public IEnumerable<RelatorioListagemOcorrenciasRegistroServidorDto> Servidores { get; set; }
    }
}
