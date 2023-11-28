namespace RecipeManagementWebAPI.Dto.Recipe
{
    public class UpdateRecipeDto
    {
        public int RecipeId { get; set; }   
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public int CategoryId { get; set; }
    }
}
