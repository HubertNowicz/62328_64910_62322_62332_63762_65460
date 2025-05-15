using Organizer_przepisów_kulinarnych.DAL.Entities;
using System.ComponentModel.DataAnnotations;

public class RecipeInstructionStep
{
    public int Id { get; set; }
    public int StepNumber { get; set; }
    public string Description { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
}
