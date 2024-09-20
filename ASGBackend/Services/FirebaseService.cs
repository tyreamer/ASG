using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace ASGBackend.Services
{
    public class FirebaseService
    {
        public FirebaseService()
        {
            var firebaseCredentialPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

            if (string.IsNullOrEmpty(firebaseCredentialPath))
            {
                throw new InvalidOperationException("The GOOGLE_APPLICATION_CREDENTIALS environment variable is not set.");
            }

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(firebaseCredentialPath)
                });
            }
        }
    }
}
