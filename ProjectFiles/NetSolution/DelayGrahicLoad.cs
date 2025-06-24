#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.Recipe;
using FTOptix.Alarm;
using FTOptix.Store;
using FTOptix.InfluxDBStoreLocal;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.S7TiaProfinet;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.InfluxDBStore;
using FTOptix.CommunicationDriver;
using FTOptix.SerialPort;
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.WebUI;
using System.Threading.Tasks;
using UAManagedCore;
using FTOptix.InfluxDBStoreRemote;
using FTOptix.DataLogger;
#endregion

public class DelayGrahicLoad : BaseNetLogic
{
    private IUAVariable outputVar;

    public override async void Start()
    {
        outputVar = LogicObject.GetVariable("Output");
        if (outputVar == null)
        {
            Log.Error(this.GetType().Name, "Output varijabla nije pronaðena.");
            return;
        }

        // Postavi na false odmah
        outputVar.Value = false;
        Log.Info(this.GetType().Name, "Output postavljen na false");

        // Èekaj 1 sekundu
        await Task.Delay(1000);

        // Postavi na true nakon odgode
        outputVar.Value = true;
        Log.Info(this.GetType().Name, "Output postavljen na true nakon odgode");
    }
}
