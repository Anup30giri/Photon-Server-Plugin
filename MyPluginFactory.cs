using Photon.Hive.Plugin;

namespace MyFirstPlugin
{
    public class MyPluginFactory : PluginFactoryBase
    {
        public override IGamePlugin CreatePlugin(string pluginName)
        {
            return new MyFirstPlugin();
        }
    }
}
