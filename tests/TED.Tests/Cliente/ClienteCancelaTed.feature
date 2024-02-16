#Cliente CancelaTed.feature

Funcionalidade: CancelaTed
  Preciso testar o método CancelaTed
  Para garantir que ele funcione corretamente em diferentes cenários

  Cenário: Cancelar TED pelo Cliente ID retorna sucesso
    Dado que eu tenho um Cliente ID válido para cancelamento
    Quando eu chamo o método CancelaTed com esse Cliente ID
    Então o cancelamento é processado com sucesso

  Cenário: Cancelar TED pelo Cliente ID com erro de validação retorna exceção
    Dado que eu tenho um Cliente ID inválido para cancelamento
    Quando eu chamo o método CancelaTed com esse Cliente ID
    Então uma exceção de validação é lançada

    //tem mais cenários