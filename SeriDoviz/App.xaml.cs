namespace SeriDoviz;
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;
    public App(IServiceProvider services)
    {
        InitializeComponent();
        Services = services ?? throw new ArgumentNullException(nameof(services));
        MainPage = new AppShell();
    }
}
