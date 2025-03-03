using Microsoft.Extensions.Hosting;

namespace Store.Common.Interfaces;

public abstract class IAppLayer
{
    /// <summary>
    /// This method is meant for doing things that are required to be done before everything is loaded.
    /// Only use this method if the actions that are to be done need to be done before everything else no matter what.
    /// Otherwise, use the Standard Load() method.
    /// </summary>
    public virtual void PreLoad(string[] args, IHostBuilder host) { }
    
    /// <summary>
    /// The standard Load method for the loading of this layer of the Application. Usage of this method over either Pre- or PostLoad() is preferred for the transparency of execution.
    /// </summary>
    public virtual void Load(string[] args, IHostBuilder host) { }
    
    /// <summary>
    /// Use this method for the things that need to be done after everything is loaded.
    /// Only use this method if the actions that are to be done need to be done after everything else no matter what.
    /// Otherwise, use the Standard Load() method
    /// </summary>
    public virtual void PostLoad(string[] args, IHost host) { }

    /// <summary>
    /// THe method called upon the unloading of the Application. This is called in the same sequence as the Load sequence. 
    /// </summary>
    public virtual void Unload() { }
}