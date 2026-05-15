using eventLib.Models;

namespace eventLib.Dal
{
    public interface IApi
    {
        Task<UserLoginResult?> UserLogin(UserLogin userLogin);
        Task<int> UserRegister(User newUser);
        Task<IList<User>> UsersGet(string jwtKey);
        Task<User?> UserGet(string? jwtKey, int? idUser, string? username);
        Task UserUpdate(string jwtKey, User updatedUser);
        Task UserDelete(string jwtKey, int? idUser);
        Task<IList<UserRole>> UserRolesGet(string jwtKey);

        Task<IList<Event>> EventsGet(string jwtKey, string? search);
        Task<IList<Event>> MyEventsGet(string jwtKey, string? search);
        Task<Event?> EventGet(string jwtKey, int? idEvent);
        Task<int> EventAdd(string jwtKey, Event value);
        Task EventUpdate(string jwtKey, Event value);
        Task EventDelete(string jwtKey, int? idEvent);
        Task<IList<EventType>> EventTypesGet(string jwtKey);
        Task<IList<EventPerformer>> EventPerformersGet(string jwtKey, int? eventID);
        Task EventPerformerAdd(string jwtKey, int? eventID, int? performerID);
        Task EventPerformerDelete(string jwtKey, int? eventID, int? performerID);

        Task<IList<Performer>> PerformersGet(string jwtKey, string? search);
        Task<Performer?> PerformerGet(string jwtKey, int? idPerformer);
        Task PerformerAdd(string jwtKey, Performer performer);
        Task PerformerUpdate(string jwtKey, Performer performer);
        Task PerformerDelete(string jwtKey, int? idPerformer);

        Task<IList<EventRegistration>> EventRegistrationsGet(string jwtKey, int? userID, string? search);
        Task EventRegistrationAdd(string jwtKey, int? eventID, int? userID);
        Task EventRegistrationDelete(string jwtKey, int? eventID, int? userID);
    }
}
