using Claims.Features.Claims.Models;
using Riok.Mapperly.Abstractions;

namespace Claims.Features.Claims.Mappers;

[Mapper]
public static partial class ClaimMapper
{
    [MapperIgnoreTarget(nameof(ClaimEntity.Id))]
    [MapperIgnoreTarget(nameof(ClaimEntity.Created))]
    public static partial ClaimEntity ToEntity(this CreateClaimDto dto);


    [MapperIgnoreSource(nameof(ClaimEntity.Created))]
    public static partial ClaimDto ToDto(this ClaimEntity dto);
}