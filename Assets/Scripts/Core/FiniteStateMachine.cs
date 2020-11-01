using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPSDemo.Core
{
	public class FiniteStateMachine : MonoBehaviour
	{
		int[,] stateMatrix;
		public Action<int> onStateChanged;

		public FiniteStateMachine(int statesCount , int eventsCount)
		{
			stateMatrix = new int[statesCount , eventsCount];

			for (int i = 0; i < statesCount; i++)
			{
				for (int j = 0; j < eventsCount; j++)
				{
					stateMatrix[i , j] = -1;
				}
			}
		}
		
		public void RegisterTransition(int sourceState , int eventNumber , int newState)
		{
			stateMatrix[sourceState , eventNumber] = newState;
		}

		public int CheckTransition(int state , int eventNumber)
		{
			int result = stateMatrix[state , eventNumber];

			if (result != -1) 
			{
				if (onStateChanged != null) onStateChanged.Invoke(result);
				return result;
			}

			return -1;
		}
	}
}