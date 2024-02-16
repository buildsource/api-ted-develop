#Cliente SolicitaTed.feature

Funcionalidade: SolicitaTed
  Preciso testar o método SolicitaTed
  Para garantir que ele funcione corretamente em diferentes cenários

  Cenário: Solicitar TED dentro do limite retorna sucesso
    Dado que a solicitação de TED está dentro do limite
    Quando eu chamo o método SolicitaTed
    Então a solicitação é processada com sucesso

  Cenário: Solicitar TED dentro do limite com erro de validação retorna exceção
    Dado que a solicitação de TED está dentro do limite mas com dados inválidos
    Quando eu chamo o método SolicitaTed
    Então uma exceção de validação é lançada

  Cenário: Solicitar TED acima da quantidade máxima por dia retorna exceção
    Dado que a solicitação de TED excede a quantidade máxima por dia
    Quando eu chamo o método SolicitaTed
    Então uma exceção de limite de quantidade é lançada

  Cenário: Solicitar TED acima do valor máximo por dia retorna exceção
    Dado que a solicitação de TED excede o valor máximo por dia
    Quando eu chamo o método SolicitaTed
    Então uma exceção de limite de valor diário é lançada

  Cenário: Solicitar TED acima do valor máximo por saque retorna exceção
    Dado que a solicitação de TED excede o valor máximo por saque
    Quando eu chamo o método SolicitaTed
    Então uma exceção de limite de valor por saque é lançada