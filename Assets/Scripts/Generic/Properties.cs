using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Properties {
	public float damage = 0;

	private float _hp = 0;
	public float hp{
		get { return _hp; }
		set { 
			_hp = (value < maxHp) ? value : maxHp;
		}
	}
	public float maxHp = 0;
	public float armor = 0;
	public float range = 0;
	public float rateOfFire = 0;
	public bool flying = false;
	public bool stun = false;

	public float coins = 0;
	
	public virtual void Load(string type, int level){}

	public virtual void Add(Properties addedProperties){
		damage += addedProperties.damage;
		maxHp += addedProperties.maxHp;
		hp += addedProperties.hp;
		armor += addedProperties.armor;
		range += addedProperties.range;
		rateOfFire += addedProperties.rateOfFire;
		flying |= addedProperties.flying;
		coins += addedProperties.coins;
		stun |= addedProperties.stun;
	}
	
	public virtual void Remove(Properties removedProperties){
		damage -= removedProperties.damage;
		maxHp -= removedProperties.maxHp;
		hp -= removedProperties.hp;
		armor -= removedProperties.armor;
		range -= removedProperties.range;
		rateOfFire -= removedProperties.rateOfFire;
		flying |= removedProperties.flying;
		coins -= removedProperties.coins;
	}
}

[System.Serializable]
public class PropertiesQueue<T> where T : Properties{
	/*
	 * A properties event with the time it's sceduled
	 */
	[System.Serializable]
	private class PropertiesEvent : Properties{
		public T value;
		public float time;

		public PropertiesEvent(T e, float t)
		{
			value = e;
			time = t;
		}
	}

	public T active;
	List<PropertiesEvent> queue;
	protected T baseProperties;
	float currentTime = 0;

	/// <summary>
	/// Initialize a new queue, setting the base properties and syncing the time.
	/// </summary>
	/// <param name="initial">Initial.</param>
	/// <param name="time">Time.</param>
	public void Initialize(T initial, float time)
	{
		// Initialize queue
		queue = new List<PropertiesEvent>(); 

		// Set base and active properties
		baseProperties = initial;
		active = (T) Activator.CreateInstance(typeof(T), new object[] {});
		active.Add (baseProperties);

		// Syncrhonize time
		SyncTime (time);
	}
	
	/// <summary>
	/// Reset the active properties by recalculating base and queue values
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="level">Level.</param>
	public void Reset(string type, int level)
	{
		baseProperties.Load (type, level);
		active = baseProperties;
		for (int i = 0; i < queue.Count; i++) {
			active.Add(queue[i].value);
		}
	}
	/// <summary>
	/// Syncs the current time to a given time
	/// </summary>
	/// <param name="time">Time.</param>
	public void SyncTime(float time)
	{
		currentTime = time;
	}

	/// <summary>
	/// Update the current time given the latest deltaTime.
	/// </summary>
	/// <param name="deltaTime">Delta time.</param>
	public void Update(float deltaTime)
	{
		currentTime += deltaTime;
		RemoveProperties (currentTime);
	}

	/// <summary>
	/// Calculates the active properties.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="addProperties">If set to <c>true</c> <param name="value"> is added properties,
	/// if set to <c>false</c>, it's removed properties.</param>
	void CalculateActiveProperties (T value, bool addProperties)
	{
		if (addProperties) {
			active.Add(value);
		}
		else {
			active.Remove(value);

			active.flying = baseProperties.flying;
			active.stun = baseProperties.stun;
			for (int i = 0 ; i < queue.Count ; i++)
			{
				active.flying |= queue[i].flying;
				active.stun |= queue[i].stun;
			}
		}
	}

	/// <summary>
	/// Adds a new properties event scheduled at a given time
	/// </summary>
	/// <param name="properties">Properties.</param>
	/// <param name="time">Time.</param>
	public void AddProperties(T properties, float duration)
	{
		PropertiesEvent newEvent = new PropertiesEvent (properties, currentTime + duration);
		int eventOrder = 0;
		for (eventOrder = 0; eventOrder < queue.Count; eventOrder++) {
			if (newEvent.time < queue[eventOrder].time)
				break;
		}
		queue.Insert (eventOrder, newEvent);
		CalculateActiveProperties (newEvent.value, true);
	}

	/// <summary>
	/// Removes properties from the queue based on the given time
	/// </summary>
	/// <param name="time">Time.</param>
	public void RemoveProperties(float time)
	{
		if (queue.Count > 0) {
			for (int i = 0 ; i < queue.Count; )
			{
				if (queue [i].time <= time) {
					RemoveProperties (i);
				}
				else
				{
					break;
				}
			}
		}
	}

	/// <summary>
	/// Removes the properties from the queue at the given index
	/// </summary>
	/// <param name="index">Index.</param>
	private void RemoveProperties(int index)
	{
		if (queue.Count > 0 && index < queue.Count)
		{
			PropertiesEvent dequeuedEvent = queue[index];
			queue.RemoveAt(index);
			CalculateActiveProperties(dequeuedEvent.value, false);
		}
	}
}
