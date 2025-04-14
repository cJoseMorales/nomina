using System.ComponentModel.DataAnnotations;

namespace nomina.Attributes;

public class DatePeriodAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string id || !IsValidPeriod(id))
        {
            return new ValidationResult("El período especificado no es válido.");
        }

        return ValidationResult.Success;
    }

    private static bool IsValidPeriod(string period)
    {
        if (string.IsNullOrWhiteSpace(period)
            || period.Length < 6
            || !period.Contains('-'))
        {
            return false;
        }

        var parts = period.Split('-');
        
        int year;
        int month;
        try
        {
            year = int.Parse(parts[0]);
            month = int.Parse(parts[1]);
        }
        catch (Exception)
        {
            return false;
        }

        return year <= DateTime.Now.Year
               && month is >= 1 and <= 12;
    }
}