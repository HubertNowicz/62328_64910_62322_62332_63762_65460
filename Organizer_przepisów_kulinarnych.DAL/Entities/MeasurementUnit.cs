using Organizer_przepisów_kulinarnych.DAL.Entities;

public class MeasurementUnit
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
    public required ICollection<IngredientUnit> IngredientUnits { get; set; }
}
