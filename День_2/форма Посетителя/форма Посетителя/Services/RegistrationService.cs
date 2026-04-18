using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using форма_Посетителя.Helpers;
using форма_Посетителя.Models;

namespace форма_Посетителя.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext _context;
        private readonly string? _connectionString;

        public RegistrationService(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetConnectionString();
        }

        /// <summary>
        /// Способ 1: Прямой SQL запрос
        /// </summary>
        public async Task<(bool Success, string Message, int VisitorId)> RegisterWithSQL(RegisterModel model)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return (false, "Ошибка подключения к базе данных", 0);
            }

            try
            {
                string hashedPassword = PasswordHasher.HashPasswordMD5(model.Password);

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"
                        INSERT INTO ""Посетитель"" 
                        (""Фамилия"", ""Имя"", ""Отчество"", ""Телефон"", ""Email"", ""ДатаРождения"", 
                         ""Примечание"", ""СерияПаспорта"", ""НомерПаспорта"", ""Организация"", ""Пароль"", ""Логин"")
                        VALUES (@LastName, @FirstName, @MiddleName, @Phone, @Email, @BirthDate,
                                @Note, @PassportSeries, @PassportNumber, @Organization, @Password, @Login)
                        RETURNING ""Id""";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@LastName", model.LastName);
                        cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                        cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(model.MiddleName) ? DBNull.Value : (object)model.MiddleName);
                        cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(model.Phone) ? DBNull.Value : (object)model.Phone);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@BirthDate", model.BirthDate);
                        cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(model.Note) ? DBNull.Value : (object)model.Note);
                        cmd.Parameters.AddWithValue("@PassportSeries", model.PassportSeries);
                        cmd.Parameters.AddWithValue("@PassportNumber", model.PassportNumber);
                        cmd.Parameters.AddWithValue("@Organization", string.IsNullOrEmpty(model.Organization) ? DBNull.Value : (object)model.Organization);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Login", model.Email); // Используем email как логин

                        var result = await cmd.ExecuteScalarAsync();
                        int visitorId = Convert.ToInt32(result);

                        return (true, "Регистрация успешно завершена!", visitorId);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка регистрации: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Способ 2: Хранимая процедура
        /// </summary>
        public async Task<(bool Success, string Message, int VisitorId)> RegisterWithStoredProcedure(RegisterModel model)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return (false, "Ошибка подключения к базе данных", 0);
            }

            try
            {
                string hashedPassword = PasswordHasher.HashPasswordMD5(model.Password);

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string createProcedureSql = @"
                        CREATE OR REPLACE FUNCTION register_visitor(
                            p_lastname VARCHAR,
                            p_firstname VARCHAR,
                            p_middlename VARCHAR,
                            p_phone VARCHAR,
                            p_email VARCHAR,
                            p_birthdate DATE,
                            p_note TEXT,
                            p_passport_series VARCHAR,
                            p_passport_number VARCHAR,
                            p_organization VARCHAR,
                            p_password VARCHAR,
                            p_login VARCHAR
                        )
                        RETURNS INTEGER AS $$
                        DECLARE
                            v_visitor_id INTEGER;
                        BEGIN
                            INSERT INTO ""Посетитель"" 
                            (""Фамилия"", ""Имя"", ""Отчество"", ""Телефон"", ""Email"", ""ДатаРождения"", 
                             ""Примечание"", ""СерияПаспорта"", ""НомерПаспорта"", ""Организация"", ""Пароль"", ""Логин"")
                            VALUES (p_lastname, p_firstname, p_middlename, p_phone, p_email, p_birthdate,
                                    p_note, p_passport_series, p_passport_number, p_organization, p_password, p_login)
                            RETURNING ""Id"" INTO v_visitor_id;
                            
                            RETURN v_visitor_id;
                        END;
                        $$ LANGUAGE plpgsql;";

                    using (var cmdCreate = new NpgsqlCommand(createProcedureSql, conn))
                    {
                        await cmdCreate.ExecuteNonQueryAsync();
                    }

                    string callProcedureSql = "SELECT register_visitor(@LastName, @FirstName, @MiddleName, @Phone, @Email, @BirthDate, @Note, @PassportSeries, @PassportNumber, @Organization, @Password, @Login)";

                    using (var cmd = new NpgsqlCommand(callProcedureSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@LastName", model.LastName);
                        cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                        cmd.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(model.MiddleName) ? DBNull.Value : (object)model.MiddleName);
                        cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(model.Phone) ? DBNull.Value : (object)model.Phone);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@BirthDate", model.BirthDate);
                        cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(model.Note) ? DBNull.Value : (object)model.Note);
                        cmd.Parameters.AddWithValue("@PassportSeries", model.PassportSeries);
                        cmd.Parameters.AddWithValue("@PassportNumber", model.PassportNumber);
                        cmd.Parameters.AddWithValue("@Organization", string.IsNullOrEmpty(model.Organization) ? DBNull.Value : (object)model.Organization);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Login", model.Email);

                        var result = await cmd.ExecuteScalarAsync();
                        int visitorId = Convert.ToInt32(result);

                        return (true, "Регистрация успешно завершена (через хранимую процедуру)!", visitorId);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка регистрации: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Способ 3: Entity Framework Core
        /// </summary>
        public async Task<(bool Success, string Message, int VisitorId)> RegisterWithEF(RegisterModel model)
        {
            try
            {
                // Проверка, существует ли пользователь с таким email
                var existingVisitor = await _context.Посетительs
                    .FirstOrDefaultAsync(v => v.Email == model.Email);

                if (existingVisitor != null)
                {
                    return (false, "Пользователь с таким email уже существует!", 0);
                }

                string hashedPassword = PasswordHasher.HashPasswordMD5(model.Password);

                // Создаём посетителя (Логин сгенерируется автоматически триггером)
                var visitor = new Посетитель
                {
                    Фамилия = model.LastName,
                    Имя = model.FirstName,
                    Отчество = model.MiddleName ?? "",
                    Телефон = model.Phone ?? "",
                    Email = model.Email,
                    ДатаРождения = DateOnly.FromDateTime(model.BirthDate),
                    Примечание = model.Note ?? "",
                    СерияПаспорта = model.PassportSeries,
                    НомерПаспорта = model.PassportNumber,
                    Организация = model.Organization ?? "",
                    Пароль = hashedPassword
                    // Логин не заполняем, триггер сгенерирует его автоматически
                };

                _context.Посетительs.Add(visitor);
                await _context.SaveChangesAsync();

                // После сохранения получаем сгенерированный логин из БД
                string generatedLogin = visitor.Логин ?? "не удалось получить логин";

                // Эмулируем отправку сообщения пользователю о созданном логине
                await EmulateSendLoginNotification(model.Email, generatedLogin, visitor.Id);

                return (true, $"Регистрация успешно завершена! Ваш логин: {generatedLogin}", visitor.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка регистрации: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Эмуляция отправки сообщения пользователю о созданном логине
        /// </summary>
        private async Task EmulateSendLoginNotification(string email, string login, int visitorId)
        {
            string message = $@"
            ═══════════════════════════════════════════════════════════════
            📧 КОМУ: {email}
            📋 ТЕМА: Регистрация в системе ХранительПРО

            Уважаемый пользователь!

            Вы успешно зарегистрированы в системе контроля пропускного режима
            'ХранительПРО'.
        
            🔑 ВАШИ ДАННЫЕ ДЛЯ ВХОДА:
               • Логин: { login}
               • ID: { visitorId}
               • Email: { email}

            📅 Дата регистрации: { DateTime.Now:dd.MM.yyyy HH:mm: ss}

                        С уважением,
                        Команда ХранительПРО
        ═══════════════════════════════════════════════════════════════";
        
            // Запись в файл лога
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "registration_log.txt");
            await File.AppendAllTextAsync(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{message}\n");

            // Вывод в отладку (окно Output в Visual Studio)
            System.Diagnostics.Debug.WriteLine(message);

            // Показываем пользователю сообщение с логином
            MessageBox.Show($"Регистрация успешно завершена!\n\nВаш логин для входа: {login}\n\nИнформация отправлена на почту {email}",
                "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
