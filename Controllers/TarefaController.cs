using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            // TODO: Buscar o Id no banco utilizando o EF
            var tarefa = _context.Tarefas.Find(id);
            // TODO: Validar o tipo de retorno. Se não encontrar a tarefa, retornar NotFound,
            if(tarefa == null)
            {
                return NotFound();
            }
            // caso contrário retornar OK com a tarefa encontrada
            return Ok();
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            try
            {
                var tarefas = _context.Tarefas.ToList();
                
                // Log para verificar se tarefas estão sendo buscadas
                Console.WriteLine($"Tarefas encontradas: {tarefas.Count}");

                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                // Log da exceção
                Console.WriteLine($"Erro ao buscar tarefas: {ex.Message}");
                return StatusCode(500, "Erro interno ao buscar tarefas.");
            }
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            // TODO: Buscar  as tarefas no banco utilizando o EF, que contenha o titulo recebido por parâmetro
            var tarefas = _context.Tarefas
            .Where(t => t.Titulo.Contains(titulo))
            .ToList();
            // Dica: Usar como exemplo o endpoint ObterPorData
            if (tarefas.Count == 0)
            {
                return NotFound($"Nenhuma tarefa encontrada com o título '{titulo}'.");
            }
            return Ok();
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            // Busca as tarefas que têm o status fornecido
            var tarefas = _context.Tarefas
                .Where(t => t.Status == status)
                .ToList();

            // Verifica se foram encontradas tarefas
            if (!tarefas.Any())
            {
                return NotFound($"Nenhuma tarefa encontrada com o status '{status}'.");
            }

            // Retorna a lista de tarefas com status OK
            return Ok(tarefas);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Adiciona a tarefa recebida no contexto do EF
            _context.Tarefas.Add(tarefa);

            // Salva as mudanças no banco de dados
            _context.SaveChanges();

            // Retorna a tarefa criada com o status 201 Created
            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            // Buscar a tarefa no banco
            var tarefaBanco = _context.Tarefas.Find(id);

            // Verificar se a tarefa existe
            if (tarefaBanco == null)
                return NotFound();

            // Verificar se a data da tarefa é válida
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Atualizar as informações da tarefaBanco com a tarefa recebida
            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            // Salvar as mudanças no EF
            _context.SaveChanges();

            // Retornar a tarefa atualizada
            return Ok(tarefaBanco);
        }


        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            // Buscar a tarefa no banco
            var tarefaBanco = _context.Tarefas.Find(id);

            // Verificar se a tarefa existe
            if (tarefaBanco == null)
                return NotFound();

            // Remover a tarefa do contexto
            _context.Tarefas.Remove(tarefaBanco);

            // Salvar as mudanças no EF
            _context.SaveChanges();

            // Retornar 204 No Content
            return NoContent();
        }

    }
}
