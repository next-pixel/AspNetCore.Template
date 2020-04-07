using System;

namespace MvcTemplate.Resources
{
    public static class Message
    {
        public static String For<TView>(String key, params Object[] args)
        {
            String message = Resource.Localized(typeof(TView).Name, "Messages", key);

            return message == "" || args.Length == 0 ? message : String.Format(message, args);
        }
    }
}
