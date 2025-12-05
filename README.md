# API de Gerenciamento de Usuários

## Descrição
API RESTful desenvolvida em ASP.NET Core 8 com Minimal APIs para gerenciamento de usuários. A aplicação implementa operações CRUD completas seguindo os princípios de Clean Architecture e padrões de projeto como Repository Pattern e Service Pattern.

A solução foi desenvolvida como parte de avaliação acadêmica, demonstrando a aplicação de boas práticas de desenvolvimento, validação de dados com FluentValidation e persistência com Entity Framework Core.

## Vídeo de Apresentação

Link do vídeo apresentando o código com testes no Postman:

https://drive.google.com/file/d/1JVpTaXz9dTOuq75oicEbVOARgznyTq5H/view?usp=sharing

## Tecnologias Utilizadas
- .NET 8.0
- ASP.NET Core Minimal APIs
- Entity Framework Core 8.0
- SQLite (banco de dados)
- FluentValidation 11.3
- Swagger/OpenAPI (documentação)
- Dependency Injection

## Padrões de Projeto Implementados
- **Repository Pattern**: Abstração da camada de acesso a dados
- **Service Pattern**: Centralização da lógica de negócio
- **DTO Pattern**: Separação entre modelos de domínio e transferência
- **Dependency Injection**: Injeção de dependências para baixo acoplamento
- **Clean Architecture**: Separação em camadas (Domain, Application, Infrastructure)

## Como Executar o Projeto

### Pré-requisitos
- .NET SDK 8.0 ou superior
- Visual Studio 2022 / Visual Studio Code ou qualquer editor de código
- Postman ou similar para testar endpoints

### Passos
1. Clone o repositório
   ```bash
   git clone https://github.com/seu-usuario/api-usuarios-as-seu-nome.git
   cd APIUsuarios