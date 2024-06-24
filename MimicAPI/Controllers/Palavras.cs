using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Data;
using MimicAPI.Models;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")] // responsavel por definir o caminho pra localizar todas as acoes do controlador palavras
    public class Palavras : ControllerBase
    {
        private readonly Context _banco;

        public Palavras(Context banco)
        {
            _banco = banco;
        }

        [Route("")] // deixar vazio porque assim que entrar no endereço: site.../api/palavras já vai obter todas as palavras sem precisar entrar no metodo
        [HttpGet]
        public ActionResult ObterTodasPalavras(DateTime? Data)
        {
            var item = _banco.Palavras.AsQueryable(); //converte para query ao inves de usar dbset para realizar os filtros por data
            //verifica se contem valor
            if (Data.HasValue)
            {
                item = item.Where(a => a.Criado > Data.Value);
            }
            return Ok(item);
        }

        [Route("{id}")] // o id é para obter uma palavra especifica, para acessar o obter o usuario o endereço sera: site.../api/palavras/1
        [HttpGet]
        public ActionResult ObterPalavra(int id)
        {
            var obj = _banco.Palavras.Find(id);
            
            //verifica retorno do obj de busca
            if (obj == null)
                return NotFound();
                //ou
                //return StatusCode(404);
            
            return Ok(_banco.Palavras.Find(id));
        }

        [Route("")] //site.../api/palavras(POST: id, nome......)
        [HttpPost]
        public ActionResult Cadastrar([FromBody]palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
            return Created($"/api/palavras/{palavra.Id}",palavra); // apos criar ele retorna encaminhando para consulta junto com o id criado
        }

        [Route("{id}")] // site.../api/palavras/1 (POST: id, nome......)
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]palavra palavra)
        {
            // AsNoTracking().FirstOrDefault evita o erro entityframework para obter o id com mesmo valor
            // e por ter mais de um objeto dentro da consulta com o mesmo id
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a=>a.Id == id);

            //verifica retorno do obj de busca
            if (obj == null)
                return NotFound();

            palavra.Id = id; //caso não tenha entregado o id, força
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            return Ok();
        }

        [Route("{id}")] // site.../api/palavras/1 (DELETE)
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var palavra = _banco.Palavras.Find(id);

            //verifica retorno do obj de busca
            if (palavra == null)
                return NotFound();

            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            return NoContent(); //status ok, porem como é uma exclusão não tem dados para apresentar, por isso retorna Nocontent
        }
    }
}
