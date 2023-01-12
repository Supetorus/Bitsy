namespace moveen.utils
{
    public class BadException : System.Exception
    {
        public BadException()
        {
        }

        public BadException(string message)
        {
//            super(message);
        }

        public BadException(string message, System.Exception cause)
        {
//            super(message, cause);
        }

        public BadException(System.Exception cause)
        {
//            super(cause);
        }

        public static BadException die(string message)
        {
            throw new BadException(message);
        }

        public static BadException die(string message, System.Exception cause)
        {
            throw new BadException(message, cause);
        }

        public static BadException die(System.Exception cause)
        {
            throw new BadException(cause);
        }

        public static BadException notImplemented()
        {
            throw die("not implemented");
        }

        public static BadException shouldNeverReachHere()
        {
            return shouldNeverReachHere("");
        }

        public static BadException shouldNeverReachHere(string msg)
        {
            throw die("should never reach here " + msg);
        }

    }
}