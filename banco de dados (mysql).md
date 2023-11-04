CREATE TABLE Users (
    UserId INT NOT NULL AUTO_INCREMENT,
    NomeCompleto VARCHAR(100) NOT NULL,
    Endereco VARCHAR(100),
    CPF VARCHAR(11) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    ConfirmPassword VARCHAR(255) NOT NULL,
    Celular VARCHAR(20) NOT NULL,
    PRIMARY KEY (UserId),
    UNIQUE KEY (Email)
);

CREATE TABLE Accounts (
    AccountId INT NOT NULL AUTO_INCREMENT,
    UserId INT NOT NULL,
    AccountNumber VARCHAR(50) NOT NULL,
    Balance DECIMAL(10, 2) NOT NULL DEFAULT '0.00',
    AccountType ENUM('Checking', 'Savings') NOT NULL,
    OpenedDate DATE NOT NULL,
    PRIMARY KEY (AccountId),
    UNIQUE KEY (AccountNumber),
    FOREIGN KEY (UserId) REFERENCES Users (UserId)
);

CREATE TABLE Transactions (
    TransactionId INT NOT NULL AUTO_INCREMENT,
    AccountId INT NOT NULL,
    TransactionType ENUM('Deposit', 'Withdrawal', 'Transfer') NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    Date DATETIME NOT NULL,
    OtherAccount INT,
    PRIMARY KEY (TransactionId),
    FOREIGN KEY (AccountId) REFERENCES Accounts (AccountId)
);

CREATE TABLE CartoesDeCredito (
    NumeroDoCartao VARCHAR(16) PRIMARY KEY,
    TitularDoCartao VARCHAR(100) NOT NULL,
    LimiteDeCredito DECIMAL(10, 2) NOT NULL,
    SaldoDisponivel DECIMAL(10, 2) NOT NULL,
    DataDeVencimento DATE NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users (UserId),
    Bloqueado CHAR(3) CHECK (Bloqueado IN ('Sim', 'Não')) NOT NULL
);

CREATE TABLE Emprestimos (
    IdEmprestimo INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    NumeroDoContrato VARCHAR(20) UNIQUE NOT NULL,
    ValorDoEmprestimo DECIMAL(12, 2) NOT NULL,
    TaxaDeJuros DECIMAL(5, 2) NOT NULL,
    Parcelas INT NOT NULL,
    ValorDaParcela DECIMAL(10, 2) NOT NULL,
    DataDoContrato DATE NOT NULL,
    DataDeVencimento DATE NOT NULL,
    StatusDoContrato VARCHAR(20) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users (UserId)
);

CREATE TABLE Investimentos (
    IdInvestimento INT PRIMARY KEY AUTO_INCREMENT,
    TipoDeInvestimento VARCHAR(50) NOT NULL,
    TitularDoInvestimento VARCHAR(100) NOT NULL,
    ValorInvestido DECIMAL(12, 2) NOT NULL,
    DataDeAplicacao DATE NOT NULL,
    DataDeVencimento DATE NOT NULL,
    TaxaDeRendimento DECIMAL(5, 2) NOT NULL,
    StatusDoInvestimento VARCHAR(20) NOT NULL
);
