using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using RecipeManagementWebAPI.Data;
using RecipeManagementWebAPI.Dto.Recipe;
using RecipeManagementWebAPI.Services;

namespace RecipeManagementWebAPI.Repository
{
    public class RecipeRepository:IRecipeService
    {
        private readonly DapperContext _dapperContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryRepository> _logger;
        

        public RecipeRepository(
            DapperContext dapperContext,
            IMapper mapper,
            ILogger<CategoryRepository> logger
            )
        {
            _dapperContext = dapperContext;
            _mapper = mapper;
            _logger = logger;
            
        }

        public Task<int> CreateRecipeAsync(CreateRecipeDto createRecipeDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRecipeAsync(int userId, int recipeId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecipeDto>> GetAllRecipesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecipeDto>> GetRecipesByCategoriesAsync(IEnumerable<string> categories)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecipeDto>> GetRecipesByCreatorNameAsync(string creatorName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecipeDto>> GetRecipesByTitleAsync(string title)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecipeDto>> SearchRecipesByPartialAsync(string partialTitle)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRecipeAsync(int userId, UpdateRecipeDto updateRecipeDto)
        {
            throw new NotImplementedException();
        }
    }
}
