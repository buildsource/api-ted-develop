#Admin ReprovaTed.feature

Funcionalidade: ReprovaTed
  Preciso testar o método ReprovaTed
  Para garantir que ele funcione corretamente em diferentes cenários

  Cenário: Reprovar TED pelo ID retorna sucesso
    Dado que eu tenho um TED ID válido para reprovação
    Quando eu chamo o método ReprovaTed com esse TED ID
    Então a reprovação é processada com sucesso

  Cenário: Reprovar TED pelo ID com erro de validação retorna erro
    Dado que eu tenho um TED ID inválido
    Quando eu chamo o método ReprovaTed com esse TED ID
    Então um erro de validação é retornado