﻿namespace RecipeManagementWebAPI.Dto.Rating
{
    public class RatingDto
    {
     
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public int RatingValue { get; set; }
    }
}
