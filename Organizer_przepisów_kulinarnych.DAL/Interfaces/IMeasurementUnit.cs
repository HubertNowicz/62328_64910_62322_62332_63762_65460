namespace Organizer_przepisów_kulinarnych.DAL.Interfaces
{
    public interface IMeasurementUnitRepository
    {
        Task<List<MeasurementUnit>> GetAllAsync();
    }
}
