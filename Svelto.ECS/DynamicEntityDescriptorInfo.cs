using System;

namespace Svelto.ECS
{
    public struct DynamicEntityDescriptorInfo<TType>:IEntityDescriptor where TType : IEntityDescriptor, new()
    {
        public DynamicEntityDescriptorInfo(IEntityBuilder[] extraEntities)
        {
            DBC.ECS.Check.Require(extraEntities.Length > 0,
                                  "don't use a DynamicEntityDescriptorInfo if you don't need to use extra EntityViews");

            var defaultEntities = EntityDescriptorTemplate<TType>.descriptor.entitiesToBuild;
            var length = defaultEntities.Length;

            entitiesToBuild = new IEntityBuilder[length + extraEntities.Length + 1];

            Array.Copy(defaultEntities, 0, entitiesToBuild, 0, length);
            Array.Copy(extraEntities, 0, entitiesToBuild, length, extraEntities.Length);

            var _builder = new EntityBuilder<EntityInfoView>
            {
                _initializer = new EntityInfoView { entitiesToBuild = entitiesToBuild }
            };
            entitiesToBuild[entitiesToBuild.Length - 1] = _builder;
        }

        public IEntityBuilder[] entitiesToBuild { get; }
    }

    public class DynamicEntityDescriptorInfoRef<TType>:IEntityDescriptor where TType : IEntityDescriptor, new()
    {
        public DynamicEntityDescriptorInfoRef(IEntityBuilder[] extraEntities)
        {
            _dynamicDescriptor = new  DynamicEntityDescriptorInfo<TType>(extraEntities);
        }

        public IEntityBuilder[] entitiesToBuild { get { return _dynamicDescriptor.entitiesToBuild; } }

        DynamicEntityDescriptorInfo<TType> _dynamicDescriptor;
    }
    
    public struct EntityInfoView : IEntityStruct
    {
        public EGID ID { get; set; }
        
        public IEntityBuilder[] entitiesToBuild;
    }
}