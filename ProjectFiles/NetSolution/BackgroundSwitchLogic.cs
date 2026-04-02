#region Using directives
using FTOptix.NetLogic;
using UAManagedCore;
#endregion

public class BackgroundSwitchLogic : BaseNetLogic
{
    private IUAVariable backgroundEnable;
    private IUAVariable screenNo;

    public override void Start()
    {
        backgroundEnable = Owner.GetVariable("BackgroundEnable");
        screenNo = Owner.GetVariable("ScreenNo");

        if (backgroundEnable == null)
        {
            Log.Error("BackgroundSwitchLogic", "Varijabla 'BackgroundEnable' nije pronađena.");
            return;
        }

        if (screenNo == null)
        {
            Log.Error("BackgroundSwitchLogic", "Varijabla 'ScreenNo' nije pronađena.");
            return;
        }

        backgroundEnable.VariableChange += OnBackgroundChanged;

        Log.Info("BackgroundSwitchLogic", "Inicijalizacija uspješna.");
    }

    public override void Stop()
    {
        if (backgroundEnable != null)
            backgroundEnable.VariableChange -= OnBackgroundChanged;
    }

    private void OnBackgroundChanged(object sender, VariableChangeEventArgs e)
    {
        try
        {
            bool isActive = (bool)backgroundEnable.Value;
            int current = (int)screenNo.Value;

            Log.Info("BackgroundSwitchLogic", $"Trigger! BackgroundEnable={isActive}, ScreenNo trenutno={current}");

            screenNo.Value = isActive ? current + 10 : current - 10;

            Log.Info("BackgroundSwitchLogic", $"ScreenNo postavljeno na={screenNo.Value}");
        }
        catch (System.Exception ex)
        {
            Log.Error("BackgroundSwitchLogic", $"Greška: {ex.Message}");
        }
    }
}