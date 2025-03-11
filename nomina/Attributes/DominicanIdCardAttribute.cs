using System.ComponentModel.DataAnnotations;

namespace nomina;

public class DominicanIdCardAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string id || !ValidateDominicanIdCard(id))
        {
            return new ValidationResult("La cédula ingresada no es válida.");
        }
        return ValidationResult.Success;
    }

    private static bool ValidateDominicanIdCard(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        var card = id.Replace("-", "");

        if (card.Length != 11 || !card.All(char.IsDigit))
        {
            return false;
        }

        var checkDigit = card[10] - '0';
        var sum = 0;

        for (var i = 0; i < 10; i++)
        {
            var digit = card[i] - '0';
            var product = i % 2 == 0 ? digit : digit * 2;

            sum += product > 9 ? product - 9 : product;
        }

        var nextTen = (sum / 10 + 1) * 10;
        var verifierNumber = nextTen - sum;

        return verifierNumber == checkDigit || verifierNumber % 10 == 0;
    }
}