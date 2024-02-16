#Cliente EnviaTedParaSinacor.feature

Funcionalidade: EnvioDeTedParaSinacor
  Preciso testar o método de envio de TED para Sinacor
  Para garantir que ele funcione corretamente em diferentes cenários

  Cenário: Enviar TED para Sinacor retorna sucesso
    Dado que eu tenho uma solicitação de TED válida para Sinacor
    Quando eu chamo o método de envio de TED para Sinacor
    Então a solicitação é processada com sucesso

  Cenário: Enviar TED para Sinacor com erro de validação retorna exceção
    Dado que eu tenho uma solicitação de TED inválida para Sinacor
    Quando eu chamo o método de envio de TED para Sinacor
    Então uma exceção de validação é lançada

  Cenário: Enviar TED para Sinacor retorna exceção
    Dado que eu tenho uma solicitação de TED que causa uma exceção
    Quando eu chamo o método de envio de TED para Sinacor
    Então uma exceção é lançada
