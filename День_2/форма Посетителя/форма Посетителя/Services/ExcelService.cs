using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using форма_Посетителя.Models;

namespace форма_Посетителя.Services
{
    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            // Для EPPlus 8+
            ExcelPackage.License.SetNonCommercialPersonal("Student");
        }

        /// <summary>
        /// Генерация шаблона Excel для списка посетителей
        /// </summary>
        public byte[] GenerateTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Посетители");

                // Заголовки
                worksheet.Cells[1, 1].Value = "№ п/п";
                worksheet.Cells[1, 2].Value = "Фамилия";
                worksheet.Cells[1, 3].Value = "Имя";
                worksheet.Cells[1, 4].Value = "Отчество";
                worksheet.Cells[1, 5].Value = "Телефон";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Примечание";

                // Стиль заголовков
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Пример данных
                worksheet.Cells[2, 1].Value = "1";
                worksheet.Cells[2, 2].Value = "Иванов";
                worksheet.Cells[2, 3].Value = "Иван";
                worksheet.Cells[2, 4].Value = "Иванович";
                worksheet.Cells[2, 5].Value = "+7 (999) 123-45-67";
                worksheet.Cells[2, 6].Value = "ivan@example.com";
                worksheet.Cells[2, 7].Value = "Руководитель отдела";

                worksheet.Cells[3, 1].Value = "2";
                worksheet.Cells[3, 2].Value = "Петрова";
                worksheet.Cells[3, 3].Value = "Мария";
                worksheet.Cells[3, 4].Value = "Сергеевна";
                worksheet.Cells[3, 5].Value = "+7 (999) 234-56-78";
                worksheet.Cells[3, 6].Value = "maria@example.com";
                worksheet.Cells[3, 7].Value = "Главный специалист";

                worksheet.Cells.AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        /// <summary>
        /// Импорт списка посетителей из Excel
        /// </summary>
        public async Task<(bool Success, List<GroupVisitorModel> Visitors, string ErrorMessage)> ImportVisitorsFromExcel(string filePath)
        {
            return await Task.Run(() =>
            {
                var visitors = new List<GroupVisitorModel>();

                try
                {
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        if (worksheet == null)
                        {
                            return (false, new List<GroupVisitorModel>(), "Файл не содержит листов");
                        }

                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var rowNumber = worksheet.Cells[row, 1]?.Value?.ToString();
                            var lastName = worksheet.Cells[row, 2]?.Value?.ToString();
                            var firstName = worksheet.Cells[row, 3]?.Value?.ToString();
                            var middleName = worksheet.Cells[row, 4]?.Value?.ToString();
                            var phone = worksheet.Cells[row, 5]?.Value?.ToString();
                            var email = worksheet.Cells[row, 6]?.Value?.ToString();
                            var note = worksheet.Cells[row, 7]?.Value?.ToString();

                            if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(firstName))
                                continue;

                            var fullName = $"{lastName} {firstName} {middleName}".Trim();
                            var hasError = false;
                            var errorMessage = "";

                            if (string.IsNullOrWhiteSpace(lastName))
                            {
                                hasError = true;
                                errorMessage += "Фамилия обязательна; ";
                            }
                            if (string.IsNullOrWhiteSpace(firstName))
                            {
                                hasError = true;
                                errorMessage += "Имя обязательно; ";
                            }
                            if (string.IsNullOrWhiteSpace(email))
                            {
                                hasError = true;
                                errorMessage += "Email обязателен; ";
                            }
                            else if (!IsValidEmail(email))
                            {
                                hasError = true;
                                errorMessage += "Неверный формат email; ";
                            }
                            if (!string.IsNullOrWhiteSpace(phone) && !IsValidPhone(phone))
                            {
                                hasError = true;
                                errorMessage += "Неверный формат телефона; ";
                            }

                            visitors.Add(new GroupVisitorModel
                            {
                                RowNumber = row - 1,
                                FullName = fullName ?? "",
                                Phone = phone ?? "",
                                Email = email ?? "",
                                HasError = hasError,
                                ErrorMessage = errorMessage
                            });
                        }

                        if (visitors.Count == 0)
                        {
                            return (false, new List<GroupVisitorModel>(), "Файл не содержит данных о посетителях");
                        }

                        var hasErrors = visitors.Any(v => v.HasError);
                        if (hasErrors)
                        {
                            var errorList = string.Join("\n", visitors.Where(v => v.HasError).Select(v => $"Строка {v.RowNumber}: {v.ErrorMessage}"));
                            return (false, visitors, $"Обнаружены ошибки в данных:\n{errorList}");
                        }

                        return (true, visitors, "Данные успешно загружены");
                    }
                }
                catch (Exception ex)
                {
                    return (false, new List<GroupVisitorModel>(), $"Ошибка при чтении файла: {ex.Message}");
                }
            });
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            var regex = new Regex(@"^\+7\s?\(?[0-9]{3}\)?\s?[0-9]{3}-?[0-9]{2}-?[0-9]{2}$");
            return regex.IsMatch(phone);
        }
    }
}
