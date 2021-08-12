CREATE TABLE Estabelecimento 
(
  Id int not NULL auto_increment,
  Nome Varchar(200) not NULL,
  Logradouro Varchar(200) not NULL,
  Numero numeric(6) NULL,
  Bairro varchar(100) not NULL,
  Cidade Varchar(100) not NULL,
  Estado varchar(20) not NULL,
  Pais Varchar(20) not NULL,
  CEP Varchar(15) not NULL,
  Telefone varchar(15) not NULL,
  HorarioFuncionamento varchar(100) not NULL,
  
  CONSTRAINT PK_Estabelecimento
  PRIMARY KEY (Id)
  )




CREATE table Vaga
(
  Id int not NULL auto_increment,
  IdEstabelecimento int not NULL,
  Localizacao varchar(200) not NULL,
  Status varchar(100) not NULL,
  
  CONSTRAINT PK_Vaga
  PRIMARY KEY (Id),
  
  CONSTRAINT FK_Vaga_Estabelecimento
  FOREIGN key (IdEstabelecimento)
  REFERENCES Estabelecimento (Id)
  )




Create table Cliente
(
  Id int NOT NULL auto_increment,
  Nome Varchar(200) not NULL,
  CPF varchar (20) not NULL,
  RG varchar (20) NULL,
  Telefone varchar(15) NULL,
  Logradouro varchar(200) NULL,
  Numero numeric(6) NULL,
  Bairro varchar(100) NULL,
  Cidade Varchar(100) NULL,
  Estado varchar(20) NULL,
  Pais varchar(100) NULL,
  CEP varchar(20) NULL,
  Genero char not NULL,
  DataNascimento Datetime NULL,
  
  CONSTRAINT PK_Cliente
  Primary key (Id)
  )





Create table Manobrista 
(
 Id int NOT NULL auto_increment,
  Nome Varchar(200) not NULL,
  CPF varchar (20) not NULL,
  RG varchar (20) not NULL,
  Telefone varchar(15) NULL,
  Logradouro varchar(200) NULL,
  Numero numeric(6) NULL,
  Bairro varchar(100) NULL,
  Cidade Varchar(100) NULL,
  Estado varchar(20) NULL,
  Pais varchar(100) NULL,
  CEP varchar(20) NULL,
  Genero char not NULL,
  DataNascimento DateTime not NULL,
  DataAdmissao DateTime not NULL,
  
  CONSTRAINT PK_Manobrista
  PRIMARY KEY (Id)
  )





CREATE TABLE TipoVeiculo
(
Id int not NULL auto_increment,
Tipo varchar(100) not NULL,  
Valor decimal(10,2) not NULL,

CONTRAINT PK_TipoVeiculo
PRIMARY KEY(Id)
)




  CREATE TABLE Veiculo
(
  Id int not NULL auto_increment,
  IdTipoVeiculo int not NULL,
  Marca varchar(100) not NULL,
  Modelo varchar (100) not NULL,
  Placa varchar(15) not NULL,
  Cor varchar(50) not NULL, 

  CONSTRAINT PK_Veiculo
  PRIMARY KEY (Id),

  CONTRAINT FK_Veiculo_TipoVeiculo
  FOREIGN KEY (IdTipoVeiculo)
  REFERENCES TipoVeiculo(Id)
)




CREATE TABLE EstabelecimentoTipoVeiculo
(
Id int not NULL auto_increment,
IdEstabelecimento int not NULL,
IdTipoVeiculo int not NULL,

CONSTRAINT PK_EstabelecimentoTipoVeiculo
PRIMARY KEY(Id),

CONSTRAINT FK_EstabelecimentoTipoVeiculo_Estabelecimento
FOREIGN KEY (IdEstabelecimento)
REFERENCES Estabelecimento(Id),

CONSTRAINT FK_EstabelecimentoTipoVeiculo_TipoVeiculo
FOREIGN KEY (IdTipoVeiculo)
REFERENCES TipoVeiculo(Id)
)


CREATE TABLE Estacionamento
(
  Id Int not NULL auto_increment,
  IdVeiculo int not NULL,
  IdVaga int not NULL,
  IdManobrista int not NULL,
  IdCliente int NULL,
  Data Datetime not NULL,
  Situacao varchar(50) not NULL,
  FormaPagamento varchar(100) NULL,
  ValorTotal decimal(10,2) NULL,
  DataPagamento Datetime NULL,
   
  
  CONSTRAINT PK_Estacionamento
  PRIMARY KEY (Id),
  
  CONSTRAINT FK_Estacionamento_Vaga
  FOREIGN key (IdVaga)
  References Vaga (Id),
  
  CONSTRAINT FK_Estacionamento_Cliente
  FOREIGN KEY (IdCliente)
  REFERENCES Cliente (Id),
  
  CONSTRAINT FK_Estacionamento_Manobrista
  FOREIGN KEY (IdManobrista)
  REFERENCES Manobrista (Id),

  CONSTRAINT FK_Estacionamento_Veiculo
  FOREIGN KEY (IdVeiculo)
  REFERENCES Veiculo(Id)

)