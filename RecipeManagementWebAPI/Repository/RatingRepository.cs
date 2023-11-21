using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Services;

namespace RecipeManagementWebAPI.Repository
{
    public class RatingRepository:IRatingService
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
       


        public RatingRepository(
            DapperContext dapperContext,
            IMapper mapper,
            ILogger<CategoryRepository> logger
            )
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
            _logger = logger;
            
        }

        public Task<bool> RateRecipeAsync(int userId, int recipeId, int rating)
        {
            throw new NotImplementedException();
        }
    }
}
