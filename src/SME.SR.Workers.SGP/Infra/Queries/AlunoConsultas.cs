using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Workers.SGP.Infra.Queries
{
    public static class AlunoConsultas
    {
		internal static string DadosAluno = @"SELECT
			CodigoAluno,
			NomeAluno,
			NomeSocialAluno,
			DataNascimento,
			CodigoSituacaoMatricula,
			SituacaoMatricula,
			MAX(DataSituacao) DataSituacao ,
			NumeroAlunoChamada,
			PossuiDeficiencia
		FROM
			tempdb..#tmpAlunosFrequencia
		GROUP BY
			CodigoAluno,
			NomeAluno,
			NomeSocialAluno,
			DataNascimento,
			CodigoSituacaoMatricula,
			SituacaoMatricula,
			NumeroAlunoChamada,
			PossuiDeficiencia";

	}
}
