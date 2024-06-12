using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using System.Text;

namespace AppControleFinanceiro.Views;

public partial class TransactionEdit : ContentPage
{
    private ITransactionRepository _repository;
    private Transaction _transaction;
	public TransactionEdit(ITransactionRepository repository)
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
        WeakReferenceMessenger.Default.Send<string>(string.Empty);
    }

    public void SetTransactionToEdit(Transaction transaction)
	{ 
		_transaction = transaction;

		edName.Text = _transaction.Name; 
		if(_transaction.Type == TransactionType.Income)
			edReceita.IsChecked = true;
		else 
			edDespesa.IsChecked = true;
		edDate.Date = _transaction.Date.Date;
		edValue.Text = _transaction.Value.ToString("");		
	}
    private void SaveTransactionInDatabase()
    {
        Transaction transaction = new Transaction()
        {
            Id = _transaction.Id,
            Name = edName.Text,
            Type = edReceita.IsChecked ? TransactionType.Income : TransactionType.Expenses, // Operador ternário, faz um "IF" seco
            Date = edDate.Date,
            Value = double.Parse(edValue.Text)
        };

        _repository.Update(transaction);
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