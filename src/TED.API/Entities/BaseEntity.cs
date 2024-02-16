using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TED.API.Entities;

/// <summary>
/// Classe base para entidades, fornecendo propriedades comuns como Id, CriadoEm e AtualizadoEm.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Chave primária da entidade, gerada automaticamente pelo banco de dados.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Data e hora de criação da entidade, definida automaticamente quando a entidade é criada.
    /// </summary>
    public DateTime CriadoEm { get; private set; } = DateTime.Now;

    /// <summary>
    /// Data e hora da última atualização da entidade, atualizada automaticamente quando a entidade é modificada.
    /// </summary>
    public DateTime AtualizadoEm { get; set; } = DateTime.Now;
}
