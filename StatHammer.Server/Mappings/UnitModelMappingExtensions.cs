using StatHammer.Server.Models.DTOs.UnitModels;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Mappings
{
    public static class UnitModelMappingExtensions
    {
        public static UnitModelLinkReadDto ToReadDto(this UnitModel unitModel)
        {
            return new UnitModelLinkReadDto
            {
                Id = unitModel.Id,
                UnitId = unitModel.UnitId,
                UnitName = unitModel.Unit?.Name ?? string.Empty,
                ModelId = unitModel.ModelId,
                ModelName = unitModel.Model?.Name ?? string.Empty,
                MinCount = unitModel.MinCount,
                MaxCount = unitModel.MaxCount
            };
        }

        public static UnitModel ToEntity(this UnitModelLinkCreateDto dto)
        {
            return new UnitModel
            {
                UnitId = dto.UnitId,
                ModelId = dto.ModelId,
                MinCount = dto.MinCount,
                MaxCount = dto.MaxCount
            };
        }
    }
}
