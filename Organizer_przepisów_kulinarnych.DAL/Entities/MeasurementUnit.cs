using Organizer_przepisów_kulinarnych.DAL.Entities;

public class MeasurementUnit
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
    public ICollection<IngredientUnit> IngredientUnits { get; set; }
}
