namespace citas.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BusinessId { get; set; }
        public Business Business { get; set; }
    }
}
