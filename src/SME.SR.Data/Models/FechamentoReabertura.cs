using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SR.Data
{
    public class FechamentoReabertura
    { 
        public long Id { get; set; }
        public string Descricao { get; set; }
        public Dre Dre { get; set; }
        public long? DreId { get; set; }
        public bool Excluido { get; set; }
        public DateTime Fim { get; set; }
        public DateTime Inicio { get; set; }
        public bool Migrado { get; set; }
        public TipoCalendario TipoCalendario { get; set; }
        public long TipoCalendarioId { get; set; }
        public Ue Ue { get; set; }
        public long? UeId { get; set; }
        public Usuario Aprovador { get; set; }
        public long? AprovadorId { get; set; }
        public DateTime? AprovadoEm { get; set; }

        public void AtualizarDre(Dre dre)
        {
            if (dre == null)
                return;

            Dre = dre;
            DreId = dre.Id;
        }

        public void AtualizarTipoCalendario(TipoCalendario tipoCalendario)
        {
            if (tipoCalendario == null)
                return;

            TipoCalendario = tipoCalendario;
            TipoCalendarioId = tipoCalendario.Id;
        }

        public void AtualizarUe(Ue ue)
        {
            if (ue == null)
                return;

            Ue = ue;
            UeId = ue.Id;
        }

        private bool EhParaDre()
        {
            return UeId is null && DreId > 0;
        }

        private bool EhParaSme()
        {
            return DreId is null && UeId is null;
        }

        public bool EhParaUe()
        {
            return DreId > 0 && UeId > 0;
        }

        private bool EstaNoRangeDeDatas(DateTime dataInicio, DateTime datafim)
        {
            return (Inicio.Date <= dataInicio.Date && Fim >= datafim.Date)
                || (Inicio.Date <= datafim.Date && Fim >= datafim.Date)
                || (Inicio.Date >= dataInicio.Date && Fim <= datafim.Date);
        }

        public void Excluir()
        {
            if (Excluido)
                throw new NegocioException($"Não é possível excluir o fechamento {Id} pois o mesmo já se encontra excluído.");

            Excluido = true;
        }


        public void PodeSalvar(IEnumerable<FechamentoReabertura> fechamentosCadastrados, Usuario usuario)
        {
            if (Inicio > Fim)
                throw new NegocioException("A data início não pode ser maior que a data fim.");

            if (usuario.EhPerfilDRE() && VerificaAnoAtual())
                throw new NegocioException("Somente SME pode cadastrar reabertura no ano atual.");

            VerificaFechamentosNoMesmoPeriodo(fechamentosCadastrados);
        }


        private bool VerificaAnoAtual()
        {
            return TipoCalendario.AnoLetivo == DateTime.Today.Year;
        }

        private void VerificaFechamentosNoMesmoPeriodo(IEnumerable<FechamentoReabertura> fechamentosCadastrados)
        {
            if (EhParaSme())
            {
                var fechamentosSME = fechamentosCadastrados.Where(a => a.EhParaSme());

                if (fechamentosSME is null || !fechamentosSME.Any())
                    return;

                foreach (var fechamento in fechamentosSME)
                {
                    if (EstaNoRangeDeDatas(fechamento.Inicio, fechamento.Fim))
                        throw new NegocioException($"Não é possível persistir pois já existe uma reabertura cadastrada que começa em {fechamento.Inicio.ToString("dd/MM/yyyy")} e termina em {fechamento.Fim.ToString("dd/MM/yyyy")}");
                }
            }
            else if (EhParaDre())
            {
                var fechamentosDre = (fechamentosCadastrados.Where(a => a.EhParaDre() && a.DreId == DreId)).ToList();

                if (!fechamentosDre.Any())
                    return;

                foreach (var fechamento in fechamentosDre.Where(fechamento => EstaNoRangeDeDatas(fechamento.Inicio, fechamento.Fim)))
                    throw new NegocioException($"Não é possível persistir pois já existe uma reabertura cadastrada que começa em {fechamento.Inicio:dd/MM/yyyy} e termina em {fechamento.Fim:dd/MM/yyyy}");
            }
            else if (EhParaUe())
            {
                var fechamentosUe = fechamentosCadastrados.Where(a => a.EhParaUe() && a.UeId == UeId).ToList();

                if (!fechamentosUe.Any())
                    return;

                foreach (var fechamento in fechamentosUe.Where(fechamento => EstaNoRangeDeDatas(fechamento.Inicio, fechamento.Fim)))
                    throw new NegocioException($"Não é possível persistir pois já existe uma reabertura cadastrada que começa em {fechamento.Inicio:dd/MM/yyyy} e termina em {fechamento.Fim:dd/MM/yyyy}");
            }
        }
    }
}
