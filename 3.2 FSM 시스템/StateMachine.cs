using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
	private T sender;

	public IState<T> curState { get; set; }

	public StateMachine(T sender, IState<T> state)
	{
		this.sender = sender;
		curState = state;
	}

	public void SetState(IState<T> state)
	{
		if(this.sender == null)
		{
			return;
		}

		if(curState == state)
		{
			return;
		}

		if(curState != null)
		{
			curState.OperateExit(this.sender);
		}

		curState = state;

		if(curState != null)
		{
			curState.OperateEnter(this.sender);
		}
	}

	public void DoOperateUpdate()
	{
		if(this.sender == null)
		{
			return;
		}

		curState.OperateUpdate(this.sender);
	}	
}
