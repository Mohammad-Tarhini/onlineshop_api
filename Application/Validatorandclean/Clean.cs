using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public static class Clean
    {
        public static void CleanStrings(this object dto)
        {
            var props = dto.GetType().GetProperties(); // use GetProperties() if properties

            foreach (var prop in props)
            {
                try
                {
                    if (prop.PropertyType == typeof(string) && prop.CanWrite)
                    {
                        var valueObj = prop.GetValue(dto);
                        if (valueObj is string value)
                        {
                            Console.WriteLine($"Cleaning: {prop.Name} = \"{value}\"");
                            var cleaned = value?.Trim()?.ToLower() ?? string.Empty;
                            prop.SetValue(dto, cleaned);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error cleaning property {prop.Name}: {ex.Message}");
                }
            }
        }
    }
}