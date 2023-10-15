using System.Security.Cryptography;

namespace EmployeeManagementAPI.Helpers
{
    public static class EncDescPassword // Algorithm to encrypt & decrypt password (or texts). // No need to create instance of static to access its properties. You can directly call the methods.
    {
       
        public static void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt) // No need to create instance of static to access its properties. You can directly call the methods.
        {
            using (var hmac = new HMACSHA512()) //HMACSHA512 is the encryption model created ny .net team. It's a class, so we created an instance 'hmac' using 'new' keyword.
            {
                passwordSalt = hmac.Key;        // 'Key' property genearates random value
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // converts the 'string password' passed by the user into 'bytes' and then to 'hash'
            }
        }
        public static bool VerifyHashPassword(string password, byte[] passwordHash, byte[] passwordSalt) // this method will return boolean type
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash); // compares the 'computed hash' with the stored `passwordHash` using the `SequenceEqual` method and returns the result.
            }
        }
    }
}
/*The code you provided is a C# class named `EncDescPassword` that contains methods for creating and verifying password hashes using the HMACSHA512 algorithm from the `System.Security.Cryptography` namespace.

Here's an explanation of each method:

*   `CreateHashPassword`: This method takes a password as input and generates a password hash and salt. It uses an instance of the `HMACSHA512` class to perform the encryption. 
*   The `passwordSalt` is set to the `Key` property of the `hmac` instance, which generates a random salt value. 
*   The `passwordHash` is computed by passing the UTF-8 encoded bytes of the password to the `ComputeHash` method of the `hmac` instance.
*   By using the out keyword, the passwordHash variable in the caller's context will contain the computed password hash after calling the CreateHashPassword method.
*   The out keyword is used to indicate that the parameter is an output parameter, which means that any changes made to the parameter within the method will be reflected in the caller.

*   `VerifyHashPassword`: This method takes a password, a password hash, and a password salt as input and verifies if the provided password matches the stored password hash. 
*   It uses the same salt provided when creating the hash to perform the verification. It creates a new `HMACSHA512` instance using the `passwordSalt` and computes the hash of the provided password. 
*   Finally, it compares the computed hash with the stored `passwordHash` using the `SequenceEqual` method and returns the result.

Overall, this code provides a simple implementation for securely hashing and verifying passwords using the HMACSHA512 algorithm. It is important to note that this code only focuses on the encryption aspect and does not handle other password security considerations such as salting, iteration count, or storing the hash securely. It is recommended to use a comprehensive password hashing library that handles these aspects for production-level applications.*/