using UAManagedCore;
using FTOptix.NetLogic;
using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.AuditSigning;
using FTOptix.Modbus;

public class SendEmailButtonLogic : BaseNetLogic
{
    private IUAVariable sendTriggerVar;
    private IUAVariable recipientEmailVar;

    public override void Start()
    {
        //// Get the SendTrigger variable
        //sendTriggerVar = LogicObject.GetVariable("SendTrigger");
        //// Get the RecipientEmail variable
        //recipientEmailVar = LogicObject.GetVariable("RecipientEmail");

        //if (sendTriggerVar != null)
        //{
        //    sendTriggerVar.VariableChange += OnSendTriggerChanged;
        //}
        //else
        //{
        //    Log.Error("SendEmailButtonLogic", "SendTrigger variable not found.");
        //}

        //if (recipientEmailVar == null)
        //{
        //    Log.Error("SendEmailButtonLogic", "RecipientEmail variable not found.");
        //}
    }

    //private void OnSendTriggerChanged(object sender, VariableChangeEventArgs e)
    //{
    //    // Only send email when SendTrigger is set to true
    //    if (e.NewValue != null && e.NewValue.Value is bool triggered && triggered)
    //    {
    //        if (recipientEmailVar == null || recipientEmailVar.Value == null)
    //        {
    //            Log.Error("SendEmailButtonLogic", "RecipientEmail variable is not set.");
    //            sendTriggerVar.Value = false;
    //            return;
    //        }

    //        string recipient = recipientEmailVar.Value.ToString();

    //        // Find the EmailSenderLogic object
    //        var emailSenderLogic = Project.Current.GetObject("NetLogic/EmailSenderLogic") as UAObject;
    //        if (emailSenderLogic != null)
    //        {
    //            object[] inputArgs = { recipient, "Button Pressed", "The button was pressed." };
    //            emailSenderLogic.ExecuteMethod("SendEmail", inputArgs);
    //        }
    //        else
    //        {
    //            Log.Error("SendEmailButtonLogic", "EmailSenderLogic object not found.");
    //        }

    //        // Optionally reset the trigger
    //        sendTriggerVar.Value = false;
    //    }
    //}

    public override void Stop()
    {
        //if (sendTriggerVar != null)
        //    sendTriggerVar.VariableChange -= OnSendTriggerChanged;
    }
}
