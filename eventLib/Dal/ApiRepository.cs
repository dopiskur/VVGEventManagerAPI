using eventLib.Models;
using eventLib.Utils;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace eventLib.Dal
{
    public class ApiRepository : IApi
    {
        private readonly HttpClient _client;

        public ApiRepository(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        private void SetAuth(string? jwtKey)
        {
            _client.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(jwtKey)
                ? null
                : new AuthenticationHeaderValue("Bearer", jwtKey);
        }

        private static string BuildQuery(params (string key, string? value)[] parameters)
        {
            var dict = new Dictionary<string, string?>();
            foreach (var (key, value) in parameters)
            {
                if (!string.IsNullOrEmpty(value))
                    dict[key] = value;
            }
            return dict.Count > 0 ? QueryHelpers.AddQueryString("", dict!) : string.Empty;
        }

        public async Task<UserLoginResult?> UserLogin(UserLogin userLogin)
        {
            SetAuth(null);
            var response = await _client.PostAsJsonAsync("/api/User/Login", userLogin);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Login API failed ({(int)response.StatusCode}): {body}");
            }
            return await response.Content.ReadFromJsonAsync<UserLoginResult>();
        }

        public async Task<int> UserRegister(User newUser)
        {
            SetAuth(null);
            var response = await _client.PostAsJsonAsync("/api/User/Register", newUser);
            return await response.ReadContentAsync<int>();
        }

        public async Task<IList<User>> UsersGet(string jwtKey)
        {
            SetAuth(jwtKey);
            var response = await _client.GetAsync("/api/User/All");
            return await response.ReadContentAsync<IList<User>>() ?? new List<User>();
        }

        public async Task<User?> UserGet(string? jwtKey, int? idUser, string? username)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(
                ("idUser", idUser?.ToString()),
                ("username", username));
            var response = await _client.GetAsync("/api/User" + query);
            return await response.ReadContentAsync<User>();
        }

        public async Task UserUpdate(string jwtKey, User updatedUser)
        {
            SetAuth(jwtKey);
            var response = await _client.PutAsJsonAsync("/api/User", updatedUser);
            response.EnsureSuccessStatusCode();
        }

        public async Task UserDelete(string jwtKey, int? idUser)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("idUser", idUser?.ToString()));
            var response = await _client.DeleteAsync("/api/User" + query);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IList<UserRole>> UserRolesGet(string jwtKey)
        {
            SetAuth(jwtKey);
            var response = await _client.GetAsync("/api/User/Roles");
            return await response.ReadContentAsync<IList<UserRole>>() ?? new List<UserRole>();
        }

        public async Task<IList<Event>> EventsGet(string jwtKey, string? search)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("search", search));
            var response = await _client.GetAsync("/api/Event/All" + query);
            return await response.ReadContentAsync<IList<Event>>() ?? new List<Event>();
        }

        public async Task<IList<Event>> MyEventsGet(string jwtKey, string? search)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("search", search));
            var response = await _client.GetAsync("/api/Event/Browse" + query);
            return await response.ReadContentAsync<IList<Event>>() ?? new List<Event>();
        }

        public async Task<Event?> EventGet(string jwtKey, int? idEvent)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("idEvent", idEvent?.ToString()));
            var response = await _client.GetAsync("/api/Event" + query);
            return await response.ReadContentAsync<Event>();
        }

        public async Task<int> EventAdd(string jwtKey, Event value)
        {
            SetAuth(jwtKey);
            var response = await _client.PostAsJsonAsync("/api/Event", value);
            return await response.ReadContentAsync<int>();
        }

        public async Task EventUpdate(string jwtKey, Event value)
        {
            SetAuth(jwtKey);
            var response = await _client.PutAsJsonAsync("/api/Event", value);
            response.EnsureSuccessStatusCode();
        }

        public async Task EventDelete(string jwtKey, int? idEvent)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("idEvent", idEvent?.ToString()));
            var response = await _client.DeleteAsync("/api/Event" + query);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IList<EventType>> EventTypesGet(string jwtKey)
        {
            SetAuth(jwtKey);
            var response = await _client.GetAsync("/api/Event/Types");
            return await response.ReadContentAsync<IList<EventType>>() ?? new List<EventType>();
        }

        public async Task<IList<EventPerformer>> EventPerformersGet(string jwtKey, int? eventID)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("eventID", eventID?.ToString()));
            var response = await _client.GetAsync("/api/Event/Performers" + query);
            return await response.ReadContentAsync<IList<EventPerformer>>() ?? new List<EventPerformer>();
        }

        public async Task EventPerformerAdd(string jwtKey, int? eventID, int? performerID)
        {
            SetAuth(jwtKey);
            var payload = new { eventID, performerID };
            var response = await _client.PostAsJsonAsync("/api/Event/Performers", payload);
            response.EnsureSuccessStatusCode();
        }

        public async Task EventPerformerDelete(string jwtKey, int? eventID, int? performerID)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(
                ("eventID", eventID?.ToString()),
                ("performerID", performerID?.ToString()));
            var response = await _client.DeleteAsync("/api/Event/Performers" + query);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IList<Performer>> PerformersGet(string jwtKey, string? search)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("search", search));
            var response = await _client.GetAsync("/api/Performer/All" + query);
            return await response.ReadContentAsync<IList<Performer>>() ?? new List<Performer>();
        }

        public async Task<Performer?> PerformerGet(string jwtKey, int? idPerformer)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("idPerformer", idPerformer?.ToString()));
            var response = await _client.GetAsync("/api/Performer" + query);
            return await response.ReadContentAsync<Performer>();
        }

        public async Task PerformerAdd(string jwtKey, Performer performer)
        {
            SetAuth(jwtKey);
            var response = await _client.PostAsJsonAsync("/api/Performer", performer);
            response.EnsureSuccessStatusCode();
        }

        public async Task PerformerUpdate(string jwtKey, Performer performer)
        {
            SetAuth(jwtKey);
            var response = await _client.PutAsJsonAsync("/api/Performer", performer);
            response.EnsureSuccessStatusCode();
        }

        public async Task PerformerDelete(string jwtKey, int? idPerformer)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(("idPerformer", idPerformer?.ToString()));
            var response = await _client.DeleteAsync("/api/Performer" + query);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IList<EventRegistration>> EventRegistrationsGet(string jwtKey, int? userID, string? search)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(
                ("userID", userID?.ToString()),
                ("search", search));
            var response = await _client.GetAsync("/api/EventRegistration" + query);
            return await response.ReadContentAsync<IList<EventRegistration>>() ?? new List<EventRegistration>();
        }

        public async Task EventRegistrationAdd(string jwtKey, int? eventID, int? userID)
        {
            SetAuth(jwtKey);
            var payload = new { eventID, userID };
            var response = await _client.PostAsJsonAsync("/api/EventRegistration", payload);
            response.EnsureSuccessStatusCode();
        }

        public async Task EventRegistrationDelete(string jwtKey, int? eventID, int? userID)
        {
            SetAuth(jwtKey);
            var query = BuildQuery(
                ("eventID", eventID?.ToString()),
                ("userID", userID?.ToString()));
            var response = await _client.DeleteAsync("/api/EventRegistration" + query);
            response.EnsureSuccessStatusCode();
        }
    }
}
