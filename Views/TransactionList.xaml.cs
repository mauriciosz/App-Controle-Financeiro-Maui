using AppControleFinanceiro.Repositories;
using AppControleFinanceiro.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;	

namespace AppControleFinanceiro.Views;

public partial class TransactionList : ContentPage
{
	private TransactionAdd _transactionAdd;
	private TransactionEdit _transactionEdit;
	private ITransactionRepository _repository;

    public TransactionList(ITransactionRepository repository)
	{
		this._repository = repository;	
		InitializeComponent();

		Reload();

		//  Eszse código "escuta" para ver se teve algum novo cadastro e caso tenha, ele recarrega os dados da tela
		WeakReferenceMessenger.Default.Register<string>(this, (e, msg) => 
			{
				Reload();
			});
	}

	private void Reload()
	{
		var itens = _repository.GetAll();
		CollectionViewTransactions.ItemsSource = itens;

		// enquanto o Type do item 'a' for do tipo 'Income', vai somar o Value de 'a'
		double receitas = itens.Where(a => a.Type == Models.TransactionType.Income).Sum(a => a.Value);
        double despesas = itens.Where(a => a.Type == Models.TransactionType.Expenses).Sum(a => a.Value);
		double saldo = receitas - despesas;


		lblReceitas.Text = receitas != 0 ? receitas.ToString("C") : " -- -- ";
		lblDespesas.Text = despesas != 0 ? despesas.ToString("C") : " -- -- ";
        lblSaldo.Text = saldo != 0 ? saldo.ToString("C") : " -- -- ";
    }
	private void OnButtonClicked_To_TransactionAdd(object sender, EventArgs e)
	{
		var transactionAdd = Handler.MauiContext.Services.GetService<TransactionAdd>();
		Navigation.PushModalAsync(transactionAdd);
	}
    private void TapGestureRecognizerTapped_To_TransactionEdit(object sender, TappedEventArgs e)
    {
		var grid = (Grid)sender; // pega o grid clicado
		var gesto = (TapGestureRecognizer)grid.GestureRecognizers[0]; // faz um cast do gesto do grid (click) e passa para a variavel
		Transaction transaction = (Transaction)gesto.CommandParameter; // faz um novo cast passando os parametros do grid clicado para a transação

        var transactionEdit = Handler.MauiContext.Services.GetService<TransactionEdit>();
		transactionEdit.SetTransactionToEdit(transaction); // chama o método de edição passando a transação que será editada
        Navigation.PushModalAsync(transactionEdit);
    }

    private async void TapGestureRecognizerTapped_To_Delete(object sender, TappedEventArgs e)
    {
		await AnimationBorder((Border)sender, true);

		bool result =  await App.Current.MainPage.DisplayAlert("Exclusão", "Deseja Excluir o registro atual?", "Sim", "Não");
		if (result)
		{
			Transaction transaction = (Transaction)e.Parameter; // faz praticamente a mesma coisa que é feito com o Grid nas linhas 50 e 51
			_repository.Delete(transaction);

			Reload();
		}
		else 
		{
			await AnimationBorder((Border)sender, false);
		}
    }

	private Color _borderOriginalBackgroundColor;
	private string _labelOriginalText;
	private async Task AnimationBorder(Border border, bool IsDeleteAnimation)
	{

        var label = (Label)border.Content;

        if (IsDeleteAnimation)
		{
			// Armazena a cor e texto originais
			_borderOriginalBackgroundColor = border.BackgroundColor;
			_labelOriginalText = label.Text;

			// faz a animação de rotação de 90° em 1 segundo 
			await border.RotateYTo(90, 50);

			// passa nova cor e texto para a label
			border.BackgroundColor = Colors.Red;
			label.TextColor = Colors.White;
			label.Text = "X";

			// faz o restante da animação após trocar a cor e texto
            await border.RotateYTo(180, 50);
        }
		else 
		{	
			// desfaz animação voltando para o status anterior
			await border.RotateYTo(90, 50);

			// retorna a cor do texto para o Preto (que ja era o padrão)
			label.TextColor = Colors.Black;

			// Volta a Cor da Borda e texto do Label originais
			label.Text = _labelOriginalText;
			border.BackgroundColor = _borderOriginalBackgroundColor;

            await border.RotateYTo(0, 50);
        }
	
	}
}