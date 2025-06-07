using Organizer_przepisów_kulinarnych.DAL.Entities;
using System.ComponentModel.DataAnnotations;

public class RecipeInstructionStep
{
    public int Id { get; set; }
    public int StepNumber { get; set; }
    public required string Description { get; set; }
    public int RecipeId { get; set; }
    public required Recipe Recipe { get; set; }
}
