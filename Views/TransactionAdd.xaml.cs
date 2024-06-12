using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using System.Text;

namespace AppControleFinanceiro.Views;

public partial class TransactionAdd : ContentPage
{
    private ITransactionRepository _repository;
	public TransactionAdd(ITransactionRepository repository)
    {
        InitializeComponent();
        _repository = repository;
    }

    private void TapGestureRecognizerTapped_To_Close(object sender, TappedEventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private void OnButtonClicked_Save(object sender, EventArgs e)
    {
        if (IsValidData() == false)
        {
            return;
        }

        SaveTransactionInDatabase();
        Navigation.PopModalAsync();

      //  #IF ANDROID
            

      //  #ENDIF

        WeakReferenceMessenger.Default.Send<string>(string.Empty);

        var count = _repository.GetAll().Count;
    }

    private void SaveTransactionInDatabase()
    {
        Transaction transaction = new Transaction()
        {
            Name = edName.Text,
            Type = edReceita.IsChecked ? TransactionType.Income : TransactionType.Expenses, // Operador ternário, fa zum "IF" seco
            Date = edDate.Date,
            Value = double.Parse(edValue.Text)
        };

        //-- As duas linhas de código abaixo funcionam e são aceitáveis para desenvolvimento. Porém, foi decidido fazer 
        //  diferente, onde será feito a injeção de dependência de forma mais complexa e completa... Nela, nós jogamos 
        //  todas as Views dentro da injeção de dependência. Agora é chamado pela linha a seguir...

        //var repository = this.Handler.MauiContext.Services.GetService<ITransactionRepository>();
        //repository.Add(transaction);

        _repository.Add(transaction);
    }

    private bool IsValidData()
    {
        bool valid = true;
        StringBuilder sb = new StringBuilder();
        double result;

        if (string.IsNullOrEmpty(edName.Text) || string.IsNullOrWhiteSpace(edName.Text))
        {
            sb.AppendLine("O campo 'Nome' deve ser preenchido!");
            valid = false;
        }

        if (string.IsNullOrEmpty(edValue.Text) || string.IsNullOrWhiteSpace(edValue.Text))
        {
            sb.AppendLine("O campo 'Valor' deve ser preenchido!");
            valid = false;
        }

        if (!string.IsNullOrEmpty(edValue.Text) && !double.TryParse(edValue.Text, out result))
        {
            sb.AppendLine("O campo 'Valor' é inválido!");
            valid = false;
        }

        if (!string.IsNullOrEmpty(edValue.Text) && (double.Parse(edValue.Text) <= 0))
        {
            sb.AppendLine("O campo 'Valor' deve ser maior que 0 (zero)!");
            valid = false;
        }

        if (valid == false)
        {
            App.Current.MainPage.DisplayAlert("Mensagem!", $"{sb.ToString()}", "OK");
        }

        return valid;
    }

}