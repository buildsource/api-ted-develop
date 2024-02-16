using System.Text.Json.Serialization;
using TED.API.Annotations;

namespace TED.API.DTOs;

/// <summary>
/// Representa uma solicitação de TED no sistema Sinacor. 
/// Esta classe é usada para encapsular todos os dados necessários para processar uma transferência eletrônica de fundos, 
/// incluindo informações detalhadas sobre o lançamento, cliente, e detalhes bancários.
/// </summary>
[IncludeInSwagger]
public class SinacorTedRequestDto
{
    /// <summary>
    /// Lista de lançamentos associados à solicitação de TED.
    /// </summary>
    [JsonPropertyName("listaLancamentos")]
    public List<Lancamento> Lancamentos { get; set; }

    public SinacorTedRequestDto() {
        Lancamentos = new List<Lancamento>();
    }
}

/// <summary>
/// Representa um lançamento individual dentro de uma solicitação de TED.
/// Contém detalhes sobre o movimento financeiro, cliente e informações bancárias.
/// </summary>
public class Lancamento
{
    public Lancamento()
    {
        DescricaoLcto = "TED";
        CodigoGrupoLiquidacao = 17;
        CodigoBanco = "467";
        CodigoAgencia = "0001";
        NumeroConta = 10001;
        DigitoAgenciaCliente = "";
        TipoContaCliente = "CC";
        IndicadorSituacao = "E";
        CodigoSistemaExterno = "SPBX";
    }

    /// <summary>
    /// Identificador único do lançamento.
    /// </summary>
    [JsonPropertyName("idLcto")]
    public int IdLcto { get; set; }

    /// <summary>
    /// Data do movimento financeiro.
    /// </summary>
    [JsonPropertyName("dataMovimento")]
    public string DataMovimento { get; set; }

    /// <summary>
    /// Data de referência para o lançamento.
    /// </summary>
    [JsonPropertyName("dataReferencia")]
    public string DataReferencia { get; set; }

    /// <summary>
    /// Código identificador do cliente.
    /// </summary>
    [JsonPropertyName("codigoCliente")]
    public int CodigoCliente { get; set; }

    /// <summary>
    /// Valor monetário do lançamento.
    /// </summary>
    [JsonPropertyName("valorLcto")]
    public double ValorLcto { get; set; }

    /// <summary>
    /// Descrição detalhada do lançamento.
    /// </summary>
    [JsonPropertyName("descricaoLcto")]
    public string DescricaoLcto { get; set; }

    /// <summary>
    /// Código do grupo de liquidação associado ao lançamento.
    /// </summary>
    [JsonPropertyName("codigoGrupoLiquidacao")]
    public int CodigoGrupoLiquidacao { get; set; }

    /// <summary>
    /// Código do banco para transações.
    /// </summary>
    [JsonPropertyName("codigoBanco")]
    public string CodigoBanco { get; set; }

    /// <summary>
    /// Código da agência bancária.
    /// </summary>
    [JsonPropertyName("codigoAgencia")]
    public string CodigoAgencia { get; set; }

    /// <summary>
    /// Número da conta bancária.
    /// </summary>
    [JsonPropertyName("numeroConta")]
    public int NumeroConta { get; set; }

    /// <summary>
    /// Código do banco do cliente.
    /// </summary>
    [JsonPropertyName("codigoBancoCliente")]
    public string CodigoBancoCliente { get; set; }

    /// <summary>
    /// Código da agência do cliente.
    /// </summary>
    [JsonPropertyName("codigoAgenciaCliente")]
    public string CodigoAgenciaCliente { get; set; }

    /// <summary>
    /// Dígito verificador da agência do cliente.
    /// </summary>
    [JsonPropertyName("digitoAgenciaCliente")]
    public string DigitoAgenciaCliente { get; set; }

    /// <summary>
    /// Número da conta do cliente.
    /// </summary>
    [JsonPropertyName("numeroContaCliente")]
    public string NumeroContaCliente { get; set; }

    /// <summary>
    /// Dígito verificador da conta do cliente.
    /// </summary>
    [JsonPropertyName("digitoContaCliente")]
    public string DigitoContaCliente { get; set; }

    /// <summary>
    /// Tipo da conta do cliente (ex: corrente, poupança).
    /// </summary>
    [JsonPropertyName("tipoContaCliente")]
    public string TipoContaCliente { get; set; }

    /// <summary>
    /// Indicador da situação atual do lançamento.
    /// </summary>
    [JsonPropertyName("indicadorSituacao")]
    public string IndicadorSituacao { get; set; }

    /// <summary>
    /// Código do sistema externo que está inserindo os dados.
    /// </summary>
    [JsonPropertyName("codigoSistemaExterno")]
    public string CodigoSistemaExterno { get; set; }
}

