using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Kafee.AzureFunctions
{
    public class MailBrewerFunction(ILogger<MailBrewerFunction> logger)
    {
        [Function(nameof(MailBrewerFunction))]
        public async Task Run(
            [ServiceBusTrigger("mailbrewerqueue", Connection = "KafeeServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            logger.LogInformation("Message ID: {id}", message.MessageId);
            logger.LogInformation("Message Body: {body}", message.Body);
            logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            await messageActions.CompleteMessageAsync(message);
        }
    }
}
