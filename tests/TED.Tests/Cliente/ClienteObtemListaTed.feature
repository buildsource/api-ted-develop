# Cliente ObtemListaTed.feature

Funcionalidade: ObtemListaTed
  Preciso testar o método ObtemListaTed
  Para garantir que ele funcione corretamente em diferentes cenários

  Cenário: Obter lista TED com Cliente ID válido retorna lista não vazia
    Dado que eu tenho um Cliente ID válido
    Quando eu chamo o método ObtemListaTed com esse Cliente ID
    Então uma lista não vazia de TEDs é retornada

  Cenário: Obter lista TED pelo Cliente ID com erro de validação retorna erro
    Dado que eu tenho um Cliente ID inválido
    Quando eu chamo o método ObtemListaTed com esse Cliente ID
    Então um erro de validação é retornado

  Cenário: Obter lista TED pelo Cliente ID retorna lista vazia
    Dado que eu tenho um Cliente ID sem TEDs associados
    Quando eu chamo o método ObtemListaTed com esse Cliente ID
    Então uma lista vazia é retornada

  Cenário: Obter lista TED pelo Cliente ID filtrada por status retorna lista não vazia
    Dado que eu tenho um Cliente ID válido e um status específico
    Quando eu chamo o método ObtemListaTed com esse Cliente ID e status
    Então uma lista não vazia filtrada de TEDs é retornada

  Cenário: Obter lista TED pelo Cliente ID filtrada por status retorna lista vazia
    Dado que eu tenho um Cliente ID válido e um status sem TEDs associados
    Quando eu chamo o método ObtemListaTed com esse Cliente ID e status
    Então uma lista vazia é retornada

  Cenário: Obter lista TED pelo Cliente ID filtrada por data de início retorna lista não vazia
    Dado que eu tenho um Cliente ID válido e uma data de início específica
    Quando eu chamo o método ObtemListaTed com esse Cliente ID e data de início
    Então uma lista não vazia filtrada de TEDs é retornada

  Cenário: Obter lista TED pelo Cliente ID filtrada por data de início retorna lista vazia
    Dado que eu tenho um Cliente ID válido e uma data de início sem TEDs associados
    Quando eu chamo o método ObtemListaTed com esse Cliente ID e data de início
    Então uma lista vazia é retornada

  Cenário: Obter lista TED pelo Cliente ID filtrada por data de início e data de fim retorna lista não vazia
    Dado que eu tenho um Cliente ID válido, uma data de início e uma data de fim
    Quando eu chamo o método ObtemListaTed com esse Cliente ID, data de início e data de fim
    Então uma lista não vazia filtrada de TEDs é retornada

  Cenário: Obter lista TED pelo Cliente ID filtrada por data de início e data de fim retorna lista vazia
    Dado que eu tenho um Cliente ID válido, uma data de início e uma data de fim sem TEDs associados
    Quando eu chamo o método ObtemListaTed com esse Cliente ID, data de início e data de fim
    Então uma lista vazia é retornada

  Cenário: Obter lista TED pelo Cliente ID filtrada pelo status, data de início e data de fim retorna lista não vazia
    Dado que eu tenho um Cliente ID válido, um status específico, uma data de início e uma data de fim
    Quando eu chamo o método ObtemListaTed com esse Cliente ID, status, data de início e data de fim
    Então uma lista não vazia filtrada de TEDs é retornada

  Cenário: Obter lista TED pelo Cliente ID filtrada pelo status, data de início e data de fim retorna lista vazia
    Dado que eu tenho um Cliente ID válido, um status específico, uma data de início e uma data de fim sem TEDs associados
    Quando eu chamo o método ObtemListaTed com esse Cliente ID, status, data de início e data de fim
    Então uma lista vazia é retornada
