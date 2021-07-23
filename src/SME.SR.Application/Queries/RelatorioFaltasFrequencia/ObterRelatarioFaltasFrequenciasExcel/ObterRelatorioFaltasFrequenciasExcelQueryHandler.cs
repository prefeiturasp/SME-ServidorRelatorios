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
                                                                                            ano.NomeAno, componente.NomeComponente, aluno));
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
                                                                                      string ano, string componenteCurricular, RelatorioFaltaFrequenciaAlunoDto aluno)
        {
            RelatorioFaltasFrequenciasBaseExcelDto relatorioBase;

            if (tipoRelatorio == TipoRelatorioFaltasFrequencia.Ano)
            {
                var relatorioAmbos = new RelatorioFaltasFrequenciasExcelDto();
                ObterRelatorioFaltasFrequenciaBase(ref relatorioAmbos, dreNome, ueNome, bimestre, ano, aluno.NomeTurma, componenteCurricular, aluno.CodigoAluno.ToString(), aluno.NomeAluno);
                relatorioBase = relatorioAmbos;
            }
            else if (tipoRelatorio == TipoRelatorioFaltasFrequencia.Turma)
            {
                var relatorioFaltas = new RelatorioFaltasExcelDto();
                ObterRelatorioFaltasFrequenciaBase(ref relatorioFaltas, dreNome, ueNome, bimestre, ano, aluno.NomeTurma, componenteCurricular, aluno.CodigoAluno.ToString(), aluno.NomeAluno);
                relatorioBase = relatorioFaltas;
            }
            else
            {
                var relatorioFrequencia = new RelatorioFrequenciasExcelDto();
                ObterRelatorioFaltasFrequenciaBase(ref relatorioFrequencia, dreNome, ueNome, bimestre, ano, aluno.NomeTurma, componenteCurricular, aluno.CodigoAluno.ToString(), aluno.NomeAluno);
                relatorioBase = relatorioFrequencia;
            }

            if (tipoRelatorio != TipoRelatorioFaltasFrequencia.Turma)
                SetarFrequencia(ref relatorioBase, tipoRelatorio, aluno);

            if (tipoRelatorio != TipoRelatorioFaltasFrequencia.Ano)
                SetarFaltas(ref relatorioBase, tipoRelatorio, aluno);

            return relatorioBase;
        }


        private void SetarFrequencia(ref RelatorioFaltasFrequenciasBaseExcelDto relatorioDto, TipoRelatorioFaltasFrequencia tipoRelatorio, RelatorioFaltaFrequenciaAlunoDto aluno)
        {

            ((RelatorioFaltasFrequenciasExcelDto)relatorioDto).FrequenciaPercentual = aluno.Frequencia;
            ((RelatorioFaltasFrequenciasExcelDto)relatorioDto).TotalRemoto = aluno.TotalRemoto;
            ((RelatorioFaltasFrequenciasExcelDto)relatorioDto).TotalPresenca = aluno.TotalPresenca;
            ((RelatorioFaltasFrequenciasExcelDto)relatorioDto).FaltasQuantidade = aluno.TotalAusencias;
            ((RelatorioFaltasFrequenciasExcelDto)relatorioDto).TotalCompensacoes = aluno.TotalCompensacoes;
        }

        private void SetarFaltas(ref RelatorioFaltasFrequenciasBaseExcelDto relatorioDto, TipoRelatorioFaltasFrequencia tipoRelatorio, RelatorioFaltaFrequenciaAlunoDto aluno)
        {

            ((RelatorioFaltasExcelDto)relatorioDto).FaltasQuantidade = aluno.TotalAusencias;
            ((RelatorioFaltasExcelDto)relatorioDto).TotalRemoto = aluno.TotalRemoto;
            ((RelatorioFaltasExcelDto)relatorioDto).AulasQuantidade = aluno.TotalAulas;
            ((RelatorioFaltasExcelDto)relatorioDto).TotalPresenca = aluno.TotalPresenca;
            ((RelatorioFaltasExcelDto)relatorioDto).TotalCompensacoes = aluno.TotalCompensacoes;
        }
    }
}
