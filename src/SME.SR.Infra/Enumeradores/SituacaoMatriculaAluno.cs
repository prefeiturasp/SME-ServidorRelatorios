using System.ComponentModel.DataAnnotations;

namespace SME.SR.Infra
{
    public enum SituacaoMatriculaAluno
    {
        [Display(Name = "Ativo")]
        Ativo = 1,

        [Display(Name = "Desistente", ShortName = "Ret Freq")]
        Desistente = 2,

        [Display(Name = "Transferido", ShortName = "TR")]
        Transferido = 3,

        [Display(Name = "Vínculo Indevido", ShortName = "VI")]
        VinculoIndevido = 4,

        [Display(Name = "Concluído")]
        Concluido = 5,

        [Display(Name = "Pendente Rematricula")]
        PendenteRematricula = 6,

        [Display(Name = "Falecido", ShortName = "FL")]
        Falecido = 7,

        [Display(Name = "Não Compareceu", ShortName = "NC")]
        NaoCompareceu = 8,

        [Display(Name = "Rematriculado")]
        Rematriculado = 10,

        [Display(Name = "Deslocamento", ShortName = "DESL")]
        Deslocamento = 11,

        [Display(Name = "Cessado", ShortName = "CES")]
        Cessado = 12,

        [Display(Name = "Sem Continuidade")]
        SemContinuidade = 13,

        [Display(Name = "Remanejado", ShortName = "RM")]
        RemanejadoSaida = 14,

        [Display(Name = "Reclassificado", ShortName = "RC")]
        ReclassificadoSaida = 15,

        [Display(Name = "Transferido SED")]
        TransferidoSED = 16,

        [Display(Name = "Dispensado Ed. Física")]
        DispensadoEdFisica = 17
    }
}
