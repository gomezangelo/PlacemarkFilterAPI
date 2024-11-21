using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlacemarkFilterAPI.Models;
using PlacemarkFilterAPI.Services;

namespace PlacemarkFilterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacemarkController : ControllerBase
    {
        private readonly KmlService _kmlService;

        public PlacemarkController(KmlService kmlService)
        {
            _kmlService = kmlService;
        }

        [HttpPost("export")]
        public IActionResult ExportKml([FromBody] FilterCriteriaModel filtroPlacemark)
        {
            if (!IsValidCriteria(filtroPlacemark))
                return BadRequest("Filtros incorretos");

            var placemarks = _kmlService.LoadKml();
            var filteredPlacemarks = _kmlService.FilterPlacemarks(placemarks, filtroPlacemark);
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "novo_arquivo.kml");

            _kmlService.ExportFilteredKml(filteredPlacemarks, outputPath);

            return Ok(new { Message = "Arquivo exportado com sucesso", FilePath = outputPath });
        }

        [HttpGet]
        public IActionResult GetPlacemarks([FromQuery] FilterCriteriaModel filtroPlacemark)
        {
            if (!IsValidCriteria(filtroPlacemark))
                return BadRequest("Filtros incorretos");

            var placemarks = _kmlService.LoadKml();
            var filteredPlacemarks = _kmlService.FilterPlacemarks(placemarks, filtroPlacemark);

            return Ok(filteredPlacemarks);
        }

        [HttpGet("filters")]
        public IActionResult GetAvailableFilters()
        {
            var placemarks = _kmlService.LoadKml();
            var clientes = _kmlService.GetUniqueValues(placemarks, "Cliente");
            var situacoes = _kmlService.GetUniqueValues(placemarks, "Situação");
            var bairros = _kmlService.GetUniqueValues(placemarks, "Bairro");

            return Ok(new { Clientes = clientes, Situacoes = situacoes, Bairros = bairros });
        }

        private bool IsValidCriteria(FilterCriteriaModel filtroPlacemark)
        {
            if (!string.IsNullOrEmpty(filtroPlacemark.Referencia) && filtroPlacemark.Referencia.Length < 3)
                return false;

            if (!string.IsNullOrEmpty(filtroPlacemark.RuaCruzamento) && filtroPlacemark.RuaCruzamento.Length < 3)
                return false;

            return true;
        }
    }
}