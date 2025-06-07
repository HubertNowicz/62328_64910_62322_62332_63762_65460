namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public required string RecipeName { get; set; }
        public required string Description { get; set; }
        public int Preptime { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public required UserDto User { get; set; }
        public required CategoryDto Category { get; set; }
        public required List<RecipeIngredientDto> RecipeIngredients { get; set; }
        public required List<RecipeInstructionStepDto> InstructionSteps { get; set; }
        public bool IsFavorite { get; set; }
        public int FavoriteCount { get; set; }
    }
}
