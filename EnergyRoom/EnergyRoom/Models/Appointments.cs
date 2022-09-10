namespace EnergyRoom.Models
{
    public class Appointments
    {
        public long ID { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string UserFullname { get; set; }
        public string UserPhone { get; set; }
    }
}