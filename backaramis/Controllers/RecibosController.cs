﻿using AutoMapper;
using backaramis.Helpers;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Recibos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backaramis.Controllers
{
    [Route("[controller]")]
    [ApiController]
 
    public class RecibosController : ControllerBase
    {
        private readonly IRecibosService _recibosService;
        private readonly IGenericService<Point> _genericPoint;
        private readonly ILoggService _loggService;
        private readonly IMapper _mapper;
        private readonly SecurityService _securityService;
        private readonly string _userName;

        public RecibosController(
            IRecibosService recibosService,
            IGenericService<Point> genericPoint,
            ILoggService loggService,
            IMapper mapper,
            SecurityService securityService
            )
        {
            _recibosService= recibosService;
            _genericPoint= genericPoint;
            _loggService= loggService;  
            _mapper= mapper;
            _securityService= securityService;
            _userName = _securityService.GetUserAuthenticated();
        }

        [HttpPost("InsertRecibo")]
        public IActionResult InsertRecibo([FromBody] ReciboInsertDto model)
        {
           
            model.Operador = _userName;
            model.Fecha = DateTime.Now;
           
            try
            {
               var data = _recibosService.Insert(model);
                _loggService.Log($"InsertRecibo {model.Cliente}", "Recibos", "Insert", _userName);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _loggService.Log($"ErrorInsertRecibo {model.Cliente}", "Recibos", "Insert", _userName);
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("MP")]
        public IActionResult MP(PaymentIntentDto paymentIntentDto)
        {

            try
            {
                var data = _recibosService.CreatePaymentIntent(paymentIntentDto); 
                return Ok(data);
            }
            catch (Exception ex)
            {
                
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetPoints")]
        public IActionResult GetPoints()
        {
            List<Point>? data = _genericPoint.Get();
            return Ok(data);
        }

    }
}