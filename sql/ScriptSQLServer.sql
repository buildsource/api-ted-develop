CREATE DATABASE ted;

USE ted;

CREATE LOGIN api_gtw_ted WITH PASSWORD = 'A!G_c34K2*1$';

CREATE USER api_gtw_ted FOR LOGIN api_gtw_ted;


DROP TABLE IF EXISTS LimiteTed;

CREATE TABLE LimiteTed (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ValorMaximoDia FLOAT NOT NULL,
    QuantidadeMaximaDia INT NOT NULL,
    ValorMaximoPorSaque FLOAT NOT NULL,
    CriadoEm DATETIME2 NOT NULL DEFAULT GETDATE(),
    AtualizadoEm DATETIME2 NOT NULL DEFAULT GETDATE(),
);

INSERT INTO LimiteTed (ValorMaximoDia, QuantidadeMaximaDia, ValorMaximoPorSaque, CriadoEm, AtualizadoEm)
VALUES (10000.00, 3, 5000.00, GETDATE(), GETDATE());


DROP TABLE IF EXISTS Ted;

CREATE TABLE Ted (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Status INT NOT NULL,
    ClienteId INT NOT NULL,
    NomeCliente NVARCHAR(MAX) NOT NULL,
    DataAgendamento DATETIME2 NOT NULL,
    ValorSolicitado FLOAT NOT NULL,
    NumeroAgencia NVARCHAR(MAX) NOT NULL,
    NumeroConta NVARCHAR(MAX) NOT NULL,
    DigitoConta NVARCHAR(MAX) NOT NULL,
    NumeroBanco NVARCHAR(MAX) NOT NULL,
    NomeBanco NVARCHAR(MAX) NOT NULL,
    MotivoReprovacao NVARCHAR(MAX),
    SinacorConfirmacaoId NVARCHAR(MAX),
    CriadoEm DATETIME2 NOT NULL DEFAULT GETDATE(),
    AtualizadoEm DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE INDEX IDX_Ted_Status ON Ted (Status);
CREATE INDEX IDX_Ted_ClienteId ON Ted (ClienteId);
