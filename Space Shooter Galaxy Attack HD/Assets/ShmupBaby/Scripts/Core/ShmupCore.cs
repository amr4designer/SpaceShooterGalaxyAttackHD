
namespace ShmupBaby
{

    /// <summary>
    /// Handles most of the events for this package.
    /// </summary>
    public delegate void ShmupDelegate(ShmupEventArgs args);

    /// <summary>
    /// This class act as the base class for any custom data
    /// that needs to pass by ShmupDelegate events.
    /// </summary>
    public abstract class ShmupEventArgs : System.EventArgs
    {

    }

}
