
namespace Framework
{
	public class BehaviourStateMachine
	{
	}

	public abstract class BehaviourState<T> where T : Entity
	{
		public void OnEnter() { }
		public void OnUpdate() { }
		public void OnLeave() { }
	}
}
