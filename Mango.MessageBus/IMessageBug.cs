namespace Mango.MessageBus
{
    public interface IMessageBug
    {
        Task PublishMessage(object message,string topic_queue_name);
    }
}
