using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Datos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MagicVilla_API.Repositorio.IRepositorio;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        //private readonly ApplicationDbContext _db;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)

        {
            _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]

        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obtener las villas");

                IEnumerable<Villa> villalist = await _villaRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villalist);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
           
            //IEnumerable<Villa> villalist = await _db.Villas.ToListAsync();

            ////return Ok(await _db.Villas.ToListAsync());
            //return new List<VillaDto>
            //{
            //    //new VillaDto{Id=1, Nombre="Vista a la piscina"},
            //    //new VillaDto{Id=2, Nombre="Vista a la Playa"}
            //};
        }

        [HttpGet("id:int", Name = "GetVilla")]

        [ProducesResponseType(StatusCodes.Status200OK)] // Esto es para documentar los códigos de estado que se van a utilizar
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task <ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Villa con Id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }


                ////var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

                var villa = await _villaRepo.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<VillaDto>(villa);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe!");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                ////villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
                ////VillaStore.villaList.Add(villaDto);
                //return Ok(villaDto);

                ////Villa modelo = new() // En el post no se envía el id, ya que es autoincrementable
                ////{
                ////    Nombre = villaDto.Nombre,
                ////    Detalle = villaDto.Detalle,
                ////    ImagenUrl = villaDto.ImagenUrl,
                ////    Ocupantes = villaDto.Ocupantes,
                ////    Tarifa = villaDto.Tarifa,
                ////    MetrosCuadrados = villaDto.MetrosCuadrados,
                ////    Amenidad = villaDto.Amenidad
                ////};

                Villa modelo = _mapper.Map<Villa>(createDto);

                await _villaRepo.Crear(modelo);

                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpDelete("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _villaRepo.Obtener(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            _villaRepo.Remover(villa);  

            return NoContent(); // Cuando hay un delete siempre se retorna un NoContent()
        }

        [HttpPut("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if(updateDto == null || id!= updateDto.Id)
            {
                return BadRequest();
            }

            ////var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            ////villa.Nombre = villaDto.Nombre;
            ////villa.Ocupantes = villaDto.Ocupantes;
            ////villa.MetrosCuadrados = villaDto.MetrosCuadrados;
            
            Villa modelo = _mapper.Map<Villa>(updateDto);

            ////Villa modelo = new() 
            ////{
            ////    Id = villaDto.Id,
            ////    Nombre = villaDto.Nombre,
            ////    Detalle = villaDto.Detalle,
            ////    ImagenUrl = villaDto.ImagenUrl,
            ////    Ocupantes = villaDto.Ocupantes,
            ////    Tarifa = villaDto.Tarifa,
            ////    MetrosCuadrados = villaDto.MetrosCuadrados,
            ////    Amenidad = villaDto.Amenidad
            ////};

            _villaRepo.Actualizar(modelo);

            return NoContent();
        }


        [HttpPatch("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task <IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto>patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            ////var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked:false);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            ////VillaUpdateDto villaDto = new()
            ////{
            ////    Id = villa.Id,
            ////    Nombre = villa.Nombre,
            ////    Detalle = villa.Detalle,
            ////    ImagenUrl = villa.ImagenUrl,
            ////    Ocupantes = villa.Ocupantes,
            ////    Tarifa = villa.Tarifa,
            ////    MetrosCuadrados = villa.MetrosCuadrados,
            ////    Amenidad = villa.Amenidad
            ////};

            if(villa == null) return BadRequest();

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);

            ////Villa modelo = new()
            ////{
            ////    Id = villaDto.Id,
            ////    Nombre = villaDto.Nombre,
            ////    Detalle = villaDto.Detalle,
            ////    ImagenUrl = villaDto.ImagenUrl,
            ////    Ocupantes = villaDto.Ocupantes,
            ////    Tarifa = villaDto.Tarifa,
            ////    MetrosCuadrados = villaDto.MetrosCuadrados,
            ////    Amenidad = villaDto.Amenidad
            ////};

            _villaRepo.Actualizar(modelo);

            return NoContent();
        }

    }
}
