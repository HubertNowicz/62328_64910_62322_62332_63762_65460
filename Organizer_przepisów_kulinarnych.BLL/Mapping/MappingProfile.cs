using AutoMapper;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Recipe, RecipeDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.FavoriteRecipes.Count));

            CreateMap<RecipeCreateDto, Recipe>()
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps));

            CreateMap<RecipeInstructionStep, RecipeInstructionStepDto>()
                .ReverseMap();

            CreateMap<RecipeIngredient, RecipeIngredientDto>()
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ReverseMap();

            CreateMap<Ingredient, IngredientDto>()
                .ForMember(dest => dest.Units, opt => opt.MapFrom(src => src.IngredientUnits))
                .ReverseMap();

            CreateMap<PendingIngredient, PendingIngredientDto>();

            CreateMap<MeasurementUnit, MeasurementUnitDto>()
                .ReverseMap();

            CreateMap<User, UserDto>();

            CreateMap<UserRegistrationDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserRole, opt => opt.Ignore());

            CreateMap<Category, CategoryDto>();
        }
    }
}
