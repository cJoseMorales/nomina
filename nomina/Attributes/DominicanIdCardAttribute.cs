using System.ComponentModel.DataAnnotations;

namespace nomina.Attributes;

public class DominicanIdCardAttribute : ValidationAttribute
{
    private static readonly int[] DigitMult = [1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1];

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
        var vnTotal = 0;
        var vcCedula = id.Replace("-", "");
        var pLongCed = vcCedula.Trim().Length;

        if (pLongCed is < 11 or > 11)
        {
            return false;
        }

        for (var vDig = 1; vDig <= pLongCed; vDig++)
        {
            var vCalculo = int.Parse(vcCedula.Substring(vDig - 1, 1)) * DigitMult[vDig - 1];
            if (vCalculo < 10)
            {
                vnTotal += vCalculo;
            }
            else
            {
                vnTotal += int.Parse(vCalculo.ToString()[..1]) +
                           int.Parse(vCalculo.ToString().Substring(1, 1));
            }
        }

        return vnTotal % 10 == 0;
    }
}