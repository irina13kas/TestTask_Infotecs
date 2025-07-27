using Domain.DTOs;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IResultFilterService
    {
        Task<List<ResultEntry>> GetFilteredResultsAsync(ResultFilterDto filter);
    }
}
