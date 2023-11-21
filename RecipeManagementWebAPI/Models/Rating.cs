namespace RecipeManagementWebAPI.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public int RatingValue { get; set; }

        public Recipe Recipe { get; set; }
        public User User { get; set; }
    }
}
