using AppControleFinanceiro.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppControleFinanceiro.Libraries.Converters
{
    public class TransactionValueColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Transaction transaction = (Transaction)value; // converte 'value' em Transaction e passa para a variavel transaction
            if (transaction == null) 
                return null;

            if (transaction.Type == TransactionType.Income)
            {
                return Color.FromArgb("#FF939E5A");
            }
            else
            {
                return Colors.Red;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
