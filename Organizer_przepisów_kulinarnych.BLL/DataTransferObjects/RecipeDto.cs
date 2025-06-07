namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public int Preptime { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public UserDto User { get; set; }
        public CategoryDto Category { get; set; }
        public List<RecipeIngredientDto> RecipeIngredients { get; set; }
        public List<RecipeInstructionStepDto> InstructionSteps { get; set; }
        public bool IsFavorite { get; set; }
        public int FavoriteCount { get; set; }
    }
}
