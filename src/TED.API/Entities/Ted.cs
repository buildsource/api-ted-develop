using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TED.API.Enums;

namespace TED.API.Entities;

/// <summary>
/// Representa uma transação TED (Transferência Eletrônica Disponível) no sistema.
/// </summary>
[Index(nameof(Status))]
[Index(nameof(ClienteId))]
public class Ted : BaseEntity
{
    /// <summary>
    /// Inicializa uma nova instância da classe Ted com o status padrão 'InProcess'.
    /// </summary>
    public Ted()
    {
        Status = StatusEnum.InProcess;
    }

    /// <summary>
    /// Status atual da transação TED.
    /// </summary>
    [EnumDataType(typeof(StatusEnum))]
    public StatusEnum Status { get; set; }

    /// <summary>
    /// ID do cliente associado à transação TED.
    /// </summary>
    public int ClienteId { get; set; }

    /// <summary>
    ///Nome do cliente associado à transação TED.
    /// </summary>
    public string NomeCliente { get; set; }

    /// <summary>
    /// Data agendada para a realização da transação TED.
    /// </summary>
    public DateTime DataAgendamento { get; set; }

    /// <summary>
    /// Valor solicitado para a transação TED.
    /// </summary>
    public double ValorSolicitado { get; set; }

    /// <summary>
    /// Número da agência do cliente.
    /// </summary>
    public string NumeroAgencia { get; set; }

    /// <summary>
    /// Número da conta do cliente.
    /// </summary>
    public string NumeroConta { get; set; }

    /// <summary>
    /// Dígito da conta do cliente.
    /// </summary>
    public string DigitoConta { get; set; }

    /// <summary>
    /// Número do banco para a transação TED.
    /// </summary>
    public string NumeroBanco { get; set; }

    /// <summary>
    /// Nome do banco para a transação TED.
    /// </summary>
    public string NomeBanco { get; set; }

    /// <summary>
    /// Motivo da reprovação da transação TED, se aplicável.
    /// </summary>
    public string? MotivoReprovacao { get; set; }

    /// <summary>
    /// ID de confirmação Sinacor, se aplicável.
    /// </summary>
    public string? SinacorConfirmacaoId { get; set; }
}
