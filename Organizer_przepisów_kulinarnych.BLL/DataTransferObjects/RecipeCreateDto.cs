namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class RecipeCreateDto
    {
        public int Id { get; set; }
        public required string RecipeName { get; set; }
        public required string Description { get; set; }
        public int Preptime { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public required List<RecipeIngredientDto> RecipeIngredients { get; set; }
        public required List<RecipeInstructionStepDto> InstructionSteps { get; set; }
    }
}
