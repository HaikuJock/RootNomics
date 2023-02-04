namespace Haiku
{
    public interface Logging
    {
        void WriteLine(string message);
        void Flush();
    }

    internal class NullLogger : Logging
    {
        public void WriteLine(string message) { }
        public void Flush() { }
    }
}
