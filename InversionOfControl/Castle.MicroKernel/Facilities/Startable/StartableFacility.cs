// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.Startable
{
	using System;
	using System.Collections;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;
	using Castle.Core;

	public class StartableFacility : AbstractFacility
	{
		private ArrayList waitList = new ArrayList();
		private ITypeConverter converter;

		// Records whether we are in the start method.
		private bool inStart;

		protected override void Init()
		{
			converter = (ITypeConverter) Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);

			Kernel.ComponentModelCreated +=
				new ComponentModelDelegate(OnComponentModelCreated);
			Kernel.ComponentRegistered +=
				new ComponentDataDelegate(OnComponentRegistered);
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			bool startable =
				CheckIfComponentImplementsIStartable(model) || HasStartableAttributeSet(model);

			model.ExtendedProperties["startable"] = startable;

			if (startable)
			{
				model.LifecycleSteps.Add(
					LifecycleStepType.Commission, StartConcern.Instance);
				model.LifecycleSteps.AddFirst(
					LifecycleStepType.Decommission, StopConcern.Instance);
			}
		}

		private void OnComponentRegistered(String key, IHandler handler)
		{
		    bool startable = (bool?) handler.ComponentModel.ExtendedProperties["startable"] ?? false;

			if (startable)
			{
				if (handler.CurrentState == HandlerState.WaitingDependency)
				{
					AddHandlerToWaitingList(handler);
				}
				else
				{
					Start(key);
				}
			}

			CheckWaitingList();
		}

		private void OnHandlerStateChanged(object source, EventArgs args)
		{
			CheckWaitingList();
		}

		private void AddHandlerToWaitingList(IHandler handler)
		{
			waitList.Add(handler);

			handler.OnHandlerStateChanged += new HandlerStateDelegate(OnHandlerStateChanged);
		}

		/// <summary>
		/// For each new component registered,
		/// some components in the WaitingDependency
		/// state may have became valid, so we check them
		/// </summary>
		private void CheckWaitingList()
		{
			// Don't check the waiting list while this flag is set as this could result in
			// duplicate singletons.
			if (!inStart)
			{
				IHandler[] handlers = (IHandler[])waitList.ToArray(typeof(IHandler));

				IList validList = new ArrayList();

				foreach (IHandler handler in handlers)
				{
					if (handler.CurrentState == HandlerState.Valid)
					{
						validList.Add(handler);
						waitList.Remove(handler);

						handler.OnHandlerStateChanged -= new HandlerStateDelegate(OnHandlerStateChanged);
					}
				}

				foreach (IHandler handler in validList)
				{
					Start(handler.ComponentModel.Name);
				}
			}
		}

		/// <summary>
		/// Request the component instance
		/// </summary>
		/// <param name="key"></param>
		private void Start(String key)
		{
			try
			{
				// Set the flag so that if we get called again as a result of a kernel event
				// we don't check the waiting list and call this function recursively because
				// this could result in duplicate singletons or dependency cycles.
				inStart = true;

				object instance = Kernel[key];
			}
			finally
			{
				inStart = false;
			}
		}

		private bool CheckIfComponentImplementsIStartable(ComponentModel model)
		{
			return typeof(IStartable).IsAssignableFrom(model.Implementation);
		}

		private bool HasStartableAttributeSet(ComponentModel model)
		{
			bool result = false;

			if (model.Configuration != null)
			{
				String startable = model.Configuration.Attributes["startable"];

				if (startable != null)
				{
					result = (bool) converter.PerformConversion(startable, typeof(bool));
				}
			}

			return result;
		}
	}
}
