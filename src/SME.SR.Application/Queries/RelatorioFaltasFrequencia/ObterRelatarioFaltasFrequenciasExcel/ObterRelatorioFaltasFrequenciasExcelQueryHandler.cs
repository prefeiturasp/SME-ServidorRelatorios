using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioFaltasFrequenciasExcelQueryHandler : IRequestHandler<ObterRelatorioFaltasFrequenciasExcelQuery, IEnumerable<RelatorioFaltasFrequenciasBaseExcelDto>>
    {
        public async Task<IEnumerable<RelatorioFaltasFrequenciasBaseExcelDto>> Handle(ObterRelatorioFaltasFrequenciasExcelQuery request, CancellationToken cancellationToken)
        {
            var listaFaltaFrequencia = new List<RelatorioFaltasFrequenciasBaseExcelDto>();

            foreach (var dre in request.RelatorioFaltasFrequencias.Dres)
            {
                foreach (var ue in dre.Ues)
                {
                    foreach (var ano in ue.Anos)
                    {
                        foreach (var bimestre in ano.Bimestres)
                        {
                            foreach (var componente in bimestre.Componentes)
                            {
                                foreach (var aluno in componente.Alunos)
                                {
                                    listaFaltaFrequencia.Add(ObterRelatorioFaltasFrequencia(request.TipoRelatorio, dre.NomeDre, ue.NomeUe, bimestre.NomeBimestre,
                                                                                            ano.NomeAno, aluno.NomeTurma, componente.NomeComponente,
                                                                                            aluno.CodigoAluno, aluno.NomeAluno, aluno.TotalAulas, aluno.TotalAusencias,
                                                                                            aluno.Frequencia));
                                }
                            }
                        }

                    }
                }
            }
            return await Task.FromResult(listaFaltaFrequencia);
        }

        private void ObterRelatorioFaltasFrequenciaBase(ref RelatorioFaltasFrequenciasExcelDto relatorioDto,
                                                        string dreNome, string ueNome, string bimestre, string ano,
                                                        string turma, string componenteCurricular,
                                                        string alunoCodigo, string alunoNome)
        {
            relatorioDto.DreNome = dreNome;
            relatorioDto.UnidadeEscolarNome = ueNome;
            relatorioDto.Bimestre = bimestre;
            relatorioDto.Ano = ano;
            relatorioDto.Turma = turma;
            relatorioDto.ComponenteCurricular = componenteCurricular;
            relatorioDto.EstudanteCodigo = alunoCodigo;
            relatorioDto.EstudanteNome = alunoNome;
        }

        private void ObterRelatorioFaltasFrequenciaBase(ref RelatorioFaltasExcelDto relatorioDto,
                                                       string dreNome, string ueNome, string bimestre, string ano,
                                                       string turma, string componenteCurricular,
                                                       string alunoCodigo, string alunoNome)
        {
            relatorioDto.DreNome = dreNome;
            relatorioDto.UnidadeEscolarNome = ueNome;
            relatorioDto.Bimestre = bimestre;
            relatorioDto.Ano = ano;
            relatorioDto.Turma = turma;
            relatorioDto.ComponenteCurricular = componenteCurricular;
            relatorioDto.EstudanteCodigo = alunoCodigo;
            relatorioDto.EstudanteNome = alunoNome;
        }

        private void ObterRelatorioFaltasFrequenciaBase(ref RelatorioFrequenciasExcelDto relatorioDto,
                                                       string dreNome, string ueNome, string bimestre, string ano,
                                                       string turma, string componenteCurricular,
                                                       string alunoCodigo, string alunoNome)
        {
            relatorioDto.DreNome = dreNome;
            relatorioDto.UnidadeEscolarNome = ueNome;
            relatorioDto.Bimestre = bimestre;
            relatorioDto.Ano = ano;
            relatorioDto.Turma = turma;
            relatorioDto.ComponenteCurricular = componenteCurricular;
            relatorioDto.EstudanteCodigo = alunoCodigo;
            relatorioDto.EstudanteNome = alunoNome;
        }

        private RelatorioFaltasFrequenciasBaseExcelDto ObterRelatorioFaltasFrequencia(TipoRelatorioFaltasFrequencia tipoRelatorio,
                                                                                      string dreNome, string ueNome, string bimestre,
                                                                                      string ano, string turma, string componenteCurricular,
                                                                                      int alunoCodigo, string alunoNome, int totalAulas,
                                                                                      int totalAusencias, double frequencia)
        {
            RelatorioFaltasFrequenciasBaseExcelDto relatorioBase;

            if (tipoRelatorio == TipoRelatorioFaltasFrequencia.Ano)
            {
                var relatorioAmbos = new RelatorioFaltasFrequenciasExcelDto();
                ObterRelatorioFaltasFrequenciaBase(ref relatorioAmbos, dreNome, ueNome, bimestre, ano, turma, componenteCurricular, alunoCodigo.ToString(), alunoNome);
                relatorioBase = relatorioAmbos;
            }
            else if (tipoRelatorio == TipoRelatorioFaltasFrequencia.Turma)
            {
                var relatorioFaltas = new RelatorioFaltasExcelDto();
                ObterRelatorioFaltasFrequenciaBase(ref relatorioFaltas, dreNome, ueNome, bimestre, ano, turma, componenteCurricular, alunoCodigo.ToString(), alunoNome);
                relatorioBase = relatorioFaltas;
            }
            else
            {
                var relatorioFrequencia = new RelatorioFrequenciasExcelDto();
                ObterRelatorioFaltasFrequenciaBase(ref relatorioFrequencia, dreNome, ueNome, bimestre, ano, turma, componenteCurricular, alunoCodigo.ToString(), alunoNome);
                relatorioBase = relatorioFrequencia;
            }

            if (tipoRelatorio != TipoRelatorioFaltasFrequencia.Turma)
                SetarFrequencia(ref relatorioBase, tipoRelatorio, frequencia);

            if (tipoRelatorio != TipoRelatorioFaltasFrequencia.Ano)
                SetarFaltas(ref relatorioBase, tipoRelatorio, totalAulas, totalAusencias);

            return relatorioBase;
        }


        private void SetarFrequencia(ref RelatorioFaltasFrequenciasBaseExcelDto relatorioDto, TipoRelatorioFaltasFrequencia tipoRelatorio, double frequenciaPercentual)
        {
            if (tipoRelatorio == TipoRelatorioFaltasFrequencia.Ano)
                ((RelatorioFaltasFrequenciasExcelDto)relatorioDto).FrequenciaPercentual = frequenciaPercentual;
            else
                ((RelatorioFrequenciasExcelDto)relatorioDto).FrequenciaPercentual = frequenciaPercentual;
        }

        private void SetarFaltas(ref RelatorioFaltasFrequenciasBaseExcelDto relatorioDto, TipoRelatorioFaltasFrequencia tipoRelatorio, int totalAulas, int totalFaltas)
        {
            if (tipoRelatorio == TipoRelatorioFaltasFrequencia.Ano)
            {
                ((RelatorioFaltasFrequenciasExcelDto)relatorioDto).FaltasQuantidade = totalFaltas;
                ((RelatorioFaltasFrequenciasExcelDto)relatorioDto).AulasQuantidade = totalAulas;
            }
            else
            {
                ((RelatorioFaltasExcelDto)relatorioDto).FaltasQuantidade = totalFaltas;
                ((RelatorioFaltasExcelDto)relatorioDto).AulasQuantidade = totalAulas;
            }
        }
    }
}
