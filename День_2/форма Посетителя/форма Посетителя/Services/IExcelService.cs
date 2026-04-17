using System;
using System.Collections.Generic;
using System.Text;
using форма_Посетителя.Models;

namespace форма_Посетителя.Services
{
    public interface IExcelService
    {
        byte[] GenerateTemplate();
        Task<(bool Success, List<GroupVisitorModel> Visitors, string ErrorMessage)> ImportVisitorsFromExcel(string filePath);
    }
}
