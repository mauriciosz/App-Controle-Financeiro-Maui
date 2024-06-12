using AppControleFinanceiro.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppControleFinanceiro.Libraries.Converters
{
    public class TransactionValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Transaction transaction = (Transaction)value; // converte 'value' em Transaction e passa para a variavel transaction
            if (transaction == null) 
                return null;

            if (transaction.Type == TransactionType.Income)
            {
                return transaction.Value.ToString("C"); // retorna o valor formatado para moeda corrente (exemplo: R$250,00)
            }
            else
            {
                return $"- {transaction.Value.ToString("C")}"; // faz o mesmo de cima, mas concatena com um menos (exemplo: - R$250,00)
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
