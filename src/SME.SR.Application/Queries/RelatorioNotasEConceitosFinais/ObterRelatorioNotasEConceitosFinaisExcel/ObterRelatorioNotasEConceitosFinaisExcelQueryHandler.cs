using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioNotasEConceitosFinaisExcelQueryHandler : IRequestHandler<ObterRelatorioNotasEConceitosFinaisExcelQuery, IEnumerable<RelatorioNotasEConceitosFinaisExcelDto>>
    {
        public async Task<IEnumerable<RelatorioNotasEConceitosFinaisExcelDto>> Handle(ObterRelatorioNotasEConceitosFinaisExcelQuery request, CancellationToken cancellationToken)
        {
            var listaNotasEConceitosFinais = new List<RelatorioNotasEConceitosFinaisExcelDto>();

            foreach (var dre in request.RelatorioNotasEConceitosFinais.Dres)
            {
                foreach (var ue in dre.Ues)
                {
                    foreach (var ano in ue.Anos)
                    {
                        foreach (var bimestre in ano.Bimestres)
                        {
                            foreach (var componente in bimestre.ComponentesCurriculares)
                            {
                                foreach (var aluno in componente.NotaConceitoAlunos)
                                {
                                    listaNotasEConceitosFinais.Add(ObterRelatorioNotasEConceito(dre.Nome, ue.Nome, bimestre.Nome,
                                                                                            ano.Nome, aluno.TurmaNome, componente.Nome,
                                                                                            aluno.AlunoCodigo, aluno.AlunoNomeCompleto, aluno.NotaConceito));
                                }
                            }
                        }

                    }
                }
            }

            if(listaNotasEConceitosFinais.Any())
            {
                listaNotasEConceitosFinais = listaNotasEConceitosFinais.OrderBy(n => n.DreNome)
                                                                       .ThenBy(n => n.UnidadeEscolarNome)
                                                                       .ThenBy(n => n.Bimestre)
                                                                       .ThenBy(n => n.Ano)
                                                                       .ThenBy(n => n.Turma)
                                                                       .ThenBy(n => n.ComponenteCurricular)
                                                                       .ThenBy(n => n.EstudanteNome)
                                                                       .ToList();
            }

            return await Task.FromResult(listaNotasEConceitosFinais);
        }

      
        private RelatorioNotasEConceitosFinaisExcelDto ObterRelatorioNotasEConceito(string dreNome, string ueNome, string bimestre,
                                                                                      string ano, string turma, string componenteCurricular,
                                                                                      int alunoCodigo, string alunoNome, string notaConceito)
        {
            RelatorioNotasEConceitosFinaisExcelDto relatorioDto = new RelatorioNotasEConceitosFinaisExcelDto();

            relatorioDto.DreNome = dreNome;
            relatorioDto.UnidadeEscolarNome = ueNome;
            relatorioDto.Bimestre = bimestre;
            relatorioDto.Ano = ano;
            relatorioDto.Turma = turma;
            relatorioDto.ComponenteCurricular = componenteCurricular;
            relatorioDto.EstudanteCodigo = alunoCodigo.ToString();
            relatorioDto.EstudanteNome = alunoNome;
            relatorioDto.NotaConceito = notaConceito.Replace("*","");

            return relatorioDto;
        }
    }
}
