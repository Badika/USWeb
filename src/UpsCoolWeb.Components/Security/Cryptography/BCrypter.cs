using System;

namespace UpsCoolWeb.Components.Security
{
    public class BCrypter : IHasher
    {
        public String Hash(String value)
        {
            return BCrypt.Net.BCrypt.HashString(value, 6);
        }
        public String HashPassword(String value)
        {
            return BCrypt.Net.BCrypt.HashPassword(value, 13);
        }

        public Boolean Verify(String value, String hash)
        {
            if (value == null)
                return false;

            if (hash == null)
            {
                BCrypt.Net.BCrypt.Verify("TakeSameTime", "$2a$06$L01HfIu56AJsQWhsvzbByujj9XtGht5qJ/rxjA4bsKEJzu7fxQxqu");

                return false;
            }

            return BCrypt.Net.BCrypt.Verify(value, hash);
        }
        public Boolean VerifyPassword(String value, String passhash)
        {
            if (value == null)
                return false;

            if (passhash == null)
            {
                BCrypt.Net.BCrypt.Verify("TakeSameTime", "$2a$13$06DpsSNHCcSaVJ4cdSfLEeWXs2PYVXQ0bVXvShTt/g0I4t1pTwgTu");

                return false;
            }

            return BCrypt.Net.BCrypt.Verify(value, passhash);
        }
    }
}
