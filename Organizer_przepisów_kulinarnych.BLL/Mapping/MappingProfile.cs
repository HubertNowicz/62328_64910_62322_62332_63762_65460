using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using AutoMapper;
using Organizer_przepisów_kulinarnych.DAL.Entities.Enums;

namespace Organizer_przepisów_kulinarnych.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Recipe, RecipeDto>()
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.UserSurname, opt => opt.MapFrom(src => src.User.Surname))
                .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps));

            CreateMap<RecipeIngredient, RecipeIngredientDto>()
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                 .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<RecipeCreateDto, Recipe>()
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps));

            CreateMap<UserRegistrationDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserRole, opt => opt.Ignore());
         

            CreateMap<RecipeInstructionStep, RecipeInstructionStepDto>().ReverseMap();

            CreateMap<MeasurementUnit, MeasurementUnitDto>().ReverseMap();


        }
    }
}
