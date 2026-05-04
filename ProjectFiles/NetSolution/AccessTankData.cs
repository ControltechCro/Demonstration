using FTOptix.HMIProject;
using FTOptix.NetLogic;
using UAManagedCore;

public class AccessTankData : BaseNetLogic
{
    public override void Start()
    {
        // Access the Tank1 variable using the full path
        var tank1Var = Project.Current.GetObject("CommDrivers/S7TIAPROFINETDriver1/S7TIAPROFINETStation1/Tags/PLC/Data/LvlData/Tank1") as IUAVariable;
        if (tank1Var != null && tank1Var.Value != null)
        {
            // Use the correct type cast for your data, e.g., double, int, string, etc.
            var tank1Value = tank1Var.Value;
            Log.Info("AccessTankData", $"Tank1 value: {tank1Value}");
            var displayVar = LogicObject.GetVariable("DisplayValue");
            if (displayVar != null)
                displayVar.Value = tank1Value;
        }
        else
        {
            Log.Error("AccessTankData", "Tank1 variable not found or has no value.");
        }
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
}
