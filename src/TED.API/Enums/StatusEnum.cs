using System.ComponentModel;

namespace TED.API.Enums;

/// <summary>
/// Enumerador para representar os diferentes status de um TED.
/// </summary>
public enum StatusEnum
{
    /// <summary>
    /// O TED está em processo de aprovação.
    /// </summary>
    [Description("In process of approval")]
    InProcess,

    /// <summary>
    /// O TED foi aprovado.
    /// </summary>
    [Description("Approved status")]
    Approved,

    /// <summary>
    /// O TED foi cancelado.
    /// </summary>
    [Description("Canceled status")]
    Canceled,

    /// <summary>
    /// O TED foi reprovado.
    /// </summary>
    [Description("Disapproved status")]
    Disapproved
}

