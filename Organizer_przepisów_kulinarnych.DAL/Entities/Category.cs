namespace Organizer_przepisów_kulinarnych.BLL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = [];


    }
}
