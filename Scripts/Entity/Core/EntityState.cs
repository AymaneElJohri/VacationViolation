using Godot;
using System;

public abstract class EntityState
{
	protected Entity Entity;
	public string AnimationName;
	
	public EntityState(Entity entity)
	{
		Entity = entity;
	}
	
	public virtual void Enter() {}
	public virtual void Exit() {}
	public virtual void Update(double delta) {}
	public virtual void PhysicsUpdate(double delta) {}
	public virtual string GetAnimationName() => GetType().Name;
}
