using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnroladorStandAlone
{
    public class ValidadorRUT
    {
        public static bool Validar(string rut, out string rut_validado)
        {
            rut_validado = "";
            int verificador = 0;
            int sum = 0;
            int numeros;

            rut = rut.Replace("-", string.Empty).Replace(".", string.Empty);
            if (rut.Length < 2)
            {
                return false;
            }
            string nums = rut.Substring(0, rut.Length - 1);
            string sdv = rut.Substring(rut.Length - 1);


            if (sdv == "k" || sdv == "K")
            {
                verificador = 10;
            }
            else if (sdv == "0")
            {
                verificador = 11;
            }
            else if (!int.TryParse(sdv, out verificador))
            {
                return false;
            }

            if (!int.TryParse(nums, out numeros))
            {
                return false;
            }

            nums = numeros.ToString();

            if (nums.Length > 8)
            {
                return false;
            }

            for (var i = 0; i < nums.Length; i++)
            {
                string digito = nums.Substring(nums.Length - i - 1, 1);
                sum += int.Parse(digito) * ((i) % (7 - 1) + 2);
            }

            if ((11 - (sum % 11)) != verificador)
            {
                return false;
            }

            rut_validado = string.Format("{0}-{1}", nums, sdv.ToUpper());
            return true;
        }
    }
}
