using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LegendApi.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class LegendsController : ControllerBase
    {
        private IMemoryCache _cache;
        public LegendsController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Апи чтения файла.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get(string fileName)
        {
            string text = "";

            DateTime? readed = null;

            string path = String.Format(@"C:\Users\daure\source\repos\LegendApi\LegendApi\Files\{0}.txt",
                         fileName);

            try
            {
                text = System.IO.File.ReadAllText(path);
                
                // Проверка: существует ли кеш с таким ключом. Если да, то вовзращает ответ, что ресурс занят.
                if (_cache.TryGetValue(path, out readed))
                    return Ok("Busy!");

                // Если проверка не проходит, то создает кеш и вовзращает текст файла.
                else
                {
                    readed = DateTime.Now;
                    await Task.Run(() => _cache.Set(path, readed, new MemoryCacheEntryOptions
                    {
                        // Здесь указывается срок действия кеша - 2 секунды.
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2)
                    }));
                }
            }

            catch
            {
                // Если такой файл не существует, то возвращает 400 ошибку с сообщением.
                return BadRequest("Wrong file name!");
            }

            return Ok(text);
        }
    }
}
