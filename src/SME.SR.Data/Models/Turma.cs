using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SME.SR.Data
{
    public class Turma
    {
        public static readonly TipoTurma[] TiposRegulares = { TipoTurma.Regular, TipoTurma.EdFisica, TipoTurma.Itinerarios2AAno };
        public string Codigo { get; set; }
        public string RegularCodigo { get; set; }        
        public int AnoLetivo { get; set; }
        public string Ano { get; set; }
        public string Nome { get; set; }
        public string Ciclo { get; set; }
        public int Semestre { get; set; }
        public int EtapaEJA { get; set; }
        public int EtapaEnsino { get; set; }
        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public Modalidade ModalidadeCodigo { get; set; }
        public TipoTurma TipoTurma { get; set; }

        public ModalidadeTipoCalendario ModalidadeTipoCalendario
        {
            get => ModalidadeCodigo == Modalidade.EJA ?
                ModalidadeTipoCalendario.EJA :
                ModalidadeCodigo == Modalidade.Infantil ? 
                ModalidadeTipoCalendario.Infantil :
                ModalidadeTipoCalendario.FundamentalMedio;
        }

        public bool EhTurmaMagisterio
        {
            get => EtapaEnsino == 9;
        }

        public bool EhEja
        {
            get => ModalidadeCodigo == Modalidade.EJA;
        }

        public string NomeRelatorio => $"{ModalidadeCodigo.GetAttribute<DisplayAttribute>().ShortName} - {Nome}";

        public string DescricaoRelatorioTransferencia
        {
            get
            {
                switch (ModalidadeCodigo)
                {
                    case Modalidade.EJA:
                        return $"NOTAS E Nº DE FALTAS DA ETAPA {Ciclo.ToUpper()} {EtapaEJA}";
                    case Modalidade.Fundamental:
                        return $"NOTAS E Nº DE FALTAS DO {Ano}º ANO DO CICLO {Ciclo.ToUpper()} DO ENSINO FUNDAMENTAL";
                    default:
                        return $"NOTAS E Nº DE FALTAS DO {Ano}º ANO DO ENSINO MÉDIO";
                }
            }

        }

        public string RodapeRelatorioTransferencia
        {
            get
            {
                switch (ModalidadeCodigo)
                {
                    case Modalidade.EJA:
                        return $"O (a) aluno (a) tem direito à matrícula na etapa {Ciclo.ToUpper()} {EtapaEJA}";
                    case Modalidade.Fundamental:
                        return $"O (a) aluno (a) tem direito à matrícula no {Ano}º ano do ciclo {Ciclo.ToUpper()} do Ensino Fundamental";
                    default:
                        return $"O (a) aluno (a) tem direito à matrícula no {Ano}º ano do Ensino Médio";
                }
            }
        }

        public string NomePorFiltroModalidade(Modalidade? filtroModalidade)
        {
            if (filtroModalidade.HasValue)
                return Nome;
            else
                return $"{ModalidadeCodigo.ShortName()} - {Nome}";

        }

        public IEnumerable<int> ObterTiposRegularesDiferentes()
        {
            return TiposRegulares
                .Where(a => a != TipoTurma).Select(s => (int)s)
                .ToList();
        }
    }
}
