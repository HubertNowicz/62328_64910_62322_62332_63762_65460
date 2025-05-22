using AutoMapper;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.DAL.Entities;
using Organizer_przepisów_kulinarnych.Models;
using Organizer_przepisów_kulinarnych.Models.Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;

namespace Organizer_przepisów_kulinarnych.Mapping
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<RecipeDto, RecipeViewModel>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps));

            CreateMap<RecipeCreateViewModel, RecipeCreateDto>()
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.Ingredients))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps));

            CreateMap<RecipeCreateViewModel, RecipeDto>()
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.Ingredients))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps))
                .ReverseMap();


            CreateMap<RecipeIngredientDto, RecipeIngredientViewModel>()
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ReverseMap();

            CreateMap<RecipeInstructionStepDto, RecipeInstructionStepViewModel>()
                .ReverseMap();

            CreateMap<IngredientDto, IngredientViewModel>()
                .ForMember(dest => dest.Units, opt => opt.MapFrom(src => src.Units))
                .ReverseMap();

            CreateMap<IngredientUnit, MeasurementUnitDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Unit.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Unit.Name));

            CreateMap<MeasurementUnitDto, MeasurementUnitViewModel>()
                .ReverseMap();

            CreateMap<UserDto, UserViewModel>()
                .ReverseMap();

            CreateMap<CategoryDto, CategoryViewModel>();

            CreateMap<PendingIngredientDto, PendingIngredientViewModel>();

            CreateMap<UserRegisterViewModel, UserRegistrationDto>();
        }
    }
}
