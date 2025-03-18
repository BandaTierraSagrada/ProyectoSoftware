using Microsoft.EntityFrameworkCore;
using SalonDeBelleza.src.models;
using SalonDeBelleza.src.repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalonDeBelleza.src.services
{
    public class CitaService
    {
        private readonly CitaRepository _citaRepository;

        public CitaService(CitaRepository citaRepository)
        {
            _citaRepository = citaRepository;
        }

        public bool prueba()
        {
            return false;
        }
    }
}
