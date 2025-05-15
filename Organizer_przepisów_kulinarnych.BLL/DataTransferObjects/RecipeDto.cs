namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class RecipeDto
    {
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public int Preptime { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserSurname { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<RecipeIngredientDto> RecipeIngredients { get; set; }
        public List<RecipeInstructionStepDto> InstructionSteps { get; set; }
        public bool IsFavorite { get; set; }
    }
}
