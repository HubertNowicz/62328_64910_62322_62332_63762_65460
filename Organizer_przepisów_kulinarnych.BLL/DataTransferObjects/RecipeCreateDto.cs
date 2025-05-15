namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class RecipeCreateDto
    {
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public int Preptime { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public List<RecipeIngredientDto> RecipeIngredients { get; set; }
        public List<RecipeInstructionStepDto> InstructionSteps { get; set; }
    }
}
