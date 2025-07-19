using Claims.Features.Covers.Models;
using Riok.Mapperly.Abstractions;

namespace Claims.Features.Covers.Mappers;

[Mapper]
public static partial class CoverMapper
{
    [MapperIgnoreTarget(nameof(CoverEntity.Id))]
    public static partial CoverEntity ToEntity(this CreateCoverDto dto);

    public static partial CoverEntity ToEntity(this CoverDto dto);
    public static partial CoverDto ToDto(this CoverEntity entity);
}