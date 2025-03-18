using Spectre.Console;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Phonebook.selnoom.Sms;

public class SmsService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;

    public SmsService(string accountSid, string authToken, string fromNumber)
    {
        _accountSid = accountSid;
        _authToken = authToken;
        _fromNumber = fromNumber;
        TwilioClient.Init(_accountSid, _authToken);
    }

    public virtual void SendSms(string toNumber, string message)
    {
        var msg = MessageResource.Create(
            body: message,
            from: new PhoneNumber(_fromNumber),
            to: new PhoneNumber("+55" + toNumber)
        );
        AnsiConsole.MarkupLine("[green]Success![/]");
    }
}

public class DisabledSmsService : SmsService
{
    public DisabledSmsService() : base(string.Empty, string.Empty, string.Empty) { }

    public override void SendSms(string toNumber, string message)
    {
        Console.WriteLine("SMS feature is disabled. Please configure the Twilio settings in the appsettings.json file.");
    }
}
