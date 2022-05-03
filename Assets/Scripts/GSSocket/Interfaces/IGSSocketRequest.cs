namespace Assets.Scripts.Socket.Interfaces
{
    public interface IGSSocketRequest
    {
        /// <summary>
        /// Returns true when the executed item is this.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="onExecuted"></param>
        /// <returns></returns>
        bool OnResponseReceived(GSSocketResponse e);
    }
}
