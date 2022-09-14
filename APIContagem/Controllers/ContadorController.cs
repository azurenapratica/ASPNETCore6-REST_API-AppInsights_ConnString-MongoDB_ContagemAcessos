using Microsoft.AspNetCore.Mvc;
using APIContagem.Data;
using APIContagem.Models;

namespace APIContagem.Controllers;

[ApiController]
[Route("[controller]")]
public class ContadorController : ControllerBase
{
    private static readonly Contador _CONTADOR = new Contador();
    private readonly ILogger<ContadorController> _logger;
    private readonly IConfiguration _configuration;
    private readonly ContagemRepository _repository;

    public ContadorController(ILogger<ContadorController> logger,
        IConfiguration configuration, ContagemRepository repository)
    {
        _logger = logger;
        _configuration = configuration;
        _repository = repository;
    }

    [HttpGet]
    public ResultadoContador Get()
    {
        _logger.LogInformation("Gerando valor...");
        
        int valorAtualContador;
        lock (_CONTADOR)
        {
            _CONTADOR.Incrementar();
            valorAtualContador = _CONTADOR.ValorAtual;
        }

        var resultado = new ResultadoContador()
        {
            ValorAtual = valorAtualContador,
            Producer = _CONTADOR.Local,
            Kernel = _CONTADOR.Kernel,
            Framework = _CONTADOR.Framework,
            Mensagem = _configuration["MensagemVariavel"]
        };

        _logger.LogInformation("Persistindo documento...");
        _repository.Save(resultado);

        return resultado;
    }
}