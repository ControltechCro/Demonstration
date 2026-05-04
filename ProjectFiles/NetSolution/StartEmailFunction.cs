#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.WebUI;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.Alarm;
using FTOptix.Recipe;
using FTOptix.DataLogger;
using FTOptix.Store;
using FTOptix.InfluxDBStoreRemote;
using FTOptix.SQLiteStore;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.S7TiaProfinet;
using FTOptix.MicroController;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.MQTTClient;
using FTOptix.MQTTBroker;
using FTOptix.InfluxDBStore;
using FTOptix.CommunicationDriver;
using FTOptix.SerialPort;
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.AuditSigning;
#endregion

public class StartEmailFunction : BaseNetLogic
{
    public override void Start()
    {
        //// Find the EmailSenderLogic object in the project
        //var emailSenderLogic = Project.Current.GetObject("NetLogic/EmailSenderLogic") as UAObject;
        //if (emailSenderLogic != null)
        //{
        //    // Call the SendEmail method
        //    object[] inputArgs = { "andro.veic2@gmail.com", "App started", "App started" };
        //    emailSenderLogic.ExecuteMethod("SendEmail", inputArgs);
        //}
        //else
        //{
        //    Log.Error("StartEmailFunction", "EmailSenderLogic object not found.");
        //}
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
}
