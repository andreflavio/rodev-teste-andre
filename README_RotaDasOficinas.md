
# 🚗 Rota das Oficinas – Teste Técnico

Bem-vindo ao projeto **Rota das Oficinas**, uma **Web API construída com .NET 8**, que simula um e-commerce simples com funcionalidades de **cadastro, leitura, atualização e exclusão de usuários** (CRUD).

---

## 📌 Visão Geral

Este projeto tem como objetivo demonstrar boas práticas no desenvolvimento de uma API RESTful, utilizando:

- 🧱 **Padrão CQRS (Command Query Responsibility Segregation)**
- 📦 **Padrão Repository**
- 🗃️ **Integração com PostgreSQL via Entity Framework Core**
- ✅ **Testes automatizados**
- 🧾 **Documentação interativa com Swagger**

---

## 🛠️ Tecnologias Utilizadas

- .NET 8  
- Entity Framework Core  
- PostgreSQL  
- Swagger  
- Xunit, Bogus, FluentAssertions

---

## 🎯 Objetivo

O desafio propõe implementar os `TODOs` indicados no projeto.  
A arquitetura está preparada para que você siga o mesmo padrão e mantenha a organização.

---

## ⚙️ Como Executar

1. **Clone o repositório**
   ```bash
   git clone <URL_DO_REPOSITORIO>
   cd RO.DevTest
   ```

2. **Configure o banco**
   Atualize a string de conexão no `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=rotadasoficinas;Username=postgres;Password=123456"
   }
   ```

3. **Execute as migrações**
   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef database update --startup-project RO.DevTest.WebApi
   ```

4. **Inicie a aplicação**
   ```bash
   cd RO.DevTest.WebApi
   dotnet run
   ```

5. **Acesse o Swagger**
   👉 [`http://localhost:5087/swagger/index.html`](http://localhost:5087/swagger/index.html)

6. **(Opcional) Execute os testes**
   ```bash
   cd RO.DevTest.Tests
   dotnet test
   ```

---

## 🧪 Testando a API com Swagger

A API está **documentada e interativa via Swagger**, permitindo:

- ✅ Testar todos os endpoints (CRUD) diretamente na interface
- ✍️ Inserir dados por query string ou JSON
- 📤 Enviar requisições e ver respostas detalhadas
- 🔄 Evitar uso de ferramentas externas como Postman/curl

> 💡 Basta acessar: [`http://localhost:5087/swagger`](http://localhost:5087/swagger)

---

## 🔌 Endpoints da API

> **Base URL:** `http://localhost:5087/api/user`

### ➕ Criar Usuário (POST)
Cria um novo usuário.

- **Endpoint:** `POST /api/user`  
- **Parâmetros (query string):**  
  - `UserName`: Nome de usuário único (string)  
  - `Name`: Nome completo (string)  
  - `Email`: Email válido (string)  
  - `Password`: Mínimo de 8 caracteres com maiúsculas, minúsculas, números e caracteres especiais  
  - `PasswordConfirmation`: Deve ser igual a `Password`  
  - `Role`: Inteiro (0 = padrão, 1 = admin)

- **Exemplo com `curl`:**
```bash
curl -X 'POST'   'http://localhost:5087/api/user?UserName=TestUser1&Name=JohnDoe&Email=johndoe%40example.com&Password=SecurePass1%40&PasswordConfirmation=SecurePass1%40&Role=0'   -H 'accept: text/plain' -d ''
```

- **Resposta (201 Created):**
```json
{
  "id": "d60443ed-f183-43c8-96d3-829f10e21180",
  "userName": "TestUser1",
  "name": "JohnDoe",
  "email": "johndoe@example.com"
}
```

---

### ✏️ Atualizar Usuário (PUT)
Atualiza um usuário existente.

- **Endpoint:** `PUT /api/user/{id}`  
- **Parâmetros:**
  - `id`: UUID do usuário (na URL)

- **Corpo da Requisição (JSON):**
```json
{
  "id": "896b60c6-6749-40bf-be9d-ff5725c86ace",
  "name": "Mel2",
  "userName": "melzinho",
  "email": "melzinho@gmail.com",
  "role": 1,
  "command": "update_user"
}
```

- **Exemplo com `curl`:**
```bash
curl -X 'PUT'   'http://localhost:5087/api/user/896b60c6-6749-40bf-be9d-ff5725c86ace'   -H 'accept: text/plain'   -H 'Content-Type: application/json'   -d '{
    "id": "896b60c6-6749-40bf-be9d-ff5725c86ace",
    "name": "Mel2",
    "userName": "melzinho",
    "email": "melzinho@gmail.com",
    "role": 1,
    "command": "update_user"
  }'
```

- **Resposta (200 OK):**
```json
{
  "success": true,
  "message": "Usuário com ID 896b60c6-6749-40bf-be9d-ff5725c86ace atualizado com sucesso."
}
```

> ⚠️ O campo `command` com valor `"update_user"` é necessário para seguir o padrão de comandos CQRS.


---

## 📬 Submissão

- Suba o projeto no GitHub  
- Utilize Git Flow simples (`main`, `develop`)  
- Envie o link do repositório para avaliação

---

## 🤝 Suporte

Dúvidas?  
- Acesse a documentação via Swagger  
- Entre em contato com o responsável técnico do teste
