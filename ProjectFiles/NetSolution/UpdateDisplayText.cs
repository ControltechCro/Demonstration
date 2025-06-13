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
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Core;
using FTOptix.NetLogic;
#endregion

public class UpdateDisplayText : BaseNetLogic
{
    [ExportMethod]
    public void UpdateText()
    {
        try
        {
            // 1. Dohvat indeksa
            var indexVar = Project.Current.GetVariable("Model/SelectedVentilIndex");
            if (indexVar == null) throw new Exception("Varijabla za indeks nije pronaðena");

            int index = int.Parse(indexVar.Value.ToString());

            // 2. Dohvat liste naziva
            var listDI = Project.Current.GetVariable("Model/List_DI/Entry");
            if (listDI == null) throw new Exception("Lista DI nije pronaðena");

            // 3. Provjera raspona
            if (index < 0 || index >= listDI.Children.Count)
                throw new Exception($"Indeks {index} izvan raspona 0-{listDI.Children.Count - 1}");

            // 4. Dohvat naziva ventila
            var ventilVar = listDI.Children[index] as IUAVariable;
            string naziv = ventilVar?.Value?.ToString() ?? "Nepoznato";

            // 5. Postavljanje vrijednosti
            var displayVar = Project.Current.GetVariable("Model/DisplayText");
            displayVar.Value = naziv;

            Log.Info("UpdateDisplayText", $"Ažurirano: {naziv}");
        }
        catch (Exception ex)
        {
            Log.Error("UpdateDisplayText", $"Greška: {ex.Message}");
        }
    }
}