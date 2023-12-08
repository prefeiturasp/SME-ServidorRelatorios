using FluentValidation;
using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class VerificarPercursoTurmaAlunoComImagemBase64Query : IRequest<IEnumerable<AcompanhamentoTurmaAlunoImagemBase64Dto>>
    {
        public VerificarPercursoTurmaAlunoComImagemBase64Query(long turmaId, int semestre, params string[] tagsImagemConsideradas)
        {
            TurmaId = turmaId;
            Semestre = semestre;
            TagsImagemConsideradas = tagsImagemConsideradas;
        }

        public long TurmaId { get; set; }
        public int Semestre { get; set; }
        public string[] TagsImagemConsideradas { get; set; }
    }

    public class VerificarPercursoTurmaAlunoComImagemBase64QueryValidator : AbstractValidator<VerificarPercursoTurmaAlunoComImagemBase64Query>
    {
        public VerificarPercursoTurmaAlunoComImagemBase64QueryValidator()
        {
            RuleFor(x => x.TurmaId)
                .GreaterThan(0)
                .WithMessage("O id da turma deve ser informado.");

            RuleFor(x => x.Semestre)
                .GreaterThan(0)
                .WithMessage("O semestre deve ser informado");

            RuleFor(x => x.TagsImagemConsideradas)
                .NotEmpty()
                .WithMessage("As tags de imagem consideradas deve ser informadas");
        }
    }
}
