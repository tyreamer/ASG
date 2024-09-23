using Google.Cloud.Firestore;
using ASGShared.Models;

namespace ASGBackend.Services
{
    public class FirestoreService(FirestoreDb firestoreDb)
    {
        private readonly FirestoreDb _firestoreDb = firestoreDb;

        public async Task SaveUserPreferencesAsync(UserPreferences preferences)
        {
            DocumentReference docRef = _firestoreDb.Collection("userPreferences").Document(preferences.UserId.ToString());
            await docRef.SetAsync(preferences);
        }

        public async Task<UserPreferences> GetUserPreferencesAsync(int userId)
        {
            DocumentReference docRef = _firestoreDb.Collection("userPreferences").Document(userId.ToString());
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.ConvertTo<UserPreferences>();
        }
    }
}