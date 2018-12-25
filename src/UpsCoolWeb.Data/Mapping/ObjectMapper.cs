using AutoMapper;
using UpsCoolWeb.Objects;
using System.Collections.Generic;

namespace UpsCoolWeb.Data.Mapping
{
    public class ObjectMapper
    {
        public static void MapObjects()
        {
            Mapper.Reset();
            Mapper.Initialize(configuration => new ObjectMapper(configuration).Map());
        }

        private IMapperConfigurationExpression Configuration { get; }

        private ObjectMapper(IMapperConfigurationExpression configuration)
        {
            Configuration = configuration;
            configuration.ValidateInlineMaps = false;
            Configuration.AddConditionalObjectMapper().Conventions.Add(pair => pair.SourceType.Namespace != "Castle.Proxies");
        }

        private void Map()
        {
            MapRoles();
        }

        #region Administration

        private void MapRoles()
        {
            Configuration.CreateMap<Role, RoleView>()
                .ForMember(role => role.Permissions, member => member.Ignore());
            Configuration.CreateMap<RoleView, Role>()
                .ForMember(role => role.Permissions, member => member.MapFrom(role => new List<RolePermission>()));
        }

        #endregion
    }
}
