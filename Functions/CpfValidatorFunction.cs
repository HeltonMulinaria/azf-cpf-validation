using CpfValidationApp.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;

namespace CpfValidationApp.Functions
{
    public class CpfValidatorFunction
    {
        private readonly ILogger<CpfValidatorFunction> _logger;
        private readonly IValidator<CpfValidatorRequestDto> _validator;

        public CpfValidatorFunction(ILogger<CpfValidatorFunction> logger, IValidator<CpfValidatorRequestDto> validator)
        {
            _logger = logger;
            _validator = validator;
        }

        [Function("ValidateCpf")]
        [OpenApiOperation(operationId: "ValidateCpf", tags: new[] { "CPF" }, Summary = "Validar CPF", Description = "Valida um CPF brasileiro")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CpfValidatorRequestDto), Required = true, Description = "Dados do CPF a ser validado")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(object), Summary = "CPF v�lido", Description = "Retorna sucesso quando o CPF � v�lido")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object), Summary = "CPF inv�lido", Description = "Retorna erro quando o CPF � inv�lido ou a requisi��o est� malformada")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(object), Summary = "Erro interno", Description = "Retorna erro quando ocorre uma falha no servidor")]
        public async Task<IActionResult> Run(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "cpf/validate")] HttpRequest req)
        {
            _logger.LogInformation("Iniciando valida��o de CPF");

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    return new BadRequestObjectResult(new
                    {
                        success = false,
                        message = "O corpo da requisi��o � inv�lido"
                    });
                }

                var cpfRequest = JsonSerializer.Deserialize<CpfValidatorRequestDto>(requestBody);

                if (cpfRequest == null)
                {
                    return new BadRequestObjectResult(new
                    {
                        success = false,
                        message = "Dados inv�lidos na requisi��o"
                    });
                }

                var validationResult = await _validator.ValidateAsync(cpfRequest);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("CPF inv�lido: {Cpf}", cpfRequest.Cpf);

                    return new BadRequestObjectResult(new
                    {
                        success = false,
                        message = "Valida��o falhou",
                        errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                    });
                }

                _logger.LogInformation("CPF v�lido: {Cpf}", cpfRequest.Cpf);

                return new OkObjectResult(new
                {
                    success = true,
                    message = "CPF v�lido",
                    cpf = cpfRequest.Cpf
                });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Erro ao processar JSON");
                return new BadRequestObjectResult(new
                {
                    success = false,
                    message = "Formato JSON inv�lido"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar CPF");
                return new ObjectResult(new
                {
                    success = false,
                    message = "Erro interno ao processar a requisi��o"
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
