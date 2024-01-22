using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Datos;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {

            return Ok(VillaStore.villaList);
            //return new List<VillaDto>
            //{
            //    //new VillaDto{Id=1, Nombre="Vista a la piscina"},
            //    //new VillaDto{Id=2, Nombre="Vista a la Playa"}
            //};
        }

        [HttpGet("id:int")]

        public VillaDto GetVilla(int id)
        {
            return VillaStore.villaList.FirstOrDefault(v=> v.Id == id);
        }
    }
}
