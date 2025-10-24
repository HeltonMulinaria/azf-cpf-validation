#  Azure Functions - Validação de CPF

API serverless desenvolvida com Azure Functions para validação de CPF brasileiro, utilizando .NET 8 e FluentValidation.

##  Sobre o Projeto

Este projeto implementa uma Azure Function HTTP-triggered que valida números de CPF brasileiros. A API oferece validaçãoo robusta com documentaçãoo OpenAPI/Swagger integrada, logging  e validação de dados usando FluentValidation.

##  Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **C# 12.0** - Linguagem de programação
- **Azure Functions Worker** - Runtime serverless
- **FluentValidation** - Validação de dados
- **OpenAPI/Swagger** - Documentação da API


##  Ferramentas

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) (opcional, para deploy)

## Configuração e Instalação

1. **Clone o repositório**
```bash
   git clone https://github.com/HeltonMulinaria/azf-cpf-validation.git
   cd azf-cpf-validation
```

2. **Restaure as dependências**
```bash
   dotnet restore
```

3. **Execute localmente**
```bash
   dotnet build
   func start
```

Ou no Visual Studio: pressione `F5` para iniciar o debug.

##  Endpoints da API

### POST /api/cpf/validate

Valida um número de CPF brasileiro.

**Request Body:**
```json
{
  "cpf": "10221877088"
}
```

**Respostas:**

 **200 OK** - CPF válido
```json
{
  "success": true,
  "message": "CPF válido",
  "cpf": "10221877088"
}
```

**400 Bad Request** - CPF inválido ou requisição inválida
```json
{
  "success": false,
  "message": "Validação falhou",
  "errors": [
    {
      "field": "Cpf",
      "message": "CPF inválido"
    }
  ]
}
```

 **400 Bad Request** - JSON inválido
```json
{
  "success": false,
  "message": "Formato JSON inválido"
}
```

 **500 Internal Server Error** - Erro interno do servidor
```json
{
  "success": false,
  "message": "Erro interno ao processar a requisição"
}
```

##  Exemplos de Uso

### cURL
```bash
curl -X POST http://localhost:7071/api/cpf/validate \
  -H "Content-Type: application/json" \
  -d '{"cpf":"10221877088"}'
```


##  Documentação OpenAPI/Swagger

Quando a aplicação estiver em execução, acesse:

- **Swagger UI**: `http://localhost:7071/api/swagger/ui`
- **OpenAPI JSON**: `http://localhost:7071/api/openapi/v3.json`

##  Validações Implementadas

A validação de CPF inclui:

-  Verificação de formato (11 dígitos)
-  Validação de dígitos verificadores
-  Rejeição de CPFs com todos os dígitos iguais (ex: 111.111.111-11)
-  Validação de campo obrigatório
-  Case-insensitive JSON deserialization

## Deploy no Azure

1. **Criar Function App no Azure**
```bash
   az functionapp create \
  --resource-group <resource-group> \
     --consumption-plan-location <location> \
     --runtime dotnet-isolated \
     --functions-version 4 \
     --name <function-app-name> \
     --storage-account <storage-account>
```

2. **Publicar a aplicação**
```bash
   func azure functionapp publish <function-app-name>
```

Ou pelo Visual Studio: **Botão direito no projeto clique em  Publish**


```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
  }
}
```

##  Licença

Este projeto está sob a licençaa MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

##  Autor

**Helton Mulinaria**

- GitHub: [@HeltonMulinaria](https://github.com/HeltonMulinaria)
- Repositório: [azf-cpf-validation](https://github.com/HeltonMulinaria/azf-cpf-validation)
