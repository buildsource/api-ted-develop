#Admin AprovaTed.feature

Funcionalidade: AprovaTed
  Preciso testar o método AprovaTed
  Para garantir que ele funcione corretamente em diferentes cenários

  Cenário: Aprovar TED pelo ID retorna sucesso
    Dado que eu tenho um TED ID válido para aprovação
    Quando eu chamo o método AprovaTed com esse TED ID
    Então a aprovação é processada com sucesso

  Cenário: Aprovar TED pelo ID com erro de validação retorna erro
    Dado que eu tenho um TED ID inválido
    Quando eu chamo o método AprovaTed com esse TED ID
    Então um erro de validação é retornado