using AutoMapper;

namespace EngramaCore
{
	public class MapperHelper
	{
		public IMapper Mapper { get; set; }

		public MapperHelper(IMapper iMapper)
		{
			Mapper = iMapper;
		}

		public K Get<T, K>(T modelo)
		{
			return new MapperConfiguration(x => x.CreateMap<T, K>().ReverseMap()).CreateMapper().Map<T, K>(modelo);
		}

		public IList<K> Get<T, K>(IList<T> modelo)
		{
			return new MapperConfiguration(x => x.CreateMap<T, K>().ReverseMap()).CreateMapper().Map<IList<T>, IList<K>>(modelo);
		}

		public K Map<T, K>(T emisor, K receptor)
		{
			return new MapperConfiguration(x => x.CreateMap<T, K>().ReverseMap()).CreateMapper().Map(emisor, receptor);
		}

		public IList<K> Map<T, K>(IList<T> emisor, IList<K> receptor)
		{
			return new MapperConfiguration(x => x.CreateMap<T, K>().ReverseMap()).CreateMapper().Map(emisor, receptor);
		}
	}
}
