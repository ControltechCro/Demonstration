#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.RAEtherNetIP;
using FTOptix.NativeUI;
using FTOptix.Alarm;
using FTOptix.S7TiaProfinet;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.SerialPort;
using FTOptix.UI;
using FTOptix.Core;
using System.Collections.Generic;
using FTOptix.Report;
using FTOptix.Recipe;
using FTOptix.Store;
using FTOptix.InfluxDBStoreLocal;
using FTOptix.InfluxDBStore;
#endregion

public class DigitalInputMux : BaseNetLogic
{
    private IUAVariable inputListVar;
    private IUAVariable inputIndexVar;
    private IUAVariable outputVar;

    public override void Start()
    {
        // Dohvaæanje varijabli iz istog Node-a gdje se nalazi ovaj NetLogic
        inputListVar = LogicObject.GetVariable("InputList");
        inputIndexVar = LogicObject.GetVariable("InputIndex");
        outputVar = LogicObject.GetVariable("OutputString");

        if (inputListVar == null || inputIndexVar == null || outputVar == null)
        {
            Log.Error(this.GetType().Name, "Jedna ili više varijabli nisu pronağene.");
            return;
        }

        // Pretplata na promjene vrijednosti varijabli
        inputListVar.VariableChange += OnInputsChanged;
        inputIndexVar.VariableChange += OnInputsChanged;

        // Prvo auriranje pri pokretanju logike
        UpdateSelectedValue();
    }

    public override void Stop()
    {
        // Odjava s dogağaja na zaustavljanju logike
        if (inputListVar != null)
            inputListVar.VariableChange -= OnInputsChanged;

        if (inputIndexVar != null)
            inputIndexVar.VariableChange -= OnInputsChanged;
    }

    private void OnInputsChanged(object sender, VariableChangeEventArgs e)
    {
        UpdateSelectedValue();
    }

    private void UpdateSelectedValue()
    {
        var inputArrayVariant = inputListVar.Value;
        var indexVariant = inputIndexVar.Value;

        if (inputArrayVariant == null || !(inputArrayVariant.Value is Array) || indexVariant == null)
        {
            outputVar.Value = "Greška u podacima";
            return;
        }

        try
        {
            string[] vrijednosti = (string[])inputArrayVariant.Value;
            int indeks = (int)indexVariant;

            if (indeks >= 0 && indeks < vrijednosti.Length)
            {
                outputVar.Value = vrijednosti[indeks];
            }
            else
            {
                outputVar.Value = "Nevaeæi indeks";
            }
        }
        catch (Exception ex)
        {
            Log.Error(this.GetType().Name, $"Greška prilikom obrade: {ex.Message}");
            outputVar.Value = "Greška u obradi";
        }
    }
}
