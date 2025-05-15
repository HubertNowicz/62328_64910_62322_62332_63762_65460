using AutoMapper;
using Organizer_przepisów_kulinarnych.BLL.DataTransferObjects;
using Organizer_przepisów_kulinarnych.Models;

namespace Organizer_przepisów_kulinarnych.Mapping
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<RecipeDto, RecipeViewModel>()
                .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.RecipeId))
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps));

            CreateMap<RecipeCreateViewModel, RecipeCreateDto>()
                .ForMember(dest => dest.RecipeIngredients, opt => opt.MapFrom(src => src.Ingredients))
                .ForMember(dest => dest.InstructionSteps, opt => opt.MapFrom(src => src.InstructionSteps));

            CreateMap<RecipeIngredientDto, RecipeIngredientViewModel>()
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.IngredientName))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ReverseMap();

            CreateMap<RecipeInstructionStepDto, RecipeInstructionStepViewModel>().ReverseMap();

            CreateMap<MeasurementUnit, MeasurementUnitViewModel>().ReverseMap();
        }
    }
}
