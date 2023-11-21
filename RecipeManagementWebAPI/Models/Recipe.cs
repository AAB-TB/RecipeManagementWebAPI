namespace RecipeManagementWebAPI.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public decimal AverageRating { get; set; }

        public Category Category { get; set; }
        public User User { get; set; }
    }
}
