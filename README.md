# Teste Técnico Rota das Oficinas

Este projeto serve como template para a criação de uma Web API básica de e-commerce, parte do Teste Técnico da Rota das Oficinas.

Ele já contém a estrutura básica de uma API que **deve ser seguida** ao adicionar novas funcionalidades.

### Características do Template

Algumas características e tecnologias utilizadas neste template incluem:

- Construído com **.NET 8.0**.
- Utiliza **EntityFramework Core** como ORM (Object-Relational Mapper).
- Segue os padrões **CQRS** (Command Query Responsibility Segregation) e **Repository**.
- Utiliza **PostgreSQL** como engine de banco de dados.
- Utiliza **Xunit**, **Bogus** e **FluentAssertions** para a criação de testes.

### Pontos a Implementar

No template fornecido, existem funcionalidades pendentes que precisam ser implementadas para completar a API conforme os requisitos do teste. Procure por comentários contendo **[TODO]** no código para encontrar esses pontos.

---

### Comandos Essenciais do .NET CLI

Aqui estão alguns comandos básicos do .NET CLI (Command Line Interface) úteis para desenvolver e gerenciar este projeto. Embora este template já venha com a estrutura pronta, entender estes comandos ajuda a compreender como projetos .NET são criados e gerenciados.

- **`dotnet new <template>`**: Cria um novo projeto ou arquivo baseado em um template.
  - `dotnet new sln -o MinhaSolucao`: Cria um arquivo de solução (`.sln`).
  - `dotnet new webapi -o MeuProjetoWebApi`: Cria um novo projeto de Web API.
  - `dotnet new classlib -o MinhaBibliotecaDeClasses`: Cria um novo projeto de biblioteca de classes (como as camadas Domain, Application, Persistence, Infrastructure).
- **`dotnet sln <ARQUIVO_SLN> add <ARQUIVO_CSPROJ>`**: Adiciona um arquivo de projeto (`.csproj`) a um arquivo de solução (`.sln`).
  - `dotnet sln RO.DevTest.sln add RO.DevTest.WebApi\RO.DevTest.WebApi.csproj`: Exemplo de como adicionar um projeto Web API à solução (se o esqueleto não viesse pronto).
- **`dotnet add <ARQUIVO_CSPROJ> reference <ARQUIVO_CSPROJ_REFERENCIADO>`**: Adiciona uma referência de projeto a outro projeto. Essencial para conectar as camadas.
  - `dotnet add RO.DevTest.WebApi\RO.DevTest.WebApi.csproj reference RO.DevTest.Application\RO.DevTest.Application.csproj`: Exemplo: WebApi referenciando Application.
- **`dotnet restore`**: Restaura as dependências e ferramentas de um projeto (geralmente executado automaticamente por `build`, `run`, etc.).
- **`dotnet build`**: Compila o projeto ou solução.
  - Execute na raiz da solução (`.sln`) para compilar todos os projetos.
  - Execute na pasta de um projeto (`.csproj`) para compilar apenas aquele projeto.
- **`dotnet run`**: Compila e executa o projeto (útil para projetos executáveis como o WebApi).
  - Execute na pasta do projeto `RO.DevTest.WebApi`.
- **`dotnet watch run`**: Executa o projeto WebApi e o reinicia automaticamente ao detectar mudanças nos arquivos. Muito útil durante o desenvolvimento.
  - Execute na pasta do projeto `RO.DevTest.WebApi`.
- **`dotnet test`**: Executa os testes unitários do projeto de teste.
  - Execute na pasta do projeto `RO.DevTest.Tests`.
- **`dotnet clean`**: Limpa os outputs de build (pastas `bin` e `obj`). Útil para resolver problemas de build/cache.
  - Execute na raiz da solução para limpar todos os projetos.
- **Comandos do Entity Framework Core (requer a ferramenta `dotnet-ef` instalada globalmente: `dotnet tool install --global dotnet-ef`)**:
  - `dotnet ef migrations add <NomeDaMigracao> --startup-project <CaminhoProjetoStartup>`: Cria uma nova migração de banco de dados. O `--startup-project` deve apontar para o projeto executável (WebApi ou um projeto separado de EF Core) configurado com o `DbContext`.
  - `dotnet ef database update --startup-project <CaminhoProjetoStartup>`: Aplica as migrações pendentes no banco de dados.

---

### Endpoints da API - Usuários

A API de Usuários está disponível na rota base `/api/user`.

#### Atualizar Usuário (PUT)

Permite atualizar os dados de um usuário existente.

- **Método:** `PUT`
- **Rota:** `/api/user/{id}`
  - `{id}`: O ID (GUID) do usuário a ser atualizado.
- **Corpo da Requisição (JSON):** Deve conter os dados do usuário a serem atualizados. O ID no corpo deve **corresponder** ao ID na rota.

Exemplo de corpo da requisição:

```json
{
  "id": "16cb3d6d-5bab-4d6e-991a-9f84af55e47e",
  "name": "mel",
  "userName": "melzinho",
  "email": "melzinho@gmail.com",
  "role": 1
}
Descrição dos Campos:

id (string, formato GUID): O ID do usuário.
name (string): O nome completo do usuário.
userName (string): O nome de login do usuário.
email (string): O endereço de e-mail do usuário.
role (integer): O papel/perfil do usuário (corresponde aos valores do enum UserRoles).
Respostas Típicas:

200 OK: A atualização foi bem-sucedida. Retorna um UpdateUserResult com sucesso e mensagem.
400 Bad Request: A requisição é inválida (erros de validação, ID da rota não coincide com o corpo, usuário não encontrado, ou erros do sistema de identidade).
500 Internal Server Error: Ocorreu um erro inesperado no servidor.
Você pode testar este endpoint usando a interface do Swagger UI (/swagger) ou ferramentas como Postman ou curl.

Ferramenta para Documentação da Estrutura
Para gerar a representação em texto da estrutura de pastas e arquivos do projeto (como mostrado em algumas descrições), foi utilizado um script simples em PowerShell.

.\gerar_esqueleto.ps1: Este script escaneia as pastas do projeto e gera uma árvore de arquivos e diretórios em formato texto.
Otimização e Refatoração
Sinta-se à vontade para otimizar ou refatorar partes do código que você acredite que possam ser aprimoradas. No entanto, as refatorações devem seguir os padrões já estabelecidos no projeto. Além disso, cada refatoração significativa deve ser separada em um commit próprio, com uma mensagem clara que a identifique (seguindo, por exemplo, o padrão de commits semânticos como refactor:).

Criação de um Frontend (Diferencial Opcional)
Como um diferencial opcional, você pode criar uma interface de frontend que se comunique com a Web API desenvolvida. Se optar por criar o frontend, ele deverá se comunicar com a API via requisições HTTP, e o código desse frontend deve residir no mesmo repositório da Web API (preferencialmente em uma pasta separada na raiz, ex: /frontend).

Lembre-se de que a submissão do teste envolve enviar o link deste repositório público no GitHub, com o histórico de commits organizados seguindo o GitFlow básico e commits semânticos.
```
