namespace ionix.Data.Mongo
{
    using System.Text;

    public interface IMongoDbScriptProvider
    {
        StringBuilder ToScript();
    }
}
