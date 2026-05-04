#region Using directives
using System.Net.Mail;
using System.Net;
using FTOptix.Core;
using UAManagedCore;
using FTOptix.NetLogic;
using System.Collections.Generic;
using FTOptix.AuditSigning;
#endregion

public class EmailSenderLogic : BaseNetLogic
{
    private IUAVariable sendTriggerVar;
    private IUAVariable recipientEmailVar;
    private IUAVariable createEmailMessage;
    private IUAVariable textFieldVar;
    private IUAVariable spinBoxVar;
    private IUAVariable optionButtonVar;

    public override void Start()
    {
        ValidateCertificate();
        emailStatus = GetVariableValue("EmailSendingStatus");
        maxDelay = GetVariableValue("DelayBeforeRetry");
        maxDelay.VariableChange += RestartPeriodicTask;

        // Trigger monitoring logic
        sendTriggerVar = LogicObject.GetVariable("SendTrigger");
        recipientEmailVar = LogicObject.GetVariable("RecipientEmail");
        createEmailMessage = LogicObject.GetVariable("EmailMessage");

        if (sendTriggerVar != null)
        {
            sendTriggerVar.VariableChange += OnSendTriggerChanged;
        }
        else
        {
            Log.Error("EmailSenderLogic", "SendTrigger variable not found.");
        }

        if (recipientEmailVar == null)
        {
            Log.Error("EmailSenderLogic", "RecipientEmail variable not found.");
        }

        textFieldVar = LogicObject.GetVariable("TextFieldValue");
        spinBoxVar = LogicObject.GetVariable("SpinBoxValue");
        optionButtonVar = LogicObject.GetVariable("OptionButtonValue");

        if (textFieldVar == null)
            Log.Error("EmailSenderLogic", "TextFieldValue variable not found.");
        if (spinBoxVar == null)
            Log.Error("EmailSenderLogic", "SpinBoxValue variable not found.");
        if (optionButtonVar == null)
            Log.Error("EmailSenderLogic", "OptionButtonValue variable not found.");

    }

    /// <summary>
    /// This method restarts a periodic task with updated parameters.
    /// If the minimum delay before retrying is less than 10 seconds or null, it logs a warning message.
    /// Otherwise, it cancels the existing task, creates a new one with the updated parameters, and starts the new task.
    /// </summary>
    /// <param name="sender">The source object for the event.</param>
    /// <param name="e">Event arguments containing the current value of the delay before retrying.</param>
    /// <remarks>
    /// The method ensures that the email sending logic continues to function correctly by managing the periodic task's execution based on the specified delay before retrying.
    /// </remarks>

    private void OnSendTriggerChanged(object sender, VariableChangeEventArgs e)
    {
        // Only send email when SendTrigger is set to true
        if (e.NewValue != null && e.NewValue.Value is bool triggered && triggered)
        {
            if (recipientEmailVar == null || recipientEmailVar.Value == null)
            {
                Log.Error("EmailSenderLogic", "RecipientEmail variable is not set.");
                sendTriggerVar.Value = false;
                return;
            }

            string recipient = recipientEmailVar.Value.ToString();
            string message = (string)createEmailMessage.Value;
                
            // Call the internal SendEmail method
            SendEmail(recipient, "Button Pressed", message);

            // Optionally reset the trigger
            sendTriggerVar.Value = false;
        }
    }
    private readonly Dictionary<int, string> optionButtonLabels = new Dictionary<int, string>
    {
        { 0, "Preuzimanje" },
        { 1, "Kuæna dostava" },
        { 2, "Paketomat" }
    };

    private string BuildDataStructureMessage()
    {
        string textField = textFieldVar?.Value != null ? textFieldVar.Value.ToString() : "(not set)";
        string spinBox = spinBoxVar?.Value != null ? spinBoxVar.Value.ToString() : "(not set)";
        string optionButtonLabel = "(not set)";

        if (optionButtonVar?.Value != null)
        {
            // Convert UAValue to int before checking the dictionary
            if (optionButtonVar.Value.Value is int optionValue && optionButtonLabels.TryGetValue(optionValue, out string label))
            {
                optionButtonLabel = label;
            }
            else
            {
                optionButtonLabel = optionButtonVar.Value.ToString();
            }
        }

        return $"Podaci o narudžbi:\n" +
               $"- Ime proizvoda: {textField}\n" +
               $"- Kolièina: {spinBox}\n" +
               $"- Vrsta dostave: {optionButtonLabel}";
    }

    [ExportMethod]
    public void SendDataStructureEmail()
    {
        if (recipientEmailVar == null || recipientEmailVar.Value == null)
        {
            Log.Error("EmailSenderLogic", "RecipientEmail variable is not set.");
            return;
        }

        string recipient = recipientEmailVar.Value.ToString();
        string subject = "Data Structure Submission";
        string body = BuildDataStructureMessage();

        SendEmail(recipient, subject, body);
    }

    public override void Stop()
    {
        if (maxDelay != null)
            maxDelay.VariableChange -= RestartPeriodicTask;
        if (sendTriggerVar != null)
            sendTriggerVar.VariableChange -= OnSendTriggerChanged;
    }

    private void RestartPeriodicTask(object sender, VariableChangeEventArgs e)
    {
        if (e.NewValue < 10000 || e.NewValue == null)
        {
            Log.Warning("EmailSenderLogic", "Minimum delay before retrying should be 10 seconds");
            return;
        }

        retryPeriodicTask?.Cancel();
        retryPeriodicTask = new PeriodicTask(SendQueuedMessage, e.NewValue, LogicObject);
        retryPeriodicTask.Start();
    }

    /// <summary>
    /// Sends an email using SMTP parameters provided during initialization.
    /// If SMTP parameters are not valid or email validation fails, no action is taken.
    /// A periodic task is started with a specified delay before retries if necessary.
    /// The email's content is constructed based on the provided parameters.
    /// </summary>
    /// <param name="mailToAddress">The recipient address for the email.</param>
    /// <param name="mailSubject">The subject line for the email.</param>
    /// <param name="mailBody">The body text for the email.</param>
    [ExportMethod]
    public void SendEmail(string mailToAddress, string mailSubject, string mailBody)
    {
        if (!InitializeAndValidateSMTPParameters())
            return;

        if (!ValidateEmail(mailToAddress, mailSubject, mailBody))
            return;

        var fromAddress = new MailAddress(senderAddress, "From");
        var toAddress = new MailAddress(mailToAddress, "To");

        if (retryPeriodicTask == null)
        {
            var delayBeforeRetry = GetVariableValue("DelayBeforeRetry").Value;
            if (delayBeforeRetry >= 10000)
            {
                retryPeriodicTask = new PeriodicTask(SendQueuedMessage, delayBeforeRetry, LogicObject);
                retryPeriodicTask.Start();
            }
        }

        smtpClient = new SmtpClient
        {
            Host = smtpHostname,
            Port = smtpPort,
            EnableSsl = enableSSL,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, senderPassword)
        };
        var message = CreateEmailMessage(fromAddress, toAddress, mailBody, mailSubject);
        TrySendEmail(message);
    }

    /// <summary>
    /// This method creates an email message with retries capabilities.
    /// It sets the sender address, recipient address, body content, subject, encoding, and attachments.
    /// If an attachment URI is provided, it adds it as an attachment to the email.
    /// The reply-to list is also set for the recipient address.
    /// </summary>
    /// <param name="fromAddress">The sender's email address.</param>
    /// <param name="toAddress">The recipient's email address.</param>
    /// <param name="mailBody">The body content of the email.</param>
    /// <param name="mailSubject">The subject of the email.</param>
    /// <returns>An instance of MailMessageWithRetries containing the created email message.</returns>
    private MailMessageWithRetries CreateEmailMessage(MailAddress fromAddress, MailAddress toAddress, string mailBody, string mailSubject)
    {
        var mailMessage = new MailMessageWithRetries(fromAddress, toAddress)
        {
            Body = mailBody,
            Subject = mailSubject,
            BodyEncoding = System.Text.Encoding.UTF8,
        };

        var attachment = GetVariableValue("Attachment").Value;
        if (!string.IsNullOrEmpty(attachment))
        {
            var attachmentUri = new ResourceUri(attachment);
            mailMessage.Attachments.Add(new Attachment(attachmentUri.Uri));
        }

        mailMessage.ReplyToList.Add(toAddress);
        return mailMessage;
    }

    /// <summary>
    /// This method attempts to send an email message using the SMTP client.
    /// If the sending fails, it checks if retrying is possible based on the maximum retries value.
    /// If retrying is allowed, it enqueues the failed message for later processing.
    /// </summary>
    /// <param name="message">The mail message with retry information.</param>
    private void TrySendEmail(MailMessageWithRetries message)
    {
        if (!CanRetrySendingMessage(message))
            return;

        using (message)
        {
            try
            {
                message.AttemptNumber++;
                Log.Info("EmailSender", $"Sending Email... ");
                smtpClient.Send(message);

                emailStatus.Value = true;
                Log.Info("EmailSenderLogic", "Email sent successfully");
            }
            catch (SmtpException e)
            {
                emailStatus.Value = false;
                Log.Error("EmailSenderLogic", $"Email failed to send: {e.StatusCode} {e.Message}");

                if (CanRetrySendingMessage(message))
                    EnqueueFailedMessage(message);
            }
        }
    }

    /// <summary>
    /// This method sends a queued message that has failed its retry attempts or was canceled by the user.
    /// If there are no messages left in the failed queue or the periodic task is marked as canceled, it exits without sending anything.
    /// Otherwise, it retrieves the next message from the queue, checks if retrying sending the message is possible based on a variable setting,
    /// logs the retry attempt information, and then tries to send the email again.
    /// </summary>
    /// <param name="task">The periodic task associated with this operation.</param>
    private void SendQueuedMessage(PeriodicTask task)
    {
        if (failedMessagesQueue.Count == 0 || task.IsCancellationRequested)
            return;

        var message = failedMessagesQueue.Pop();

        if (CanRetrySendingMessage(message))
        {
            var retries = GetVariableValue("MaxRetriesOnFailure").Value;
            Log.Info($"Retry Sending email attempt {message.AttemptNumber} of {retries}");
            TrySendEmail(message);
        }
    }

    /// <summary>
    /// Adds an enqueued failed message to the queue for retries.
    /// </summary>
    /// <param name="message">The mail message with retry information.</param>
    /// <remarks>
    /// This method adds a failed message to the `failedMessagesQueue` for potential reprocessing or handling after a retry attempt.
    /// </remarks>
    private void EnqueueFailedMessage(MailMessageWithRetries message)
    {
        failedMessagesQueue.Push(message);
    }

    /// <summary>
    /// Initializes and validates SMTP parameters for email sending.
    /// Validates the sender's email address, password, SMTP hostname, port, and SSL settings.
    /// Returns true on successful initialization or false otherwise.
    /// </summary>
    /// <returns>
    /// A boolean indicating whether the SMTP parameters were successfully initialized.
    /// </returns>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// bool isValid = InitializeAndValidateSMTPParameters();
    /// if (!isValid) {
    ///     // Handle invalid parameters
    /// }
    /// </code>
    /// </remarks>
    private bool InitializeAndValidateSMTPParameters()
    {
        senderAddress = (string)GetVariableValue("SenderEmailAddress").Value;
        if (string.IsNullOrEmpty(senderAddress))
        {
            Log.Error("EmailSenderLogic", "Invalid Sender Email address");
            return false;
        }

        senderPassword = (string)GetVariableValue("SenderEmailPassword").Value;
        if (string.IsNullOrEmpty(senderPassword))
        {
            Log.Error("EmailSenderLogic", "Invalid sender password");
            return false;
        }

        smtpHostname = (string)GetVariableValue("SMTPHostname").Value;
        if (string.IsNullOrEmpty(smtpHostname))
        {
            Log.Error("EmailSenderLogic", "Invalid SMTP hostname");
            return false;
        }

        smtpPort = (int)GetVariableValue("SMTPPort").Value;
        enableSSL = (bool)GetVariableValue("EnableSSL").Value;

        return true;
    }

    /// <summary>
    /// Determines if retrying sending the message is allowed based on the maximum retries value.
    /// <example>
    /// For example:
    /// <code>
    /// bool canRetry = CanRetrySendingMessage(new MailMessageWithRetries());
    /// </code>
    /// results in <c>canRetry</c>'s value depending on whether the configured maximum retries are non-negative and within the current attempt count.
    /// </example>
    /// </summary>
    /// <param name="message">The mail message with retries for processing.</param>
    /// <returns>
    /// A boolean indicating whether retrying is allowed based on the given configuration.
    /// </returns>
    /// <remarks>
    /// The method checks if there are positive retries defined and ensures that the attempted send operation has not exceeded these limits.
    /// </remarks>
    private bool CanRetrySendingMessage(MailMessageWithRetries message)
    {
        var maxRetries = GetVariableValue("MaxRetriesOnFailure").Value;
        return maxRetries >= 0 && message.AttemptNumber <= maxRetries;
    }

    private class MailMessageWithRetries : MailMessage
    {
        public MailMessageWithRetries(MailAddress fromAddress, MailAddress toAddress)
            : base(fromAddress, toAddress)
        {

        }

        public int AttemptNumber { get; set; } = 0;
    }

    /// <summary>
    /// Retrieves the value associated with the specified variable name from the logic object.
    /// If the variable is not found, an error message is logged and null is returned.
    /// </summary>
    /// <param name="variableName">The name of the variable to retrieve.</param>
    /// <returns>The value of the variable or null if it does not exist.</returns>
    private IUAVariable GetVariableValue(string variableName)
    {
        var variable = LogicObject.GetVariable(variableName);
        if (variable == null)
        {
            Log.Error($"{variableName} not found");
            return null;
        }
        return variable;
    }

    /// <summary>
    /// Validates an email address, subject, and body for an email sender logic.
    /// If any part is empty or invalid, logs an error message and returns false.
    /// Otherwise, returns true.
    /// </summary>
    /// <param name="recieverEmail">The recipient's email address.</param>
    /// <param name="emailSubject">The subject of the email.</param>
    /// <param name="emailBody">The body content of the email.</param>
    /// <returns>
    /// Returns true if all parts are valid; otherwise, false.
    /// </returns>
    /// <remarks>
    /// Logs errors related to email validation when necessary.
    /// </remarks>
    private bool ValidateEmail(string recieverEmail, string emailSubject, string emailBody)
    {
        if (string.IsNullOrEmpty(emailSubject))
        {
            Log.Error("EmailSenderLogic", "Email subject is empty or malformed");
            return false;
        }

        if (string.IsNullOrEmpty(emailBody))
        {
            Log.Error("EmailSenderLogic", "Email body is empty or malformed");
            return false;
        }

        if (string.IsNullOrEmpty(recieverEmail))
        {
            Log.Error("EmailSenderLogic", "RecieverEmail is empty or null");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validates the certificate by disabling SSL/TLS certificate validation on Linux platforms.
    /// </summary>
    /// <remarks>
    /// On Linux systems, this method disables SSL/TLS certificate validation for improved security when using service points.
    /// </remarks>
    /// <param name="sender">Not used.</param>
    /// <param name="certificate">Not used.</param>
    /// <param name="chain">Not used.</param>
    /// <param name="error">Not used.</param>
    private void ValidateCertificate()
    {
        if (System.Runtime.InteropServices.RuntimeInformation
                                               .IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            ServicePointManager.ServerCertificateValidationCallback = (_, __, ___, ____) => { return true; };
    }

    private string senderAddress;
    private string senderPassword;
    private string smtpHostname;
    private int smtpPort;
    private bool enableSSL;

    private SmtpClient smtpClient;
    private PeriodicTask retryPeriodicTask;
    private IUAVariable maxDelay;
    private IUAVariable emailStatus;
    private readonly Stack<MailMessageWithRetries> failedMessagesQueue = new Stack<MailMessageWithRetries>();
}
