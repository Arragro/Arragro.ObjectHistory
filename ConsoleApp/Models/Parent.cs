namespace ConsoleApp.Models
{
    public class Parent
    {
        public int ParentId { get; set; }
        public string Name { get; set; }

        public Child Child { get; set; }
    }
}
