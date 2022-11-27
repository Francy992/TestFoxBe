using MediatR;
using Newtonsoft.Json;

namespace TestFoxBe.Mediators;

public interface INotifierMediatorService
{
    Task Notify(NotificationTypeEnum notificationTypeEnum, object obj);
}

public class NotifierMediatorService : INotifierMediatorService
{
    private readonly IMediator _mediator;

    public NotifierMediatorService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Notify(NotificationTypeEnum notificationTypeEnum, object obj)
    {
        await _mediator.Publish(new NotificationMessage { NotificationType = notificationTypeEnum, SerializedObj = JsonConvert.SerializeObject(obj) });
    }
}