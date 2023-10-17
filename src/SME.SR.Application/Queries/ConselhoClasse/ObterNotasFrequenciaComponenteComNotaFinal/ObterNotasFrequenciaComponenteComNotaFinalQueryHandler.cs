using MediatR;
using SME.SR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterNotasFrequenciaComponenteComNotaFinalQueryHandler : IRequestHandler<ObterNotasFrequenciaComponenteComNotaFinalQuery, ComponenteComNotaFinal>
    {
        public async Task<ComponenteComNotaFinal> Handle(ObterNotasFrequenciaComponenteComNotaFinalQuery request, CancellationToken cancellationToken)
        {
            var notasComponente = ObterNotasComponente(request.ComponenteCurricular, request.PeriodoEscolar, request.NotasFechamentoAluno);

            var conselhoClasseComponente = new ComponenteComNotaFinal()
            {
                Componente = request.ComponenteCurricular.Disciplina,
                Faltas = request.FrequenciaAluno?.TotalAusencias ?? 0,
                AusenciasCompensadas = request.FrequenciaAluno?.TotalCompensacoes ?? 0,
                Frequencia = request.FrequenciaAluno?.PercentualFrequenciaFormatado,
                NotaConceitoBimestre1 = notasComponente.FirstOrDefault(n => n.Bimestre == 1)?.NotaConceito,
                NotaConceitoBimestre2 = notasComponente.FirstOrDefault(n => n.Bimestre == 2)?.NotaConceito,
                NotaConceitoBimestre3 = notasComponente.FirstOrDefault(n => n.Bimestre == 3)?.NotaConceito,
                NotaConceitoBimestre4 = notasComponente.FirstOrDefault(n => n.Bimestre == 4)?.NotaConceito,
                NotaFinal = ObterNotaPosConselho(request.ComponenteCurricular, request.PeriodoEscolar?.Bimestre, request.NotasConselhoClasseAluno, request.NotasFechamentoAluno)
            };

            return conselhoClasseComponente;
        }

        private List<NotaBimestre> ObterNotasComponente(ComponenteCurricularPorTurma componenteCurricular, PeriodoEscolar periodoEscolar, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var notasFinais = new List<NotaBimestre>();

            if (periodoEscolar != null)
                notasFinais.Add(ObterNotaFinalComponentePeriodo(componenteCurricular.CodDisciplina, periodoEscolar.Bimestre, notasFechamentoAluno));
            else
                notasFinais.AddRange(ObterNotasFinaisComponentePeriodos(componenteCurricular.CodDisciplina, notasFechamentoAluno));

            return notasFinais;
        }

        private NotaBimestre ObterNotaFinalComponentePeriodo(long codigoComponenteCurricular, int bimestre, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            string notaConceito = null;
            // Busca nota do FechamentoNota
            var notaFechamento = notasFechamentoAluno.FirstOrDefault(c => c.ComponenteCurricularCodigo == codigoComponenteCurricular);
            if (notaFechamento != null)
                notaConceito = notaFechamento.NotaConceito;

            return new NotaBimestre()
            {
                Bimestre = bimestre,
                NotaConceito = notaConceito
            };
        }

        private IEnumerable<NotaBimestre> ObterNotasFinaisComponentePeriodos(long codigoComponenteCurricular, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var notasPeriodos = new List<NotaBimestre>();

            var notasFechamentoBimestres = notasFechamentoAluno.Where(c => c.ComponenteCurricularCodigo == codigoComponenteCurricular && c.Bimestre.HasValue);
            foreach (var notaFechamento in notasFechamentoBimestres)
            {
                notasPeriodos.Add(new NotaBimestre()
                {
                    Bimestre = notaFechamento.Bimestre.Value,
                    NotaConceito = notaFechamento.NotaConceito
                });
            }

            return notasPeriodos;
        }

        private string ObterNotaPosConselho(ComponenteCurricularPorTurma componenteCurricular, int? bimestre, IEnumerable<NotaConceitoBimestreComponente> notasConselhoClasseAluno, IEnumerable<NotaConceitoBimestreComponente> notasFechamentoAluno)
        {
            var componenteCurricularCodigo = componenteCurricular.CodDisciplina;
            // Busca nota do conselho de classe consultado
            var notaComponente = notasConselhoClasseAluno.FirstOrDefault(c => c.ComponenteCurricularCodigo == componenteCurricularCodigo);
            if (notaComponente == null)
                // Sugere nota final do fechamento
                notaComponente = notasFechamentoAluno.FirstOrDefault(c => c.ComponenteCurricularCodigo == componenteCurricularCodigo && c.Bimestre == bimestre);

            return notaComponente?.NotaConceito.ToString();
        }
    }
}
