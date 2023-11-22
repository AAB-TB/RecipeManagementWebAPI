namespace RecipeManagementWebAPI.Dto.Recipe
{
    public class RecipeDto
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal AverageRating { get; set; }
    }
}
