namespace Hana;

public partial class MainPage : ContentPage
{
    string _counterBtnText = "Click Me";

    public string CounterBtnText
    {
        get => _counterBtnText;
        set
        {
            _counterBtnText = value;
            OnPropertyChanged(nameof(CounterBtnText));
        }
    }

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        CounterBtnText = "#" + CounterBtnText + "#";
    }
}

