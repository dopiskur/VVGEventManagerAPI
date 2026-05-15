using eventLib.Models;

namespace eventLib.Dal
{
    public interface IRepository
    {
        int UserAdd(User user);
        void UserUpdate(User user);
        void UserDelete(int? idUser);
        User? UserGet(int? idUser, string? username);
        IList<User> UsersGet();
        IList<UserRole> UserRolesGet();

        int EventAdd(Event value);
        void EventUpdate(Event value);
        void EventDelete(int? idEvent);
        Event? EventGet(int? idEvent);
        IList<Event> EventsGet(string search);
        IList<EventType> EventTypesGet();

        void EventPerformerAdd(int? eventID, int? performerID);
        void EventPerformerDelete(int? eventID, int? performerID);
        IList<EventPerformer> EventPerformersGet(int? eventID);

        IList<Performer> PerformersGet(string? search);
        void PerformerAdd(Performer performer);
        Performer? PerformerGet(int? idPerformer);
        void PerformerUpdate(Performer? value);
        void PerformerDelete(int? idPerformer);

        IList<Event> MyEventsGet(string search);
        IList<EventRegistration> EventRegistrationsGet(int? userID, string? search);
        void EventRegistrationAdd(int? eventID, int? userID);
        void EventRegistrationDelete(int? eventID, int? userID);
    }
}
